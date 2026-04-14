-- =====================================================================================
-- Script SQLite SIMPLIFICADO para otorgar permisos básicos al usuario admin@rubricas.edu
-- Sistema de Rúbricas Académicas - Versión Simplificada para SQLite
-- =====================================================================================

-- Verificar si el usuario existe
SELECT 'Verificando usuario admin@rubricas.edu...';

SELECT CASE 
    WHEN EXISTS(SELECT 1 FROM AspNetUsers WHERE Email = 'admin@rubricas.edu')
    THEN 'Usuario encontrado'
    ELSE 'Usuario no encontrado - necesita crearlo primero'
END as StatusUsuario;

-- =====================================================================================
-- PASO 1: Crear/Verificar roles básicos
-- =====================================================================================

-- Crear rol Admin si no existe
INSERT OR IGNORE INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
VALUES (
    lower(hex(randomblob(16))), 
    'Admin', 
    'ADMIN', 
    lower(hex(randomblob(16)))
);

-- Crear rol SuperAdmin si no existe
INSERT OR IGNORE INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
VALUES (
    lower(hex(randomblob(16))), 
    'SuperAdmin', 
    'SUPERADMIN', 
    lower(hex(randomblob(16)))
);

SELECT 'Roles Admin y SuperAdmin verificados/creados';

-- =====================================================================================
-- PASO 2: Asignar roles al usuario
-- =====================================================================================

-- Asignar rol Admin al usuario
INSERT OR IGNORE INTO AspNetUserRoles (UserId, RoleId)
SELECT 
    u.Id,
    r.Id
FROM AspNetUsers u, AspNetRoles r
WHERE u.Email = 'admin@rubricas.edu'
AND r.Name = 'Admin';

-- Asignar rol SuperAdmin al usuario
INSERT OR IGNORE INTO AspNetUserRoles (UserId, RoleId)
SELECT 
    u.Id,
    r.Id
FROM AspNetUsers u, AspNetRoles r
WHERE u.Email = 'admin@rubricas.edu'
AND r.Name = 'SuperAdmin';

SELECT 'Roles asignados al usuario admin@rubricas.edu';

-- =====================================================================================
-- PASO 3: Asignar permisos críticos directamente al usuario
-- =====================================================================================

-- Crear tabla temporal con permisos esenciales
CREATE TEMP TABLE PermisosEsenciales (permiso TEXT);

INSERT INTO PermisosEsenciales VALUES ('configuracion.ver');
INSERT INTO PermisosEsenciales VALUES ('configuracion.gestionar');
INSERT INTO PermisosEsenciales VALUES ('configuracion.inicializar');
INSERT INTO PermisosEsenciales VALUES ('usuarios.ver');
INSERT INTO PermisosEsenciales VALUES ('usuarios.crear');
INSERT INTO PermisosEsenciales VALUES ('usuarios.editar');
INSERT INTO PermisosEsenciales VALUES ('usuarios.eliminar');
INSERT INTO PermisosEsenciales VALUES ('usuarios.gestionar_roles');
INSERT INTO PermisosEsenciales VALUES ('usuarios.gestionar_permisos');
INSERT INTO PermisosEsenciales VALUES ('rubricas.ver_todas');
INSERT INTO PermisosEsenciales VALUES ('rubricas.crear');
INSERT INTO PermisosEsenciales VALUES ('rubricas.editar_todas');
INSERT INTO PermisosEsenciales VALUES ('rubricas.eliminar_todas');
INSERT INTO PermisosEsenciales VALUES ('evaluaciones.ver_todas');
INSERT INTO PermisosEsenciales VALUES ('evaluaciones.crear');
INSERT INTO PermisosEsenciales VALUES ('evaluaciones.editar_todas');
INSERT INTO PermisosEsenciales VALUES ('estudiantes.ver');
INSERT INTO PermisosEsenciales VALUES ('estudiantes.crear');
INSERT INTO PermisosEsenciales VALUES ('estudiantes.editar');
INSERT INTO PermisosEsenciales VALUES ('estudiantes.importar');
INSERT INTO PermisosEsenciales VALUES ('reportes.ver_todos');
INSERT INTO PermisosEsenciales VALUES ('auditoria.ver');
INSERT INTO PermisosEsenciales VALUES ('periodos.ver');
INSERT INTO PermisosEsenciales VALUES ('periodos.crear');
INSERT INTO PermisosEsenciales VALUES ('periodos.editar');
INSERT INTO PermisosEsenciales VALUES ('niveles.ver');
INSERT INTO PermisosEsenciales VALUES ('niveles.crear');
INSERT INTO PermisosEsenciales VALUES ('niveles.editar');

