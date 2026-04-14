-- Script para crear la tabla AuditoriasOperaciones
-- Ejecutar este script en la base de datos

CREATE TABLE [dbo].[AuditoriasOperaciones] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [TipoOperacion] NVARCHAR(50) NOT NULL,
    [TablaAfectada] NVARCHAR(100) NOT NULL,
    [RegistroId] INT NOT NULL,
    [Descripcion] NVARCHAR(500) NOT NULL,
    [Motivo] NVARCHAR(500) NULL,
    [UsuarioId] NVARCHAR(450) NOT NULL,
    [DireccionIP] NVARCHAR(45) NULL,
    [UserAgent] NVARCHAR(500) NULL,
    [FechaOperacion] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [OperacionExitosa] BIT NOT NULL DEFAULT 1,
    [MensajeError] NVARCHAR(1000) NULL,
    [DatosAnteriores] NTEXT NULL,
    [DatosNuevos] NTEXT NULL,
    
    CONSTRAINT [PK_AuditoriasOperaciones] PRIMARY KEY CLUSTERED ([Id] ASC)
);

-- Crear índices para optimizar consultas
CREATE NONCLUSTERED INDEX [IX_AuditoriasOperaciones_FechaOperacion] 
ON [dbo].[AuditoriasOperaciones] ([FechaOperacion] DESC);

CREATE NONCLUSTERED INDEX [IX_AuditoriasOperaciones_TablaAfectada] 
ON [dbo].[AuditoriasOperaciones] ([TablaAfectada] ASC);

CREATE NONCLUSTERED INDEX [IX_AuditoriasOperaciones_UsuarioId] 
ON [dbo].[AuditoriasOperaciones] ([UsuarioId] ASC);

CREATE NONCLUSTERED INDEX [IX_AuditoriasOperaciones_Tabla_Registro] 
ON [dbo].[AuditoriasOperaciones] ([TablaAfectada] ASC, [RegistroId] ASC);

-- Crear foreign key con AspNetUsers si existe
-- (Opcional, dependiendo de si quieres integridad referencial estricta)
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'AspNetUsers')
BEGIN
    ALTER TABLE [dbo].[AuditoriasOperaciones]
    ADD CONSTRAINT [FK_AuditoriasOperaciones_AspNetUsers_UsuarioId] 
    FOREIGN KEY ([UsuarioId]) REFERENCES [dbo].[AspNetUsers] ([Id])
    ON DELETE NO ACTION;
END

-- Verificar la creación
SELECT TOP 5 * FROM [dbo].[AuditoriasOperaciones];