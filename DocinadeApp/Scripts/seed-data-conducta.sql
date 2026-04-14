-- =============================================
-- Script de Datos Semilla para Sistema de Conducta
-- Fecha: 2025-11-10
-- Propósito: Insertar datos de prueba para visualizar
--            estudiantes en riesgo, aplazados, con programas
--            de acciones y decisiones profesionales
-- =============================================

USE RubricasDb;
GO

SET NOCOUNT ON;

PRINT '========================================';
PRINT 'Iniciando inserción de datos semilla...';
PRINT '========================================';

-- Variables
DECLARE @PeriodoId INT = 1;
DECLARE @DocenteId NVARCHAR(450) = (SELECT TOP 1 Id FROM AspNetUsers WHERE Email = 'admin@rubricas.edu');
DECLARE @GrupoId INT = 60;
DECLARE @ProfesorGuiaId INT = 1;

PRINT 'Periodo Académico ID: ' + CAST(@PeriodoId AS NVARCHAR);
PRINT 'Docente Emisor ID: ' + ISNULL(@DocenteId, 'NO ENCONTRADO');
PRINT 'Grupo ID: ' + CAST(@GrupoId AS NVARCHAR);

-- =============================================
-- 1. CREAR ESTUDIANTES DE PRUEBA
-- =============================================
PRINT '';
PRINT '1. Creando estudiantes de prueba...';

-- Verificar si ya existen estudiantes de prueba
IF NOT EXISTS (SELECT 1 FROM Estudiantes WHERE NumeroId LIKE 'TEST-%')
BEGIN
    -- Estudiante 1: En RIESGO (nota entre 60-64)
    INSERT INTO Estudiantes (Nombre, Apellidos, NumeroId, DireccionCorreo, Estado, Grupos, Institucion, Año, PeriodoAcademicoId)
    VALUES 
    ('María', 'González Riesgo', 'TEST-001', 'maria.riesgo@test.edu', 1, 'Décimo-A', 'IES TEST', '2025', @PeriodoId);
    PRINT '  ✓ Estudiante en RIESGO creado: María González (TEST-001)';

    -- Estudiante 2: APLAZADO sin programa (nota < 60)
    INSERT INTO Estudiantes (Nombre, Apellidos, NumeroId, DireccionCorreo, Estado, Grupos, Institucion, Año, PeriodoAcademicoId)
    VALUES 
    ('Carlos', 'Ramírez Aplazado', 'TEST-002', 'carlos.aplazado@test.edu', 1, 'Décimo-A', 'IES TEST', '2025', @PeriodoId);
    PRINT '  ✓ Estudiante APLAZADO creado: Carlos Ramírez (TEST-002)';

    -- Estudiante 3: APLAZADO con programa de acciones
    INSERT INTO Estudiantes (Nombre, Apellidos, NumeroId, DireccionCorreo, Estado, Grupos, Institucion, Año, PeriodoAcademicoId)
    VALUES 
    ('Ana', 'Martínez Programa', 'TEST-003', 'ana.programa@test.edu', 1, 'Décimo-A', 'IES TEST', '2025', @PeriodoId);
    PRINT '  ✓ Estudiante con PROGRAMA creado: Ana Martínez (TEST-003)';

    -- Estudiante 4: APLAZADO con decisión profesional
    INSERT INTO Estudiantes (Nombre, Apellidos, NumeroId, DireccionCorreo, Estado, Grupos, Institucion, Año, PeriodoAcademicoId)
    VALUES 
    ('Luis', 'Pérez Decisión', 'TEST-004', 'luis.decision@test.edu', 1, 'Décimo-A', 'IES TEST', '2025', @PeriodoId);
    PRINT '  ✓ Estudiante con DECISIÓN creado: Luis Pérez (TEST-004)';

    -- Estudiante 5: APROBADO (buena conducta)
    INSERT INTO Estudiantes (Nombre, Apellidos, NumeroId, DireccionCorreo, Estado, Grupos, Institucion, Año, PeriodoAcademicoId)
    VALUES 
    ('Elena', 'Torres Buena', 'TEST-005', 'elena.buena@test.edu', 1, 'Décimo-A', 'IES TEST', '2025', @PeriodoId);
    PRINT '  ✓ Estudiante APROBADO creado: Elena Torres (TEST-005)';

    -- Estudiante 6: Múltiples incidentes graves
    INSERT INTO Estudiantes (Nombre, Apellidos, NumeroId, DireccionCorreo, Estado, Grupos, Institucion, Año, PeriodoAcademicoId)
    VALUES 
    ('Pedro', 'Sánchez Múltiple', 'TEST-006', 'pedro.multiple@test.edu', 1, 'Décimo-A', 'IES TEST', '2025', @PeriodoId);
    PRINT '  ✓ Estudiante con incidentes MÚLTIPLES creado: Pedro Sánchez (TEST-006)';
