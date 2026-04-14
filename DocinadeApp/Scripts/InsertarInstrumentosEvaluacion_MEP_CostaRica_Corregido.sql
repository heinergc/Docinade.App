-- ===============================================
-- SCRIPT CORREGIDO: INSTRUMENTOS DE EVALUACIÓN DEL MEP
-- COSTA RICA - MINISTERIO DE EDUCACIÓN PÚBLICA
-- ===============================================
-- Versión corregida sin errores de agregación
-- ===============================================

USE RubricasDb;
GO

PRINT '📚 INSERTANDO INSTRUMENTOS DE EVALUACIÓN DEL MEP - COSTA RICA';
PRINT '================================================================';
PRINT '';

-- =====================================================================
-- 1. INSTRUMENTOS GENERALES (TRANSVERSALES A TODAS LAS MATERIAS)
-- =====================================================================

PRINT '1. Insertando instrumentos generales (transversales)...';

-- Verificar si ya existen para evitar duplicados
IF NOT EXISTS (SELECT 1 FROM InstrumentosEvaluacion WHERE Nombre = 'Lista de Cotejo')
BEGIN
    INSERT INTO InstrumentosEvaluacion (Nombre, Descripcion, Activo, FechaCreacion)
    VALUES 
    -- Instrumentos de evaluación formativa
    ('Lista de Cotejo', 'Instrumento para verificar el cumplimiento de criterios específicos de manera dicotómica (Sí/No)', 1, GETDATE()),
    ('Escala de Calificación', 'Instrumento que permite graduar el nivel de logro de los criterios evaluados', 1, GETDATE()),
    ('Rúbrica Analítica', 'Evaluación detallada por criterios específicos con diferentes niveles de desempeño', 1, GETDATE()),
    ('Rúbrica Holística', 'Evaluación global del desempeño del estudiante de manera integral', 1, GETDATE()),
    ('Autoevaluación', 'Proceso de reflexión del estudiante sobre su propio aprendizaje y desempeño', 1, GETDATE()),
    ('Coevaluación', 'Evaluación realizada entre pares o compañeros de clase', 1, GETDATE()),
    ('Observación Sistemática', 'Registro estructurado del comportamiento y desempeño académico del estudiante', 1, GETDATE()),
    ('Prueba Diagnóstica', 'Evaluación inicial para identificar el nivel de conocimientos previos', 1, GETDATE()),
    ('Portafolio de Evidencias', 'Compilación organizada de trabajos que evidencian el progreso del aprendizaje', 1, GETDATE()),

    -- Instrumentos de evaluación sumativa
    ('Prueba Escrita', 'Evaluación formal mediante examen escrito con preguntas estructuradas', 1, GETDATE()),
    ('Prueba Oral', 'Evaluación mediante interrogatorio oral o presentación verbal', 1, GETDATE()),
    ('Proyecto de Investigación', 'Trabajo de investigación que demuestra aplicación de conocimientos', 1, GETDATE()),
    ('Ensayo', 'Texto argumentativo o expositivo que demuestra capacidad de análisis y síntesis', 1, GETDATE()),
    ('Presentación Oral', 'Exposición oral individual o grupal sobre tema específico', 1, GETDATE()),
    ('Trabajo Colaborativo', 'Proyecto realizado en equipo que evalúa competencias sociales y académicas', 1, GETDATE());
    
    PRINT '✅ Instrumentos transversales insertados';
END
ELSE
BEGIN
    PRINT '⚠️ Instrumentos transversales ya existen';
END

-- =====================================================================
-- 2. INSTRUMENTOS ESPECÍFICOS POR ÁREA ACADÉMICA
-- =====================================================================

PRINT '2. Insertando instrumentos específicos por área académica...';

-- ESPAÑOL Y LITERATURA
IF NOT EXISTS (SELECT 1 FROM InstrumentosEvaluacion WHERE Nombre = 'Análisis Literario')
BEGIN
    INSERT INTO InstrumentosEvaluacion (Nombre, Descripcion, Activo, FechaCreacion)
    VALUES 
    ('Análisis Literario', 'Evaluación de comprensión y análisis de obras literarias', 1, GETDATE()),
    ('Composición Escrita', 'Evaluación de redacción creativa y académica', 1, GETDATE()),
    ('Debate Estructurado', 'Evaluación de argumentación y expresión oral mediante debate', 1, GETDATE()),
    ('Comprensión Lectora', 'Evaluación de habilidades de comprensión de textos', 1, GETDATE()),
    ('Declamación', 'Evaluación de expresión oral mediante recitación poética', 1, GETDATE()),
    ('Dramatización', 'Representación teatral para evaluar expresión corporal y oral', 1, GETDATE());
    
    PRINT '✅ Instrumentos de Español insertados';
