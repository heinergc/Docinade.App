-- Insertar Datos Semilla para Conducta
USE RubricasDb;
GO

SET NOCOUNT ON;
SET QUOTED_IDENTIFIER ON;
GO

DECLARE @PeriodoId INT = 1;
DECLARE @DocenteId NVARCHAR(450);
DECLARE @GrupoId INT = 60;
DECLARE @ProfesorGuiaId INT = 1;

-- Obtener ID de docente
SELECT @DocenteId = Id FROM AspNetUsers WHERE Email = 'admin@rubricas.edu';

-- Obtener IDs de tipos de falta
-- IDs fijos de tipos de falta
DECLARE @MuyLeveId INT = 1;
DECLARE @LeveId INT = 2;
DECLARE @GraveId INT = 3;
DECLARE @MuyGraveId INT = 4;
DECLARE @GravisimaId INT = 5;

PRINT 'Periodo ID: ' + CAST(@PeriodoId AS NVARCHAR);
PRINT 'Docente ID: ' + ISNULL(@DocenteId, 'NO ENCONTRADO');

-- Obtener IDs de estudiantes
DECLARE @EstudianteRiesgo INT = (SELECT IdEstudiante FROM Estudiantes WHERE NumeroId = 'TEST-001');
DECLARE @EstudianteAplazado INT = (SELECT IdEstudiante FROM Estudiantes WHERE NumeroId = 'TEST-002');
DECLARE @EstudiantePrograma INT = (SELECT IdEstudiante FROM Estudiantes WHERE NumeroId = 'TEST-003');
DECLARE @EstudianteDecision INT = (SELECT IdEstudiante FROM Estudiantes WHERE NumeroId = 'TEST-004');
DECLARE @EstudianteBueno INT = (SELECT IdEstudiante FROM Estudiantes WHERE NumeroId = 'TEST-005');
DECLARE @EstudianteMultiple INT = (SELECT IdEstudiante FROM Estudiantes WHERE NumeroId = 'TEST-006');

PRINT 'Estudiante Riesgo ID: ' + CAST(@EstudianteRiesgo AS NVARCHAR);

-- ASIGNAR ESTUDIANTES A GRUPOS
IF @EstudianteRiesgo IS NOT NULL AND NOT EXISTS (SELECT 1 FROM EstudianteGrupos WHERE EstudianteId = @EstudianteRiesgo AND GrupoId = @GrupoId)
BEGIN
    INSERT INTO EstudianteGrupos (EstudianteId, GrupoId, Estado, FechaAsignacion, EsGrupoPrincipal, AsignadoPorId)
    VALUES 
        (@EstudianteRiesgo, @GrupoId, 'Activo', GETDATE(), 1, @DocenteId),
        (@EstudianteAplazado, @GrupoId, 'Activo', GETDATE(), 1, @DocenteId),
        (@EstudiantePrograma, @GrupoId, 'Activo', GETDATE(), 1, @DocenteId),
        (@EstudianteDecision, @GrupoId, 'Activo', GETDATE(), 1, @DocenteId),
        (@EstudianteBueno, @GrupoId, 'Activo', GETDATE(), 1, @DocenteId),
        (@EstudianteMultiple, @GrupoId, 'Activo', GETDATE(), 1, @DocenteId);
    PRINT 'Estudiantes asignados a grupos';
END

-- LIMPIAR BOLETAS ANTERIORES
DELETE FROM BoletasConducta WHERE IdEstudiante IN (@EstudianteRiesgo, @EstudianteAplazado, @EstudiantePrograma, @EstudianteDecision, @EstudianteBueno, @EstudianteMultiple);
PRINT 'Boletas anteriores eliminadas';

-- ESTUDIANTE 1: EN RIESGO (40 puntos de rebajo = nota 60)
INSERT INTO BoletasConducta (IdEstudiante, IdPeriodo, IdTipoFalta, RebajoAplicado, Descripcion, FechaEmision, Estado, DocenteEmisorId, NotificacionEnviada, ProfesorGuiaId)
VALUES 
(@EstudianteRiesgo, @PeriodoId, @GraveId, 15, 'Uso de celular en clase durante evaluacion', DATEADD(DAY, -30, GETDATE()), 'Activa', @DocenteId, 1, @ProfesorGuiaId),
(@EstudianteRiesgo, @PeriodoId, @LeveId, 10, 'Llegada tardia reiterada (3 veces en una semana)', DATEADD(DAY, -20, GETDATE()), 'Activa', @DocenteId, 1, @ProfesorGuiaId),
(@EstudianteRiesgo, @PeriodoId, @GraveId, 15, 'Falta de respeto verbal a companero', DATEADD(DAY, -10, GETDATE()), 'Activa', @DocenteId, 1, @ProfesorGuiaId);
PRINT 'Boletas para estudiante EN RIESGO creadas';