END
ELSE
BEGIN
    PRINT '  ⚠ Los estudiantes de prueba ya existen, omitiendo creación...';
END

-- Obtener IDs de estudiantes
DECLARE @EstudianteRiesgo INT = (SELECT IdEstudiante FROM Estudiantes WHERE NumeroId = 'TEST-001');
DECLARE @EstudianteAplazado INT = (SELECT IdEstudiante FROM Estudiantes WHERE NumeroId = 'TEST-002');
DECLARE @EstudiantePrograma INT = (SELECT IdEstudiante FROM Estudiantes WHERE NumeroId = 'TEST-003');
DECLARE @EstudianteDecision INT = (SELECT IdEstudiante FROM Estudiantes WHERE NumeroId = 'TEST-004');
DECLARE @EstudianteBueno INT = (SELECT IdEstudiante FROM Estudiantes WHERE NumeroId = 'TEST-005');
DECLARE @EstudianteMultiple INT = (SELECT IdEstudiante FROM Estudiantes WHERE NumeroId = 'TEST-006');

-- =============================================
-- 2. ASIGNAR ESTUDIANTES A GRUPOS
-- =============================================
PRINT '';
PRINT '2. Asignando estudiantes a grupos...';

IF NOT EXISTS (SELECT 1 FROM EstudianteGrupos WHERE EstudianteId = @EstudianteRiesgo)
BEGIN
    INSERT INTO EstudianteGrupos (EstudianteId, GrupoId, Estado, FechaAsignacion, EsGrupoPrincipal, AsignadoPorId)
    SELECT IdEstudiante, @GrupoId, 'Activo', GETDATE(), 1, @DocenteId
    FROM Estudiantes 
    WHERE NumeroId LIKE 'TEST-%' 
    AND IdEstudiante NOT IN (SELECT EstudianteId FROM EstudianteGrupos);
    
    PRINT '  ✓ ' + CAST(@@ROWCOUNT AS NVARCHAR) + ' estudiantes asignados al grupo';
END

-- =============================================
-- 3. CREAR TIPOS DE FALTA (SI NO EXISTEN)
-- =============================================
PRINT '';
PRINT '3. Verificando tipos de falta...';

DECLARE @MuyLeveId INT = (SELECT IdTipoFalta FROM TiposFalta WHERE Nombre = 'Muy leve');
DECLARE @LeveId INT = (SELECT IdTipoFalta FROM TiposFalta WHERE Nombre = 'Leve');
DECLARE @GraveId INT = (SELECT IdTipoFalta FROM TiposFalta WHERE Nombre = 'Grave');
DECLARE @MuyGraveId INT = (SELECT IdTipoFalta FROM TiposFalta WHERE Nombre = 'Muy Grave');
DECLARE @GravisimaId INT = (SELECT IdTipoFalta FROM TiposFalta WHERE Nombre = 'Gravísima');

PRINT '  ✓ Tipos de falta verificados';

-- =============================================
-- 4. CREAR BOLETAS DE CONDUCTA
-- =============================================
PRINT '';
PRINT '4. Creando boletas de conducta...';

