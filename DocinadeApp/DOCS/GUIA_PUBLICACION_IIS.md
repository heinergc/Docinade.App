# 🚀 Guía Rápida - Publicar RubricasApp en IIS Local

## ✅ Script Funcional Creado: `deploy-iis.ps1`

### 🎯 Opciones de Publicación

#### **Opción 1: Todo en Uno (Recomendado)**
```powershell
# Desde VS Code:
Ctrl+Shift+P → "Run Task" → "Publicar + Desplegar IIS (Todo en uno)"
```

#### **Opción 2: Paso a Paso**
```powershell
# 1. Compilar aplicación
Ctrl+Shift+P → "Run Task" → "Publicar para IIS"

# 2. Desplegar en IIS (como Administrador)
.\deploy-iis.ps1
```

#### **Opción 3: PowerShell Directo**
```powershell
# Como Administrador
.\deploy-iis.ps1 -SiteName "MiApp" -Port "8080"
```

### 📋 Requisitos Previos
- ✅ VS Code ejecutado como **Administrador**
- ✅ IIS habilitado en Windows
- ✅ ASP.NET Core Hosting Bundle instalado

### 🔧 Instalar IIS (si no está instalado)
```powershell
# Como Administrador
Enable-WindowsOptionalFeature -Online -FeatureName IIS-WebServerRole -All
Enable-WindowsOptionalFeature -Online -FeatureName IIS-AspNetCoreModuleV2 -All
```

### 🌐 URLs por Defecto
- **Aplicación**: http://localhost:8080
- **IIS Manager**: Buscar "IIS" en el menú inicio

### 🛠️ Solución de Problemas

#### Error: "No se puede cargar WebAdministration"
```powershell
# Instalar IIS con PowerShell
Enable-WindowsOptionalFeature -Online -FeatureName IIS-WebServerRole -All
```

#### Error: "Puerto en uso"
```powershell
# Usar otro puerto
.\deploy-iis.ps1 -Port "9090"

# O verificar qué usa el puerto
netstat -ano | findstr :8080
```

#### Error: HTTP 500.31
- Descargar e instalar [ASP.NET Core Hosting Bundle](https://dotnet.microsoft.com/download/dotnet)

### 📝 Archivos Importantes
- ✅ `deploy-iis.ps1` - Script funcional de despliegue
- ✅ `web.config` - Configuración IIS
- ✅ `appsettings.json` - Configuración aplicación
- ❌ `publish-to-iis.ps1` - (Con errores, no usar)
- ❌ `publish-to-iis-simple.ps1` - (Con errores, no usar)

### 🎉 ¡Listo para Usar!

El script `deploy-iis.ps1` está completamente funcional y probado. 
Usa las tareas de VS Code para publicar fácilmente en IIS.

---
**Nota**: Recuerda siempre ejecutar VS Code como Administrador cuando publiques en IIS.
