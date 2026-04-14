-- =====================================================================================
-- Script para verificar permisos del usuario admin@rubricas.edu
-- =====================================================================================

DECLARE @UserId NVARCHAR(450);
SELECT @UserId = Id FROM AspNetUsers WHERE Email = 'admin@rubricas.edu';

IF @UserId IS NULL
BEGIN
    PRINT 'Usuario admin@rubricas.edu no encontrado';
    RETURN;
END

PRINT '=== INFORMACIÓN DEL USUARIO ===';
SELECT 
    Id,
    UserName,
    Email,
    Nombre + ' ' + Apellidos as NombreCompleto,
    IsActive,
    Activo,
    EmailConfirmed,
    CreatedDate,
    LastLoginDate
FROM AspNetUsers 
WHERE Id = @UserId;

PRINT '';
PRINT '=== ROLES ASIGNADOS ===';
SELECT 
    r.Name as RoleName,
    r.NormalizedName
FROM AspNetRoles r
INNER JOIN AspNetUserRoles ur ON r.Id = ur.RoleId
WHERE ur.UserId = @UserId;

PRINT '';
PRINT '=== PERMISOS DIRECTOS DEL USUARIO ===';
SELECT 
    uc.ClaimValue as Permiso,
    uc.ClaimType
FROM AspNetUserClaims uc
WHERE uc.UserId = @UserId 
AND uc.ClaimType = 'permission'
ORDER BY uc.ClaimValue;

PRINT '';
PRINT '=== PERMISOS A TRAVÉS DE ROLES ===';
SELECT DISTINCT
    rc.ClaimValue as Permiso,
    r.Name as RoleName
FROM AspNetRoleClaims rc
INNER JOIN AspNetRoles r ON rc.RoleId = r.Id
INNER JOIN AspNetUserRoles ur ON r.Id = ur.RoleId
WHERE ur.UserId = @UserId 
AND rc.ClaimType = 'permission'
ORDER BY rc.ClaimValue;

PRINT '';
PRINT '=== RESUMEN DE PERMISOS ===';
SELECT 
    'Permisos directos del usuario' as TipoPermiso,
    COUNT(*) as Cantidad
FROM AspNetUserClaims 
WHERE UserId = @UserId AND ClaimType = 'permission'

UNION ALL

SELECT 
    'Permisos a través de roles' as TipoPermiso,
    COUNT(DISTINCT rc.ClaimValue) as Cantidad
FROM AspNetRoleClaims rc
INNER JOIN AspNetUserRoles ur ON rc.RoleId = ur.RoleId
WHERE ur.UserId = @UserId AND rc.ClaimType = 'permission';

PRINT '';
PRINT '=== PERMISOS POR CATEGORÍA ===';
SELECT 
    CASE 
        WHEN ClaimValue LIKE 'usuarios.%' THEN 'Usuarios'
        WHEN ClaimValue LIKE 'rubricas.%' THEN 'Rúbricas'
        WHEN ClaimValue LIKE 'evaluaciones.%' THEN 'Evaluaciones'
        WHEN ClaimValue LIKE 'estudiantes.%' THEN 'Estudiantes'
        WHEN ClaimValue LIKE 'reportes.%' THEN 'Reportes'
        WHEN ClaimValue LIKE 'configuracion.%' THEN 'Configuración'
        WHEN ClaimValue LIKE 'auditoria.%' THEN 'Auditoría'
        WHEN ClaimValue LIKE 'periodos.%' THEN 'Períodos'
        WHEN ClaimValue LIKE 'niveles.%' THEN 'Niveles'
        ELSE 'Otros'
    END as Categoria,
    COUNT(*) as CantidadPermisos
FROM (
    SELECT ClaimValue 
    FROM AspNetUserClaims 
    WHERE UserId = @UserId AND ClaimType = 'permission'
    
    UNION
    
    SELECT DISTINCT rc.ClaimValue
    FROM AspNetRoleClaims rc
    INNER JOIN AspNetUserRoles ur ON rc.RoleId = ur.RoleId
    WHERE ur.UserId = @UserId AND rc.ClaimType = 'permission'
) AS AllPermisos
GROUP BY 
    CASE 
        WHEN ClaimValue LIKE 'usuarios.%' THEN 'Usuarios'
        WHEN ClaimValue LIKE 'rubricas.%' THEN 'Rúbricas'
        WHEN ClaimValue LIKE 'evaluaciones.%' THEN 'Evaluaciones'
        WHEN ClaimValue LIKE 'estudiantes.%' THEN 'Estudiantes'
        WHEN ClaimValue LIKE 'reportes.%' THEN 'Reportes'
        WHEN ClaimValue LIKE 'configuracion.%' THEN 'Configuración'
        WHEN ClaimValue LIKE 'auditoria.%' THEN 'Auditoría'
        WHEN ClaimValue LIKE 'periodos.%' THEN 'Períodos'
        WHEN ClaimValue LIKE 'niveles.%' THEN 'Niveles'
        ELSE 'Otros'
    END
ORDER BY CantidadPermisos DESC;