-- ESTUDIANTE 2: APLAZADO SIN PROGRAMA (50 puntos de rebajo = nota 50)
INSERT INTO BoletasConducta (IdEstudiante, IdPeriodo, IdTipoFalta, RebajoAplicado, Descripcion, FechaEmision, Estado, DocenteEmisorId, NotificacionEnviada, ProfesorGuiaId)
VALUES 
(@EstudianteAplazado, @PeriodoId, @GravisimaId, 25, 'Agresion fisica a companero en el recreo', DATEADD(DAY, -25, GETDATE()), 'Activa', @DocenteId, 1, @ProfesorGuiaId),
(@EstudianteAplazado, @PeriodoId, @GraveId, 15, 'Sustraccion de material didactico', DATEADD(DAY, -15, GETDATE()), 'Activa', @DocenteId, 1, @ProfesorGuiaId),
(@EstudianteAplazado, @PeriodoId, @LeveId, 10, 'No traer material requerido repetidamente', DATEADD(DAY, -5, GETDATE()), 'Activa', @DocenteId, 0, @ProfesorGuiaId);
PRINT 'Boletas para estudiante APLAZADO sin programa creadas';

-- ESTUDIANTE 3: APLAZADO CON PROGRAMA (45 puntos de rebajo = nota 55)
INSERT INTO BoletasConducta (IdEstudiante, IdPeriodo, IdTipoFalta, RebajoAplicado, Descripcion, FechaEmision, Estado, DocenteEmisorId, NotificacionEnviada, ProfesorGuiaId)
VALUES 
(@EstudiantePrograma, @PeriodoId, @MuyGraveId, 20, 'Copia en examen final de matematicas', DATEADD(DAY, -40, GETDATE()), 'Activa', @DocenteId, 1, @ProfesorGuiaId),
(@EstudiantePrograma, @PeriodoId, @GraveId, 15, 'Falsificacion de firma en documento de permiso', DATEADD(DAY, -28, GETDATE()), 'Activa', @DocenteId, 1, @ProfesorGuiaId),
(@EstudiantePrograma, @PeriodoId, @LeveId, 10, 'Interrupciones constantes en clase', DATEADD(DAY, -12, GETDATE()), 'Activa', @DocenteId, 1, @ProfesorGuiaId);
PRINT 'Boletas para estudiante con PROGRAMA creadas';

-- ESTUDIANTE 4: APLAZADO CON DECISION (60 puntos de rebajo = nota 40, pero ajustado a 70)
INSERT INTO BoletasConducta (IdEstudiante, IdPeriodo, IdTipoFalta, RebajoAplicado, Descripcion, FechaEmision, Estado, DocenteEmisorId, NotificacionEnviada, ProfesorGuiaId)
VALUES 
(@EstudianteDecision, @PeriodoId, @GravisimaId, 25, 'Dano intencional a propiedad institucional', DATEADD(DAY, -50, GETDATE()), 'Activa', @DocenteId, 1, @ProfesorGuiaId),
(@EstudianteDecision, @PeriodoId, @MuyGraveId, 20, 'Acoso verbal sistematico a companero', DATEADD(DAY, -35, GETDATE()), 'Activa', @DocenteId, 1, @ProfesorGuiaId),
(@EstudianteDecision, @PeriodoId, @GraveId, 15, 'Salida no autorizada de la institucion', DATEADD(DAY, -18, GETDATE()), 'Activa', @DocenteId, 1, @ProfesorGuiaId);
PRINT 'Boletas para estudiante con DECISION PROFESIONAL creadas';

