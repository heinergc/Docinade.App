# Flujo de Eliminación en Cascada de Profesores

## Diagrama de Flujo

```
┌─────────────────────────────────────────────────────────────┐
│  Usuario hace clic en "Eliminar" en /Profesores/Delete/1   │
└────────────────────────┬────────────────────────────────────┘
                         │
                         ▼
┌─────────────────────────────────────────────────────────────┐
│  DeleteConfirmed(id) - Verifica si profesor existe         │
└────────────────────────┬────────────────────────────────────┘
                         │
                         ▼
                 ┌───────────────┐
                 │ ¿Existe?      │
                 └───────┬───────┘
                         │
            ┌────────────┴────────────┐
            │ NO                      │ SÍ
            ▼                         ▼
   ┌─────────────────┐    ┌──────────────────────────┐
   │ Error: No       │    │ Iniciar eliminación      │
   │ encontrado      │    │ en cascada               │
   └─────────────────┘    └────────┬─────────────────┘
                                   │
                                   ▼
                    ┌──────────────────────────────┐
                    │ PASO 1: Eliminar             │
                    │ ProfesorGuia                 │
                    │ WHERE ProfesorId = @id       │
                    └────────┬─────────────────────┘
                             │
                             ▼
                    ┌──────────────────────────────┐
                    │ PASO 2: Eliminar             │
                    │ ProfesorGrupo                │
                    │ WHERE ProfesorId = @id       │
                    └────────┬─────────────────────┘
                             │
                             ▼
                    ┌──────────────────────────────┐
                    │ PASO 3: Eliminar             │
                    │ ProfesorCapacitacion         │
                    │ WHERE ProfesorId = @id       │
                    └────────┬─────────────────────┘
                             │
                             ▼
                    ┌──────────────────────────────┐
                    │ PASO 4: Eliminar             │
                    │ ProfesorExperienciaLaboral   │
                    │ WHERE ProfesorId = @id       │
                    └────────┬─────────────────────┘
                             │
                             ▼
                    ┌──────────────────────────────┐
                    │ PASO 5: Eliminar             │
                    │ ProfesorFormacionAcademica   │
                    │ WHERE ProfesorId = @id       │
                    └────────┬─────────────────────┘
                             │
                             ▼
                    ┌──────────────────────────────┐
                    │ PASO 6: Eliminar             │
                    │ Profesor                     │
                    │ WHERE Id = @id               │
                    └────────┬─────────────────────┘
                             │
                             ▼
                    ┌──────────────────────────────┐
                    │ SaveChangesAsync()           │
                    │ (Transacción única)          │
                    └────────┬─────────────────────┘
                             │
                ┌────────────┴────────────┐
                │                         │
                ▼                         ▼
       ┌─────────────────┐      ┌─────────────────┐
       │ ÉXITO            │      │ ERROR            │
       │ - Log warning    │      │ - Log error      │
       │ - TempData OK    │      │ - TempData Error │
       │ - Redirect Index │      │ - Rollback auto  │
       └─────────────────┘      │ - Redirect Delete│
                                 └─────────────────┘
```

## Dependencias entre Tablas

```
                    ┌───────────────┐
                    │   Profesores  │ ◄── Tabla Principal
                    │   (Id: PK)    │
                    └───────┬───────┘
                            │
            ┌───────────────┼───────────────┬───────────────┬───────────────┐
            │               │               │               │               │
            ▼               ▼               ▼               ▼               ▼
    ┌──────────────┐ ┌──────────────┐ ┌──────────────┐ ┌──────────────┐ ┌──────────────┐
    │ProfesorGuia  │ │ProfesorGrupo │ │  Profesor    │ │  Profesor    │ │  Profesor    │
    │              │ │              │ │ Capacitacion │ │ Experiencia  │ │  Formacion   │
    │ProfesorId:FK │ │ProfesorId:FK │ │ProfesorId:FK │ │ProfesorId:FK │ │ProfesorId:FK │
    └──────────────┘ └──────────────┘ └──────────────┘ └──────────────┘ └──────────────┘
    
    Todas las tablas hijas tienen ProfesorId como Foreign Key
    Deben eliminarse ANTES de eliminar el profesor
```

## Orden de Eliminación (Crítico)

```
┌────┬─────────────────────────────────┬──────────────────────────┐
│ #  │ Tabla                           │ Razón del Orden          │
├────┼─────────────────────────────────┼──────────────────────────┤
│ 1  │ ProfesorGuia                    │ No depende de otras hijas│
│    │ - FK: GrupoId, ProfesorId       │ Solo FK a Profesores     │
├────┼─────────────────────────────────┼──────────────────────────┤
│ 2  │ ProfesorGrupo                   │ No depende de otras hijas│
│    │ - FK: Profesor, Grupo, Materia  │ Solo FK a Profesores     │
├────┼─────────────────────────────────┼──────────────────────────┤
│ 3  │ ProfesorCapacitacion            │ No depende de otras hijas│
│    │ - FK: ProfesorId                │ Solo FK a Profesores     │
├────┼─────────────────────────────────┼──────────────────────────┤
│ 4  │ ProfesorExperienciaLaboral      │ No depende de otras hijas│
│    │ - FK: ProfesorId                │ Solo FK a Profesores     │
├────┼─────────────────────────────────┼──────────────────────────┤
│ 5  │ ProfesorFormacionAcademica      │ No depende de otras hijas│
│    │ - FK: ProfesorId                │ Solo FK a Profesores     │
├────┼─────────────────────────────────┼──────────────────────────┤
│ 6  │ Profesor                        │ DEBE SER ÚLTIMA          │
│    │ - PK: Id                        │ Es la tabla padre        │
└────┴─────────────────────────────────┴──────────────────────────┘

IMPORTANTE: El orden es FLEXIBLE para pasos 1-5 porque ninguna tabla hija
depende de otra tabla hija. Solo el paso 6 debe ser último.
```

