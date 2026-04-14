-- ================================================
-- Script: Crear tabla Lecciones para Módulo MEP
-- Fecha: 2025-11-14
-- Descripción: Implementación de horarios por bloque/lección según especificación MEP
-- ================================================

-- Verificar si la tabla ya existe
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Lecciones]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Lecciones](
        [IdLeccion] [int] IDENTITY(1,1) NOT NULL,
        [IdGrupo] [int] NOT NULL,
        [MateriaId] [int] NOT NULL,
        [NumeroBloque] [int] NOT NULL,
        [DiaSemana] [int] NOT NULL,
        [HoraInicio] [time](7) NOT NULL,
        [HoraFin] [time](7) NOT NULL,
        [Activa] [bit] NOT NULL DEFAULT 1,
        [Observaciones] [nvarchar](500) NULL,
        [FechaCreacion] [datetime2](7) NOT NULL DEFAULT GETDATE(),
        [FechaModificacion] [datetime2](7) NULL,
        CONSTRAINT [PK_Lecciones] PRIMARY KEY CLUSTERED ([IdLeccion] ASC)
    )
    
    PRINT 'Tabla Lecciones creada exitosamente'
END
ELSE
BEGIN
    PRINT 'La tabla Lecciones ya existe'
END
GO

-- Crear relaciones de clave foránea
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Lecciones_GruposEstudiantes_IdGrupo]'))
BEGIN
    ALTER TABLE [dbo].[Lecciones] WITH CHECK 
    ADD CONSTRAINT [FK_Lecciones_GruposEstudiantes_IdGrupo] 
    FOREIGN KEY([IdGrupo]) REFERENCES [dbo].[GruposEstudiantes] ([GrupoId])
    ON DELETE NO ACTION
    
    PRINT 'FK Lecciones -> GruposEstudiantes creada'
END
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Lecciones_Materias_MateriaId]'))
BEGIN
    ALTER TABLE [dbo].[Lecciones] WITH CHECK 
    ADD CONSTRAINT [FK_Lecciones_Materias_MateriaId] 
    FOREIGN KEY([MateriaId]) REFERENCES [dbo].[Materias] ([MateriaId])
    ON DELETE NO ACTION
    
    PRINT 'FK Lecciones -> Materias creada'
END
GO

-- Crear índices para optimización
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Lecciones_IdGrupo' AND object_id = OBJECT_ID('Lecciones'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Lecciones_IdGrupo] 
    ON [dbo].[Lecciones]([IdGrupo] ASC)
    
    PRINT 'Índice IX_Lecciones_IdGrupo creado'
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Lecciones_MateriaId' AND object_id = OBJECT_ID('Lecciones'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Lecciones_MateriaId] 
    ON [dbo].[Lecciones]([MateriaId] ASC)
    
    PRINT 'Índice IX_Lecciones_MateriaId creado'
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Lecciones_DiaSemana' AND object_id = OBJECT_ID('Lecciones'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Lecciones_DiaSemana] 
    ON [dbo].[Lecciones]([DiaSemana] ASC)
    
    PRINT 'Índice IX_Lecciones_DiaSemana creado'
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Lecciones_NumeroBloque' AND object_id = OBJECT_ID('Lecciones'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Lecciones_NumeroBloque] 
    ON [dbo].[Lecciones]([NumeroBloque] ASC)
    
    PRINT 'Índice IX_Lecciones_NumeroBloque creado'
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Lecciones_Activa' AND object_id = OBJECT_ID('Lecciones'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Lecciones_Activa] 
    ON [dbo].[Lecciones]([Activa] ASC)
    
    PRINT 'Índice IX_Lecciones_Activa creado'
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Lecciones_Grupo_Dia_Hora' AND object_id = OBJECT_ID('Lecciones'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Lecciones_Grupo_Dia_Hora] 
    ON [dbo].[Lecciones]([IdGrupo] ASC, [DiaSemana] ASC, [HoraInicio] ASC)
    
    PRINT 'Índice compuesto IX_Lecciones_Grupo_Dia_Hora creado'
END
GO

-- Crear restricción única para evitar duplicados
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Lecciones_Unique_Grupo_Materia_Dia_Bloque' AND object_id = OBJECT_ID('Lecciones'))
BEGIN
    CREATE UNIQUE NONCLUSTERED INDEX [IX_Lecciones_Unique_Grupo_Materia_Dia_Bloque] 
    ON [dbo].[Lecciones]([IdGrupo] ASC, [MateriaId] ASC, [DiaSemana] ASC, [NumeroBloque] ASC)
    
    PRINT 'Restricción única IX_Lecciones_Unique_Grupo_Materia_Dia_Bloque creada'
END
GO

PRINT '✅ Script de creación de tabla Lecciones completado exitosamente'
GO
