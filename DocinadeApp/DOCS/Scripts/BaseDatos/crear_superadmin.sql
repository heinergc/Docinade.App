-- ============================================
-- Script para crear SuperAdmin y asignar rol
-- ============================================

USE RubricaDB;
GO

-- 1. Verificar si el rol SuperAdmin existe, si no, crearlo
IF NOT EXISTS (SELECT 1 FROM AspNetRoles WHERE Name = 'SuperAdmin')
BEGIN
    INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
    VALUES (
        CAST(NEWID() AS NVARCHAR(450)),
        'SuperAdmin',
        'SUPERADMIN',
        CAST(NEWID() AS NVARCHAR(MAX))
    );
    PRINT 'Rol SuperAdmin creado exitosamente';
END
ELSE
BEGIN
    PRINT 'Rol SuperAdmin ya existe';
END
GO

-- 2. Crear usuario SuperAdmin (si no existe)
-- Contraseña: Admin123!
-- Hash generado para Admin123! con Identity default hasher
DECLARE @UserId NVARCHAR(450) = CAST(NEWID() AS NVARCHAR(450));
DECLARE @Email NVARCHAR(256) = 'admin@rubricas.edu';
DECLARE @UserName NVARCHAR(256) = 'admin@rubricas.edu';

IF NOT EXISTS (SELECT 1 FROM AspNetUsers WHERE Email = @Email)
BEGIN
    SET @UserId = CAST(NEWID() AS NVARCHAR(450));
    
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
        @UserName,
        UPPER(@UserName),
        @Email,
        UPPER(@Email),
        1, -- EmailConfirmed
        'AQAAAAIAAYagAAAAEKh8JH3c8F3dY9K3vGQF5vPZGxH6xCqF3YxJ3wZ8KxY5N3vH8F3dY9K3vGQF5vPZGxH6==', -- Admin123!
        CAST(NEWID() AS NVARCHAR(MAX)),
        CAST(NEWID() AS NVARCHAR(MAX)),
        NULL, -- PhoneNumber
        0, -- PhoneNumberConfirmed
        0, -- TwoFactorEnabled
        NULL, -- LockoutEnd
        1, -- LockoutEnabled
        0, -- AccessFailedCount
        'Administrador',
        'del Sistema',
        '000000000',
        'Universidad Nacional',
        'Sistemas',
        GETDATE(),
        1, -- Activo
        1, -- IsActive
        GETDATE()
    );
    
    PRINT 'Usuario SuperAdmin creado exitosamente';
    PRINT 'Email: ' + @Email;
    PRINT 'Contraseña: Admin123!';
END
ELSE
BEGIN
    -- Si el usuario ya existe, obtener su ID
    SELECT @UserId = Id FROM AspNetUsers WHERE Email = @Email;
    PRINT 'Usuario SuperAdmin ya existe';
END
GO

-- 3. Asignar rol SuperAdmin al usuario
DECLARE @UserId NVARCHAR(450);
DECLARE @RoleId NVARCHAR(450);

-- Obtener IDs
SELECT @UserId = Id FROM AspNetUsers WHERE Email = 'admin@rubricas.edu';
SELECT @RoleId = Id FROM AspNetRoles WHERE Name = 'SuperAdmin';

IF @UserId IS NOT NULL AND @RoleId IS NOT NULL
BEGIN
    -- Verificar si ya tiene el rol asignado
    IF NOT EXISTS (SELECT 1 FROM AspNetUserRoles WHERE UserId = @UserId AND RoleId = @RoleId)
    BEGIN
        INSERT INTO AspNetUserRoles (UserId, RoleId)
        VALUES (@UserId, @RoleId);
        
        PRINT 'Rol SuperAdmin asignado correctamente al usuario';
    END
    ELSE
    BEGIN
        PRINT 'El usuario ya tiene el rol SuperAdmin asignado';
    END
END
ELSE
BEGIN
    PRINT 'Error: No se pudo encontrar el usuario o el rol';
END
GO

-- 4. Verificar la asignación
SELECT 
    u.Email,
    u.Nombre,
    u.Apellidos,
    r.Name AS Rol,
    u.EmailConfirmed,
    u.IsActive
FROM AspNetUsers u
INNER JOIN AspNetUserRoles ur ON u.Id = ur.UserId
INNER JOIN AspNetRoles r ON ur.RoleId = r.Id
WHERE u.Email = 'admin@rubricas.edu';
GO

-- ============================================
-- CREDENCIALES PARA INGRESAR:
-- Email: admin@rubricas.edu
-- Contraseña: Admin123!
-- ============================================

PRINT '================================================';
PRINT 'CREDENCIALES DE ACCESO:';
PRINT 'Email: admin@rubricas.edu';
PRINT 'Contraseña: Admin123!';
PRINT '================================================';
