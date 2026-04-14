-- =====================================================================
-- DATOS INICIALES MÍNIMOS PARA PRODUCCIÓN
-- Sistema de Rúbricas - RubricasApp.Web
-- =====================================================================
-- Este script contiene SOLO los datos esenciales para que el sistema
-- funcione después del primer despliegue.
-- 
-- NOTA: Las tablas y estructura ya se crean automáticamente.
-- Este script solo agrega datos de catálogo necesarios.
-- =====================================================================

USE [RubricaDB];
GO

PRINT '🚀 Iniciando carga de datos iniciales para producción...';
PRINT '';

-- =====================================================================
-- 1. PERÍODOS ACADÉMICOS
-- =====================================================================
PRINT '📅 Configurando períodos académicos...';

-- Desactivar cualquier período activo existente
UPDATE PeriodosAcademicos SET Activo = 0 WHERE Activo = 1;

-- II CICLO 2025 (ACTIVO - Costa Rica)
IF NOT EXISTS (SELECT 1 FROM PeriodosAcademicos WHERE Código = '2025-II')
BEGIN
    INSERT INTO PeriodosAcademicos (Año, Anio, Ciclo, FechaInicio, FechaFin, Activo, Codigo, Nombre, Tipo, NumeroPeriodo, FechaCreacion, Estado)
    VALUES (2025, 2025, 'II', '2025-08-04', '2025-12-19', 1, '2025-II', 'Segundo Ciclo 2025', 1, 2, GETDATE(), 'Activo');
    PRINT '  ✅ Período 2025-II creado (ACTIVO)';
END
ELSE
BEGIN
    UPDATE PeriodosAcademicos SET Activo = 1 WHERE Codigo = '2025-II';
    PRINT '  ✅ Período 2025-II activado';
END

-- I CICLO 2026 (PLANIFICADO)
IF NOT EXISTS (SELECT 1 FROM PeriodosAcademicos WHERE Codigo = '2026-I')
BEGIN
    INSERT INTO PeriodosAcademicos (Año, Anio, Ciclo, FechaInicio, FechaFin, Activo, Codigo, Nombre, Tipo, NumeroPeriodo, FechaCreacion, Estado)
    VALUES (2026, 2026, 'I', '2026-02-02', '2026-07-17', 0, '2026-I', 'Primer Ciclo 2026', 1, 1, GETDATE(), 'Planificado');
    PRINT '  ✅ Período 2026-I creado (PLANIFICADO)';
END

PRINT '';

-- =====================================================================
-- 2. MATERIAS BÁSICAS (MEP - Costa Rica)
-- =====================================================================
PRINT '📚 Creando materias básicas...';

-- Matemáticas
IF NOT EXISTS (SELECT 1 FROM Materias WHERE Codigo = 'MAT-11')
BEGIN
    INSERT INTO Materias (Codigo, Nombre, Creditos, Activo, Tipo, CicloSugerido, Descripcion, FechaCreacion, FechaModificacion, Estado, EscuelaId)
    VALUES ('MAT-11', 'Matemáticas 11°', 4, 1, 'Undécimo', 2, 'Matemática para undécimo año de educación media', GETDATE(), NULL, 'Activo', NULL);
    PRINT '  ✅ Matemáticas 11° creada';
END

IF NOT EXISTS (SELECT 1 FROM Materias WHERE Codigo = 'MAT-10')
BEGIN
    INSERT INTO Materias (Codigo, Nombre, Creditos, Activo, Tipo, CicloSugerido, Descripcion, FechaCreacion, FechaModificacion, Estado, EscuelaId)
    VALUES ('MAT-10', 'Matemáticas 10°', 4, 1, 'Décimo', 2, 'Matemática para décimo año de educación media', GETDATE(), NULL, 'Activo', NULL);
    PRINT '  ✅ Matemáticas 10° creada';
END

-- Español
IF NOT EXISTS (SELECT 1 FROM Materias WHERE Codigo = 'ESP-11')
BEGIN
    INSERT INTO Materias (Codigo, Nombre, Creditos, Activo, Tipo, CicloSugerido, Descripcion, FechaCreacion, FechaModificacion, Estado, EscuelaId)
    VALUES ('ESP-11', 'Español 11°', 4, 1, 'Undécimo', 2, 'Español para undécimo año de educación media', GETDATE(), NULL, 'Activo', NULL);
    PRINT '  ✅ Español 11° creada';
END

