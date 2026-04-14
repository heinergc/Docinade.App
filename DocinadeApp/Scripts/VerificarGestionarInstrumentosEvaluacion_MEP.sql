-- ===============================================
-- SCRIPT DE VERIFICACIÓN Y GESTIÓN
-- INSTRUMENTOS DE EVALUACIÓN MEP
-- ===============================================
-- Este script permite verificar, consultar y gestionar 
-- los instrumentos de evaluación y sus asignaciones
-- ===============================================

USE RubricasDb;
GO

PRINT '🔍 VERIFICACIÓN Y GESTIÓN DE INSTRUMENTOS DE EVALUACIÓN MEP';
PRINT '==========================================================';

-- =====================================================================
-- 1. CONSULTAS DE VERIFICACIÓN
-- =====================================================================

PRINT '1. VERIFICANDO ESTADO ACTUAL...';
PRINT '';

-- 1.1 Contar instrumentos existentes
SELECT 'Instrumentos de Evaluación' as Categoria, COUNT(*) as Total
FROM InstrumentosEvaluacion
UNION ALL
SELECT 'Asignaciones a Materias' as Categoria, COUNT(*) as Total
FROM InstrumentoMaterias
UNION ALL  
SELECT 'Materias MEP' as Categoria, COUNT(*) as Total
FROM Materias WHERE Codigo LIKE 'MEP-%';

PRINT '';

-- 1.2 Instrumentos por estado
SELECT 
    CASE WHEN Activo = 1 THEN 'Activos' ELSE 'Inactivos' END as Estado,
    COUNT(*) as Cantidad
FROM InstrumentosEvaluacion
GROUP BY Activo;

PRINT '';

-- =====================================================================
-- 2. CONSULTAR INSTRUMENTOS POR MATERIA
-- =====================================================================

PRINT '2. INSTRUMENTOS ASIGNADOS POR ÁREA ACADÉMICA:';
PRINT '============================================';

SELECT 
    CASE 
        WHEN m.Codigo LIKE 'MEP-ESP-%' THEN '📖 Español'
        WHEN m.Codigo LIKE 'MEP-MAT-%' THEN '🧮 Matemáticas'
        WHEN m.Codigo LIKE 'MEP-CIE-%' OR m.Codigo LIKE 'MEP-FIS-%' OR m.Codigo LIKE 'MEP-QUI-%' OR m.Codigo LIKE 'MEP-BIO-%' THEN '🔬 Ciencias'
        WHEN m.Codigo LIKE 'MEP-SS-%' THEN '🌍 Estudios Sociales'
        WHEN m.Codigo LIKE 'MEP-ING-%' THEN '🌐 Inglés'
        WHEN m.Codigo LIKE 'MEP-ART-%' OR m.Codigo LIKE 'MEP-MUS-%' THEN '🎨 Educación Artística'
        WHEN m.Codigo LIKE 'MEP-EF-%' THEN '⚽ Educación Física'
        WHEN m.Codigo LIKE 'MEP-TEC-%' THEN '🔧 Educación Técnica'
        WHEN m.Codigo LIKE 'MEP-INF-%' THEN '💻 Informática'
        ELSE '📚 Otras'
    END AS Area_Academica,
    COUNT(DISTINCT m.MateriaId) AS Total_Materias,
    COUNT(im.InstrumentoEvaluacionId) AS Total_Instrumentos_Asignados,
    CAST(ROUND(AVG(CAST(COUNT(im.InstrumentoEvaluacionId) AS FLOAT) / NULLIF(COUNT(DISTINCT m.MateriaId), 0)), 1) AS DECIMAL(3,1)) AS Promedio_Por_Materia
