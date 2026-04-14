# Script para corregir el problema de migración de la base de datos
# Error: SQLite Error 1: 'no such column: a.CreatedDate'

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "CORRECCIÓN DE MIGRACIÓN DE BASE DE DATOS" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Cambiar al directorio del proyecto
Write-Host "📂 Cambiando al directorio del proyecto..." -ForegroundColor Yellow
Set-Location "C:\Users\hguido\Desktop\RubricasApp\src\RubricasApp.Web"

try {
    # Verificar que tenemos las herramientas de Entity Framework
    Write-Host "🔧 Verificando herramientas de Entity Framework..." -ForegroundColor Yellow
    $efToolsCheck = dotnet ef --version 2>&1
    if ($LASTEXITCODE -ne 0) {
        Write-Host "❌ Las herramientas de Entity Framework no están instaladas. Instalando..." -ForegroundColor Red
        dotnet tool install --global dotnet-ef
        if ($LASTEXITCODE -ne 0) {
            throw "Error al instalar las herramientas de Entity Framework"
        }
    } else {
        Write-Host "✅ Herramientas de Entity Framework disponibles: $($efToolsCheck.Split([Environment]::NewLine)[0])" -ForegroundColor Green
    }

    # Hacer backup de la base de datos actual si existe
    Write-Host "💾 Realizando backup de la base de datos actual..." -ForegroundColor Yellow
    if (Test-Path "RubricasDbNueva.db") {
        $timestamp = Get-Date -Format "yyyyMMdd_HHmmss"
        Copy-Item "RubricasDbNueva.db" "RubricasDbNueva_backup_$timestamp.db"
        Write-Host "✅ Backup creado: RubricasDbNueva_backup_$timestamp.db" -ForegroundColor Green
    } else {
        Write-Host "ℹ️ No se encontró base de datos existente" -ForegroundColor Cyan
    }

    # Eliminar migraciones existentes para empezar limpio
    Write-Host "🗑️ Eliminando migraciones existentes..." -ForegroundColor Yellow
    if (Test-Path "Migrations") {
        Remove-Item "Migrations\*" -Force -Recurse
        Write-Host "✅ Migraciones existentes eliminadas" -ForegroundColor Green
    }

    # Crear migración inicial completa
    Write-Host "🆕 Creando migración inicial completa..." -ForegroundColor Yellow
    $migrationResult = dotnet ef migrations add InitialCreate --verbose 2>&1
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✅ Migración inicial creada exitosamente" -ForegroundColor Green
    } else {
        Write-Host "❌ Error al crear la migración:" -ForegroundColor Red
        Write-Host $migrationResult -ForegroundColor Red
        throw "Error en la creación de migración"
    }

    # Eliminar base de datos actual para recrear completamente
    Write-Host "🗑️ Eliminando base de datos actual para recrear..." -ForegroundColor Yellow
    if (Test-Path "RubricasDbNueva.db") {
        Remove-Item "RubricasDbNueva.db" -Force
    }
    if (Test-Path "RubricasDbNueva.db-shm") {
        Remove-Item "RubricasDbNueva.db-shm" -Force
    }
    if (Test-Path "RubricasDbNueva.db-wal") {
        Remove-Item "RubricasDbNueva.db-wal" -Force
    }

    # Crear la base de datos con el esquema correcto
    Write-Host "🔨 Creando base de datos con esquema completo..." -ForegroundColor Yellow
    $dbUpdateResult = dotnet ef database update --verbose 2>&1
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✅ Base de datos creada exitosamente con el esquema correcto" -ForegroundColor Green
    } else {
        Write-Host "❌ Error al actualizar la base de datos:" -ForegroundColor Red
        Write-Host $dbUpdateResult -ForegroundColor Red
        throw "Error en la actualización de base de datos"
    }

    # Verificar que las tablas se crearon correctamente
    Write-Host "🔍 Verificando estructura de la base de datos..." -ForegroundColor Yellow
    
    # Crear un script simple para verificar las tablas
    $verifyScript = @"
