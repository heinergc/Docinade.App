# Script para ejecutar la aplicación de reseteo de contraseña
# Uso: .\reset-admin-password.ps1

Write-Host "===============================================" -ForegroundColor Cyan
Write-Host "  RESETEAR CONTRASEÑA DE ADMINISTRADOR" -ForegroundColor Cyan
Write-Host "  Sistema de Rúbricas MEP" -ForegroundColor Cyan
Write-Host "===============================================" -ForegroundColor Cyan
Write-Host ""

# Verificar que existe el archivo
if (-not (Test-Path "ResetAdminPassword.cs")) {
    Write-Host "[ERROR] No se encontró ResetAdminPassword.cs" -ForegroundColor Red
    exit 1
}

Write-Host "[INFO] Compilando aplicación..." -ForegroundColor Yellow
Write-Host ""

# Compilar y ejecutar usando dotnet-script o csc
$cscPath = "C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe"

if (Test-Path $cscPath) {
    Write-Host "[INFO] Usando C# Compiler..." -ForegroundColor Green
    
    # Compilar
    & $cscPath /t:exe /out:ResetAdminPassword.exe ResetAdminPassword.cs /r:System.dll /r:System.Core.dll
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "[SUCCESS] Compilación exitosa" -ForegroundColor Green
        Write-Host ""
        
        # Ejecutar
        & .\ResetAdminPassword.exe
    } else {
        Write-Host "[ERROR] Error en la compilación" -ForegroundColor Red
    }
} else {
    Write-Host "[INFO] Usando dotnet para ejecutar el código directamente..." -ForegroundColor Yellow
    Write-Host "[INFO] Esto requiere que el código esté en el contexto del proyecto principal" -ForegroundColor Yellow
    Write-Host ""
    
    # Alternativa: Ejecutar como parte del proyecto
    Write-Host "Para ejecutar esta herramienta, use una de estas opciones:" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "OPCIÓN 1: Crear método estático en Program.cs y llamarlo con:" -ForegroundColor White
    Write-Host "  dotnet run -- reset-password" -ForegroundColor Gray
    Write-Host ""
    Write-Host "OPCIÓN 2: Usar el comando interactivo de C# (dotnet-script):" -ForegroundColor White
    Write-Host "  dotnet tool install -g dotnet-script" -ForegroundColor Gray
    Write-Host "  dotnet script ResetAdminPassword.cs" -ForegroundColor Gray
    Write-Host ""
    Write-Host "OPCIÓN 3: Ejecutar el SQL directamente en SSMS:" -ForegroundColor White
    Write-Host "  resetear-admin.sql" -ForegroundColor Gray
}

Write-Host ""
Write-Host "Presione cualquier tecla para continuar..." -ForegroundColor Yellow
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
