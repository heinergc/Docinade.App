# 🔧 Guía de Solución de Errores de Autorización en ASP.NET Core

## 📋 Problema Común
```
InvalidOperationException: The AuthorizationPolicy named: 'nombre_politica' was not found.
```

## 🎯 Pasos para Diagnosticar y Solucionar

### 1. 🔍 **IDENTIFICAR EL ERROR**

#### 1.1 Leer el mensaje de error completo
- Buscar el nombre exacto de la política que falta
- Identificar en qué controlador/acción ocurre
- Revisar el stack trace para ubicar el origen

#### 1.2 Buscar dónde se usa la política
```bash
# Buscar en todo el proyecto
grep -r "nombre_politica" .
# O en Windows PowerShell:
Select-String -Path "." -Pattern "nombre_politica" -Recurse
```

### 2. 🗂️ **VERIFICAR ARQUITECTURA DE AUTORIZACIÓN**

#### 2.1 Revisar la estructura de archivos
```
Authorization/
├── PermissionPolicyProvider.cs       # Proveedor dinámico de políticas
├── PermissionAuthorizationHandler.cs # Manejador de autorización
└── PermissionRequirement.cs         # Requerimiento (puede estar en el handler)

Configuration/
└── AuthorizationExtensions.cs       # Configuración de autorización

Models/Permissions/
├── ApplicationPermissions.cs        # Definición de todos los permisos
├── ApplicationRoles.cs              # Definición de roles
└── PermissionInfo.cs                # Clase de información de permisos

Services/Permissions/
├── IPermissionService.cs            # Interfaz del servicio
└── PermissionService.cs             # Implementación del servicio
```

### 3. 🔧 **VERIFICAR COMPONENTES CLAVE**

#### 3.1 PermissionPolicyProvider.cs
**Revisar método `IsPermissionPolicy`:**
```csharp
private bool IsPermissionPolicy(string policyName)
{
    // Verificar formato: "modulo.accion"
    if (string.IsNullOrEmpty(policyName) || !policyName.Contains('.')) 
        return false;
    
    var parts = policyName.Split('.');
    if (parts.Length != 2) return false;
    
    var module = parts[0].ToLower();
    var action = parts[1].ToUpper();
    
    // VERIFICAR QUE ESTOS ARRAYS ESTÉN COMPLETOS
    var validModules = new[] { /* todos los módulos */ };
    var validActions = new[] { /* todas las acciones */ };
    
    return validModules.Contains(module) && validActions.Contains(action);
}
```

**Lista de verificación:**
- [ ] ¿Está el módulo en `validModules`?
- [ ] ¿Está la acción en `validActions`?
- [ ] ¿El formato es correcto (minúscula.MAYÚSCULA)?

#### 3.2 ApplicationPermissions.cs
**Verificar que el permiso esté definido:**
```csharp
public static class Configuracion
{
    public const string GESTIONAR_PERMISOS = "configuracion.gestionar_permisos";
    // ¿Existe esta línea?
}
```

**Verificar que esté en el diccionario de permisos:**
```csharp
[CONFIGURACION] = new List<PermissionInfo>
{
    // ¿Está incluido aquí?
    new(Configuracion.GESTIONAR_PERMISOS, "Gestionar permisos", "Descripción"),
}
```

#### 3.3 AuthorizationExtensions.cs
**Verificar registro de servicios:**
```csharp
public static IServiceCollection AddCustomAuthorization(this IServiceCollection services)
{
    // ¿Están registrados correctamente?
    services.AddScoped<IPermissionService, PermissionService>();
    services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
    services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
    
    services.AddAuthorization(options =>
    {
        // ¿Está la política explícita?
        options.AddPolicy("configuracion.gestionar_permisos", policy =>
            policy.Requirements.Add(new PermissionRequirement("configuracion.gestionar_permisos")));
    });
}
```

### 4. 🛠️ **SOLUCIÓN PASO A PASO**

#### 4.1 Agregar la acción faltante al PermissionPolicyProvider
```csharp
// En PermissionPolicyProvider.cs
var validActions = new[] 
{
    "VER", "CREAR", "EDITAR", "ELIMINAR", "GESTIONAR",
    "GESTIONAR_PERMISOS", // ← AGREGAR AQUÍ
    // ... otras acciones
};
```

#### 4.2 Agregar política explícita (si es necesaria)
```csharp
// En AuthorizationExtensions.cs
options.AddPolicy("configuracion.gestionar_permisos", policy =>
    policy.Requirements.Add(new PermissionRequirement("configuracion.gestionar_permisos")));
```

#### 4.3 Verificar que el permiso esté en ApplicationPermissions
```csharp
// En ApplicationPermissions.cs
public static class Configuracion
{
    public const string GESTIONAR_PERMISOS = "configuracion.gestionar_permisos";
}

// Y en el diccionario:
[CONFIGURACION] = new List<PermissionInfo>
{
    new(Configuracion.GESTIONAR_PERMISOS, "Gestionar permisos", "Permite gestionar permisos del sistema"),
}
```

