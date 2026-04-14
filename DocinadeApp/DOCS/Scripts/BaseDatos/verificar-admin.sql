-- Verificar usuario admin
SELECT 
    u.Id,
    u.UserName,
    u.Email,
    u.Nombre,
    u.Apellidos,
    u.EmailConfirmed,
    u.Activo,
    u.LockoutEnd,
    u.AccessFailedCount,
    u.LockoutEnabled,
    u.PhoneNumberConfirmed,
    u.TwoFactorEnabled
FROM AspNetUsers u
WHERE u.Email = 'admin@rubricas.edu'
   OR u.UserName = 'admin@rubricas.edu';

-- Verificar roles del usuario
SELECT 
    r.Name AS RoleName,
    u.Email AS UserEmail
FROM AspNetUserRoles ur
INNER JOIN AspNetUsers u ON ur.UserId = u.Id
INNER JOIN AspNetRoles r ON ur.RoleId = r.Id
WHERE u.Email = 'admin@rubricas.edu';

-- Verificar hash de contraseña
SELECT 
    u.Email,
    SUBSTRING(u.PasswordHash, 1, 50) + '...' AS PasswordHashPreview
FROM AspNetUsers u
WHERE u.Email = 'admin@rubricas.edu';