-- ESTUDIANTE 5: APROBADO (5 puntos de rebajo = nota 95)
INSERT INTO BoletasConducta (IdEstudiante, IdPeriodo, IdTipoFalta, RebajoAplicado, Descripcion, FechaEmision, Estado, DocenteEmisorId, NotificacionEnviada, ProfesorGuiaId)
VALUES 
(@EstudianteBueno, @PeriodoId, @MuyLeveId, 5, 'Olvido menor: no traer cuaderno en una ocasion', DATEADD(DAY, -45, GETDATE()), 'Activa', @DocenteId, 1, @ProfesorGuiaId);
PRINT 'Boletas para estudiante APROBADO creadas';

-- ESTUDIANTE 6: MULTIPLES INCIDENTES (38 puntos de rebajo = nota 62 - Riesgo)
INSERT INTO BoletasConducta (IdEstudiante, IdPeriodo, IdTipoFalta, RebajoAplicado, Descripcion, FechaEmision, Estado, DocenteEmisorId, NotificacionEnviada, ProfesorGuiaId)
VALUES 
(@EstudianteMultiple, @PeriodoId, @LeveId, 8, 'Incumplimiento de tareas 4 veces consecutivas', DATEADD(DAY, -42, GETDATE()), 'Activa', @DocenteId, 1, @ProfesorGuiaId),
(@EstudianteMultiple, @PeriodoId, @GraveId, 15, 'Lenguaje inapropiado en clase', DATEADD(DAY, -30, GETDATE()), 'Activa', @DocenteId, 1, @ProfesorGuiaId),
(@EstudianteMultiple, @PeriodoId, @LeveId, 8, 'Uniforme incompleto repetidamente', DATEADD(DAY, -22, GETDATE()), 'Activa', @DocenteId, 0, @ProfesorGuiaId),
(@EstudianteMultiple, @PeriodoId, @MuyLeveId, 2, 'Masticar chicle en clase', DATEADD(DAY, -15, GETDATE()), 'Activa', @DocenteId, 1, @ProfesorGuiaId),
(@EstudianteMultiple, @PeriodoId, @MuyLeveId, 5, 'Conversaciones durante explicacion del profesor', DATEADD(DAY, -8, GETDATE()), 'Activa', @DocenteId, 0, @ProfesorGuiaId);
PRINT 'Boletas para estudiante con MULTIPLES INCIDENTES creadas';

-- LIMPIAR NOTAS ANTERIORES
DELETE FROM NotasConducta WHERE IdEstudiante IN (@EstudianteRiesgo, @EstudianteAplazado, @EstudiantePrograma, @EstudianteDecision, @EstudianteBueno, @EstudianteMultiple);
PRINT 'Notas anteriores eliminadas';

-- CREAR NOTAS DE CONDUCTA
INSERT INTO NotasConducta (IdEstudiante, IdPeriodo, NotaInicial, TotalRebajos, NotaFinal, Estado, FechaCalculo, RequiereProgramaAcciones, DecisionProfesionalAplicada)
VALUES 
(@EstudianteRiesgo, @PeriodoId, 100, 40, 60, 'Riesgo', GETDATE(), 0, 0),
(@EstudianteAplazado, @PeriodoId, 100, 50, 50, 'Aplazado', GETDATE(), 1, 0),
(@EstudiantePrograma, @PeriodoId, 100, 45, 55, 'Aplazado', GETDATE(), 1, 0),
(@EstudianteDecision, @PeriodoId, 100, 60, 40, 'Aplazado', GETDATE(), 0, 1),
(@EstudianteBueno, @PeriodoId, 100, 5, 95, 'Aprobado', GETDATE(), 0, 0),
(@EstudianteMultiple, @PeriodoId, 100, 38, 62, 'Riesgo', GETDATE(), 0, 0);
PRINT 'Notas de conducta creadas';

-- CREAR PROGRAMA DE ACCIONES
DECLARE @NotaConductaPrograma INT = (SELECT IdNotaConducta FROM NotasConducta WHERE IdEstudiante = @EstudiantePrograma AND IdPeriodo = @PeriodoId);

