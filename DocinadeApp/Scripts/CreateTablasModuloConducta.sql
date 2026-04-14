-- Script para crear tablas del Módulo de Conducta
-- Solo crea las tablas que NO existen en la base de datos

USE RubricasDb;
GO

-- 1. Tabla ParametrosInstitucion
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ParametrosInstitucion]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[ParametrosInstitucion](
        [IdParametro] [int] IDENTITY(1,1) NOT NULL,
        [Clave] [nvarchar](100) NOT NULL,
        [Nombre] [nvarchar](200) NOT NULL,
        [Descripcion] [nvarchar](500) NULL,
        [Valor] [nvarchar](200) NOT NULL,
        [TipoDato] [nvarchar](50) NOT NULL,
        [Categoria] [nvarchar](50) NULL,
        [Activo] [bit] NOT NULL,
        [FechaCreacion] [datetime2](7) NOT NULL,
        [FechaModificacion] [datetime2](7) NULL,
        [ModificadoPorId] [nvarchar](450) NULL,
        CONSTRAINT [PK_ParametrosInstitucion] PRIMARY KEY CLUSTERED ([IdParametro] ASC)
    );
    PRINT 'Tabla ParametrosInstitucion creada exitosamente.';
END
ELSE
    PRINT 'Tabla ParametrosInstitucion ya existe.';
GO

-- 2. Tabla TiposFalta
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TiposFalta]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[TiposFalta](
        [IdTipoFalta] [int] IDENTITY(1,1) NOT NULL,
        [Nombre] [nvarchar](50) NOT NULL,
        [Definicion] [nvarchar](2000) NOT NULL,
        [Ejemplos] [nvarchar](4000) NULL,
        [AccionCorrectiva] [nvarchar](2000) NULL,
        [RebajoMinimo] [int] NOT NULL,
        [RebajoMaximo] [int] NOT NULL,
        [Orden] [int] NOT NULL,
        [Activo] [bit] NOT NULL,
        [FechaCreacion] [datetime2](7) NOT NULL,
        CONSTRAINT [PK_TiposFalta] PRIMARY KEY CLUSTERED ([IdTipoFalta] ASC)
    );
    PRINT 'Tabla TiposFalta creada exitosamente.';
END
ELSE
    PRINT 'Tabla TiposFalta ya existe.';
GO

-- 3. Tabla BoletasConducta
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BoletasConducta]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[BoletasConducta](
        [IdBoleta] [int] IDENTITY(1,1) NOT NULL,
        [IdEstudiante] [int] NOT NULL,
        [IdTipoFalta] [int] NOT NULL,
        [IdPeriodo] [int] NOT NULL,
        [RebajoAplicado] [int] NOT NULL,
        [Descripcion] [nvarchar](2000) NOT NULL,
        [RutaEvidencia] [nvarchar](500) NULL,
        [DocenteEmisorId] [nvarchar](450) NOT NULL,
        [FechaEmision] [datetime2](7) NOT NULL,
        [ProfesorGuiaId] [nvarchar](450) NULL,
        [FechaNotificacion] [datetime2](7) NULL,
        [NotificacionEnviada] [bit] NOT NULL,
        [ObservacionesProfesorGuia] [nvarchar](1000) NULL,
        [Estado] [nvarchar](50) NOT NULL,
        [MotivoAnulacion] [nvarchar](1000) NULL,
        [FechaAnulacion] [datetime2](7) NULL,
        [AnuladaPorId] [nvarchar](450) NULL,
        CONSTRAINT [PK_BoletasConducta] PRIMARY KEY CLUSTERED ([IdBoleta] ASC),
        CONSTRAINT [FK_BoletasConducta_Estudiantes] FOREIGN KEY([IdEstudiante]) 
            REFERENCES [dbo].[Estudiantes] ([IdEstudiante]) ON DELETE CASCADE,
        CONSTRAINT [FK_BoletasConducta_TiposFalta] FOREIGN KEY([IdTipoFalta]) 
            REFERENCES [dbo].[TiposFalta] ([IdTipoFalta]) ON DELETE CASCADE,
        CONSTRAINT [FK_BoletasConducta_PeriodosAcademicos] FOREIGN KEY([IdPeriodo]) 
            REFERENCES [dbo].[PeriodosAcademicos] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_BoletasConducta_AspNetUsers_DocenteEmisor] FOREIGN KEY([DocenteEmisorId]) 
            REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_BoletasConducta_AspNetUsers_ProfesorGuia] FOREIGN KEY([ProfesorGuiaId]) 
            REFERENCES [dbo].[AspNetUsers] ([Id]),
        CONSTRAINT [FK_BoletasConducta_AspNetUsers_AnuladaPor] FOREIGN KEY([AnuladaPorId]) 
            REFERENCES [dbo].[AspNetUsers] ([Id])
    );
    
    CREATE INDEX [IX_BoletasConducta_IdEstudiante] ON [dbo].[BoletasConducta] ([IdEstudiante]);
    CREATE INDEX [IX_BoletasConducta_IdTipoFalta] ON [dbo].[BoletasConducta] ([IdTipoFalta]);
    CREATE INDEX [IX_BoletasConducta_IdPeriodo] ON [dbo].[BoletasConducta] ([IdPeriodo]);
    CREATE INDEX [IX_BoletasConducta_DocenteEmisorId] ON [dbo].[BoletasConducta] ([DocenteEmisorId]);
    CREATE INDEX [IX_BoletasConducta_ProfesorGuiaId] ON [dbo].[BoletasConducta] ([ProfesorGuiaId]);
    CREATE INDEX [IX_BoletasConducta_AnuladaPorId] ON [dbo].[BoletasConducta] ([AnuladaPorId]);
    
    PRINT 'Tabla BoletasConducta creada exitosamente.';
