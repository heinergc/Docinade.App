# Script para agregar columna Estado a la tabla Evaluaciones
# Ejecutar desde: C:\Users\hguido\Desktop\RubricasApp\src\RubricasApp.Web

Write-Host "=== Migración: Agregar columna Estado a Evaluaciones ===" -ForegroundColor Green

# Cambiar al directorio del proyecto
Set-Location "C:\Users\hguido\Desktop\RubricasApp\src\RubricasApp.Web"

Write-Host "Directorio actual: $(Get-Location)" -ForegroundColor Yellow

# Verificar si existe el archivo de base de datos
if (-not (Test-Path "RubricasDbNueva.db")) {
    Write-Host "ERROR: No se encontró el archivo RubricasDbNueva.db" -ForegroundColor Red
    exit 1
}

Write-Host "Base de datos encontrada: RubricasDbNueva.db" -ForegroundColor Green

try {
    # Opción 1: Usar Entity Framework Core Tools
    Write-Host "Intentando usar Entity Framework Core Tools..." -ForegroundColor Yellow
    
    # Crear migración
    Write-Host "Creando migración..." -ForegroundColor Cyan
    dotnet ef migrations add AgregarColumnaEstado --verbose
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "Migración creada exitosamente." -ForegroundColor Green
        
        # Aplicar migración
        Write-Host "Aplicando migración a la base de datos..." -ForegroundColor Cyan
        dotnet ef database update --verbose
        
        if ($LASTEXITCODE -eq 0) {
            Write-Host "Migración aplicada exitosamente!" -ForegroundColor Green
        } else {
            Write-Host "Error al aplicar la migración." -ForegroundColor Red
        }
    } else {
        Write-Host "Error al crear la migración. Intentando método alternativo..." -ForegroundColor Yellow
        
        # Opción 2: Usar SQL directo con System.Data.SQLite
        Write-Host "Usando método de SQL directo..." -ForegroundColor Yellow
        
        # Crear script temporal de C#
        $csharpScript = @"
using System;
using System.Data;
using Microsoft.Data.Sqlite;

var connectionString = "Data Source=RubricasDbNueva.db";
using var connection = new SqliteConnection(connectionString);
connection.Open();

// Verificar si la columna existe
var checkCommand = connection.CreateCommand();
checkCommand.CommandText = "PRAGMA table_info(Evaluaciones)";
var reader = checkCommand.ExecuteReader();
bool estadoExists = false;
while (reader.Read()) {
    if (reader["name"].ToString() == "Estado") {
        estadoExists = true;
        break;
    }
}
reader.Close();

if (estadoExists) {
    Console.WriteLine("La columna Estado ya existe.");
} else {
    // Agregar columna
    var addCommand = connection.CreateCommand();
    addCommand.CommandText = "ALTER TABLE Evaluaciones ADD COLUMN Estado TEXT DEFAULT 'BORRADOR'";
    addCommand.ExecuteNonQuery();
    Console.WriteLine("Columna Estado agregada.");
    
    // Actualizar registros existentes
    var updateCommand = connection.CreateCommand();
    updateCommand.CommandText = "UPDATE Evaluaciones SET Estado = 'COMPLETADA' WHERE Estado IS NULL OR Estado = ''";
    var affected = updateCommand.ExecuteNonQuery();
    Console.WriteLine($"Se actualizaron {affected} registros.");
}

connection.Close();
Console.WriteLine("Migración completada!");
"@
        
        # Guardar y ejecutar script
        $csharpScript | Out-File -FilePath "temp_migration.csx" -Encoding UTF8
        dotnet script temp_migration.csx
        Remove-Item "temp_migration.csx" -ErrorAction SilentlyContinue
    }
    
} catch {
    Write-Host "Error durante la migración: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "Detalles: $($_.Exception.StackTrace)" -ForegroundColor Red
}

Write-Host "`nMigración finalizada. Presione cualquier tecla para continuar..." -ForegroundColor Green
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")