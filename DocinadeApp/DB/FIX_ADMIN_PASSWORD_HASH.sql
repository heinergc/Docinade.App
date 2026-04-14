-- =====================================================
-- SCRIPT: Recrear Usuario Admin con Password Válido
-- Base de datos: RubricaDB (Somee.com)
-- Fecha: 2025-11-23
-- =====================================================

BEGIN TRANSACTION;

PRINT '=== INICIO: Limpieza y recreación de usuario admin ===';

-- 1. ELIMINAR datos relacionados del usuario admin corrupto
PRINT 'Paso 1: Eliminando relaciones del usuario admin...';

DELETE FROM AspNetUserRoles 
WHERE UserId IN (SELECT Id FROM AspNetUsers WHERE Email = 'admin@rubricas.edu');
PRINT '[OK] Roles eliminados';

DELETE FROM AspNetUserClaims 
WHERE UserId IN (SELECT Id FROM AspNetUsers WHERE Email = 'admin@rubricas.edu');
PRINT '[OK] Claims eliminados';

DELETE FROM AspNetUserLogins 
WHERE UserId IN (SELECT Id FROM AspNetUsers WHERE Email = 'admin@rubricas.edu');
PRINT '[OK] Logins eliminados';

DELETE FROM AspNetUserTokens 
WHERE UserId IN (SELECT Id FROM AspNetUsers WHERE Email = 'admin@rubricas.edu');
PRINT '[OK] Tokens eliminados';

-- 2. ELIMINAR el usuario admin corrupto
PRINT 'Paso 2: Eliminando usuario admin corrupto...';
DELETE FROM AspNetUsers WHERE Email = 'admin@rubricas.edu';
PRINT '[OK] Usuario admin eliminado';

-- 3. Verificar que el rol SuperAdministrador existe
IF NOT EXISTS (SELECT 1 FROM AspNetRoles WHERE Name = 'SuperAdministrador')
BEGIN
    PRINT '[WARNING] Rol SuperAdministrador no existe, creándolo...';
    INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
    VALUES (NEWID(), 'SuperAdministrador', 'SUPERADMINISTRADOR', NEWID());
    PRINT '[OK] Rol SuperAdministrador creado';
END
ELSE
BEGIN
    PRINT '[INFO] Rol SuperAdministrador ya existe';
END

COMMIT TRANSACTION;

PRINT '=== FIN: Limpieza completada exitosamente ===';
PRINT '';
PRINT 'SIGUIENTE PASO:';
PRINT '1. Ve a Render.com';
PRINT '2. En tu servicio rubricasapp-web, haz clic en "Manual Deploy" > "Clear build cache & deploy"';
PRINT '3. O simplemente haz un nuevo push a GitHub (Render redesplegará automáticamente)';
PRINT '4. IdentitySeederService recreará el usuario admin automáticamente en el próximo inicio';
PRINT '';
PRINT 'CREDENCIALES DEL ADMIN (después del redeploy):';
PRINT '   Email: admin@rubricas.edu';
PRINT '   Password: Admin@2025!';
PRINT '';
PRINT '[SUCCESS] Script completado';
