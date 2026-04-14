-- =====================================================================
-- SCRIPT SIMPLIFICADO - PERÍODOS ACADÉMICOS MEP COSTA RICA
-- Sistema de Rúbricas - Base de Datos RubricasDb
-- =====================================================================
-- Versión simplificada para insertar solo los períodos esenciales
-- =====================================================================

USE [RubricasDb];
GO

PRINT '📚 Insertando períodos académicos esenciales MEP Costa Rica...';
PRINT '';

-- =====================================================================
-- PERÍODOS ACADÉMICOS ESENCIALES
-- =====================================================================

-- Asegurar que no hay períodos activos antes de insertar
UPDATE PeriodosAcademicos SET Activo = 0 WHERE Activo = 1;

-- I CICLO 2025 (FINALIZADO)
INSERT INTO PeriodosAcademicos (Año, Anio, Ciclo, FechaInicio, FechaFin, Activo, Codigo, Nombre, Tipo, NumeroPeriodo, FechaCreacion, Estado)
SELECT 2025, 2025, 'I', '2025-02-03', '2025-07-18', 0, '2025-I', 'Primer Ciclo 2025', 1, 1, GETDATE(), 'Finalizado'
WHERE NOT EXISTS (SELECT 1 FROM PeriodosAcademicos WHERE Año = 2025 AND Ciclo = 'I');

-- II CICLO 2025 (ACTIVO ACTUAL)
INSERT INTO PeriodosAcademicos (Año, Anio, Ciclo, FechaInicio, FechaFin, Activo, Codigo, Nombre, Tipo, NumeroPeriodo, FechaCreacion, Estado)
SELECT 2025, 2025, 'II', '2025-08-05', '2025-12-20', 1, '2025-II', 'Segundo Ciclo 2025', 1, 2, GETDATE(), 'Activo'
WHERE NOT EXISTS (SELECT 1 FROM PeriodosAcademicos WHERE Año = 2025 AND Ciclo = 'II');

-- I CICLO 2026 (PRÓXIMO)
INSERT INTO PeriodosAcademicos (Año, Anio, Ciclo, FechaInicio, FechaFin, Activo, Codigo, Nombre, Tipo, NumeroPeriodo, FechaCreacion, Estado)
SELECT 2026, 2026, 'I', '2026-02-02', '2026-07-17', 0, '2026-I', 'Primer Ciclo 2026', 1, 1, GETDATE(), 'Planificado'
WHERE NOT EXISTS (SELECT 1 FROM PeriodosAcademicos WHERE Año = 2026 AND Ciclo = 'I');

-- II CICLO 2026 (FUTURO)
INSERT INTO PeriodosAcademicos (Año, Anio, Ciclo, FechaInicio, FechaFin, Activo, Codigo, Nombre, Tipo, NumeroPeriodo, FechaCreacion, Estado)
SELECT 2026, 2026, 'II', '2026-08-03', '2026-12-18', 0, '2026-II', 'Segundo Ciclo 2026', 1, 2, GETDATE(), 'Planificado'
WHERE NOT EXISTS (SELECT 1 FROM PeriodosAcademicos WHERE Año = 2026 AND Ciclo = 'II');

PRINT '✅ Períodos académicos esenciales insertados.';

-- Verificación
SELECT 
    NombreCompleto as 'Período Académico',
    FORMAT(FechaInicio, 'dd/MM/yyyy') as Inicio,
    FORMAT(FechaFin, 'dd/MM/yyyy') as Fin,
    CASE WHEN Activo = 1 THEN '🟢 ACTIVO' ELSE Estado END as Estado
FROM PeriodosAcademicos 
WHERE Año >= 2025
ORDER BY Año, CASE WHEN Ciclo = 'I' THEN 1 ELSE 2 END;

PRINT '';
PRINT '🎯 Período actual activo: II Ciclo 2025';
GO