FROM Materias m
LEFT JOIN InstrumentoMaterias im ON m.MateriaId = im.MateriaId
WHERE m.Codigo LIKE 'MEP-%'
GROUP BY 
    CASE 
        WHEN m.Codigo LIKE 'MEP-ESP-%' THEN '📖 Español'
        WHEN m.Codigo LIKE 'MEP-MAT-%' THEN '🧮 Matemáticas'
        WHEN m.Codigo LIKE 'MEP-CIE-%' OR m.Codigo LIKE 'MEP-FIS-%' OR m.Codigo LIKE 'MEP-QUI-%' OR m.Codigo LIKE 'MEP-BIO-%' THEN '🔬 Ciencias'
        WHEN m.Codigo LIKE 'MEP-SS-%' THEN '🌍 Estudios Sociales'
        WHEN m.Codigo LIKE 'MEP-ING-%' THEN '🌐 Inglés'
        WHEN m.Codigo LIKE 'MEP-ART-%' OR m.Codigo LIKE 'MEP-MUS-%' THEN '🎨 Educación Artística'
        WHEN m.Codigo LIKE 'MEP-EF-%' THEN '⚽ Educación Física'
        WHEN m.Codigo LIKE 'MEP-TEC-%' THEN '🔧 Educación Técnica'
        WHEN m.Codigo LIKE 'MEP-INF-%' THEN '💻 Informática'
        ELSE '📚 Otras'
    END
ORDER BY Area_Academica;

PRINT '';

-- =====================================================================
-- 3. TOP 10 INSTRUMENTOS MÁS UTILIZADOS
-- =====================================================================

PRINT '3. TOP 10 INSTRUMENTOS MÁS UTILIZADOS:';
PRINT '======================================';

SELECT TOP 10
    ie.Nombre,
    ie.Descripcion,
    COUNT(im.MateriaId) as Materias_Asignadas,
    CAST(ROUND((COUNT(im.MateriaId) * 100.0 / (SELECT COUNT(*) FROM Materias WHERE Codigo LIKE 'MEP-%')), 1) AS DECIMAL(5,1)) as Porcentaje_Cobertura
FROM InstrumentosEvaluacion ie
LEFT JOIN InstrumentoMaterias im ON ie.InstrumentoId = im.InstrumentoEvaluacionId
INNER JOIN Materias m ON im.MateriaId = m.MateriaId AND m.Codigo LIKE 'MEP-%'
GROUP BY ie.InstrumentoId, ie.Nombre, ie.Descripcion
ORDER BY COUNT(im.MateriaId) DESC;

PRINT '';

-- =====================================================================
-- 4. MATERIAS SIN INSTRUMENTOS ASIGNADOS
-- =====================================================================

PRINT '4. MATERIAS SIN INSTRUMENTOS ASIGNADOS:';
PRINT '======================================';

SELECT 
    m.Codigo,
    m.Nombre,
    m.Tipo,
    m.CicloSugerido
FROM Materias m
LEFT JOIN InstrumentoMaterias im ON m.MateriaId = im.MateriaId
WHERE m.Codigo LIKE 'MEP-%'
  AND im.MateriaId IS NULL
ORDER BY m.Codigo;

PRINT '';

-- =====================================================================
-- 5. INSTRUMENTOS NO ASIGNADOS A NINGUNA MATERIA
-- =====================================================================

PRINT '5. INSTRUMENTOS NO ASIGNADOS:';
PRINT '=============================';

SELECT 
    ie.Nombre,
    ie.Descripcion,
    ie.FechaCreacion
FROM InstrumentosEvaluacion ie
LEFT JOIN InstrumentoMaterias im ON ie.InstrumentoId = im.InstrumentoEvaluacionId
WHERE im.InstrumentoEvaluacionId IS NULL
  AND ie.Activo = 1
ORDER BY ie.Nombre;

PRINT '';

-- =====================================================================
-- 6. DETALLE COMPLETO DE UNA MATERIA ESPECÍFICA
-- =====================================================================

PRINT '6. EJEMPLO: INSTRUMENTOS DE ESPAÑOL 7°:';
PRINT '======================================';

SELECT 
    m.Codigo,
    m.Nombre AS Materia,
    ie.Nombre AS Instrumento,
    ie.Descripcion,
    im.OrdenPresentacion,
    CASE WHEN im.EsObligatorio = 1 THEN 'Sí' ELSE 'No' END AS Obligatorio,
    im.FechaAsignacion
