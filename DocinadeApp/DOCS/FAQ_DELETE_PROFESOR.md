# FAQ - Eliminación de Profesores

## Preguntas Frecuentes

### 1. ¿Por qué no puedo eliminar un profesor?

**Respuesta:** Este problema ya fue resuelto. Anteriormente, el sistema no permitía eliminar profesores porque existían registros relacionados en otras tablas (formaciones, experiencias, capacitaciones, etc.). La solución implementada elimina automáticamente todos estos registros antes de eliminar al profesor.

### 2. ¿Qué datos se eliminan cuando elimino un profesor?

**Respuesta:** Se eliminan TODOS los datos relacionados con el profesor:
- ✅ Asignaciones como profesor guía (`ProfesorGuia`)
- ✅ Asignaciones de grupos y materias (`ProfesorGrupo`)
- ✅ Capacitaciones realizadas (`ProfesorCapacitacion`)
- ✅ Experiencia laboral (`ProfesorExperienciaLaboral`)
- ✅ Formación académica (`ProfesorFormacionAcademica`)
- ✅ El profesor mismo (`Profesores`)

### 3. ¿Es reversible la eliminación?

**Respuesta:** **NO**. La eliminación es **permanente** e **irreversible**. Por eso el sistema muestra una página de confirmación antes de proceder. Asegúrese de que realmente desea eliminar al profesor antes de confirmar.

### 4. ¿Puedo desactivar un profesor en lugar de eliminarlo?

**Respuesta:** **SÍ**. Esta es la opción recomendada en la mayoría de casos. Use el botón "Activar/Desactivar" en la página de detalles del profesor. Esto mantiene todos los datos pero marca al profesor como inactivo, evitando que aparezca en listados activos.

```
Activar/Desactivar ✅ RECOMENDADO (reversible)
Eliminar ⚠️ Solo para casos especiales (irreversible)
```

### 5. ¿Cómo puedo saber qué se eliminará antes de confirmar?

**Respuesta:** En la página de confirmación de eliminación (`/Profesores/Delete/{id}`), puede ver:
- Nombre completo del profesor
- Información básica (cédula, email, etc.)
- Ubicación (provincia, cantón, distrito)

Para ver el detalle completo de registros relacionados, puede ejecutar el script SQL:
```sql
-- Ver en Scripts/test_delete_profesor.sql
EXEC sp_test_delete_profesor @ProfesorId = 1
```

### 6. ¿Qué pasa si hay un error durante la eliminación?

**Respuesta:** La eliminación es **transaccional**. Si ocurre un error en cualquier paso:
1. Se revierten TODOS los cambios (rollback automático)
2. No se elimina nada
3. Se muestra un mensaje de error al usuario
4. Se registra el error en los logs

### 7. ¿Dónde puedo ver el registro de eliminaciones?

**Respuesta:** Todas las eliminaciones se registran en los logs de la aplicación. Busque entradas como:
```
warn: Profesor 1 (Ana Maria Gonzalez) y todos sus datos relacionados 
      eliminados permanentemente por admin@rubricas.edu
```

También puede revisar los logs detallados que muestran cada paso:
```
info: Eliminando 2 registros de ProfesorGuia para profesor 1
info: Eliminando 3 registros de ProfesorGrupo para profesor 1
...
```

### 8. ¿Cuánto tiempo tarda en eliminarse un profesor?

**Respuesta:** Típicamente menos de 1 segundo. El proceso es muy rápido porque:
- Usa una transacción única
- Elimina registros por lotes (`RemoveRange`)
- Ejecuta todo en una sola conexión a la BD

### 9. ¿Puedo eliminar múltiples profesores a la vez?

**Respuesta:** Actualmente NO. Debe eliminar profesores uno por uno. Esto es intencional para evitar eliminaciones masivas accidentales.

### 10. ¿Qué permisos necesito para eliminar un profesor?

**Respuesta:** Debe:
- Estar autenticado en el sistema
- Tener permisos de administración (rol adecuado)
- El sistema valida el token anti-falsificación

### 11. ¿Afecta el rendimiento del sistema eliminar profesores?

**Respuesta:** NO. La eliminación es rápida y no afecta el rendimiento. Sin embargo, evite:
- Eliminar muchos profesores en secuencia rápida
- Eliminar profesores durante horarios de uso intensivo

### 12. ¿Qué debo hacer antes de eliminar un profesor?