END

-- MATEMÁTICAS
IF NOT EXISTS (SELECT 1 FROM InstrumentosEvaluacion WHERE Nombre = 'Resolución de Problemas')
BEGIN
    INSERT INTO InstrumentosEvaluacion (Nombre, Descripcion, Activo, FechaCreacion)
    VALUES 
    ('Resolución de Problemas', 'Evaluación del proceso de resolución de problemas matemáticos', 1, GETDATE()),
    ('Laboratorio Matemático', 'Uso de software especializado para resolver problemas matemáticos', 1, GETDATE()),
    ('Proyecto Matemático', 'Aplicación práctica de conceptos matemáticos a situaciones reales', 1, GETDATE()),
    ('Demostración Matemática', 'Evaluación de la capacidad de demostrar teoremas y propiedades', 1, GETDATE()),
    ('Calculadora Gráfica', 'Evaluación usando herramientas tecnológicas para matemáticas', 1, GETDATE());
    
    PRINT '✅ Instrumentos de Matemáticas insertados';
END

-- CIENCIAS NATURALES
IF NOT EXISTS (SELECT 1 FROM InstrumentosEvaluacion WHERE Nombre = 'Informe de Laboratorio')
BEGIN
    INSERT INTO InstrumentosEvaluacion (Nombre, Descripcion, Activo, FechaCreacion)
    VALUES 
    ('Informe de Laboratorio', 'Documentación detallada de experimentos científicos realizados', 1, GETDATE()),
    ('Proyecto Científico', 'Investigación científica siguiendo el método científico', 1, GETDATE()),
    ('Bitácora de Laboratorio', 'Registro continuo del proceso experimental y observaciones', 1, GETDATE()),
    ('Mapa Conceptual Científico', 'Organización visual de conceptos científicos y sus relaciones', 1, GETDATE()),
    ('Feria Científica', 'Presentación pública de proyectos de investigación científica', 1, GETDATE()),
    ('Práctica de Laboratorio', 'Evaluación práctica del manejo de equipos y procedimientos', 1, GETDATE());
    
    PRINT '✅ Instrumentos de Ciencias insertados';
END

-- ESTUDIOS SOCIALES
IF NOT EXISTS (SELECT 1 FROM InstrumentosEvaluacion WHERE Nombre = 'Ensayo Histórico')
BEGIN
    INSERT INTO InstrumentosEvaluacion (Nombre, Descripcion, Activo, FechaCreacion)
    VALUES 
    ('Ensayo Histórico', 'Análisis escrito de procesos y eventos históricos', 1, GETDATE()),
    ('Línea de Tiempo', 'Representación cronológica de eventos históricos', 1, GETDATE()),
    ('Análisis de Fuentes', 'Evaluación de documentos históricos y fuentes primarias', 1, GETDATE()),
    ('Mapa Histórico', 'Representación geográfica de eventos y procesos históricos', 1, GETDATE()),
    ('Debate Social', 'Discusión estructurada sobre temas de actualidad social', 1, GETDATE()),
    ('Proyecto Comunitario', 'Investigación sobre problemáticas sociales de la comunidad', 1, GETDATE());
    
    PRINT '✅ Instrumentos de Estudios Sociales insertados';
END

