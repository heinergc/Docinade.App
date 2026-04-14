-- Script para cambiar la columna Estado de ProfesorGuia de nvarchar a bit
-- Fecha: 2025-11-07
-- Propósito: Corregir el tipo de datos de la columna Estado para que coincida con el modelo

USE RubricasDb;
GO

-- Verificar el tipo actual de la columna
SELECT 
    TABLE_NAME,
    COLUMN_NAME,
    DATA_TYPE,
    IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'ProfesorGuia' 
  AND COLUMN_NAME = 'Estado';
GO

-- Eliminar el constraint DEFAULT existente si existe
DECLARE @ConstraintName NVARCHAR(200);
SELECT @ConstraintName = dc.name
FROM sys.default_constraints dc
INNER JOIN sys.columns c ON dc.parent_object_id = c.object_id AND dc.parent_column_id = c.column_id
INNER JOIN sys.tables t ON c.object_id = t.object_id
WHERE t.name = 'ProfesorGuia' AND c.name = 'Estado';

IF @ConstraintName IS NOT NULL
BEGIN
    DECLARE @SQL NVARCHAR(MAX) = 'ALTER TABLE ProfesorGuia DROP CONSTRAINT ' + @ConstraintName;
    EXEC sp_executesql @SQL;
    PRINT 'Constraint DEFAULT eliminado: ' + @ConstraintName;
END
GO

-- Convertir valores existentes a formato compatible con bit
-- Cualquier valor que no sea NULL, vacío, '0', 'false', 'Inactivo' se convertirá a 1 (true)
-- Los demás valores se convertirán a 0 (false)
UPDATE ProfesorGuia
SET Estado = CASE 
    WHEN Estado IS NULL OR 
         Estado = '' OR 
         Estado = '0' OR 
         LOWER(Estado) = 'false' OR 
         LOWER(Estado) = 'inactivo' THEN '0'
    ELSE '1'
END
WHERE TRY_CAST(Estado AS bit) IS NULL;
GO

-- Alterar la columna de nvarchar a bit
ALTER TABLE ProfesorGuia
ALTER COLUMN Estado bit NOT NULL;
GO

-- Establecer valor predeterminado
ALTER TABLE ProfesorGuia
ADD CONSTRAINT DF_ProfesorGuia_Estado DEFAULT 1 FOR Estado;
GO

-- Verificar el cambio
SELECT 
    TABLE_NAME,
    COLUMN_NAME,
    DATA_TYPE,
    IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'ProfesorGuia' 
  AND COLUMN_NAME = 'Estado';
GO

-- Ver los datos convertidos
SELECT TOP 10
    Id,
    ProfesorId,
    GrupoId,
    Estado,
    CASE WHEN Estado = 1 THEN 'Activo' ELSE 'Inactivo' END AS EstadoTexto,
    FechaAsignacion
FROM ProfesorGuia
ORDER BY FechaAsignacion DESC;
GO

PRINT 'Columna Estado de ProfesorGuia cambiada exitosamente de nvarchar a bit';
GO
