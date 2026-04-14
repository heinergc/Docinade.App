-- =====================================================================================
-- Script SQL SIMPLIFICADO para otorgar permisos básicos al usuario admin@rubricas.edu
-- Sistema de Rúbricas Académicas - Versión Simplificada
-- =====================================================================================

-- Variables
DECLARE @UserId NVARCHAR(450);
DECLARE @AdminRoleId NVARCHAR(450);

-- Buscar usuario
SELECT @UserId = Id FROM AspNetUsers WHERE Email = 'admin@rubricas.edu';

IF @UserId IS NULL
BEGIN
    PRINT 'Usuario admin@rubricas.edu no encontrado';
    
    -- Crear usuario si no existe
    SET @UserId = NEWID();
    INSERT INTO AspNetUsers (
        Id, UserName, NormalizedUserName, Email, NormalizedEmail, 
        EmailConfirmed, PasswordHash, SecurityStamp, ConcurrencyStamp,
        PhoneNumberConfirmed, TwoFactorEnabled, LockoutEnabled, AccessFailedCount,
        Nombre, Apellidos, IsActive, Activo, CreatedDate, FechaRegistro
    ) VALUES (
        @UserId, 'admin@rubricas.edu', 'ADMIN@RUBRICAS.EDU', 'admin@rubricas.edu', 'ADMIN@RUBRICAS.EDU',
        1, 'AQAAAAIAAYagAAAAEK8Q7Y9F8t0dYhWlJ+J9xKjJ7Q==', -- Contraseña: Admin123!
        NEWID(), NEWID(),
        0, 0, 0, 0,
        'Administrador', 'Sistema', 1, 1, GETDATE(), GETDATE()
    );
    PRINT 'Usuario admin@rubricas.edu creado: ' + @UserId;
END
ELSE
BEGIN
    PRINT 'Usuario encontrado: ' + @UserId;
END

-- Buscar/Crear rol Admin
SELECT @AdminRoleId = Id FROM AspNetRoles WHERE Name = 'Admin';

IF @AdminRoleId IS NULL
BEGIN
    SET @AdminRoleId = NEWID();
    INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
    VALUES (@AdminRoleId, 'Admin', 'ADMIN', NEWID());
    PRINT 'Rol Admin creado: ' + @AdminRoleId;
END

-- Asignar rol al usuario
IF NOT EXISTS (SELECT 1 FROM AspNetUserRoles WHERE UserId = @UserId AND RoleId = @AdminRoleId)
BEGIN
    INSERT INTO AspNetUserRoles (UserId, RoleId) VALUES (@UserId, @AdminRoleId);
    PRINT 'Rol Admin asignado al usuario';
END

-- Asignar permisos críticos
DECLARE @PermisosAdmin TABLE (Permiso NVARCHAR(100));
INSERT INTO @PermisosAdmin VALUES 
('configuracion.ver'),
('configuracion.gestionar'),
('usuarios.ver'),
('usuarios.crear'),
('usuarios.editar'),
('usuarios.gestionar_roles'),
('rubricas.ver_todas'),
('rubricas.editar_todas'),
('evaluaciones.ver_todas'),
('reportes.ver_todos');

-- Asignar permisos al usuario
INSERT INTO AspNetUserClaims (UserId, ClaimType, ClaimValue)
SELECT @UserId, 'permission', Permiso
FROM @PermisosAdmin p
WHERE NOT EXISTS (
    SELECT 1 FROM AspNetUserClaims 
    WHERE UserId = @UserId AND ClaimType = 'permission' AND ClaimValue = p.Permiso
);

-- Asignar permisos al rol
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue)
SELECT @AdminRoleId, 'permission', Permiso
FROM @PermisosAdmin p
WHERE NOT EXISTS (
    SELECT 1 FROM AspNetRoleClaims 
    WHERE RoleId = @AdminRoleId AND ClaimType = 'permission' AND ClaimValue = p.Permiso
);

PRINT 'Permisos básicos asignados correctamente';
PRINT 'Usuario admin@rubricas.edu configurado con permisos de administrador';