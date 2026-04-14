-- Script para crear la tabla de auditoría manualmente
-- Ejecutar este script en la base de datos si las migraciones fallan

USE [RubricasDbNueva]; -- Cambia el nombre de la base de datos según corresponda
GO

-- Crear tabla AuditoriasOperaciones si no existe
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='AuditoriasOperaciones' AND xtype='U')
BEGIN
    CREATE TABLE [dbo].[AuditoriasOperaciones] (
        [Id] int IDENTITY(1,1) NOT NULL,
        [TipoOperacion] nvarchar(20) NOT NULL,
        [TablaAfectada] nvarchar(100) NOT NULL,
        [RegistroId] int NOT NULL,
        [Descripcion] nvarchar(500) NOT NULL,
        [Motivo] nvarchar(1000) NULL,
        [DatosAnteriores] nvarchar(max) NULL,
        [DatosNuevos] nvarchar(max) NULL,
        [DireccionIP] nvarchar(45) NULL,
        [UserAgent] nvarchar(500) NULL,
        [FechaOperacion] datetime2 NOT NULL DEFAULT (GETDATE()),
        [UsuarioId] nvarchar(450) NOT NULL,
        [OperacionExitosa] bit NOT NULL DEFAULT 1,
        [MensajeError] nvarchar(1000) NULL,
        CONSTRAINT [PK_AuditoriasOperaciones] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_AuditoriasOperaciones_AspNetUsers_UsuarioId] FOREIGN KEY([UsuarioId]) REFERENCES [dbo].[AspNetUsers] ([Id])
    );

    -- Crear índices para optimizar las consultas
    CREATE NONCLUSTERED INDEX [IX_AuditoriaOperacion_TablaAfectada] ON [dbo].[AuditoriasOperaciones]
    (
        [TablaAfectada] ASC
    );

    CREATE NONCLUSTERED INDEX [IX_AuditoriaOperacion_RegistroId] ON [dbo].[AuditoriasOperaciones]
    (
        [RegistroId] ASC
    );

    CREATE NONCLUSTERED INDEX [IX_AuditoriaOperacion_FechaOperacion] ON [dbo].[AuditoriasOperaciones]
    (
        [FechaOperacion] ASC
    );

    CREATE NONCLUSTERED INDEX [IX_AuditoriaOperacion_UsuarioId] ON [dbo].[AuditoriasOperaciones]
    (
        [UsuarioId] ASC
    );

    CREATE NONCLUSTERED INDEX [IX_AuditoriaOperacion_Tabla_Registro] ON [dbo].[AuditoriasOperaciones]
    (
        [TablaAfectada] ASC,
        [RegistroId] ASC
    );

    PRINT 'Tabla AuditoriasOperaciones creada exitosamente con todos los índices.';
END
ELSE
BEGIN
    PRINT 'La tabla AuditoriasOperaciones ya existe.';
END
GO