END
ELSE
    PRINT 'Tabla BoletasConducta ya existe.';
GO

-- 4. Tabla NotasConducta (primero sin FKs circulares)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[NotasConducta]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[NotasConducta](
        [IdNotaConducta] [int] IDENTITY(1,1) NOT NULL,
        [IdEstudiante] [int] NOT NULL,
        [IdPeriodo] [int] NOT NULL,
        [NotaInicial] [decimal](5, 2) NOT NULL,
        [TotalRebajos] [decimal](5, 2) NOT NULL,
        [NotaFinal] [decimal](5, 2) NOT NULL,
        [Estado] [nvarchar](50) NOT NULL,
        [RequiereProgramaAcciones] [bit] NOT NULL,
        [IdProgramaAcciones] [int] NULL,
        [DecisionProfesionalAplicada] [bit] NOT NULL,
        [IdDecisionProfesional] [int] NULL,
        [FechaCalculo] [datetime2](7) NOT NULL,
        [FechaUltimaActualizacion] [datetime2](7) NULL,
        CONSTRAINT [PK_NotasConducta] PRIMARY KEY CLUSTERED ([IdNotaConducta] ASC),
        CONSTRAINT [FK_NotasConducta_Estudiantes] FOREIGN KEY([IdEstudiante]) 
            REFERENCES [dbo].[Estudiantes] ([IdEstudiante]) ON DELETE CASCADE,
        CONSTRAINT [FK_NotasConducta_PeriodosAcademicos] FOREIGN KEY([IdPeriodo]) 
            REFERENCES [dbo].[PeriodosAcademicos] ([Id]) ON DELETE NO ACTION
    );
    
    CREATE INDEX [IX_NotasConducta_IdEstudiante] ON [dbo].[NotasConducta] ([IdEstudiante]);
    CREATE INDEX [IX_NotasConducta_IdPeriodo] ON [dbo].[NotasConducta] ([IdPeriodo]);
    CREATE INDEX [IX_NotasConducta_IdProgramaAcciones] ON [dbo].[NotasConducta] ([IdProgramaAcciones]);
    CREATE INDEX [IX_NotasConducta_IdDecisionProfesional] ON [dbo].[NotasConducta] ([IdDecisionProfesional]);
    
    PRINT 'Tabla NotasConducta creada exitosamente.';
END
ELSE
    PRINT 'Tabla NotasConducta ya existe.';
GO

