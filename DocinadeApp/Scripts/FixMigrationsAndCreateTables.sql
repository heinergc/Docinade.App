-- Script para marcar las migraciones como aplicadas en la base de datos existente
USE RubricasDb;

-- Verificar si existe la tabla de historial de migraciones
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '__EFMigrationsHistory')
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
    PRINT 'Tabla __EFMigrationsHistory creada.';
END

-- Marcar la migración inicial como aplicada si no está registrada
IF NOT EXISTS (SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = '20250828040336_InitialCreateSqlServer')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES ('20250828040336_InitialCreateSqlServer', '8.0.8');
    PRINT 'Migración inicial marcada como aplicada.';
END
ELSE
BEGIN
    PRINT 'Migración inicial ya está registrada.';
END

-- Ejecutar el script de creación de tablas de grupos
EXEC('
-- 1. Tabla GruposEstudiantes
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = ''GruposEstudiantes'')
BEGIN
    CREATE TABLE [dbo].[GruposEstudiantes](
        [GrupoId] [int] IDENTITY(1,1) NOT NULL,
        [Codigo] [nvarchar](20) NOT NULL,
        [Nombre] [nvarchar](100) NOT NULL,
        [Descripcion] [nvarchar](500) NULL,
        [TipoGrupo] [nvarchar](50) NOT NULL,
        [Nivel] [nvarchar](50) NULL,
        [CapacidadMaxima] [int] NULL,
        [PeriodoAcademicoId] [int] NOT NULL,
        [Estado] [nvarchar](20) NOT NULL DEFAULT ''Activo'',
        [FechaCreacion] [datetime2](7) NOT NULL DEFAULT GETDATE(),
        [FechaModificacion] [datetime2](7) NULL,
        [CreadoPorId] [nvarchar](450) NULL,
        [Observaciones] [nvarchar](1000) NULL,
        
        CONSTRAINT [PK_GruposEstudiantes] PRIMARY KEY ([GrupoId]),
        CONSTRAINT [FK_GruposEstudiantes_PeriodosAcademicos] 
            FOREIGN KEY ([PeriodoAcademicoId]) REFERENCES [PeriodosAcademicos] ([Id]),
        CONSTRAINT [FK_GruposEstudiantes_AspNetUsers] 
            FOREIGN KEY ([CreadoPorId]) REFERENCES [AspNetUsers] ([Id])
    );
    
    CREATE UNIQUE INDEX [IX_GruposEstudiantes_Codigo_Periodo] 
        ON [GruposEstudiantes] ([Codigo], [PeriodoAcademicoId]);
    CREATE INDEX [IX_GruposEstudiantes_Estado] ON [GruposEstudiantes] ([Estado]);
    CREATE INDEX [IX_GruposEstudiantes_TipoGrupo] ON [GruposEstudiantes] ([TipoGrupo]);
    
    PRINT ''Tabla GruposEstudiantes creada exitosamente.'';
END
ELSE
BEGIN
    PRINT ''Tabla GruposEstudiantes ya existe.'';
END

-- 2. Tabla EstudianteGrupos
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = ''EstudianteGrupos'')
BEGIN
    CREATE TABLE [dbo].[EstudianteGrupos](
        [Id] [int] IDENTITY(1,1) NOT NULL,
        [EstudianteId] [int] NOT NULL,
        [GrupoId] [int] NOT NULL,
        [FechaAsignacion] [datetime2](7) NOT NULL DEFAULT GETDATE(),
        [FechaDesasignacion] [datetime2](7) NULL,
        [Estado] [nvarchar](20) NOT NULL DEFAULT ''Activo'',
        [AsignadoPorId] [nvarchar](450) NULL,
        [MotivoAsignacion] [nvarchar](200) NULL,
        [EsGrupoPrincipal] [bit] NOT NULL DEFAULT 1,
        
        CONSTRAINT [PK_EstudianteGrupos] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_EstudianteGrupos_Estudiantes] 
            FOREIGN KEY ([EstudianteId]) REFERENCES [Estudiantes] ([IdEstudiante]),
        CONSTRAINT [FK_EstudianteGrupos_GruposEstudiantes] 
            FOREIGN KEY ([GrupoId]) REFERENCES [GruposEstudiantes] ([GrupoId]),
        CONSTRAINT [FK_EstudianteGrupos_AspNetUsers] 
            FOREIGN KEY ([AsignadoPorId]) REFERENCES [AspNetUsers] ([Id])
    );
    
    CREATE INDEX [IX_EstudianteGrupos_GrupoId] ON [EstudianteGrupos] ([GrupoId]);
    CREATE INDEX [IX_EstudianteGrupos_FechaAsignacion] ON [EstudianteGrupos] ([FechaAsignacion]);
    CREATE INDEX [IX_EstudianteGrupos_Estado] ON [EstudianteGrupos] ([Estado]);
    
    CREATE UNIQUE INDEX [IX_EstudianteGrupos_Unique_Activo] 
        ON [EstudianteGrupos] ([EstudianteId], [GrupoId])
        WHERE [Estado] = ''Activo'';
    
    PRINT ''Tabla EstudianteGrupos creada exitosamente.'';
END
ELSE
BEGIN
    PRINT ''Tabla EstudianteGrupos ya existe.'';
END

-- 3. Tabla GrupoMaterias
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = ''GrupoMaterias'')
BEGIN
    CREATE TABLE [dbo].[GrupoMaterias](
        [Id] [int] IDENTITY(1,1) NOT NULL,
        [GrupoId] [int] NOT NULL,
        [MateriaId] [int] NOT NULL,
        [FechaAsignacion] [datetime2](7) NOT NULL DEFAULT GETDATE(),
        [Estado] [nvarchar](20) NOT NULL DEFAULT ''Activo'',
        [Observaciones] [nvarchar](500) NULL,
        
        CONSTRAINT [PK_GrupoMaterias] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_GrupoMaterias_GruposEstudiantes] 
            FOREIGN KEY ([GrupoId]) REFERENCES [GruposEstudiantes] ([GrupoId]),
        CONSTRAINT [FK_GrupoMaterias_Materias] 
            FOREIGN KEY ([MateriaId]) REFERENCES [Materias] ([MateriaId])
    );
    
    CREATE UNIQUE INDEX [IX_GrupoMaterias_Unique] 
        ON [GrupoMaterias] ([GrupoId], [MateriaId]);
    
    PRINT ''Tabla GrupoMaterias creada exitosamente.'';
END
ELSE
BEGIN
    PRINT ''Tabla GrupoMaterias ya existe.'';
END
');

PRINT 'Script completado exitosamente.';