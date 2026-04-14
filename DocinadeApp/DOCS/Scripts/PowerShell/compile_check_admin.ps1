# Script para verificar compilación del AdminController
Write-Host "=== Verificando compilación del AdminController ===" -ForegroundColor Green

# Cambiar al directorio del proyecto
Set-Location "C:\Users\hguido\Desktop\RubricasApp\src\RubricasApp.Web"

# Limpiar y compilar
Write-Host "Limpiando proyecto..." -ForegroundColor Yellow
dotnet clean --verbosity quiet

Write-Host "Compilando proyecto..." -ForegroundColor Yellow
$buildResult = dotnet build --verbosity normal --no-restore 2>&1

# Mostrar solo errores relacionados con AdminController
$adminErrors = $buildResult | Where-Object { $_ -match "AdminController" -or $_ -match "error CS" }

if ($adminErrors) {
    Write-Host "=== ERRORES ENCONTRADOS ===" -ForegroundColor Red
    $adminErrors | ForEach-Object { Write-Host $_ -ForegroundColor Red }
} else {
    Write-Host "=== COMPILACIÓN EXITOSA ===" -ForegroundColor Green
    Write-Host "No se encontraron errores en AdminController.cs" -ForegroundColor Green
}

# Buscar warnings específicos
$warnings = $buildResult | Where-Object { $_ -match "warning" -and $_ -match "AdminController" }
if ($warnings) {
    Write-Host "=== WARNINGS ===" -ForegroundColor Yellow
    $warnings | ForEach-Object { Write-Host $_ -ForegroundColor Yellow }
}

Write-Host "=== VERIFICACIÓN COMPLETADA ===" -ForegroundColor Cyan