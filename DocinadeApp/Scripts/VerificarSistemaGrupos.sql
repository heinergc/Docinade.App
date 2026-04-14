-- Script para verificar el estado actual del sistema de grupos
USE RubricasDb;

-- 1. Verificar tabla de migraciones
PRINT '=== TABLA DE MIGRACIONES ===';
SELECT MigrationId, ProductVersion 
FROM [__EFMigrationsHistory] 
WHERE MigrationId LIKE '%Grupos%'
ORDER BY MigrationId;

-- 2. Verificar estructura de tablas
PRINT '';
PRINT '=== ESTRUCTURA DE TABLAS ===';
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'GruposEstudiantes')
    PRINT '? Tabla GruposEstudiantes existe';
ELSE
    PRINT '? Tabla GruposEstudiantes NO existe';

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'EstudianteGrupos')
    PRINT '? Tabla EstudianteGrupos existe';
ELSE
    PRINT '? Tabla EstudianteGrupos NO existe';

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'GrupoMaterias')
    PRINT '? Tabla GrupoMaterias existe';
ELSE
    PRINT '? Tabla GrupoMaterias NO existe';

-- 3. Verificar contenido de las tablas
PRINT '';
PRINT '=== CONTENIDO DE TABLAS ===';
SELECT 'GruposEstudiantes' AS Tabla, COUNT(*) AS Registros FROM GruposEstudiantes
UNION ALL
SELECT 'EstudianteGrupos' AS Tabla, COUNT(*) AS Registros FROM EstudianteGrupos
UNION ALL
SELECT 'GrupoMaterias' AS Tabla, COUNT(*) AS Registros FROM GrupoMaterias;

-- 4. Verificar períodos académicos disponibles
PRINT '';
PRINT '=== PERÍODOS ACADÉMICOS ===';
SELECT Id, Nombre, Estado, FechaInicio, FechaFin 
FROM PeriodosAcademicos 
ORDER BY FechaInicio DESC;

-- 5. Verificar grupos si existen
PRINT '';
PRINT '=== GRUPOS EXISTENTES ===';
IF EXISTS (SELECT 1 FROM GruposEstudiantes)
BEGIN
    SELECT 
        GrupoId,
        Codigo,
        Nombre,
        TipoGrupo,
        Estado,
        PeriodoAcademicoId,
        FechaCreacion
    FROM GruposEstudiantes
    ORDER BY FechaCreacion DESC;
END
ELSE
BEGIN
    PRINT 'No hay grupos registrados en la tabla GruposEstudiantes';
END

-- 6. Verificar estudiantes disponibles
PRINT '';
PRINT '=== ESTUDIANTES DISPONIBLES ===';
SELECT COUNT(*) AS TotalEstudiantes FROM Estudiantes;

-- 7. Verificar materias disponibles
PRINT '';
PRINT '=== MATERIAS DISPONIBLES ===';
SELECT COUNT(*) AS TotalMaterias FROM Materias WHERE Activa = 1;

PRINT '';
PRINT '=== VERIFICACIÓN COMPLETADA ===';
PRINT 'Si todas las tablas existen y hay al menos un período académico activo,';
PRINT 'el sistema de grupos debería funcionar correctamente.';