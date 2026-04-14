param(
    [string]$SiteName = "RubricasApp", 
    [string]$Port = "8080",
    [string]$AppPoolName = "RubricasAppPool"
)

Write-Host "=== Desplegando en IIS ===" -ForegroundColor Green

# Verificar privilegios
if (-NOT ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole] "Administrator")) {
    Write-Host "ERROR: Ejecuta como Administrador" -ForegroundColor Red
    exit 1
}

# Variables
$publishPath = Join-Path $PWD "bin\Release\Publish"
$iisPath = "C:\inetpub\wwwroot\$SiteName"

Write-Host "Configuración: $SiteName -> Puerto $Port" -ForegroundColor Cyan

try {
    # 1. Verificar archivos publicados
    if (!(Test-Path $publishPath)) {
        Write-Host "ERROR: No hay archivos en $publishPath" -ForegroundColor Red
        exit 1
    }
    Write-Host "✓ Archivos encontrados" -ForegroundColor Green
    
    # 2. Crear directorio IIS
    if (!(Test-Path $iisPath)) {
        New-Item -ItemType Directory -Path $iisPath -Force | Out-Null
    }
    Write-Host "✓ Directorio IIS creado: $iisPath" -ForegroundColor Green
    
    # 3. Copiar archivos
    Write-Host "Copiando archivos..." -ForegroundColor Yellow
    Copy-Item -Path "$publishPath\*" -Destination $iisPath -Recurse -Force
    Write-Host "✓ Archivos copiados" -ForegroundColor Green
    
    # 4. Configurar usando appcmd (comando nativo de IIS)
    Write-Host "Configurando IIS..." -ForegroundColor Yellow
    
    # Crear Application Pool
    $cmd = "C:\Windows\System32\inetsrv\appcmd.exe"
    & $cmd add apppool /name:$AppPoolName /managedRuntimeVersion: /processModel.identityType:ApplicationPoolIdentity 2>$null
    Write-Host "✓ App Pool: $AppPoolName" -ForegroundColor Green
    
    # Detener sitio por defecto si usa el mismo puerto
    if ($Port -eq "80") {
        & $cmd stop site "Default Web Site" 2>$null
    }
    
    # Crear sitio web
    & $cmd add site /name:$SiteName /physicalPath:$iisPath /bindings:http/*:${Port}: 2>$null
    & $cmd set site $SiteName /applicationPool:$AppPoolName 2>$null
    Write-Host "✓ Sitio web: $SiteName" -ForegroundColor Green
    
    # 5. Configurar permisos
    Write-Host "Configurando permisos..." -ForegroundColor Yellow
    $acl = Get-Acl $iisPath
    $rule = New-Object System.Security.AccessControl.FileSystemAccessRule("IIS_IUSRS", "FullControl", "ContainerInherit,ObjectInherit", "None", "Allow")
    $acl.SetAccessRule($rule)
    $acl | Set-Acl $iisPath
    Write-Host "✓ Permisos configurados" -ForegroundColor Green
    
    # 6. Iniciar servicios
    Write-Host "Iniciando servicios..." -ForegroundColor Yellow
    & $cmd start apppool $AppPoolName 2>$null
    & $cmd start site $SiteName 2>$null
    Write-Host "✓ Servicios iniciados" -ForegroundColor Green
    
    Write-Host ""
    Write-Host "🎉 DESPLIEGUE COMPLETADO 🎉" -ForegroundColor Green
    Write-Host "URL: http://localhost:$Port" -ForegroundColor Cyan
    Write-Host "Sitio: $SiteName" -ForegroundColor Cyan
    Write-Host "Ubicación: $iisPath" -ForegroundColor Cyan
    
    $open = Read-Host "¿Abrir en el navegador? (s/n)"
    if ($open -eq 's' -or $open -eq 'S') {
        Start-Process "http://localhost:$Port"
    }
    
} catch {
    Write-Host ""
    Write-Host "ERROR: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "Verifica que IIS esté correctamente instalado" -ForegroundColor Yellow
}