FROM Materias m
INNER JOIN InstrumentoMaterias im ON m.MateriaId = im.MateriaId
INNER JOIN InstrumentosEvaluacion ie ON im.InstrumentoEvaluacionId = ie.InstrumentoId
WHERE m.Codigo = 'MEP-ESP-7'
ORDER BY im.OrdenPresentacion, ie.Nombre;

PRINT '';

-- =====================================================================
-- 7. FUNCIONES DE MANTENIMIENTO
-- =====================================================================

PRINT '7. FUNCIONES DE MANTENIMIENTO DISPONIBLES:';
PRINT '==========================================';
PRINT '';

PRINT '-- Para activar/desactivar un instrumento:';
PRINT '-- UPDATE InstrumentosEvaluacion SET Activo = 0 WHERE Nombre = ''Nombre del Instrumento'';';
PRINT '';

PRINT '-- Para cambiar el orden de presentación:';
PRINT '-- UPDATE InstrumentoMaterias SET OrdenPresentacion = 1 WHERE InstrumentoEvaluacionId = ID AND MateriaId = ID;';
PRINT '';

PRINT '-- Para marcar un instrumento como obligatorio:';
PRINT '-- UPDATE InstrumentoMaterias SET EsObligatorio = 1 WHERE InstrumentoEvaluacionId = ID AND MateriaId = ID;';
PRINT '';

PRINT '-- Para remover la asignación de un instrumento a una materia:';
PRINT '-- DELETE FROM InstrumentoMaterias WHERE InstrumentoEvaluacionId = ID AND MateriaId = ID;';
PRINT '';

-- =====================================================================
-- 8. ESTADÍSTICAS GENERALES
-- =====================================================================

PRINT '8. ESTADÍSTICAS GENERALES:';
PRINT '==========================';

DECLARE @TotalInstrumentosActivos INT = (SELECT COUNT(*) FROM InstrumentosEvaluacion WHERE Activo = 1);
DECLARE @TotalAsignacionesActivas INT = (SELECT COUNT(*) FROM InstrumentoMaterias im INNER JOIN InstrumentosEvaluacion ie ON im.InstrumentoEvaluacionId = ie.InstrumentoId WHERE ie.Activo = 1);
DECLARE @TotalMateriasMEP INT = (SELECT COUNT(*) FROM Materias WHERE Codigo LIKE 'MEP-%');
DECLARE @MateriasConInstrumentos INT = (SELECT COUNT(DISTINCT im.MateriaId) FROM InstrumentoMaterias im INNER JOIN Materias m ON im.MateriaId = m.MateriaId WHERE m.Codigo LIKE 'MEP-%');

SELECT 
    'Instrumentos Activos' as Concepto, 
    @TotalInstrumentosActivos as Cantidad,
    '📋' as Icono
UNION ALL
SELECT 
    'Asignaciones Activas' as Concepto, 
    @TotalAsignacionesActivas as Cantidad,
    '🔗' as Icono
UNION ALL
SELECT 
    'Materias MEP Total' as Concepto, 
    @TotalMateriasMEP as Cantidad,
    '📚' as Icono
UNION ALL
SELECT 
    'Materias con Instrumentos' as Concepto, 
    @MateriasConInstrumentos as Cantidad,
    '✅' as Icono
UNION ALL
SELECT 
    'Promedio Instrumentos/Materia' as Concepto, 
    CASE WHEN @TotalMateriasMEP > 0 THEN @TotalAsignacionesActivas / @TotalMateriasMEP ELSE 0 END as Cantidad,
    '📊' as Icono;

PRINT '';
PRINT '🎯 COBERTURA: ' + CAST(ROUND((@MateriasConInstrumentos * 100.0 / NULLIF(@TotalMateriasMEP, 0)), 1) AS VARCHAR(10)) + '% de materias MEP tienen instrumentos asignados';

PRINT '';
PRINT '✅ Verificación completada exitosamente!';
PRINT '';

GO
