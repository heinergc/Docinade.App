# Script de Despliegue a Azure - RubricasApp.Web
# Ejecutar con: .\deploy-azure.ps1

param(
    [Parameter(Mandatory=$false)]
    [string]$ResourceGroup = "RubricasAppRG",
    
    [Parameter(Mandatory=$false)]
    [string]$Location = "eastus",
    
    [Parameter(Mandatory=$false)]
    [string]$AppName = "rubricasapp",
    
    [Parameter(Mandatory=$false)]
    [string]$SqlServer = "serverrubricasapp",
    
    [Parameter(Mandatory=$false)]
    [string]$SqlDatabase = "RubricasDb",
    
    [Parameter(Mandatory=$false)]
    [string]$SqlUser = "RubricasUser",
    
    [Parameter(Mandatory=$true)]
    [string]$SqlPassword
)

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Despliegue de RubricasApp a Azure" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Verificar si Azure CLI está instalado
try {
    az --version | Out-Null
    Write-Host "[OK] Azure CLI detectado" -ForegroundColor Green
} catch {
    Write-Host "[ERROR] Azure CLI no está instalado" -ForegroundColor Red
    Write-Host "Descarga desde: https://aka.ms/installazurecliwindows" -ForegroundColor Yellow
    exit 1
}

# Login en Azure
Write-Host ""
Write-Host "[PASO 1] Login en Azure..." -ForegroundColor Yellow
az login

if ($LASTEXITCODE -ne 0) {
    Write-Host "[ERROR] No se pudo iniciar sesión en Azure" -ForegroundColor Red
    exit 1
}

# Crear Resource Group
Write-Host ""
Write-Host "[PASO 2] Creando Resource Group: $ResourceGroup..." -ForegroundColor Yellow
az group create --name $ResourceGroup --location $Location

# Crear SQL Server
Write-Host ""
Write-Host "[PASO 3] Creando SQL Server: $SqlServer..." -ForegroundColor Yellow
az sql server create `
    --name $SqlServer `
    --resource-group $ResourceGroup `
    --location $Location `
    --admin-user $SqlUser `
    --admin-password $SqlPassword

# Configurar Firewall SQL Server
Write-Host ""
Write-Host "[PASO 4] Configurando Firewall de SQL Server..." -ForegroundColor Yellow

# Permitir servicios de Azure
az sql server firewall-rule create `
    --resource-group $ResourceGroup `
    --server $SqlServer `
    --name AllowAzureServices `
    --start-ip-address 0.0.0.0 `
    --end-ip-address 0.0.0.0

# Obtener IP pública local
$MyIP = (Invoke-WebRequest -Uri "https://api.ipify.org" -UseBasicParsing).Content.Trim()
Write-Host "Tu IP pública: $MyIP" -ForegroundColor Cyan

az sql server firewall-rule create `
    --resource-group $ResourceGroup `
    --server $SqlServer `
    --name AllowMyIP `
    --start-ip-address $MyIP `
    --end-ip-address $MyIP

# Crear SQL Database
Write-Host ""
Write-Host "[PASO 5] Creando Base de Datos: $SqlDatabase..." -ForegroundColor Yellow
az sql db create `
    --resource-group $ResourceGroup `
    --server $SqlServer `
    --name $SqlDatabase `
    --service-objective S0 `
    --backup-storage-redundancy Local

# Crear App Service Plan
Write-Host ""
Write-Host "[PASO 6] Creando App Service Plan..." -ForegroundColor Yellow
az appservice plan create `
    --name "$AppName-plan" `
    --resource-group $ResourceGroup `
    --sku B1 `
    --is-linux

# Crear Web App
Write-Host ""
Write-Host "[PASO 7] Creando Web App: $AppName..." -ForegroundColor Yellow
az webapp create `
    --resource-group $ResourceGroup `
    --plan "$AppName-plan" `
    --name $AppName `
    --runtime "DOTNETCORE:8.0"

# Configurar Connection String
Write-Host ""
Write-Host "[PASO 8] Configurando Connection String..." -ForegroundColor Yellow
$ConnectionString = "Server=tcp:$SqlServer.database.windows.net,1433;Initial Catalog=$SqlDatabase;Persist Security Info=False;User ID=$SqlUser;Password=$SqlPassword;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"

