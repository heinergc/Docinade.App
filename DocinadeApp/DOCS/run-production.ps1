# Script para Ejecutar en Modo Produccion Local
# Ejecutar con: .\run-production.ps1

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Ejecutar en Modo PRODUCCION (Local)" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

Write-Host "[INFO] Configurando entorno..." -ForegroundColor Yellow
$env:ASPNETCORE_ENVIRONMENT = "Production"
$env:DOTNET_ENVIRONMENT = "Production"

Write-Host "[INFO] Ambiente: $env:ASPNETCORE_ENVIRONMENT" -ForegroundColor Green
Write-Host ""

# Verificar si existe appsettings.Production.json
if (Test-Path "appsettings.Production.json") {
    Write-Host "[OK] appsettings.Production.json encontrado" -ForegroundColor Green
} else {
    Write-Host "[WARN] appsettings.Production.json no encontrado" -ForegroundColor Yellow
    Write-Host "       Usando appsettings.json" -ForegroundColor Gray
}

Write-Host ""
Write-Host "[INFO] Compilando aplicación..." -ForegroundColor Yellow
dotnet build --configuration Release

if ($LASTEXITCODE -ne 0) {
    Write-Host "[ERROR] Falló la compilación" -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "[INFO] Iniciando aplicación en modo Production..." -ForegroundColor Yellow
Write-Host ""
Write-Host "========================================" -ForegroundColor Green
Write-Host "  Aplicacion ejecutandose en:" -ForegroundColor Green
Write-Host "  http://localhost:5000" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Green
Write-Host ""
Write-Host "Presione Ctrl+C para detener" -ForegroundColor Yellow
Write-Host ""

dotnet run --configuration Release --urls "http://localhost:5000"