IF NOT EXISTS (SELECT 1 FROM Materias WHERE Codigo = 'ESP-10')
BEGIN
    INSERT INTO Materias (Codigo, Nombre, Creditos, Activo, Tipo, CicloSugerido, Descripcion, FechaCreacion, FechaModificacion, Estado, EscuelaId)
    VALUES ('ESP-10', 'Español 10°', 4, 1, 'Décimo', 2, 'Español para décimo año de educación media', GETDATE(), NULL, 'Activo', NULL);
    PRINT '  ✅ Español 10° creada';
END

-- Ciencias
IF NOT EXISTS (SELECT 1 FROM Materias WHERE Codigo = 'BIO-11')
BEGIN
    INSERT INTO Materias (Codigo, Nombre, Creditos, Activo, Tipo, CicloSugerido, Descripcion, FechaCreacion, FechaModificacion, Estado, EscuelaId)
    VALUES ('BIO-11', 'Biología 11°', 3, 1, 'Undécimo', 2, 'Biología para undécimo año de educación media', GETDATE(), NULL, 'Activo', NULL);
    PRINT '  ✅ Biología 11° creada';
END

IF NOT EXISTS (SELECT 1 FROM Materias WHERE Codigo = 'FIS-11')
BEGIN
    INSERT INTO Materias (Codigo, Nombre, Creditos, Activo, Tipo, CicloSugerido, Descripcion, FechaCreacion, FechaModificacion, Estado, EscuelaId)
    VALUES ('FIS-11', 'Física 11°', 3, 1, 'Undécimo', 2, 'Física para undécimo año de educación media', GETDATE(), NULL, 'Activo', NULL);
    PRINT '  ✅ Física 11° creada';
END

IF NOT EXISTS (SELECT 1 FROM Materias WHERE Codigo = 'QUIM-11')
BEGIN
    INSERT INTO Materias (Codigo, Nombre, Creditos, Activo, Tipo, CicloSugerido, Descripcion, FechaCreacion, FechaModificacion, Estado, EscuelaId)
    VALUES ('QUIM-11', 'Química 11°', 3, 1, 'Undécimo', 2, 'Química para undécimo año de educación media', GETDATE(), NULL, 'Activo', NULL);
    PRINT '  ✅ Química 11° creada';
END

-- Ciencias Sociales
IF NOT EXISTS (SELECT 1 FROM Materias WHERE Codigo = 'ESTU-11')
BEGIN
    INSERT INTO Materias (Codigo, Nombre, Creditos, Activo, Tipo, CicloSugerido, Descripcion, FechaCreacion, FechaModificacion, Estado, EscuelaId)
    VALUES ('ESTU-11', 'Estudios Sociales 11°', 3, 1, 'Undécimo', 2, 'Estudios Sociales para undécimo año', GETDATE(), NULL, 'Activo', NULL);
    PRINT '  ✅ Estudios Sociales 11° creada';
END

IF NOT EXISTS (SELECT 1 FROM Materias WHERE Codigo = 'ESTU-10')
BEGIN
    INSERT INTO Materias (Codigo, Nombre, Creditos, Activo, Tipo, CicloSugerido, Descripcion, FechaCreacion, FechaModificacion, Estado, EscuelaId)
    VALUES ('ESTU-10', 'Estudios Sociales 10°', 3, 1, 'Décimo', 2, 'Estudios Sociales para décimo año', GETDATE(), NULL, 'Activo', NULL);
    PRINT '  ✅ Estudios Sociales 10° creada';
END

-- Inglés
IF NOT EXISTS (SELECT 1 FROM Materias WHERE Codigo = 'ING-11')
BEGIN
    INSERT INTO Materias (Codigo, Nombre, Creditos, Activo, Tipo, CicloSugerido, Descripcion, FechaCreacion, FechaModificacion, Estado, EscuelaId)
    VALUES ('ING-11', 'Inglés 11°', 3, 1, 'Undécimo', 2, 'Inglés para undécimo año de educación media', GETDATE(), NULL, 'Activo', NULL);
    PRINT '  ✅ Inglés 11° creada';
END

IF NOT EXISTS (SELECT 1 FROM Materias WHERE Codigo = 'ING-10')
BEGIN
    INSERT INTO Materias (Codigo, Nombre, Creditos, Activo, Tipo, CicloSugerido, Descripcion, FechaCreacion, FechaModificacion, Estado, EscuelaId)
    VALUES ('ING-10', 'Inglés 10°', 3, 1, 'Décimo', 2, 'Inglés para décimo año de educación media', GETDATE(), NULL, 'Activo', NULL);
    PRINT '  ✅ Inglés 10° creada';
END

PRINT '';

-- =====================================================================
-- 3. GRUPOS DE ESTUDIANTES BÁSICOS
-- =====================================================================
PRINT '🏫 Creando grupos básicos...';