-- Asignar permisos esenciales al usuario
INSERT OR IGNORE INTO AspNetUserClaims (UserId, ClaimType, ClaimValue)
SELECT 
    u.Id,
    'permission',
    pe.permiso
FROM AspNetUsers u, PermisosEsenciales pe
WHERE u.Email = 'admin@rubricas.edu';

-- Asignar permisos esenciales al rol Admin
INSERT OR IGNORE INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue)
SELECT 
    r.Id,
    'permission',
    pe.permiso
FROM AspNetRoles r, PermisosEsenciales pe
WHERE r.Name = 'Admin';

-- Asignar permisos esenciales al rol SuperAdmin
INSERT OR IGNORE INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue)
SELECT 
    r.Id,
    'permission',
    pe.permiso
FROM AspNetRoles r, PermisosEsenciales pe
WHERE r.Name = 'SuperAdmin';

SELECT 'Permisos esenciales asignados: ' || (SELECT COUNT(*) FROM PermisosEsenciales) || ' permisos';

-- =====================================================================================
-- PASO 4: Activar y confirmar usuario
-- =====================================================================================

-- Asegurar que el usuario esté activo y confirmado
UPDATE AspNetUsers 
SET 
    IsActive = 1,
    Activo = 1,
    EmailConfirmed = 1,
    PhoneNumberConfirmed = COALESCE(PhoneNumberConfirmed, 0),
    LockoutEnabled = 0,
    LockoutEnd = NULL,
    AccessFailedCount = 0
WHERE Email = 'admin@rubricas.edu';

SELECT 'Usuario admin@rubricas.edu activado y confirmado';

-- =====================================================================================
-- PASO 5: Resumen de configuración
-- =====================================================================================

SELECT 'RESUMEN DE CONFIGURACIÓN:';

-- Información del usuario
SELECT 
    u.UserName as Usuario,
    u.Email,
    u.Nombre || ' ' || u.Apellidos as NombreCompleto,
    CASE WHEN u.IsActive = 1 THEN 'Activo' ELSE 'Inactivo' END as Estado,
    CASE WHEN u.EmailConfirmed = 1 THEN 'Confirmado' ELSE 'No confirmado' END as EmailStatus
FROM AspNetUsers u
WHERE u.Email = 'admin@rubricas.edu';

-- Roles asignados
SELECT 'Roles asignados:';
SELECT r.Name as Rol
FROM AspNetUsers u
JOIN AspNetUserRoles ur ON u.Id = ur.UserId
JOIN AspNetRoles r ON ur.RoleId = r.Id
WHERE u.Email = 'admin@rubricas.edu';

-- Conteo de permisos directos
SELECT 
    'Permisos directos asignados: ' || COUNT(*) as Resumen
FROM AspNetUsers u
JOIN AspNetUserClaims uc ON u.Id = uc.UserId
WHERE u.Email = 'admin@rubricas.edu'
AND uc.ClaimType = 'permission';

-- Algunos permisos (muestra)
SELECT 'Algunos permisos asignados:';
SELECT uc.ClaimValue as Permiso
FROM AspNetUsers u
JOIN AspNetUserClaims uc ON u.Id = uc.UserId
WHERE u.Email = 'admin@rubricas.edu'
AND uc.ClaimType = 'permission'
ORDER BY uc.ClaimValue
LIMIT 10;

-- Limpiar
DROP TABLE PermisosEsenciales;

SELECT 'Script completado - Usuario admin@rubricas.edu configurado con permisos de administrador';