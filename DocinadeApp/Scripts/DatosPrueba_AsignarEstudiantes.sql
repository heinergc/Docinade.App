-- Script para crear datos de prueba para AsignarEstudiantes
-- Este script crea períodos, grupos y estudiantes necesarios para probar la funcionalidad

USE RubricasDb;
GO

PRINT '🚀 Iniciando creación de datos de prueba para AsignarEstudiantes...';

-- 1. Verificar/Crear período académico
DECLARE @PeriodoId INT;
SELECT TOP 1 @PeriodoId = Id FROM PeriodosAcademicos WHERE Activo = 1;

IF @PeriodoId IS NULL
BEGIN
    PRINT '📅 Creando período académico...';
    INSERT INTO PeriodosAcademicos (Nombre, FechaInicio, FechaFin, Activo, FechaCreacion)
    VALUES ('2025 Curso Lectivo', '2025-02-12', '2025-12-06', 1, GETDATE());
    
    SELECT @PeriodoId = SCOPE_IDENTITY();
    PRINT '✅ Período académico creado con ID: ' + CAST(@PeriodoId AS VARCHAR(10));
END
ELSE
BEGIN
    PRINT '✅ Usando período académico existente con ID: ' + CAST(@PeriodoId AS VARCHAR(10));
END

-- 2. Crear grupos de estudiantes si no existen
IF NOT EXISTS (SELECT 1 FROM GruposEstudiantes WHERE PeriodoAcademicoId = @PeriodoId)
BEGIN
    PRINT '🏫 Creando grupos de estudiantes...';
    
    INSERT INTO GruposEstudiantes (Codigo, Nombre, Descripcion, TipoGrupo, Nivel, CapacidadMaxima, PeriodoAcademicoId, Estado, FechaCreacion)
    VALUES 
    ('11-A', '11° Año Sección A', 'Undécimo año, sección A - Matemáticas y Ciencias', 'Académico', 'Undécimo', 30, @PeriodoId, 'Activo', GETDATE()),
    ('11-B', '11° Año Sección B', 'Undécimo año, sección B - Matemáticas y Ciencias', 'Académico', 'Undécimo', 30, @PeriodoId, 'Activo', GETDATE()),
    ('10-A', '10° Año Sección A', 'Décimo año, sección A - Ciencias Naturales', 'Académico', 'Décimo', 28, @PeriodoId, 'Activo', GETDATE()),
    ('10-B', '10° Año Sección B', 'Décimo año, sección B - Ciencias Naturales', 'Académico', 'Décimo', 28, @PeriodoId, 'Activo', GETDATE()),
    ('9-A', '9° Año Sección A', 'Noveno año, sección A - Ciencias Básicas', 'Académico', 'Noveno', 25, @PeriodoId, 'Activo', GETDATE());
    
    PRINT '✅ Grupos creados exitosamente';
END
ELSE
BEGIN
    PRINT '✅ Grupos ya existen para este período';
END

-- 3. Crear estudiantes si no existen
IF NOT EXISTS (SELECT 1 FROM Estudiantes WHERE PeriodoAcademicoId = @PeriodoId)
BEGIN
    PRINT '👥 Creando estudiantes de prueba...';
    
    INSERT INTO Estudiantes (Nombre, Apellidos, NumeroId, DireccionCorreo, Institucion, Grupos, Año, PeriodoAcademicoId)
    VALUES 
    -- Estudiantes para 11-A
    ('José Andrés', 'Vargas Solano', '117850421', 'jose.vargas@estudiantes.mep.go.cr', 'Colegio Científico de Costa Rica', '11-A', 2025, @PeriodoId),
    ('María Fernanda', 'Jiménez Castillo', '207940312', 'maria.jimenez@estudiantes.mep.go.cr', 'Colegio Científico de Costa Rica', '11-A', 2025, @PeriodoId),
    ('Carlos Daniel', 'Morales Hernández', '305820193', 'carlos.morales@estudiantes.mep.go.cr', 'Colegio Científico de Costa Rica', '11-A', 2025, @PeriodoId),
    ('Ana Lucía', 'Rodríguez Campos', '401730264', 'ana.rodriguez@estudiantes.mep.go.cr', 'Colegio Científico de Costa Rica', '11-A', 2025, @PeriodoId),
    ('Diego Alejandro', 'Sánchez Mora', '118640375', 'diego.sanchez@estudiantes.mep.go.cr', 'Colegio Científico de Costa Rica', '11-A', 2025, @PeriodoId),
    
    -- Estudiantes para 11-B  
    ('Valeria Sofía', 'Quesada Villalobos', '209550186', 'valeria.quesada@estudiantes.mep.go.cr', 'Colegio Científico de Costa Rica', '11-B', 2025, @PeriodoId),
    ('Sebastián José', 'Araya Ramírez', '306460297', 'sebastian.araya@estudiantes.mep.go.cr', 'Colegio Científico de Costa Rica', '11-B', 2025, @PeriodoId),
    ('Camila Andrea', 'Alfaro González', '402370158', 'camila.alfaro@estudiantes.mep.go.cr', 'Colegio Científico de Costa Rica', '11-B', 2025, @PeriodoId),
    ('Adrián Fernando', 'Chacón Herrera', '119280349', 'adrian.chacon@estudiantes.mep.go.cr', 'Colegio Científico de Costa Rica', '11-B', 2025, @PeriodoId),
    ('Isabella María', 'Cordero Vega', '210190450', 'isabella.cordero@estudiantes.mep.go.cr', 'Colegio Científico de Costa Rica', '11-B', 2025, @PeriodoId),
    
    -- Estudiantes sin asignar (disponibles)
    ('Esteban Andrés', 'Bolaños Picado', '307080211', 'esteban.bolanos@estudiantes.mep.go.cr', 'Colegio Científico de Costa Rica', '', 2025, @PeriodoId),
    ('Natalia Paola', 'Monge Fallas', '403990322', 'natalia.monge@estudiantes.mep.go.cr', 'Colegio Científico de Costa Rica', '', 2025, @PeriodoId),
    ('Mauricio Andrés', 'Cascante Aguilar', '120920433', 'mauricio.cascante@estudiantes.mep.go.cr', 'Colegio Científico de Costa Rica', '', 2025, @PeriodoId),
    ('Gabriela Patricia', 'Vindas Rojas', '211830544', 'gabriela.vindas@estudiantes.mep.go.cr', 'Colegio Científico de Costa Rica', '', 2025, @PeriodoId),
    ('Fernando José', 'Trejos Méndez', '308720255', 'fernando.trejos@estudiantes.mep.go.cr', 'Colegio Científico de Costa Rica', '', 2025, @PeriodoId);
    
    PRINT '✅ Estudiantes creados exitosamente';
