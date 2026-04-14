-- ================================================
-- Script: Agregar columna IdLeccion a tabla Asistencias
-- Fecha: 2025-11-14
-- Descripción: Extensión de Asistencias para vincular con Lecciones específicas (Módulo MEP)
-- ================================================

-- Agregar columna IdLeccion
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Asistencias]') AND name = 'IdLeccion')
BEGIN
    ALTER TABLE [dbo].[Asistencias]
    ADD [IdLeccion] [int] NULL
    
    PRINT 'Columna IdLeccion agregada a tabla Asistencias'
END
ELSE
BEGIN
    PRINT 'La columna IdLeccion ya existe en tabla Asistencias'
END
GO

-- Crear relación de clave foránea
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Asistencias_Lecciones_IdLeccion]'))
BEGIN
    ALTER TABLE [dbo].[Asistencias] WITH CHECK 
    ADD CONSTRAINT [FK_Asistencias_Lecciones_IdLeccion] 
    FOREIGN KEY([IdLeccion]) REFERENCES [dbo].[Lecciones] ([IdLeccion])
    ON DELETE NO ACTION
    
    PRINT 'FK Asistencias -> Lecciones creada'
END
GO

-- Eliminar índice único antiguo si existe
IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Asistencias_Unique_Estudiante_Grupo_Materia_Fecha' AND object_id = OBJECT_ID('Asistencias'))
BEGIN
    DROP INDEX [IX_Asistencias_Unique_Estudiante_Grupo_Materia_Fecha] ON [dbo].[Asistencias]
    PRINT 'Índice único antiguo eliminado'
END
GO

-- Crear nuevo índice único para registros con lección especificada
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Asistencias_Unique_Estudiante_Grupo_Leccion_Fecha' AND object_id = OBJECT_ID('Asistencias'))
BEGIN
    SET QUOTED_IDENTIFIER ON;
    SET ANSI_NULLS ON;
    
    CREATE UNIQUE NONCLUSTERED INDEX [IX_Asistencias_Unique_Estudiante_Grupo_Leccion_Fecha] 
    ON [dbo].[Asistencias]([EstudianteId] ASC, [GrupoId] ASC, [IdLeccion] ASC, [Fecha] ASC)
    WHERE [IdLeccion] IS NOT NULL
    
    PRINT 'Índice único para asistencias con lección creado'
END
GO

-- Crear índice para registros antiguos sin lección
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Asistencias_Estudiante_Grupo_Materia_Fecha' AND object_id = OBJECT_ID('Asistencias'))
BEGIN
    SET QUOTED_IDENTIFIER ON;
    SET ANSI_NULLS ON;
    
    CREATE NONCLUSTERED INDEX [IX_Asistencias_Estudiante_Grupo_Materia_Fecha] 
    ON [dbo].[Asistencias]([EstudianteId] ASC, [GrupoId] ASC, [MateriaId] ASC, [Fecha] ASC)
    WHERE [IdLeccion] IS NULL
    
    PRINT 'Índice para asistencias sin lección (compatibilidad) creado'
END
GO

-- Crear índice en IdLeccion
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Asistencias_IdLeccion' AND object_id = OBJECT_ID('Asistencias'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Asistencias_IdLeccion] 
    ON [dbo].[Asistencias]([IdLeccion] ASC)
    
    PRINT 'Índice IX_Asistencias_IdLeccion creado'
END
GO

PRINT '✅ Script de extensión de tabla Asistencias completado exitosamente'
GO
