-- Cuaderno Calificador Automático - Primer Cuatrimestre 2025
-- Script SQL de demostración para entender la lógica del cuaderno

-- =====================================================================
-- CONSULTA BASE: Obtener estructura del cuaderno para una materia
-- =====================================================================

-- Parámetros de ejemplo
DECLARE @MateriaId INT = 1;  -- ID de la materia
DECLARE @PeriodoAcademicoId INT = 1;  -- ID del "Primer Cuatrimestre 2025"

-- 1. Obtener información básica de la materia y periodo
SELECT 
    m.MateriaId,
    m.Nombre AS MateriaNombre,
    p.Id AS PeriodoId,
    p.NombreCompleto AS PeriodoNombre
FROM Materias m
CROSS JOIN PeriodosAcademicos p
WHERE m.MateriaId = @MateriaId 
  AND p.Id = @PeriodoAcademicoId
  AND m.Activa = 1 
  AND p.Activo = 1;

-- 2. Obtener columnas dinámicas (Instrumento ? Rúbrica) para la materia en el periodo
SELECT DISTINCT
    ie.InstrumentoId,
    ie.Nombre AS InstrumentoNombre,
    r.IdRubrica AS RubricaId,
    r.NombreRubrica,
    ir.Ponderacion,
    COALESCE(ir.OrdenPresentacion, 0) AS OrdenPresentacion,
    -- Clave única para la columna
    CAST(ie.InstrumentoId AS VARCHAR) + '_' + CAST(r.IdRubrica AS VARCHAR) AS ClaveColumna
FROM InstrumentoMaterias im
INNER JOIN InstrumentosEvaluacion ie ON im.InstrumentoEvaluacionId = ie.InstrumentoId
INNER JOIN InstrumentoRubricas ir ON ie.InstrumentoId = ir.InstrumentoEvaluacionId
INNER JOIN Rubricas r ON ir.RubricaId = r.IdRubrica
WHERE im.MateriaId = @MateriaId 
  AND im.PeriodoAcademicoId = @PeriodoAcademicoId
  AND ie.Activo = 1
  AND r.Estado = 'ACTIVO'
ORDER BY ie.InstrumentoId, ir.OrdenPresentacion;

-- 3. Obtener estudiantes de la materia en el periodo
SELECT 
    e.IdEstudiante,
    e.Apellidos + ', ' + e.Nombre AS EstudianteNombre,
    e.NumeroId,
    CASE WHEN e.Activo = 1 THEN 'ACTIVO' ELSE 'INACTIVO' END AS Estado
FROM Estudiantes e
WHERE e.PeriodoAcademicoId = @PeriodoAcademicoId
  AND e.Activo = 1  -- Cambiar a 0 para incluir inactivos
ORDER BY e.Apellidos, e.Nombre;

-- =====================================================================
-- CONSULTA PRINCIPAL: Generar filas del cuaderno con calificaciones
-- =====================================================================

WITH ColumnasMateria AS (
    -- Obtener todas las combinaciones Instrumento-Rúbrica para la materia
    SELECT DISTINCT
        ie.InstrumentoId,
        ie.Nombre AS InstrumentoNombre,
        r.IdRubrica AS RubricaId,
        r.NombreRubrica,
        ir.Ponderacion,
        CAST(ie.InstrumentoId AS VARCHAR) + '_' + CAST(r.IdRubrica AS VARCHAR) AS ClaveColumna
    FROM InstrumentoMaterias im
    INNER JOIN InstrumentosEvaluacion ie ON im.InstrumentoEvaluacionId = ie.InstrumentoId
    INNER JOIN InstrumentoRubricas ir ON ie.InstrumentoId = ir.InstrumentoEvaluacionId
    INNER JOIN Rubricas r ON ir.RubricaId = r.IdRubrica
    WHERE im.MateriaId = @MateriaId 
      AND im.PeriodoAcademicoId = @PeriodoAcademicoId
      AND ie.Activo = 1
      AND r.Estado = 'ACTIVO'
),
EstudiantesMateria AS (
    -- Obtener estudiantes del periodo
    SELECT 
        e.IdEstudiante,
        e.Apellidos + ', ' + e.Nombre AS EstudianteNombre,
        e.NumeroId
    FROM Estudiantes e
    WHERE e.PeriodoAcademicoId = @PeriodoAcademicoId
      AND e.Activo = 1
),
CalificacionesBase AS (
    -- Obtener calificaciones de los estudiantes para las rúbricas relevantes
    SELECT 
        ev.IdEstudiante,
        ev.IdRubrica,
        COALESCE(ev.TotalPuntos, 0) AS Calificacion,
        ev.Estado
    FROM Evaluaciones ev
    WHERE ev.IdRubrica IN (SELECT RubricaId FROM ColumnasMateria)
      AND ev.Estado = 'FINALIZADA'
)

