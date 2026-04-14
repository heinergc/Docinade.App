-- Script para crear tabla Profesores
-- Ejecutar en SQL Server Management Studio o sqlcmd

USE RubricasDb;
GO

-- Verificar si la tabla ya existe
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Profesores')
BEGIN
    -- Crear tabla Profesores
    CREATE TABLE [dbo].[Profesores] (
        [Id] int IDENTITY(1,1) NOT NULL,
        [Nombres] nvarchar(100) NOT NULL,
        [PrimerApellido] nvarchar(100) NOT NULL,
        [SegundoApellido] nvarchar(100) NULL,
        [Cedula] nvarchar(20) NOT NULL,
        [TipoCedula] nvarchar(10) NOT NULL DEFAULT 'Nacional',
        [Sexo] nvarchar(1) NULL,
        [FechaNacimiento] datetime2 NULL,
        [EstadoCivil] nvarchar(20) NULL,
        [Nacionalidad] nvarchar(50) NOT NULL DEFAULT 'Costarricense',
        [EmailPersonal] nvarchar(150) NOT NULL,
        [EmailInstitucional] nvarchar(150) NULL,
        [TelefonoFijo] nvarchar(20) NULL,
        [TelefonoCelular] nvarchar(20) NOT NULL,
        [TelefonoOficina] nvarchar(20) NULL,
        [Extension] nvarchar(10) NULL,
        [DireccionExacta] nvarchar(400) NULL,
        [ProvinciaId] int NULL,
        [CantonId] int NULL,
        [DistritoId] int NULL,
        [CodigoPostal] nvarchar(10) NULL,
        [GradoAcademico] nvarchar(100) NULL,
        [TituloAcademico] nvarchar(200) NULL,
        [InstitucionGraduacion] nvarchar(200) NULL,
        [PaisGraduacion] nvarchar(50) NOT NULL DEFAULT 'Costa Rica',
        [AnioGraduacion] int NULL,
        [NumeroColegiadoProfesional] nvarchar(30) NULL,
        [EscuelaId] int NULL,
        [CodigoEmpleado] nvarchar(20) NULL,
        [FechaIngreso] datetime2 NOT NULL,
        [FechaRetiro] datetime2 NULL,
        [TipoContrato] nvarchar(50) NOT NULL,
        [RegimenLaboral] nvarchar(50) NULL,
        [CategoriaLaboral] nvarchar(50) NULL,
        [TipoJornada] nvarchar(20) NOT NULL,
        [HorasLaborales] decimal(5,2) NULL,
        [SalarioBase] decimal(12,2) NULL,
        [CuentaBancaria] nvarchar(30) NULL,
        [TipoCuenta] nvarchar(20) NULL,
        [BancoNombre] nvarchar(100) NULL,
        [EsDirector] bit NOT NULL DEFAULT 0,
        [EsCoordinador] bit NOT NULL DEFAULT 0,
        [EsDecano] bit NOT NULL DEFAULT 0,
        [CargoAdministrativo] nvarchar(100) NULL,
        [FechaInicioCargoAdmin] datetime2 NULL,
        [PuedeCrearRubricas] bit NOT NULL DEFAULT 0,
        [PuedeEvaluarEstudiantes] bit NOT NULL DEFAULT 0,
        [PuedeVerReportes] bit NOT NULL DEFAULT 0,
        [EsAdministradorSistema] bit NOT NULL DEFAULT 0,
        [PuedeGestionarUsuarios] bit NOT NULL DEFAULT 0,
        [Estado] bit NOT NULL DEFAULT 1,
        [MotivoInactividad] nvarchar(200) NULL,
        [NotificacionesEmail] bit NOT NULL DEFAULT 1,
        [NotificacionesSMS] bit NOT NULL DEFAULT 0,
        [AreasEspecializacion] nvarchar(500) NULL,
        [IdiomasHabla] nvarchar(200) NULL,
        [NivelIngles] nvarchar(20) NULL,
        [ExperienciaDocente] int NULL,
        [ContactoEmergenciaNombre] nvarchar(150) NULL,
        [ContactoEmergenciaParentesco] nvarchar(50) NULL,
        [ContactoEmergenciaTelefono] nvarchar(20) NULL,
        [FechaCreacion] datetime2 NOT NULL DEFAULT GETUTCDATE(),
        [CreadoPor] nvarchar(100) NULL,
        [FechaModificacion] datetime2 NULL,
        [ModificadoPor] nvarchar(100) NULL,
        [Version] int NOT NULL DEFAULT 1,
        [UsuarioId] int NULL,
        
        CONSTRAINT [PK_Profesores] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Profesores_Provincias_ProvinciaId] FOREIGN KEY ([ProvinciaId]) REFERENCES [Provincias] ([Id]),
        CONSTRAINT [FK_Profesores_Cantones_CantonId] FOREIGN KEY ([CantonId]) REFERENCES [Cantones] ([Id]),
        CONSTRAINT [FK_Profesores_Distritos_DistritoId] FOREIGN KEY ([DistritoId]) REFERENCES [Distritos] ([Id]),
        CONSTRAINT [FK_Profesores_Escuelas_EscuelaId] FOREIGN KEY ([EscuelaId]) REFERENCES [Escuelas] ([Id])
    );
    
    -- Crear índices para mejorar rendimiento
    CREATE INDEX [IX_Profesores_Cedula] ON [Profesores] ([Cedula]);
    CREATE INDEX [IX_Profesores_EmailPersonal] ON [Profesores] ([EmailPersonal]);
    CREATE INDEX [IX_Profesores_ProvinciaId] ON [Profesores] ([ProvinciaId]);
    CREATE INDEX [IX_Profesores_CantonId] ON [Profesores] ([CantonId]);
    CREATE INDEX [IX_Profesores_DistritoId] ON [Profesores] ([DistritoId]);
    CREATE INDEX [IX_Profesores_EscuelaId] ON [Profesores] ([EscuelaId]);
    
    PRINT 'Tabla Profesores creada exitosamente.';
END
ELSE
BEGIN
    PRINT 'La tabla Profesores ya existe.';
END

-- Verificar si necesitamos agregar EscuelaId a la tabla Materias
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Materias' AND COLUMN_NAME = 'EscuelaId')
BEGIN
    ALTER TABLE [Materias] ADD [EscuelaId] int NULL;
    ALTER TABLE [Materias] ADD CONSTRAINT [FK_Materias_Escuelas_EscuelaId] FOREIGN KEY ([EscuelaId]) REFERENCES [Escuelas] ([Id]);
    PRINT 'Campo EscuelaId agregado a tabla Materias.';
END
ELSE
BEGIN
    PRINT 'El campo EscuelaId ya existe en tabla Materias.';
END

GO