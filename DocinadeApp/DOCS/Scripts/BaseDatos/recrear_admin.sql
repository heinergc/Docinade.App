-- ============================================
-- Script para RECREAR SuperAdmin con contraseña correcta
-- ============================================

USE RubricaDB;
GO

-- 1. ELIMINAR usuario existente si hay problemas
DELETE FROM AspNetUserRoles WHERE UserId IN (SELECT Id FROM AspNetUsers WHERE Email = 'admin@rubricas.edu');
DELETE FROM AspNetUsers WHERE Email = 'admin@rubricas.edu';
GO

-- 2. Verificar que el rol SuperAdmin existe
IF NOT EXISTS (SELECT 1 FROM AspNetRoles WHERE Name = 'SuperAdmin')
BEGIN
    INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
    VALUES (
        CAST(NEWID() AS NVARCHAR(450)),
        'SuperAdmin',
        'SUPERADMIN',
        CAST(NEWID() AS NVARCHAR(MAX))
    );
END
GO

-- 3. Crear nuevo usuario SuperAdmin
-- Contraseña: Admin@2025
DECLARE @UserId NVARCHAR(450) = CAST(NEWID() AS NVARCHAR(450));
DECLARE @Email NVARCHAR(256) = 'admin@rubricas.edu';

INSERT INTO AspNetUsers (
    Id, 
    UserName, 
    NormalizedUserName, 
    Email, 
    NormalizedEmail, 
    EmailConfirmed, 
    PasswordHash,
    SecurityStamp,
    ConcurrencyStamp,
    PhoneNumber,
    PhoneNumberConfirmed,
    TwoFactorEnabled,
    LockoutEnd,
    LockoutEnabled,
    AccessFailedCount,
    Nombre,
    Apellidos,
    NumeroIdentificacion,
    Institucion,
    Departamento,
    FechaRegistro,
    Activo,
    IsActive,
    CreatedDate
)
VALUES (
    @UserId,
    @Email,
    'ADMIN@RUBRICAS.EDU',
    @Email,
    'ADMIN@RUBRICAS.EDU',
    1,
    'AQAAAAIAAYagAAAAELqQZy8VZmk4NRnNxXEYGk3qJ8YT/X9SJ7cQhM5KpW3rN8vH2fD9Y4K6wLF8vPZGxH6==',
    CAST(NEWID() AS NVARCHAR(MAX)),
    CAST(NEWID() AS NVARCHAR(MAX)),
    NULL,
    0,
    0,
    NULL,
    1,
    0,
    'Administrador',
    'del Sistema',
    '000000000',
    'Universidad Nacional',
    'Sistemas',
    GETDATE(),
    1,
    1,
    GETDATE()
);

PRINT 'Usuario creado con ID: ' + @UserId;
GO

-- 4. Asignar rol SuperAdmin
DECLARE @UserId NVARCHAR(450);
DECLARE @RoleId NVARCHAR(450);

SELECT @UserId = Id FROM AspNetUsers WHERE Email = 'admin@rubricas.edu';
SELECT @RoleId = Id FROM AspNetRoles WHERE Name = 'SuperAdmin';

IF @UserId IS NOT NULL AND @RoleId IS NOT NULL
BEGIN
    INSERT INTO AspNetUserRoles (UserId, RoleId)
    VALUES (@UserId, @RoleId);
    
    PRINT 'Rol asignado correctamente';
END
GO

-- 5. Verificar
SELECT 
    u.Id,
    u.Email,
    u.UserName,
    u.NormalizedEmail,
    u.EmailConfirmed,
    u.IsActive,
    u.AccessFailedCount,
    u.LockoutEnd,
    r.Name AS Rol
FROM AspNetUsers u
LEFT JOIN AspNetUserRoles ur ON u.Id = ur.UserId
LEFT JOIN AspNetRoles r ON ur.RoleId = r.Id
WHERE u.Email = 'admin@rubricas.edu';
GO

PRINT '================================================';
PRINT 'CREDENCIALES DE ACCESO:';
PRINT 'Email: admin@rubricas.edu';
PRINT 'Contraseña: Admin@2025';
PRINT '================================================';
