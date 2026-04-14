# Solución: Eliminación en Cascada de Profesores

## Problema
Al intentar eliminar un profesor, el sistema fallaba debido a restricciones de foreign keys. Las tablas relacionadas (`ProfesorFormacionAcademica`, `ProfesorExperienciaLaboral`, `ProfesorCapacitacion`, `ProfesorGrupo`, `ProfesorGuia`) tenían registros que impedían la eliminación del profesor padre.

## Causa
El método `DeleteConfirmed` en `ProfesoresController.cs` solo eliminaba el registro de la tabla `Profesores`, asumiendo que existía una configuración de `ON DELETE CASCADE` en la base de datos. Sin embargo, esta configuración no estaba presente, causando violaciones de integridad referencial.

## Solución Implementada
Se modificó el método `DeleteConfirmed` para eliminar **explícitamente** todos los registros relacionados antes de eliminar el profesor principal. La eliminación se realiza en el siguiente orden (respetando las dependencias):

### Orden de Eliminación
1. **ProfesorGuia** - Asignaciones como profesor guía
2. **ProfesorGrupo** - Asignaciones de grupos y materias
3. **ProfesorCapacitacion** - Capacitaciones realizadas
4. **ProfesorExperienciaLaboral** - Experiencia laboral
5. **ProfesorFormacionAcademica** - Formación académica
6. **Profesor** - Registro principal

### Características de la Solución
- ✅ Eliminación transaccional: Si falla cualquier paso, se revierten todos los cambios
- ✅ Logging detallado: Registra cuántos elementos se eliminan de cada tabla
- ✅ Mensajes informativos: El usuario recibe confirmación de la eliminación completa
- ✅ Manejo de errores: Captura y reporta errores con mensajes descriptivos

## Código Modificado

### Archivo: `Controllers/ProfesoresController.cs`
**Método:** `DeleteConfirmed` (línea 1870)

```csharp
public async Task<IActionResult> DeleteConfirmed(int id)
{
    try
    {
        var profesor = await _context.Set<Profesor>()
            .FirstOrDefaultAsync(p => p.Id == id);

        if (profesor == null)
        {
            TempData["Error"] = "Profesor no encontrado.";
            return RedirectToAction(nameof(Index));
        }

        var nombreCompleto = $"{profesor.Nombres} {profesor.PrimerApellido} {profesor.SegundoApellido}".Trim();

        // Eliminar explícitamente todos los registros relacionados
        
        // 1. Eliminar ProfesorGuia
        var profesorGuias = await _context.Set<ProfesorGuia>()
            .Where(pg => pg.ProfesorId == id)
            .ToListAsync();
        if (profesorGuias.Any())
        {
            _context.Set<ProfesorGuia>().RemoveRange(profesorGuias);
            _logger.LogInformation("Eliminando {Count} registros de ProfesorGuia", profesorGuias.Count);
        }

        // 2. Eliminar ProfesorGrupo
        var profesorGrupos = await _context.Set<ProfesorGrupo>()
            .Where(pg => pg.ProfesorId == id)
            .ToListAsync();
        if (profesorGrupos.Any())
        {
            _context.Set<ProfesorGrupo>().RemoveRange(profesorGrupos);
            _logger.LogInformation("Eliminando {Count} registros de ProfesorGrupo", profesorGrupos.Count);
        }

        // 3-5. Eliminar capacitaciones, experiencias y formaciones...
        
        // 6. Eliminar el profesor
        _context.Set<Profesor>().Remove(profesor);
        
        // Guardar todos los cambios en una transacción
        await _context.SaveChangesAsync();

        TempData["Success"] = $"Profesor {nombreCompleto} y todos sus datos relacionados eliminados permanentemente.";
        return RedirectToAction(nameof(Index));
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error al eliminar profesor {ProfesorId}: {Message}", id, ex.Message);
        TempData["Error"] = $"Error al eliminar el profesor: {ex.Message}";
        return RedirectToAction(nameof(Delete), new { id });
    }
}
```

## Scripts de Prueba

### 1. Verificar Foreign Keys
**Archivo:** `Scripts/check_foreign_keys.sql`

Este script verifica la configuración actual de las foreign keys:
```sql
SELECT 
    OBJECT_NAME(fk.parent_object_id) AS TablaHija,
    fk.name AS NombreConstraint,
    OBJECT_NAME(fk.referenced_object_id) AS TablaPadre,
    fk.delete_referential_action_desc AS AccionEliminar
FROM sys.foreign_keys AS fk
WHERE OBJECT_NAME(fk.referenced_object_id) = 'Profesores'
```

