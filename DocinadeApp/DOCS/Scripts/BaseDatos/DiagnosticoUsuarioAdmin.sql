-- Script de Diagnóstico Completo para Usuario Admin
-- Fecha: 2025-12-09
-- Propósito: Verificar por qué el usuario admin se autentica pero no tiene permisos

SET NOCOUNT ON;
GO

PRINT '=== DIAGNÓSTICO COMPLETO DEL USUARIO ADMIN ===';
PRINT 'Fecha: ' + CONVERT(VARCHAR, GETDATE(), 120);
PRINT '';

-- 1. Verificar existencia del usuario
PRINT '1. VERIFICACIÓN DEL USUARIO:';
PRINT '============================';
SELECT 
    Id as UserId,
    Email,
    EmailConfirmed,
    Activo,
    LockoutEnabled,
    LockoutEnd,
    AccessFailedCount,
    SecurityStamp
FROM AspNetUsers 
WHERE Email = 'admin@rubricas.edu';

PRINT '';

-- 2. Verificar roles asignados al usuario
PRINT '2. ROLES ASIGNADOS AL USUARIO:';
PRINT '==============================';
SELECT 
    u.Email,
    r.Id as RoleId,
    r.Name as RoleName,
    r.NormalizedName,
    ur.UserId,
    ur.RoleId as UserRoleId
FROM AspNetUsers u
LEFT JOIN AspNetUserRoles ur ON u.Id = ur.UserId
LEFT JOIN AspNetRoles r ON ur.RoleId = r.Id
WHERE u.Email = 'admin@rubricas.edu';

PRINT '';

-- 3. Verificar todos los roles existentes en el sistema
PRINT '3. TODOS LOS ROLES DEL SISTEMA:';
PRINT '===============================';
SELECT 
    Id,
    Name,
    NormalizedName,
    ConcurrencyStamp
FROM AspNetRoles
ORDER BY Name;

PRINT '';

-- 4. Contar permisos por rol
PRINT '4. PERMISOS POR ROL:';
PRINT '===================';
SELECT 
    r.Name as RoleName,
    COUNT(rc.Id) as TotalPermisos
FROM AspNetRoles r
LEFT JOIN AspNetRoleClaims rc ON r.Id = rc.RoleId AND rc.ClaimType = 'Permission'
GROUP BY r.Name, r.Id
ORDER BY COUNT(rc.Id) DESC;

PRINT '';

-- 5. Verificar permisos específicos del rol SuperAdministrador
PRINT '5. PERMISOS DEL ROL SUPERADMINISTRADOR:';
PRINT '======================================';
SELECT 
    rc.ClaimType,
    rc.ClaimValue,
    r.Name as RoleName
FROM AspNetRoleClaims rc
INNER JOIN AspNetRoles r ON rc.RoleId = r.Id
WHERE r.NormalizedName = 'SUPERADMINISTRADOR'
    AND rc.ClaimType = 'Permission'
ORDER BY rc.ClaimValue;

PRINT '';

-- 6. Verificar Claims directos del usuario (no por rol)
PRINT '6. CLAIMS DIRECTOS DEL USUARIO:';
PRINT '===============================';
SELECT 
    u.Email,
    uc.ClaimType,
    uc.ClaimValue
FROM AspNetUsers u
LEFT JOIN AspNetUserClaims uc ON u.Id = uc.UserId
WHERE u.Email = 'admin@rubricas.edu';

PRINT '';

-- 7. Resumen consolidado de permisos del usuario
PRINT '7. RESUMEN CONSOLIDADO DE PERMISOS:';
PRINT '==================================';
-- Permisos por rol
SELECT 
    'Por Rol' as TipoPermiso,
    rc.ClaimValue as Permiso,
    r.Name as Origen
FROM AspNetUsers u
INNER JOIN AspNetUserRoles ur ON u.Id = ur.UserId
INNER JOIN AspNetRoles r ON ur.RoleId = r.Id
INNER JOIN AspNetRoleClaims rc ON r.Id = rc.RoleId
WHERE u.Email = 'admin@rubricas.edu' 
    AND rc.ClaimType = 'Permission'

UNION ALL

-- Permisos directos
SELECT 
    'Directo' as TipoPermiso,
    uc.ClaimValue as Permiso,
    'Usuario' as Origen
FROM AspNetUsers u
INNER JOIN AspNetUserClaims uc ON u.Id = uc.UserId
WHERE u.Email = 'admin@rubricas.edu' 
    AND uc.ClaimType = 'Permission'

ORDER BY TipoPermiso, Permiso;

PRINT '';

-- 8. Diagnóstico de problemas comunes
PRINT '8. DIAGNÓSTICO DE PROBLEMAS:';
PRINT '============================';

-- Verificar si el usuario existe
IF NOT EXISTS (SELECT 1 FROM AspNetUsers WHERE Email = 'admin@rubricas.edu')
    PRINT '❌ PROBLEMA: Usuario admin@rubricas.edu NO EXISTE';
ELSE
    PRINT '✅ Usuario admin@rubricas.edu existe';

-- Verificar si el usuario está activo
IF EXISTS (SELECT 1 FROM AspNetUsers WHERE Email = 'admin@rubricas.edu' AND Activo = 0)
    PRINT '❌ PROBLEMA: Usuario está INACTIVO (Activo = 0)';
ELSE
    PRINT '✅ Usuario está activo';

-- Verificar si el usuario tiene roles
IF NOT EXISTS (
    SELECT 1 FROM AspNetUsers u
    INNER JOIN AspNetUserRoles ur ON u.Id = ur.UserId
    WHERE u.Email = 'admin@rubricas.edu'
)
    PRINT '❌ PROBLEMA: Usuario NO tiene roles asignados';
ELSE
    PRINT '✅ Usuario tiene roles asignados';

-- Verificar si el rol SuperAdministrador existe
IF NOT EXISTS (SELECT 1 FROM AspNetRoles WHERE NormalizedName = 'SUPERADMINISTRADOR')
    PRINT '❌ PROBLEMA: Rol SuperAdministrador NO EXISTE';
ELSE
    PRINT '✅ Rol SuperAdministrador existe';

-- Verificar si el rol tiene permisos
IF NOT EXISTS (
    SELECT 1 FROM AspNetRoles r
    INNER JOIN AspNetRoleClaims rc ON r.Id = rc.RoleId
    WHERE r.NormalizedName = 'SUPERADMINISTRADOR' AND rc.ClaimType = 'Permission'
)
    PRINT '❌ PROBLEMA: Rol SuperAdministrador NO tiene permisos asignados';
ELSE
    PRINT '✅ Rol SuperAdministrador tiene permisos';

-- Verificar si el usuario está bloqueado
IF EXISTS (
    SELECT 1 FROM AspNetUsers 
    WHERE Email = 'admin@rubricas.edu' 
    AND LockoutEnd IS NOT NULL 
    AND LockoutEnd > GETDATE()
)
    PRINT '❌ PROBLEMA: Usuario está BLOQUEADO temporalmente';
ELSE
    PRINT '✅ Usuario no está bloqueado';

PRINT '';
PRINT '=== FIN DEL DIAGNÓSTICO ===';

GO