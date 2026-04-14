# 📋 Guía para Publicar RubricasApp en IIS Local

## 🎯 Requisitos Previos

### 1. IIS y ASP.NET Core Hosting Bundle
```bash
# Verificar si IIS está instalado
Get-WindowsOptionalFeature -Online -FeatureName IIS-WebServerRole

# Si no está instalado, habilitar IIS:
Enable-WindowsOptionalFeature -Online -FeatureName IIS-WebServerRole -All
Enable-WindowsOptionalFeature -Online -FeatureName IIS-WebServer -All
Enable-WindowsOptionalFeature -Online -FeatureName IIS-CommonHttpFeatures -All
Enable-WindowsOptionalFeature -Online -FeatureName IIS-HttpErrors -All
Enable-WindowsOptionalFeature -Online -FeatureName IIS-HttpLogging -All
Enable-WindowsOptionalFeature -Online -FeatureName IIS-AspNetCoreModuleV2 -All
```

### 2. Descargar ASP.NET Core Hosting Bundle
- Descarga desde: https://dotnet.microsoft.com/download/dotnet
- Instala el "ASP.NET Core Runtime" y "Hosting Bundle" para Windows

## 🚀 Métodos de Publicación

### Método 1: Script Automatizado (Recomendado)

#### Opción A: Desde VS Code
1. Abrir VS Code como **Administrador**
2. Ir a `Terminal > Run Task`
3. Seleccionar: **"Publicar en IIS Local (Completo)"**

#### Opción B: Desde PowerShell
```powershell
# Ejecutar PowerShell como Administrador
cd "d:\Fuentes_gitHub\RubricasApp.Web"

# Ejecutar script con parámetros predeterminados
.\publish-to-iis.ps1

# O con parámetros personalizados
.\publish-to-iis.ps1 -SiteName "MiRubricasApp" -Port "9090" -AppPoolName "MiAppPool"
```

### Método 2: Manual Paso a Paso

#### Paso 1: Publicar la Aplicación
```bash
dotnet publish --configuration Release --output "bin/Release/Publish" --self-contained false --runtime win-x64
```

#### Paso 2: Configurar IIS Manager
1. Abrir **IIS Manager** como administrador
2. **Crear Application Pool:**
   - Clic derecho en "Application Pools" > "Add Application Pool"
   - Name: `RubricasAppPool`
   - .NET CLR version: **No Managed Code**
   - Managed pipeline mode: `Integrated`

3. **Crear Website:**
   - Clic derecho en "Sites" > "Add Website"
   - Site name: `RubricasApp`
   - Application pool: `RubricasAppPool`
   - Physical path: `C:\inetpub\wwwroot\RubricasApp`
   - Port: `8080`

#### Paso 3: Copiar Archivos
```powershell
# Crear directorio si no existe
New-Item -ItemType Directory -Path "C:\inetpub\wwwroot\RubricasApp" -Force

# Copiar archivos publicados
Copy-Item -Path ".\bin\Release\Publish\*" -Destination "C:\inetpub\wwwroot\RubricasApp" -Recurse -Force
```

#### Paso 4: Configurar Permisos
```powershell
# Dar permisos a IIS_IUSRS
$path = "C:\inetpub\wwwroot\RubricasApp"
$acl = Get-Acl $path
$accessRule = New-Object System.Security.AccessControl.FileSystemAccessRule("IIS_IUSRS", "FullControl", "ContainerInherit,ObjectInherit", "None", "Allow")
$acl.SetAccessRule($accessRule)
$acl | Set-Acl $path
```

## 🔧 Configuración de Base de Datos

### SQLite (Configuración actual)
La base de datos SQLite se copiará automáticamente con la publicación. Asegúrate de que la cadena de conexión en `appsettings.json` sea correcta:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=RubricasDb.db"
  }
}
```

### SQL Server (Si decides cambiar)
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.\\SQLEXPRESS;Database=RubricasDb;Trusted_Connection=true;MultipleActiveResultSets=true;TrustServerCertificate=true"
  }
}
```

## 🌐 URLs de Acceso

Después de la publicación exitosa:
- **URL Local**: http://localhost:8080
- **URL de Red**: http://[TU-IP]:8080 (si configuras firewall)

## 🛠️ Solución de Problemas

### Error: "HTTP Error 500.31 - Failed to load ASP.NET Core runtime"
```bash
# Reinstalar ASP.NET Core Hosting Bundle
# Descargar desde: https://dotnet.microsoft.com/download/dotnet/8.0
```

### Error: "HTTP Error 403.14 - Forbidden"
```powershell
# Verificar permisos de IIS_IUSRS
icacls "C:\inetpub\wwwroot\RubricasApp" /grant "IIS_IUSRS:(OI)(CI)F"
```

### Error: Puerto en uso
```powershell
# Verificar qué proceso usa el puerto
netstat -ano | findstr :8080

# Cambiar puerto en IIS Manager o usar otro puerto
.\publish-to-iis.ps1 -Port "8081"
```

### Verificar Logs
```powershell
# Los logs se guardan en:
Get-Content "C:\inetpub\wwwroot\RubricasApp\logs\stdout*.log" -Tail 50
```

## 📝 Archivos Importantes

- `web.config` - Configuración de IIS
- `appsettings.json` - Configuración de la aplicación
- `publish-to-iis.ps1` - Script de publicación automatizada
- `RubricasDb.db` - Base de datos SQLite

## 🔄 Actualizar la Aplicación

Para actualizar una instalación existente:

```powershell
# Método rápido: Solo republicar
.\publish-to-iis.ps1

# Método manual: Detener > Copiar > Iniciar
Stop-IISAppPool -Name "RubricasAppPool"
Copy-Item -Path ".\bin\Release\Publish\*" -Destination "C:\inetpub\wwwroot\RubricasApp" -Recurse -Force
Start-IISAppPool -Name "RubricasAppPool"
```

## ✅ Verificación de Instalación

1. **Verificar servicios**:
   ```powershell
   Get-IISAppPool -Name "RubricasAppPool"
   Get-IISSite -Name "RubricasApp"
   ```

2. **Probar URL**: http://localhost:8080

3. **Verificar logs**: `C:\inetpub\wwwroot\RubricasApp\logs\`

---

## 📞 Comandos Útiles

```powershell
# Ver todos los sitios IIS
Get-IISSite

# Ver todos los Application Pools
Get-IISAppPool

# Reiniciar sitio específico
Restart-IISSite -Name "RubricasApp"

# Ver procesos en puerto específico
netstat -ano | findstr :8080
```
