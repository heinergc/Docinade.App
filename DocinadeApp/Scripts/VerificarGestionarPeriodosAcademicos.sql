-- =====================================================================
-- SCRIPT DE VERIFICACIÓN Y GESTIÓN DE PERÍODOS ACADÉMICOS
-- Sistema de Rúbricas - Base de Datos RubricasDb
-- =====================================================================
-- Este script permite verificar, activar y gestionar períodos académicos
-- =====================================================================

USE [RubricasDb];
GO

PRINT '🔍 Script de Verificación y Gestión de Períodos Académicos';
PRINT '============================================================';
PRINT '';

-- =====================================================================
-- 1. VERIFICAR PERÍODOS EXISTENTES
-- =====================================================================

PRINT '1. PERÍODOS ACADÉMICOS EXISTENTES:';
PRINT '-----------------------------------';

IF EXISTS (SELECT 1 FROM PeriodosAcademicos)
BEGIN
    SELECT 
        ROW_NUMBER() OVER (ORDER BY Año DESC, 
            CASE WHEN Ciclo = 'I' THEN 1 WHEN Ciclo = 'II' THEN 2 ELSE 3 END) as '#',
        Año,
        Ciclo,
        NombreCompleto as 'Nombre Completo',
        FORMAT(FechaInicio, 'dd/MM/yyyy') as 'Fecha Inicio',
        FORMAT(FechaFin, 'dd/MM/yyyy') as 'Fecha Fin',
        CASE 
            WHEN Activo = 1 THEN '🟢 ACTIVO'
            WHEN Estado = 'Finalizado' THEN '🔴 FINALIZADO'
            WHEN Estado = 'Planificado' THEN '🟡 PLANIFICADO'
            ELSE '⚪ ' + ISNULL(Estado, 'Sin estado')
        END as Estado,
        DATEDIFF(DAY, FechaInicio, FechaFin) as 'Días'
    FROM PeriodosAcademicos
    ORDER BY Año DESC, 
             CASE WHEN Ciclo = 'I' THEN 1 WHEN Ciclo = 'II' THEN 2 ELSE 3 END;
END
ELSE
BEGIN
    PRINT '⚠️  No se encontraron períodos académicos en la base de datos.';
    PRINT 'Ejecute el script de inserción de períodos académicos primero.';
END

PRINT '';

-- =====================================================================
-- 2. PERÍODO ACTIVO ACTUAL
-- =====================================================================

PRINT '2. PERÍODO ACTIVO ACTUAL:';
PRINT '-------------------------';

DECLARE @PeriodoActivo NVARCHAR(100);
SELECT @PeriodoActivo = NombreCompleto 
FROM PeriodosAcademicos 
WHERE Activo = 1;

IF @PeriodoActivo IS NOT NULL
BEGIN
    SELECT 
        '🎯 PERÍODO ACTIVO ACTUAL' as Información,
        NombreCompleto as 'Período',
        FORMAT(FechaInicio, 'dd/MM/yyyy') as 'Inicio',
        FORMAT(FechaFin, 'dd/MM/yyyy') as 'Fin',
        CASE 
            WHEN GETDATE() < FechaInicio THEN '⏳ Aún no inicia'
            WHEN GETDATE() > FechaFin THEN '⚠️ Ya finalizó'
            ELSE '✅ En curso'
        END as 'Estado Actual',
        CASE 
            WHEN GETDATE() < FechaInicio THEN DATEDIFF(DAY, GETDATE(), FechaInicio)
            WHEN GETDATE() <= FechaFin THEN DATEDIFF(DAY, GETDATE(), FechaFin)
            ELSE 0
        END as 'Días Restantes'
    FROM PeriodosAcademicos 
    WHERE Activo = 1;
END
ELSE
BEGIN
    PRINT '⚠️  No hay ningún período académico activo.';
    PRINT 'Use las opciones de gestión para activar un período.';
END

PRINT '';

-- =====================================================================
-- 3. ANÁLISIS DE FECHAS
-- =====================================================================

PRINT '3. ANÁLISIS DE FECHAS Y ESTADOS:';
PRINT '--------------------------------';

SELECT 
    'Fecha actual del sistema' as Descripción,
    FORMAT(GETDATE(), 'dd/MM/yyyy HH:mm') as Valor
UNION ALL
SELECT 
    'Períodos vencidos (deberían estar inactivos)',
    CAST(COUNT(*) AS VARCHAR(10))
FROM PeriodosAcademicos 
WHERE FechaFin < GETDATE() AND Activo = 1
UNION ALL
SELECT 
    'Períodos futuros marcados como activos',
    CAST(COUNT(*) AS VARCHAR(10))