-- INGLÉS (IDIOMA EXTRANJERO)
IF NOT EXISTS (SELECT 1 FROM InstrumentosEvaluacion WHERE Nombre = 'Listening Comprehension')
BEGIN
    INSERT INTO InstrumentosEvaluacion (Nombre, Descripcion, Activo, FechaCreacion)
    VALUES 
    ('Listening Comprehension', 'Evaluación de comprensión auditiva en inglés', 1, GETDATE()),
    ('Speaking Assessment', 'Evaluación de expresión oral en inglés', 1, GETDATE()),
    ('Writing Skills', 'Evaluación de habilidades de escritura en inglés', 1, GETDATE()),
    ('Reading Comprehension', 'Evaluación de comprensión lectora en inglés', 1, GETDATE()),
    ('Role Play', 'Dramatización de situaciones comunicativas en inglés', 1, GETDATE()),
    ('Cultural Project', 'Proyecto sobre culturas de países anglófonos', 1, GETDATE());
    
    PRINT '✅ Instrumentos de Inglés insertados';
END

-- EDUCACIÓN ARTÍSTICA
IF NOT EXISTS (SELECT 1 FROM InstrumentosEvaluacion WHERE Nombre = 'Portafolio Artístico')
BEGIN
    INSERT INTO InstrumentosEvaluacion (Nombre, Descripcion, Activo, FechaCreacion)
    VALUES 
    ('Portafolio Artístico', 'Compilación de obras artísticas creadas durante el período', 1, GETDATE()),
    ('Presentación Musical', 'Interpretación musical individual o grupal', 1, GETDATE()),
    ('Crítica Artística', 'Análisis y evaluación de obras artísticas', 1, GETDATE()),
    ('Exposición Artística', 'Muestra pública de trabajos artísticos realizados', 1, GETDATE()),
    ('Performance Artístico', 'Presentación artística en vivo que integra diversas disciplinas', 1, GETDATE());
    
    PRINT '✅ Instrumentos de Educación Artística insertados';
END

-- EDUCACIÓN FÍSICA
IF NOT EXISTS (SELECT 1 FROM InstrumentosEvaluacion WHERE Nombre = 'Prueba de Aptitud Física')
BEGIN
    INSERT INTO InstrumentosEvaluacion (Nombre, Descripcion, Activo, FechaCreacion)
    VALUES 
    ('Prueba de Aptitud Física', 'Evaluación de capacidades físicas y motoras', 1, GETDATE()),
    ('Proyecto Vida Saludable', 'Investigación sobre hábitos de ejercicio y nutrición', 1, GETDATE()),
    ('Evaluación Deportiva', 'Evaluación de destrezas específicas en deportes', 1, GETDATE()),
    ('Plan de Ejercicios', 'Diseño y ejecución de rutina de ejercicios personalizada', 1, GETDATE());
    
    PRINT '✅ Instrumentos de Educación Física insertados';
END

-- EDUCACIÓN TÉCNICA
IF NOT EXISTS (SELECT 1 FROM InstrumentosEvaluacion WHERE Nombre = 'Proyecto Técnico')
BEGIN
    INSERT INTO InstrumentosEvaluacion (Nombre, Descripcion, Activo, FechaCreacion)
    VALUES 
    ('Proyecto Técnico', 'Aplicación práctica de conocimientos técnicos especializados', 1, GETDATE()),
    ('Informe de Práctica', 'Documentación de procesos técnicos realizados', 1, GETDATE()),
    ('Evaluación de Destrezas', 'Evaluación práctica de habilidades técnicas específicas', 1, GETDATE()),
    ('Simulación Laboral', 'Evaluación en situaciones simuladas del ambiente laboral', 1, GETDATE());
    
    PRINT '✅ Instrumentos de Educación Técnica insertados';
END

-- INFORMÁTICA EDUCATIVA
IF NOT EXISTS (SELECT 1 FROM InstrumentosEvaluacion WHERE Nombre = 'Proyecto Multimedia')
BEGIN
    INSERT INTO InstrumentosEvaluacion (Nombre, Descripcion, Activo, FechaCreacion)
    VALUES 
    ('Proyecto Multimedia', 'Creación de presentaciones y videos educativos', 1, GETDATE()),
    ('Programa Informático', 'Desarrollo de aplicaciones o programas básicos', 1, GETDATE()),
    ('Portafolio Digital', 'Compilación electrónica de trabajos y evidencias', 1, GETDATE()),
    ('Investigación Digital', 'Uso ético y efectivo de tecnologías de información', 1, GETDATE());
    
    PRINT '✅ Instrumentos de Informática insertados';
END

PRINT '✅ Todos los instrumentos de evaluación han sido insertados';
PRINT '';