-- 5. Tabla DecisionesProfesionalesConducta
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DecisionesProfesionalesConducta]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[DecisionesProfesionalesConducta](
        [IdDecision] [int] IDENTITY(1,1) NOT NULL,
        [IdNotaConducta] [int] NOT NULL,
        [IdEstudiante] [int] NOT NULL,
        [IdPeriodo] [int] NOT NULL,
        [JustificacionPedagogica] [nvarchar](3000) NOT NULL,
        [ConsideracionesAdicionales] [nvarchar](2000) NULL,
        [DecisionTomada] [nvarchar](50) NOT NULL,
        [NotaAjustada] [decimal](5, 2) NULL,
        [TomaDecisionPorId] [nvarchar](450) NOT NULL,
        [FechaDecision] [datetime2](7) NOT NULL,
        [NumeroActa] [nvarchar](50) NULL,
        [FechaActa] [datetime2](7) NULL,
        [MiembrosComitePresentes] [nvarchar](1000) NULL,
        [ObservacionesComite] [nvarchar](2000) NULL,
        [RegistradoEnExpediente] [bit] NOT NULL,
        [FechaRegistroExpediente] [datetime2](7) NULL,
        CONSTRAINT [PK_DecisionesProfesionalesConducta] PRIMARY KEY CLUSTERED ([IdDecision] ASC),
        CONSTRAINT [FK_DecisionesProfesionalesConducta_NotasConducta] FOREIGN KEY([IdNotaConducta]) 
            REFERENCES [dbo].[NotasConducta] ([IdNotaConducta]) ON DELETE CASCADE,
        CONSTRAINT [FK_DecisionesProfesionalesConducta_Estudiantes] FOREIGN KEY([IdEstudiante]) 
            REFERENCES [dbo].[Estudiantes] ([IdEstudiante]) ON DELETE NO ACTION,
        CONSTRAINT [FK_DecisionesProfesionalesConducta_PeriodosAcademicos] FOREIGN KEY([IdPeriodo]) 
            REFERENCES [dbo].[PeriodosAcademicos] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_DecisionesProfesionalesConducta_AspNetUsers] FOREIGN KEY([TomaDecisionPorId]) 
            REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE NO ACTION
    );
    
    CREATE INDEX [IX_DecisionesProfesionalesConducta_IdNotaConducta] ON [dbo].[DecisionesProfesionalesConducta] ([IdNotaConducta]);
    CREATE INDEX [IX_DecisionesProfesionalesConducta_IdEstudiante] ON [dbo].[DecisionesProfesionalesConducta] ([IdEstudiante]);
    CREATE INDEX [IX_DecisionesProfesionalesConducta_IdPeriodo] ON [dbo].[DecisionesProfesionalesConducta] ([IdPeriodo]);
    CREATE INDEX [IX_DecisionesProfesionalesConducta_TomaDecisionPorId] ON [dbo].[DecisionesProfesionalesConducta] ([TomaDecisionPorId]);
    
    PRINT 'Tabla DecisionesProfesionalesConducta creada exitosamente.';
END
ELSE
    PRINT 'Tabla DecisionesProfesionalesConducta ya existe.';
GO

