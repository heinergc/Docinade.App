#!/usr/bin/env pwsh
# Script para verificar compilación del proyecto

Write-Host "Iniciando verificación de compilación..." -ForegroundColor Green

try {
    # Navegar al directorio del proyecto
    Set-Location -Path $PSScriptRoot
    
    Write-Host "Directorio actual: $(Get-Location)" -ForegroundColor Yellow
    
    # Limpiar proyecto
    Write-Host "Limpiando proyecto..." -ForegroundColor Cyan
    dotnet clean --verbosity quiet
    
    # Restaurar paquetes
    Write-Host "Restaurando paquetes NuGet..." -ForegroundColor Cyan
    dotnet restore --verbosity quiet
    
    # Compilar proyecto
    Write-Host "Compilando proyecto..." -ForegroundColor Cyan
    $buildOutput = dotnet build --no-restore --verbosity normal 2>&1
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✅ Compilación exitosa!" -ForegroundColor Green
        Write-Host "Proyecto compilado sin errores." -ForegroundColor Green
    } else {
        Write-Host "❌ Error en la compilación:" -ForegroundColor Red
        Write-Host $buildOutput -ForegroundColor Red
    }
    
    return $LASTEXITCODE
}
catch {
    Write-Host "❌ Error inesperado durante la compilación:" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Red
    return 1
}