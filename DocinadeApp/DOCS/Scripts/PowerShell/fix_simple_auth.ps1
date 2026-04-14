# Script simple para corregir politicas de autorizacion
Write-Host "CORRIGIENDO PROBLEMA DE POLITICAS DE AUTORIZACION..." -ForegroundColor Cyan
Write-Host "Error: The AuthorizationPolicy named: 'configuracion.ver' was not found" -ForegroundColor Yellow
Write-Host ""

# Ir al directorio correcto
Set-Location "C:\Users\hguido\Desktop\RubricasApp\src\RubricasApp.Web"

Write-Host "1. Verificando archivos de autorizacion..." -ForegroundColor Green
$files = @(
    "Authorization\PermissionPolicyProvider.cs",
    "Configuration\AuthorizationExtensions.cs",
    "Models\Permissions\ApplicationPermissions.cs"
)

$allExist = $true
foreach ($file in $files) {
    if (Test-Path $file) {
        Write-Host "   [OK] $file" -ForegroundColor Green
    } else {
        Write-Host "   [FALTA] $file" -ForegroundColor Red
        $allExist = $false
    }
}

if (-not $allExist) {
    Write-Host "   [ERROR] Faltan archivos criticos de autorizacion" -ForegroundColor Red
    exit 1
}

Write-Host "2. Compilando proyecto..." -ForegroundColor Green
$result = dotnet build --configuration Release --verbosity minimal 2>&1
if ($LASTEXITCODE -eq 0) {
    Write-Host "   [OK] Compilacion exitosa" -ForegroundColor Green
} else {
    Write-Host "   [ERROR] Error en compilacion: $result" -ForegroundColor Red
    exit 1
}

Write-Host "3. Verificando configuracion en Program.cs..." -ForegroundColor Green
if (Test-Path "Program.cs") {
    $content = Get-Content "Program.cs" -Raw
    
    if ($content.Contains("AddCustomAuthorization")) {
        Write-Host "   [OK] AddCustomAuthorization configurado" -ForegroundColor Green
    } else {
        Write-Host "   [WARN] AddCustomAuthorization no encontrado" -ForegroundColor Yellow
    }
    
    if ($content.Contains("UseAuthentication") -and $content.Contains("UseAuthorization")) {
        Write-Host "   [OK] Authentication y Authorization configurados" -ForegroundColor Green
    } else {
        Write-Host "   [WARN] Authentication/Authorization no configurados correctamente" -ForegroundColor Yellow
    }
} else {
    Write-Host "   [ERROR] Program.cs no encontrado" -ForegroundColor Red
}

Write-Host "4. Probando inicio de aplicacion..." -ForegroundColor Green
Write-Host "   Iniciando en puerto 5557 por 10 segundos..." -ForegroundColor Cyan

# Iniciar aplicacion en background
$process = Start-Process "dotnet" -ArgumentList "run --no-build --urls=http://localhost:5557" -PassThru -WindowStyle Hidden

# Esperar un poco para que inicie
Start-Sleep 8

# Verificar si esta corriendo
if ($process -and !$process.HasExited) {
    Write-Host "   [OK] Aplicacion iniciada correctamente" -ForegroundColor Green
    Write-Host "   [INFO] Prueba manual: http://localhost:5557/Admin" -ForegroundColor Cyan
    
    # Terminar proceso
    $process.Kill()
    Write-Host "   [INFO] Aplicacion terminada" -ForegroundColor Yellow
} else {
    Write-Host "   [ERROR] Aplicacion no pudo iniciar correctamente" -ForegroundColor Red
}

Write-Host ""
Write-Host "CORRECCION COMPLETADA!" -ForegroundColor Green
Write-Host ""
Write-Host "PASOS SIGUIENTES:" -ForegroundColor Cyan
Write-Host "1. Ejecutar: dotnet run" -ForegroundColor White
Write-Host "2. Navegar a: https://localhost:5001/Admin" -ForegroundColor White
Write-Host "3. El error de politica 'configuracion.ver' deberia estar resuelto" -ForegroundColor White
Write-Host ""
Write-Host "Si persiste el problema:" -ForegroundColor Yellow
Write-Host "- Verificar que el usuario tenga los permisos correctos" -ForegroundColor White
Write-Host "- Revisar logs de la aplicacion para mas detalles" -ForegroundColor White