-- Script para crear las tablas del sistema de grupos de estudiantes
-- Este script debe ejecutarse en la base de datos SQL Server

-- 1. Tabla GruposEstudiantes
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'GruposEstudiantes')
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
        [Estado] [nvarchar](20) NOT NULL DEFAULT 'Activo',
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
    
    -- Índices para optimización
    CREATE UNIQUE INDEX [IX_GruposEstudiantes_Codigo_Periodo] 
        ON [GruposEstudiantes] ([Codigo], [PeriodoAcademicoId]);
    CREATE INDEX [IX_GruposEstudiantes_Estado] ON [GruposEstudiantes] ([Estado]);
    CREATE INDEX [IX_GruposEstudiantes_TipoGrupo] ON [GruposEstudiantes] ([TipoGrupo]);
    
    PRINT 'Tabla GruposEstudiantes creada exitosamente.';
END
ELSE
BEGIN
    PRINT 'Tabla GruposEstudiantes ya existe.';
END

-- 2. Tabla EstudianteGrupos
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'EstudianteGrupos')
BEGIN
    CREATE TABLE [dbo].[EstudianteGrupos](
        [Id] [int] IDENTITY(1,1) NOT NULL,
        [EstudianteId] [int] NOT NULL,
        [GrupoId] [int] NOT NULL,
        [FechaAsignacion] [datetime2](7) NOT NULL DEFAULT GETDATE(),
        [FechaDesasignacion] [datetime2](7) NULL,
        [Estado] [nvarchar](20) NOT NULL DEFAULT 'Activo',
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
    
    -- Índices
    CREATE INDEX [IX_EstudianteGrupos_GrupoId] ON [EstudianteGrupos] ([GrupoId]);
    CREATE INDEX [IX_EstudianteGrupos_FechaAsignacion] ON [EstudianteGrupos] ([FechaAsignacion]);
    CREATE INDEX [IX_EstudianteGrupos_Estado] ON [EstudianteGrupos] ([Estado]);
    
    -- Índice único para evitar duplicados activos (usando índice filtrado)
    CREATE UNIQUE INDEX [IX_EstudianteGrupos_Unique_Activo] 
        ON [EstudianteGrupos] ([EstudianteId], [GrupoId])
        WHERE [Estado] = 'Activo';
    
    PRINT 'Tabla EstudianteGrupos creada exitosamente.';
END
ELSE
BEGIN
    PRINT 'Tabla EstudianteGrupos ya existe.';
END

-- 3. Tabla GrupoMaterias
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'GrupoMaterias')
BEGIN
    CREATE TABLE [dbo].[GrupoMaterias](
        [Id] [int] IDENTITY(1,1) NOT NULL,
        [GrupoId] [int] NOT NULL,
        [MateriaId] [int] NOT NULL,
        [FechaAsignacion] [datetime2](7) NOT NULL DEFAULT GETDATE(),
        [Estado] [nvarchar](20) NOT NULL DEFAULT 'Activo',
        [Observaciones] [nvarchar](500) NULL,
        
        CONSTRAINT [PK_GrupoMaterias] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_GrupoMaterias_GruposEstudiantes] 
            FOREIGN KEY ([GrupoId]) REFERENCES [GruposEstudiantes] ([GrupoId]),
        CONSTRAINT [FK_GrupoMaterias_Materias] 
            FOREIGN KEY ([MateriaId]) REFERENCES [Materias] ([MateriaId])
    );
    
    -- Índice único para evitar duplicados
    CREATE UNIQUE INDEX [IX_GrupoMaterias_Unique] 
        ON [GrupoMaterias] ([GrupoId], [MateriaId]);
    
    PRINT 'Tabla GrupoMaterias creada exitosamente.';
END
ELSE
BEGIN
    PRINT 'Tabla GrupoMaterias ya existe.';
END

-- 4. Insertar datos de ejemplo (opcional)
-- Verificar si ya hay datos para evitar duplicados
IF NOT EXISTS (SELECT 1 FROM GruposEstudiantes)
BEGIN
    -- Insertar grupos de ejemplo solo si no existen datos
    DECLARE @PeriodoId INT;
    SELECT TOP 1 @PeriodoId = Id FROM PeriodosAcademicos WHERE Estado = 'Activo';
    
    IF @PeriodoId IS NOT NULL
    BEGIN
        INSERT INTO GruposEstudiantes (Codigo, Nombre, Descripcion, TipoGrupo, Nivel, CapacidadMaxima, PeriodoAcademicoId, Estado)
        VALUES 
        ('7-1', 'Séptimo Sección 1', 'Grupo de séptimo año, sección 1', 'Seccion', 'SEPTIMO', 30, @PeriodoId, 'Activo'),
        ('7-2', 'Séptimo Sección 2', 'Grupo de séptimo año, sección 2', 'Seccion', 'SEPTIMO', 28, @PeriodoId, 'Activo'),
        ('8-A', 'Octavo Sección A', 'Grupo de octavo año, sección A', 'Seccion', 'OCTAVO', 32, @PeriodoId, 'Activo'),
        ('G1', 'Grupo 1', 'Grupo general número 1', 'Custom', NULL, 25, @PeriodoId, 'Activo'),
        ('VIRT-A', 'Virtual Grupo A', 'Grupo modalidad virtual A', 'Modalidad', 'VIRTUAL', 50, @PeriodoId, 'Activo');
        
        PRINT 'Datos de ejemplo insertados en GruposEstudiantes.';
    END
    ELSE
    BEGIN
        PRINT 'No se encontró un período académico activo para insertar datos de ejemplo.';
    END
END
ELSE
BEGIN
    PRINT 'Ya existen datos en GruposEstudiantes, no se insertan datos de ejemplo.';
END

PRINT 'Script completado exitosamente.';