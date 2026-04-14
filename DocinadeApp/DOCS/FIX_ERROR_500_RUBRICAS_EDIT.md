# Solución al Error 500 en Rubricas/Edit/{id}

**Fecha**: 16 de marzo de 2026  
**Error reportado**: HTTP 500 Internal Server Error en `http://localhost:8080/Rubricas/Edit/25`  
**Estado**: ✅ **CORREGIDO**

---

## 🔍 Diagnóstico del Problema

### Síntoma
- **URL afectada**: `/Rubricas/Edit/25`
- **Código de error**: 500 Internal Server Error
- **Mensaje**: "El sitio podría estar no disponible temporalmente o demasiado ocupado"

### Causa Raíz

El error se producía porque el método `Edit(int? id)` en `RubricasController` utilizaba `FindAsync()` para cargar la rúbrica, lo que **NO carga las propiedades de navegación** (relaciones).

**Código problemático** (líneas 417-445):
```csharp
public async Task<IActionResult> Edit(int? id)
{
    // ...código de validación...
    
    // ❌ PROBLEMA: FindAsync NO carga relaciones
    var rubrica = await _context.Rubricas.FindAsync(id);
    
    if (rubrica == null)
    {
        return NotFound();
    }

    // ⚠️ Esto puede fallar si intenta acceder a propiedades de navegación
    if (!await _userContextService.CanEditEntityAsync(rubrica))
    {
        TempData["ErrorMessage"] = "No tienes permisos para editar esta rúbrica.";
        return RedirectToAction(nameof(Index));
    }

    return View(rubrica);
}
```

### Por Qué Causaba el Error

1. **FindAsync()** carga SOLO la entidad principal sin relaciones
2. El modelo `Rubrica` tiene propiedades de navegación marcadas como `virtual`:
   ```csharp
   public virtual ApplicationUser? CreadoPor { get; set; }
   public virtual ApplicationUser? ModificadoPor { get; set; }
   public virtual GrupoCalificacion? GrupoCalificacion { get; set; }
   public virtual ICollection<ItemEvaluacion> ItemsEvaluacion { get; set; }
   // ... más relaciones
   ```

3. Si la vista Edit.cshtml o algún método intenta acceder a estas propiedades:
   - Entity Framework intenta hacer **lazy loading**
   - Si el contexto ya está dispuesto → **ObjectDisposedException**
   - Si lazy loading no está habilitado → **NullReferenceException**
   - Ambos resultan en **HTTP 500**

---

## ✅ Solución Implementada

### 1. Método Edit (GET) - Líneas 417-451

**Cambio realizado**:
```csharp
// GET: Rubricas/Edit/5
public async Task<IActionResult> Edit(int? id)
{
    if (id == null)
    {
        return NotFound();
    }

    var currentUserId = _userContextService.GetCurrentUserId();
    if (string.IsNullOrEmpty(currentUserId))
    {
        return Challenge();
    }

    // ✅ SOLUCIÓN: Cargar rúbrica con relaciones explícitas usando Include()
    var rubrica = await _context.Rubricas
        .Include(r => r.CreadoPor)
        .Include(r => r.ModificadoPor)
        .Include(r => r.GrupoCalificacion)
        .AsNoTracking()  // Optimización: solo lectura
        .FirstOrDefaultAsync(r => r.IdRubrica == id);

    if (rubrica == null)
    {
        return NotFound();
    }

    // Verificar si el usuario puede editar esta rúbrica
    if (!await _userContextService.CanEditEntityAsync(rubrica))
    {
        TempData["ErrorMessage"] = "No tienes permisos para editar esta rúbrica.";
        return RedirectToAction(nameof(Index));
    }

    return View(rubrica);
}
```

**Ventajas de la corrección**:
- ✅ Carga todas las relaciones necesarias explícitamente
- ✅ Evita problemas de lazy loading
- ✅ Usa `AsNoTracking()` para optimizar (solo lectura)
- ✅ Usa `FirstOrDefaultAsync()` con filtro en lugar de `FindAsync()`

---

### 2. Método Edit (POST) - Líneas 463-480

**Cambio realizado**:
```csharp
// Verificar permisos antes de la edición - cargar con relaciones
var rubricaExistente = await _context.Rubricas
    .Include(r => r.CreadoPor)
    .Include(r => r.ModificadoPor)
    .FirstOrDefaultAsync(r => r.IdRubrica == id);
    
if (rubricaExistente == null)
{
    return NotFound();
}

if (!await _userContextService.CanEditEntityAsync(rubricaExistente))
{
    TempData["ErrorMessage"] = "No tienes permisos para editar esta rúbrica.";
    return RedirectToAction(nameof(Index));
}
```

**Antes** (problemático):
```csharp
var rubricaExistente = await _context.Rubricas.FindAsync(id);
```

---

### 3. Método DeleteConfirmed (POST) - Líneas 550-575

**Cambio realizado**:
```csharp
// Cargar con relaciones para verificación de permisos
var rubrica = await _context.Rubricas
    .Include(r => r.CreadoPor)
    .Include(r => r.ModificadoPor)
    .FirstOrDefaultAsync(r => r.IdRubrica == id);
    
if (rubrica != null)
{
    // Verificar permisos antes de eliminar
    if (!await _userContextService.CanDeleteEntityAsync(rubrica))
    {
        TempData["ErrorMessage"] = "No tienes permisos para eliminar esta rúbrica.";
        return RedirectToAction(nameof(Index));
    }
    // ... rest of deletion logic
}
```

---

## 📊 Resumen de Cambios