END
ELSE
BEGIN
    PRINT '✅ Estudiantes ya existen para este período';
END

-- 4. Crear algunas asignaciones de estudiantes a grupos
PRINT '🔗 Asignando estudiantes a grupos...';

-- Obtener IDs de grupos
DECLARE @Grupo11A INT, @Grupo11B INT;
SELECT @Grupo11A = GrupoId FROM GruposEstudiantes WHERE Codigo = '11-A' AND PeriodoAcademicoId = @PeriodoId;
SELECT @Grupo11B = GrupoId FROM GruposEstudiantes WHERE Codigo = '11-B' AND PeriodoAcademicoId = @PeriodoId;

-- Asignar estudiantes a grupos (si las asignaciones no existen)
IF @Grupo11A IS NOT NULL AND @Grupo11B IS NOT NULL
BEGIN
    -- Asignar estudiantes del 11-A
    INSERT INTO EstudianteGrupos (EstudianteId, GrupoId, FechaAsignacion, Estado)
    SELECT e.IdEstudiante, @Grupo11A, GETDATE(), 'Activo'
    FROM Estudiantes e 
    WHERE e.PeriodoAcademicoId = @PeriodoId 
    AND e.Grupos = '11-A'
    AND NOT EXISTS (SELECT 1 FROM EstudianteGrupos eg WHERE eg.EstudianteId = e.IdEstudiante AND eg.GrupoId = @Grupo11A);
    
    -- Asignar estudiantes del 11-B  
    INSERT INTO EstudianteGrupos (EstudianteId, GrupoId, FechaAsignacion, Estado)
    SELECT e.IdEstudiante, @Grupo11B, GETDATE(), 'Activo'
    FROM Estudiantes e 
    WHERE e.PeriodoAcademicoId = @PeriodoId 
    AND e.Grupos = '11-B'
    AND NOT EXISTS (SELECT 1 FROM EstudianteGrupos eg WHERE eg.EstudianteId = e.IdEstudiante AND eg.GrupoId = @Grupo11B);
    
    PRINT '✅ Asignaciones creadas exitosamente';
END

-- 5. Mostrar resumen de datos creados
PRINT '📊 === RESUMEN DE DATOS CREADOS ===';

DECLARE @TotalEstudiantes INT, @TotalGrupos INT, @TotalAsignaciones INT;
SELECT @TotalEstudiantes = COUNT(*) FROM Estudiantes WHERE PeriodoAcademicoId = @PeriodoId;
SELECT @TotalGrupos = COUNT(*) FROM GruposEstudiantes WHERE PeriodoAcademicoId = @PeriodoId;  
SELECT @TotalAsignaciones = COUNT(*) FROM EstudianteGrupos eg 
    INNER JOIN GruposEstudiantes g ON eg.GrupoId = g.GrupoId 
    WHERE g.PeriodoAcademicoId = @PeriodoId AND eg.Estado = 'Activo';

PRINT '👥 Total estudiantes: ' + CAST(@TotalEstudiantes AS VARCHAR(10));
PRINT '🏫 Total grupos: ' + CAST(@TotalGrupos AS VARCHAR(10));
PRINT '🔗 Total asignaciones: ' + CAST(@TotalAsignaciones AS VARCHAR(10));

-- Mostrar estudiantes disponibles (sin asignar)
DECLARE @EstudiantesDisponibles INT;
SELECT @EstudiantesDisponibles = COUNT(*)
FROM Estudiantes e
WHERE e.PeriodoAcademicoId = @PeriodoId 
AND NOT EXISTS (
    SELECT 1 FROM EstudianteGrupos eg 
    INNER JOIN GruposEstudiantes g ON eg.GrupoId = g.GrupoId
    WHERE eg.EstudianteId = e.IdEstudiante 
    AND g.PeriodoAcademicoId = @PeriodoId 
    AND eg.Estado = 'Activo'
);

PRINT '🆓 Estudiantes disponibles para asignar: ' + CAST(@EstudiantesDisponibles AS VARCHAR(10));

PRINT '🎉 ¡Datos de prueba creados exitosamente para AsignarEstudiantes!';
