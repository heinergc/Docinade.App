# Implementación de Permisos - Instrumentos de Evaluación

## 📄 Resumen

Este documento detalla la implementación del sistema de permisos granular para el módulo de **Instrumentos de Evaluación** en RubricasApp.Web.

**Fecha de implementación:** 2025-01-17  
**Módulo:** InstrumentosEvaluacionController  
**Categoría:** Instrumentos de Evaluación

---

## 🛡️ Permisos Implementados

### Categoría: `INSTRUMENTOS_EVALUACION = "Instrumentos de Evaluación"`

| Permiso | Constante | Descripción |
|---------|-----------|-------------|
| `instrumentos_evaluacion.ver` | `VER` | Ver instrumentos de evaluación |
| `instrumentos_evaluacion.crear` | `CREAR` | Crear nuevos instrumentos |
| `instrumentos_evaluacion.editar` | `EDITAR` | Editar instrumentos existentes |
| `instrumentos_evaluacion.eliminar` | `ELIMINAR` | Eliminar instrumentos |
| `instrumentos_evaluacion.duplicar` | `DUPLICAR` | Duplicar instrumentos |
| `instrumentos_evaluacion.asignar_materias` | `ASIGNAR_MATERIAS` | Asignar instrumentos a materias |
| `instrumentos_evaluacion.activar_desactivar` | `ACTIVAR_DESACTIVAR` | Activar/desactivar instrumentos |
| `instrumentos_evaluacion.gestionar_configuracion` | `GESTIONAR_CONFIGURACION` | Gestionar configuración |
| `instrumentos_evaluacion.exportar` | `EXPORTAR` | Exportar instrumentos |
| `instrumentos_evaluacion.importar` | `IMPORTAR` | Importar instrumentos |
| `instrumentos_evaluacion.ver_estadisticas` | `VER_ESTADISTICAS` | Ver estadísticas de uso |
| `instrumentos_evaluacion.publicar` | `PUBLICAR` | Publicar instrumentos |

---

## 🔗 Mapeo de Acciones del Controlador

| Acción del Controlador | Método HTTP | Permiso Requerido |
|------------------------|-------------|-------------------|
| `Index()` | GET | `instrumentos_evaluacion.ver` |
| `Details(id)` | GET | `instrumentos_evaluacion.ver` |
| `Create()` | GET | `instrumentos_evaluacion.crear` |
| `Create(modelo)` | POST | `instrumentos_evaluacion.crear` |
| `Edit(id)` | GET | `instrumentos_evaluacion.editar` |
| `Edit(id, modelo)` | POST | `instrumentos_evaluacion.editar` |
| `Delete(id)` | GET | `instrumentos_evaluacion.eliminar` |
| `DeleteConfirmed(id)` | POST | `instrumentos_evaluacion.eliminar` |

---

## 👥 Perfiles de Usuario y Casos de Uso

### 🔧 Administrador del Sistema
**Permisos:** TODOS (12 permisos)
```
instrumentos_evaluacion.ver, instrumentos_evaluacion.crear, instrumentos_evaluacion.editar,
instrumentos_evaluacion.eliminar, instrumentos_evaluacion.duplicar, instrumentos_evaluacion.asignar_materias,
instrumentos_evaluacion.activar_desactivar, instrumentos_evaluacion.gestionar_configuracion,
instrumentos_evaluacion.exportar, instrumentos_evaluacion.importar, instrumentos_evaluacion.ver_estadisticas,
instrumentos_evaluacion.publicar
```
**Casos de uso:**
- Gestión completa de instrumentos de evaluación
- Configuración avanzada del sistema
- Administración de asignaciones y publicaciones
- Control total de estadísticas e importación/exportación

### 👨‍🏫 Coordinador Académico
**Permisos:** VER, CREAR, EDITAR, DUPLICAR, ASIGNAR_MATERIAS, VER_ESTADISTICAS, PUBLICAR (7 permisos)
```
instrumentos_evaluacion.ver, instrumentos_evaluacion.crear, instrumentos_evaluacion.editar,
instrumentos_evaluacion.duplicar, instrumentos_evaluacion.asignar_materias, 
instrumentos_evaluacion.ver_estadisticas, instrumentos_evaluacion.publicar
```
**Casos de uso:**
- Crear instrumentos para su área académica
- Asignar instrumentos a materias de su coordinación
- Ver estadísticas de uso en su área
- Publicar instrumentos para uso de profesores

### 👩‍🏫 Profesor
**Permisos:** VER, DUPLICAR (2 permisos)
```
instrumentos_evaluacion.ver, instrumentos_evaluacion.duplicar
```
**Casos de uso:**
- Ver instrumentos disponibles para sus materias
- Duplicar instrumentos para personalizar evaluaciones

### 👀 Observador
**Permisos:** VER (1 permiso)
```
instrumentos_evaluacion.ver
```
**Casos de uso:**
- Consultar catálogo de instrumentos disponibles
- Ver detalles de instrumentos publicados

---

## 📁 Archivos Modificados

