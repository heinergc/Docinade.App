-- Script de diagnóstico para verificar estudiantes

USE RubricasDb;
GO

PRINT '🔍 === DIAGNÓSTICO DE ESTUDIANTES ===';

-- 1. Verificar períodos académicos
PRINT '📅 Períodos Académicos:';
SELECT Id, Nombre, Activo, FechaInicio, FechaFin FROM PeriodosAcademicos ORDER BY Id;

-- 2. Verificar total de estudiantes
DECLARE @TotalEstudiantes INT;
SELECT @TotalEstudiantes = COUNT(*) FROM Estudiantes;
PRINT '👥 Total de estudiantes: ' + CAST(@TotalEstudiantes AS VARCHAR);

-- 3. Verificar estudiantes por período
PRINT '📊 Estudiantes por período:';
SELECT 
    pa.Nombre as Periodo,
    COUNT(e.IdEstudiante) as CantidadEstudiantes
FROM PeriodosAcademicos pa
LEFT JOIN Estudiantes e ON pa.Id = e.PeriodoAcademicoId
GROUP BY pa.Id, pa.Nombre
ORDER BY pa.Id;

-- 4. Verificar grupos
PRINT '🏫 Grupos disponibles:';
SELECT GrupoId, Codigo, Nombre, CapacidadMaxima, PeriodoAcademicoId FROM GruposEstudiantes ORDER BY GrupoId;

-- 5. Verificar asignaciones de estudiantes a grupos
PRINT '👨‍🎓 Asignaciones estudiante-grupo:';
SELECT 
    eg.GrupoId,
    ge.Codigo as GrupoCodigo,
    COUNT(*) as EstudiantesAsignados
FROM EstudianteGrupos eg
INNER JOIN GruposEstudiantes ge ON eg.GrupoId = ge.GrupoId
WHERE eg.Estado = 'Activo'
GROUP BY eg.GrupoId, ge.Codigo
ORDER BY eg.GrupoId;

-- 6. Listar primeros 10 estudiantes para verificar
PRINT '📋 Primeros 10 estudiantes:';
SELECT TOP 10 
    IdEstudiante, 
    Nombre, 
    Apellidos, 
    NumeroId, 
    DireccionCorreo,
    PeriodoAcademicoId
FROM Estudiantes 
ORDER BY IdEstudiante;

PRINT '✅ Diagnóstico completado';
