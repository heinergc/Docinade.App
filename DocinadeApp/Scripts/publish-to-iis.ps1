# Script para publicar RubricasApp en IIS Local
# Ejecutar como Administrador

param(
    [string]$SiteName = "RubricasApp",
    [string]$Port = "8080",
    [string]$AppPoolName = "RubricasAppPool"
)

Write-Host "=== Publicando RubricasApp en IIS Local ===" -ForegroundColor Green

# Verificar si se ejecuta como administrador
if (-NOT ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole] "Administrator")) {
    Write-Error "Este script debe ejecutarse como Administrador"
    exit 1
}

# Importar módulo WebAdministration si no está cargado
if (!(Get-Module -Name WebAdministration)) {
    Import-Module WebAdministration -ErrorAction SilentlyContinue
    if (!(Get-Module -Name WebAdministration)) {
        Write-Error "No se puede cargar el módulo WebAdministration. Asegúrate de que IIS esté instalado."
        exit 1
    }
}

try {
    # 1. Compilar y publicar la aplicación
    Write-Host "1. Compilando y publicando la aplicación..." -ForegroundColor Yellow
    dotnet publish --configuration Release --output "bin/Release/Publish" --self-contained false --runtime win-x64
    
    if ($LASTEXITCODE -ne 0) {
        throw "Error al publicar la aplicación"
    }

    # 2. Definir rutas
    $PublishPath = Join-Path $PWD "bin\Release\Publish"
    $IISPath = "C:\inetpub\wwwroot\$SiteName"

    Write-Host "Ruta de publicación: $PublishPath" -ForegroundColor Cyan
    Write-Host "Ruta de IIS: $IISPath" -ForegroundColor Cyan

    # 3. Crear directorio en IIS si no existe
    if (!(Test-Path $IISPath)) {
        Write-Host "2. Creando directorio en IIS: $IISPath" -ForegroundColor Yellow
        New-Item -ItemType Directory -Path $IISPath -Force | Out-Null
    }

    # 4. Detener Application Pool si existe
    if (Get-IISAppPool -Name $AppPoolName -ErrorAction SilentlyContinue) {
        Write-Host "3. Deteniendo Application Pool existente..." -ForegroundColor Yellow
        Stop-IISAppPool -Name $AppPoolName -ErrorAction SilentlyContinue
        Start-Sleep -Seconds 2
    }

    # 5. Copiar archivos publicados
    Write-Host "4. Copiando archivos a IIS..." -ForegroundColor Yellow
    Copy-Item -Path "$PublishPath\*" -Destination $IISPath -Recurse -Force

    # 6. Crear Application Pool para .NET Core
    if (!(Get-IISAppPool -Name $AppPoolName -ErrorAction SilentlyContinue)) {
        Write-Host "5. Creando Application Pool: $AppPoolName" -ForegroundColor Yellow
        New-IISAppPool -Name $AppPoolName -Force
    }
    
    # Configurar Application Pool para .NET Core
    Set-ItemProperty -Path "IIS:\AppPools\$AppPoolName" -Name processModel.identityType -Value ApplicationPoolIdentity
    Set-ItemProperty -Path "IIS:\AppPools\$AppPoolName" -Name managedRuntimeVersion -Value ""
    Set-ItemProperty -Path "IIS:\AppPools\$AppPoolName" -Name enable32BitAppOnWin64 -Value $false

    # 7. Crear o actualizar sitio web
    if (Get-IISSite -Name $SiteName -ErrorAction SilentlyContinue) {
        Write-Host "6. Actualizando sitio existente: $SiteName" -ForegroundColor Yellow
        Set-ItemProperty -Path "IIS:\Sites\$SiteName" -Name physicalPath -Value $IISPath
        Set-ItemProperty -Path "IIS:\Sites\$SiteName" -Name applicationPool -Value $AppPoolName
    } else {
        Write-Host "6. Creando nuevo sitio: $SiteName en puerto $Port" -ForegroundColor Yellow
        New-IISSite -Name $SiteName -PhysicalPath $IISPath -Port $Port -ApplicationPool $AppPoolName
    }

    # 8. Crear directorio de logs si no existe
    $LogsPath = Join-Path $IISPath "logs"
    if (!(Test-Path $LogsPath)) {
        New-Item -ItemType Directory -Path $LogsPath -Force | Out-Null
    }

    # 9. Configurar permisos
    Write-Host "7. Configurando permisos..." -ForegroundColor Yellow
    $acl = Get-Acl $IISPath
    $accessRule = New-Object System.Security.AccessControl.FileSystemAccessRule("IIS_IUSRS", "FullControl", "ContainerInherit,ObjectInherit", "None", "Allow")
    $acl.SetAccessRule($accessRule)
    $acl | Set-Acl $IISPath

    # 10. Iniciar Application Pool y sitio
    Write-Host "8. Iniciando Application Pool y sitio..." -ForegroundColor Yellow
    Start-IISAppPool -Name $AppPoolName
    Start-IISSite -Name $SiteName

    # 11. Verificar estado
    Start-Sleep -Seconds 3
    $appPool = Get-IISAppPool -Name $AppPoolName
    $site = Get-IISSite -Name $SiteName

    Write-Host ""
    Write-Host "=== PUBLICACIÓN COMPLETADA ===" -ForegroundColor Green
    Write-Host "Sitio: $SiteName" -ForegroundColor Cyan
    Write-Host "URL: http://localhost:$Port" -ForegroundColor Cyan
    Write-Host "Application Pool: $AppPoolName (Estado: $($appPool.State))" -ForegroundColor Cyan
    Write-Host "Sitio Estado: $($site.State)" -ForegroundColor Cyan
    Write-Host "Ruta física: $IISPath" -ForegroundColor Cyan
    
    Write-Host ""
    Write-Host "Puedes acceder a la aplicación en: http://localhost:$Port" -ForegroundColor Green
    
    # Preguntar si abrir el navegador
    $response = Read-Host "¿Deseas abrir la aplicación en el navegador? (S/N)"
    if ($response -eq 'S' -or $response -eq 's') {
        Start-Process "http://localhost:$Port"
    }

} catch {
    Write-Error "Error durante la publicación: $($_.Exception.Message)"
    Write-Host ""
    Write-Host "Pasos para solucionar problemas:" -ForegroundColor Yellow
    Write-Host "1. Verifica que IIS esté instalado y el módulo ASP.NET Core esté configurado" -ForegroundColor White
    Write-Host "2. Ejecuta este script como Administrador" -ForegroundColor White
    Write-Host "3. Verifica que el puerto $Port no esté en uso" -ForegroundColor White
    Write-Host "4. Revisa los logs en: ${IISPath}\logs" -ForegroundColor White
    exit 1
}
