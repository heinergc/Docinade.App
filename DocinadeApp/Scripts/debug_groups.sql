-- Debug script para verificar datos de grupos C1 y Grupo 1

-- 1. Verificar los grupos disponibles
SELECT 
    GrupoId,
    Codigo,
    Nombre,
    Estado,
    PeriodoAcademicoId
FROM GruposEstudiantes
WHERE (Codigo = 'C1' OR Nombre = 'Grupo 1' OR Nombre LIKE '%Grupo 1%' OR Codigo LIKE '%C1%')
ORDER BY GrupoId;

-- 2. Verificar estudiantes asignados a estos grupos
SELECT 
    g.GrupoId,
    g.Codigo,
    g.Nombre as NombreGrupo,
    eg.Id as AsignacionId,
    eg.EstudianteId,
    e.Nombres + ' ' + e.Apellidos as NombreEstudiante,
    eg.Estado,
    eg.FechaAsignacion
FROM GruposEstudiantes g
LEFT JOIN EstudianteGrupos eg ON g.GrupoId = eg.GrupoId
LEFT JOIN Estudiantes e ON eg.EstudianteId = e.EstudianteId
WHERE (g.Codigo = 'C1' OR g.Nombre = 'Grupo 1' OR g.Nombre LIKE '%Grupo 1%' OR g.Codigo LIKE '%C1%')
ORDER BY g.GrupoId, eg.EstudianteId;

-- 3. Verificar materias asignadas a estos grupos
SELECT 
    g.GrupoId,
    g.Codigo,
    g.Nombre as NombreGrupo,
    gm.Id as AsignacionMateriaId,
    gm.MateriaId,
    m.Nombre as NombreMateria,
    gm.Estado,
    gm.FechaAsignacion
FROM GruposEstudiantes g
LEFT JOIN GrupoMaterias gm ON g.GrupoId = gm.GrupoId
LEFT JOIN Materias m ON gm.MateriaId = m.MateriaId
WHERE (g.Codigo = 'C1' OR g.Nombre = 'Grupo 1' OR g.Nombre LIKE '%Grupo 1%' OR g.Codigo LIKE '%C1%')
ORDER BY g.GrupoId, gm.MateriaId;

-- 4. Verificar estados disponibles
SELECT DISTINCT Estado FROM EstudianteGrupos;
SELECT DISTINCT Estado FROM GrupoMaterias;

-- 5. Contar totales por estado
SELECT 
    g.Codigo,
    g.Nombre,
    COUNT(CASE WHEN eg.Estado = 1 THEN 1 END) as EstudiantesActivos,
    COUNT(CASE WHEN eg.Estado = 2 THEN 1 END) as EstudiantesInactivos,
    COUNT(eg.Id) as TotalEstudiantes
FROM GruposEstudiantes g
LEFT JOIN EstudianteGrupos eg ON g.GrupoId = eg.GrupoId
WHERE (g.Codigo = 'C1' OR g.Nombre = 'Grupo 1' OR g.Nombre LIKE '%Grupo 1%' OR g.Codigo LIKE '%C1%')
GROUP BY g.GrupoId, g.Codigo, g.Nombre;

-- 6. Verificar enum de estados
-- Estado 1 = Activo
-- Estado 2 = Inactivo  
-- Estado 3 = Trasladado