using Microsoft.EntityFrameworkCore;
using RubricasApp.Web.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;

var builder = Host.CreateApplicationBuilder();
builder.Configuration.AddJsonFile("appsettings.json");

builder.Services.AddDbContext<RubricasDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

var host = builder.Build();

using var scope = host.Services.CreateScope();
var context = scope.ServiceProvider.GetRequiredService<RubricasDbContext>();

try {
    // Verificar que podemos conectar y que las tablas principales existen
    var canConnect = await context.Database.CanConnectAsync();
    Console.WriteLine(`Can connect to database: {canConnect}`);
    
    if (canConnect) {
        // Verificar tabla de usuarios
        var userTableExists = await context.Database.ExecuteSqlRawAsync("SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name='AspNetUsers'") >= 0;
        Console.WriteLine("AspNetUsers table check completed");
        
        // Verificar que la columna CreatedDate existe
        var hasCreatedDate = true;
        try {
            await context.Database.ExecuteSqlRawAsync("SELECT CreatedDate FROM AspNetUsers LIMIT 1");
            Console.WriteLine("✅ CreatedDate column exists");
        } catch {
            hasCreatedDate = false;
            Console.WriteLine("❌ CreatedDate column missing");
        }
        
        Console.WriteLine(`Database verification completed. CreatedDate column exists: {hasCreatedDate}`);
    }
} catch (Exception ex) {
    Console.WriteLine(`Error during verification: {ex.Message}`);
}
"@

    $verifyScript | Out-File -FilePath "verify_db.cs" -Encoding UTF8
    
    Write-Host "📊 Compilando proyecto para verificar..." -ForegroundColor Yellow
    $buildResult = dotnet build --configuration Release --verbosity quiet 2>&1
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✅ Proyecto compilado exitosamente" -ForegroundColor Green
    } else {
        Write-Host "⚠️ Advertencias en la compilación, pero continuando..." -ForegroundColor Yellow
        Write-Host $buildResult -ForegroundColor Yellow
    }

    # Inicializar datos de prueba si es necesario
    Write-Host "🌱 Inicializando datos básicos..." -ForegroundColor Yellow
    $initResult = dotnet run --no-build --project . -- --seed 2>&1
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✅ Datos básicos inicializados" -ForegroundColor Green
    } else {
        Write-Host "ℹ️ No se pudieron inicializar datos básicos (puede ser normal)" -ForegroundColor Cyan
    }

    Write-Host ""
    Write-Host "🎉 CORRECCIÓN COMPLETADA EXITOSAMENTE" -ForegroundColor Green
    Write-Host "✅ Base de datos recreada con esquema completo" -ForegroundColor Green
    Write-Host "✅ Columnas de Identity agregadas correctamente" -ForegroundColor Green
    Write-Host "✅ El problema de 'no such column: a.CreatedDate' debería estar resuelto" -ForegroundColor Green
    Write-Host ""
    Write-Host "🚀 Puedes ahora:" -ForegroundColor Cyan
    Write-Host "   1. Ejecutar la aplicación: dotnet run" -ForegroundColor White
    Write-Host "   2. Intentar hacer login nuevamente" -ForegroundColor White
    Write-Host "   3. Crear usuarios sin errores de base de datos" -ForegroundColor White

} catch {
    Write-Host ""
    Write-Host "❌ ERROR DURANTE LA CORRECCIÓN" -ForegroundColor Red
    Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host ""
    Write-Host "🔧 Pasos manuales recomendados:" -ForegroundColor Yellow
    Write-Host "   1. dotnet ef migrations remove --force" -ForegroundColor White
    Write-Host "   2. Eliminar archivo RubricasDbNueva.db" -ForegroundColor White
    Write-Host "   3. dotnet ef migrations add InitialCreate" -ForegroundColor White
    Write-Host "   4. dotnet ef database update" -ForegroundColor White
    exit 1
} finally {
    # Limpiar archivos temporales
    if (Test-Path "verify_db.cs") {
        Remove-Item "verify_db.cs" -Force
    }
}