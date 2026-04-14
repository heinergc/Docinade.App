-- Script para crear las tablas relacionadas con Profesor
-- Ejecutar en SQL Server

-- Tabla ProfesorFormacionAcademica
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ProfesorFormacionAcademica')
BEGIN
    CREATE TABLE ProfesorFormacionAcademica (
        Id INT PRIMARY KEY IDENTITY(1,1),
        ProfesorId INT NOT NULL,
        TipoFormacion NVARCHAR(50) NOT NULL,
        InstitucionEducativa NVARCHAR(200) NOT NULL,
        PaisInstitucion NVARCHAR(50) NOT NULL DEFAULT 'Costa Rica',
        TituloObtenido NVARCHAR(200) NOT NULL,
        AnioInicio INT NULL,
        AnioFinalizacion INT NULL,
        EnCurso BIT NOT NULL DEFAULT 0,
        PromedioGeneral DECIMAL(4,2) NULL,
        EsTituloReconocidoCONARE BIT NOT NULL DEFAULT 0,
        NumeroReconocimiento NVARCHAR(50) NULL,
        FechaCreacion DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT FK_ProfesorFormacionAcademica_Profesores FOREIGN KEY (ProfesorId) 
            REFERENCES Profesores(Id) ON DELETE CASCADE
    );
END
GO

-- Tabla ProfesorExperienciaLaboral
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ProfesorExperienciaLaboral')
BEGIN
    CREATE TABLE ProfesorExperienciaLaboral (
        Id INT PRIMARY KEY IDENTITY(1,1),
        ProfesorId INT NOT NULL,
        TipoInstitucion NVARCHAR(50) NOT NULL,
        NombreInstitucion NVARCHAR(200) NOT NULL,
        CargoDesempenado NVARCHAR(100) NOT NULL,
        FechaInicio DATETIME2 NOT NULL,
        FechaFin DATETIME2 NULL,
        TrabajandoActualmente BIT NOT NULL DEFAULT 0,
        TipoContrato NVARCHAR(50) NULL,
        JornadaLaboral NVARCHAR(50) NULL,
        DescripcionFunciones NVARCHAR(1000) NULL,
        FechaCreacion DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT FK_ProfesorExperienciaLaboral_Profesores FOREIGN KEY (ProfesorId) 
            REFERENCES Profesores(Id) ON DELETE CASCADE
    );
END
GO

-- Tabla ProfesorCapacitacion
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ProfesorCapacitacion')
BEGIN
    CREATE TABLE ProfesorCapacitacion (
        Id INT PRIMARY KEY IDENTITY(1,1),
        ProfesorId INT NOT NULL,
        TipoCapacitacion NVARCHAR(50) NOT NULL,
        NombreCapacitacion NVARCHAR(200) NOT NULL,
        InstitucionOrganizadora NVARCHAR(200) NOT NULL,
        AreaConocimiento NVARCHAR(100) NULL,
        Modalidad NVARCHAR(50) NULL,
        FechaInicio DATETIME2 NOT NULL,
        FechaFin DATETIME2 NULL,
        HorasCapacitacion INT NULL,
        CalificacionObtenida NVARCHAR(20) NULL,
        CertificadoObtenido BIT NOT NULL DEFAULT 0,
        FechaCreacion DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT FK_ProfesorCapacitacion_Profesores FOREIGN KEY (ProfesorId) 
            REFERENCES Profesores(Id) ON DELETE CASCADE
    );
END
GO

-- Tabla ProfesorGrupo
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ProfesorGrupo')
BEGIN
    CREATE TABLE ProfesorGrupo (
        Id INT PRIMARY KEY IDENTITY(1,1),
        ProfesorId INT NOT NULL,
        GrupoId INT NOT NULL,
        MateriaId INT NOT NULL,
        PeriodoAcademicoId INT NOT NULL,
        EsProfesorPrincipal BIT NOT NULL DEFAULT 1,
        FechaAsignacion DATETIME2 NOT NULL DEFAULT GETDATE(),
        FechaInicio DATETIME2 NULL,
        FechaFin DATETIME2 NULL,
        Estado NVARCHAR(20) NOT NULL DEFAULT 'Activo',
        AulaAsignada NVARCHAR(50) NULL,
        Observaciones NVARCHAR(500) NULL,
        CONSTRAINT FK_ProfesorGrupo_Profesores FOREIGN KEY (ProfesorId) 
            REFERENCES Profesores(Id) ON DELETE CASCADE,
        CONSTRAINT FK_ProfesorGrupo_GruposEstudiantes FOREIGN KEY (GrupoId) 
            REFERENCES GruposEstudiantes(GrupoId) ON DELETE NO ACTION,
        CONSTRAINT FK_ProfesorGrupo_Materias FOREIGN KEY (MateriaId) 
            REFERENCES Materias(MateriaId) ON DELETE NO ACTION,
        CONSTRAINT FK_ProfesorGrupo_PeriodosAcademicos FOREIGN KEY (PeriodoAcademicoId) 
            REFERENCES PeriodosAcademicos(Id) ON DELETE NO ACTION
    );
END
GO

-- Tabla ProfesorGuia
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ProfesorGuia')
BEGIN
    CREATE TABLE ProfesorGuia (
        Id INT PRIMARY KEY IDENTITY(1,1),
        ProfesorId INT NOT NULL,
        GrupoId INT NOT NULL,
        FechaAsignacion DATETIME2 NOT NULL DEFAULT GETDATE(),
        FechaInicio DATETIME2 NULL,
        FechaFin DATETIME2 NULL,
        Estado NVARCHAR(20) NOT NULL DEFAULT 'Activo',
        Observaciones NVARCHAR(500) NULL,
        CONSTRAINT FK_ProfesorGuia_Profesores FOREIGN KEY (ProfesorId) 
            REFERENCES Profesores(Id) ON DELETE CASCADE,
        CONSTRAINT FK_ProfesorGuia_GruposEstudiantes FOREIGN KEY (GrupoId) 
            REFERENCES GruposEstudiantes(GrupoId) ON DELETE NO ACTION
    );
END
GO

PRINT 'Tablas relacionadas de Profesor creadas exitosamente';
