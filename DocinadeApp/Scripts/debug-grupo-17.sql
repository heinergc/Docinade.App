-- Script para diagnosticar el error con el grupo ID 17

-- 1. Verificar si el grupo 17 existe
SELECT 
    'GRUPO 17 - INFORMACIÓN BÁSICA' AS Seccion,
    GrupoId,
    Codigo,
    Nombre,
    TipoGrupo,
    Nivel,
    CapacidadMaxima,
    PeriodoAcademicoId,
    Estado,
    FechaCreacion
FROM GruposEstudiantes 
WHERE GrupoId = 17;

-- 2. Verificar el período académico del grupo 17
SELECT 
    'PERIODO ACADÉMICO DEL GRUPO 17' AS Seccion,
    pa.Id,
    pa.Codigo,
    pa.Nombre,
    pa.Estado,
    pa.Activo,
    pa.FechaInicio,
    pa.FechaFin
FROM GruposEstudiantes g
INNER JOIN PeriodosAcademicos pa ON g.PeriodoAcademicoId = pa.Id
WHERE g.GrupoId = 17;

-- 3. Contar estudiantes en el período del grupo 17
SELECT 
    'ESTUDIANTES EN EL PERIODO' AS Seccion,
    COUNT(*) AS TotalEstudiantes
FROM Estudiantes e
INNER JOIN GruposEstudiantes g ON e.PeriodoAcademicoId = g.PeriodoAcademicoId
WHERE g.GrupoId = 17;

-- 4. Estudiantes ya asignados al grupo 17
SELECT 
    'ESTUDIANTES YA ASIGNADOS AL GRUPO 17' AS Seccion,
    e.IdEstudiante,
    e.NumeroId,
    e.NombreCompleto,
    eg.Estado,
    eg.FechaAsignacion
FROM EstudianteGrupos eg
INNER JOIN Estudiantes e ON eg.EstudianteId = e.IdEstudiante
WHERE eg.GrupoId = 17 AND eg.Estado = 'Activo';

-- 5. Estudiantes disponibles para asignar al grupo 17 (primeros 10)
SELECT TOP 10
    'ESTUDIANTES DISPONIBLES PARA GRUPO 17' AS Seccion,
    e.IdEstudiante,
    e.NumeroId,
    e.NombreCompleto,
    e.DireccionCorreo,
    e.PeriodoAcademicoId
FROM Estudiantes e
INNER JOIN GruposEstudiantes g ON e.PeriodoAcademicoId = g.PeriodoAcademicoId
WHERE g.GrupoId = 17
  AND NOT EXISTS (
    SELECT 1 FROM EstudianteGrupos eg 
    WHERE eg.EstudianteId = e.IdEstudiante 
      AND eg.GrupoId = 17 
      AND eg.Estado = 'Activo'
  );

-- 6. Verificar tablas necesarias
SELECT 
    'VERIFICACIÓN DE TABLAS' AS Seccion,
    'GruposEstudiantes' AS Tabla,
    COUNT(*) AS RegistrosTotal
FROM GruposEstudiantes
UNION ALL
SELECT 
    'VERIFICACIÓN DE TABLAS' AS Seccion,
    'Estudiantes' AS Tabla,
    COUNT(*) AS RegistrosTotal
FROM Estudiantes
UNION ALL
SELECT 
    'VERIFICACIÓN DE TABLAS' AS Seccion,
    'EstudianteGrupos' AS Tabla,
    COUNT(*) AS RegistrosTotal
FROM EstudianteGrupos
UNION ALL
SELECT 
    'VERIFICACIÓN DE TABLAS' AS Seccion,
    'PeriodosAcademicos' AS Tabla,
    COUNT(*) AS RegistrosTotal
FROM PeriodosAcademicos;