-- Resultado principal: Filas del cuaderno con calificaciones pivoteadas
SELECT 
    em.IdEstudiante,
    em.EstudianteNombre,
    em.NumeroId,
    -- Columnas dinámicas para cada Instrumento-Rúbrica
    -- Ejemplo para 3 columnas específicas (se generarían dinámicamente):
    COALESCE(
        (SELECT cb.Calificacion 
         FROM CalificacionesBase cb 
         WHERE cb.IdEstudiante = em.IdEstudiante 
           AND cb.IdRubrica = (SELECT TOP 1 RubricaId FROM ColumnasMateria WHERE InstrumentoId = 1)
        ), 0
    ) AS Instrumento1_Rubrica1,
    
    COALESCE(
        (SELECT cb.Calificacion 
         FROM CalificacionesBase cb 
         WHERE cb.IdEstudiante = em.IdEstudiante 
           AND cb.IdRubrica = (SELECT TOP 1 RubricaId FROM ColumnasMateria WHERE InstrumentoId = 2)
        ), 0
    ) AS Instrumento2_Rubrica2,
    
    COALESCE(
        (SELECT cb.Calificacion 
         FROM CalificacionesBase cb 
         WHERE cb.IdEstudiante = em.IdEstudiante 
           AND cb.IdRubrica = (SELECT TOP 1 RubricaId FROM ColumnasMateria WHERE InstrumentoId = 3)
        ), 0
    ) AS Instrumento3_Rubrica3,
    
    -- Cálculo del Total Final (ejemplo con 3 instrumentos)
    ROUND(
        COALESCE(
            (SELECT cb.Calificacion FROM CalificacionesBase cb 
             WHERE cb.IdEstudiante = em.IdEstudiante 
               AND cb.IdRubrica = (SELECT TOP 1 RubricaId FROM ColumnasMateria WHERE InstrumentoId = 1)
            ), 0
        ) * 0.30 +  -- Tarea 1: 30%
        COALESCE(
            (SELECT cb.Calificacion FROM CalificacionesBase cb 
             WHERE cb.IdEstudiante = em.IdEstudiante 
               AND cb.IdRubrica = (SELECT TOP 1 RubricaId FROM ColumnasMateria WHERE InstrumentoId = 2)
            ), 0
        ) * 0.30 +  -- Tarea 2: 30%
        COALESCE(
            (SELECT cb.Calificacion FROM CalificacionesBase cb 
             WHERE cb.IdEstudiante = em.IdEstudiante 
               AND cb.IdRubrica = (SELECT TOP 1 RubricaId FROM ColumnasMateria WHERE InstrumentoId = 3)
            ), 0
        ) * 0.40,   -- Proyecto 1: 40%
        2
    ) AS TotalFinal

FROM EstudiantesMateria em
ORDER BY em.EstudianteNombre;

-- =====================================================================
-- CONSULTA DE ESTADÍSTICAS
-- =====================================================================

WITH DatosCompletos AS (
    -- Reutilizar la lógica anterior para obtener datos completos
    SELECT 
        em.IdEstudiante,
        em.EstudianteNombre,
        -- Calcular total final por estudiante
        ROUND(
            AVG(COALESCE(cb.Calificacion, 0)) * 
            CASE 
                WHEN COUNT(DISTINCT cm.InstrumentoId) = 3 THEN 1.0  -- Si hay 3 instrumentos, usar promedio directo
                ELSE 1.0  -- Ajustar según la lógica de ponderación
            END,
            2
        ) AS TotalFinal,
        COUNT(DISTINCT cm.RubricaId) AS TotalRubricasEstudiante,
        COUNT(DISTINCT CASE WHEN cb.Calificacion > 0 THEN cm.RubricaId END) AS RubricasConNota
    FROM EstudiantesMateria em
    CROSS JOIN ColumnasMateria cm
    LEFT JOIN CalificacionesBase cb ON em.IdEstudiante = cb.IdEstudiante AND cm.RubricaId = cb.IdRubrica
    GROUP BY em.IdEstudiante, em.EstudianteNombre
)

SELECT 
    COUNT(*) AS TotalEstudiantes,
    (SELECT COUNT(DISTINCT InstrumentoId) FROM ColumnasMateria) AS TotalInstrumentos,
    (SELECT COUNT(*) FROM ColumnasMateria) AS TotalRubricas,
    ROUND(AVG(TotalFinal), 2) AS PromedioGeneral,
    MAX(TotalFinal) AS NotaMaxima,
    MIN(TotalFinal) AS NotaMinima,
    COUNT(CASE WHEN RubricasConNota = TotalRubricasEstudiante THEN 1 END) AS EstudiantesConTodasLasNotas,
    COUNT(CASE WHEN RubricasConNota < TotalRubricasEstudiante THEN 1 END) AS EstudiantesConNotasPendientes
FROM DatosCompletos;

-- =====================================================================
-- EJEMPLO DE FÓRMULA DE CÁLCULO
-- =====================================================================

-- Para un estudiante con las siguientes calificaciones:
-- Tarea 1 (Rúbrica A): 100 puntos
-- Tarea 2 (Rúbrica B): 80 puntos  
-- Proyecto 1 (Rúbrica C): 90 puntos

-- Cálculo del Total Final:
-- Total = (100 × 0.30) + (80 × 0.30) + (90 × 0.40)
-- Total = 30 + 24 + 36 = 90.00

SELECT 
    'Ejemplo de Cálculo' AS Descripcion,
    100 AS Tarea1_Nota,
    80 AS Tarea2_Nota,
    90 AS Proyecto1_Nota,
    ROUND((100 * 0.30) + (80 * 0.30) + (90 * 0.40), 2) AS TotalFinal_Calculado;

-- =====================================================================
-- VISTA RECOMENDADA PARA IMPLEMENTACIÓN
-- =====================================================================

-- Esta vista podría crearse dinámicamente para cada materia-periodo
-- Nombre sugerido: vw_Calificador_PQ2025_MateriaX

/*
CREATE VIEW vw_Calificador_PQ2025_Materia1 AS
WITH [lógica anterior]
SELECT 
    IdEstudiante,
    EstudianteNombre,
    NumeroId,
    [Columnas dinámicas generadas en runtime],
    TotalFinal
FROM [consulta principal]
*/