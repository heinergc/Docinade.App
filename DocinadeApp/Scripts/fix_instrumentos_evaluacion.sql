-- ===============================================
-- SCRIPT DE CORRECCIÓN PARA InstrumentosEvaluacion
-- ===============================================
-- Ejecutar este script en la base de datos SQLite RubricasApp.db
-- para corregir el problema de NOT NULL constraint failed

-- 1. Ver estructura actual de la tabla
PRAGMA table_info(InstrumentosEvaluacion);

-- 2. Verificar datos actuales
SELECT 
    InstrumentoId,
    Nombre, 
    CASE WHEN Activo IS NULL THEN 'NULL' ELSE CAST(Activo AS TEXT) END as Activo_Estado,
    CASE WHEN EstaActivo IS NULL THEN 'NULL' ELSE CAST(EstaActivo AS TEXT) END as EstaActivo_Estado
FROM InstrumentosEvaluacion;

-- 3. Agregar columna Activo si no existe (ignorar error si ya existe)
-- SQLite no soporta IF NOT EXISTS para ALTER TABLE, usar con cuidado
BEGIN TRANSACTION;

-- Intentar agregar la columna (fallará si ya existe, pero eso está bien)
ALTER TABLE InstrumentosEvaluacion ADD COLUMN Activo INTEGER DEFAULT 1;

COMMIT;

-- 4. Actualizar valores NULL en Activo
UPDATE InstrumentosEvaluacion 
SET Activo = 1 
WHERE Activo IS NULL;

-- 5. Si existe EstaActivo, sincronizar datos
UPDATE InstrumentosEvaluacion 
SET Activo = COALESCE(EstaActivo, 1)
WHERE EstaActivo IS NOT NULL;

-- 6. Verificar que no quedan valores NULL
SELECT 
    COUNT(*) as "Total_Registros",
    COUNT(CASE WHEN Activo IS NULL THEN 1 END) as "Activo_NULL",
    COUNT(CASE WHEN Activo = 1 THEN 1 END) as "Activo_True",
    COUNT(CASE WHEN Activo = 0 THEN 1 END) as "Activo_False"
FROM InstrumentosEvaluacion;

-- 7. Mostrar resultado final
SELECT 
    InstrumentoId,
    Nombre, 
    Activo,
    FechaCreacion
FROM InstrumentosEvaluacion
ORDER BY InstrumentoId;

-- ===============================================
-- NOTAS IMPORTANTES:
-- ===============================================
-- 1. Si el ALTER TABLE falla porque la columna ya existe, eso es normal
-- 2. Después de ejecutar este script, reinicia la aplicación
-- 3. El problema debería estar resuelto
-- ===============================================