-- Limpiar boletas anteriores de estudiantes de prueba
DELETE FROM BoletasConducta WHERE IdEstudiante IN (@EstudianteRiesgo, @EstudianteAplazado, @EstudiantePrograma, @EstudianteDecision, @EstudianteBueno, @EstudianteMultiple);

-- ESTUDIANTE 1: EN RIESGO (40 puntos de rebajo = nota 60)
INSERT INTO BoletasConducta (IdEstudiante, IdPeriodo, IdTipoFalta, RebajoAplicado, Descripcion, FechaEmision, Estado, DocenteEmisorId, NotificacionEnviada, ProfesorGuiaId)
VALUES 
(@EstudianteRiesgo, @PeriodoId, @GraveId, 15, 'Uso de celular en clase durante evaluación', DATEADD(DAY, -30, GETDATE()), 'Activa', @DocenteId, 1, @ProfesorGuiaId),
(@EstudianteRiesgo, @PeriodoId, @LeveId, 10, 'Llegada tardía reiterada (3 veces en una semana)', DATEADD(DAY, -20, GETDATE()), 'Activa', @DocenteId, 1, @ProfesorGuiaId),
(@EstudianteRiesgo, @PeriodoId, @GraveId, 15, 'Falta de respeto verbal a compañero', DATEADD(DAY, -10, GETDATE()), 'Activa', @DocenteId, 1, @ProfesorGuiaId);

PRINT '  ✓ Boletas para estudiante EN RIESGO (40 pts rebajo)';

-- ESTUDIANTE 2: APLAZADO SIN PROGRAMA (50 puntos de rebajo = nota 50)
INSERT INTO BoletasConducta (IdEstudiante, IdPeriodo, IdTipoFalta, RebajoAplicado, Descripcion, FechaEmision, Estado, DocenteEmisorId, NotificacionEnviada, ProfesorGuiaId)
VALUES 
(@EstudianteAplazado, @PeriodoId, @GravisimaId, 25, 'Agresión física a compañero en el recreo', DATEADD(DAY, -25, GETDATE()), 'Activa', @DocenteId, 1, @ProfesorGuiaId),
(@EstudianteAplazado, @PeriodoId, @GraveId, 15, 'Sustracción de material didáctico', DATEADD(DAY, -15, GETDATE()), 'Activa', @DocenteId, 1, @ProfesorGuiaId),
(@EstudianteAplazado, @PeriodoId, @LeveId, 10, 'No traer material requerido repetidamente', DATEADD(DAY, -5, GETDATE()), 'Activa', @DocenteId, 0, @ProfesorGuiaId);

PRINT '  ✓ Boletas para estudiante APLAZADO sin programa (50 pts rebajo)';

-- ESTUDIANTE 3: APLAZADO CON PROGRAMA (45 puntos de rebajo = nota 55)
INSERT INTO BoletasConducta (IdEstudiante, IdPeriodo, IdTipoFalta, RebajoAplicado, Descripcion, FechaEmision, Estado, DocenteEmisorId, NotificacionEnviada, ProfesorGuiaId)
VALUES 
(@EstudiantePrograma, @PeriodoId, @MuyGraveId, 20, 'Copia en examen final de matemáticas', DATEADD(DAY, -40, GETDATE()), 'Activa', @DocenteId, 1, @ProfesorGuiaId),
(@EstudiantePrograma, @PeriodoId, @GraveId, 15, 'Falsificación de firma en documento de permiso', DATEADD(DAY, -28, GETDATE()), 'Activa', @DocenteId, 1, @ProfesorGuiaId),
(@EstudiantePrograma, @PeriodoId, @LeveId, 10, 'Interrupciones constantes en clase', DATEADD(DAY, -12, GETDATE()), 'Activa', @DocenteId, 1, @ProfesorGuiaId);

PRINT '  ✓ Boletas para estudiante con PROGRAMA (45 pts rebajo)';