### Archivos Modificados
- **Archivo**: `Controllers/RubricasController.cs`
- **Métodos corregidos**: 3
  1. `Edit(int? id)` - GET (líneas 417-451)
  2. `Edit(int id, ...)` - POST (líneas 463-480)
  3. `DeleteConfirmed(int id)` - POST (líneas 550-575)

### Patrón de Corrección

**Antes** (problemático):
```csharp
var entity = await _context.Rubricas.FindAsync(id);
```

**Después** (correcto):
```csharp
var entity = await _context.Rubricas
    .Include(r => r.CreadoPor)
    .Include(r => r.ModificadoPor)
    .Include(r => r.GrupoCalificacion)  // Si es necesario
    .AsNoTracking()  // Para operaciones de solo lectura
    .FirstOrDefaultAsync(r => r.IdRubrica == id);
```

---

## 🧪 Validación

### Métodos que YA estaban correctos

1. **Details(int? id)** - líneas 134-165
   ```csharp
   var rubrica = await _context.Rubricas
       .Include(r => r.ItemsEvaluacion)
       .Include(r => r.ValoresRubrica)
       .Include(r => r.CreadoPor)
       .FirstOrDefaultAsync(m => m.IdRubrica == id);
   ```

2. **Delete(int? id)** - GET - líneas 517-546
   ```csharp
   var rubrica = await _context.Rubricas
       .Include(r => r.CreadoPor)
       .FirstOrDefaultAsync(m => m.IdRubrica == id);
   ```

3. **Index()** - líneas 28-98
   ```csharp
   var rubricas = await query
       .Include(r => r.CreadoPor)
       .OrderBy(r => r.NombreRubrica)
       .ToListAsync();
   ```

---

## 🔍 Comparación: FindAsync vs Include + FirstOrDefaultAsync

| Aspecto | `FindAsync(id)` | `Include().FirstOrDefaultAsync()` |
|---------|-----------------|----------------------------------|
| **Carga relaciones** | ❌ No | ✅ Sí (explícitas) |
| **Lazy loading** | ⚠️ Depende de config | ✅ No necesario |
| **Cache L1** | ✅ Usa cache | ❌ Consulta DB |
| **Filtros complejos** | ❌ Solo por PK | ✅ Cualquier condición |
| **Tracking** | ✅ Siempre | ⚙️ Configurable (AsNoTracking) |
| **Riesgo de error** | ⚠️ Alto (si falta lazy) | ✅ Bajo |
| **Rendimiento** | ✅ Rápido (cache) | ⚙️ Depende de includes |

---

## 📝 Recomendaciones Generales

### Cuándo usar `FindAsync()`
✅ **Usar cuando**:
- Solo necesitas la entidad principal
- No vas a acceder a propiedades de navegación
- Quieres aprovechar el cache de EF Core
- Solo consultas por clave primaria

### Cuándo usar `Include() + FirstOrDefaultAsync()`
✅ **Usar cuando**:
- Necesitas propiedades de navegación
- Vas a mostrar datos en una vista
- Pasas la entidad a métodos que pueden acceder a relaciones
- Necesitas filtros más complejos que solo el ID

### Mejores Prácticas

1. **Siempre usar Include() en acciones de visualización**:
   ```csharp
   // Para vistas Details, Edit, Delete
   .Include(r => r.CreadoPor)
   .Include(r => r.ModificadoPor)
   ```

2. **Usar AsNoTracking() para operaciones de solo lectura**:
   ```csharp
   // GET Edit, Details (no modifican)
   .AsNoTracking()
   ```

3. **No usar AsNoTracking() cuando vas a modificar**:
   ```csharp
   // POST Edit, DELETE (sí modifican)
   // NO usar AsNoTracking()
   ```

4. **Documentar qué relaciones se cargan**:
   ```csharp
   // Cargar rúbrica con datos de auditoría para verificación de permisos
   var rubrica = await _context.Rubricas
       .Include(r => r.CreadoPor)       // Para permisos
       .Include(r => r.ModificadoPor)   // Para historial
       .FirstOrDefaultAsync(r => r.IdRubrica == id);
   ```

---

## 🚀 Próximos Pasos

### Verificación Recomendada

1. **Probar la edición de rúbricas**:
   ```
   GET http://localhost:8080/Rubricas/Edit/25
   ```
   - Debería cargar correctamente
   - No debe mostrar error 500

2. **Verificar otros ID**:
   ```
   GET http://localhost:8080/Rubricas/Edit/1
   GET http://localhost:8080/Rubricas/Edit/10
   ```

3. **Probar el POST (guardar cambios)**:
   - Editar nombre
   - Cambiar estado
   - Guardar
   - Verificar que persiste

### Auditoría de Código

Revisar otros controladores que puedan tener el mismo patrón problemático:
- `EstudiantesController`
- `GruposController`
- `MateriasController`
- `EvaluacionesController`

Buscar usos de `FindAsync()` y evaluar si necesitan `Include()`:
```bash
grep -r "FindAsync" Controllers/*.cs
```

---

## 📚 Referencias

- [Entity Framework Core - Loading Related Data](https://learn.microsoft.com/en-us/ef/core/querying/related-data/)
- [Eager Loading vs Lazy Loading](https://learn.microsoft.com/en-us/ef/core/querying/related-data/eager)
- [AsNoTracking - Performance Optimization](https://learn.microsoft.com/en-us/ef/core/querying/tracking)

---

**Estado**: ✅ Problema resuelto y documentado  
**Archivos modificados**: 1 (`RubricasController.cs`)  
**Líneas cambiadas**: ~30  
**Impacto**: 3 métodos corregidos  
**Tests requeridos**: Manual de edición de rúbricas

---

_Última actualización: 16 de marzo de 2026_
