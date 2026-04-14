# Script de Actualización de Paquetes NuGet Vulnerables
# RubricasApp.Web - Sistema de Rúbricas MEP
# Fecha: 28 de enero de 2025

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  ACTUALIZACIÓN DE PAQUETES NUGET" -ForegroundColor Cyan
Write-Host "  RubricasApp.Web - Sistema MEP" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Configuración
$ErrorActionPreference = "Stop"
$projectPath = "RubricasApp.Web.csproj"

# Verificar que estamos en el directorio correcto
if (-not (Test-Path $projectPath)) {
    Write-Host "[ERROR] No se encontró el archivo $projectPath" -ForegroundColor Red
    Write-Host "[ERROR] Ejecute este script desde el directorio raíz del proyecto" -ForegroundColor Red
    exit 1
}

Write-Host "[INFO] Proyecto encontrado: $projectPath" -ForegroundColor Green
Write-Host ""

# Función para actualizar paquete con retry
function Update-Package {
    param(
        [string]$PackageName,
        [string]$Version,
        [string]$Priority
    )
    
    Write-Host "[$Priority] Actualizando $PackageName a versión $Version..." -ForegroundColor Yellow
    
    try {
        $output = dotnet add package $PackageName --version $Version 2>&1
        
        if ($LASTEXITCODE -eq 0) {
            Write-Host "[SUCCESS] $PackageName actualizado correctamente" -ForegroundColor Green
            return $true
        } else {
            Write-Host "[ERROR] Falló la actualización de $PackageName" -ForegroundColor Red
            Write-Host $output -ForegroundColor Red
            return $false
        }
    }
    catch {
        Write-Host "[ERROR] Excepción al actualizar $PackageName : $_" -ForegroundColor Red
        return $false
    }
}

# Contador de actualizaciones
$totalPackages = 0
$successCount = 0
$failCount = 0

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "FASE 1: VULNERABILIDADES CRÍTICAS" -ForegroundColor Red
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

$packages = @(
    @{ Name = "NuGet.Common"; Version = "8.0.1"; Priority = "CRÍTICO" }
)

