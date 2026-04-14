-- =====================================================================================
-- Script para asignar rol SuperAdmin al usuario Administrador
-- Sistema de Rúbricas Académicas - SQLite
-- =====================================================================================

-- Verificar datos antes de la inserción
SELECT 'Verificando usuario Administrador...';
SELECT 
    Id,
    UserName,
    Email,
    Nombre,
    Apellidos
FROM AspNetUsers 
WHERE Nombre = 'Administrador';

SELECT 'Verificando rol SuperAdmin...';
SELECT 
    Id,
    Name,
    NormalizedName
FROM AspNetRoles 
WHERE Name = 'SuperAdmin';

-- =====================================================================================
-- OPCIÓN 1: Script corregido (más seguro)
-- =====================================================================================

-- Insertar en AspNetUserRoles con verificación
INSERT INTO AspNetUserRoles (UserId, RoleId)
SELECT 
    u.Id as UserId,
    r.Id as RoleId
FROM AspNetUsers u, AspNetRoles r
WHERE u.Nombre = 'Administrador'
AND r.Name = 'SuperAdmin'
AND NOT EXISTS (
    SELECT 1 FROM AspNetUserRoles ur 
    WHERE ur.UserId = u.Id 
    AND ur.RoleId = r.Id
);

-- Verificar que se insertó correctamente
SELECT 'Verificando asignación de rol...';
SELECT 
    u.UserName,
    u.Nombre,
    u.Email,
    r.Name as Rol
FROM AspNetUsers u
JOIN AspNetUserRoles ur ON u.Id = ur.UserId
JOIN AspNetRoles r ON ur.RoleId = r.Id
WHERE u.Nombre = 'Administrador'
AND r.Name = 'SuperAdmin';

-- =====================================================================================
-- OPCIÓN 2: Tu script original corregido
-- =====================================================================================

-- Versión corregida de tu consulta original:
/*
INSERT INTO AspNetUserRoles (UserId, RoleId)
SELECT 
    (SELECT Id FROM AspNetUsers WHERE Nombre = 'Administrador'),
    (SELECT Id FROM AspNetRoles WHERE Name = 'SuperAdmin');
*/

-- =====================================================================================
-- OPCIÓN 3: Script con manejo de errores más robusto
-- =====================================================================================

-- Crear script con validaciones completas
/*
-- Verificar que existe el usuario
INSERT INTO AspNetUserRoles (UserId, RoleId)
SELECT u.Id, r.Id
FROM 
    (SELECT Id FROM AspNetUsers WHERE Nombre = 'Administrador' LIMIT 1) u,
    (SELECT Id FROM AspNetRoles WHERE Name = 'SuperAdmin' LIMIT 1) r
WHERE EXISTS (SELECT 1 FROM AspNetUsers WHERE Nombre = 'Administrador')
AND EXISTS (SELECT 1 FROM AspNetRoles WHERE Name = 'SuperAdmin')
AND NOT EXISTS (
    SELECT 1 FROM AspNetUserRoles 
    WHERE UserId = u.Id AND RoleId = r.Id
);
*/

SELECT 'Script completado - Rol SuperAdmin asignado al usuario Administrador';