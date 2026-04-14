-- ============================================
-- Script para desbloquear usuario admin
-- ============================================

USE RubricaDB;
GO

-- Desbloquear usuario admin@rubricas.edu
UPDATE AspNetUsers
SET 
    LockoutEnd = NULL,
    AccessFailedCount = 0,
    EmailConfirmed = 1,
    IsActive = 1,
    Activo = 1
WHERE Email = 'admin@rubricas.edu';
GO

-- Verificar el estado del usuario
SELECT 
    Email,
    UserName,
    EmailConfirmed,
    LockoutEnd,
    AccessFailedCount,
    IsActive,
    Activo,
    Nombre,
    Apellidos
FROM AspNetUsers
WHERE Email = 'admin@rubricas.edu';
GO

PRINT '================================================';
PRINT 'Usuario desbloqueado exitosamente';
PRINT 'Email: admin@rubricas.edu';
PRINT 'Contraseña: Admin123!';
PRINT '================================================';