foreach ($pkg in $packages) {
    $totalPackages++
    if (Update-Package -PackageName $pkg.Name -Version $pkg.Version -Priority $pkg.Priority) {
        $successCount++
    } else {
        $failCount++
    }
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "FASE 2: VULNERABILIDADES ALTAS" -ForegroundColor DarkYellow
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

$packages = @(
    @{ Name = "Microsoft.EntityFrameworkCore"; Version = "8.0.13"; Priority = "ALTO" },
    @{ Name = "Microsoft.EntityFrameworkCore.SqlServer"; Version = "8.0.13"; Priority = "ALTO" },
    @{ Name = "Microsoft.EntityFrameworkCore.Design"; Version = "8.0.13"; Priority = "ALTO" },
    @{ Name = "Microsoft.EntityFrameworkCore.Tools"; Version = "8.0.13"; Priority = "ALTO" },
    @{ Name = "Microsoft.EntityFrameworkCore.Sqlite"; Version = "8.0.13"; Priority = "ALTO" },
    @{ Name = "Microsoft.AspNetCore.Identity.EntityFrameworkCore"; Version = "8.0.13"; Priority = "ALTO" },
    @{ Name = "Microsoft.AspNetCore.Identity.UI"; Version = "8.0.13"; Priority = "ALTO" },
    @{ Name = "Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation"; Version = "8.0.13"; Priority = "ALTO" },
    @{ Name = "Azure.Identity"; Version = "1.14.1"; Priority = "ALTO" },
    @{ Name = "Rotativa.AspNetCore"; Version = "1.4.0"; Priority = "ALTO" }
)

foreach ($pkg in $packages) {
    $totalPackages++
    if (Update-Package -PackageName $pkg.Name -Version $pkg.Version -Priority $pkg.Priority) {
        $successCount++
    } else {
        $failCount++
    }
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "FASE 3: CORRECCIÓN DE CONFLICTOS" -ForegroundColor Yellow
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

$packages = @(
    @{ Name = "AutoMapper"; Version = "13.0.1"; Priority = "CONFLICTO" },
    @{ Name = "AutoMapper.Extensions.Microsoft.DependencyInjection"; Version = "13.0.1"; Priority = "CONFLICTO" }
)

foreach ($pkg in $packages) {
    $totalPackages++
    if (Update-Package -PackageName $pkg.Name -Version $pkg.Version -Priority $pkg.Priority) {
        $successCount++
    } else {
        $failCount++
    }
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "FASE 4: ACTUALIZACIONES OPCIONALES" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

$packages = @(
    @{ Name = "Bootstrap"; Version = "5.3.8"; Priority = "OPCIONAL" },
    @{ Name = "jQuery"; Version = "3.7.1"; Priority = "OPCIONAL" },
    @{ Name = "PdfSharp"; Version = "6.2.4"; Priority = "OPCIONAL" },
    @{ Name = "QuestPDF"; Version = "2026.2.4"; Priority = "OPCIONAL" },
    @{ Name = "Microsoft.VisualStudio.Web.CodeGeneration.Design"; Version = "8.0.10"; Priority = "OPCIONAL" }
)

foreach ($pkg in $packages) {
    $totalPackages++
    if (Update-Package -PackageName $pkg.Name -Version $pkg.Version -Priority $pkg.Priority) {
        $successCount++
    } else {
        $failCount++
    }
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "RESTAURANDO Y COMPILANDO" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

Write-Host "[INFO] Ejecutando dotnet restore..." -ForegroundColor Yellow
dotnet restore

if ($LASTEXITCODE -eq 0) {
    Write-Host "[SUCCESS] Restore completado" -ForegroundColor Green
} else {
    Write-Host "[ERROR] Falló el restore" -ForegroundColor Red
}

Write-Host ""
Write-Host "[INFO] Ejecutando dotnet build..." -ForegroundColor Yellow
dotnet build --no-restore

if ($LASTEXITCODE -eq 0) {
    Write-Host "[SUCCESS] Compilación exitosa" -ForegroundColor Green
} else {
    Write-Host "[ERROR] Falló la compilación" -ForegroundColor Red
    Write-Host "[WARNING] Revise los errores de compilación antes de continuar" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "VERIFICACIÓN DE VULNERABILIDADES" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

Write-Host "[INFO] Verificando vulnerabilidades restantes..." -ForegroundColor Yellow
dotnet list package --vulnerable --include-transitive

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "RESUMEN DE ACTUALIZACIONES" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Total de paquetes procesados: $totalPackages" -ForegroundColor White
Write-Host "Actualizados exitosamente:    $successCount" -ForegroundColor Green
Write-Host "Fallos:                       $failCount" -ForegroundColor Red
Write-Host ""

if ($failCount -eq 0) {
    Write-Host "[SUCCESS] Todas las actualizaciones completadas exitosamente" -ForegroundColor Green
    Write-Host ""
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host "PRÓXIMOS PASOS:" -ForegroundColor Cyan
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host "1. Ejecutar pruebas funcionales críticas" -ForegroundColor Yellow
    Write-Host "2. Verificar que no hay vulnerabilidades:" -ForegroundColor Yellow
    Write-Host "   dotnet list package --vulnerable" -ForegroundColor Gray
    Write-Host "3. Hacer commit de los cambios:" -ForegroundColor Yellow
    Write-Host "   git add RubricasApp.Web.csproj" -ForegroundColor Gray
    Write-Host "   git commit -m 'fix: Actualizar paquetes NuGet vulnerables'" -ForegroundColor Gray
    Write-Host "4. Push al repositorio:" -ForegroundColor Yellow
    Write-Host "   git push origin feature-sea-mep" -ForegroundColor Gray
} else {
    Write-Host "[WARNING] Algunas actualizaciones fallaron" -ForegroundColor Yellow
    Write-Host "[WARNING] Revise los errores arriba antes de continuar" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Script completado" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