-- ESTUDIANTE 4: APLAZADO CON DECISIÓN (60 puntos de rebajo = nota 40, pero ajustado a 70)
INSERT INTO BoletasConducta (IdEstudiante, IdPeriodo, IdTipoFalta, RebajoAplicado, Descripcion, FechaEmision, Estado, DocenteEmisorId, NotificacionEnviada, ProfesorGuiaId)
VALUES 
(@EstudianteDecision, @PeriodoId, @GravisimaId, 25, 'Daño intencional a propiedad institucional', DATEADD(DAY, -50, GETDATE()), 'Activa', @DocenteId, 1, @ProfesorGuiaId),
(@EstudianteDecision, @PeriodoId, @MuyGraveId, 20, 'Acoso verbal sistemático a compañero', DATEADD(DAY, -35, GETDATE()), 'Activa', @DocenteId, 1, @ProfesorGuiaId),
(@EstudianteDecision, @PeriodoId, @GraveId, 15, 'Salida no autorizada de la institución', DATEADD(DAY, -18, GETDATE()), 'Activa', @DocenteId, 1, @ProfesorGuiaId);

PRINT '  ✓ Boletas para estudiante con DECISIÓN PROFESIONAL (60 pts rebajo)';

-- ESTUDIANTE 5: APROBADO (5 puntos de rebajo = nota 95)
INSERT INTO BoletasConducta (IdEstudiante, IdPeriodo, IdTipoFalta, RebajoAplicado, Descripcion, FechaEmision, Estado, DocenteEmisorId, NotificacionEnviada, ProfesorGuiaId)
VALUES 
(@EstudianteBueno, @PeriodoId, @MuyLeveId, 5, 'Olvido menor: no traer cuaderno en una ocasión', DATEADD(DAY, -45, GETDATE()), 'Activa', @DocenteId, 1, @ProfesorGuiaId);

PRINT '  ✓ Boletas para estudiante APROBADO (5 pts rebajo)';

-- ESTUDIANTE 6: MÚLTIPLES INCIDENTES (38 puntos de rebajo = nota 62 - Riesgo)
INSERT INTO BoletasConducta (IdEstudiante, IdPeriodo, IdTipoFalta, RebajoAplicado, Descripcion, FechaEmision, Estado, DocenteEmisorId, NotificacionEnviada, ProfesorGuiaId)
VALUES 
(@EstudianteMultiple, @PeriodoId, @LeveId, 8, 'Incumplimiento de tareas 4 veces consecutivas', DATEADD(DAY, -42, GETDATE()), 'Activa', @DocenteId, 1, @ProfesorGuiaId),
(@EstudianteMultiple, @PeriodoId, @GraveId, 15, 'Lenguaje inapropiado en clase', DATEADD(DAY, -30, GETDATE()), 'Activa', @DocenteId, 1, @ProfesorGuiaId),
(@EstudianteMultiple, @PeriodoId, @LeveId, 8, 'Uniforme incompleto repetidamente', DATEADD(DAY, -22, GETDATE()), 'Activa', @DocenteId, 0, @ProfesorGuiaId),
(@EstudianteMultiple, @PeriodoId, @MuyLeveId, 2, 'Masticar chicle en clase', DATEADD(DAY, -15, GETDATE()), 'Activa', @DocenteId, 1, @ProfesorGuiaId),
(@EstudianteMultiple, @PeriodoId, @MuyLeveId, 5, 'Conversaciones durante explicación del profesor', DATEADD(DAY, -8, GETDATE()), 'Activa', @DocenteId, 0, @ProfesorGuiaId);

PRINT '  ✓ Boletas para estudiante con MÚLTIPLES INCIDENTES (38 pts rebajo)';

-- También agregar una boleta ANULADA para ejemplo
INSERT INTO BoletasConducta (IdEstudiante, IdPeriodo, IdTipoFalta, RebajoAplicado, Descripcion, FechaEmision, Estado, DocenteEmisorId, NotificacionEnviada, ProfesorGuiaId, MotivoAnulacion, FechaAnulacion, AnuladaPorId)
VALUES 
(@EstudianteMultiple, @PeriodoId, @GraveId, 15, 'Incidente mal interpretado - ANULADO', DATEADD(DAY, -20, GETDATE()), 'Anulada', @DocenteId, 1, @ProfesorGuiaId, 'Se comprobó que el estudiante no estuvo involucrado', DATEADD(DAY, -18, GETDATE()), @DocenteId);