## Estructura del Código

```csharp
// Pseudocódigo del método DeleteConfirmed

try {
    // 1. Buscar profesor
    profesor = Find(id)
    
    if (profesor == null) {
        return Error("No encontrado")
    }
    
    // 2. Eliminar tablas relacionadas
    foreach (tabla in [ProfesorGuia, ProfesorGrupo, Capacitaciones, 
                       Experiencias, Formaciones]) {
        registros = tabla.Where(r => r.ProfesorId == id)
        if (registros.Any()) {
            Remove(registros)
            Log($"Eliminando {registros.Count} de {tabla}")
        }
    }
    
    // 3. Eliminar profesor
    Remove(profesor)
    
    // 4. Guardar cambios (transacción única)
    SaveChanges()
    
    // 5. Log y mensaje de éxito
    LogWarning($"Profesor {id} eliminado")
    TempData["Success"] = "Eliminado exitosamente"
    
    return RedirectToAction("Index")
    
} catch (Exception ex) {
    // 6. Manejo de errores (rollback automático)
    LogError(ex)
    TempData["Error"] = ex.Message
    return RedirectToAction("Delete", id)
}
```

## Ejemplo de Datos

### Antes de la Eliminación
```
Profesor ID: 1 (Ana Maria Gonzalez)
├── ProfesorGuia: 2 registros
├── ProfesorGrupo: 3 registros
├── ProfesorCapacitacion: 3 registros
├── ProfesorExperienciaLaboral: 3 registros
└── ProfesorFormacionAcademica: 3 registros

TOTAL: 14 registros (1 profesor + 13 relacionados)
```

### Después de la Eliminación
```
Profesor ID: 1 (Ana Maria Gonzalez)
├── ProfesorGuia: 0 registros ✅
├── ProfesorGrupo: 0 registros ✅
├── ProfesorCapacitacion: 0 registros ✅
├── ProfesorExperienciaLaboral: 0 registros ✅
└── ProfesorFormacionAcademica: 0 registros ✅

TOTAL: 0 registros (todos eliminados)
```

## Transacción en Entity Framework Core

```
┌─────────────────────────────────────────────────────────┐
│ SaveChangesAsync() - Transacción Implícita             │
├─────────────────────────────────────────────────────────┤
│                                                         │
│  BEGIN TRANSACTION                                      │
│  ├── DELETE FROM ProfesorGuia WHERE ProfesorId = 1     │
│  ├── DELETE FROM ProfesorGrupo WHERE ProfesorId = 1    │
│  ├── DELETE FROM ProfesorCapacitacion WHERE ...        │
│  ├── DELETE FROM ProfesorExperienciaLaboral WHERE ...  │
│  ├── DELETE FROM ProfesorFormacionAcademica WHERE ...  │
│  └── DELETE FROM Profesores WHERE Id = 1               │
│                                                         │
│  Si TODO OK → COMMIT                                    │
│  Si ERROR  → ROLLBACK (automático)                     │
│                                                         │
└─────────────────────────────────────────────────────────┘
```

## Ventajas de Esta Solución

✅ **Control Total:** Sabemos exactamente qué se elimina  
✅ **Logging Detallado:** Cada paso queda registrado  
✅ **Transaccional:** Todo o nada (atomicidad)  
✅ **Sin cambios en BD:** No requiere ALTER TABLE  
✅ **Fácil Debug:** Podemos ver dónde falla  
✅ **Auditoría:** Registro completo en logs  

## Comparación con ON DELETE CASCADE

```
╔═══════════════════════════════════════════════════════════════╗
║           Eliminación Explícita  vs  ON DELETE CASCADE        ║
╠═══════════════════════════════════════════════════════════════╣
║ CONTROL                                                       ║
║   Explícita:  Alto ✅              CASCADE: Bajo ⚠️           ║
║                                                               ║
║ LOGGING                                                       ║
║   Explícita:  Detallado ✅         CASCADE: Mínimo ⚠️         ║
║                                                               ║
║ CAMBIOS EN BD                                                 ║
║   Explícita:  No requiere ✅       CASCADE: Requiere ⚠️       ║
║                                                               ║
║ DEBUG                                                         ║
║   Explícita:  Fácil ✅             CASCADE: Difícil ⚠️        ║
║                                                               ║
║ RENDIMIENTO                                                   ║
║   Explícita:  Bueno ✅             CASCADE: Excelente ✅      ║
║                                                               ║
║ MANTENIMIENTO                                                 ║
║   Explícita:  Medio ⚠️             CASCADE: Fácil ✅          ║
╚═══════════════════════════════════════════════════════════════╝
```

## Conclusión

La solución de **eliminación explícita** fue elegida porque:
- No requiere cambios en el esquema de la base de datos
- Proporciona control total y logging detallado
- Permite auditoría completa de las operaciones
- Facilita el debugging y mantenimiento

Esta implementación garantiza que al eliminar un profesor, 
**TODOS** sus datos relacionados se eliminan correctamente.
