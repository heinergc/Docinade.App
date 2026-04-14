# Script simplificado para publicar RubricasApp en IIS Local
# Ejecutar como Administrador

param(
    [string]$SiteName = "RubricasApp",
    [string]$Port = "8080",
    [string]$AppPoolName = "RubricasAppPool"
)

Write-Host "=== TEST: Publicando RubricasApp en IIS Local ===" -ForegroundColor Green
Write-Host "Sitio: $SiteName" -ForegroundColor Cyan
Write-Host "Puerto: $Port" -ForegroundColor Cyan  
Write-Host "App Pool: $AppPoolName" -ForegroundColor Cyan

# Verificar si se ejecuta como administrador
$isAdmin = ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole] "Administrator")

if (-NOT $isAdmin) {
    Write-Error "Este script debe ejecutarse como Administrador"
    Read-Host "Presiona Enter para salir"
    exit 1
}

# Verificar si IIS está disponible
try {
    Import-Module WebAdministration -ErrorAction Stop
    Write-Host "✓ Módulo WebAdministration cargado correctamente" -ForegroundColor Green
} catch {
    Write-Error "No se puede cargar el módulo WebAdministration. IIS no está instalado o configurado."
    Write-Host "Para instalar IIS, ejecuta este comando como Administrador:"
    Write-Host "Enable-WindowsOptionalFeature -Online -FeatureName IIS-WebServerRole -All" -ForegroundColor Yellow
    Read-Host "Presiona Enter para salir"
    exit 1
}