-- =====================================================================
-- 3. ASIGNAR INSTRUMENTOS A MATERIAS
-- =====================================================================

PRINT '3. Asignando instrumentos a materias según el MEP...';

-- Variables para almacenar IDs
DECLARE @PeriodoId INT = 1; -- Asumiendo que existe un período académico

-- =====================================================================
-- 3.1 INSTRUMENTOS TRANSVERSALES (TODAS LAS MATERIAS MEP)
-- =====================================================================

PRINT '3.1 Asignando instrumentos transversales a materias MEP...';

INSERT INTO InstrumentoMaterias (InstrumentoEvaluacionId, MateriaId, PeriodoAcademicoId, FechaAsignacion, OrdenPresentacion, EsObligatorio)
SELECT 
    ie.InstrumentoId,
    m.MateriaId,
    @PeriodoId,
    GETDATE(),
    CASE ie.Nombre 
        WHEN 'Prueba Diagnóstica' THEN 1
        WHEN 'Lista de Cotejo' THEN 2
        WHEN 'Rúbrica Analítica' THEN 3
        WHEN 'Autoevaluación' THEN 4
        WHEN 'Prueba Escrita' THEN 5
        WHEN 'Presentación Oral' THEN 6
        WHEN 'Proyecto de Investigación' THEN 7
        WHEN 'Portafolio de Evidencias' THEN 8
        ELSE 10
    END,
    CASE ie.Nombre 
        WHEN 'Prueba Diagnóstica' THEN 1
        WHEN 'Prueba Escrita' THEN 1
        WHEN 'Rúbrica Analítica' THEN 1
        ELSE 0
    END
FROM InstrumentosEvaluacion ie
CROSS JOIN Materias m
WHERE ie.Nombre IN ('Lista de Cotejo', 'Escala de Calificación', 'Rúbrica Analítica', 
                    'Rúbrica Holística', 'Autoevaluación', 'Coevaluación', 
                    'Observación Sistemática', 'Prueba Diagnóstica', 
                    'Portafolio de Evidencias', 'Prueba Escrita', 'Presentación Oral')
  AND m.Codigo LIKE 'MEP-%'
  AND NOT EXISTS (
      SELECT 1 FROM InstrumentoMaterias im 
      WHERE im.InstrumentoEvaluacionId = ie.InstrumentoId 
        AND im.MateriaId = m.MateriaId
  );

PRINT '✅ Instrumentos transversales asignados';

-- =====================================================================
-- 3.2 INSTRUMENTOS ESPECÍFICOS POR ÁREA
-- =====================================================================

-- ESPAÑOL
INSERT INTO InstrumentoMaterias (InstrumentoEvaluacionId, MateriaId, PeriodoAcademicoId, FechaAsignacion, OrdenPresentacion, EsObligatorio)
SELECT ie.InstrumentoId, m.MateriaId, @PeriodoId, GETDATE(), 20, 1
FROM InstrumentosEvaluacion ie
CROSS JOIN Materias m
WHERE ie.Nombre IN ('Análisis Literario', 'Composición Escrita', 'Debate Estructurado', 
                    'Comprensión Lectora', 'Declamación', 'Dramatización', 'Ensayo')
  AND m.Codigo LIKE 'MEP-ESP-%'
  AND NOT EXISTS (SELECT 1 FROM InstrumentoMaterias im WHERE im.InstrumentoEvaluacionId = ie.InstrumentoId AND im.MateriaId = m.MateriaId);

-- MATEMÁTICAS
INSERT INTO InstrumentoMaterias (InstrumentoEvaluacionId, MateriaId, PeriodoAcademicoId, FechaAsignacion, OrdenPresentacion, EsObligatorio)
SELECT ie.InstrumentoId, m.MateriaId, @PeriodoId, GETDATE(), 20, 1
FROM InstrumentosEvaluacion ie
CROSS JOIN Materias m
WHERE ie.Nombre IN ('Resolución de Problemas', 'Laboratorio Matemático', 'Proyecto Matemático', 
                    'Demostración Matemática', 'Calculadora Gráfica')
  AND m.Codigo LIKE 'MEP-MAT-%'
  AND NOT EXISTS (SELECT 1 FROM InstrumentoMaterias im WHERE im.InstrumentoEvaluacionId = ie.InstrumentoId AND im.MateriaId = m.MateriaId);

