-- Script para actualizar el ProfesorGuiaId en boletas existentes
-- que no tienen profesor guía asignado
-- Ejecutar en SQL Server Management Studio o sqlcmd

USE RubricasDb;
GO

PRINT '========================================';
PRINT 'ACTUALIZACIÓN DE PROFESORES GUÍA';
PRINT 'EN BOLETAS EXISTENTES';
PRINT '========================================';
PRINT '';

-- Primero, verificar cuántas boletas no tienen profesor guía
DECLARE @BoletasSinProfesor INT;
SELECT @BoletasSinProfesor = COUNT(*)
FROM BoletasConducta
WHERE ProfesorGuiaId IS NULL OR ProfesorGuiaId = '';

PRINT 'Boletas sin profesor guía asignado: ' + CAST(@BoletasSinProfesor AS VARCHAR(10));
PRINT '';

-- Mostrar las boletas que se van a actualizar
PRINT 'Boletas que se van a actualizar:';
PRINT '========================================';
SELECT 
    bc.IdBoleta,
    bc.IdEstudiante,
    e.Nombre + ' ' + e.Apellidos AS Estudiante,
    eg.GrupoId,
    g.Nombre AS Grupo,
    pg.ProfesorId,
    p.Nombre + ' ' + p.Apellido AS ProfesorGuia,
    u.Id AS UserId,
    u.Email
FROM BoletasConducta bc
INNER JOIN Estudiantes e ON bc.IdEstudiante = e.IdEstudiante
LEFT JOIN EstudianteGrupos eg ON bc.IdEstudiante = eg.EstudianteId 
    AND eg.Estado = 1 
    AND eg.EsGrupoPrincipal = 1
LEFT JOIN GruposEstudiantes g ON eg.GrupoId = g.Id
LEFT JOIN ProfesorGuia pg ON eg.GrupoId = pg.GrupoId AND pg.Estado = 1
LEFT JOIN Profesores p ON pg.ProfesorId = p.Id
LEFT JOIN AspNetUsers u ON p.Cedula = u.NumeroIdentificacion
WHERE (bc.ProfesorGuiaId IS NULL OR bc.ProfesorGuiaId = '')
    AND bc.Estado = 'Activa';
GO

PRINT '';
PRINT '========================================';
PRINT 'Ejecutando actualización...';
PRINT '========================================';

-- Actualizar las boletas con el profesor guía correspondiente
UPDATE bc
SET 
    bc.ProfesorGuiaId = u.Id,
    bc.NotificacionEnviada = CASE 
        WHEN u.Id IS NOT NULL THEN 0  -- Marcar como pendiente de envío
        ELSE bc.NotificacionEnviada 
    END
FROM BoletasConducta bc
INNER JOIN Estudiantes e ON bc.IdEstudiante = e.IdEstudiante
INNER JOIN EstudianteGrupos eg ON bc.IdEstudiante = eg.EstudianteId 
    AND eg.Estado = 1 
    AND eg.EsGrupoPrincipal = 1
INNER JOIN ProfesorGuia pg ON eg.GrupoId = pg.GrupoId 
    AND pg.Estado = 1
INNER JOIN Profesores p ON pg.ProfesorId = p.Id
INNER JOIN AspNetUsers u ON p.Cedula = u.NumeroIdentificacion
WHERE (bc.ProfesorGuiaId IS NULL OR bc.ProfesorGuiaId = '')
    AND bc.Estado = 'Activa';

DECLARE @FilasActualizadas INT = @@ROWCOUNT;
PRINT '';
PRINT 'Boletas actualizadas: ' + CAST(@FilasActualizadas AS VARCHAR(10));
PRINT '';

-- Verificar el resultado
PRINT '========================================';
PRINT 'Verificación post-actualización:';
PRINT '========================================';
SELECT 
    bc.IdBoleta,
    bc.IdEstudiante,
    e.Nombre + ' ' + e.Apellidos AS Estudiante,
    bc.ProfesorGuiaId,
    u.Nombre + ' ' + u.Apellidos AS ProfesorGuia,
    u.Email,
    bc.NotificacionEnviada,
    bc.Estado
FROM BoletasConducta bc
INNER JOIN Estudiantes e ON bc.IdEstudiante = e.IdEstudiante
LEFT JOIN AspNetUsers u ON bc.ProfesorGuiaId = u.Id
WHERE bc.Estado = 'Activa'
ORDER BY bc.IdBoleta DESC;
GO

PRINT '';
PRINT '========================================';
PRINT 'ACTUALIZACIÓN COMPLETADA';
PRINT '========================================';