PRINT '  ✓ Boleta ANULADA agregada como ejemplo';

-- =============================================
-- 5. CREAR NOTAS DE CONDUCTA
-- =============================================
PRINT '';
PRINT '5. Creando notas de conducta...';

-- Limpiar notas anteriores
DELETE FROM NotasConducta WHERE IdEstudiante IN (@EstudianteRiesgo, @EstudianteAplazado, @EstudiantePrograma, @EstudianteDecision, @EstudianteBueno, @EstudianteMultiple);

-- Estudiante 1: RIESGO
INSERT INTO NotasConducta (IdEstudiante, IdPeriodo, NotaInicial, TotalRebajos, NotaFinal, Estado, FechaCalculo, RequiereProgramaAcciones, DecisionProfesionalAplicada)
VALUES (@EstudianteRiesgo, @PeriodoId, 100, 40, 60, 'Riesgo', GETDATE(), 0, 0);

-- Estudiante 2: APLAZADO sin programa
INSERT INTO NotasConducta (IdEstudiante, IdPeriodo, NotaInicial, TotalRebajos, NotaFinal, Estado, FechaCalculo, RequiereProgramaAcciones, DecisionProfesionalAplicada)
VALUES (@EstudianteAplazado, @PeriodoId, 100, 50, 50, 'Aplazado', GETDATE(), 1, 0);

-- Estudiante 3: APLAZADO con programa (se creará el programa después)
INSERT INTO NotasConducta (IdEstudiante, IdPeriodo, NotaInicial, TotalRebajos, NotaFinal, Estado, FechaCalculo, RequiereProgramaAcciones, DecisionProfesionalAplicada)
VALUES (@EstudiantePrograma, @PeriodoId, 100, 45, 55, 'Aplazado', GETDATE(), 1, 0);

-- Estudiante 4: APLAZADO con decisión profesional (se creará después)
INSERT INTO NotasConducta (IdEstudiante, IdPeriodo, NotaInicial, TotalRebajos, NotaFinal, Estado, FechaCalculo, RequiereProgramaAcciones, DecisionProfesionalAplicada)
VALUES (@EstudianteDecision, @PeriodoId, 100, 60, 40, 'Aplazado', GETDATE(), 0, 1);

-- Estudiante 5: APROBADO
INSERT INTO NotasConducta (IdEstudiante, IdPeriodo, NotaInicial, TotalRebajos, NotaFinal, Estado, FechaCalculo, RequiereProgramaAcciones, DecisionProfesionalAplicada)
VALUES (@EstudianteBueno, @PeriodoId, 100, 5, 95, 'Aprobado', GETDATE(), 0, 0);

-- Estudiante 6: RIESGO
INSERT INTO NotasConducta (IdEstudiante, IdPeriodo, NotaInicial, TotalRebajos, NotaFinal, Estado, FechaCalculo, RequiereProgramaAcciones, DecisionProfesionalAplicada)
VALUES (@EstudianteMultiple, @PeriodoId, 100, 38, 62, 'Riesgo', GETDATE(), 0, 0);

PRINT '  ✓ ' + CAST(@@ROWCOUNT AS NVARCHAR) + ' notas de conducta creadas';

-- =============================================
-- 6. CREAR PROGRAMA DE ACCIONES INSTITUCIONAL
-- =============================================
PRINT '';
PRINT '6. Creando programa de acciones...';

DECLARE @NotaConductaPrograma INT = (SELECT IdNotaConducta FROM NotasConducta WHERE IdEstudiante = @EstudiantePrograma AND IdPeriodo = @PeriodoId);

