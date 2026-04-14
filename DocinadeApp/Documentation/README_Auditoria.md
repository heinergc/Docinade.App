# Sistema de Auditoría - Configuración de Base de Datos

## Problema Resuelto
El error `Invalid object name 'AuditoriasOperaciones'` se debe a que la tabla no existe en la base de datos.

## Soluciones Disponibles

### Opción 1: Migración Entity Framework (Recomendada)
```bash
# En la consola del administrador de paquetes o terminal
cd c:\Users\hguido\Desktop\RubricasApp\src\RubricasApp.Web

# Crear la migración
dotnet ef migrations add AddAuditoriasOperaciones

# Aplicar la migración
dotnet ef database update
```

### Opción 2: Script SQL Manual
1. Ejecutar el script `Scripts/CreateAuditoriasOperaciones.sql` en SQL Server Management Studio
2. Conectar a la base de datos del proyecto
3. Ejecutar todo el script

### Opción 3: Verificar DbContext Existente
Si ya existe una configuración previa:

1. Verificar en `RubricasDbContext.cs` si existe:
```csharp
public DbSet<AuditoriaOperacion> AuditoriasOperaciones { get; set; }
```

2. Si no existe, agregar en el DbContext:
```csharp
public DbSet<AuditoriaOperacion> AuditoriasOperaciones { get; set; }
```

3. Crear y aplicar migración:
```bash
dotnet ef migrations add AddAuditoriasOperaciones
dotnet ef database update
```

## Archivos Creados

1. **Models/AuditoriaOperacion.cs** - Modelo de la entidad
2. **Scripts/CreateAuditoriasOperaciones.sql** - Script SQL manual
3. **Migrations/AddAuditoriasOperaciones.cs** - Migración Entity Framework

## Verificación
Después de aplicar cualquiera de las soluciones, verificar que la tabla existe:

```sql
SELECT TOP 5 * FROM AuditoriasOperaciones
```

## Funcionalidades del Sistema de Auditoría

### Registros Automáticos
- ✅ Operaciones CRUD (Create, Read, Update, Delete)
- ✅ Información del usuario que ejecuta la acción
- ✅ IP y User Agent
- ✅ Datos anteriores y nuevos (JSON)
- ✅ Timestamps precisos
- ✅ Registro de errores

### Consultas Disponibles
- ✅ Historial por tabla y registro específico
- ✅ Historial por usuario
- ✅ Filtros por fechas, tipo de operación
- ✅ Estadísticas y métricas
- ✅ Paginación para grandes volúmenes

### Índices de Rendimiento
- ✅ Índice por fecha de operación (DESC)
- ✅ Índice por tabla afectada
- ✅ Índice por usuario
- ✅ Índice compuesto tabla + registro

## Uso del Servicio

```csharp
// Inyección en el constructor
private readonly IAuditoriaService _auditoriaService;

// Registrar operación exitosa
await _auditoriaService.RegistrarExitoAsync(
    "UPDATE", 
    "Grupos", 
    grupoId, 
    "Actualización de grupo", 
    userId,
    motivo: "Cambio de capacidad",
    datosAnteriores: grupoAnterior,
    datosNuevos: grupoNuevo
);

// Registrar error
await _auditoriaService.RegistrarErrorAsync(
    "DELETE", 
    "Grupos", 
    grupoId, 
    "Intento de eliminación", 
    userId, 
    "No se puede eliminar grupo con estudiantes asignados"
);

// Obtener historial
var historial = await _auditoriaService.ObtenerHistorialAsync("Grupos", grupoId);
```

¡El sistema de auditoría está listo para usar una vez que se cree la tabla! 🎉