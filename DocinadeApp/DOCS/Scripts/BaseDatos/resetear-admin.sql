-- Script para resetear usuario admin
-- Ejecutar este script si el usuario admin tiene problemas de acceso

-- 1. Desbloquear usuario
UPDATE AspNetUsers
SET 
    LockoutEnd = NULL,
    AccessFailedCount = 0,
    EmailConfirmed = 1,
    Activo = 1,
    LockoutEnabled = 0
WHERE Email = 'admin@rubricas.edu';

-- 2. Verificar resultado
SELECT 
    Email,
    UserName,
    EmailConfirmed,
    Activo,
    LockoutEnd,
    AccessFailedCount,
    'Usuario desbloqueado correctamente' AS Mensaje
FROM AspNetUsers
WHERE Email = 'admin@rubricas.edu';

PRINT 'Usuario admin desbloqueado. Use la contraseña: Admin@2025!';
PRINT 'Si sigue sin funcionar, puede que necesite recrear el usuario desde la aplicación.';
