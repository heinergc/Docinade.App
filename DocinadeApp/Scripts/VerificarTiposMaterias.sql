-- =====================================================================
-- SCRIPT PARA VERIFICAR TIPOS DE MATERIAS EXISTENTES
-- Sistema de Rúbricas - Base de Datos RubricasDb
-- =====================================================================
-- Este script muestra los tipos únicos de materias en la base de datos
-- y permite verificar que existen datos para probar el filtro
-- =====================================================================

USE [RubricasDb];
GO

PRINT 'Verificando tipos de materias existentes...';
PRINT '';

-- =====================================================================
-- 1. MOSTRAR TIPOS ÚNICOS DE MATERIAS (Agrupados)
-- =====================================================================
PRINT '1. TIPOS ÚNICOS DE MATERIAS:';
PRINT '========================================';

SELECT 
    Tipo,
    COUNT(*) as CantidadMaterias
FROM Materias 
WHERE Tipo IS NOT NULL AND Tipo != ''
GROUP BY Tipo 
ORDER BY Tipo;

PRINT '';

-- =====================================================================
-- 2. MOSTRAR TODAS LAS MATERIAS CON SUS TIPOS
-- =====================================================================
PRINT '2. LISTA DETALLADA DE MATERIAS POR TIPO:';
PRINT '========================================';

SELECT 
    Tipo,
    Codigo,
    Nombre,
    Creditos,
    CASE 
        WHEN Activa = 1 THEN 'Activa'
        ELSE 'Inactiva'
    END as Estado
FROM Materias 
ORDER BY Tipo, Nombre;

PRINT '';

-- =====================================================================
-- 3. ESTADÍSTICAS GENERALES
-- =====================================================================
PRINT '3. ESTADÍSTICAS GENERALES:';
PRINT '========================================';

SELECT 
    'Total de materias' as Descripcion,
    COUNT(*) as Cantidad
FROM Materias
UNION ALL
SELECT 
    'Materias con tipo definido',
    COUNT(*)
FROM Materias 
WHERE Tipo IS NOT NULL AND Tipo != ''
UNION ALL
SELECT 
    'Materias sin tipo',
    COUNT(*)
FROM Materias 
WHERE Tipo IS NULL OR Tipo = ''
UNION ALL
SELECT 
    'Tipos únicos',
    COUNT(DISTINCT Tipo)
FROM Materias 
WHERE Tipo IS NOT NULL AND Tipo != '';

PRINT '';

-- =====================================================================
-- 4. QUERY PARA EL FILTRO (Similar al que usa la aplicación)
-- =====================================================================
PRINT '4. QUERY DE EJEMPLO PARA FILTRO:';
PRINT '========================================';

-- Ejemplo: Filtrar materias de tipo 'Obligatoria' (si existe)
DECLARE @TipoEjemplo VARCHAR(50);
SELECT TOP 1 @TipoEjemplo = Tipo 
FROM Materias 
WHERE Tipo IS NOT NULL AND Tipo != ''
GROUP BY Tipo 
ORDER BY COUNT(*) DESC;

IF @TipoEjemplo IS NOT NULL
BEGIN
    PRINT 'Filtrando materias de tipo: ' + @TipoEjemplo;
    PRINT '';
    
    SELECT 
        Codigo,
        Nombre,
        Tipo,
        Creditos,
        CASE 
            WHEN Activa = 1 THEN 'Activa'
            ELSE 'Inactiva'
        END as Estado
    FROM Materias 
    WHERE Tipo = @TipoEjemplo
    ORDER BY Nombre;
END
ELSE
BEGIN
    PRINT 'No se encontraron tipos de materias para filtrar.';
END

PRINT '';
PRINT '🎯 Script de verificación completado!';
PRINT 'Los datos mostrados arriba se pueden usar para probar el filtro en la aplicación.';
