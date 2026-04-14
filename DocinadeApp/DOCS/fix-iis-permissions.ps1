# Script para configurar permisos IIS en la carpeta uploads
# Ejecutar como Administrador

$appPath = "C:\inetpub\wwwroot\RubricasApp"
$uploadsPath = "$appPath\wwwroot\uploads"

Write-Host "[INFO] Configurando permisos para IIS..." -ForegroundColor Cyan

# Verificar que la carpeta existe
if (!(Test-Path $uploadsPath)) {
    Write-Host "[WARNING] La carpeta $uploadsPath no existe. Creando..." -ForegroundColor Yellow
    New-Item -ItemType Directory -Path $uploadsPath -Force
    New-Item -ItemType Directory -Path "$uploadsPath\sliders" -Force
}

# Dar permisos a IIS_IUSRS (cuenta de proceso de aplicación)
Write-Host "[INFO] Agregando permisos a IIS_IUSRS..." -ForegroundColor Green
icacls "$uploadsPath" /grant "IIS_IUSRS:(OI)(CI)M" /T

# Dar permisos a IUSR (usuario anónimo de IIS)
Write-Host "[INFO] Agregando permisos a IUSR..." -ForegroundColor Green
icacls "$uploadsPath" /grant "IUSR:(OI)(CI)R" /T

# Dar permisos al grupo Users (lectura)
Write-Host "[INFO] Agregando permisos a Users..." -ForegroundColor Green
icacls "$uploadsPath" /grant "Users:(OI)(CI)R" /T

# Dar permisos al AppPool (si existe)
$appPoolName = "RubricasApp"
Write-Host "[INFO] Agregando permisos al AppPool: IIS AppPool\$appPoolName..." -ForegroundColor Green
icacls "$uploadsPath" /grant "IIS AppPool\$appPoolName:(OI)(CI)M" /T

Write-Host ""
Write-Host "[SUCCESS] Permisos configurados exitosamente" -ForegroundColor Green
Write-Host ""
Write-Host "Permisos actuales:" -ForegroundColor Cyan
icacls "$uploadsPath"

Write-Host ""
Write-Host "[INFO] Reiniciando IIS..." -ForegroundColor Yellow
iisreset

Write-Host ""
Write-Host "[SUCCESS] Proceso completado. Verifica que ahora se puedan leer las imagenes." -ForegroundColor Green
