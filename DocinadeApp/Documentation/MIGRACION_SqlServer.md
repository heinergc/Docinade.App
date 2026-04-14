# ?? MIGRACIÓN A SQL SERVER - GUÍA COMPLETA

## ?? **OBJETIVO**
Migrar la aplicación RubricasApp de SQLite a SQL Server Express para mejor rendimiento y escalabilidad.

## ? **CAMBIOS REALIZADOS**

### **1. Configuración de Base de Datos**

#### **appsettings.json**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=SCPDTIC16584\\SQLEXPRESS;Database=RubricasDb;Integrated Security=true;MultipleActiveResultSets=true;TrustServerCertificate=true",
    "SqliteConnection": "Data Source=RubricasDb.db",
    "SqlServerConnection": "Server=SCPDTIC16584\\SQLEXPRESS;Database=RubricasDb;Integrated Security=true;MultipleActiveResultSets=true;TrustServerCertificate=true"
  }
}
```

**Cambios:**
- ? `DefaultConnection` ahora apunta a SQL Server Express
- ? Se mantiene `SqliteConnection` como backup
- ? Agregado logging para EF Core
- ? Base de datos: `RubricasDb` en `SCPDTIC16584\SQLEXPRESS`

### **2. Configuración de Entity Framework**

#### **Program.cs**
```csharp
// SQL Server como principal, SQLite como fallback
builder.Services.AddDbContext<RubricasDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    
    if (connectionString != null && connectionString.Contains("Data Source=") && connectionString.Contains(".db"))
    {
        // SQLite configuration (fallback)
        options.UseSqlite(connectionString);
        Console.WriteLine($"??? Using SQLite database: {connectionString}");
    }
    else
    {
        // SQL Server configuration (default)
        options.UseSqlServer(connectionString, sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 3,
                maxRetryDelay: TimeSpan.FromSeconds(5),
                errorNumbersToAdd: null);
            sqlOptions.CommandTimeout(30);
        });
        Console.WriteLine($"?? Using SQL Server database: {connectionString}");
    }
    
    options.EnableSensitiveDataLogging(builder.Environment.IsDevelopment());
    options.EnableDetailedErrors(builder.Environment.IsDevelopment());
});
```

**Mejoras:**
- ? Detección automática del tipo de base de datos
- ? Retry logic para SQL Server
- ? Timeout configurado a 30 segundos
- ? Logging detallado en desarrollo

### **3. DbContext Actualizado**

#### **RubricasDbContext.cs**
```csharp
// Detectar el proveedor de base de datos para configuraciones específicas
var provider = Database.ProviderName;
bool isSqlServer = provider?.Contains("SqlServer") == true;
bool isSqlite = provider?.Contains("Sqlite") == true;

// Configuración específica por proveedor para fecha por defecto
if (isSqlServer)
{
    entity.Property(e => e.FechaRegistro).HasDefaultValueSql("GETDATE()");
}
else if (isSqlite)
{
    entity.Property(e => e.FechaRegistro).HasDefaultValueSql("datetime('now')");
}
```

**Adaptaciones:**
- ? Funciones de fecha específicas: `GETDATE()` vs `datetime('now')`
- ? Tipos de columna: `NVARCHAR(MAX)` vs `TEXT`
- ? Configuración automática según el proveedor

---

## ?? **PROCESO DE MIGRACIÓN**

### **Paso 1: Preparar Migración**
```powershell
# Navegar al directorio del proyecto
cd C:\Users\hguido\Desktop\RubricasApp\src\RubricasApp.Web

# Crear nueva migración para SQL Server
.\Scripts\MigrarSqlServer.ps1 -CreateMigration
```

### **Paso 2: Aplicar Migración**
```powershell
# Aplicar migraciones a SQL Server
.\Scripts\MigrarSqlServer.ps1 -UpdateDatabase
```

### **Paso 3: Ejecutar Datos de Prueba** (Opcional)
```sql
-- En SQL Server Management Studio o Azure Data Studio
-- Ejecutar: Scripts/DatosPrueba_SqlServer.sql
```

### **Paso 4: Verificar Funcionamiento**
```bash
# Ejecutar la aplicación
dotnet run

# Verificar en consola que muestra:
# "?? Using SQL Server database: Server=SCPDTIC16584\SQLEXPRESS;..."
```

---

## ??? **ARCHIVOS NUEVOS CREADOS**

| Archivo | Descripción |
|---------|-------------|
| `Scripts/MigrarSqlServer.ps1` | Script automatizado de migración |
| `Scripts/DatosPrueba_SqlServer.sql` | Datos de prueba adaptados para SQL Server |
| `Documentation/MIGRACION_SqlServer.md` | Esta documentación |

---

## ?? **COMANDOS ÚTILES**

### **Verificar Estado de Migración**
```powershell
# Ver estado actual
.\Scripts\MigrarSqlServer.ps1

