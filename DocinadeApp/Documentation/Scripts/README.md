# Scripts SQL para Configuración del Usuario Administrador

Este directorio contiene scripts SQL para garantizar que el usuario `admin@rubricas.edu` tenga todos los permisos del sistema en SQLite.

## 📋 Archivos Disponibles

### 1. `SetupAdminUser_SQLite.sql` - Script Completo
- **Propósito**: Script completo y detallado para configurar todos los permisos
- **Características**:
  - Crea todos los roles del sistema (SuperAdmin, Admin, Profesor, Estudiante)
  - Asigna **93 permisos** organizados en 11 categorías
  - Incluye verificaciones y reportes detallados
  - Documentación completa de cada paso

### 2. `QuickAdminSetup_SQLite.sql` - Script Rápido
- **Propósito**: Versión compacta para configuración rápida
- **Características**:
  - Configuración mínima pero completa
  - Ejecución rápida
  - Ideal para automatización

### 3. `VerifyAdminPermissions_SQLite.sql` - Verificación
- **Propósito**: Verificar que los permisos estén correctamente asignados
- **Características**:
  - Muestra información detallada del usuario
  - Lista roles asignados
  - Cuenta permisos por categoría
  - Identifica problemas de configuración

### 4. Scripts Legacy
- `GrantAllPermissionsToAdmin_SQLite.sql` - Script original
- `VerifyAdminPermissions.sql` - Verificación para SQL Server

## 🚀 Instrucciones de Uso

### Escenario 1: Base de Datos Nueva (Recomendado)
```bash
# 1. Crear/eliminar base de datos
Remove-Item RubricasDb.db*

# 2. Ejecutar aplicación para crear estructura
dotnet run

# 3. Registrar usuario admin@rubricas.edu en la interfaz web
# 4. Ejecutar script de permisos
sqlite3 RubricasDb.db < Scripts/SetupAdminUser_SQLite.sql

# 5. Verificar configuración
sqlite3 RubricasDb.db < Scripts/VerifyAdminPermissions_SQLite.sql
```

### Escenario 2: Base de Datos Existente
```bash
# 1. Ejecutar script rápido
sqlite3 RubricasDb.db < Scripts/QuickAdminSetup_SQLite.sql

# 2. Verificar
sqlite3 RubricasDb.db < Scripts/VerifyAdminPermissions_SQLite.sql
```

### Escenario 3: Solo Verificación
```bash
sqlite3 RubricasDb.db < Scripts/VerifyAdminPermissions_SQLite.sql
```

## 📊 Permisos Incluidos

El script configura **93 permisos** organizados en las siguientes categorías:

| Categoría | Permisos | Descripción |
|-----------|----------|-------------|
| **Usuarios** | 12 | Gestión completa de usuarios, roles y permisos |
| **Rúbricas** | 13 | Creación, edición, compartir rúbricas |
| **Evaluaciones** | 15 | Gestión completa del proceso de evaluación |
| **Estudiantes** | 9 | Administración de información de estudiantes |
| **Reportes** | 7 | Generación y exportación de reportes |
| **Configuración** | 14 | Configuración del sistema y seguridad |
| **Auditoría** | 7 | Logs, métricas y seguimiento |
| **Períodos** | 7 | Gestión de períodos académicos |
| **Niveles** | 6 | Configuración de niveles de calificación |
| **Materias** | 7 | Gestión de materias y ofertas académicas |
| **Administración** | 3 | Acceso completo al panel de administración |

## ⚠️ Notas Importantes

1. **Seguridad**: Estos scripts asignan permisos de administrador completo. Úselos solo para cuentas de administrador.

2. **Idempotencia**: Los scripts son seguros de ejecutar múltiples veces (usan `INSERT OR IGNORE`).

3. **Prerequisitos**: El usuario `admin@rubricas.edu` debe existir antes de ejecutar los scripts.

4. **Base de Datos**: Los scripts están diseñados específicamente para SQLite.

## 🔧 Solución de Problemas

### Error: "Usuario no encontrado"
```sql
-- Verificar si el usuario existe
SELECT * FROM AspNetUsers WHERE Email = 'admin@rubricas.edu';
```
**Solución**: Registrar el usuario en la interfaz web primero.

### Error: "No se pueden asignar permisos"
```sql
-- Verificar estructura de tablas
.schema AspNetUsers
.schema AspNetUserClaims
```
**Solución**: Asegurarse de que la base de datos esté completamente inicializada.

### Permisos no aparecen en la interfaz
**Solución**: 
1. Reiniciar la aplicación
2. Limpiar cache del navegador
3. Verificar con el script de verificación

## 📝 Ejemplo de Uso

```bash
# Reiniciar sistema completo
Remove-Item RubricasDb.db*
dotnet run --urls="http://localhost:5000" &

# Registrar admin@rubricas.edu en http://localhost:5000/Account/Register

# Configurar permisos
sqlite3 RubricasDb.db < Scripts/SetupAdminUser_SQLite.sql

# Verificar resultado
sqlite3 RubricasDb.db < Scripts/VerifyAdminPermissions_SQLite.sql
```

## 🔄 Mantenimiento

Para mantener los permisos actualizados cuando se agreguen nuevas funcionalidades:

1. Actualizar `ApplicationPermissions.cs`
2. Actualizar los scripts SQL con los nuevos permisos
3. Ejecutar `QuickAdminSetup_SQLite.sql` para aplicar cambios
4. Verificar con `VerifyAdminPermissions_SQLite.sql`

---

**Última actualización**: 13 de agosto de 2025  
**Versión del sistema**: ASP.NET Core 8 MVC  
**Base de datos**: SQLite