IF NOT EXISTS (SELECT 1 FROM ProgramasAccionesInstitucional WHERE IdEstudiante = @EstudiantePrograma)
BEGIN
    INSERT INTO ProgramasAccionesInstitucional 
    (IdEstudiante, IdPeriodo, IdNotaConducta, TituloPrograma, Descripcion, ObjetivosEspecificos, ActividadesARealizar, 
     ResponsableSupervisionId, FechaInicio, FechaFinPrevista, Estado, FechaCreacion)
    VALUES 
    (@EstudiantePrograma, @PeriodoId, @NotaConductaPrograma,
     'Programa de Mejoramiento Conductual - Ana Martínez',
     'Programa diseñado para mejorar el comportamiento académico y ético del estudiante mediante actividades de reflexión y compromiso.',
     '1. Reflexionar sobre las consecuencias de las acciones\n2. Desarrollar habilidades de autorregulación\n3. Fortalecer valores de honestidad académica\n4. Mejorar la convivencia escolar',
     '1. Sesiones semanales con orientador (8 semanas)\n2. Elaboración de diario reflexivo\n3. Servicio comunitario: 20 horas\n4. Taller de valores y ética (4 sesiones)\n5. Tutorías académicas en matemáticas\n6. Reuniones quincenales con profesor guía',
     @DocenteId,
     GETDATE(),
     DATEADD(DAY, 60, GETDATE()),
     'En Proceso',
     GETDATE());

    -- Actualizar la nota de conducta con el ID del programa
    UPDATE NotasConducta 
    SET IdProgramaAcciones = SCOPE_IDENTITY()
    WHERE IdNotaConducta = @NotaConductaPrograma;

    PRINT '  ✓ Programa de acciones creado para Ana Martínez';
END

-- =============================================
-- 7. CREAR DECISIÓN PROFESIONAL
-- =============================================
PRINT '';
PRINT '7. Creando decisión profesional...';

DECLARE @NotaConductaDecision INT = (SELECT IdNotaConducta FROM NotasConducta WHERE IdEstudiante = @EstudianteDecision AND IdPeriodo = @PeriodoId);

IF NOT EXISTS (SELECT 1 FROM DecisionesProfesionalesConducta WHERE IdEstudiante = @EstudianteDecision)
BEGIN
    INSERT INTO DecisionesProfesionalesConducta
    (IdEstudiante, IdPeriodo, IdNotaConducta, NumeroActa, FechaActa, FechaDecision, 
     DecisionTomada, JustificacionPedagogica, ObservacionesComite, ConsideracionesAdicionales,
     NotaAjustada, MiembrosComitePresentes, TomaDecisionPorId, RegistradoEnExpediente, FechaRegistroExpediente)
    VALUES
    (@EstudianteDecision, @PeriodoId, @NotaConductaDecision,
     'ACTA-2025-001',
     DATEADD(DAY, -3, GETDATE()),
     DATEADD(DAY, -3, GETDATE()),
     'Ajustar nota de conducta a 70 puntos con compromiso de mejora',
     'El comité considera que el estudiante ha mostrado arrepentimiento genuino y compromiso de cambio. Las circunstancias familiares difíciles (divorcio de padres, situación económica) contribuyeron a los incidentes. Se evidencia potencial académico y disposición para mejorar.',
     'Comité reunido el ' + CONVERT(VARCHAR, DATEADD(DAY, -3, GETDATE()), 103) + '\nPresentes: Director, Orientador, Profesor Guía, Representante Estudiantil\nSe analizó historial completo del estudiante\nSe acordó por unanimidad aplicar medida pedagógica',
     '- Estudiante se compromete a asistir a terapia psicológica\n- Padres se comprometen a mayor supervisión\n- Seguimiento mensual por orientación\n- Cualquier nuevo incidente grave revierte la decisión',
     70,
     'Director: José Fernández\nOrientadora: María López\nProfesor Guía: Carlos Rodríguez\nRepresentante Estudiantil: Laura Gómez',
     @DocenteId,
     1,
     DATEADD(DAY, -2, GETDATE()));

    -- Actualizar la nota de conducta con el ID de la decisión y la nota ajustada
    UPDATE NotasConducta 
    SET IdDecisionProfesional = SCOPE_IDENTITY(),
        NotaFinal = 70,
        Estado = 'Aprobado'
    WHERE IdNotaConducta = @NotaConductaDecision;

    PRINT '  ✓ Decisión profesional creada para Luis Pérez (nota ajustada: 40 → 70)';