IF @NotaConductaPrograma IS NOT NULL AND NOT EXISTS (SELECT 1 FROM ProgramasAccionesInstitucional WHERE IdEstudiante = @EstudiantePrograma)
BEGIN
    INSERT INTO ProgramasAccionesInstitucional 
    (IdEstudiante, IdPeriodo, IdNotaConducta, TituloPrograma, Descripcion, ObjetivosEspecificos, ActividadesARealizar, ResponsableSupervisionId, FechaInicio, FechaFinPrevista, Estado, FechaCreacion, AprobarConducta)
    VALUES 
    (@EstudiantePrograma, @PeriodoId, @NotaConductaPrograma,
     'Programa de Mejoramiento Conductual - Ana Martinez',
     'Programa disenado para mejorar el comportamiento academico y etico del estudiante',
     '1. Reflexionar sobre consecuencias\n2. Desarrollar autorregulacion\n3. Fortalecer honestidad',
     '1. Sesiones con orientador\n2. Diario reflexivo\n3. Servicio comunitario\n4. Taller de valores',
     @DocenteId, GETDATE(), DATEADD(DAY, 60, GETDATE()), 'En Proceso', GETDATE(), 1);

    UPDATE NotasConducta SET IdProgramaAcciones = SCOPE_IDENTITY() WHERE IdNotaConducta = @NotaConductaPrograma;
    PRINT 'Programa de acciones creado para Ana Martinez';
END

-- CREAR DECISION PROFESIONAL
DECLARE @NotaConductaDecision INT = (SELECT IdNotaConducta FROM NotasConducta WHERE IdEstudiante = @EstudianteDecision AND IdPeriodo = @PeriodoId);

IF @NotaConductaDecision IS NOT NULL AND NOT EXISTS (SELECT 1 FROM DecisionesProfesionalesConducta WHERE IdEstudiante = @EstudianteDecision)
BEGIN
    INSERT INTO DecisionesProfesionalesConducta
    (IdEstudiante, IdPeriodo, IdNotaConducta, NumeroActa, FechaActa, FechaDecision, DecisionTomada, JustificacionPedagogica, ObservacionesComite, ConsideracionesAdicionales, NotaAjustada, MiembrosComitePresentes, TomaDecisionPorId, RegistradoEnExpediente, FechaRegistroExpediente)
    VALUES
    (@EstudianteDecision, @PeriodoId, @NotaConductaDecision,
     'ACTA-2025-001', DATEADD(DAY, -3, GETDATE()), DATEADD(DAY, -3, GETDATE()),
     'Ajustar nota a 70 pts con compromiso',
     'El comite considera arrepentimiento genuino y compromiso de cambio. Circunstancias familiares difíciles.',
     'Comite reunido - Presentes: Director, Orientador, Profesor Guia - Decision unanime',
     'Estudiante se compromete a terapia - Padres comprometen supervision - Seguimiento mensual',
     70, 'Director: Jose Fernandez\nOrientadora: Maria Lopez\nProfesor Guia: Carlos Rodriguez',
     @DocenteId, 1, DATEADD(DAY, -2, GETDATE()));

    UPDATE NotasConducta SET IdDecisionProfesional = SCOPE_IDENTITY(), NotaFinal = 70, Estado = 'Aprobado' WHERE IdNotaConducta = @NotaConductaDecision;
    PRINT 'Decision profesional creada para Luis Perez';
END

-- RESUMEN
PRINT '';
PRINT '========================================';
PRINT 'RESUMEN DE DATOS INSERTADOS';
PRINT '========================================';

SELECT 
    e.NumeroId AS 'ID',
    e.Nombre + ' ' + e.Apellidos AS 'Estudiante',
    n.NotaFinal AS 'Nota',
    n.Estado AS 'Estado',
    n.TotalRebajos AS 'Rebajos',
    COUNT(b.IdBoleta) AS 'Boletas',
    CASE WHEN n.RequiereProgramaAcciones = 1 THEN 'Si' ELSE 'No' END AS 'Req.Programa',
    CASE WHEN n.DecisionProfesionalAplicada = 1 THEN 'Si' ELSE 'No' END AS 'Con Decision'
FROM Estudiantes e
INNER JOIN NotasConducta n ON e.IdEstudiante = n.IdEstudiante
LEFT JOIN BoletasConducta b ON e.IdEstudiante = b.IdEstudiante AND b.Estado = 'Activa'
WHERE e.NumeroId LIKE 'TEST-%'
GROUP BY e.NumeroId, e.Nombre, e.Apellidos, n.NotaFinal, n.Estado, n.TotalRebajos, n.RequiereProgramaAcciones, n.DecisionProfesionalAplicada
ORDER BY n.NotaFinal;

PRINT '';
PRINT 'DATOS SEMILLA INSERTADOS EXITOSAMENTE';
PRINT '';

SET NOCOUNT OFF;