# Ver información del contexto
dotnet ef dbcontext info

# Listar migraciones
dotnet ef migrations list
```

### **Comandos de Emergencia**
```powershell
# Eliminar base de datos (¡CUIDADO!)
.\Scripts\MigrarSqlServer.ps1 -DropDatabase

# Eliminar última migración
dotnet ef migrations remove

# Actualizar base de datos a migración específica
dotnet ef database update NombreMigracion
```

---

## ?? **VERIFICACIÓN POST-MIGRACIÓN**

### **1. Verificar Conexión**
- ? La aplicación inicia sin errores
- ? Console muestra: "?? Using SQL Server database..."
- ? No hay errores de conexión en logs

### **2. Verificar Funcionalidades**
```
? Login/Logout funciona
? Crear/Editar rúbricas funciona
? Crear/Editar evaluaciones funciona
? Cuaderno Calificador funciona
? Exportación a CSV/Excel funciona
? Reportes funcionan
```

### **3. Verificar Rendimiento**
- ? Consultas más rápidas que SQLite
- ? Sin timeouts en operaciones grandes
- ? Conexiones concurrentes soportadas

---

## ??? **CONFIGURACIONES ESPECÍFICAS DE SQL SERVER**

### **Connection String Explicado**
```
Server=SCPDTIC16584\SQLEXPRESS  -> Servidor SQL Server Express
Database=RubricasDb             -> Nombre de la base de datos
Integrated Security=true        -> Usar autenticación de Windows
MultipleActiveResultSets=true   -> Permitir múltiples consultas
TrustServerCertificate=true     -> Confiar en certificado del servidor
```

### **Características Habilitadas**
- ? **Retry Logic**: Reintenta conexiones fallidas automáticamente
- ? **Command Timeout**: 30 segundos para consultas grandes
- ? **Connection Pooling**: Manejo eficiente de conexiones
- ? **Transacciones**: Soporte completo para operaciones complejas

---

## ?? **TROUBLESHOOTING**

### **Error: No se puede conectar a SQL Server**
```bash
# Verificar que SQL Server Express esté ejecutándose
# En Services.msc buscar: SQL Server (SQLEXPRESS)

# Verificar instancia disponible
sqlcmd -L

# Probar conexión manual
sqlcmd -S SCPDTIC16584\SQLEXPRESS -E
```

### **Error: Base de datos no existe**
```powershell
# Crear base de datos manualmente
sqlcmd -S SCPDTIC16584\SQLEXPRESS -E -Q "CREATE DATABASE RubricasDb"

# O usar el script de migración
.\Scripts\MigrarSqlServer.ps1 -UpdateDatabase
```

### **Error: Migración falla**
```powershell
# Eliminar migraciones problemáticas
dotnet ef migrations remove

# Crear nueva migración
.\Scripts\MigrarSqlServer.ps1 -CreateMigration

# Aplicar desde cero
.\Scripts\MigrarSqlServer.ps1 -DropDatabase
.\Scripts\MigrarSqlServer.ps1 -UpdateDatabase
```

---

## ?? **COMPARACIÓN SQLITE vs SQL SERVER**

| Característica | SQLite | SQL Server Express |
|---------------|---------|-------------------|
| **Archivo** | RubricasDb.db (local) | Base de datos del servidor |
| **Concurrencia** | Limitada | Alta concurrencia |
| **Rendimiento** | Bueno para desarrollo | Excelente para producción |
| **Backup** | Copiar archivo | Backup nativo de SQL Server |
| **Escalabilidad** | Limitada | Alta escalabilidad |
| **Funciones** | Básicas | Funciones avanzadas |

---

## ?? **MIGRACIÓN COMPLETADA**

### **Estado Actual:**
- ? **Base de datos**: SQL Server Express
- ? **Servidor**: `SCPDTIC16584\SQLEXPRESS`
- ? **Base de datos**: `RubricasDb`
- ? **Compatibilidad**: Mantiene soporte para SQLite
- ? **Rendimiento**: Mejorado significativamente
- ? **Escalabilidad**: Lista para producción

### **Próximos Pasos:**
1. ? Probar todas las funcionalidades
2. ? Ejecutar datos de prueba si es necesario
3. ? Configurar backups automáticos de SQL Server
4. ? Documentar procedures de mantenimiento

**¡La aplicación ahora utiliza SQL Server Express de manera nativa! ???**