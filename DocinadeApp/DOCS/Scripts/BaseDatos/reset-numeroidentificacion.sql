SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
GO

-- Crear backup antes de actualizar
SELECT Id, UserName, NumeroIdentificacion 
INTO AspNetUsers_Backup_NumeroIdentificacion
FROM AspNetUsers
WHERE NumeroIdentificacion IS NOT NULL;
GO

-- Ver cuántos usuarios tienen NumeroIdentificacion
SELECT 
    'Antes de actualizar' AS Momento,
    COUNT(*) AS TotalUsuarios,
    SUM(CASE WHEN NumeroIdentificacion IS NULL THEN 1 ELSE 0 END) AS SinIdentificacion,
    SUM(CASE WHEN NumeroIdentificacion IS NOT NULL THEN 1 ELSE 0 END) AS ConIdentificacion
FROM AspNetUsers;
GO

-- Actualizar a NULL
UPDATE AspNetUsers 
SET NumeroIdentificacion = NULL 
WHERE NumeroIdentificacion IS NOT NULL;
GO

-- Ver resultado después de actualizar
SELECT 
    'Después de actualizar' AS Momento,
    COUNT(*) AS TotalUsuarios,
    SUM(CASE WHEN NumeroIdentificacion IS NULL THEN 1 ELSE 0 END) AS SinIdentificacion,
    SUM(CASE WHEN NumeroIdentificacion IS NOT NULL THEN 1 ELSE 0 END) AS ConIdentificacion
FROM AspNetUsers;
GO

PRINT 'Actualización completada exitosamente.';
GO