END

-- =============================================
-- 8. RESUMEN DE DATOS CREADOS
-- =============================================
PRINT '';
PRINT '========================================';
PRINT 'RESUMEN DE DATOS SEMILLA CREADOS';
PRINT '========================================';
PRINT '';

-- Contar registros creados
DECLARE @TotalEstudiantes INT = (SELECT COUNT(*) FROM Estudiantes WHERE NumeroId LIKE 'TEST-%');
DECLARE @TotalBoletas INT = (SELECT COUNT(*) FROM BoletasConducta WHERE IdEstudiante IN (SELECT IdEstudiante FROM Estudiantes WHERE NumeroId LIKE 'TEST-%'));
DECLARE @TotalNotas INT = (SELECT COUNT(*) FROM NotasConducta WHERE IdEstudiante IN (SELECT IdEstudiante FROM Estudiantes WHERE NumeroId LIKE 'TEST-%'));
DECLARE @TotalProgramas INT = (SELECT COUNT(*) FROM ProgramasAccionesInstitucional WHERE IdEstudiante IN (SELECT IdEstudiante FROM Estudiantes WHERE NumeroId LIKE 'TEST-%'));
DECLARE @TotalDecisiones INT = (SELECT COUNT(*) FROM DecisionesProfesionalesConducta WHERE IdEstudiante IN (SELECT IdEstudiante FROM Estudiantes WHERE NumeroId LIKE 'TEST-%'));

PRINT 'Estudiantes creados: ' + CAST(@TotalEstudiantes AS NVARCHAR);
PRINT 'Boletas de conducta: ' + CAST(@TotalBoletas AS NVARCHAR);
PRINT 'Notas de conducta: ' + CAST(@TotalNotas AS NVARCHAR);
PRINT 'Programas de acciones: ' + CAST(@TotalProgramas AS NVARCHAR);
PRINT 'Decisiones profesionales: ' + CAST(@TotalDecisiones AS NVARCHAR);
PRINT '';

-- Mostrar detalle de estudiantes
PRINT 'DETALLE POR ESTUDIANTE:';
PRINT '------------------------';

SELECT 
    e.NumeroId AS 'ID',
    e.Nombre + ' ' + e.Apellidos AS 'Estudiante',
    n.NotaFinal AS 'Nota',
    n.Estado AS 'Estado',
    n.TotalRebajos AS 'Rebajos',
    COUNT(b.IdBoleta) AS 'Boletas',
    CASE WHEN n.RequiereProgramaAcciones = 1 THEN 'Sí' ELSE 'No' END AS 'Req.Programa',
    CASE WHEN n.DecisionProfesionalAplicada = 1 THEN 'Sí' ELSE 'No' END AS 'Con Decisión'
FROM Estudiantes e
INNER JOIN NotasConducta n ON e.IdEstudiante = n.IdEstudiante
LEFT JOIN BoletasConducta b ON e.IdEstudiante = b.IdEstudiante AND b.Estado = 'Activa'
WHERE e.NumeroId LIKE 'TEST-%'
GROUP BY e.NumeroId, e.Nombre, e.Apellidos, n.NotaFinal, n.Estado, n.TotalRebajos, n.RequiereProgramaAcciones, n.DecisionProfesionalAplicada
ORDER BY n.NotaFinal;

PRINT '';
PRINT '========================================';
PRINT '✓ DATOS SEMILLA INSERTADOS EXITOSAMENTE';
PRINT '========================================';
PRINT '';
PRINT 'Ahora puedes visualizar:';
PRINT '  • Estudiantes en RIESGO (TEST-001, TEST-006)';
PRINT '  • Estudiantes APLAZADOS (TEST-002, TEST-003, TEST-004)';
PRINT '  • Programa de Acciones (TEST-003)';
PRINT '  • Decisión Profesional (TEST-004)';
PRINT '  • Diferentes tipos de boletas y faltas';
PRINT '';

SET NOCOUNT OFF;
