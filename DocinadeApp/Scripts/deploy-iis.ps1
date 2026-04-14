# Script de publicación IIS - Versión Funcional
param(
    [string]$SiteName = "RubricasApp",
    [string]$Port = "8080",
    [string]$AppPoolName = "RubricasAppPool"
)

Write-Host "=== Configurando IIS para RubricasApp ===" -ForegroundColor Green

# Verificar privilegios de administrador
$isAdmin = ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole] "Administrator")
if (-NOT $isAdmin) {
    Write-Host "ERROR: Ejecuta PowerShell como Administrador" -ForegroundColor Red
    exit 1
}

# Verificar IIS
try {
    Import-Module WebAdministration -ErrorAction Stop
    Write-Host "✓ IIS disponible" -ForegroundColor Green
} catch {
    Write-Host "ERROR: IIS no está instalado" -ForegroundColor Red
    exit 1
}

# Variables
$publishPath = ".\bin\Release\Publish"
$iisPath = "C:\inetpub\wwwroot\$SiteName"
$wwwrootPath = Join-Path $iisPath "wwwroot"

Write-Host "Configuración:" -ForegroundColor Cyan
Write-Host "- Sitio: $SiteName" -ForegroundColor White
Write-Host "- Puerto: $Port" -ForegroundColor White
Write-Host "- App Pool: $AppPoolName" -ForegroundColor White
Write-Host "- Origen: $publishPath" -ForegroundColor White
Write-Host "- Destino: $iisPath" -ForegroundColor White
Write-Host "- wwwroot: $wwwrootPath" -ForegroundColor White

try {
    # 1. Verificar archivos publicados
    if (!(Test-Path $publishPath)) {
        Write-Host "ERROR: No se encontraron archivos publicados en $publishPath" -ForegroundColor Red
        Write-Host "Ejecuta primero: dotnet publish --configuration Release --output bin/Release/Publish" -ForegroundColor Yellow
        exit 1
    }
    
    Write-Host "1. Archivos encontrados: $publishPath" -ForegroundColor Yellow
    
    # 2. Crear directorio IIS
    if (!(Test-Path $iisPath)) {
        New-Item -ItemType Directory -Path $iisPath -Force | Out-Null
    }
    Write-Host "2. Directorio IIS: $iisPath" -ForegroundColor Yellow
    
    # 3. Detener App Pool si existe
    if (Get-IISAppPool -Name $AppPoolName -ErrorAction SilentlyContinue) {
        Stop-IISAppPool -Name $AppPoolName -ErrorAction SilentlyContinue
        Start-Sleep 2
    }
    Write-Host "3. App Pool preparado: $AppPoolName" -ForegroundColor Yellow
    
    # 4. Copiar archivos
    Copy-Item -Path "$publishPath\*" -Destination $iisPath -Recurse -Force
    Write-Host "4. Archivos copiados" -ForegroundColor Yellow
    
    # 5. Crear/Configurar App Pool
    if (!(Get-IISAppPool -Name $AppPoolName -ErrorAction SilentlyContinue)) {
        New-IISAppPool -Name $AppPoolName -Force
    }
    Set-ItemProperty -Path "IIS:\AppPools\$AppPoolName" -Name processModel.identityType -Value ApplicationPoolIdentity
    Set-ItemProperty -Path "IIS:\AppPools\$AppPoolName" -Name managedRuntimeVersion -Value ""
    Write-Host "5. App Pool configurado" -ForegroundColor Yellow
    
    # 6. Crear/Actualizar sitio
    if (Get-IISSite -Name $SiteName -ErrorAction SilentlyContinue) {
        Set-ItemProperty -Path "IIS:\Sites\$SiteName" -Name physicalPath -Value $iisPath
        Set-ItemProperty -Path "IIS:\Sites\$SiteName" -Name applicationPool -Value $AppPoolName
    } else {
        New-IISSite -Name $SiteName -PhysicalPath $iisPath -Port $Port -ApplicationPool $AppPoolName
    }
    Write-Host "6. Sitio web configurado" -ForegroundColor Yellow
    
    # 7. Permisos en carpeta raíz
    $acl = Get-Acl $iisPath
    $rule = New-Object System.Security.AccessControl.FileSystemAccessRule("IIS_IUSRS", "FullControl", "ContainerInherit,ObjectInherit", "None", "Allow")
    $acl.SetAccessRule($rule)
    $acl | Set-Acl $iisPath
    Write-Host "7. Permisos configurados en carpeta raíz" -ForegroundColor Yellow
    
    # 7.1 Configurar permisos específicos para uploads
    $uploadsPath = Join-Path $iisPath "wwwroot\uploads"
    if (Test-Path $uploadsPath) {
        icacls "$uploadsPath" /grant "IIS_IUSRS:(OI)(CI)M" /T /Q
        icacls "$uploadsPath" /grant "IUSR:(OI)(CI)R" /T /Q
        icacls "$uploadsPath" /grant "IIS AppPool\$AppPoolName:(OI)(CI)M" /T /Q
        Write-Host "7.1 Permisos configurados en uploads/sliders" -ForegroundColor Yellow
    } else {
        Write-Host "7.1 Creando carpeta uploads..." -ForegroundColor Yellow
        New-Item -ItemType Directory -Path "$uploadsPath\sliders" -Force | Out-Null
        icacls "$uploadsPath" /grant "IIS_IUSRS:(OI)(CI)M" /T /Q
        icacls "$uploadsPath" /grant "IUSR:(OI)(CI)R" /T /Q
        icacls "$uploadsPath" /grant "IIS AppPool\$AppPoolName:(OI)(CI)M" /T /Q
    }
    
    # 8. Iniciar servicios
    Start-IISAppPool -Name $AppPoolName
    Start-IISSite -Name $SiteName
    Write-Host "8. Servicios iniciados" -ForegroundColor Yellow
    
    # Resultado
    Write-Host ""
    Write-Host "🎉 PUBLICACIÓN EXITOSA 🎉" -ForegroundColor Green
    Write-Host "URL: http://localhost:$Port" -ForegroundColor Cyan
    Write-Host "Sitio: $SiteName" -ForegroundColor Cyan
    Write-Host "App Pool: $AppPoolName" -ForegroundColor Cyan
    
    $openBrowser = Read-Host "¿Abrir en navegador? (S/N)"
    if ($openBrowser -eq 'S' -or $openBrowser -eq 's') {
        Start-Process "http://localhost:$Port"
    }
    
} catch {
    Write-Host ""
    Write-Host "❌ ERROR: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host ""
    Write-Host "Soluciones:" -ForegroundColor Yellow
    Write-Host "1. Ejecuta como Administrador" -ForegroundColor White
    Write-Host "2. Verifica que IIS esté instalado" -ForegroundColor White
    Write-Host "3. Puerto $Port debe estar disponible" -ForegroundColor White
    Write-Host "4. Instala ASP.NET Core Hosting Bundle" -ForegroundColor White
}
