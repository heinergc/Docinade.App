-- Script para verificar la asignación de Profesor Guía en Boletas de Conducta
-- Ejecutar en SQL Server Management Studio o sqlcmd

USE RubricasDb;
GO

-- 1. Verificar estudiantes y sus grupos principales
PRINT '========================================';
PRINT '1. ESTUDIANTES Y SUS GRUPOS PRINCIPALES';
PRINT '========================================';
SELECT 
    e.IdEstudiante,
    e.Nombre + ' ' + e.Apellidos AS NombreEstudiante,
    e.NumeroId AS Cedula,
    eg.GrupoId,
    g.Nombre AS NombreGrupo,
    eg.EsGrupoPrincipal,
    eg.Estado
FROM Estudiantes e
LEFT JOIN EstudianteGrupos eg ON e.IdEstudiante = eg.EstudianteId 
    AND eg.Estado = 1 -- EstadoAsignacion.Activo
    AND eg.EsGrupoPrincipal = 1
LEFT JOIN GruposEstudiantes g ON eg.GrupoId = g.GrupoId
ORDER BY e.IdEstudiante;
GO

-- 2. Verificar profesores guía asignados a grupos
PRINT '';
PRINT '========================================';
PRINT '2. PROFESORES GUÍA POR GRUPO';
PRINT '========================================';
SELECT 
    pg.Id,
    pg.GrupoId,
    g.Nombre AS NombreGrupo,
    pg.ProfesorId,
    p.Nombres + ' ' + p.PrimerApellido + ' ' + ISNULL(p.SegundoApellido, '') AS NombreProfesor,
    p.Cedula AS CedulaProfesor,
    pg.Estado,
    pg.FechaAsignacion
FROM ProfesorGuia pg
INNER JOIN GruposEstudiantes g ON pg.GrupoId = g.GrupoId
INNER JOIN Profesores p ON pg.ProfesorId = p.Id
ORDER BY pg.GrupoId, pg.FechaAsignacion DESC;
GO

-- 3. Verificar usuarios (ApplicationUser) de profesores
PRINT '';
PRINT '========================================';
PRINT '3. USUARIOS DE PROFESORES';
PRINT '========================================';
SELECT 
    u.Id AS UserId,
    u.UserName,
    u.Email,
    u.Nombre + ' ' + u.Apellidos AS NombreCompleto,
    u.NumeroIdentificacion AS Cedula,
    p.Id AS ProfesorId,
    p.Nombres + ' ' + p.PrimerApellido + ' ' + ISNULL(p.SegundoApellido, '') AS NombreProfesor,
    p.Cedula AS CedulaProfesor
FROM AspNetUsers u
LEFT JOIN Profesores p ON u.NumeroIdentificacion = p.Cedula
WHERE p.Id IS NOT NULL
ORDER BY p.Id;
GO

-- 4. Verificar boletas de conducta y su profesor guía asignado
PRINT '';
PRINT '========================================';
PRINT '4. BOLETAS DE CONDUCTA';
PRINT '========================================';
SELECT 
    bc.IdBoleta,
    bc.IdEstudiante,
    e.Nombre + ' ' + e.Apellidos AS NombreEstudiante,
    bc.ProfesorGuiaId,
    u.Nombre + ' ' + u.Apellidos AS ProfesorGuia,
    u.Email AS EmailProfesorGuia,
    bc.NotificacionEnviada,
    bc.FechaNotificacion,
    bc.FechaEmision,
    bc.Estado
FROM BoletasConducta bc
INNER JOIN Estudiantes e ON bc.IdEstudiante = e.IdEstudiante
LEFT JOIN AspNetUsers u ON bc.ProfesorGuiaId = u.Id
ORDER BY bc.IdBoleta DESC;
GO

-- 5. Diagnóstico completo para una boleta específica (cambiar el ID)
PRINT '';
PRINT '========================================';
PRINT '5. DIAGNÓSTICO COMPLETO BOLETA #1';
PRINT '========================================';
DECLARE @IdBoleta INT = 1;

SELECT 
    'BOLETA' AS Tipo,
    bc.IdBoleta,
    bc.IdEstudiante,
    e.Nombre + ' ' + e.Apellidos AS NombreEstudiante,
    bc.ProfesorGuiaId,
    bc.NotificacionEnviada
FROM BoletasConducta bc
INNER JOIN Estudiantes e ON bc.IdEstudiante = e.IdEstudiante
WHERE bc.IdBoleta = @IdBoleta;

SELECT 
    'GRUPO DEL ESTUDIANTE' AS Tipo,
    eg.GrupoId,
    g.Nombre AS NombreGrupo,
    eg.EsGrupoPrincipal,
    eg.Estado
FROM BoletasConducta bc
INNER JOIN EstudianteGrupos eg ON bc.IdEstudiante = eg.EstudianteId
INNER JOIN GruposEstudiantes g ON eg.GrupoId = g.Id
WHERE bc.IdBoleta = @IdBoleta
    AND eg.Estado = 1 
    AND eg.EsGrupoPrincipal = 1;

SELECT 
    'PROFESOR GUÍA DEL GRUPO' AS Tipo,
    pg.Id,
    pg.ProfesorId,
    p.Nombres + ' ' + p.PrimerApellido + ' ' + ISNULL(p.SegundoApellido, '') AS NombreProfesor,
    p.Cedula AS CedulaProfesor,
    pg.Estado AS EstadoAsignacion
FROM BoletasConducta bc
INNER JOIN EstudianteGrupos eg ON bc.IdEstudiante = eg.EstudianteId 
    AND eg.Estado = 1 
    AND eg.EsGrupoPrincipal = 1
INNER JOIN ProfesorGuia pg ON eg.GrupoId = pg.GrupoId
INNER JOIN Profesores p ON pg.ProfesorId = p.Id
WHERE bc.IdBoleta = @IdBoleta
    AND pg.Estado = 1;

SELECT 
    'USUARIO DEL PROFESOR' AS Tipo,
    u.Id AS UserId,
    u.Nombre + ' ' + u.Apellidos AS NombreCompleto,
    u.Email,
    u.NumeroIdentificacion AS CedulaUsuario,
    p.Cedula AS CedulaProfesor
FROM BoletasConducta bc
INNER JOIN EstudianteGrupos eg ON bc.IdEstudiante = eg.EstudianteId 
    AND eg.Estado = 1 
    AND eg.EsGrupoPrincipal = 1
INNER JOIN ProfesorGuia pg ON eg.GrupoId = pg.GrupoId AND pg.Estado = 1
INNER JOIN Profesores p ON pg.ProfesorId = p.Id
INNER JOIN AspNetUsers u ON p.Cedula = u.NumeroIdentificacion
WHERE bc.IdBoleta = @IdBoleta;
GO

PRINT '';
PRINT '========================================';
PRINT 'VERIFICACIÓN COMPLETADA';
PRINT '========================================';
