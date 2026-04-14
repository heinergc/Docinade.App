-- =====================================================================
-- SCRIPT PARA INSERTAR PERÍODOS ACADÉMICOS DEL MEP COSTA RICA
-- Sistema de Rúbricas - Base de Datos RubricasDb
-- =====================================================================
-- Este script inserta los períodos académicos utilizados en el 
-- sistema educativo costarricense según las directrices del MEP
-- =====================================================================

USE [RubricasDb];
GO

PRINT '🎓 Iniciando inserción de Períodos Académicos del MEP Costa Rica...';
PRINT '';

-- =====================================================================
-- CONFIGURACIÓN INICIAL
-- =====================================================================

-- Verificar si ya existen períodos académicos
DECLARE @PeriodosExistentes INT;
SELECT @PeriodosExistentes = COUNT(*) FROM PeriodosAcademicos;

PRINT 'Períodos académicos existentes en la base de datos: ' + CAST(@PeriodosExistentes AS VARCHAR(10));
PRINT '';

-- =====================================================================
-- PERÍODOS ACADÉMICOS 2024
-- =====================================================================

PRINT '📅 Insertando períodos académicos 2024...';

-- I CICLO 2024 (Febrero - Julio)
IF NOT EXISTS (SELECT 1 FROM PeriodosAcademicos WHERE Año = 2024 AND Ciclo = 'I')
BEGIN
    INSERT INTO PeriodosAcademicos (
        Año, Anio, Ciclo, FechaInicio, FechaFin, Activo, 
        Codigo, Nombre, Tipo, NumeroPeriodo, 
        FechaCreacion, Estado
    )
    VALUES (
        2024, 2024, 'I', '2024-02-05', '2024-07-19', 0,
        '2024-I', 'Primer Ciclo 2024', 1, -- Semestre
        1, GETDATE(), 'Finalizado'
    );
    PRINT '  ✓ Insertado: I Ciclo 2024 (Feb - Jul)';
END
ELSE
    PRINT '  ⚠️ I Ciclo 2024 ya existe, omitiendo...';

-- II CICLO 2024 (Agosto - Diciembre)
IF NOT EXISTS (SELECT 1 FROM PeriodosAcademicos WHERE Año = 2024 AND Ciclo = 'II')
BEGIN
    INSERT INTO PeriodosAcademicos (
        Año, Anio, Ciclo, FechaInicio, FechaFin, Activo, 
        Codigo, Nombre, Tipo, NumeroPeriodo, 
        FechaCreacion, Estado
    )
    VALUES (
        2024, 2024, 'II', '2024-08-05', '2024-12-20', 0,
        '2024-II', 'Segundo Ciclo 2024', 1, -- Semestre
        2, GETDATE(), 'Finalizado'
    );
    PRINT '  ✓ Insertado: II Ciclo 2024 (Ago - Dic)';
END
ELSE
    PRINT '  ⚠️ II Ciclo 2024 ya existe, omitiendo...';

-- =====================================================================
-- PERÍODOS ACADÉMICOS 2025 (AÑO ACTUAL)
-- =====================================================================

PRINT '';
PRINT '📅 Insertando períodos académicos 2025 (año actual)...';

-- I CICLO 2025 (Febrero - Julio)
IF NOT EXISTS (SELECT 1 FROM PeriodosAcademicos WHERE Año = 2025 AND Ciclo = 'I')
BEGIN
    INSERT INTO PeriodosAcademicos (
        Año, Anio, Ciclo, FechaInicio, FechaFin, Activo, 
        Codigo, Nombre, Tipo, NumeroPeriodo, 
        FechaCreacion, Estado
    )
    VALUES (
        2025, 2025, 'I', '2025-02-03', '2025-07-18', 0,
        '2025-I', 'Primer Ciclo 2025', 1, -- Semestre
        1, GETDATE(), 'Finalizado'
    );
    PRINT '  ✓ Insertado: I Ciclo 2025 (Feb - Jul) - FINALIZADO';
END
ELSE
    PRINT '  ⚠️ I Ciclo 2025 ya existe, omitiendo...';