FROM PeriodosAcademicos 
WHERE FechaInicio > GETDATE() AND Activo = 1
UNION ALL
SELECT 
    'Total períodos activos (debería ser 1)',
    CAST(COUNT(*) AS VARCHAR(10))
FROM PeriodosAcademicos 
WHERE Activo = 1;

PRINT '';

-- =====================================================================
-- 4. RECOMENDACIONES
-- =====================================================================

PRINT '4. RECOMENDACIONES:';
PRINT '------------------';

-- Verificar si hay períodos vencidos activos
IF EXISTS (SELECT 1 FROM PeriodosAcademicos WHERE FechaFin < GETDATE() AND Activo = 1)
BEGIN
    PRINT '⚠️  ADVERTENCIA: Hay períodos vencidos marcados como activos.';
    PRINT '   Recomendación: Desactivar períodos vencidos.';
    PRINT '';
END

-- Verificar si hay múltiples períodos activos
DECLARE @PeriodosActivos INT;
SELECT @PeriodosActivos = COUNT(*) FROM PeriodosAcademicos WHERE Activo = 1;

IF @PeriodosActivos > 1
BEGIN
    PRINT '⚠️  ADVERTENCIA: Hay múltiples períodos marcados como activos (' + CAST(@PeriodosActivos AS VARCHAR(10)) + ').';
    PRINT '   Recomendación: Solo un período debe estar activo a la vez.';
    PRINT '';
END
ELSE IF @PeriodosActivos = 0
BEGIN
    PRINT '⚠️  ADVERTENCIA: No hay ningún período activo.';
    PRINT '   Recomendación: Activar el período correspondiente a la fecha actual.';
    PRINT '';
END

-- Verificar período apropiado para la fecha actual
DECLARE @PeriodoApropiado NVARCHAR(100);
SELECT TOP 1 @PeriodoApropiado = NombreCompleto
FROM PeriodosAcademicos 
WHERE GETDATE() BETWEEN FechaInicio AND FechaFin;

IF @PeriodoApropiado IS NOT NULL AND @PeriodoApropiado != @PeriodoActivo
BEGIN
    PRINT '💡 SUGERENCIA: Según la fecha actual, el período activo debería ser: ' + @PeriodoApropiado;
    PRINT '';
END

-- =====================================================================
-- 5. SCRIPTS DE GESTIÓN COMUNES
-- =====================================================================

PRINT '============================================================';
PRINT '5. SCRIPTS DE GESTIÓN DISPONIBLES:';
PRINT '============================================================';
PRINT '';

PRINT '🔧 Para ACTIVAR el período correcto según la fecha actual:';
PRINT '   -- Desactivar todos los períodos';
PRINT '   UPDATE PeriodosAcademicos SET Activo = 0;';
PRINT '   -- Activar el período apropiado';
PRINT '   UPDATE PeriodosAcademicos SET Activo = 1 ';
PRINT '   WHERE GETDATE() BETWEEN FechaInicio AND FechaFin;';
PRINT '';

PRINT '🔧 Para ACTIVAR manualmente II Ciclo 2025:';
PRINT '   UPDATE PeriodosAcademicos SET Activo = 0;  -- Desactivar todos';
PRINT '   UPDATE PeriodosAcademicos SET Activo = 1   -- Activar específico';
PRINT '   WHERE Año = 2025 AND Ciclo = ''II'';';
PRINT '';

PRINT '🔧 Para FINALIZAR un período y activar el siguiente:';
PRINT '   UPDATE PeriodosAcademicos SET Activo = 0, Estado = ''Finalizado''';
PRINT '   WHERE Activo = 1;  -- Finalizar período actual';
PRINT '   UPDATE PeriodosAcademicos SET Activo = 1, Estado = ''Activo''';
PRINT '   WHERE Año = 2026 AND Ciclo = ''I'';  -- Activar siguiente';
PRINT '';

PRINT '🔧 Para VER solo el período activo:';
PRINT '   SELECT * FROM PeriodosAcademicos WHERE Activo = 1;';
PRINT '';

PRINT '============================================================';
PRINT '✅ VERIFICACIÓN COMPLETADA';
PRINT '============================================================';

-- Script automático para activar el período correcto si no hay ninguno activo
IF @PeriodosActivos = 0 AND @PeriodoApropiado IS NOT NULL
BEGIN
    PRINT '';
    PRINT '🤖 ACTIVACIÓN AUTOMÁTICA:';
    PRINT 'No hay período activo y se detectó que debería estar activo: ' + @PeriodoApropiado;
    PRINT 'Activando automáticamente...';
    
    UPDATE PeriodosAcademicos SET Activo = 1, Estado = 'Activo'
    WHERE GETDATE() BETWEEN FechaInicio AND FechaFin;
    
    PRINT '✅ Período activado automáticamente: ' + @PeriodoApropiado;
END

GO
