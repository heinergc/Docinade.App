-- ===============================================
-- SCRIPT SIMPLIFICADO: INSTRUMENTOS BÁSICOS MEP
-- COSTA RICA - MINISTERIO DE EDUCACIÓN PÚBLICA
-- ===============================================
-- Version simplificada con solo los instrumentos esenciales
-- ===============================================

USE RubricasDb;
GO

PRINT '📚 INSERTANDO INSTRUMENTOS BÁSICOS DE EVALUACIÓN - MEP COSTA RICA';
PRINT '================================================================';

-- =====================================================================
-- 1. INSTRUMENTOS BÁSICOS TRANSVERSALES
-- =====================================================================

-- Verificar si ya existen instrumentos para evitar duplicados
IF NOT EXISTS (SELECT 1 FROM InstrumentosEvaluacion WHERE Nombre = 'Lista de Cotejo')
BEGIN
    INSERT INTO InstrumentosEvaluacion (Nombre, Descripcion, Activo, FechaCreacion)
    VALUES 
    ('Lista de Cotejo', 'Verificación de criterios específicos (Sí/No)', 1, GETDATE()),
    ('Rúbrica Analítica', 'Evaluación detallada por criterios con niveles de desempeño', 1, GETDATE()),
    ('Prueba Escrita', 'Examen escrito formal', 1, GETDATE()),
    ('Prueba Oral', 'Evaluación mediante interrogatorio oral', 1, GETDATE()),
    ('Proyecto de Investigación', 'Trabajo de investigación aplicada', 1, GETDATE()),
    ('Portafolio de Evidencias', 'Compilación de trabajos del estudiante', 1, GETDATE()),
    ('Presentación Oral', 'Exposición oral sobre tema específico', 1, GETDATE()),
    ('Autoevaluación', 'Reflexión del estudiante sobre su aprendizaje', 1, GETDATE());
    
    PRINT '✅ Instrumentos básicos insertados correctamente';
END
ELSE
BEGIN
    PRINT '⚠️ Los instrumentos básicos ya existen en la base de datos';
END

-- =====================================================================
-- 2. ASIGNACIÓN BÁSICA A TODAS LAS MATERIAS MEP
-- =====================================================================

PRINT 'Asignando instrumentos básicos a todas las materias MEP...';

-- Asignar instrumentos básicos a todas las materias MEP
INSERT INTO InstrumentoMaterias (InstrumentoEvaluacionId, MateriaId, PeriodoAcademicoId, FechaAsignacion, OrdenPresentacion, EsObligatorio)
SELECT 
    ie.InstrumentoId,
    m.MateriaId,
    1 as PeriodoAcademicoId, -- Período por defecto
    GETDATE(),
    CASE ie.Nombre
        WHEN 'Prueba Escrita' THEN 1
        WHEN 'Lista de Cotejo' THEN 2
        WHEN 'Rúbrica Analítica' THEN 3
        WHEN 'Presentación Oral' THEN 4
        WHEN 'Proyecto de Investigación' THEN 5
        WHEN 'Portafolio de Evidencias' THEN 6
        WHEN 'Autoevaluación' THEN 7
        WHEN 'Prueba Oral' THEN 8
        ELSE 9
    END,
    CASE ie.Nombre
        WHEN 'Prueba Escrita' THEN 1
        WHEN 'Rúbrica Analítica' THEN 1
        ELSE 0
    END
FROM InstrumentosEvaluacion ie
CROSS JOIN Materias m
WHERE ie.Nombre IN ('Lista de Cotejo', 'Rúbrica Analítica', 'Prueba Escrita', 'Prueba Oral', 
                    'Proyecto de Investigación', 'Portafolio de Evidencias', 'Presentación Oral', 'Autoevaluación')
  AND m.Codigo LIKE 'MEP-%'
  AND NOT EXISTS (
      SELECT 1 FROM InstrumentoMaterias im2 
      WHERE im2.InstrumentoEvaluacionId = ie.InstrumentoId 
        AND im2.MateriaId = m.MateriaId
  );

PRINT '✅ Asignación básica completada';

-- =====================================================================
-- 3. VERIFICACIÓN FINAL
-- =====================================================================

DECLARE @TotalInstrumentos INT = (SELECT COUNT(*) FROM InstrumentosEvaluacion);
DECLARE @TotalAsignaciones INT = (SELECT COUNT(*) FROM InstrumentoMaterias WHERE MateriaId IN (SELECT MateriaId FROM Materias WHERE Codigo LIKE 'MEP-%'));
DECLARE @TotalMateriasMEP INT = (SELECT COUNT(*) FROM Materias WHERE Codigo LIKE 'MEP-%');

PRINT '';
PRINT 'RESUMEN FINAL:';
PRINT '==============';
PRINT 'Total instrumentos: ' + CAST(@TotalInstrumentos AS VARCHAR(10));
PRINT 'Materias MEP: ' + CAST(@TotalMateriasMEP AS VARCHAR(10));
PRINT 'Asignaciones realizadas: ' + CAST(@TotalAsignaciones AS VARCHAR(10));

IF @TotalMateriasMEP > 0
    PRINT 'Promedio instrumentos por materia: ' + CAST(@TotalAsignaciones / @TotalMateriasMEP AS VARCHAR(10));

PRINT '';
PRINT '🎉 Script básico completado exitosamente!';

GO
