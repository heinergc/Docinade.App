# Configuración de Variables de Entorno - Producción

## ✅ Archivos de Configuración Actualizados

### 1. `web.config` (IIS)
```xml
<environmentVariable name="ASPNETCORE_ENVIRONMENT" value="Production" />
```
✅ **Ya configurado** - Listo para IIS

### 2. `launchSettings.json`
Agregado nuevo perfil: **"RubricasApp.Web (Production)"**
- ASPNETCORE_ENVIRONMENT = Production
- URL: http://localhost:5000

### 3. `appsettings.Production.json`
✅ **Ya configurado** con connection string de Azure:
```
Server=tcp:serverrubricasapp.database.windows.net,1433;...
```

---

## 🚀 Opciones para Ejecutar en Modo Producción

### Opción 1: Script Automatizado (Recomendado)

```powershell
.\run-production.ps1
```

**Esto hará:**
- ✅ Establecer ASPNETCORE_ENVIRONMENT=Production
- ✅ Compilar en modo Release
- ✅ Ejecutar aplicación en http://localhost:5000
- ✅ Usar appsettings.Production.json

---

### Opción 2: Manual con dotnet CLI

```powershell
# Establecer variable de entorno
$env:ASPNETCORE_ENVIRONMENT = "Production"

# Ejecutar aplicación
dotnet run --configuration Release --urls "http://localhost:5000"
```

---

### Opción 3: Visual Studio

1. En la barra superior, seleccionar perfil: **"RubricasApp.Web (Production)"**
2. Click en ▶️ Run

---

### Opción 4: Variable de Entorno del Sistema (Permanente)

**Ejecutar como Administrador:**

```powershell
.\set-production-env.ps1
```

**Esto configurará:**
- ✅ ASPNETCORE_ENVIRONMENT=Production (nivel sistema)
- ✅ ConnectionStrings__DefaultConnection (opcional)
- ✅ PORT (opcional)
- ✅ Reiniciar IIS automáticamente

**Ventajas:**
- La variable persiste después de reiniciar
- Se aplica a IIS y servicios Windows
- No necesita configuración adicional

**Verificar configuración:**
```powershell
[System.Environment]::GetEnvironmentVariable('ASPNETCORE_ENVIRONMENT', 'Machine')
```

---

## 🔐 Configuración para Diferentes Entornos

### Desarrollo (Local)
```powershell
$env:ASPNETCORE_ENVIRONMENT = "Development"
dotnet run
```
**Usa:** `appsettings.json` + `appsettings.Development.json`

### Producción (Local)
```powershell
$env:ASPNETCORE_ENVIRONMENT = "Production"
dotnet run --configuration Release
```
**Usa:** `appsettings.json` + `appsettings.Production.json`

### Producción (IIS)
**Usa:** `web.config` (ya configurado con Production)

### Producción (Azure)
**Usa:** Variables de entorno de Azure App Service
```bash
az webapp config appsettings set \
  --resource-group RubricasAppRG \
  --name rubricasapp \
  --settings ASPNETCORE_ENVIRONMENT="Production"
```

---

## 📋 Verificar Ambiente Actual

### Opción 1: Desde PowerShell
```powershell
$env:ASPNETCORE_ENVIRONMENT
```

### Opción 2: Desde la aplicación
La aplicación muestra en los logs:
```
[INFO] Ambiente: Production
[INFO] Configurado para SQL Server: Server=tcp:serverrubricasapp...
```

### Opción 3: En el navegador (Development mode only)
En modo Development, verás el banner de desarrollo.
En Production, no habrá banner.

---

## ⚙️ Configuraciones por Ambiente

| Configuración | Development | Production |
|--------------|-------------|------------|
| Connection String | LocalDB/SQLEXPRESS | Azure SQL |
| Logging Level | Information | Warning |
| Detailed Errors | Enabled | Disabled |
| Empadronamiento Público | Enabled | Disabled |
| HTTPS Redirect | Optional | Required |
| Sensitive Data Logging | Enabled | Disabled |

---

## 🔧 Solución de Problemas

### Error: "appsettings.Production.json not found"
```powershell
# Verificar que el archivo existe
Test-Path "appsettings.Production.json"

# Si no existe, créalo desde template
Copy-Item "appsettings.json" "appsettings.Production.json"
```

### Error: "Variable de entorno no se aplica"
```powershell
# Cerrar TODAS las ventanas de PowerShell/CMD
# Abrir nueva ventana y verificar
$env:ASPNETCORE_ENVIRONMENT

# Si persiste, reiniciar el sistema
Restart-Computer
```

### Error: "Connection string not found"
Verificar en orden:
1. Variable de entorno: `ConnectionStrings__DefaultConnection`
2. `appsettings.Production.json` → `ConnectionStrings:DefaultConnection`
3. `appsettings.json` → `ConnectionStrings:DefaultConnection`

---

## 📊 Checklist de Producción

- [x] `web.config` con ASPNETCORE_ENVIRONMENT=Production
- [x] `appsettings.Production.json` con Azure connection string
- [x] `launchSettings.json` con perfil de producción
- [ ] Variable de entorno del sistema configurada (opcional)
- [ ] Connection string de producción probado
- [ ] Logs configurados en nivel Warning
- [ ] HTTPS habilitado
- [ ] Contraseñas de admin cambiadas
- [ ] Empadronamiento público deshabilitado

---

## 🚦 Scripts Disponibles

| Script | Propósito | Permisos |
|--------|-----------|----------|
| `run-production.ps1` | Ejecutar localmente en modo producción | Usuario |
| `set-production-env.ps1` | Configurar variables del sistema | Administrador |
| `deploy-azure.ps1` | Despliegue completo a Azure | Usuario (Azure CLI) |
| `publish-azure.ps1` | Actualización rápida en Azure | Usuario (Azure CLI) |

---

## 📚 Referencias

- **ASP.NET Core Environments:** https://docs.microsoft.com/aspnet/core/fundamentals/environments
- **Configuration in ASP.NET Core:** https://docs.microsoft.com/aspnet/core/fundamentals/configuration/
- **Deploy to IIS:** https://docs.microsoft.com/aspnet/core/host-and-deploy/iis/