#### 4.4 Verificar Program.cs
```csharp
// Asegurarse de que esté registrado:
builder.Services.AddCustomAuthorization();

// Y que se inicialice correctamente:
var permissionService = services.GetRequiredService<IPermissionService>();
await permissionService.InitializeDefaultRolesAndPermissionsAsync();
```

### 5. 🧪 **VERIFICACIÓN Y TESTING**

#### 5.1 Compilar y ejecutar
```bash
dotnet build
dotnet run
```

#### 5.2 Verificar en logs
Buscar mensajes como:
```
✅ Autorización inicializada: roles y permisos configurados correctamente
✅ Configuración de autorización validada correctamente
```

#### 5.3 Test de la política
```csharp
// En un controlador de prueba:
[Authorize("configuracion.gestionar_permisos")]
public IActionResult TestAction()
{
    return Ok("Política funciona correctamente");
}
```

### 6. 🔍 **DEBUGGING AVANZADO**

#### 6.1 Habilitar logging detallado
```csharp
// En Program.cs o appsettings.json
builder.Logging.AddFilter("RubricasApp.Web.Authorization", LogLevel.Debug);
```

#### 6.2 Verificar en runtime
```csharp
// En PermissionPolicyProvider.cs
public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
{
    _logger?.LogDebug($"Solicitando política: {policyName}");
    
    if (IsPermissionPolicy(policyName))
    {
        _logger?.LogDebug($"✅ Política válida: {policyName}");
        // crear política...
    }
    else
    {
        _logger?.LogWarning($"❌ Política inválida: {policyName}");
    }
}
```

### 7. 📝 **CHECKLIST DE VERIFICACIÓN FINAL**

- [ ] **PermissionPolicyProvider**: Acción está en `validActions`
- [ ] **ApplicationPermissions**: Permiso definido como constante
- [ ] **ApplicationPermissions**: Permiso incluido en diccionario de categorías
- [ ] **AuthorizationExtensions**: Servicio registrado correctamente
- [ ] **AuthorizationExtensions**: Política explícita agregada (si es necesaria)
- [ ] **Program.cs**: `AddCustomAuthorization()` llamado
- [ ] **Program.cs**: Permisos inicializados correctamente
- [ ] **Compilación**: Sin errores de compilación
- [ ] **Runtime**: Logs confirman inicialización exitosa

### 8. 🚨 **ERRORES COMUNES Y SOLUCIONES**

#### Error: "IPermissionService not registered"
**Solución:**
```csharp
// En AuthorizationExtensions.cs
services.AddScoped<IPermissionService, PermissionService>();
```

#### Error: "PermissionAuthorizationHandler not registered"
**Solución:**
```csharp
// En AuthorizationExtensions.cs
services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
```

#### Error: Política encontrada pero usuario no autorizado
**Verificar:**
1. Usuario tiene el rol correcto
2. Rol tiene el permiso asignado
3. Claims están correctamente configurados

#### Error: "Circular dependency"
**Solución:**
```csharp
// Cambiar IAuthorizationPolicyProvider de Scoped a Singleton
services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
```

### 9. 🔧 **HERRAMIENTAS DE AYUDA**

#### 9.1 Script de verificación de permisos
```csharp
public async Task<IActionResult> VerifyPermissions()
{
    var allPermissions = ApplicationPermissions.GetAllPermissions();
    var invalidPolicies = new List<string>();
    
    foreach (var permission in allPermissions)
    {
        var policy = await _policyProvider.GetPolicyAsync(permission);
        if (policy == null)
            invalidPolicies.Add(permission);
    }
    
    return Json(new { InvalidPolicies = invalidPolicies });
}
```

#### 9.2 Comando para buscar referencias
```bash
# PowerShell
Get-ChildItem -Recurse -Include "*.cs" | Select-String "Authorize.*gestionar_permisos"

# CMD
findstr /s /i "gestionar_permisos" *.cs
```

### 10. 📚 **RECURSOS ADICIONALES**

- [Documentación de ASP.NET Core Authorization](https://docs.microsoft.com/en-us/aspnet/core/security/authorization/)
- [Custom Authorization Policy Providers](https://docs.microsoft.com/en-us/aspnet/core/security/authorization/iauthorizationpolicyprovider)
- [Claims-based Authorization](https://docs.microsoft.com/en-us/aspnet/core/security/authorization/claims)

---

## 💡 **CONSEJOS FINALES**

1. **Mantén consistencia** en la nomenclatura de permisos
2. **Documenta** todos los permisos nuevos que agregues
3. **Usa logging** para debugging de políticas
4. **Verifica** que todos los módulos y acciones estén en las listas válidas
5. **Testea** cada nuevo permiso después de agregarlo

---

**Creado:** $(Get-Date)  
**Versión:** 1.0  
**Proyecto:** RubricasApp.Web