### 1. `Models/Permissions/ApplicationPermissions.cs`
```csharp
// Nueva categoría
public const string INSTRUMENTOS_EVALUACION = "Instrumentos de Evaluación";

// Nueva clase de permisos
public static class InstrumentosEvaluacion
{
    public const string VER = "instrumentos_evaluacion.ver";
    public const string CREAR = "instrumentos_evaluacion.crear";
    // ... (12 permisos en total)
}

// Agregado a GetAllPermissionsGrouped()
[INSTRUMENTOS_EVALUACION] = new List<PermissionInfo>
{
    new(InstrumentosEvaluacion.VER, "Ver instrumentos", "..."),
    // ... (12 permisos con descripciones)
}
```

### 2. `Configuration/AuthorizationExtensions.cs`
```csharp
// 12 nuevas políticas agregadas
options.AddPolicy("instrumentos_evaluacion.ver", policy =>
    policy.Requirements.Add(new PermissionRequirement("instrumentos_evaluacion.ver")));
// ... (11 políticas adicionales)
```

### 3. `Authorization/PermissionPolicyProvider.cs`
```csharp
// Módulo agregado a validModules
var validModules = new[] 
{
    // ... módulos existentes ...,
    "instrumentos_evaluacion"
};

// Acciones específicas agregadas a validActions
var validActions = new[] 
{
    // ... acciones existentes ...,
    "asignar_materias", "activar_desactivar", "gestionar_configuracion"
};
```

### 4. `Controllers/InstrumentosEvaluacionController.cs`
```csharp
// Imports agregados
using RubricasApp.Web.Authorization;
using RubricasApp.Web.Models.Permissions;

// Atributos aplicados a todos los métodos
[RequirePermission(ApplicationPermissions.InstrumentosEvaluacion.VER)]
[RequirePermission(ApplicationPermissions.InstrumentosEvaluacion.CREAR)]
[RequirePermission(ApplicationPermissions.InstrumentosEvaluacion.EDITAR)]
[RequirePermission(ApplicationPermissions.InstrumentosEvaluacion.ELIMINAR)]
```

---

## 🧪 Verificación y Pruebas

### Comandos de Verificación
```bash
# 1. Verificar compilación
dotnet build

# 2. Verificar permisos en código
grep -r "instrumentos_evaluacion" Models/Permissions/
grep -r "InstrumentosEvaluacion" Controllers/

# 3. Ejecutar script de verificación
node Tests/test-instrumentos-evaluacion-permissions.js
```

### URLs de Prueba
```
GET  /InstrumentosEvaluacion           # Requiere: ver
GET  /InstrumentosEvaluacion/Details/1 # Requiere: ver
GET  /InstrumentosEvaluacion/Create    # Requiere: crear
POST /InstrumentosEvaluacion/Create    # Requiere: crear
GET  /InstrumentosEvaluacion/Edit/1    # Requiere: editar
POST /InstrumentosEvaluacion/Edit/1    # Requiere: editar
GET  /InstrumentosEvaluacion/Delete/1  # Requiere: eliminar
POST /InstrumentosEvaluacion/Delete/1  # Requiere: eliminar
```

---

## ✅ Estado de Implementación

| Componente | Estado | Observaciones |
|------------|--------|---------------|
| ✅ Definición de permisos | Completado | 12 permisos definidos |
| ✅ Categoría en constantes | Completado | `INSTRUMENTOS_EVALUACION` agregada |
| ✅ Clase de permisos | Completado | `InstrumentosEvaluacion` creada |
| ✅ Lista de permisos | Completado | Agregado a `GetAllPermissionsGrouped()` |
| ✅ Políticas de autorización | Completado | 12 políticas configuradas |
| ✅ Provider de políticas | Completado | Módulo y acciones agregadas |
| ✅ Atributos en controlador | Completado | Todos los métodos protegidos |
| ✅ Script de verificación | Completado | `test-instrumentos-evaluacion-permissions.js` |
| ✅ Documentación | Completado | Este archivo |

---

## 🔮 Próximos Pasos

1. **Asignación de permisos a roles** - Configurar qué roles tienen qué permisos
2. **Pruebas de integración** - Verificar funcionamiento con usuarios reales
3. **Funcionalidades futuras** - Implementar acciones adicionales como:
   - Duplicación de instrumentos
   - Asignación masiva a materias
   - Exportación/importación
   - Dashboard de estadísticas

---

## 📊 Métricas de Implementación

- **Permisos totales:** 12
- **Políticas creadas:** 12
- **Archivos modificados:** 4
- **Métodos protegidos:** 8
- **Líneas de código agregadas:** ~150
- **Tiempo de implementación:** ~2 horas

---

## 🔗 Referencias

- [Sistema de Autorización](Authorization/README.md)
- [Guía de Permisos](Models/Permissions/README.md)
- [Documentación de Controladores](Controllers/README.md)

---

**Implementación completada exitosamente** ✅  
*Documentado por: GitHub Copilot*  
*Fecha: 2025-01-17*
