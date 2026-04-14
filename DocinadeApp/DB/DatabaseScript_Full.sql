IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250926035418_MigracionAllDB'
)
BEGIN
    CREATE TABLE [AspNetRoles] (
        [Id] nvarchar(450) NOT NULL,
        [Name] nvarchar(256) NULL,
        [NormalizedName] nvarchar(256) NULL,
        [ConcurrencyStamp] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250926035418_MigracionAllDB'
)
BEGIN
    CREATE TABLE [AspNetUsers] (
        [Id] nvarchar(450) NOT NULL,
        [Nombre] nvarchar(100) NOT NULL,
        [Apellidos] nvarchar(100) NOT NULL,
        [IsActive] bit NOT NULL DEFAULT CAST(1 AS bit),
        [CreatedDate] datetime2 NOT NULL DEFAULT (GETDATE()),
        [LastLoginDate] datetime2 NULL,
        [NumeroIdentificacion] nvarchar(20) NULL,
        [Institucion] nvarchar(100) NULL,
        [Departamento] nvarchar(50) NULL,
        [FechaRegistro] datetime2 NOT NULL DEFAULT (GETDATE()),
        [UltimoAcceso] datetime2 NULL,
        [Activo] bit NOT NULL DEFAULT CAST(1 AS bit),
        [Observaciones] nvarchar(500) NULL,
        [UserName] nvarchar(256) NULL,
        [NormalizedUserName] nvarchar(256) NULL,
        [Email] nvarchar(256) NULL,
        [NormalizedEmail] nvarchar(256) NULL,
        [EmailConfirmed] bit NOT NULL,
        [PasswordHash] nvarchar(max) NULL,
        [SecurityStamp] nvarchar(max) NULL,
        [ConcurrencyStamp] nvarchar(max) NULL,
        [PhoneNumber] nvarchar(max) NULL,
        [PhoneNumberConfirmed] bit NOT NULL,
        [TwoFactorEnabled] bit NOT NULL,
        [LockoutEnd] datetimeoffset NULL,
        [LockoutEnabled] bit NOT NULL,
        [AccessFailedCount] int NOT NULL,
        CONSTRAINT [PK_AspNetUsers] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250926035418_MigracionAllDB'
)
BEGIN
    CREATE TABLE [AuditLogs] (
        [Id] int NOT NULL IDENTITY,
        [UserId] nvarchar(450) NOT NULL,
        [UserName] nvarchar(100) NOT NULL,
        [Action] nvarchar(100) NOT NULL,
        [EntityType] nvarchar(100) NOT NULL,
        [EntityId] nvarchar(50) NULL,
        [EntityName] nvarchar(200) NOT NULL,
        [OldValues] NVARCHAR(MAX) NULL,
        [NewValues] NVARCHAR(MAX) NULL,
        [IpAddress] nvarchar(45) NOT NULL,
        [UserAgent] nvarchar(500) NOT NULL,
        [Timestamp] datetime2 NOT NULL DEFAULT (GETDATE()),
        [LogLevel] nvarchar(50) NOT NULL DEFAULT N'Information',
        [AdditionalInfo] nvarchar(500) NULL,
        [Success] bit NOT NULL DEFAULT CAST(1 AS bit),
        [ErrorMessage] nvarchar(1000) NULL,
        [DurationMs] bigint NULL,
        [SessionId] nvarchar(36) NULL,
        [ClientInfo] nvarchar(100) NULL,
        [Referrer] nvarchar(500) NULL,
        [Metadata] NVARCHAR(MAX) NULL,
        [HttpMethod] nvarchar(10) NULL,
        [RequestUrl] nvarchar(500) NULL,
        [ResponseStatusCode] int NULL,
        CONSTRAINT [PK_AuditLogs] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250926035418_MigracionAllDB'
)
BEGIN
    CREATE TABLE [ConfiguracionesSistema] (
        [Id] int NOT NULL IDENTITY,
        [Clave] nvarchar(50) NOT NULL,
        [Valor] nvarchar(200) NOT NULL,
        [Descripcion] nvarchar(500) NULL,
        [FechaCreacion] datetime2 NOT NULL,
        [FechaModificacion] datetime2 NOT NULL DEFAULT (GETDATE()),
        [UsuarioModificacion] nvarchar(100) NULL,
        CONSTRAINT [PK_ConfiguracionesSistema] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250926035418_MigracionAllDB'
)
BEGIN
    CREATE TABLE [GruposCalificacion] (
        [IdGrupo] int NOT NULL IDENTITY,
        [NombreGrupo] nvarchar(100) NOT NULL,
        [Descripcion] nvarchar(500) NULL,
        [Estado] nvarchar(max) NOT NULL,
        [FechaCreacion] datetime2 NOT NULL,
        CONSTRAINT [PK_GruposCalificacion] PRIMARY KEY ([IdGrupo])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250926035418_MigracionAllDB'
)
BEGIN
    CREATE TABLE [InstrumentosEvaluacion] (
        [InstrumentoId] int NOT NULL IDENTITY,
        [Nombre] nvarchar(200) NOT NULL,
        [Descripcion] nvarchar(500) NULL,
        [Activo] bit NOT NULL DEFAULT CAST(1 AS bit),
        [FechaCreacion] datetime2 NOT NULL DEFAULT (GETDATE()),
        CONSTRAINT [PK_InstrumentosEvaluacion] PRIMARY KEY ([InstrumentoId])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250926035418_MigracionAllDB'
)
BEGIN
    CREATE TABLE [Materias] (
        [MateriaId] int NOT NULL IDENTITY,
        [Codigo] nvarchar(100) NOT NULL,
        [Nombre] nvarchar(200) NOT NULL,
        [Creditos] int NOT NULL,
        [Activa] bit NOT NULL DEFAULT CAST(1 AS bit),
        [FechaCreacion] datetime2 NOT NULL DEFAULT (GETDATE()),
        [Tipo] nvarchar(50) NULL,
        [CicloSugerido] int NOT NULL,
        [Descripcion] nvarchar(500) NULL,
        [FechaModificacion] datetime2 NULL,
        [Estado] nvarchar(20) NULL,
        CONSTRAINT [PK_Materias] PRIMARY KEY ([MateriaId])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250926035418_MigracionAllDB'
)
BEGIN
    CREATE TABLE [PeriodosAcademicos] (
        [Id] int NOT NULL IDENTITY,
        [Año] int NOT NULL,
        [Anio] int NOT NULL,
        [Ciclo] nvarchar(10) NOT NULL,
        [FechaInicio] datetime2 NOT NULL,
        [FechaFin] datetime2 NOT NULL,
        [Activo] bit NOT NULL DEFAULT CAST(0 AS bit),
        [Codigo] nvarchar(10) NOT NULL,
        [Nombre] nvarchar(100) NOT NULL,
        [Tipo] int NOT NULL,
        [NumeroPeriodo] int NOT NULL,
        [FechaCreacion] datetime2 NOT NULL,
        [FechaModificacion] datetime2 NULL,
        [Estado] nvarchar(20) NULL DEFAULT N'Activo',
        CONSTRAINT [PK_PeriodosAcademicos] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250926035418_MigracionAllDB'
)
BEGIN
    CREATE TABLE [TiposGrupo] (
        [IdTipoGrupo] int NOT NULL IDENTITY,
        [Nombre] nvarchar(100) NOT NULL,
        [FechaRegistro] datetime2 NOT NULL DEFAULT (GETDATE()),
        [Estado] nvarchar(20) NOT NULL DEFAULT N'Activo',
        CONSTRAINT [PK_TiposGrupo] PRIMARY KEY ([IdTipoGrupo])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250926035418_MigracionAllDB'
)
BEGIN
    CREATE TABLE [AspNetRoleClaims] (
        [Id] int NOT NULL IDENTITY,
        [RoleId] nvarchar(450) NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250926035418_MigracionAllDB'
)
BEGIN
    CREATE TABLE [AspNetUserClaims] (
        [Id] int NOT NULL IDENTITY,
        [UserId] nvarchar(450) NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250926035418_MigracionAllDB'
)
BEGIN
    CREATE TABLE [AspNetUserLogins] (
        [LoginProvider] nvarchar(450) NOT NULL,
        [ProviderKey] nvarchar(450) NOT NULL,
        [ProviderDisplayName] nvarchar(max) NULL,
        [UserId] nvarchar(450) NOT NULL,
        CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
        CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250926035418_MigracionAllDB'
)
BEGIN
    CREATE TABLE [AspNetUserRoles] (
        [UserId] nvarchar(450) NOT NULL,
        [RoleId] nvarchar(450) NOT NULL,
        CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId]),
        CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250926035418_MigracionAllDB'
)
BEGIN
    CREATE TABLE [AspNetUserTokens] (
        [UserId] nvarchar(450) NOT NULL,
        [LoginProvider] nvarchar(450) NOT NULL,
        [Name] nvarchar(450) NOT NULL,
        [Value] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
        CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250926035418_MigracionAllDB'
)
BEGIN
    CREATE TABLE [AuditoriasOperaciones] (
        [Id] int NOT NULL IDENTITY,
        [TipoOperacion] nvarchar(20) NOT NULL,
        [TablaAfectada] nvarchar(100) NOT NULL,
        [RegistroId] int NOT NULL,
        [Descripcion] nvarchar(500) NOT NULL,
        [Motivo] nvarchar(1000) NULL,
        [DireccionIP] nvarchar(45) NULL,
        [UserAgent] nvarchar(500) NULL,
        [FechaOperacion] datetime2 NOT NULL DEFAULT (GETDATE()),
        [UsuarioId] nvarchar(450) NOT NULL,
        [OperacionExitosa] bit NOT NULL,
        [MensajeError] nvarchar(1000) NULL,
        [DatosAnteriores] ntext NULL,
        [DatosNuevos] ntext NULL,
        CONSTRAINT [PK_AuditoriasOperaciones] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AuditoriasOperaciones_AspNetUsers_UsuarioId] FOREIGN KEY ([UsuarioId]) REFERENCES [AspNetUsers] ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250926035418_MigracionAllDB'
)
BEGIN
    CREATE TABLE [NivelesCalificacion] (
        [IdNivel] int NOT NULL IDENTITY,
        [NombreNivel] nvarchar(100) NOT NULL,
        [Descripcion] nvarchar(500) NULL,
        [OrdenNivel] int NULL,
        [IdGrupo] int NULL,
        CONSTRAINT [PK_NivelesCalificacion] PRIMARY KEY ([IdNivel]),
        CONSTRAINT [FK_NivelesCalificacion_GruposCalificacion_IdGrupo] FOREIGN KEY ([IdGrupo]) REFERENCES [GruposCalificacion] ([IdGrupo]) ON DELETE SET NULL
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250926035418_MigracionAllDB'
)
BEGIN
    CREATE TABLE [Rubricas] (
        [IdRubrica] int NOT NULL IDENTITY,
        [NombreRubrica] nvarchar(200) NOT NULL,
        [Descripcion] nvarchar(500) NULL,
        [FechaCreacion] datetime2 NOT NULL DEFAULT (GETDATE()),
        [Estado] nvarchar(20) NULL DEFAULT N'ACTIVO',
        [EsPublica] int NOT NULL,
        [IdGrupo] int NULL,
        [CreadoPorId] nvarchar(450) NULL,
        [ModificadoPorId] nvarchar(450) NULL,
        [FechaModificacion] datetime2 NULL,
        [Titulo] nvarchar(max) NOT NULL,
        [Vigente] bit NOT NULL,
        CONSTRAINT [PK_Rubricas] PRIMARY KEY ([IdRubrica]),
        CONSTRAINT [FK_Rubricas_AspNetUsers_CreadoPorId] FOREIGN KEY ([CreadoPorId]) REFERENCES [AspNetUsers] ([Id]),
        CONSTRAINT [FK_Rubricas_AspNetUsers_ModificadoPorId] FOREIGN KEY ([ModificadoPorId]) REFERENCES [AspNetUsers] ([Id]),
        CONSTRAINT [FK_Rubricas_GruposCalificacion_IdGrupo] FOREIGN KEY ([IdGrupo]) REFERENCES [GruposCalificacion] ([IdGrupo]) ON DELETE SET NULL
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250926035418_MigracionAllDB'
)
BEGIN
    CREATE TABLE [MateriaRequisitos] (
        [MateriaId] int NOT NULL,
        [RequisitoId] int NOT NULL,
        CONSTRAINT [PK_MateriaRequisitos] PRIMARY KEY ([MateriaId], [RequisitoId]),
        CONSTRAINT [FK_MateriaRequisitos_Materias_MateriaId] FOREIGN KEY ([MateriaId]) REFERENCES [Materias] ([MateriaId]) ON DELETE NO ACTION,
        CONSTRAINT [FK_MateriaRequisitos_Materias_RequisitoId] FOREIGN KEY ([RequisitoId]) REFERENCES [Materias] ([MateriaId]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250926035418_MigracionAllDB'
)
BEGIN
    CREATE TABLE [CuadernosCalificadores] (
        [Id] int NOT NULL IDENTITY,
        [MateriaId] int NOT NULL,
        [PeriodoAcademicoId] int NOT NULL,
        [Nombre] nvarchar(200) NOT NULL,
        [FechaCreacion] datetime2 NOT NULL DEFAULT (GETDATE()),
        [Estado] nvarchar(20) NOT NULL DEFAULT N'ACTIVO',
        [FechaCierre] datetime2 NULL,
        CONSTRAINT [PK_CuadernosCalificadores] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_CuadernosCalificadores_Materias_MateriaId] FOREIGN KEY ([MateriaId]) REFERENCES [Materias] ([MateriaId]) ON DELETE NO ACTION,
        CONSTRAINT [FK_CuadernosCalificadores_PeriodosAcademicos_PeriodoAcademicoId] FOREIGN KEY ([PeriodoAcademicoId]) REFERENCES [PeriodosAcademicos] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250926035418_MigracionAllDB'
)
BEGIN
    CREATE TABLE [Estudiantes] (
        [IdEstudiante] int NOT NULL IDENTITY,
        [Nombre] nvarchar(100) NOT NULL,
        [Apellidos] nvarchar(100) NOT NULL,
        [NumeroId] nvarchar(20) NOT NULL,
        [DireccionCorreo] nvarchar(100) NOT NULL,
        [Institucion] nvarchar(100) NOT NULL,
        [Grupos] nvarchar(50) NULL,
        [Año] int NOT NULL,
        [PeriodoAcademicoId] int NOT NULL,
        CONSTRAINT [PK_Estudiantes] PRIMARY KEY ([IdEstudiante]),
        CONSTRAINT [FK_Estudiantes_PeriodosAcademicos_PeriodoAcademicoId] FOREIGN KEY ([PeriodoAcademicoId]) REFERENCES [PeriodosAcademicos] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250926035418_MigracionAllDB'
)
BEGIN
    CREATE TABLE [InstrumentoMaterias] (
        [InstrumentoEvaluacionId] int NOT NULL,
        [MateriaId] int NOT NULL,
        [PeriodoAcademicoId] int NOT NULL,
        [FechaAsignacion] datetime2 NOT NULL DEFAULT (GETDATE()),
        [OrdenPresentacion] int NULL,
        [EsObligatorio] bit NOT NULL DEFAULT CAST(0 AS bit),
        [Observaciones] nvarchar(max) NULL,
        CONSTRAINT [PK_InstrumentoMaterias] PRIMARY KEY ([InstrumentoEvaluacionId], [MateriaId]),
        CONSTRAINT [FK_InstrumentoMaterias_InstrumentosEvaluacion_InstrumentoEvaluacionId] FOREIGN KEY ([InstrumentoEvaluacionId]) REFERENCES [InstrumentosEvaluacion] ([InstrumentoId]) ON DELETE NO ACTION,
        CONSTRAINT [FK_InstrumentoMaterias_Materias_MateriaId] FOREIGN KEY ([MateriaId]) REFERENCES [Materias] ([MateriaId]) ON DELETE NO ACTION,
        CONSTRAINT [FK_InstrumentoMaterias_PeriodosAcademicos_PeriodoAcademicoId] FOREIGN KEY ([PeriodoAcademicoId]) REFERENCES [PeriodosAcademicos] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250926035418_MigracionAllDB'
)
BEGIN
    CREATE TABLE [MateriaPeriodos] (
        [Id] int NOT NULL IDENTITY,
        [MateriaId] int NOT NULL,
        [PeriodoAcademicoId] int NOT NULL,
        [Cupo] int NOT NULL DEFAULT 0,
        [Estado] nvarchar(20) NOT NULL DEFAULT N'Abierta',
        [FechaPublicacion] datetime2 NULL,
        [FechaCreacion] datetime2 NOT NULL DEFAULT (GETDATE()),
        [Observaciones] nvarchar(500) NULL,
        [PeriodoAcademicoId1] int NULL,
        CONSTRAINT [PK_MateriaPeriodos] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_MateriaPeriodos_Materias_MateriaId] FOREIGN KEY ([MateriaId]) REFERENCES [Materias] ([MateriaId]) ON DELETE CASCADE,
        CONSTRAINT [FK_MateriaPeriodos_PeriodosAcademicos_PeriodoAcademicoId] FOREIGN KEY ([PeriodoAcademicoId]) REFERENCES [PeriodosAcademicos] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_MateriaPeriodos_PeriodosAcademicos_PeriodoAcademicoId1] FOREIGN KEY ([PeriodoAcademicoId1]) REFERENCES [PeriodosAcademicos] ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250926035418_MigracionAllDB'
)
BEGIN
    CREATE TABLE [GruposEstudiantes] (
        [GrupoId] int NOT NULL IDENTITY,
        [Codigo] nvarchar(20) NOT NULL,
        [Nombre] nvarchar(100) NOT NULL,
        [Descripcion] nvarchar(500) NULL,
        [IdTipoGrupo] int NOT NULL,
        [TipoGrupo] nvarchar(max) NOT NULL,
        [Nivel] nvarchar(50) NULL,
        [CapacidadMaxima] int NULL,
        [PeriodoAcademicoId] int NOT NULL,
        [Estado] nvarchar(20) NOT NULL,
        [FechaCreacion] datetime2 NOT NULL DEFAULT (GETDATE()),
        [FechaModificacion] datetime2 NULL,
        [CreadoPorId] nvarchar(450) NULL,
        [Observaciones] nvarchar(1000) NULL,
        CONSTRAINT [PK_GruposEstudiantes] PRIMARY KEY ([GrupoId]),
        CONSTRAINT [FK_GruposEstudiantes_AspNetUsers_CreadoPorId] FOREIGN KEY ([CreadoPorId]) REFERENCES [AspNetUsers] ([Id]),
        CONSTRAINT [FK_GruposEstudiantes_PeriodosAcademicos_PeriodoAcademicoId] FOREIGN KEY ([PeriodoAcademicoId]) REFERENCES [PeriodosAcademicos] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_GruposEstudiantes_TiposGrupo_IdTipoGrupo] FOREIGN KEY ([IdTipoGrupo]) REFERENCES [TiposGrupo] ([IdTipoGrupo]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250926035418_MigracionAllDB'
)
BEGIN
    CREATE TABLE [InstrumentoRubricas] (
        [InstrumentoEvaluacionId] int NOT NULL,
        [RubricaId] int NOT NULL,
        [InstrumentoId] int NOT NULL,
        [FechaAsignacion] datetime2 NOT NULL DEFAULT (GETDATE()),
        [OrdenPresentacion] int NULL,
        [EsObligatorio] bit NOT NULL DEFAULT CAST(0 AS bit),
        [Ponderacion] decimal(5,2) NOT NULL DEFAULT 0.0,
        CONSTRAINT [PK_InstrumentoRubricas] PRIMARY KEY ([InstrumentoEvaluacionId], [RubricaId]),
        CONSTRAINT [FK_InstrumentoRubricas_InstrumentosEvaluacion_InstrumentoEvaluacionId] FOREIGN KEY ([InstrumentoEvaluacionId]) REFERENCES [InstrumentosEvaluacion] ([InstrumentoId]) ON DELETE NO ACTION,
        CONSTRAINT [FK_InstrumentoRubricas_Rubricas_RubricaId] FOREIGN KEY ([RubricaId]) REFERENCES [Rubricas] ([IdRubrica]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250926035418_MigracionAllDB'
)
BEGIN
    CREATE TABLE [ItemsEvaluacion] (
        [IdItem] int NOT NULL IDENTITY,
        [IdRubrica] int NOT NULL,
        [NombreItem] nvarchar(200) NOT NULL,
        [Descripcion] nvarchar(max) NULL,
        [OrdenItem] int NULL,
        [Peso] decimal(5,2) NOT NULL DEFAULT 0.0,
        CONSTRAINT [PK_ItemsEvaluacion] PRIMARY KEY ([IdItem]),
        CONSTRAINT [FK_ItemsEvaluacion_Rubricas_IdRubrica] FOREIGN KEY ([IdRubrica]) REFERENCES [Rubricas] ([IdRubrica]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250926035418_MigracionAllDB'
)
BEGIN
    CREATE TABLE [RubricaNiveles] (
        [IdRubrica] int NOT NULL,
        [IdNivel] int NOT NULL,
        [OrdenEnRubrica] int NOT NULL DEFAULT 0,
        CONSTRAINT [PK_RubricaNiveles] PRIMARY KEY ([IdRubrica], [IdNivel]),
        CONSTRAINT [FK_RubricaNiveles_NivelesCalificacion_IdNivel] FOREIGN KEY ([IdNivel]) REFERENCES [NivelesCalificacion] ([IdNivel]) ON DELETE NO ACTION,
        CONSTRAINT [FK_RubricaNiveles_Rubricas_IdRubrica] FOREIGN KEY ([IdRubrica]) REFERENCES [Rubricas] ([IdRubrica]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250926035418_MigracionAllDB'
)
BEGIN
    CREATE TABLE [CuadernoInstrumentos] (
        [Id] int NOT NULL IDENTITY,
        [CuadernoCalificadorId] int NOT NULL,
        [RubricaId] int NOT NULL,
        [PonderacionPorcentaje] decimal(5,2) NOT NULL,
        [Orden] int NOT NULL,
        [EsObligatorio] bit NOT NULL DEFAULT CAST(1 AS bit),
        CONSTRAINT [PK_CuadernoInstrumentos] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_CuadernoInstrumentos_CuadernosCalificadores_CuadernoCalificadorId] FOREIGN KEY ([CuadernoCalificadorId]) REFERENCES [CuadernosCalificadores] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_CuadernoInstrumentos_Rubricas_RubricaId] FOREIGN KEY ([RubricaId]) REFERENCES [Rubricas] ([IdRubrica]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250926035418_MigracionAllDB'
)
BEGIN
    CREATE TABLE [Evaluaciones] (
        [IdEvaluacion] int NOT NULL IDENTITY,
        [IdEstudiante] int NOT NULL,
        [IdRubrica] int NOT NULL,
        [FechaEvaluacion] datetime2 NOT NULL DEFAULT (GETDATE()),
        [TotalPuntos] decimal(5,2) NULL,
        [Observaciones] nvarchar(1000) NULL,
        [Estado] nvarchar(20) NOT NULL DEFAULT N'BORRADOR',
        [EvaluadoPorId] nvarchar(450) NULL,
        [FechaFinalizacion] datetime2 NULL,
        [TiempoEvaluacionMinutos] int NULL,
        CONSTRAINT [PK_Evaluaciones] PRIMARY KEY ([IdEvaluacion]),
        CONSTRAINT [FK_Evaluaciones_AspNetUsers_EvaluadoPorId] FOREIGN KEY ([EvaluadoPorId]) REFERENCES [AspNetUsers] ([Id]),
        CONSTRAINT [FK_Evaluaciones_Estudiantes_IdEstudiante] FOREIGN KEY ([IdEstudiante]) REFERENCES [Estudiantes] ([IdEstudiante]) ON DELETE CASCADE,
        CONSTRAINT [FK_Evaluaciones_Rubricas_IdRubrica] FOREIGN KEY ([IdRubrica]) REFERENCES [Rubricas] ([IdRubrica]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250926035418_MigracionAllDB'
)
BEGIN
    CREATE TABLE [EstudianteGrupos] (
        [Id] int NOT NULL IDENTITY,
        [EstudianteId] int NOT NULL,
        [GrupoId] int NOT NULL,
        [FechaAsignacion] datetime2 NOT NULL DEFAULT (GETDATE()),
        [FechaDesasignacion] datetime2 NULL,
        [Estado] nvarchar(20) NOT NULL DEFAULT N'Activo',
        [AsignadoPorId] nvarchar(450) NULL,
        [MotivoAsignacion] nvarchar(200) NULL,
        [EsGrupoPrincipal] bit NOT NULL DEFAULT CAST(1 AS bit),
        CONSTRAINT [PK_EstudianteGrupos] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_EstudianteGrupos_AspNetUsers_AsignadoPorId] FOREIGN KEY ([AsignadoPorId]) REFERENCES [AspNetUsers] ([Id]),
        CONSTRAINT [FK_EstudianteGrupos_Estudiantes_EstudianteId] FOREIGN KEY ([EstudianteId]) REFERENCES [Estudiantes] ([IdEstudiante]) ON DELETE NO ACTION,
        CONSTRAINT [FK_EstudianteGrupos_GruposEstudiantes_GrupoId] FOREIGN KEY ([GrupoId]) REFERENCES [GruposEstudiantes] ([GrupoId]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250926035418_MigracionAllDB'
)
BEGIN
    CREATE TABLE [GrupoMaterias] (
        [Id] int NOT NULL IDENTITY,
        [GrupoId] int NOT NULL,
        [MateriaId] int NOT NULL,
        [FechaAsignacion] datetime2 NOT NULL DEFAULT (GETDATE()),
        [Estado] nvarchar(20) NOT NULL DEFAULT N'Activo',
        [Observaciones] nvarchar(500) NULL,
        CONSTRAINT [PK_GrupoMaterias] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_GrupoMaterias_GruposEstudiantes_GrupoId] FOREIGN KEY ([GrupoId]) REFERENCES [GruposEstudiantes] ([GrupoId]) ON DELETE NO ACTION,
        CONSTRAINT [FK_GrupoMaterias_Materias_MateriaId] FOREIGN KEY ([MateriaId]) REFERENCES [Materias] ([MateriaId]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250926035418_MigracionAllDB'
)
BEGIN
    CREATE TABLE [ValoresRubrica] (
        [IdValor] int NOT NULL IDENTITY,
        [IdRubrica] int NOT NULL,
        [IdItem] int NOT NULL,
        [IdNivel] int NOT NULL,
        [ValorPuntos] decimal(5,2) NOT NULL DEFAULT 0.0,
        CONSTRAINT [PK_ValoresRubrica] PRIMARY KEY ([IdValor]),
        CONSTRAINT [FK_ValoresRubrica_ItemsEvaluacion_IdItem] FOREIGN KEY ([IdItem]) REFERENCES [ItemsEvaluacion] ([IdItem]) ON DELETE CASCADE,
        CONSTRAINT [FK_ValoresRubrica_NivelesCalificacion_IdNivel] FOREIGN KEY ([IdNivel]) REFERENCES [NivelesCalificacion] ([IdNivel]) ON DELETE NO ACTION,
        CONSTRAINT [FK_ValoresRubrica_Rubricas_IdRubrica] FOREIGN KEY ([IdRubrica]) REFERENCES [Rubricas] ([IdRubrica]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250926035418_MigracionAllDB'
)
BEGIN
    CREATE TABLE [DetallesEvaluacion] (
        [IdDetalle] int NOT NULL IDENTITY,
        [IdEvaluacion] int NOT NULL,
        [IdItem] int NOT NULL,
        [IdNivel] int NOT NULL,
        [PuntosObtenidos] decimal(5,2) NOT NULL,
        CONSTRAINT [PK_DetallesEvaluacion] PRIMARY KEY ([IdDetalle]),
        CONSTRAINT [FK_DetallesEvaluacion_Evaluaciones_IdEvaluacion] FOREIGN KEY ([IdEvaluacion]) REFERENCES [Evaluaciones] ([IdEvaluacion]) ON DELETE CASCADE,
        CONSTRAINT [FK_DetallesEvaluacion_ItemsEvaluacion_IdItem] FOREIGN KEY ([IdItem]) REFERENCES [ItemsEvaluacion] ([IdItem]) ON DELETE NO ACTION,
        CONSTRAINT [FK_DetallesEvaluacion_NivelesCalificacion_IdNivel] FOREIGN KEY ([IdNivel]) REFERENCES [NivelesCalificacion] ([IdNivel]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250926035418_MigracionAllDB'
)
BEGIN
    CREATE INDEX [IX_AspNetRoleClaims_RoleId] ON [AspNetRoleClaims] ([RoleId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250926035418_MigracionAllDB'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [RoleNameIndex] ON [AspNetRoles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250926035418_MigracionAllDB'
)
BEGIN
    CREATE INDEX [IX_AspNetUserClaims_UserId] ON [AspNetUserClaims] ([UserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250926035418_MigracionAllDB'
)
BEGIN
    CREATE INDEX [IX_AspNetUserLogins_UserId] ON [AspNetUserLogins] ([UserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250926035418_MigracionAllDB'
)
BEGIN
    CREATE INDEX [IX_AspNetUserRoles_RoleId] ON [AspNetUserRoles] ([RoleId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250926035418_MigracionAllDB'
)
BEGIN
    CREATE INDEX [EmailIndex] ON [AspNetUsers] ([NormalizedEmail]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250926035418_MigracionAllDB'
)
BEGIN
    CREATE INDEX [IX_AspNetUsers_CreatedDate] ON [AspNetUsers] ([CreatedDate]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250926035418_MigracionAllDB'
)
BEGIN
    CREATE INDEX [IX_AspNetUsers_IsActive] ON [AspNetUsers] ([IsActive]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250926035418_MigracionAllDB'
)
BEGIN
    CREATE INDEX [IX_AspNetUsers_LastLoginDate] ON [AspNetUsers] ([LastLoginDate]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250926035418_MigracionAllDB'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_AspNetUsers_NumeroIdentificacion] ON [AspNetUsers] ([NumeroIdentificacion]) WHERE [NumeroIdentificacion] IS NOT NULL');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250926035418_MigracionAllDB'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [UserNameIndex] ON [AspNetUsers] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250926035418_MigracionAllDB'
)
BEGIN
    CREATE INDEX [IX_AuditLogs_Action] ON [AuditLogs] ([Action]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250926035418_MigracionAllDB'
)
BEGIN
    CREATE INDEX [IX_AuditLogs_EntityType] ON [AuditLogs] ([EntityType]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250926035418_MigracionAllDB'
)
BEGIN
    CREATE INDEX [IX_AuditLogs_EntityType_EntityId] ON [AuditLogs] ([EntityType], [EntityId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250926035418_MigracionAllDB'
)
BEGIN
    CREATE INDEX [IX_AuditLogs_Timestamp] ON [AuditLogs] ([Timestamp]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250926035418_MigracionAllDB'
)
BEGIN
    CREATE INDEX [IX_AuditLogs_UserId] ON [AuditLogs] ([UserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250926035418_MigracionAllDB'
)
BEGIN
    CREATE INDEX [IX_AuditoriaOperacion_FechaOperacion] ON [AuditoriasOperaciones] ([FechaOperacion]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250926035418_MigracionAllDB'
)
BEGIN
    CREATE INDEX [IX_AuditoriaOperacion_RegistroId] ON [AuditoriasOperaciones] ([RegistroId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250926035418_MigracionAllDB'
)
BEGIN
    CREATE INDEX [IX_AuditoriaOperacion_Tabla_Registro] ON [AuditoriasOperaciones] ([TablaAfectada], [RegistroId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250926035418_MigracionAllDB'
)
BEGIN
    CREATE INDEX [IX_AuditoriaOperacion_TablaAfectada] ON [AuditoriasOperaciones] ([TablaAfectada]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250926035418_MigracionAllDB'
)
BEGIN
    CREATE INDEX [IX_AuditoriaOperacion_UsuarioId] ON [AuditoriasOperaciones] ([UsuarioId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250926035418_MigracionAllDB'
)
BEGIN
    CREATE UNIQUE INDEX [IX_ConfiguracionesSistema_Clave] ON [ConfiguracionesSistema] ([Clave]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250926035418_MigracionAllDB'
)
BEGIN
    CREATE INDEX [IX_CuadernoInstrumentos_CuadernoCalificadorId] ON [CuadernoInstrumentos] ([CuadernoCalificadorId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250926035418_MigracionAllDB'
)
BEGIN
    CREATE INDEX [IX_CuadernoInstrumentos_RubricaId] ON [CuadernoInstrumentos] ([RubricaId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250926035418_MigracionAllDB'
)
BEGIN
    CREATE INDEX [IX_CuadernosCalificadores_MateriaId] ON [CuadernosCalificadores] ([MateriaId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250926035418_MigracionAllDB'
)
BEGIN
    CREATE INDEX [IX_CuadernosCalificadores_PeriodoAcademicoId] ON [CuadernosCalificadores] ([PeriodoAcademicoId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250926035418_MigracionAllDB'
)
BEGIN
    CREATE INDEX [IX_DetallesEvaluacion_IdEvaluacion] ON [DetallesEvaluacion] ([IdEvaluacion]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250926035418_MigracionAllDB'
)
BEGIN
    CREATE INDEX [IX_DetallesEvaluacion_IdItem] ON [DetallesEvaluacion] ([IdItem]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250926035418_MigracionAllDB'
)
BEGIN
    CREATE INDEX [IX_DetallesEvaluacion_IdNivel] ON [DetallesEvaluacion] ([IdNivel]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250926035418_MigracionAllDB'
)
BEGIN
    CREATE INDEX [IX_EstudianteGrupos_AsignadoPorId] ON [EstudianteGrupos] ([AsignadoPorId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250926035418_MigracionAllDB'
)
BEGIN
    CREATE INDEX [IX_EstudianteGrupos_Estado] ON [EstudianteGrupos] ([Estado]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250926035418_MigracionAllDB'
)
BEGIN
    CREATE INDEX [IX_EstudianteGrupos_FechaAsignacion] ON [EstudianteGrupos] ([FechaAsignacion]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250926035418_MigracionAllDB'
)
BEGIN
    CREATE INDEX [IX_EstudianteGrupos_GrupoId] ON [EstudianteGrupos] ([GrupoId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250926035418_MigracionAllDB'
)
BEGIN
    EXEC(N'CREATE INDEX [IX_EstudianteGrupos_Unique_Activo] ON [EstudianteGrupos] ([EstudianteId], [GrupoId], [Estado]) WHERE [Estado] = ''Activo''');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250926035418_MigracionAllDB'
)
BEGIN
    CREATE INDEX [IX_Estudiantes_DireccionCorreo] ON [Estudiantes] ([DireccionCorreo]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250926035418_MigracionAllDB'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Estudiantes_NumeroId] ON [Estudiantes] ([NumeroId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250926035418_MigracionAllDB'
)
BEGIN
    CREATE INDEX [IX_Estudiantes_PeriodoAcademicoId] ON [Estudiantes] ([PeriodoAcademicoId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250926035418_MigracionAllDB'
)
BEGIN
    CREATE INDEX [IX_Evaluaciones_EvaluadoPorId] ON [Evaluaciones] ([EvaluadoPorId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250926035418_MigracionAllDB'
)
BEGIN
    CREATE INDEX [IX_Evaluaciones_IdEstudiante] ON [Evaluaciones] ([IdEstudiante]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250926035418_MigracionAllDB'
)
BEGIN
    CREATE INDEX [IX_Evaluaciones_IdRubrica] ON [Evaluaciones] ([IdRubrica]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250926035418_MigracionAllDB'
)
BEGIN
    CREATE INDEX [IX_GrupoMaterias_MateriaId] ON [GrupoMaterias] ([MateriaId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250926035418_MigracionAllDB'
)
BEGIN
    CREATE UNIQUE INDEX [IX_GrupoMaterias_Unique] ON [GrupoMaterias] ([GrupoId], [MateriaId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250926035418_MigracionAllDB'
)
BEGIN
    CREATE UNIQUE INDEX [IX_GruposEstudiantes_Codigo_Periodo] ON [GruposEstudiantes] ([Codigo], [PeriodoAcademicoId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250926035418_MigracionAllDB'
)
BEGIN
    CREATE INDEX [IX_GruposEstudiantes_CreadoPorId] ON [GruposEstudiantes] ([CreadoPorId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250926035418_MigracionAllDB'
)
BEGIN
    CREATE INDEX [IX_GruposEstudiantes_Estado] ON [GruposEstudiantes] ([Estado]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250926035418_MigracionAllDB'
)
BEGIN
    CREATE INDEX [IX_GruposEstudiantes_IdTipoGrupo] ON [GruposEstudiantes] ([IdTipoGrupo]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250926035418_MigracionAllDB'
)
BEGIN
    CREATE INDEX [IX_GruposEstudiantes_PeriodoAcademicoId] ON [GruposEstudiantes] ([PeriodoAcademicoId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250926035418_MigracionAllDB'
)
BEGIN
    CREATE INDEX [IX_InstrumentoMaterias_MateriaId] ON [InstrumentoMaterias] ([MateriaId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250926035418_MigracionAllDB'
)
BEGIN
    CREATE INDEX [IX_InstrumentoMaterias_PeriodoAcademicoId] ON [InstrumentoMaterias] ([PeriodoAcademicoId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250926035418_MigracionAllDB'
)
BEGIN
    CREATE INDEX [IX_InstrumentoRubricas_RubricaId] ON [InstrumentoRubricas] ([RubricaId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250926035418_MigracionAllDB'
)
BEGIN
    CREATE INDEX [IX_ItemsEvaluacion_IdRubrica] ON [ItemsEvaluacion] ([IdRubrica]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250926035418_MigracionAllDB'
)
BEGIN
    CREATE INDEX [IX_MateriaPeriodos_MateriaId] ON [MateriaPeriodos] ([MateriaId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250926035418_MigracionAllDB'
)
BEGIN
    CREATE INDEX [IX_MateriaPeriodos_PeriodoAcademicoId] ON [MateriaPeriodos] ([PeriodoAcademicoId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250926035418_MigracionAllDB'
)
BEGIN
    CREATE INDEX [IX_MateriaPeriodos_PeriodoAcademicoId1] ON [MateriaPeriodos] ([PeriodoAcademicoId1]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250926035418_MigracionAllDB'
)
BEGIN
    CREATE INDEX [IX_MateriaRequisitos_RequisitoId] ON [MateriaRequisitos] ([RequisitoId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250926035418_MigracionAllDB'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Materias_Codigo] ON [Materias] ([Codigo]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250926035418_MigracionAllDB'
)
BEGIN
    CREATE INDEX [IX_NivelesCalificacion_IdGrupo] ON [NivelesCalificacion] ([IdGrupo]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250926035418_MigracionAllDB'
)
BEGIN
    CREATE INDEX [IX_PeriodosAcademicos_Estado] ON [PeriodosAcademicos] ([Estado]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250926035418_MigracionAllDB'
)
BEGIN
    CREATE INDEX [IX_PeriodosAcademicos_FechaFin] ON [PeriodosAcademicos] ([FechaFin]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250926035418_MigracionAllDB'
)
BEGIN
    CREATE INDEX [IX_PeriodosAcademicos_FechaInicio] ON [PeriodosAcademicos] ([FechaInicio]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250926035418_MigracionAllDB'
)
BEGIN
    CREATE INDEX [IX_RubricaNiveles_IdNivel] ON [RubricaNiveles] ([IdNivel]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250926035418_MigracionAllDB'
)
BEGIN
    CREATE INDEX [IX_Rubricas_CreadoPorId] ON [Rubricas] ([CreadoPorId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250926035418_MigracionAllDB'
)
BEGIN
    CREATE INDEX [IX_Rubricas_IdGrupo] ON [Rubricas] ([IdGrupo]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250926035418_MigracionAllDB'
)
BEGIN
    CREATE INDEX [IX_Rubricas_ModificadoPorId] ON [Rubricas] ([ModificadoPorId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250926035418_MigracionAllDB'
)
BEGIN
    CREATE INDEX [IX_TiposGrupo_Estado] ON [TiposGrupo] ([Estado]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250926035418_MigracionAllDB'
)
BEGIN
    CREATE INDEX [IX_TiposGrupo_FechaRegistro] ON [TiposGrupo] ([FechaRegistro]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250926035418_MigracionAllDB'
)
BEGIN
    CREATE INDEX [IX_ValoresRubrica_IdItem] ON [ValoresRubrica] ([IdItem]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250926035418_MigracionAllDB'
)
BEGIN
    CREATE INDEX [IX_ValoresRubrica_IdNivel] ON [ValoresRubrica] ([IdNivel]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250926035418_MigracionAllDB'
)
BEGIN
    CREATE UNIQUE INDEX [IX_ValorRubrica_Unique] ON [ValoresRubrica] ([IdRubrica], [IdItem], [IdNivel]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250926035418_MigracionAllDB'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20250926035418_MigracionAllDB', N'8.0.0');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250927015007_CambiosRecientes_0926'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20250927015007_CambiosRecientes_0926', N'8.0.0');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251006233817_AgregarTablaAsistencias'
)
BEGIN
    CREATE TABLE [Asistencias] (
        [AsistenciaId] int NOT NULL IDENTITY,
        [EstudianteId] int NOT NULL,
        [GrupoId] int NOT NULL,
        [Fecha] datetime2 NOT NULL,
        [Estado] nvarchar(2) NOT NULL,
        [Justificacion] nvarchar(500) NULL,
        [Observaciones] nvarchar(1000) NULL,
        [FechaRegistro] datetime2 NOT NULL DEFAULT (GETDATE()),
        [RegistradoPorId] nvarchar(450) NULL,
        [HoraLlegada] time NULL,
        [EsModificacion] bit NOT NULL DEFAULT CAST(0 AS bit),
        [FechaModificacion] datetime2 NULL,
        [ModificadoPorId] nvarchar(450) NULL,
        CONSTRAINT [PK_Asistencias] PRIMARY KEY ([AsistenciaId]),
        CONSTRAINT [FK_Asistencias_AspNetUsers_ModificadoPorId] FOREIGN KEY ([ModificadoPorId]) REFERENCES [AspNetUsers] ([Id]),
        CONSTRAINT [FK_Asistencias_AspNetUsers_RegistradoPorId] FOREIGN KEY ([RegistradoPorId]) REFERENCES [AspNetUsers] ([Id]),
        CONSTRAINT [FK_Asistencias_Estudiantes_EstudianteId] FOREIGN KEY ([EstudianteId]) REFERENCES [Estudiantes] ([IdEstudiante]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Asistencias_GruposEstudiantes_GrupoId] FOREIGN KEY ([GrupoId]) REFERENCES [GruposEstudiantes] ([GrupoId]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251006233817_AgregarTablaAsistencias'
)
BEGIN
    CREATE INDEX [IX_Asistencias_Estado] ON [Asistencias] ([Estado]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251006233817_AgregarTablaAsistencias'
)
BEGIN
    CREATE INDEX [IX_Asistencias_Fecha] ON [Asistencias] ([Fecha]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251006233817_AgregarTablaAsistencias'
)
BEGIN
    CREATE INDEX [IX_Asistencias_FechaRegistro] ON [Asistencias] ([FechaRegistro]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251006233817_AgregarTablaAsistencias'
)
BEGIN
    CREATE INDEX [IX_Asistencias_GrupoId] ON [Asistencias] ([GrupoId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251006233817_AgregarTablaAsistencias'
)
BEGIN
    CREATE INDEX [IX_Asistencias_ModificadoPorId] ON [Asistencias] ([ModificadoPorId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251006233817_AgregarTablaAsistencias'
)
BEGIN
    CREATE INDEX [IX_Asistencias_RegistradoPorId] ON [Asistencias] ([RegistradoPorId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251006233817_AgregarTablaAsistencias'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Asistencias_Unique_Estudiante_Grupo_Fecha] ON [Asistencias] ([EstudianteId], [GrupoId], [Fecha]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251006233817_AgregarTablaAsistencias'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20251006233817_AgregarTablaAsistencias', N'8.0.0');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251007045146_AgregarMateriaIdAAsistencias'
)
BEGIN
    DROP INDEX [IX_Asistencias_Unique_Estudiante_Grupo_Fecha] ON [Asistencias];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251007045146_AgregarMateriaIdAAsistencias'
)
BEGIN
    ALTER TABLE [Asistencias] ADD [MateriaId] int NOT NULL DEFAULT 0;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251007045146_AgregarMateriaIdAAsistencias'
)
BEGIN
    CREATE INDEX [IX_Asistencias_MateriaId] ON [Asistencias] ([MateriaId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251007045146_AgregarMateriaIdAAsistencias'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Asistencias_Unique_Estudiante_Grupo_Materia_Fecha] ON [Asistencias] ([EstudianteId], [GrupoId], [MateriaId], [Fecha]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251007045146_AgregarMateriaIdAAsistencias'
)
BEGIN
    ALTER TABLE [Asistencias] ADD CONSTRAINT [FK_Asistencias_Materias_MateriaId] FOREIGN KEY ([MateriaId]) REFERENCES [Materias] ([MateriaId]) ON DELETE NO ACTION;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251007045146_AgregarMateriaIdAAsistencias'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20251007045146_AgregarMateriaIdAAsistencias', N'8.0.0');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251012005027_AddEstudianteEmpadronamientoTable'
)
BEGIN
    CREATE TABLE [EstudiantesEmpadronamiento] (
        [IdEstudiante] int NOT NULL,
        [NumeroId] nvarchar(20) NULL,
        [FechaNacimiento] datetime2 NULL,
        [Genero] nvarchar(20) NULL,
        [Nacionalidad] nvarchar(50) NULL,
        [EstadoCivil] nvarchar(30) NULL,
        [Provincia] nvarchar(50) NULL,
        [Canton] nvarchar(50) NULL,
        [Distrito] nvarchar(50) NULL,
        [Barrio] nvarchar(100) NULL,
        [Senas] nvarchar(500) NULL,
        [TelefonoAlterno] nvarchar(20) NULL,
        [CorreoAlterno] nvarchar(100) NULL,
        [NombrePadre] nvarchar(100) NULL,
        [NombreMadre] nvarchar(100) NULL,
        [NombreTutor] nvarchar(100) NULL,
        [ContactoEmergencia] nvarchar(100) NULL,
        [TelefonoEmergencia] nvarchar(20) NULL,
        [RelacionEmergencia] nvarchar(50) NULL,
        [Alergias] nvarchar(500) NULL,
        [CondicionesMedicas] nvarchar(500) NULL,
        [Medicamentos] nvarchar(500) NULL,
        [SeguroMedico] nvarchar(100) NULL,
        [CentroMedicoHabitual] nvarchar(100) NULL,
        [InstitucionProcedencia] nvarchar(100) NULL,
        [UltimoNivelCursado] nvarchar(50) NULL,
        [PromedioAnterior] decimal(5,2) NULL,
        [AdaptacionesPrevias] nvarchar(500) NULL,
        [DocumentosRecibidosJson] NVARCHAR(MAX) NULL,
        [DocumentosPendientesJson] NVARCHAR(MAX) NULL,
        [FechaEntregaDocumentos] datetime2 NULL,
        [FechaVencimientoPoliza] datetime2 NULL,
        [EtapaActual] nvarchar(50) NULL,
        [FechaEtapa] datetime2 NULL,
        [UsuarioEtapa] nvarchar(100) NULL,
        [NotasInternas] NVARCHAR(MAX) NULL,
        [FechaCreacion] datetime2 NOT NULL DEFAULT (GETDATE()),
        [FechaModificacion] datetime2 NULL,
        [UsuarioCreacion] nvarchar(100) NULL,
        [UsuarioModificacion] nvarchar(100) NULL,
        CONSTRAINT [PK_EstudiantesEmpadronamiento] PRIMARY KEY ([IdEstudiante]),
        CONSTRAINT [FK_EstudiantesEmpadronamiento_Estudiantes_IdEstudiante] FOREIGN KEY ([IdEstudiante]) REFERENCES [Estudiantes] ([IdEstudiante]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251012005027_AddEstudianteEmpadronamientoTable'
)
BEGIN
    CREATE INDEX [IX_EstudianteEmpadronamiento_EtapaActual] ON [EstudiantesEmpadronamiento] ([EtapaActual]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251012005027_AddEstudianteEmpadronamientoTable'
)
BEGIN
    CREATE INDEX [IX_EstudianteEmpadronamiento_FechaCreacion] ON [EstudiantesEmpadronamiento] ([FechaCreacion]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251012005027_AddEstudianteEmpadronamientoTable'
)
BEGIN
    CREATE INDEX [IX_EstudianteEmpadronamiento_NumeroId] ON [EstudiantesEmpadronamiento] ([NumeroId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251012005027_AddEstudianteEmpadronamientoTable'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20251012005027_AddEstudianteEmpadronamientoTable', N'8.0.0');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251012012036_CreateEstudianteEmpadronamientoTable'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20251012012036_CreateEstudianteEmpadronamientoTable', N'8.0.0');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251016033744_AgregarEstadoEstudiante'
)
BEGIN
    ALTER TABLE [Estudiantes] ADD [Estado] int NOT NULL DEFAULT 0;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251016033744_AgregarEstadoEstudiante'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20251016033744_AgregarEstadoEstudiante', N'8.0.0');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251024040453_ActualizarEsquema_Profesores_2025'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20251024040453_ActualizarEsquema_Profesores_2025', N'8.0.0');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251024044925_ActualizacionBD_Enero2025'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20251024044925_ActualizacionBD_Enero2025', N'8.0.0');
END;
GO

COMMIT;
GO

