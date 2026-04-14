-- Marcar la migración de SistemaGruposEstudiantes como aplicada
USE RubricasDb;

-- Verificar si la migración ya está marcada
IF NOT EXISTS (SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = '20250830191805_SistemaGruposEstudiantes')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES ('20250830191805_SistemaGruposEstudiantes', '8.0.8');
    PRINT 'Migración SistemaGruposEstudiantes marcada como aplicada.';
END
ELSE
BEGIN
    PRINT 'La migración SistemaGruposEstudiantes ya está marcada como aplicada.';
END

-- Verificar que las tablas existan
PRINT 'Verificando existencia de tablas...';

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

PRINT 'Script completado.';