-- II CICLO 2025 (Agosto - Diciembre) - PERÍODO ACTUAL ACTIVO
IF NOT EXISTS (SELECT 1 FROM PeriodosAcademicos WHERE Año = 2025 AND Ciclo = 'II')
BEGIN
    INSERT INTO PeriodosAcademicos (
        Año, Anio, Ciclo, FechaInicio, FechaFin, Activo, 
        Codigo, Nombre, Tipo, NumeroPeriodo, 
        FechaCreacion, Estado
    )
    VALUES (
        2025, 2025, 'II', '2025-08-05', '2025-12-20', 1,
        '2025-II', 'Segundo Ciclo 2025', 1, -- Semestre
        2, GETDATE(), 'Activo'
    );
    PRINT '  ✓ Insertado: II Ciclo 2025 (Ago - Dic) - 🟢 PERÍODO ACTIVO ACTUAL';
END
ELSE
    PRINT '  ⚠️ II Ciclo 2025 ya existe, omitiendo...';

-- =====================================================================
-- PERÍODOS ACADÉMICOS 2026 (PLANIFICACIÓN FUTURA)
-- =====================================================================

PRINT '';
PRINT '📅 Insertando períodos académicos 2026 (planificación)...';

-- I CICLO 2026 (Febrero - Julio)
IF NOT EXISTS (SELECT 1 FROM PeriodosAcademicos WHERE Año = 2026 AND Ciclo = 'I')
BEGIN
    INSERT INTO PeriodosAcademicos (
        Año, Anio, Ciclo, FechaInicio, FechaFin, Activo, 
        Codigo, Nombre, Tipo, NumeroPeriodo, 
        FechaCreacion, Estado
    )
    VALUES (
        2026, 2026, 'I', '2026-02-02', '2026-07-17', 0,
        '2026-I', 'Primer Ciclo 2026', 1, -- Semestre
        1, GETDATE(), 'Planificado'
    );
    PRINT '  ✓ Insertado: I Ciclo 2026 (Feb - Jul) - PLANIFICADO';
END
ELSE
    PRINT '  ⚠️ I Ciclo 2026 ya existe, omitiendo...';

-- II CICLO 2026 (Agosto - Diciembre)
IF NOT EXISTS (SELECT 1 FROM PeriodosAcademicos WHERE Año = 2026 AND Ciclo = 'II')
BEGIN
    INSERT INTO PeriodosAcademicos (
        Año, Anio, Ciclo, FechaInicio, FechaFin, Activo, 
        Codigo, Nombre, Tipo, NumeroPeriodo, 
        FechaCreacion, Estado
    )
    VALUES (
        2026, 2026, 'II', '2026-08-03', '2026-12-18', 0,
        '2026-II', 'Segundo Ciclo 2026', 1, -- Semestre
        2, GETDATE(), 'Planificado'
    );
    PRINT '  ✓ Insertado: II Ciclo 2026 (Ago - Dic) - PLANIFICADO';
END
ELSE
    PRINT '  ⚠️ II Ciclo 2026 ya existe, omitiendo...';

-- =====================================================================
-- PERÍODOS ESPECIALES (VERANO, REMEDIALES, ETC.)
-- =====================================================================

PRINT '';
PRINT '🌞 Insertando períodos especiales...';

-- CICLO DE VERANO 2025 (Enero)
IF NOT EXISTS (SELECT 1 FROM PeriodosAcademicos WHERE Año = 2025 AND Ciclo = 'Verano')
BEGIN
    INSERT INTO PeriodosAcademicos (
        Año, Anio, Ciclo, FechaInicio, FechaFin, Activo, 
        Codigo, Nombre, Tipo, NumeroPeriodo, 
        FechaCreacion, Estado
    )
    VALUES (
        2025, 2025, 'Verano', '2025-01-08', '2025-01-31', 0,
        '2025-VER', 'Ciclo de Verano 2025', 5, -- Personalizado
        3, GETDATE(), 'Finalizado'
    );
    PRINT '  ✓ Insertado: Ciclo de Verano 2025 (Enero) - Cursos remediales';