-- CIENCIAS
INSERT INTO InstrumentoMaterias (InstrumentoEvaluacionId, MateriaId, PeriodoAcademicoId, FechaAsignacion, OrdenPresentacion, EsObligatorio)
SELECT ie.InstrumentoId, m.MateriaId, @PeriodoId, GETDATE(), 20, 1
FROM InstrumentosEvaluacion ie
CROSS JOIN Materias m
WHERE ie.Nombre IN ('Informe de Laboratorio', 'Proyecto Científico', 'Bitácora de Laboratorio', 
                    'Mapa Conceptual Científico', 'Feria Científica', 'Práctica de Laboratorio')
  AND (m.Codigo LIKE 'MEP-CIE-%' OR m.Codigo LIKE 'MEP-FIS-%' OR m.Codigo LIKE 'MEP-QUI-%' OR m.Codigo LIKE 'MEP-BIO-%')
  AND NOT EXISTS (SELECT 1 FROM InstrumentoMaterias im WHERE im.InstrumentoEvaluacionId = ie.InstrumentoId AND im.MateriaId = m.MateriaId);

-- ESTUDIOS SOCIALES
INSERT INTO InstrumentoMaterias (InstrumentoEvaluacionId, MateriaId, PeriodoAcademicoId, FechaAsignacion, OrdenPresentacion, EsObligatorio)
SELECT ie.InstrumentoId, m.MateriaId, @PeriodoId, GETDATE(), 20, 1
FROM InstrumentosEvaluacion ie
CROSS JOIN Materias m
WHERE ie.Nombre IN ('Ensayo Histórico', 'Línea de Tiempo', 'Análisis de Fuentes', 
                    'Mapa Histórico', 'Debate Social', 'Proyecto Comunitario')
  AND m.Codigo LIKE 'MEP-SS-%'
  AND NOT EXISTS (SELECT 1 FROM InstrumentoMaterias im WHERE im.InstrumentoEvaluacionId = ie.InstrumentoId AND im.MateriaId = m.MateriaId);

-- INGLÉS
INSERT INTO InstrumentoMaterias (InstrumentoEvaluacionId, MateriaId, PeriodoAcademicoId, FechaAsignacion, OrdenPresentacion, EsObligatorio)
SELECT ie.InstrumentoId, m.MateriaId, @PeriodoId, GETDATE(), 20, 1
FROM InstrumentosEvaluacion ie
CROSS JOIN Materias m
WHERE ie.Nombre IN ('Listening Comprehension', 'Speaking Assessment', 'Writing Skills', 
                    'Reading Comprehension', 'Role Play', 'Cultural Project')
  AND m.Codigo LIKE 'MEP-ING-%'
  AND NOT EXISTS (SELECT 1 FROM InstrumentoMaterias im WHERE im.InstrumentoEvaluacionId = ie.InstrumentoId AND im.MateriaId = m.MateriaId);

-- EDUCACIÓN ARTÍSTICA
INSERT INTO InstrumentoMaterias (InstrumentoEvaluacionId, MateriaId, PeriodoAcademicoId, FechaAsignacion, OrdenPresentacion, EsObligatorio)
SELECT ie.InstrumentoId, m.MateriaId, @PeriodoId, GETDATE(), 20, 1
FROM InstrumentosEvaluacion ie
CROSS JOIN Materias m
WHERE ie.Nombre IN ('Portafolio Artístico', 'Presentación Musical', 'Crítica Artística', 
                    'Exposición Artística', 'Performance Artístico')
  AND (m.Codigo LIKE 'MEP-ART-%' OR m.Codigo LIKE 'MEP-MUS-%' OR m.Nombre LIKE '%Artística%' OR m.Nombre LIKE '%Musical%')
  AND NOT EXISTS (SELECT 1 FROM InstrumentoMaterias im WHERE im.InstrumentoEvaluacionId = ie.InstrumentoId AND im.MateriaId = m.MateriaId);

