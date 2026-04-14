# Script de Publicación Rápida a Azure
# Usar cuando la infraestructura ya existe
# Ejecutar con: .\publish-azure.ps1

param(
    [Parameter(Mandatory=$false)]
    [string]$ResourceGroup = "RubricasAppRG",
    
    [Parameter(Mandatory=$false)]
    [string]$AppName = "rubricasapp"
)

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Publicación Rápida a Azure" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Verificar Azure CLI
try {
    az --version | Out-Null
    Write-Host "[OK] Azure CLI detectado" -ForegroundColor Green
} catch {
    Write-Host "[ERROR] Azure CLI no está instalado" -ForegroundColor Red
    exit 1
}

# Verificar si está logueado
Write-Host "[INFO] Verificando sesión de Azure..." -ForegroundColor Yellow
$Account = az account show 2>$null
if ($LASTEXITCODE -ne 0) {
    Write-Host "[WARN] No hay sesión activa. Iniciando login..." -ForegroundColor Yellow
    az login
}

# Limpiar
Write-Host ""
Write-Host "[PASO 1/5] Limpiando compilaciones anteriores..." -ForegroundColor Yellow
dotnet clean --configuration Release

# Compilar
Write-Host ""
Write-Host "[PASO 2/5] Compilando en modo Release..." -ForegroundColor Yellow
dotnet build --configuration Release

if ($LASTEXITCODE -ne 0) {
    Write-Host "[ERROR] Falló la compilación" -ForegroundColor Red
    exit 1
}

# Publicar
Write-Host ""
Write-Host "[PASO 3/5] Generando paquete de publicación..." -ForegroundColor Yellow
if (Test-Path "./publish") {
    Remove-Item -Recurse -Force ./publish
}
dotnet publish --configuration Release --output ./publish

if ($LASTEXITCODE -ne 0) {
    Write-Host "[ERROR] Falló la publicación" -ForegroundColor Red
    exit 1
}

# Comprimir
Write-Host ""
Write-Host "[PASO 4/5] Comprimiendo archivos..." -ForegroundColor Yellow
if (Test-Path "./rubricasapp.zip") {
    Remove-Item -Force ./rubricasapp.zip
}
Compress-Archive -Path ./publish/* -DestinationPath ./rubricasapp.zip -Force

# Desplegar
Write-Host ""
Write-Host "[PASO 5/5] Desplegando a Azure App Service..." -ForegroundColor Yellow
Write-Host "Resource Group: $ResourceGroup" -ForegroundColor Cyan
Write-Host "App Name: $AppName" -ForegroundColor Cyan
Write-Host ""

az webapp deployment source config-zip `
    --resource-group $ResourceGroup `
    --name $AppName `
    --src ./rubricasapp.zip

if ($LASTEXITCODE -ne 0) {
    Write-Host "[ERROR] Falló el despliegue" -ForegroundColor Red
    exit 1
}

# Limpiar archivos temporales
Write-Host ""
Write-Host "[LIMPIEZA] Eliminando archivos temporales..." -ForegroundColor Yellow
Remove-Item -Recurse -Force ./publish -ErrorAction SilentlyContinue
Remove-Item -Force ./rubricasapp.zip -ErrorAction SilentlyContinue

# Resumen
Write-Host ""
Write-Host "========================================" -ForegroundColor Green
Write-Host "  PUBLICACIÓN COMPLETADA" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""
Write-Host "URL: https://$AppName.azurewebsites.net" -ForegroundColor Cyan
Write-Host ""
Write-Host "Comandos útiles:" -ForegroundColor Yellow
Write-Host "  Ver logs:" -ForegroundColor White
Write-Host "    az webapp log tail --resource-group $ResourceGroup --name $AppName" -ForegroundColor Gray
Write-Host ""
Write-Host "  Reiniciar app:" -ForegroundColor White
Write-Host "    az webapp restart --resource-group $ResourceGroup --name $AppName" -ForegroundColor Gray
Write-Host ""
Write-Host "  Abrir en navegador:" -ForegroundColor White
Write-Host "    az webapp browse --resource-group $ResourceGroup --name $AppName" -ForegroundColor Gray
Write-Host ""

# Preguntar si desea ver logs
$ViewLogs = Read-Host "¿Desea ver los logs de la aplicación? (S/N)"
if ($ViewLogs -eq "S" -or $ViewLogs -eq "s") {
    Write-Host ""
    Write-Host "Mostrando logs (Ctrl+C para salir)..." -ForegroundColor Cyan
    az webapp log tail --resource-group $ResourceGroup --name $AppName
}