DECLARE @PeriodoActivoId INT;
SELECT TOP 1 @PeriodoActivoId = Id FROM PeriodosAcademicos WHERE Activo = 1 ORDER BY FechaInicio DESC;

IF @PeriodoActivoId IS NOT NULL
BEGIN
    -- Grupo 11-A
    IF NOT EXISTS (SELECT 1 FROM GruposEstudiantes WHERE Codigo = '11-A' AND PeriodoAcademicoId = @PeriodoActivoId)
    BEGIN
        INSERT INTO GruposEstudiantes (Codigo, Nombre, Descripcion, TipoGrupo, Nivel, CapacidadMaxima, PeriodoAcademicoId, Estado, FechaCreacion)
        VALUES ('11-A', 'Undécimo Año - Sección A', 'Grupo de undécimo año, sección A', 'Seccion', 'UNDECIMO', 35, @PeriodoActivoId, 'Activo', GETDATE());
        PRINT '  ✅ Grupo 11-A creado';
    END

    -- Grupo 11-B
    IF NOT EXISTS (SELECT 1 FROM GruposEstudiantes WHERE Codigo = '11-B' AND PeriodoAcademicoId = @PeriodoActivoId)
    BEGIN
        INSERT INTO GruposEstudiantes (Codigo, Nombre, Descripcion, TipoGrupo, Nivel, CapacidadMaxima, PeriodoAcademicoId, Estado, FechaCreacion)
        VALUES ('11-B', 'Undécimo Año - Sección B', 'Grupo de undécimo año, sección B', 'Seccion', 'UNDECIMO', 35, @PeriodoActivoId, 'Activo', GETDATE());
        PRINT '  ✅ Grupo 11-B creado';
    END

    -- Grupo 10-A
    IF NOT EXISTS (SELECT 1 FROM GruposEstudiantes WHERE Codigo = '10-A' AND PeriodoAcademicoId = @PeriodoActivoId)
    BEGIN
        INSERT INTO GruposEstudiantes (Codigo, Nombre, Descripcion, TipoGrupo, Nivel, CapacidadMaxima, PeriodoAcademicoId, Estado, FechaCreacion)
        VALUES ('10-A', 'Décimo Año - Sección A', 'Grupo de décimo año, sección A', 'Seccion', 'DECIMO', 35, @PeriodoActivoId, 'Activo', GETDATE());
        PRINT '  ✅ Grupo 10-A creado';
    END

    -- Grupo 10-B
    IF NOT EXISTS (SELECT 1 FROM GruposEstudiantes WHERE Codigo = '10-B' AND PeriodoAcademicoId = @PeriodoActivoId)
    BEGIN
        INSERT INTO GruposEstudiantes (Codigo, Nombre, Descripcion, TipoGrupo, Nivel, CapacidadMaxima, PeriodoAcademicoId, Estado, FechaCreacion)
        VALUES ('10-B', 'Décimo Año - Sección B', 'Grupo de décimo año, sección B', 'Seccion', 'DECIMO', 35, @PeriodoActivoId, 'Activo', GETDATE());
        PRINT '  ✅ Grupo 10-B creado';
    END
END
ELSE
BEGIN
    PRINT '  ⚠️ No hay período activo, grupos no creados';
END

PRINT '';

-- =====================================================================
-- 4. ESTUDIANTES DE PRUEBA (OPCIONAL - Solo para desarrollo/pruebas)
-- =====================================================================
PRINT '👥 Creando 10 estudiantes de prueba...';

