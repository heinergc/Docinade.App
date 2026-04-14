# Script simplificado para publicar RubricasApp en IIS Local
# Ejecutar como Administrador
# Módulos usados: WebAdministration (evita mezclar con IISAdministration)
# Guardar como: Publish-RubricasApp.ps1

[CmdletBinding()]
param(
    [string]$SiteName = "RubricasApp",
    [string]$Port = "8081",
    [string]$AppPoolName = "RubricasAppPool"
)

Write-Host "=== TEST: Publicando RubricasApp en IIS Local ===" -ForegroundColor Green
Write-Host "Sitio: $SiteName" -ForegroundColor Cyan
Write-Host "Puerto: $Port" -ForegroundColor Cyan
Write-Host "App Pool: $AppPoolName" -ForegroundColor Cyan

# 0) Verificar si se ejecuta como administrador
$principal = New-Object Security.Principal.WindowsPrincipal([Security.Principal.WindowsIdentity]::GetCurrent())
if (-not $principal.IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)) {
    Write-Error "Este script debe ejecutarse como Administrador"
    Read-Host "Presiona Enter para salir"
    exit 1
}

# 1) Verificar e importar WebAdministration (NO mezclar con IISAdministration)
try {
    Import-Module WebAdministration -ErrorAction Stop
    Write-Host "Módulo WebAdministration cargado correctamente" -ForegroundColor Green
} catch {
    Write-Error "No se puede cargar el módulo WebAdministration. IIS no está instalado o configurado."
    Write-Host "Para instalar IIS, ejecuta como Administrador:"
    Write-Host 'dism /online /enable-feature /featurename:IIS-WebServerRole /all' -ForegroundColor Yellow
    Write-Host "y el Hosting Bundle de .NET Core (si falta)."
    Read-Host "Presiona Enter para salir"
    exit 1
}