**Checklist recomendado:**
1. ☑️ Verificar que realmente debe eliminarse (¿mejor desactivar?)
2. ☑️ Revisar que no haya evaluaciones o calificaciones pendientes
3. ☑️ Notificar a otros administradores si es necesario
4. ☑️ Exportar datos si necesita respaldo
5. ☑️ Confirmar el ID correcto del profesor

### 13. ¿Puedo recuperar un profesor eliminado?

**Respuesta:** NO directamente del sistema. La única forma es:
1. Si tiene un respaldo de la base de datos
2. Si exportó los datos antes de eliminar
3. Si tiene el SQL de creación del profesor

**Importante:** No hay "papelera de reciclaje" para profesores eliminados.

### 14. ¿Qué pasa con las evaluaciones de estudiantes si elimino un profesor?

**Respuesta:** Esta es una pregunta importante. Actualmente:
- Las evaluaciones NO se eliminan automáticamente
- Las tablas de evaluaciones NO tienen FK directo a Profesores
- Las evaluaciones quedan huérfanas pero siguen existiendo

**Recomendación:** Revise si hay evaluaciones antes de eliminar. Considere:
- ¿Hay calificaciones pendientes?
- ¿Hay rúbricas asignadas?
- ¿Mejor desactivar en lugar de eliminar?

### 15. ¿Hay alguna forma de "eliminar suavemente"?

**Respuesta:** SÍ. Use el campo `Estado` del profesor:
```csharp
// En lugar de eliminar, desactivar:
profesor.Estado = false;
profesor.MotivoInactividad = "Razón...";
```

Esto mantiene todos los datos pero:
- El profesor no aparece en listados activos
- No puede ser asignado a nuevos grupos
- Mantiene el historial completo

### 16. ¿Cómo pruebo la eliminación sin afectar datos reales?

**Respuesta:** Use los profesores de prueba creados con el script de seed:
```sql
-- Profesores de prueba (IDs 1-5)
-- Scripts/seed_profesores_completo.sql
```

Pasos:
1. Ejecutar el script de seed
2. Probar eliminación con ID 1-5
3. Re-ejecutar el script para restaurar datos de prueba

### 17. ¿El sistema valida si el profesor tiene grupos activos?

**Respuesta:** Actualmente NO. El sistema elimina el profesor y sus asignaciones sin validar si tiene grupos activos. Recomendaciones:
1. Verificar manualmente antes de eliminar
2. Considerar agregar validación en futuras versiones
3. Usar desactivación en lugar de eliminación

### 18. ¿Qué información se pierde al eliminar un profesor?

**Respuesta:** Se pierde TODA la información:

**Información Personal:**
- Datos biográficos
- Contacto de emergencia
- Información bancaria
- Ubicación geográfica

**Información Académica:**
- Títulos y certificaciones
- Formación académica
- Capacitaciones realizadas

**Información Laboral:**
- Experiencia laboral
- Asignaciones de grupos
- Rol como profesor guía

**Importante:** Esta información NO puede recuperarse después de la eliminación.

### 19. ¿Debo hacer respaldo antes de eliminar?

**Respuesta:** **Altamente recomendado** si:
- Es un profesor con mucha historia
- Tiene datos únicos o importantes
- Podría necesitar la información después
- Es una eliminación solicitada por terceros

**No es crítico** si:
- Es un profesor de prueba
- Es un registro duplicado
- Es un profesor recién creado sin datos

### 20. ¿Qué hago si eliminé un profesor por error?

**Respuesta:** 
1. **Inmediatamente:** Contactar al administrador de BD
2. **Si hay respaldo reciente:** Restaurar desde backup
3. **Si no hay respaldo:** 
   - Recrear manualmente con la información disponible
   - Revisar logs para ver qué se eliminó
   - Buscar copias de información (emails, documentos)

**Prevención:** Siempre verificar dos veces antes de confirmar la eliminación.

---

## Contacto y Soporte

Si tiene otras preguntas no cubiertas en este FAQ, contacte:
- Administrador del sistema
- Equipo de desarrollo
- Revisar documentación en `/DOCS/`

## Recursos Adicionales

- `DELETE_PROFESOR_CASCADE.md` - Documentación técnica completa
- `FLUJO_DELETE_CASCADE.md` - Diagramas y flujos
- `test_delete_profesor.sql` - Scripts de prueba
- `check_foreign_keys.sql` - Verificación de constraints

---

**Última actualización:** Octubre 27, 2025  
**Versión del sistema:** 1.0
