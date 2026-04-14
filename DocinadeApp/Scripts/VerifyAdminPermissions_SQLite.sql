-- =====================================================================================
-- Script SQLite para verificar permisos del usuario admin@rubricas.edu
-- Sistema de Rúbricas Académicas
-- =====================================================================================

SELECT '=== VERIFICACIÓN DE PERMISOS PARA admin@rubricas.edu ===';

-- Verificar si el usuario existe
SELECT 
    CASE 
        WHEN EXISTS(SELECT 1 FROM AspNetUsers WHERE UserName = 'admin@rubricas.edu' OR Email = 'admin@rubricas.edu')
        THEN 'Usuario admin@rubricas.edu ENCONTRADO'
        ELSE 'ERROR: Usuario admin@rubricas.edu NO EXISTE'
    END as EstadoUsuario;

-- Información básica del usuario
SELECT '=== INFORMACIÓN DEL USUARIO ===' as Seccion;
SELECT 
    UserName as Usuario,
    Email,
    CASE WHEN IsActive = 1 THEN 'Activo' ELSE 'Inactivo' END as Estado,
    CASE WHEN EmailConfirmed = 1 THEN 'Confirmado' ELSE 'No confirmado' END as EmailStatus,
    CASE WHEN LockoutEnd IS NULL THEN 'Desbloqueado' ELSE 'Bloqueado hasta ' || LockoutEnd END as EstadoBloqueo,
    CreatedDate as FechaCreacion,
    LastLoginDate as UltimoAcceso
FROM AspNetUsers 
WHERE UserName = 'admin@rubricas.edu' OR Email = 'admin@rubricas.edu';

-- Roles asignados
SELECT '=== ROLES ASIGNADOS ===' as Seccion;
SELECT 
    r.Name as Rol,
    r.NormalizedName as RolNormalizado
FROM AspNetUsers u
JOIN AspNetUserRoles ur ON u.Id = ur.UserId
JOIN AspNetRoles r ON ur.RoleId = r.Id
WHERE u.UserName = 'admin@rubricas.edu' OR u.Email = 'admin@rubricas.edu'
ORDER BY r.Name;

-- Conteo total de permisos directos
SELECT '=== RESUMEN DE PERMISOS DIRECTOS ===' as Seccion;
SELECT 
    'Permisos directos asignados al usuario' as Tipo,
    COUNT(*) as Cantidad
FROM AspNetUsers u
JOIN AspNetUserClaims uc ON u.Id = uc.UserId
WHERE (u.UserName = 'admin@rubricas.edu' OR u.Email = 'admin@rubricas.edu')
AND uc.ClaimType = 'permission';

-- Permisos por categoría (directos del usuario)
SELECT '=== PERMISOS DIRECTOS POR CATEGORÍA ===' as Seccion;
SELECT 
    CASE 
        WHEN uc.ClaimValue LIKE 'usuarios.%' THEN 'Usuarios'
        WHEN uc.ClaimValue LIKE 'rubricas.%' THEN 'Rúbricas'
        WHEN uc.ClaimValue LIKE 'evaluaciones.%' THEN 'Evaluaciones'
        WHEN uc.ClaimValue LIKE 'estudiantes.%' THEN 'Estudiantes'
        WHEN uc.ClaimValue LIKE 'reportes.%' THEN 'Reportes'
        WHEN uc.ClaimValue LIKE 'configuracion.%' THEN 'Configuración'
        WHEN uc.ClaimValue LIKE 'auditoria.%' THEN 'Auditoría'
        WHEN uc.ClaimValue LIKE 'periodos.%' THEN 'Períodos'
        WHEN uc.ClaimValue LIKE 'niveles.%' THEN 'Niveles'
        WHEN uc.ClaimValue LIKE 'materias.%' THEN 'Materias'
        WHEN uc.ClaimValue LIKE 'admin.%' THEN 'Administración'
        ELSE 'Otros'
    END as Categoria,
    COUNT(*) as CantidadPermisos
