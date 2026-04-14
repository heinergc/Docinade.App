-- DIAGNÓSTICO FINAL DEL SISTEMA EN PRODUCCIÓN
-- Verificación completa del estado de la base de datos Azure SQL

PRINT '[INFO] =========================================='
PRINT '[INFO] DIAGNÓSTICO FINAL - SISTEMA PRODUCCIÓN'
PRINT '[INFO] =========================================='
PRINT ''

-- 1. Verificar usuario admin
PRINT '[1] VERIFICANDO USUARIO ADMIN:'
SELECT 
    u.Id,
    u.UserName,
    u.Email,
    u.Activo,
    u.Nombre,
    u.Apellidos,
    u.EmailConfirmed,
    u.LockoutEnabled,
    u.AccessFailedCount
FROM AspNetUsers u 
WHERE u.Email = 'admin@rubricas.edu'

-- 2. Verificar roles del usuario admin
PRINT '[2] ROLES DEL USUARIO ADMIN:'
SELECT 
    r.Name as RoleName,
    r.NormalizedName
FROM AspNetUsers u
INNER JOIN AspNetUserRoles ur ON u.Id = ur.UserId
INNER JOIN AspNetRoles r ON ur.RoleId = r.Id
WHERE u.Email = 'admin@rubricas.edu'

-- 3. Contar permisos del rol SuperAdmin
PRINT '[3] CONTEO DE PERMISOS SUPERADMIN:'
SELECT 
    COUNT(*) as TotalPermisos
FROM AspNetRoles r
INNER JOIN AspNetRoleClaims rc ON r.Id = rc.RoleId
WHERE r.NormalizedName = 'SUPERADMIN' 
AND rc.ClaimType = 'permission'

-- 4. Verificar algunos permisos específicos clave
PRINT '[4] PERMISOS CLAVE VERIFICADOS:'
SELECT 
    rc.ClaimValue as Permiso,
    CASE WHEN rc.Id IS NOT NULL THEN 'ASIGNADO' ELSE 'FALTANTE' END as Estado
FROM AspNetRoles r
LEFT JOIN AspNetRoleClaims rc ON r.Id = rc.RoleId 
    AND rc.ClaimType = 'permission' 
    AND rc.ClaimValue IN ('usuarios.ver', 'estudiantes.ver', 'evaluaciones.crear', 'rubricas.crear', 'dashboard.acceso')
WHERE r.NormalizedName = 'SUPERADMIN'

-- 5. Estado de la inicialización de datos
PRINT '[5] ESTADO DE DATOS INICIALIZADOS:'
SELECT 
    'Usuarios' as Tabla,
    COUNT(*) as Total
FROM AspNetUsers
UNION ALL
SELECT 
    'Roles' as Tabla,
    COUNT(*) as Total
FROM AspNetRoles
UNION ALL
SELECT 
    'Provincias' as Tabla,
    COUNT(*) as Total
FROM Provincias
UNION ALL
SELECT 
    'TiposFalta' as Tabla,
    COUNT(*) as Total
FROM TiposFalta

-- 6. Verificar sistema de permisos completo
PRINT '[6] RESUMEN SISTEMA DE PERMISOS:'
SELECT 
    r.Name as Rol,
    COUNT(rc.Id) as PermisosAsignados
FROM AspNetRoles r
LEFT JOIN AspNetRoleClaims rc ON r.Id = rc.RoleId AND rc.ClaimType = 'permission'
GROUP BY r.Name, r.Id
ORDER BY COUNT(rc.Id) DESC

PRINT ''
PRINT '[SUCCESS] =========================================='
PRINT '[SUCCESS] DIAGNÓSTICO COMPLETADO'
PRINT '[SUCCESS] =========================================='