az webapp config connection-string set `
    --resource-group $ResourceGroup `
    --name $AppName `
    --settings DefaultConnection=$ConnectionString `
    --connection-string-type SQLAzure

# Configurar App Settings
Write-Host ""
Write-Host "[PASO 9] Configurando Variables de Entorno..." -ForegroundColor Yellow
az webapp config appsettings set `
    --resource-group $ResourceGroup `
    --name $AppName `
    --settings `
        ASPNETCORE_ENVIRONMENT="Production" `
        WEBSITE_TIME_ZONE="Central Standard Time" `
        SCM_DO_BUILD_DURING_DEPLOYMENT="true"

# Publicar Aplicación
Write-Host ""
Write-Host "[PASO 10] Publicando Aplicación..." -ForegroundColor Yellow

# Limpiar compilaciones anteriores
Write-Host "Limpiando proyecto..." -ForegroundColor Cyan
dotnet clean --configuration Release

# Compilar en Release
Write-Host "Compilando en modo Release..." -ForegroundColor Cyan
dotnet build --configuration Release

if ($LASTEXITCODE -ne 0) {
    Write-Host "[ERROR] Falló la compilación" -ForegroundColor Red
    exit 1
}

# Publicar
Write-Host "Generando paquete de publicación..." -ForegroundColor Cyan
dotnet publish --configuration Release --output ./publish

if ($LASTEXITCODE -ne 0) {
    Write-Host "[ERROR] Falló la publicación" -ForegroundColor Red
    exit 1
}

# Comprimir
Write-Host "Comprimiendo archivos..." -ForegroundColor Cyan
if (Test-Path "./rubricasapp.zip") {
    Remove-Item "./rubricasapp.zip" -Force
}
Compress-Archive -Path ./publish/* -DestinationPath ./rubricasapp.zip -Force

# Desplegar
Write-Host "Desplegando a Azure..." -ForegroundColor Cyan
az webapp deployment source config-zip `
    --resource-group $ResourceGroup `
    --name $AppName `
    --src ./rubricasapp.zip

# Limpiar archivos temporales
Write-Host ""
Write-Host "[LIMPIEZA] Eliminando archivos temporales..." -ForegroundColor Yellow
Remove-Item -Recurse -Force ./publish -ErrorAction SilentlyContinue
Remove-Item -Force ./rubricasapp.zip -ErrorAction SilentlyContinue

# Resumen
Write-Host ""
Write-Host "========================================" -ForegroundColor Green
Write-Host "  DESPLIEGUE COMPLETADO" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""
Write-Host "URL de la aplicación: https://$AppName.azurewebsites.net" -ForegroundColor Cyan
Write-Host "SQL Server: $SqlServer.database.windows.net" -ForegroundColor Cyan
Write-Host "Base de Datos: $SqlDatabase" -ForegroundColor Cyan
Write-Host ""
Write-Host "Credenciales de acceso inicial:" -ForegroundColor Yellow
Write-Host "  Email: admin@rubricas.edu" -ForegroundColor White
Write-Host "  Password: Admin@2025!" -ForegroundColor White
Write-Host ""
Write-Host "Para ver logs en tiempo real:" -ForegroundColor Yellow
Write-Host "  az webapp log tail --resource-group $ResourceGroup --name $AppName" -ForegroundColor White
Write-Host ""
Write-Host "Para abrir en navegador:" -ForegroundColor Yellow
Write-Host "  az webapp browse --resource-group $ResourceGroup --name $AppName" -ForegroundColor White
Write-Host ""

# Preguntar si desea abrir en navegador
$OpenBrowser = Read-Host "¿Desea abrir la aplicación en el navegador? (S/N)"
if ($OpenBrowser -eq "S" -or $OpenBrowser -eq "s") {
    az webapp browse --resource-group $ResourceGroup --name $AppName
}

Write-Host ""
Write-Host "[SUCCESS] Proceso de despliegue finalizado" -ForegroundColor Green