try {
    # 1. Compilar y publicar la aplicación
    Write-Host "1. Compilando y publicando la aplicación..." -ForegroundColor Yellow
    $publishResult = dotnet publish --configuration Release --output "bin/Release/Publish" --self-contained false --runtime win-x64
    
    if ($LASTEXITCODE -ne 0) {
        throw "Error al publicar la aplicación. Código de salida: $LASTEXITCODE"
    }

    Write-Host "✓ Aplicación publicada correctamente" -ForegroundColor Green

    # 2. Definir rutas
    $PublishPath = Join-Path $PWD "bin\Release\Publish"
    $IISPath = "C:\inetpub\wwwroot\$SiteName"

    Write-Host "Ruta de publicación: $PublishPath" -ForegroundColor Cyan
    Write-Host "Ruta de destino IIS: $IISPath" -ForegroundColor Cyan

    # Verificar que los archivos se publicaron
    if (!(Test-Path $PublishPath)) {
        throw "La carpeta de publicación no existe: $PublishPath"
    }

    $publishedFiles = Get-ChildItem $PublishPath
    Write-Host "✓ Archivos publicados: $($publishedFiles.Count) elementos" -ForegroundColor Green

    # 3. Crear directorio en IIS si no existe
    if (!(Test-Path $IISPath)) {
        Write-Host "2. Creando directorio en IIS..." -ForegroundColor Yellow
        New-Item -ItemType Directory -Path $IISPath -Force | Out-Null
        Write-Host "✓ Directorio IIS creado: $IISPath" -ForegroundColor Green
    }

    # 4. Detener Application Pool si existe
    $existingPool = Get-IISAppPool -Name $AppPoolName -ErrorAction SilentlyContinue
    if ($existingPool) {
        Write-Host "3. Deteniendo Application Pool existente..." -ForegroundColor Yellow
        Stop-IISAppPool -Name $AppPoolName -ErrorAction SilentlyContinue
        Start-Sleep -Seconds 2
        Write-Host "✓ Application Pool detenido" -ForegroundColor Green
    }

    # 5. Copiar archivos publicados
    Write-Host "4. Copiando archivos a IIS..." -ForegroundColor Yellow
    Copy-Item -Path "$PublishPath\*" -Destination $IISPath -Recurse -Force
    Write-Host "✓ Archivos copiados correctamente" -ForegroundColor Green

    # 6. Crear Application Pool para .NET Core
    if (!(Get-IISAppPool -Name $AppPoolName -ErrorAction SilentlyContinue)) {
        Write-Host "5. Creando Application Pool..." -ForegroundColor Yellow
        New-IISAppPool -Name $AppPoolName -Force
        Write-Host "✓ Application Pool creado: $AppPoolName" -ForegroundColor Green
    }
    
    # Configurar Application Pool para .NET Core
    Write-Host "6. Configurando Application Pool..." -ForegroundColor Yellow
    Set-ItemProperty -Path "IIS:\AppPools\$AppPoolName" -Name processModel.identityType -Value ApplicationPoolIdentity
    Set-ItemProperty -Path "IIS:\AppPools\$AppPoolName" -Name managedRuntimeVersion -Value ""
    Set-ItemProperty -Path "IIS:\AppPools\$AppPoolName" -Name enable32BitAppOnWin64 -Value $false
    Write-Host "✓ Application Pool configurado" -ForegroundColor Green

    # 7. Crear o actualizar sitio web
    $existingSite = Get-IISSite -Name $SiteName -ErrorAction SilentlyContinue
    if ($existingSite) {
        Write-Host "7. Actualizando sitio existente..." -ForegroundColor Yellow
        Set-ItemProperty -Path "IIS:\Sites\$SiteName" -Name physicalPath -Value $IISPath
        Set-ItemProperty -Path "IIS:\Sites\$SiteName" -Name applicationPool -Value $AppPoolName
        Write-Host "✓ Sitio actualizado: $SiteName" -ForegroundColor Green
    } else {
        Write-Host "7. Creando nuevo sitio..." -ForegroundColor Yellow
        New-IISSite -Name $SiteName -PhysicalPath $IISPath -Port $Port -ApplicationPool $AppPoolName
        Write-Host "✓ Sitio creado: $SiteName en puerto $Port" -ForegroundColor Green
    }

    # 8. Configurar permisos
    Write-Host "8. Configurando permisos..." -ForegroundColor Yellow
    $acl = Get-Acl $IISPath
    $accessRule = New-Object System.Security.AccessControl.FileSystemAccessRule("IIS_IUSRS", "FullControl", "ContainerInherit,ObjectInherit", "None", "Allow")
    $acl.SetAccessRule($accessRule)
    $acl | Set-Acl $IISPath
    Write-Host "✓ Permisos configurados" -ForegroundColor Green

    # 9. Iniciar Application Pool y sitio
    Write-Host "9. Iniciando servicios..." -ForegroundColor Yellow
    Start-IISAppPool -Name $AppPoolName
    Start-IISSite -Name $SiteName
    Write-Host "✓ Servicios iniciados" -ForegroundColor Green

    # 10. Verificar estado
    Start-Sleep -Seconds 2
    $appPool = Get-IISAppPool -Name $AppPoolName
    $site = Get-IISSite -Name $SiteName

    Write-Host ""
    Write-Host "========================================" -ForegroundColor Green
    Write-Host "    PUBLICACIÓN COMPLETADA CON ÉXITO   " -ForegroundColor Green
    Write-Host "========================================" -ForegroundColor Green
    Write-Host ""
    Write-Host "🌐 URL: http://localhost:$Port" -ForegroundColor Cyan
    Write-Host "📁 Sitio: $SiteName" -ForegroundColor Cyan
    Write-Host "⚙️  App Pool: $AppPoolName (Estado: $($appPool.State))" -ForegroundColor Cyan
    Write-Host "📍 Estado del Sitio: $($site.State)" -ForegroundColor Cyan
    Write-Host "📂 Ubicación: $IISPath" -ForegroundColor Cyan
    Write-Host ""
    
    # Preguntar si abrir el navegador
    $response = Read-Host "¿Deseas abrir la aplicación en el navegador? (S/N)"
    if ($response -eq 'S' -or $response -eq 's' -or $response -eq 'Y' -or $response -eq 'y') {
        Write-Host "Abriendo navegador..." -ForegroundColor Yellow
        Start-Process "http://localhost:$Port"
    }

} catch {
    Write-Host ""
    Write-Host "❌ ERROR DURANTE LA PUBLICACIÓN" -ForegroundColor Red
    Write-Host "Detalle del error: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host ""
    Write-Host "🔧 PASOS PARA SOLUCIONAR:" -ForegroundColor Yellow
    Write-Host "1. Verifica que IIS esté instalado correctamente" -ForegroundColor White
    Write-Host "2. Ejecuta este script como Administrador" -ForegroundColor White
    Write-Host "3. Verifica que el puerto $Port no esté en uso" -ForegroundColor White
    Write-Host "4. Asegúrate de que ASP.NET Core Hosting Bundle esté instalado" -ForegroundColor White
    
    if ($IISPath) {
        $logsPath = Join-Path $IISPath "logs"
        Write-Host "5. Revisa los logs en: $logsPath" -ForegroundColor White
    }
    
    Write-Host ""
    Read-Host "Presiona Enter para salir"
    exit 1
}