END
ELSE
    PRINT '  ⚠️ Ciclo de Verano 2025 ya existe, omitiendo...';

-- CICLO DE VERANO 2026 (Enero)
IF NOT EXISTS (SELECT 1 FROM PeriodosAcademicos WHERE Año = 2026 AND Ciclo = 'Verano')
BEGIN
    INSERT INTO PeriodosAcademicos (
        Año, Anio, Ciclo, FechaInicio, FechaFin, Activo, 
        Codigo, Nombre, Tipo, NumeroPeriodo, 
        FechaCreacion, Estado
    )
    VALUES (
        2026, 2026, 'Verano', '2026-01-07', '2026-01-30', 0,
        '2026-VER', 'Ciclo de Verano 2026', 5, -- Personalizado
        3, GETDATE(), 'Planificado'
    );
    PRINT '  ✓ Insertado: Ciclo de Verano 2026 (Enero) - PLANIFICADO';
END
ELSE
    PRINT '  ⚠️ Ciclo de Verano 2026 ya existe, omitiendo...';

-- =====================================================================
-- PERÍODOS HISTÓRICOS (2023 PARA REFERENCIA)
-- =====================================================================

PRINT '';
PRINT '📚 Insertando períodos históricos 2023...';

-- I CICLO 2023
IF NOT EXISTS (SELECT 1 FROM PeriodosAcademicos WHERE Año = 2023 AND Ciclo = 'I')
BEGIN
    INSERT INTO PeriodosAcademicos (
        Año, Anio, Ciclo, FechaInicio, FechaFin, Activo, 
        Codigo, Nombre, Tipo, NumeroPeriodo, 
        FechaCreacion, Estado
    )
    VALUES (
        2023, 2023, 'I', '2023-02-06', '2023-07-21', 0,
        '2023-I', 'Primer Ciclo 2023', 1, -- Semestre
        1, GETDATE(), 'Finalizado'
    );
    PRINT '  ✓ Insertado: I Ciclo 2023 (Feb - Jul) - HISTÓRICO';
END
ELSE
    PRINT '  ⚠️ I Ciclo 2023 ya existe, omitiendo...';

-- II CICLO 2023
IF NOT EXISTS (SELECT 1 FROM PeriodosAcademicos WHERE Año = 2023 AND Ciclo = 'II')
BEGIN
    INSERT INTO PeriodosAcademicos (
        Año, Anio, Ciclo, FechaInicio, FechaFin, Activo, 
        Codigo, Nombre, Tipo, NumeroPeriodo, 
        FechaCreacion, Estado
    )
    VALUES (
        2023, 2023, 'II', '2023-08-07', '2023-12-22', 0,
        '2023-II', 'Segundo Ciclo 2023', 1, -- Semestre
        2, GETDATE(), 'Finalizado'
    );
    PRINT '  ✓ Insertado: II Ciclo 2023 (Ago - Dic) - HISTÓRICO';
END
ELSE
    PRINT '  ⚠️ II Ciclo 2023 ya existe, omitiendo...';

-- =====================================================================
-- VERIFICACIÓN Y RESUMEN
-- =====================================================================

PRINT '';
PRINT '============================================================';
PRINT 'RESUMEN DE PERÍODOS ACADÉMICOS INSERTADOS';
PRINT '============================================================';

-- Mostrar todos los períodos académicos insertados
SELECT 
    Año,
    Ciclo,
    NombreCompleto,
    FORMAT(FechaInicio, 'dd/MM/yyyy') as FechaInicio,
    FORMAT(FechaFin, 'dd/MM/yyyy') as FechaFin,
    CASE 
        WHEN Activo = 1 THEN '🟢 ACTIVO'
        WHEN Estado = 'Finalizado' THEN '🔴 FINALIZADO'
        WHEN Estado = 'Planificado' THEN '🟡 PLANIFICADO'
        ELSE '⚪ ' + ISNULL(Estado, 'Sin estado')
    END as Estado,
    CASE Tipo
        WHEN 1 THEN 'Semestre'
        WHEN 2 THEN 'Cuatrimestre'
        WHEN 3 THEN 'Trimestre'
        WHEN 4 THEN 'Anual'
        WHEN 5 THEN 'Personalizado'
        ELSE 'No definido'
    END as TipoPeriodo
