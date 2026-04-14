# Script completo para publicar y configurar IIS correctamente
# Ejecutar como Administrador

Write-Host "=== PUBLICACIÓN Y CONFIGURACIÓN IIS ===" -ForegroundColor Green
Write-Host ""

# Paso 1: Compilar y publicar
Write-Host "[1/5] Compilando aplicación..." -ForegroundColor Cyan
dotnet publish --configuration Release --output bin/Release/Publish --self-contained false --runtime win-x64

if ($LASTEXITCODE -ne 0) {
    Write-Host "ERROR: Falló la compilación" -ForegroundColor Red
    exit 1
}

Write-Host "[SUCCESS] Compilación exitosa" -ForegroundColor Green
Write-Host ""

# Paso 2: Configurar carpeta uploads
Write-Host "[2/5] Configurando carpeta uploads..." -ForegroundColor Cyan
$publishUploads = ".\bin\Release\Publish\wwwroot\uploads\sliders"
if (!(Test-Path $publishUploads)) {
    New-Item -ItemType Directory -Path $publishUploads -Force | Out-Null
    Write-Host "Carpeta uploads/sliders creada" -ForegroundColor Yellow
}

# Paso 3: Copiar a IIS
Write-Host "[3/5] Copiando archivos a IIS..." -ForegroundColor Cyan
$iisPath = "C:\inetpub\wwwroot\RubricasApp"
$appPoolName = "RubricasApp"

# Detener App Pool si está corriendo
if (Get-IISAppPool -Name $appPoolName -ErrorAction SilentlyContinue) {
    Stop-WebAppPool -Name $appPoolName -ErrorAction SilentlyContinue
    Start-Sleep -Seconds 3
}

# Crear carpeta y copiar archivos
if (!(Test-Path $iisPath)) {
    New-Item -ItemType Directory -Path $iisPath -Force | Out-Null
}

Copy-Item -Path ".\bin\Release\Publish\*" -Destination $iisPath -Recurse -Force
Write-Host "Archivos copiados a $iisPath" -ForegroundColor Green

# Paso 4: Configurar permisos en uploads
Write-Host "[4/5] Configurando permisos..." -ForegroundColor Cyan
$uploadsPath = "$iisPath\wwwroot\uploads"

if (!(Test-Path $uploadsPath)) {
    New-Item -ItemType Directory -Path "$uploadsPath\sliders" -Force | Out-Null
}

icacls "$uploadsPath" /grant "IIS_IUSRS:(OI)(CI)M" /T /Q
icacls "$uploadsPath" /grant "IUSR:(OI)(CI)R" /T /Q  
icacls "$uploadsPath" /grant "IIS AppPool\$appPoolName:(OI)(CI)M" /T /Q
icacls "$iisPath" /grant "IIS_IUSRS:(OI)(CI)RX" /T /Q

Write-Host "Permisos configurados" -ForegroundColor Green

# Paso 5: Verificar web.config
Write-Host "[5/5] Verificando web.config..." -ForegroundColor Cyan
$webConfigPath = "$iisPath\web.config"

if (Test-Path $webConfigPath) {
    Write-Host "web.config encontrado" -ForegroundColor Green
} else {
    Write-Host "WARNING: web.config no encontrado" -ForegroundColor Yellow
}

# Reiniciar App Pool
if (Get-IISAppPool -Name $appPoolName -ErrorAction SilentlyContinue) {
    Start-WebAppPool -Name $appPoolName
    Write-Host "App Pool reiniciado" -ForegroundColor Green
}

# Reiniciar IIS (opcional pero recomendado)
Write-Host ""
Write-Host "Reiniciando IIS..." -ForegroundColor Yellow
iisreset /noforce

Write-Host ""
Write-Host "==========================================" -ForegroundColor Green
Write-Host "[SUCCESS] PUBLICACIÓN COMPLETADA" -ForegroundColor Green
Write-Host "==========================================" -ForegroundColor Green
Write-Host ""
Write-Host "Rutas importantes:" -ForegroundColor Cyan
Write-Host "  - Aplicación: $iisPath" -ForegroundColor White
Write-Host "  - wwwroot: $iisPath\wwwroot" -ForegroundColor White
Write-Host "  - Uploads: $iisPath\wwwroot\uploads\sliders" -ForegroundColor White
Write-Host ""
Write-Host "URL de prueba para imágenes:" -ForegroundColor Cyan
Write-Host "  http://localhost/uploads/sliders/[nombre-archivo].png" -ForegroundColor Yellow
Write-Host ""
Write-Host "Verificar en IIS Manager que:" -ForegroundColor Cyan
Write-Host "  1. Physical Path apunta a: $iisPath" -ForegroundColor White
Write-Host "  2. App Pool: $appPoolName (.NET CLR Version = No Managed Code)" -ForegroundColor White
Write-Host "  3. Authentication: Anonymous habilitado" -ForegroundColor White
Write-Host ""
