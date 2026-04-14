# 📋 Última Versión de Publicación - RubricasApp IIS

## ✅ Estado Actual: **FUNCIONANDO CORRECTAMENTE**

### 🎯 Script Final Funcional: `deploy-clean.ps1`

**Fecha de última actualización:** 21 de Octubre, 2025  
**Estado:** ✅ Probado y Funcionando  
**Versión:** v1.0 - Estable  

---

## 🚀 Cómo Publicar (FUNCIONA 100%)

### **Método Recomendado:**
```powershell
# 1. Abrir PowerShell como ADMINISTRADOR
# 2. Navegar al directorio del proyecto
cd "d:\Fuentes_gitHub\RubricasApp.Web"

# 3. Compilar aplicación
dotnet publish --configuration Release --output "bin/Release/Publish"

# 4. Desplegar en IIS
.\deploy-clean.ps1 -SiteName "RubricasApp" -Port "8080"
```

---

## ✅ Verificación de Funcionamiento

### **Última Prueba Exitosa:**
- **Fecha:** 21/10/2025
- **Sitio:** RubricasApp
- **URL:** http://localhost:8080
- **Estado:** ✅ SITE "RubricasApp" (id:2,bindings:http/*:8080:,state:Started)
- **App Pool:** ✅ APPPOOL "RubricasAppPool" (MgdVersion:,MgdMode:Integrated,state:Started)

### **Archivos Verificados:**
- ✅ `RubricasApp.Web.exe` - Presente
- ✅ `web.config` - Configurado
- ✅ `wwwroot/` - Recursos estáticos
- ✅ Dependencias .NET Core - Todas presentes

---

## 📁 Archivos de Despliegue

### **✅ USAR ESTOS ARCHIVOS:**
- `deploy-clean.ps1` - **SCRIPT PRINCIPAL (FUNCIONA)**
- `web.config` - Configuración IIS
- `appsettings.json` - Configuración aplicación

### **❌ NO USAR (Con errores):**
- `deploy-iis.ps1` - Problemas de codificación
- `deploy-simple.ps1` - Cmdlets no disponibles  
- `deploy-final.ps1` - Errores de sintaxis
- `publish-to-iis.ps1` - Estructura try-catch rota
- `publish-to-iis-simple.ps1` - Múltiples errores

---

## 🔧 Configuración Actual IIS

```
Sitio Web: RubricasApp
Puerto: 8080
Application Pool: RubricasAppPool
Ubicación Física: C:\inetpub\wwwroot\RubricasApp
Estado: Started (Funcionando)
Modo: Integrated
Runtime: .NET Core (No Managed Code)
Identidad: ApplicationPoolIdentity
```

---

## 🛠️ Características del Script Funcional

### **deploy-clean.ps1 incluye:**
- ✅ Verificación de privilegios de administrador
- ✅ Validación de archivos publicados
- ✅ Uso de `appcmd.exe` (nativo de IIS)
- ✅ Configuración automática de permisos IIS_IUSRS
- ✅ Manejo de errores robusto
- ✅ Creación automática de Application Pool
- ✅ Configuración para .NET Core
- ✅ Inicio automático de servicios

---

## 🌐 URLs de Acceso

- **Aplicación Principal:** http://localhost:8080
- **IIS Manager:** Ejecutar `inetmgr` como Administrador
- **Logs IIS:** `C:\inetpub\logs\LogFiles\`
- **Archivos App:** `C:\inetpub\wwwroot\RubricasApp\`

---

## 🎉 Proceso de Actualización

### **Para actualizar la aplicación:**
```powershell
# 1. Compilar nueva versión
dotnet publish --configuration Release --output "bin/Release/Publish"

# 2. Redesplegar (automáticamente detiene y reinicia)
.\deploy-clean.ps1
```

### **Para cambiar configuración:**
```powershell
# Cambiar puerto
.\deploy-clean.ps1 -Port "9090"

# Cambiar nombre del sitio
.\deploy-clean.ps1 -SiteName "MiRubrica" -Port "8080"
```

---

## 📞 Comandos de Verificación

```powershell
# Ver estado del sitio
& "C:\Windows\System32\inetsrv\appcmd.exe" list site RubricasApp

# Ver estado del Application Pool  
& "C:\Windows\System32\inetsrv\appcmd.exe" list apppool RubricasAppPool

# Ver todos los sitios
& "C:\Windows\System32\inetsrv\appcmd.exe" list sites

# Verificar puerto en uso
netstat -ano | findstr :8080
```

---

## ⚠️ Notas Importantes

1. **Siempre ejecutar PowerShell como Administrador**
2. **IIS debe estar instalado y habilitado**  
3. **ASP.NET Core Hosting Bundle requerido**
4. **El puerto 8080 debe estar libre**
5. **Base de datos SQLite se copia automáticamente**

---

**✅ Esta configuración está probada y funcionando correctamente al 21/10/2025**