FROM PeriodosAcademicos
ORDER BY Año DESC, 
         CASE 
            WHEN Ciclo = 'Verano' THEN 0
            WHEN Ciclo = 'I' THEN 1 
            WHEN Ciclo = 'II' THEN 2 
            ELSE 3 
         END;

PRINT '';

-- Estadísticas generales
DECLARE @TotalPeriodos INT, @PeriodosActivos INT, @PeriodosFinalizados INT, @PeriodosPlanificados INT;

SELECT @TotalPeriodos = COUNT(*) FROM PeriodosAcademicos;
SELECT @PeriodosActivos = COUNT(*) FROM PeriodosAcademicos WHERE Activo = 1;
SELECT @PeriodosFinalizados = COUNT(*) FROM PeriodosAcademicos WHERE Estado = 'Finalizado';
SELECT @PeriodosPlanificados = COUNT(*) FROM PeriodosAcademicos WHERE Estado = 'Planificado';

PRINT 'ESTADÍSTICAS:';
PRINT '📊 Total de períodos académicos: ' + CAST(@TotalPeriodos AS VARCHAR(10));
PRINT '🟢 Períodos activos: ' + CAST(@PeriodosActivos AS VARCHAR(10));
PRINT '🔴 Períodos finalizados: ' + CAST(@PeriodosFinalizados AS VARCHAR(10));
PRINT '🟡 Períodos planificados: ' + CAST(@PeriodosPlanificados AS VARCHAR(10));

-- Mostrar el período activo actual
PRINT '';
PRINT 'PERÍODO ACADÉMICO ACTUAL:';
SELECT 
    '🎯 ' + NombreCompleto + ' (' + FORMAT(FechaInicio, 'dd/MM') + ' - ' + FORMAT(FechaFin, 'dd/MM') + ')' as PeriodoActual
FROM PeriodosAcademicos 
WHERE Activo = 1;

PRINT '';
PRINT '============================================================';
PRINT 'CALENDARIO ACADÉMICO MEP COSTA RICA';
PRINT '============================================================';
PRINT '';
PRINT '📅 ESTRUCTURA TÍPICA DEL AÑO ACADÉMICO:';
PRINT '';
PRINT '  🌞 Ciclo de Verano: Enero (4 semanas)';
PRINT '     • Cursos remediales y nivelación';
PRINT '     • Actividades especiales';
PRINT '';
PRINT '  📚 I Ciclo Lectivo: Febrero - Julio (20 semanas)';
PRINT '     • Inicio: Primera semana de febrero';
PRINT '     • Fin: Tercera semana de julio';
PRINT '     • Incluye: Semana Santa, feriados nacionales';
PRINT '';
PRINT '  🏖️ Vacaciones de Medio Año: Julio (2 semanas)';
PRINT '';
PRINT '  📚 II Ciclo Lectivo: Agosto - Diciembre (20 semanas)';
PRINT '     • Inicio: Primera semana de agosto';
PRINT '     • Fin: Tercera semana de diciembre';
PRINT '     • Incluye: Feriados patrios, fin de año';
PRINT '';
PRINT '  🎄 Vacaciones de Fin de Año: Diciembre - Enero (6 semanas)';
PRINT '';

PRINT '============================================================';
PRINT '🎉 SCRIPT COMPLETADO EXITOSAMENTE';
PRINT '============================================================';
PRINT '';
PRINT '✅ Los períodos académicos del MEP han sido insertados correctamente.';
PRINT '✅ El sistema está listo para gestionar el calendario académico costarricense.';
PRINT '✅ Período actual activo: II Ciclo 2025 (Agosto - Diciembre)';
PRINT '';
PRINT '📝 NOTAS IMPORTANTES:';
PRINT '• Solo un período puede estar activo a la vez';
PRINT '• Las fechas siguen el calendario oficial del MEP';
PRINT '• Los períodos de verano son opcionales para actividades especiales';
PRINT '• Actualice el período activo según corresponda al momento actual';

GO
