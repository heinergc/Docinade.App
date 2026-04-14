-- Script de prueba para verificar la eliminación en cascada de profesores
-- Este script verifica el estado antes y después de eliminar un profesor

USE RubricasDb;
GO

-- ===================================================================
-- PASO 1: Verificar datos ANTES de la eliminación
-- ===================================================================
PRINT '=== ESTADO ANTES DE LA ELIMINACIÓN ==='
PRINT ''

DECLARE @ProfesorId INT = 1; -- Cambiar por el ID del profesor a eliminar

-- Información del profesor
PRINT '--- Profesor ---'
SELECT Id, Nombres, PrimerApellido, SegundoApellido, Cedula, EmailInstitucional
FROM Profesores 
WHERE Id = @ProfesorId;

-- Formaciones Académicas
PRINT ''
PRINT '--- Formaciones Académicas ---'
SELECT COUNT(*) AS TotalFormaciones 
FROM ProfesorFormacionAcademica 
WHERE ProfesorId = @ProfesorId;

SELECT Id, TipoFormacion, TituloObtenido, InstitucionEducativa, AnioInicio, AnioFinalizacion
FROM ProfesorFormacionAcademica 
WHERE ProfesorId = @ProfesorId;

-- Experiencias Laborales
PRINT ''
PRINT '--- Experiencias Laborales ---'
SELECT COUNT(*) AS TotalExperiencias 
FROM ProfesorExperienciaLaboral 
WHERE ProfesorId = @ProfesorId;

SELECT Id, CargoDesempenado, NombreInstitucion, FechaInicio, FechaFin
FROM ProfesorExperienciaLaboral 
WHERE ProfesorId = @ProfesorId;

-- Capacitaciones
PRINT ''
PRINT '--- Capacitaciones ---'
SELECT COUNT(*) AS TotalCapacitaciones 
FROM ProfesorCapacitacion 
WHERE ProfesorId = @ProfesorId;

SELECT Id, NombreCapacitacion, TipoCapacitacion, InstitucionOrganizadora, HorasCapacitacion, CalificacionObtenida
FROM ProfesorCapacitacion 
WHERE ProfesorId = @ProfesorId;

-- Asignaciones de Grupos
PRINT ''
PRINT '--- Asignaciones de Grupos (ProfesorGrupo) ---'
SELECT COUNT(*) AS TotalAsignaciones 
FROM ProfesorGrupo 
WHERE ProfesorId = @ProfesorId;

SELECT pg.Id, pg.GrupoId, pg.MateriaId, pg.PeriodoAcademicoId, 
       g.Nombre AS Grupo, m.Nombre AS Materia, p.Nombre AS Periodo
FROM ProfesorGrupo pg
LEFT JOIN GruposEstudiantes g ON pg.GrupoId = g.GrupoId
LEFT JOIN Materias m ON pg.MateriaId = m.MateriaId
LEFT JOIN PeriodosAcademicos p ON pg.PeriodoAcademicoId = p.Id
WHERE pg.ProfesorId = @ProfesorId;

-- Asignaciones como Profesor Guía
PRINT ''
PRINT '--- Asignaciones como Profesor Guía ---'
SELECT COUNT(*) AS TotalGuias 
FROM ProfesorGuia 
WHERE ProfesorId = @ProfesorId;

SELECT pg.Id, pg.GrupoId, g.Nombre AS Grupo, pg.FechaInicio, pg.FechaFin
FROM ProfesorGuia pg
LEFT JOIN GruposEstudiantes g ON pg.GrupoId = g.GrupoId
WHERE pg.ProfesorId = @ProfesorId;

PRINT ''
PRINT '=== FIN DEL REPORTE ANTES DE LA ELIMINACIÓN ==='
PRINT ''
PRINT 'Para eliminar este profesor, use la interfaz web:'
PRINT 'https://localhost:18163/Profesores/Delete/' + CAST(@ProfesorId AS NVARCHAR(10))
PRINT ''
PRINT 'O ejecute el siguiente comando para verificar DESPUÉS de la eliminación:'
PRINT 'SELECT COUNT(*) FROM Profesores WHERE Id = ' + CAST(@ProfesorId AS NVARCHAR(10))
PRINT 'SELECT COUNT(*) FROM ProfesorFormacionAcademica WHERE ProfesorId = ' + CAST(@ProfesorId AS NVARCHAR(10))
PRINT 'SELECT COUNT(*) FROM ProfesorExperienciaLaboral WHERE ProfesorId = ' + CAST(@ProfesorId AS NVARCHAR(10))
PRINT 'SELECT COUNT(*) FROM ProfesorCapacitacion WHERE ProfesorId = ' + CAST(@ProfesorId AS NVARCHAR(10))
PRINT 'SELECT COUNT(*) FROM ProfesorGrupo WHERE ProfesorId = ' + CAST(@ProfesorId AS NVARCHAR(10))
PRINT 'SELECT COUNT(*) FROM ProfesorGuia WHERE ProfesorId = ' + CAST(@ProfesorId AS NVARCHAR(10))
GO

-- ===================================================================
-- PASO 2: Verificar datos DESPUÉS de la eliminación (ejecutar manualmente)
-- ===================================================================
/*
-- Descomentar y ejecutar DESPUÉS de eliminar el profesor desde la web

USE RubricasDb;
GO

PRINT '=== VERIFICACIÓN DESPUÉS DE LA ELIMINACIÓN ==='
PRINT ''

DECLARE @ProfesorId INT = 1; -- Usar el mismo ID que se eliminó

PRINT 'Registros restantes (deben ser TODOS cero):'
PRINT ''

SELECT 
    'Profesor' AS Tabla,
    COUNT(*) AS Registros 
FROM Profesores 
WHERE Id = @ProfesorId
UNION ALL
SELECT 
    'ProfesorFormacionAcademica' AS Tabla,
    COUNT(*) AS Registros 
FROM ProfesorFormacionAcademica 
WHERE ProfesorId = @ProfesorId
UNION ALL
SELECT 
    'ProfesorExperienciaLaboral' AS Tabla,
    COUNT(*) AS Registros 
FROM ProfesorExperienciaLaboral 
WHERE ProfesorId = @ProfesorId
UNION ALL
SELECT 
    'ProfesorCapacitacion' AS Tabla,
    COUNT(*) AS Registros 
FROM ProfesorCapacitacion 
WHERE ProfesorId = @ProfesorId
UNION ALL
SELECT 
    'ProfesorGrupo' AS Tabla,
    COUNT(*) AS Registros 
FROM ProfesorGrupo 
WHERE ProfesorId = @ProfesorId
UNION ALL
SELECT 
    'ProfesorGuia' AS Tabla,
    COUNT(*) AS Registros 
FROM ProfesorGuia 
WHERE ProfesorId = @ProfesorId;

PRINT ''
PRINT 'Si todos los registros son 0, la eliminación en cascada funcionó correctamente.'
GO
*/