try {
    # 2) Compilar y publicar la aplicación
    Write-Host "1. Compilando y publicando la aplicación..." -ForegroundColor Yellow
    $publishOut = Join-Path -Path (Get-Location).Path -ChildPath "bin\Release\Publish"
    if (!(Test-Path $publishOut)) { New-Item -ItemType Directory -Path $publishOut -Force | Out-Null }

    dotnet publish --configuration Release --output "$publishOut" --self-contained false --runtime win-x64
    if ($LASTEXITCODE -ne 0) {
        throw "Error al publicar la aplicación. Código de salida: $LASTEXITCODE"
    }

    Write-Host "Aplicación publicada correctamente" -ForegroundColor Green

    # 3) Rutas
    $PublishPath = $publishOut
    $IISPath     = "C:\inetpub\wwwroot\$SiteName"

    Write-Host "Ruta de publicación: $PublishPath" -ForegroundColor Cyan
    Write-Host "Ruta de destino IIS: $IISPath" -ForegroundColor Cyan

    if (!(Test-Path $PublishPath)) { throw "La carpeta de publicación no existe: $PublishPath" }

    $publishedFiles = Get-ChildItem -LiteralPath $PublishPath -Recurse -Force -ErrorAction SilentlyContinue
    Write-Host "Archivos publicados: $($publishedFiles.Count) elementos" -ForegroundColor Green

    # 4) Crear directorio destino si no existe
    if (!(Test-Path $IISPath)) {
        Write-Host "2. Creando directorio en IIS..." -ForegroundColor Yellow
        New-Item -ItemType Directory -Path $IISPath -Force | Out-Null
        Write-Host "Directorio IIS creado: $IISPath" -ForegroundColor Green
    }

    # 5) Detener sitio si existe para evitar locks de archivos
    $siteExists = Test-Path "IIS:\Sites\$SiteName"
    if ($siteExists) {
        Write-Host "3. Deteniendo sitio existente (si está iniciado)..." -ForegroundColor Yellow
        Stop-Website -Name $SiteName -ErrorAction SilentlyContinue
    }

    # 6) Detener App Pool si existe
    $poolExists = Test-Path "IIS:\AppPools\$AppPoolName"
    if ($poolExists) {
        Write-Host "4. Deteniendo Application Pool existente..." -ForegroundColor Yellow
        Stop-WebAppPool -Name $AppPoolName -ErrorAction SilentlyContinue
        Start-Sleep -Seconds 2
        Write-Host "Application Pool detenido" -ForegroundColor Green
    }

    # 7) Copiar archivos publicados a IIS (sin borrar otros: usa -Recurse -Force)
    Write-Host "5. Copiando archivos a IIS..." -ForegroundColor Yellow
    Copy-Item -Path (Join-Path $PublishPath '*') -Destination $IISPath -Recurse -Force
    Write-Host "Archivos copiados correctamente" -ForegroundColor Green

    # 8) Crear App Pool si no existe (WebAdministration)
    if (-not $poolExists) {
        Write-Host "6. Creando Application Pool..." -ForegroundColor Yellow
        New-WebAppPool -Name $AppPoolName | Out-Null
        Write-Host "Application Pool creado: $AppPoolName" -ForegroundColor Green
    }

    # Configurar App Pool (No CLR para .NET Core)
    Write-Host "7. Configurando Application Pool..." -ForegroundColor Yellow
    Set-ItemProperty -Path "IIS:\AppPools\$AppPoolName" -Name processModel.identityType -Value ApplicationPoolIdentity
    Set-ItemProperty -Path "IIS:\AppPools\$AppPoolName" -Name managedRuntimeVersion -Value ""
    Set-ItemProperty -Path "IIS:\AppPools\$AppPoolName" -Name enable32BitAppOnWin64 -Value $false
    Write-Host "Application Pool configurado" -ForegroundColor Green

    # 9) Crear o actualizar sitio (WebAdministration: Get-Website/New-Website/Set-ItemProperty)
    if ($siteExists) {
        Write-Host "8. Actualizando sitio existente..." -ForegroundColor Yellow
        Set-ItemProperty -Path "IIS:\Sites\$SiteName" -Name physicalPath -Value $IISPath
        Set-ItemProperty -Path "IIS:\Sites\$SiteName" -Name applicationPool -Value $AppPoolName

        # Actualizar binding http en el puerto solicitado
        $site = Get-Website -Name $SiteName
        $bindingInfos = $site.Bindings.Collection | ForEach-Object { $_.bindingInformation }
        # Usar patrón con subexpresión para evitar el error de ':'
        $hasPort = ($bindingInfos -match ":$($Port):").Count -gt 0
        if (-not $hasPort) {
            # Eliminar bindings http anteriores y dejar solo el puerto solicitado
            $site.Bindings.Collection | ForEach-Object {
                if ($_.protocol -eq 'http') {
                    Remove-WebBinding -Name $SiteName -BindingInformation $_.bindingInformation -Protocol http -ErrorAction SilentlyContinue
                }
            }
            New-WebBinding -Name $SiteName -Protocol http -Port ([int]$Port) -IPAddress "*" -HostHeader ""
        }
        Write-Host "Sitio actualizado: $SiteName" -ForegroundColor Green
    } else {
        Write-Host "8. Creando nuevo sitio..." -ForegroundColor Yellow
        New-Website -Name $SiteName -PhysicalPath $IISPath -Port ([int]$Port) -ApplicationPool $AppPoolName | Out-Null
        Write-Host "Sitio creado: $SiteName en puerto $Port" -ForegroundColor Green
    }

    # 10) Permisos (agregar sin sobrescribir reglas existentes)
    Write-Host "9. Configurando permisos..." -ForegroundColor Yellow
    $acl = Get-Acl -LiteralPath $IISPath
    $rule = New-Object System.Security.AccessControl.FileSystemAccessRule("IIS_IUSRS","Modify","ContainerInherit,ObjectInherit","None","Allow")
    $acl.AddAccessRule($rule) | Out-Null
    Set-Acl -LiteralPath $IISPath -AclObject $acl
    Write-Host "Permisos configurados" -ForegroundColor Green

    # 11) Iniciar App Pool y sitio
    Write-Host "10. Iniciando servicios..." -ForegroundColor Yellow
    Start-WebAppPool -Name $AppPoolName
    Start-Website -Name $SiteName
    Write-Host "Servicios iniciados" -ForegroundColor Green

    # 12) Verificar estado
    Start-Sleep -Seconds 2
    $appPoolState = (Get-WebAppPoolState -Name $AppPoolName).Value
    $siteState    = (Get-Website -Name $SiteName).State

    Write-Host ""
    Write-Host "========================================" -ForegroundColor Green
    Write-Host "    PUBLICACIÓN COMPLETADA CON ÉXITO   " -ForegroundColor Green
    Write-Host "========================================" -ForegroundColor Green
    Write-Host ""
    Write-Host "URL: http://localhost:$Port" -ForegroundColor Cyan
    Write-Host "Sitio: $SiteName" -ForegroundColor Cyan
    Write-Host "App Pool: $AppPoolName (Estado: $appPoolState)" -ForegroundColor Cyan
    Write-Host "Estado del Sitio: $siteState" -ForegroundColor Cyan
    Write-Host "Ubicación: $IISPath" -ForegroundColor Cyan
    Write-Host ""

    # 13) Abrir navegador (opcional)
    $response = Read-Host "¿Deseas abrir la aplicación en el navegador? (S/N)"
    if ($response -match '^[sSyY]$') {
        Write-Host "Abriendo navegador..." -ForegroundColor Yellow
        Start-Process "http://localhost:$Port"
    }

} catch {
    Write-Host ""
    Write-Host "ERROR DURANTE LA PUBLICACIÓN" -ForegroundColor Red
    Write-Host "Detalle del error: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host ""
    Write-Host "PASOS PARA SOLUCIONAR:" -ForegroundColor Yellow
    Write-Host "1. Verifica que IIS esté instalado correctamente" -ForegroundColor White
    Write-Host "2. Ejecuta este script como Administrador" -ForegroundColor White
    Write-Host "3. Verifica que el puerto $Port no esté en uso (netstat -ano | findstr :$Port)" -ForegroundColor White
    Write-Host "4. Asegúrate de que ASP.NET Core Hosting Bundle esté instalado" -ForegroundColor White

    if ($IISPath) {
        $logsPath = Join-Path $IISPath "logs"
        Write-Host "5. Revisa los logs en: $logsPath" -ForegroundColor White
    }

    Write-Host ""
    Read-Host "Presiona Enter para salir"
    exit 1
}
