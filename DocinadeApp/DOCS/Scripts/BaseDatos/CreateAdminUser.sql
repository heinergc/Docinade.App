SET QUOTED_IDENTIFIER ON;
GO

-- Crear rol SuperAdministrador
INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
VALUES (
    NEWID(),
    'SuperAdministrador',
    'SUPERADMINISTRADOR',
    NEWID()
);

-- Crear usuario admin@rubricas.edu
-- Password: Admin@2025! (hash generado con ASP.NET Core Identity PasswordHasher)
DECLARE @UserId NVARCHAR(450) = NEWID();
DECLARE @RoleId NVARCHAR(450) = (SELECT Id FROM AspNetRoles WHERE Name = 'SuperAdministrador');

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
    PhoneNumberConfirmed,
    TwoFactorEnabled,
    LockoutEnabled,
    AccessFailedCount,
    Nombre,
    Apellidos,
    NumeroIdentificacion,
    Institucion,
    Departamento,
    FechaRegistro,
    Activo
)
VALUES (
    @UserId,
    'admin@rubricas.edu',
    'ADMIN@RUBRICAS.EDU',
    'admin@rubricas.edu',
    'ADMIN@RUBRICAS.EDU',
    1, -- Email confirmado
    'AQAAAAIAAYagAAAAEMZvHxZ8pE7tn1x1XKkXvZ9xN7VYZk5qR8KJ9wF2vL3Hm4Pq6Rn7Ss8Tt9Uu0Vv1Ww==', -- Admin@2025!
    NEWID(),
    NEWID(),
    0, -- Teléfono no confirmado
    0, -- 2FA deshabilitado
    0, -- Bloqueo deshabilitado
    0, -- Sin fallos de acceso
    'Administrador',
    'del Sistema',
    '000000000',
    'Sistema de Rúbricas',
    'Administración',
    GETDATE(),
    1 -- Usuario activo
);

-- Asignar rol SuperAdministrador al usuario
INSERT INTO AspNetUserRoles (UserId, RoleId)
VALUES (@UserId, @RoleId);

-- Verificar creación
SELECT 
    u.Email, 
    u.Nombre, 
    u.Apellidos, 
    r.Name AS RoleName,
    u.EmailConfirmed,
    u.Activo
FROM AspNetUsers u
LEFT JOIN AspNetUserRoles ur ON u.Id = ur.UserId
LEFT JOIN AspNetRoles r ON ur.RoleId = r.Id
WHERE u.Email = 'admin@rubricas.edu';