### 2. Probar Eliminación
**Archivo:** `Scripts/test_delete_profesor.sql`

Script para verificar el estado antes y después de eliminar un profesor:
- Muestra todos los registros relacionados ANTES de la eliminación
- Proporciona consultas para verificar DESPUÉS de la eliminación

## Cómo Usar

### Eliminar un Profesor
1. Navegar a: `https://localhost:18163/Profesores`
2. Seleccionar el profesor a eliminar
3. Hacer clic en el botón "Eliminar"
4. Confirmar la eliminación en la página de confirmación

### Verificar Eliminación Exitosa
Ejecutar el script de verificación con el ID del profesor eliminado:
```sql
DECLARE @ProfesorId INT = 1; -- ID del profesor eliminado

SELECT 'Profesor' AS Tabla, COUNT(*) AS Registros 
FROM Profesores WHERE Id = @ProfesorId
UNION ALL
SELECT 'ProfesorFormacionAcademica', COUNT(*) 
FROM ProfesorFormacionAcademica WHERE ProfesorId = @ProfesorId
-- ... (más tablas)
```

Todos los conteos deben ser **0** (cero).

## Datos de Prueba
Se pueden usar los profesores de prueba creados con el script `seed_profesores_completo.sql`:
- Profesor ID 1: Ana Maria Gonzalez (3 formaciones, 3 experiencias, 3 capacitaciones)
- Profesor ID 2: Carlos Alberto Ramirez (3 formaciones, 2 experiencias, 4 capacitaciones)
- Profesor ID 3: Maria Fernanda Mora (3 formaciones, 2 experiencias, 3 capacitaciones)
- Profesor ID 4: Jose Daniel Vargas (2 formaciones, 2 experiencias, 3 capacitaciones)
- Profesor ID 5: Laura Patricia Jimenez (2 formaciones, 3 experiencias, 4 capacitaciones)

## Logs Esperados
Al eliminar un profesor, el sistema registra:
```
info: Eliminando 2 registros de ProfesorGuia para profesor 1
info: Eliminando 3 registros de ProfesorGrupo para profesor 1
info: Eliminando 3 capacitaciones del profesor 1
info: Eliminando 3 experiencias laborales del profesor 1
info: Eliminando 3 formaciones académicas del profesor 1
warn: Profesor 1 (Ana Maria Gonzalez) y todos sus datos relacionados eliminados permanentemente por admin@rubricas.edu
```

## Alternativas Consideradas

### Opción 1: Configurar ON DELETE CASCADE en la Base de Datos
**Ventajas:**
- Más limpio en el código
- Manejado automáticamente por SQL Server

**Desventajas:**
- Requiere modificar el esquema de la base de datos
- Menos control sobre qué se elimina
- No permite logging detallado

### Opción 2: Eliminación Explícita en Código (IMPLEMENTADA)
**Ventajas:**
- ✅ No requiere cambios en el esquema de BD
- ✅ Control total sobre el proceso
- ✅ Logging detallado de cada paso
- ✅ Más fácil de depurar

**Desventajas:**
- Más líneas de código
- Debe mantenerse sincronizado con el modelo

## Seguridad
- ✅ Requiere autenticación
- ✅ Validación del token anti-falsificación
- ✅ Confirmación antes de eliminar
- ✅ Logging de auditoría completo

## Rendimiento
- **Query única por tabla:** Usa `RemoveRange` para eliminar múltiples registros
- **Transacción única:** `SaveChangesAsync()` se llama una sola vez al final
- **Tiempo estimado:** < 1 segundo para un profesor con ~20 registros relacionados

## Mantenimiento Futuro
Si se agregan nuevas tablas relacionadas con `Profesores`:
1. Identificar la tabla y su foreign key
2. Agregar eliminación antes del paso 6 (eliminar profesor)
3. Agregar logging para la nueva tabla
4. Actualizar los scripts de verificación

## Testing
Para probar la funcionalidad:
1. Ejecutar `test_delete_profesor.sql` con un ID de profesor de prueba
2. Eliminar el profesor desde la interfaz web
3. Ejecutar la sección de verificación del script
4. Confirmar que todos los conteos son 0

## Fecha de Implementación
**Octubre 27, 2025**

## Autor
Implementado durante sesión de troubleshooting de eliminación de profesores.
