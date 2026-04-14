-- =====================================================================
-- SCRIPT PARA AGREGAR DATOS DE PRUEBA CON TIPOS VARIADOS
-- Sistema de Rúbricas - Base de Datos RubricasDb
-- =====================================================================
-- Este script agrega materias con diferentes tipos para probar el filtro
-- Solo ejecutar si no hay suficiente variedad de tipos en la base de datos
-- =====================================================================

USE [RubricasDb];
GO

PRINT 'Agregando materias de prueba con tipos variados...';
PRINT '';

-- =====================================================================
-- VERIFICAR SI YA EXISTEN MATERIAS CON TIPOS VARIADOS
-- =====================================================================
DECLARE @TiposExistentes INT;
SELECT @TiposExistentes = COUNT(DISTINCT Tipo)
FROM Materias 
WHERE Tipo IS NOT NULL AND Tipo != '';

PRINT 'Tipos únicos existentes: ' + CAST(@TiposExistentes AS VARCHAR(10));

-- Solo agregar datos si hay pocos tipos
IF @TiposExistentes < 3
BEGIN
    PRINT 'Agregando materias de prueba para demostrar el filtro...';
    PRINT '';

    -- =====================================================================
    -- MATERIAS DE TIPO "OBLIGATORIA"
    -- =====================================================================
    INSERT INTO Materias (Codigo, Nombre, Descripcion, Creditos, Tipo, CicloSugerido, Activa, FechaCreacion, Estado)
    VALUES 
    ('MAT-001', 'Matemáticas I', 'Matemáticas básicas para primer año', 4, 'Obligatoria', 1, 1, GETDATE(), 'Activo'),
    ('ESP-001', 'Español I', 'Lengua y Literatura española básica', 3, 'Obligatoria', 1, 1, GETDATE(), 'Activo'),
    ('CIE-001', 'Ciencias Naturales I', 'Introducción a las ciencias naturales', 3, 'Obligatoria', 1, 1, GETDATE(), 'Activo');

    -- =====================================================================
    -- MATERIAS DE TIPO "ELECTIVA"
    -- =====================================================================
    INSERT INTO Materias (Codigo, Nombre, Descripcion, Creditos, Tipo, CicloSugerido, Activa, FechaCreacion, Estado)
    VALUES 
    ('ART-001', 'Artes Plásticas', 'Desarrollo de habilidades artísticas', 2, 'Electiva', 2, 1, GETDATE(), 'Activo'),
    ('MUS-001', 'Educación Musical', 'Apreciación y práctica musical', 2, 'Electiva', 2, 1, GETDATE(), 'Activo'),
    ('TEA-001', 'Teatro y Drama', 'Expresión teatral y dramática', 2, 'Electiva', 3, 1, GETDATE(), 'Activo');

    -- =====================================================================
    -- MATERIAS DE TIPO "TÉCNICA"
    -- =====================================================================
    INSERT INTO Materias (Codigo, Nombre, Descripcion, Creditos, Tipo, CicloSugerido, Activa, FechaCreacion, Estado)
    VALUES 
    ('TEC-001', 'Informática Aplicada', 'Uso de herramientas informáticas', 3, 'Técnica', 2, 1, GETDATE(), 'Activo'),
    ('TEC-002', 'Diseño Gráfico', 'Fundamentos del diseño gráfico', 4, 'Técnica', 3, 1, GETDATE(), 'Activo'),
    ('TEC-003', 'Programación Básica', 'Introducción a la programación', 4, 'Técnica', 3, 1, GETDATE(), 'Activo');

    -- =====================================================================
    -- MATERIAS DE TIPO "COMPLEMENTARIA"
    -- =====================================================================
    INSERT INTO Materias (Codigo, Nombre, Descripcion, Creditos, Tipo, CicloSugerido, Activa, FechaCreacion, Estado)
    VALUES 
    ('EDF-001', 'Educación Física', 'Desarrollo físico y deportivo', 2, 'Complementaria', 1, 1, GETDATE(), 'Activo'),
    ('REL-001', 'Educación Religiosa', 'Formación en valores religiosos', 1, 'Complementaria', 1, 1, GETDATE(), 'Activo'),
    ('ORI-001', 'Orientación Vocacional', 'Guía para la elección profesional', 1, 'Complementaria', 4, 1, GETDATE(), 'Activo');

    PRINT '✅ Se agregaron materias de prueba con los siguientes tipos:';
    PRINT '   - Obligatoria (3 materias)';
    PRINT '   - Electiva (3 materias)';
    PRINT '   - Técnica (3 materias)';
    PRINT '   - Complementaria (3 materias)';
    PRINT '';
END
ELSE
BEGIN
    PRINT '✅ Ya existen suficientes tipos de materias para probar el filtro.';
    PRINT 'No es necesario agregar datos de prueba.';
    PRINT '';
END

-- =====================================================================
-- VERIFICAR RESULTADOS
-- =====================================================================
PRINT '=====================================================';
PRINT 'TIPOS DE MATERIAS DISPONIBLES PARA FILTRAR:';
PRINT '=====================================================';

SELECT 
    Tipo,
    COUNT(*) as CantidadMaterias,
    CONCAT(
        'Filtro: ',
        'https://localhost:18163/Materias?tipoFiltro=',
        REPLACE(Tipo, ' ', '%20')
    ) as UrlEjemplo
FROM Materias 
WHERE Tipo IS NOT NULL AND Tipo != ''
GROUP BY Tipo 
ORDER BY Tipo;

PRINT '';
PRINT '🎯 Datos listos para probar el filtro!';
PRINT 'Puedes usar las URLs de ejemplo mostradas arriba para probar cada filtro.';
GO
