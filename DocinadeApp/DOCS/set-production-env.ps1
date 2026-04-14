# Script para Configurar Variables de Entorno de Producción
# Ejecutar como Administrador

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Configurar Entorno de Producción" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Verificar permisos de administrador
$currentPrincipal = New-Object Security.Principal.WindowsPrincipal([Security.Principal.WindowsIdentity]::GetCurrent())
$isAdmin = $currentPrincipal.IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)

if (-not $isAdmin) {
    Write-Host "[ERROR] Este script debe ejecutarse como Administrador" -ForegroundColor Red
    Write-Host "Click derecho en PowerShell > Ejecutar como Administrador" -ForegroundColor Yellow
    exit 1
}

Write-Host "[INFO] Configurando variables de entorno..." -ForegroundColor Yellow
Write-Host ""

# Configurar ASPNETCORE_ENVIRONMENT en Production
Write-Host "[1/3] Estableciendo ASPNETCORE_ENVIRONMENT=Production..." -ForegroundColor Cyan
[System.Environment]::SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Production", [System.EnvironmentVariableTarget]::Machine)
Write-Host "  [OK] Variable establecida a nivel de sistema" -ForegroundColor Green

# Configurar Connection String (opcional, pero recomendado para seguridad)
$SetConnectionString = Read-Host "[2/3] ¿Desea configurar Connection String como variable de entorno? (S/N)"
if ($SetConnectionString -eq "S" -or $SetConnectionString -eq "s") {
    Write-Host ""
    Write-Host "Ingrese el Connection String de producción:" -ForegroundColor Yellow
    Write-Host "(Ejemplo: Server=tcp:serverrubricasapp.database.windows.net,1433;...)" -ForegroundColor Gray
    $ConnectionString = Read-Host "Connection String"
    
    if ($ConnectionString) {
        [System.Environment]::SetEnvironmentVariable("ConnectionStrings__DefaultConnection", $ConnectionString, [System.EnvironmentVariableTarget]::Machine)
        Write-Host "  [OK] Connection String configurado" -ForegroundColor Green
    }
}

# Configurar puerto (para IIS o Kestrel standalone)
Write-Host ""
Write-Host "[3/3] Puerto de escucha (dejar vacío para usar web.config/appsettings)..." -ForegroundColor Cyan
$Port = Read-Host "Puerto (opcional, ej: 5000)"
if ($Port) {
    [System.Environment]::SetEnvironmentVariable("PORT", $Port, [System.EnvironmentVariableTarget]::Machine)
    Write-Host "  [OK] Puerto configurado: $Port" -ForegroundColor Green
}

# Resumen
Write-Host ""
Write-Host "========================================" -ForegroundColor Green
Write-Host "  CONFIGURACIÓN COMPLETADA" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""
Write-Host "Variables de entorno configuradas:" -ForegroundColor Yellow
Write-Host "  ASPNETCORE_ENVIRONMENT = Production" -ForegroundColor White

if ($ConnectionString) {
    Write-Host "  ConnectionStrings__DefaultConnection = [CONFIGURADO]" -ForegroundColor White
}

if ($Port) {
    Write-Host "  PORT = $Port" -ForegroundColor White
}

Write-Host ""
Write-Host "[IMPORTANTE] Reinicie el servidor o servicio de IIS para aplicar cambios:" -ForegroundColor Yellow
Write-Host "  iisreset" -ForegroundColor White
Write-Host ""
Write-Host "Para servicios Windows:" -ForegroundColor Yellow
Write-Host "  Restart-Service W3SVC" -ForegroundColor White
Write-Host ""
Write-Host "Para verificar variables:" -ForegroundColor Yellow
Write-Host "  [System.Environment]::GetEnvironmentVariable('ASPNETCORE_ENVIRONMENT', 'Machine')" -ForegroundColor White
Write-Host ""

# Preguntar si desea reiniciar IIS
$RestartIIS = Read-Host "¿Desea reiniciar IIS ahora? (S/N)"
if ($RestartIIS -eq "S" -or $RestartIIS -eq "s") {
    Write-Host ""
    Write-Host "Reiniciando IIS..." -ForegroundColor Cyan
    iisreset
    Write-Host "[OK] IIS reiniciado" -ForegroundColor Green
}

Write-Host ""
Write-Host "[SUCCESS] Configuración de producción completada" -ForegroundColor Green
