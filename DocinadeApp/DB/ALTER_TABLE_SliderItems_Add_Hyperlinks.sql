-- =============================================
-- Script: Agregar columnas EnlaceUrl y TextoBoton a SliderItems
-- Descripción: Añade soporte para hipervínculos personalizados en los sliders
-- Fecha: 2025-01-28
-- =============================================

USE RubricasDb;
GO

-- Verificar si la columna EnlaceUrl ya existe
IF NOT EXISTS (
    SELECT 1 
    FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_NAME = 'SliderItems' 
    AND COLUMN_NAME = 'EnlaceUrl'
)
BEGIN
    ALTER TABLE SliderItems 
    ADD EnlaceUrl NVARCHAR(500) NULL;
    
    PRINT '[SUCCESS] Columna EnlaceUrl agregada correctamente';
END
ELSE
BEGIN
    PRINT '[INFO] Columna EnlaceUrl ya existe, saltando...';
END
GO

-- Verificar si la columna TextoBoton ya existe
IF NOT EXISTS (
    SELECT 1 
    FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_NAME = 'SliderItems' 
    AND COLUMN_NAME = 'TextoBoton'
)
BEGIN
    ALTER TABLE SliderItems 
    ADD TextoBoton NVARCHAR(100) NULL;
    
    PRINT '[SUCCESS] Columna TextoBoton agregada correctamente';
END
ELSE
BEGIN
    PRINT '[INFO] Columna TextoBoton ya existe, saltando...';
END
GO

-- Verificar resultado
SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    CHARACTER_MAXIMUM_LENGTH,
    IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'SliderItems'
  AND COLUMN_NAME IN ('EnlaceUrl', 'TextoBoton');
GO

PRINT '[SUCCESS] Actualización de tabla SliderItems completada';
GO