IF @PeriodoActivoId IS NOT NULL AND NOT EXISTS (SELECT 1 FROM Estudiantes WHERE PeriodoAcademicoId = @PeriodoActivoId)
BEGIN
    INSERT INTO Estudiantes (Nombre, Apellidos, NumeroId, DireccionCorreo, Institucion, Grupos, Año, PeriodoAcademicoId)
    VALUES 
    -- Grupo 11-A
    ('José Andrés', 'Vargas Solano', '117850421', 'jose.vargas@estudiantes.mep.go.cr', 'Colegio Científico', '11-A', 2025, @PeriodoActivoId),
    ('María Fernanda', 'Rodríguez Castro', '205740322', 'maria.rodriguez@estudiantes.mep.go.cr', 'Colegio Científico', '11-A', 2025, @PeriodoActivoId),
    ('Carlos Alberto', 'Jiménez Mora', '304560213', 'carlos.jimenez@estudiantes.mep.go.cr', 'Colegio Científico', '11-A', 2025, @PeriodoActivoId),
    ('Ana Lucía', 'González Pérez', '403210645', 'ana.gonzalez@estudiantes.mep.go.cr', 'Colegio Científico', '11-A', 2025, @PeriodoActivoId),
    ('Luis Fernando', 'Hernández Arias', '502340876', 'luis.hernandez@estudiantes.mep.go.cr', 'Colegio Científico', '11-A', 2025, @PeriodoActivoId),
    
    -- Grupo 11-B
    ('Gabriela María', 'Campos Rojas', '601450987', 'gabriela.campos@estudiantes.mep.go.cr', 'Colegio Científico', '11-B', 2025, @PeriodoActivoId),
    ('Diego Alejandro', 'Ramírez Solís', '107230654', 'diego.ramirez@estudiantes.mep.go.cr', 'Colegio Científico', '11-B', 2025, @PeriodoActivoId),
    ('Sofía Isabel', 'Morales Vega', '206340721', 'sofia.morales@estudiantes.mep.go.cr', 'Colegio Científico', '11-B', 2025, @PeriodoActivoId),
    ('Andrés Felipe', 'Castro Méndez', '305120843', 'andres.castro@estudiantes.mep.go.cr', 'Colegio Científico', '11-B', 2025, @PeriodoActivoId),
    ('Valeria Andrea', 'Gutiérrez Salas', '404560912', 'valeria.gutierrez@estudiantes.mep.go.cr', 'Colegio Científico', '11-B', 2025, @PeriodoActivoId);

    PRINT '  ✅ 10 estudiantes de prueba creados';
END
ELSE
BEGIN
    PRINT '  ℹ️ Estudiantes ya existen o no hay período activo';
END

PRINT '';

-- =====================================================================
-- 5. NIVELES DE CALIFICACIÓN
-- =====================================================================
PRINT '📊 Configurando niveles de calificación...';

IF NOT EXISTS (SELECT 1 FROM NivelesCalificacion)
BEGIN
    INSERT INTO NivelesCalificacion (NombreNivel, Descripcion, OrdenNivel, Activo)
    VALUES 
    ('Excelente', 'Supera ampliamente los criterios establecidos', 1, 1),
    ('Muy Bueno', 'Supera los criterios establecidos', 2, 1),
    ('Bueno', 'Cumple satisfactoriamente con los criterios', 3, 1),
    ('Regular', 'Cumple parcialmente con los criterios', 4, 1),
    ('Insuficiente', 'No cumple con los criterios mínimos', 5, 1);
    
    PRINT '  ✅ Niveles de calificación creados';
END
ELSE
BEGIN
    PRINT '  ℹ️ Niveles de calificación ya existen';
END

PRINT '';

-- =====================================================================
-- RESUMEN DE DATOS CREADOS
-- =====================================================================
PRINT '📋 RESUMEN DE DATOS INICIALES:';
PRINT '=====================================';

DECLARE @TotalPeriodos INT, @TotalMaterias INT, @TotalGrupos INT, @TotalEstudiantes INT, @TotalNiveles INT;

SELECT @TotalPeriodos = COUNT(*) FROM PeriodosAcademicos;
SELECT @TotalMaterias = COUNT(*) FROM Materias WHERE Activo = 1;
SELECT @TotalGrupos = COUNT(*) FROM GruposEstudiantes WHERE Estado = 'Activo';
SELECT @TotalEstudiantes = COUNT(*) FROM Estudiantes;
SELECT @TotalNiveles = COUNT(*) FROM NivelesCalificacion WHERE Activo = 1;

PRINT '  📅 Períodos académicos: ' + CAST(@TotalPeriodos AS VARCHAR(10));
PRINT '  📚 Materias activas: ' + CAST(@TotalMaterias AS VARCHAR(10));
PRINT '  🏫 Grupos activos: ' + CAST(@TotalGrupos AS VARCHAR(10));
PRINT '  👥 Estudiantes: ' + CAST(@TotalEstudiantes AS VARCHAR(10));
PRINT '  📊 Niveles de calificación: ' + CAST(@TotalNiveles AS VARCHAR(10));

PRINT '';
PRINT '🎉 ¡Datos iniciales cargados exitosamente!';
PRINT '';
PRINT '📝 PRÓXIMOS PASOS:';
PRINT '  1. Ejecutar recrear_admin.sql para crear usuario administrador';
PRINT '  2. Iniciar sesión con: admin@rubricas.edu / Admin@2025';
PRINT '  3. Crear/importar estudiantes adicionales desde la UI';
PRINT '  4. Configurar instrumentos de evaluación';
PRINT '';
PRINT '✅ El sistema está listo para usar';
GO