-- EDUCACIÓN FÍSICA
INSERT INTO InstrumentoMaterias (InstrumentoEvaluacionId, MateriaId, PeriodoAcademicoId, FechaAsignacion, OrdenPresentacion, EsObligatorio)
SELECT ie.InstrumentoId, m.MateriaId, @PeriodoId, GETDATE(), 20, 1
FROM InstrumentosEvaluacion ie
CROSS JOIN Materias m
WHERE ie.Nombre IN ('Prueba de Aptitud Física', 'Proyecto Vida Saludable', 'Evaluación Deportiva', 'Plan de Ejercicios')
  AND (m.Codigo LIKE 'MEP-EF-%' OR m.Nombre LIKE '%Física%' OR m.Nombre LIKE '%Deportes%')
  AND NOT EXISTS (SELECT 1 FROM InstrumentoMaterias im WHERE im.InstrumentoEvaluacionId = ie.InstrumentoId AND im.MateriaId = m.MateriaId);

-- EDUCACIÓN TÉCNICA
INSERT INTO InstrumentoMaterias (InstrumentoEvaluacionId, MateriaId, PeriodoAcademicoId, FechaAsignacion, OrdenPresentacion, EsObligatorio)
SELECT ie.InstrumentoId, m.MateriaId, @PeriodoId, GETDATE(), 20, 1
FROM InstrumentosEvaluacion ie
CROSS JOIN Materias m
WHERE ie.Nombre IN ('Proyecto Técnico', 'Informe de Práctica', 'Evaluación de Destrezas', 'Simulación Laboral')
  AND (m.Codigo LIKE 'MEP-TEC-%' OR m.Tipo = 'Técnica' OR m.Nombre LIKE '%Técnica%')
  AND NOT EXISTS (SELECT 1 FROM InstrumentoMaterias im WHERE im.InstrumentoEvaluacionId = ie.InstrumentoId AND im.MateriaId = m.MateriaId);

-- INFORMÁTICA
INSERT INTO InstrumentoMaterias (InstrumentoEvaluacionId, MateriaId, PeriodoAcademicoId, FechaAsignacion, OrdenPresentacion, EsObligatorio)
SELECT ie.InstrumentoId, m.MateriaId, @PeriodoId, GETDATE(), 20, 1
FROM InstrumentosEvaluacion ie
CROSS JOIN Materias m
WHERE ie.Nombre IN ('Proyecto Multimedia', 'Programa Informático', 'Portafolio Digital', 'Investigación Digital')
  AND (m.Codigo LIKE 'MEP-INF-%' OR m.Nombre LIKE '%Informática%' OR m.Nombre LIKE '%Computación%')
  AND NOT EXISTS (SELECT 1 FROM InstrumentoMaterias im WHERE im.InstrumentoEvaluacionId = ie.InstrumentoId AND im.MateriaId = m.MateriaId);

PRINT '✅ Instrumentos específicos asignados por área';
PRINT '';

-- =====================================================================
-- 4. VERIFICACIÓN FINAL SIMPLIFICADA
-- =====================================================================

PRINT '4. Verificación final...';

DECLARE @TotalInstrumentos INT = (SELECT COUNT(*) FROM InstrumentosEvaluacion);
DECLARE @TotalAsignaciones INT = (SELECT COUNT(*) FROM InstrumentoMaterias WHERE MateriaId IN (SELECT MateriaId FROM Materias WHERE Codigo LIKE 'MEP-%'));
DECLARE @TotalMateriasMEP INT = (SELECT COUNT(*) FROM Materias WHERE Codigo LIKE 'MEP-%');

PRINT '';
PRINT 'ESTADÍSTICAS FINALES:';
PRINT '====================';
PRINT 'Total de instrumentos: ' + CAST(@TotalInstrumentos AS VARCHAR(10));
PRINT 'Total de materias MEP: ' + CAST(@TotalMateriasMEP AS VARCHAR(10));
PRINT 'Total de asignaciones: ' + CAST(@TotalAsignaciones AS VARCHAR(10));

IF @TotalMateriasMEP > 0
    PRINT 'Promedio por materia: ' + CAST(@TotalAsignaciones / @TotalMateriasMEP AS VARCHAR(10));

PRINT '';
PRINT '🎉 ¡SCRIPT COMPLETADO EXITOSAMENTE!';
PRINT '===================================';
PRINT 'Los instrumentos de evaluación del MEP han sido insertados';
PRINT 'y asignados correctamente a las materias correspondientes.';
PRINT '';

GO