FROM AspNetUsers u
JOIN AspNetUserClaims uc ON u.Id = uc.UserId
WHERE (u.UserName = 'admin@rubricas.edu' OR u.Email = 'admin@rubricas.edu')
AND uc.ClaimType = 'permission'
GROUP BY 
    CASE 
        WHEN uc.ClaimValue LIKE 'usuarios.%' THEN 'Usuarios'
        WHEN uc.ClaimValue LIKE 'rubricas.%' THEN 'Rúbricas'
        WHEN uc.ClaimValue LIKE 'evaluaciones.%' THEN 'Evaluaciones'
        WHEN uc.ClaimValue LIKE 'estudiantes.%' THEN 'Estudiantes'
        WHEN uc.ClaimValue LIKE 'reportes.%' THEN 'Reportes'
        WHEN uc.ClaimValue LIKE 'configuracion.%' THEN 'Configuración'
        WHEN uc.ClaimValue LIKE 'auditoria.%' THEN 'Auditoría'
        WHEN uc.ClaimValue LIKE 'periodos.%' THEN 'Períodos'
        WHEN uc.ClaimValue LIKE 'niveles.%' THEN 'Niveles'
        WHEN uc.ClaimValue LIKE 'materias.%' THEN 'Materias'
        WHEN uc.ClaimValue LIKE 'admin.%' THEN 'Administración'
        ELSE 'Otros'
    END
ORDER BY CantidadPermisos DESC;

-- Permisos específicos importantes (muestra de algunos)
SELECT '=== ALGUNOS PERMISOS CLAVE ASIGNADOS ===' as Seccion;
SELECT 
    uc.ClaimValue as Permiso
FROM AspNetUsers u
JOIN AspNetUserClaims uc ON u.Id = uc.UserId
WHERE (u.UserName = 'admin@rubricas.edu' OR u.Email = 'admin@rubricas.edu')
AND uc.ClaimType = 'permission'
AND uc.ClaimValue IN (
    'usuarios.ver', 'usuarios.crear', 'usuarios.gestionar_roles',
    'rubricas.ver_todas', 'rubricas.crear', 'rubricas.editar_todas',
    'evaluaciones.ver_todas', 'evaluaciones.crear', 'evaluaciones.editar_todas',
    'configuracion.gestionar', 'configuracion.gestionar_permisos',
    'admin.acceso_completo', 'admin.gestionar_sistema'
)
ORDER BY uc.ClaimValue;

-- Verificar permisos a través de roles
SELECT '=== PERMISOS A TRAVÉS DE ROLES ===' as Seccion;
SELECT 
    r.Name as Rol,
    COUNT(rc.ClaimValue) as TotalPermisos
FROM AspNetUsers u
JOIN AspNetUserRoles ur ON u.Id = ur.UserId
JOIN AspNetRoles r ON ur.RoleId = r.Id
LEFT JOIN AspNetRoleClaims rc ON r.Id = rc.RoleId AND rc.ClaimType = 'permission'
WHERE u.UserName = 'admin@rubricas.edu' OR u.Email = 'admin@rubricas.edu'
GROUP BY r.Id, r.Name
ORDER BY TotalPermisos DESC;

-- Estado de todos los roles del sistema
SELECT '=== TODOS LOS ROLES DEL SISTEMA ===' as Seccion;
SELECT 
    r.Name as Rol,
    COUNT(rc.ClaimValue) as PermisosAsignados,
    COUNT(ur.UserId) as UsuariosConEsteRol
FROM AspNetRoles r
LEFT JOIN AspNetRoleClaims rc ON r.Id = rc.RoleId AND rc.ClaimType = 'permission'
LEFT JOIN AspNetUserRoles ur ON r.Id = ur.RoleId
GROUP BY r.Id, r.Name
ORDER BY PermisosAsignados DESC;

-- Resumen final
SELECT '=== RESUMEN FINAL ===' as Seccion;
SELECT 
    'El usuario admin@rubricas.edu está configurado correctamente' as Estado
WHERE EXISTS(
    SELECT 1 FROM AspNetUsers u
    JOIN AspNetUserClaims uc ON u.Id = uc.UserId
    WHERE (u.UserName = 'admin@rubricas.edu' OR u.Email = 'admin@rubricas.edu')
    AND uc.ClaimType = 'permission'
    AND uc.ClaimValue = 'admin.acceso_completo'
)
UNION ALL
SELECT 
    'ADVERTENCIA: El usuario admin@rubricas.edu NO tiene permisos de administrador' as Estado
WHERE NOT EXISTS(
    SELECT 1 FROM AspNetUsers u
    JOIN AspNetUserClaims uc ON u.Id = uc.UserId
    WHERE (u.UserName = 'admin@rubricas.edu' OR u.Email = 'admin@rubricas.edu')
    AND uc.ClaimType = 'permission'
    AND uc.ClaimValue = 'admin.acceso_completo'
);

SELECT '===============================================================';
SELECT 'VERIFICACIÓN COMPLETADA';
SELECT '===============================================================';