-- 6. Tabla ProgramasAccionesInstitucional
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ProgramasAccionesInstitucional]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[ProgramasAccionesInstitucional](
        [IdPrograma] [int] IDENTITY(1,1) NOT NULL,
        [IdNotaConducta] [int] NOT NULL,
        [IdEstudiante] [int] NOT NULL,
        [IdPeriodo] [int] NOT NULL,
        [TituloPrograma] [nvarchar](200) NOT NULL,
        [Descripcion] [nvarchar](2000) NOT NULL,
        [ObjetivosEspecificos] [nvarchar](2000) NULL,
        [ActividadesARealizar] [nvarchar](2000) NULL,
        [FechaInicio] [datetime2](7) NOT NULL,
        [FechaFinPrevista] [datetime2](7) NOT NULL,
        [FechaFinReal] [datetime2](7) NULL,
        [ResponsableSupervisionId] [nvarchar](450) NOT NULL,
        [ObservacionesSupervision] [nvarchar](2000) NULL,
        [Estado] [nvarchar](50) NOT NULL,
        [ResultadoFinal] [nvarchar](50) NULL,
        [FechaVerificacion] [datetime2](7) NULL,
        [VerificadoPorId] [nvarchar](450) NULL,
        [ConclusionesComite] [nvarchar](2000) NULL,
        [AprobarConducta] [bit] NOT NULL,
        [FechaCreacion] [datetime2](7) NOT NULL,
        CONSTRAINT [PK_ProgramasAccionesInstitucional] PRIMARY KEY CLUSTERED ([IdPrograma] ASC),
        CONSTRAINT [FK_ProgramasAccionesInstitucional_NotasConducta] FOREIGN KEY([IdNotaConducta]) 
            REFERENCES [dbo].[NotasConducta] ([IdNotaConducta]) ON DELETE CASCADE,
        CONSTRAINT [FK_ProgramasAccionesInstitucional_Estudiantes] FOREIGN KEY([IdEstudiante]) 
            REFERENCES [dbo].[Estudiantes] ([IdEstudiante]) ON DELETE NO ACTION,
        CONSTRAINT [FK_ProgramasAccionesInstitucional_PeriodosAcademicos] FOREIGN KEY([IdPeriodo]) 
            REFERENCES [dbo].[PeriodosAcademicos] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_ProgramasAccionesInstitucional_AspNetUsers_Responsable] FOREIGN KEY([ResponsableSupervisionId]) 
            REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_ProgramasAccionesInstitucional_AspNetUsers_Verificado] FOREIGN KEY([VerificadoPorId]) 
            REFERENCES [dbo].[AspNetUsers] ([Id])
    );
    
    CREATE INDEX [IX_ProgramasAccionesInstitucional_IdNotaConducta] ON [dbo].[ProgramasAccionesInstitucional] ([IdNotaConducta]);
    CREATE INDEX [IX_ProgramasAccionesInstitucional_IdEstudiante] ON [dbo].[ProgramasAccionesInstitucional] ([IdEstudiante]);
    CREATE INDEX [IX_ProgramasAccionesInstitucional_IdPeriodo] ON [dbo].[ProgramasAccionesInstitucional] ([IdPeriodo]);
    CREATE INDEX [IX_ProgramasAccionesInstitucional_ResponsableSupervisionId] ON [dbo].[ProgramasAccionesInstitucional] ([ResponsableSupervisionId]);
    CREATE INDEX [IX_ProgramasAccionesInstitucional_VerificadoPorId] ON [dbo].[ProgramasAccionesInstitucional] ([VerificadoPorId]);
    
    PRINT 'Tabla ProgramasAccionesInstitucional creada exitosamente.';
END
ELSE
    PRINT 'Tabla ProgramasAccionesInstitucional ya existe.';
GO

-- Agregar FKs circulares de NotasConducta ahora que las otras tablas existen
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[NotasConducta]') AND type in (N'U'))
AND NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_NotasConducta_DecisionesProfesionalesConducta')
BEGIN
    ALTER TABLE [dbo].[NotasConducta]  WITH CHECK ADD CONSTRAINT [FK_NotasConducta_DecisionesProfesionalesConducta] 
    FOREIGN KEY([IdDecisionProfesional])
    REFERENCES [dbo].[DecisionesProfesionalesConducta] ([IdDecision]);
    PRINT 'FK NotasConducta -> DecisionesProfesionalesConducta creada.';
END
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[NotasConducta]') AND type in (N'U'))
AND NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_NotasConducta_ProgramasAccionesInstitucional')
BEGIN
    ALTER TABLE [dbo].[NotasConducta]  WITH CHECK ADD CONSTRAINT [FK_NotasConducta_ProgramasAccionesInstitucional] 
    FOREIGN KEY([IdProgramaAcciones])
    REFERENCES [dbo].[ProgramasAccionesInstitucional] ([IdPrograma]);
    PRINT 'FK NotasConducta -> ProgramasAccionesInstitucional creada.';
END
GO

PRINT '';
PRINT '======================================';
PRINT 'Script completado exitosamente.';
PRINT '======================================';
GO
