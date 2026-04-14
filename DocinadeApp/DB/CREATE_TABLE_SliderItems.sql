-- =====================================================
-- SCRIPT: Crear tabla SliderItems
-- Fecha: 2025-11-28
-- Descripción: Crea la tabla para el módulo de Slider Dinámico
-- =====================================================

-- Verificar si la tabla ya existe
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SliderItems]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[SliderItems](
        [Id] [int] IDENTITY(1,1) NOT NULL,
        [Titulo] [nvarchar](200) NOT NULL,
        [Subtitulo] [nvarchar](500) NULL,
        [EnlaceUrl] [nvarchar](500) NULL,
        [TextoBoton] [nvarchar](100) NULL,
        [ImagenUrl] [nvarchar](500) NOT NULL,
        [Orden] [int] NOT NULL DEFAULT 1,
        [Activo] [bit] NOT NULL DEFAULT 1,
        [FechaCreacion] [datetime2](7) NOT NULL DEFAULT GETDATE(),
        [FechaModificacion] [datetime2](7) NULL,
        [UsuarioCreacionId] [nvarchar](450) NULL,
        [UsuarioModificacionId] [nvarchar](450) NULL,
     CONSTRAINT [PK_SliderItems] PRIMARY KEY CLUSTERED ([Id] ASC)
    );

    -- Crear índices
    CREATE NONCLUSTERED INDEX [IX_SliderItem_Orden] ON [dbo].[SliderItems]
    (
        [Orden] ASC
    );

    CREATE NONCLUSTERED INDEX [IX_SliderItem_Activo] ON [dbo].[SliderItems]
    (
        [Activo] ASC
    );

    -- Crear foreign keys con AspNetUsers
    ALTER TABLE [dbo].[SliderItems]  WITH CHECK ADD  CONSTRAINT [FK_SliderItems_AspNetUsers_UsuarioCreacionId] 
    FOREIGN KEY([UsuarioCreacionId])
    REFERENCES [dbo].[AspNetUsers] ([Id])
    ON DELETE NO ACTION;

    ALTER TABLE [dbo].[SliderItems]  WITH CHECK ADD  CONSTRAINT [FK_SliderItems_AspNetUsers_UsuarioModificacionId] 
    FOREIGN KEY([UsuarioModificacionId])
    REFERENCES [dbo].[AspNetUsers] ([Id])
    ON DELETE NO ACTION;

    PRINT '[SUCCESS] Tabla SliderItems creada correctamente';
END
ELSE
BEGIN
    PRINT '[INFO] La tabla SliderItems ya existe';
END
GO
