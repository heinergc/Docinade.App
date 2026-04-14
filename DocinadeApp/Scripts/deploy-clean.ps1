param(
    [string]$SiteName = "RubricasApp",
    [string]$Port = "8080",
    [string]$AppPoolName = "RubricasAppPool"
)

Write-Host "Desplegando en IIS Local" -ForegroundColor Green

# Verificar privilegios de administrador
$isAdmin = ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole] "Administrator")
if (-NOT $isAdmin) {
    Write-Host "ERROR: Ejecuta PowerShell como Administrador" -ForegroundColor Red
    exit 1
}

# Variables
$publishPath = Join-Path $PWD "bin\Release\Publish"  
$iisPath = "C:\inetpub\wwwroot\$SiteName"
$appcmd = "C:\Windows\System32\inetsrv\appcmd.exe"

Write-Host "Sitio: $SiteName | Puerto: $Port" -ForegroundColor Cyan

try {
    # 1. Verificar archivos publicados
    if (!(Test-Path $publishPath)) {
        Write-Host "ERROR: No hay archivos publicados en $publishPath" -ForegroundColor Red
        Write-Host "Ejecuta: dotnet publish" -ForegroundColor Yellow
        exit 1
    }
    Write-Host "1. Archivos OK" -ForegroundColor Green
    
    # 2. Crear directorio IIS
    if (!(Test-Path $iisPath)) {
        New-Item -ItemType Directory -Path $iisPath -Force | Out-Null
    }
    Write-Host "2. Directorio IIS: $iisPath" -ForegroundColor Green
    
    # 3. Copiar archivos
    Write-Host "3. Copiando archivos..." -ForegroundColor Yellow
    Copy-Item -Path "$publishPath\*" -Destination $iisPath -Recurse -Force
    Write-Host "   Archivos copiados OK" -ForegroundColor Green
    
    # 4. Configurar App Pool usando appcmd
    Write-Host "4. Configurando App Pool..." -ForegroundColor Yellow
    & $appcmd delete apppool $AppPoolName 2>$null
    & $appcmd add apppool /name:$AppPoolName /managedRuntimeVersion: /processModel.identityType:ApplicationPoolIdentity
    Write-Host "   App Pool creado: $AppPoolName" -ForegroundColor Green
    
    # 5. Configurar sitio web
    Write-Host "5. Configurando sitio web..." -ForegroundColor Yellow
    & $appcmd delete site $SiteName 2>$null
    & $appcmd add site /name:$SiteName /physicalPath:$iisPath /bindings:http/*:${Port}:
    & $appcmd set site $SiteName /applicationPool:$AppPoolName
    Write-Host "   Sitio web creado: $SiteName" -ForegroundColor Green
    
    # 6. Configurar permisos
    Write-Host "6. Configurando permisos..." -ForegroundColor Yellow
    $acl = Get-Acl $iisPath
    $rule = New-Object System.Security.AccessControl.FileSystemAccessRule("IIS_IUSRS", "FullControl", "ContainerInherit,ObjectInherit", "None", "Allow")
    $acl.SetAccessRule($rule)
    $acl | Set-Acl $iisPath
    Write-Host "   Permisos configurados" -ForegroundColor Green
    
    # 7. Iniciar servicios
    Write-Host "7. Iniciando servicios..." -ForegroundColor Yellow
    & $appcmd start apppool $AppPoolName
    & $appcmd start site $SiteName
    Write-Host "   Servicios iniciados" -ForegroundColor Green
    
    Write-Host ""
    Write-Host "EXITO! Aplicacion desplegada correctamente" -ForegroundColor Green
    Write-Host "URL: http://localhost:$Port" -ForegroundColor Cyan
    Write-Host "Sitio: $SiteName" -ForegroundColor White
    Write-Host "Pool: $AppPoolName" -ForegroundColor White
    Write-Host "Ubicacion: $iisPath" -ForegroundColor White
    
    $response = Read-Host "Abrir navegador? (s/n)"
    if ($response -eq 's' -or $response -eq 'S') {
        Start-Process "http://localhost:$Port"
    }
    
} catch {
    Write-Host ""
    Write-Host "ERROR durante el despliegue:" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Red
    Write-Host ""
    Write-Host "Soluciones:" -ForegroundColor Yellow
    Write-Host "1. Ejecuta como Administrador" -ForegroundColor White
    Write-Host "2. Verifica que IIS este instalado" -ForegroundColor White
    Write-Host "3. Puerto $Port debe estar libre" -ForegroundColor White
}
