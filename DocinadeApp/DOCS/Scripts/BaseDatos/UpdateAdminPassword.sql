SET QUOTED_IDENTIFIER ON;
GO

UPDATE AspNetUsers 
SET PasswordHash = 'AQAAAAEAACcQAAAAEMJEvoKNDggRi8ygxSQSnw8dpks/ixkbIRyUBD8mqZxBc5KeCLlZlFQvlAAji6mQkQ=='
WHERE Email = 'admin@rubricas.edu';

SELECT 'Usuario actualizado exitosamente' as Resultado;

-- Verificar actualización
SELECT 
    Email, 
    UserName, 
    LEFT(PasswordHash, 50) + '...' as PasswordHashPreview,
    EmailConfirmed,
    Activo,
    LockoutEnabled
FROM AspNetUsers 
WHERE Email = 'admin@rubricas.edu';
