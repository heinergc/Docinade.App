-- Script para agregar columnas Observaciones faltantes

-- Verificar si la columna Observaciones existe en InstrumentoMaterias
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
               WHERE TABLE_NAME = 'InstrumentoMaterias' AND COLUMN_NAME = 'Observaciones')
BEGIN
    ALTER TABLE InstrumentoMaterias ADD Observaciones NVARCHAR(MAX) NULL;
    PRINT 'Columna Observaciones agregada a InstrumentoMaterias';
END
ELSE
BEGIN
    PRINT 'Columna Observaciones ya existe en InstrumentoMaterias';
END

-- Verificar si la columna Observaciones existe en MateriaPeriodos
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
               WHERE TABLE_NAME = 'MateriaPeriodos' AND COLUMN_NAME = 'Observaciones')
BEGIN
    ALTER TABLE MateriaPeriodos ADD Observaciones NVARCHAR(500) NULL;
    PRINT 'Columna Observaciones agregada a MateriaPeriodos';
END
ELSE
BEGIN
    PRINT 'Columna Observaciones ya existe en MateriaPeriodos';
END

-- Verificar las columnas agregadas
SELECT 'InstrumentoMaterias' as Tabla, COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'InstrumentoMaterias' AND COLUMN_NAME = 'Observaciones'

UNION ALL

SELECT 'MateriaPeriodos' as Tabla, COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'MateriaPeriodos' AND COLUMN_NAME = 'Observaciones';
