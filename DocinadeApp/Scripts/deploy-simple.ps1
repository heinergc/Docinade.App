param(
    [string]$SiteName = "RubricasApp",
    [string]$Port = "8080",
    [string]$AppPoolName = "RubricasAppPool"
)

Write-Host "=== Publicando en IIS Local ===" -ForegroundColor Green

# Verificar administrador
if (-NOT ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole] "Administrator")) {
    Write-Host "ERROR: Ejecuta como Administrador" -ForegroundColor Red
    exit 1
}

# Verificar IIS
try {
    Import-Module WebAdministration -ErrorAction Stop
    Write-Host "IIS OK" -ForegroundColor Green
} catch {
    Write-Host "ERROR: IIS no disponible" -ForegroundColor Red
    exit 1
}

# Variables
$publishPath = Join-Path $PWD "bin\Release\Publish"
$iisPath = "C:\inetpub\wwwroot\$SiteName"

Write-Host "Sitio: $SiteName | Puerto: $Port | Pool: $AppPoolName" -ForegroundColor Cyan

try {
    # 1. Verificar archivos
    if (!(Test-Path $publishPath)) {
        Write-Host "ERROR: No hay archivos en $publishPath" -ForegroundColor Red
        exit 1
    }
    Write-Host "1. Archivos OK: $publishPath" -ForegroundColor Yellow
    
    # 2. Crear directorio IIS
    if (!(Test-Path $iisPath)) {
        New-Item -ItemType Directory -Path $iisPath -Force | Out-Null
    }
    Write-Host "2. Directorio OK: $iisPath" -ForegroundColor Yellow
    
    # 3. Detener App Pool
    if (Get-WebAppPool -Name $AppPoolName -ErrorAction SilentlyContinue) {
        Write-Host "3. Deteniendo App Pool existente..." -ForegroundColor Yellow
        Stop-WebAppPool -Name $AppPoolName -ErrorAction SilentlyContinue
        Start-Sleep 2
    }
    
    # 4. Copiar archivos
    Write-Host "4. Copiando archivos..." -ForegroundColor Yellow
    Copy-Item -Path "$publishPath\*" -Destination $iisPath -Recurse -Force
    
    # 5. Crear App Pool
    if (!(Test-Path "IIS:\AppPools\$AppPoolName")) {
        Write-Host "5. Creando App Pool: $AppPoolName" -ForegroundColor Yellow
        New-WebAppPool -Name $AppPoolName
    }
    
    # Configurar App Pool para .NET Core
    Set-ItemProperty -Path "IIS:\AppPools\$AppPoolName" -Name processModel.identityType -Value ApplicationPoolIdentity
    Set-ItemProperty -Path "IIS:\AppPools\$AppPoolName" -Name managedRuntimeVersion -Value ""
    Write-Host "App Pool configurado" -ForegroundColor Green
    
    # 6. Crear sitio
    if (Get-Website -Name $SiteName -ErrorAction SilentlyContinue) {
        Write-Host "6. Actualizando sitio existente..." -ForegroundColor Yellow
        Set-ItemProperty -Path "IIS:\Sites\$SiteName" -Name physicalPath -Value $iisPath
        Set-ItemProperty -Path "IIS:\Sites\$SiteName" -Name applicationPool -Value $AppPoolName
    } else {
        Write-Host "6. Creando sitio: $SiteName" -ForegroundColor Yellow
        New-Website -Name $SiteName -PhysicalPath $iisPath -Port $Port -ApplicationPool $AppPoolName
    }
    
    # 7. Permisos
    Write-Host "7. Configurando permisos..." -ForegroundColor Yellow
    $acl = Get-Acl $iisPath
    $rule = New-Object System.Security.AccessControl.FileSystemAccessRule("IIS_IUSRS", "FullControl", "ContainerInherit,ObjectInherit", "None", "Allow")
    $acl.SetAccessRule($rule)
    $acl | Set-Acl $iisPath
    
    # 8. Iniciar
    Write-Host "8. Iniciando servicios..." -ForegroundColor Yellow
    Start-WebAppPool -Name $AppPoolName
    Start-Website -Name $SiteName
    
    Write-Host ""
    Write-Host "EXITO! Aplicacion publicada" -ForegroundColor Green
    Write-Host "URL: http://localhost:$Port" -ForegroundColor Cyan
    Write-Host "Sitio: $SiteName" -ForegroundColor Cyan
    Write-Host "Pool: $AppPoolName" -ForegroundColor Cyan
    
    $open = Read-Host "Abrir navegador? (s/n)"
    if ($open -eq 's') {
        Start-Process "http://localhost:$Port"
    }
    
} catch {
    Write-Host ""
    Write-Host "ERROR: $($_.Exception.Message)" -ForegroundColor Red
}
