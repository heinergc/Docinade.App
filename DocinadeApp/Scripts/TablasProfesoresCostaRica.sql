-- ============================================================
-- SCRIPT PARA CREAR TABLAS DE PROFESORES - COSTA RICA
-- Sistema de Rúbricas Académicas
-- Fecha: 23 de octubre, 2025
-- ============================================================

-- Tabla de Provincias de Costa Rica
CREATE TABLE Provincias (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(50) NOT NULL,
    Codigo NVARCHAR(10) NOT NULL,
    Estado BIT DEFAULT 1
);

-- Tabla de Cantones
CREATE TABLE Cantones (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ProvinciaId INT NOT NULL,
    Nombre NVARCHAR(100) NOT NULL,
    Codigo NVARCHAR(10) NOT NULL,
    Estado BIT DEFAULT 1,
    CONSTRAINT FK_Cantones_Provincia FOREIGN KEY (ProvinciaId) REFERENCES Provincias(Id)
);

-- Tabla de Distritos
CREATE TABLE Distritos (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    CantonId INT NOT NULL,
    Nombre NVARCHAR(100) NOT NULL,
    Codigo NVARCHAR(10) NOT NULL,
    Estado BIT DEFAULT 1,
    CONSTRAINT FK_Distritos_Canton FOREIGN KEY (CantonId) REFERENCES Cantones(Id)
);

-- Tabla de Universidades/Instituciones
CREATE TABLE Instituciones (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(200) NOT NULL,
    Siglas NVARCHAR(20),
    TipoInstitucion NVARCHAR(50) NOT NULL, -- 'Universidad Pública', 'Universidad Privada', 'Colegio Técnico', 'Instituto'
    CodigoMEP NVARCHAR(20), -- Código del Ministerio de Educación Pública
    Telefono NVARCHAR(20),
    Email NVARCHAR(150),
    SitioWeb NVARCHAR(200),
    Direccion NVARCHAR(300),
    DistritoId INT,
    Estado BIT DEFAULT 1,
    FechaCreacion DATETIME2 DEFAULT GETDATE(),
    CONSTRAINT FK_Instituciones_Distrito FOREIGN KEY (DistritoId) REFERENCES Distritos(Id)
);

-- Tabla de Facultades/Departamentos
CREATE TABLE Facultades (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    InstitucionId INT NOT NULL,
    Nombre NVARCHAR(200) NOT NULL,
    Codigo NVARCHAR(20),
    Decano NVARCHAR(150),
    Email NVARCHAR(150),
    Telefono NVARCHAR(20),
    Estado BIT DEFAULT 1,
    FechaCreacion DATETIME2 DEFAULT GETDATE(),
    CONSTRAINT FK_Facultades_Institucion FOREIGN KEY (InstitucionId) REFERENCES Instituciones(Id)
);

-- Tabla de Escuelas/Departamentos
CREATE TABLE Escuelas (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    FacultadId INT NOT NULL,
    Nombre NVARCHAR(200) NOT NULL,
    Codigo NVARCHAR(20),
    Director NVARCHAR(150),
    Email NVARCHAR(150),
    Telefono NVARCHAR(20),
    Estado BIT DEFAULT 1,
    FechaCreacion DATETIME2 DEFAULT GETDATE(),
    CONSTRAINT FK_Escuelas_Facultad FOREIGN KEY (FacultadId) REFERENCES Facultades(Id)
);

-- Tabla Principal de Profesores
CREATE TABLE Profesores (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    
    -- Información Personal (Costa Rica)
    Nombres NVARCHAR(100) NOT NULL,
    PrimerApellido NVARCHAR(100) NOT NULL,
    SegundoApellido NVARCHAR(100),
    Cedula NVARCHAR(20) UNIQUE NOT NULL, -- Cédula costarricense: 1-1234-5678
    TipoCedula NVARCHAR(10) NOT NULL DEFAULT 'Nacional', -- 'Nacional', 'Residencia', 'Dimex'
    Sexo NCHAR(1) CHECK (Sexo IN ('M', 'F')),
    FechaNacimiento DATE,
    EstadoCivil NVARCHAR(20), -- 'Soltero', 'Casado', 'Divorciado', 'Viudo', 'Unión Libre'
    Nacionalidad NVARCHAR(50) DEFAULT 'Costarricense',
    
    -- Información de Contacto
    EmailPersonal NVARCHAR(150) UNIQUE NOT NULL,
    EmailInstitucional NVARCHAR(150) UNIQUE,
    TelefonoFijo NVARCHAR(20),
    TelefonoCelular NVARCHAR(20) NOT NULL,
    TelefonoOficina NVARCHAR(20),
    Extension NVARCHAR(10),
    
    -- Dirección
    DireccionExacta NVARCHAR(400),
    ProvinciaId INT,
    CantonId INT,
    DistritoId INT,
    CodigoPostal NVARCHAR(10),
    
    -- Estado y Control
    Estado BIT DEFAULT 1, -- 1 = Activo, 0 = Inactivo
    MotivoInactividad NVARCHAR(200),
    NotificacionesEmail BIT DEFAULT 1,
    NotificacionesSMS BIT DEFAULT 0,
    
    -- Información Adicional Académica
    AreasEspecializacion NVARCHAR(500),
    IdiomasHabla NVARCHAR(200), -- 'Español, Inglés, Francés'
    NivelIngles NVARCHAR(20), -- 'Básico', 'Intermedio', 'Avanzado', 'Nativo'
    ExperienciaDocente INT, -- Años de experiencia
    
    -- Información de Emergencia
    ContactoEmergenciaNombre NVARCHAR(150),
    ContactoEmergenciaParentesco NVARCHAR(50),
    ContactoEmergenciaTelefono NVARCHAR(20),
    
    -- Campos de Auditoría
    FechaCreacion DATETIME2 DEFAULT GETDATE(),
    CreadoPor NVARCHAR(100),
    FechaModificacion DATETIME2,
    ModificadoPor NVARCHAR(100),
    Version INT DEFAULT 1,
    
    -- Relaciones
    UsuarioId INT,
    
    -- Constraints
    CONSTRAINT FK_Profesores_Provincia FOREIGN KEY (ProvinciaId) REFERENCES Provincias(Id),
    CONSTRAINT FK_Profesores_Canton FOREIGN KEY (CantonId) REFERENCES Cantones(Id),
    CONSTRAINT FK_Profesores_Distrito FOREIGN KEY (DistritoId) REFERENCES Distritos(Id),
    CONSTRAINT CK_Profesores_TipoCedula CHECK (TipoCedula IN ('Nacional', 'Residencia', 'Dimex'))
);

-- Tabla de asignación Profesor-Grupo (Un profesor puede enseñar a varios grupos)
CREATE TABLE ProfesorGrupos (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ProfesorId INT NOT NULL,
    GrupoId INT NOT NULL, -- Relación con GruposEstudiantes existente
    PeriodoAcademicoId INT NOT NULL, -- Relación con PeriodosAcademicos existente
    MateriaId INT NOT NULL, -- Relación con Materias existente
    EsProfesorPrincipal BIT DEFAULT 1, -- Si es el profesor principal o asistente
    AulaAsignada NVARCHAR(50),
    Estado BIT DEFAULT 1,
    FechaAsignacion DATETIME2 DEFAULT GETDATE(),
    FechaInicio DATE,
    FechaFin DATE,
    Observaciones NVARCHAR(500),
    CONSTRAINT FK_ProfesorGrupos_Profesor FOREIGN KEY (ProfesorId) REFERENCES Profesores(Id),
    CONSTRAINT FK_ProfesorGrupos_Grupo FOREIGN KEY (GrupoId) REFERENCES GruposEstudiantes(GrupoId),
    CONSTRAINT FK_ProfesorGrupos_Periodo FOREIGN KEY (PeriodoAcademicoId) REFERENCES PeriodosAcademicos(Id),
    CONSTRAINT FK_ProfesorGrupos_Materia FOREIGN KEY (MateriaId) REFERENCES Materias(MateriaId),
    CONSTRAINT UQ_ProfesorGrupo_Periodo UNIQUE (ProfesorId, GrupoId, PeriodoAcademicoId, MateriaId)
);

-- Tabla de relación Profesor-Guía (Muchos a Muchos - Un profesor puede ser guía de varios grupos)
CREATE TABLE ProfesorGuia (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ProfesorId INT NOT NULL,
    GrupoId INT NOT NULL, -- Relación con GruposEstudiantes existente
    FechaAsignacion DATETIME2 DEFAULT GETDATE(),
    FechaInicio DATE,
    FechaFin DATE,
    Estado BIT DEFAULT 1, -- 1 = Activo, 0 = Inactivo
    Observaciones NVARCHAR(500),
    CONSTRAINT FK_ProfesorGuia_Profesor FOREIGN KEY (ProfesorId) REFERENCES Profesores(Id),
    CONSTRAINT FK_ProfesorGuia_Grupo FOREIGN KEY (GrupoId) REFERENCES GruposEstudiantes(GrupoId),
    CONSTRAINT UQ_ProfesorGuia_Grupo UNIQUE (ProfesorId, GrupoId) -- Un profesor solo puede ser guía una vez del mismo grupo
);

-- Tabla de Formación Académica del Profesor (Historial académico)
CREATE TABLE ProfesorFormacionAcademica (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ProfesorId INT NOT NULL,
    TipoFormacion NVARCHAR(50) NOT NULL, -- 'Bachillerato', 'Licenciatura', 'Maestría', 'Doctorado', 'Especialización'
    TituloObtenido NVARCHAR(200) NOT NULL,
    InstitucionEducativa NVARCHAR(200) NOT NULL,
    PaisInstitucion NVARCHAR(50) DEFAULT 'Costa Rica',
    AnioInicio INT,
    AnioFinalizacion INT,
    EnCurso BIT DEFAULT 0,
    PromedioGeneral DECIMAL(4,2),
    EsTituloReconocidoCONARE BIT DEFAULT 1,
    NumeroReconocimiento NVARCHAR(50),
    FechaCreacion DATETIME2 DEFAULT GETDATE(),
    CONSTRAINT FK_ProfesorFormacion_Profesor FOREIGN KEY (ProfesorId) REFERENCES Profesores(Id)
);

-- Tabla de Experiencia Laboral del Profesor
CREATE TABLE ProfesorExperienciaLaboral (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ProfesorId INT NOT NULL,
    NombreInstitucion NVARCHAR(200) NOT NULL,
    CargoDesempenado NVARCHAR(150) NOT NULL,
    TipoInstitucion NVARCHAR(50), -- 'Universidad', 'Colegio', 'Escuela', 'Empresa Privada'
    FechaInicio DATE NOT NULL,
    FechaFin DATE,
    TrabajandoActualmente BIT DEFAULT 0,
    DescripcionFunciones NVARCHAR(1000),
    TipoContrato NVARCHAR(50),
    JornadaLaboral NVARCHAR(50),
    FechaCreacion DATETIME2 DEFAULT GETDATE(),
    CONSTRAINT FK_ProfesorExperiencia_Profesor FOREIGN KEY (ProfesorId) REFERENCES Profesores(Id)
);

-- Tabla de Capacitaciones y Cursos del Profesor
CREATE TABLE ProfesorCapacitaciones (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ProfesorId INT NOT NULL,
    NombreCapacitacion NVARCHAR(200) NOT NULL,
    InstitucionOrganizadora NVARCHAR(200),
    TipoCapacitacion NVARCHAR(50), -- 'Curso', 'Taller', 'Seminario', 'Congreso', 'Diplomado'
    Modalidad NVARCHAR(20), -- 'Presencial', 'Virtual', 'Bimodal'
    FechaInicio DATE,
    FechaFin DATE,
    HorasCapacitacion INT,
    CertificadoObtenido BIT DEFAULT 0,
    CalificacionObtenida DECIMAL(4,2),
    AreaConocimiento NVARCHAR(100),
    FechaCreacion DATETIME2 DEFAULT GETDATE(),
    CONSTRAINT FK_ProfesorCapacitaciones_Profesor FOREIGN KEY (ProfesorId) REFERENCES Profesores(Id)
);

-- ============================================================
-- ÍNDICES PARA MEJORAR RENDIMIENTO
-- ============================================================

-- Índices principales para búsquedas frecuentes
CREATE INDEX IX_Profesores_Cedula ON Profesores(Cedula);
CREATE INDEX IX_Profesores_EmailPersonal ON Profesores(EmailPersonal);
CREATE INDEX IX_Profesores_EmailInstitucional ON Profesores(EmailInstitucional);
CREATE INDEX IX_Profesores_Estado ON Profesores(Estado);
CREATE INDEX IX_Profesores_Apellidos ON Profesores(PrimerApellido, SegundoApellido);
CREATE INDEX IX_Profesores_Provincia ON Profesores(ProvinciaId);
CREATE INDEX IX_Profesores_Canton ON Profesores(CantonId);
CREATE INDEX IX_Profesores_Distrito ON Profesores(DistritoId);

-- Índices para tablas relacionadas
CREATE INDEX IX_ProfesorGrupos_Profesor ON ProfesorGrupos(ProfesorId);
CREATE INDEX IX_ProfesorGrupos_Grupo ON ProfesorGrupos(GrupoId);
CREATE INDEX IX_ProfesorGrupos_Periodo ON ProfesorGrupos(PeriodoAcademicoId);
CREATE INDEX IX_ProfesorGrupos_Materia ON ProfesorGrupos(MateriaId);
CREATE INDEX IX_ProfesorGrupos_Estado ON ProfesorGrupos(Estado);

-- Índices para tabla ProfesorGuia
CREATE INDEX IX_ProfesorGuia_Profesor ON ProfesorGuia(ProfesorId);
CREATE INDEX IX_ProfesorGuia_Grupo ON ProfesorGuia(GrupoId);
CREATE INDEX IX_ProfesorGuia_Estado ON ProfesorGuia(Estado);
CREATE INDEX IX_ProfesorGuia_FechaAsignacion ON ProfesorGuia(FechaAsignacion);

-- ============================================================
-- DATOS INICIALES - DIVISIÓN TERRITORIAL DE COSTA RICA
-- ============================================================

-- Insertar Provincias
INSERT INTO Provincias (Nombre, Codigo) VALUES 
('San José', '1'),
('Alajuela', '2'),
('Cartago', '3'),
('Heredia', '4'),
('Guanacaste', '5'),
('Puntarenas', '6'),
('Limón', '7');

-- Insertar todos los Cantones de Costa Rica
-- Provincia San José (1)
INSERT INTO Cantones (ProvinciaId, Nombre, Codigo) VALUES 
(1, 'San José', '01'),
(1, 'Escazú', '02'),
(1, 'Desamparados', '03'),
(1, 'Puriscal', '04'),
(1, 'Tarrazú', '05'),
(1, 'Aserrí', '06'),
(1, 'Mora', '07'),
(1, 'Goicoechea', '08'),
(1, 'Santa Ana', '09'),
(1, 'Alajuelita', '10'),
(1, 'Coronado', '11'),
(1, 'Acosta', '12'),
(1, 'Tibás', '13'),
(1, 'Moravia', '14'),
(1, 'Montes de Oca', '15'),
(1, 'Turrubares', '16'),
(1, 'Dota', '17'),
(1, 'Curridabat', '18'),
(1, 'Pérez Zeledón', '19'),
(1, 'León Cortés Castro', '20'),

-- Provincia Alajuela (2)
(2, 'Alajuela', '01'),
(2, 'San Ramón', '02'),
(2, 'Grecia', '03'),
(2, 'San Mateo', '04'),
(2, 'Atenas', '05'),
(2, 'Naranjo', '06'),
(2, 'Palmares', '07'),
(2, 'Poás', '08'),
(2, 'Orotina', '09'),
(2, 'San Carlos', '10'),
(2, 'Alfaro Ruiz', '11'),
(2, 'Valverde Vega', '12'),
(2, 'Upala', '13'),
(2, 'Los Chiles', '14'),
(2, 'Guatuso', '15'),
(2, 'Río Cuarto', '16'),

-- Provincia Cartago (3)
(3, 'Cartago', '01'),
(3, 'Paraíso', '02'),
(3, 'La Unión', '03'),
(3, 'Jiménez', '04'),
(3, 'Turrialba', '05'),
(3, 'Alvarado', '06'),
(3, 'Oreamuno', '07'),
(3, 'El Guarco', '08'),

-- Provincia Heredia (4)
(4, 'Heredia', '01'),
(4, 'Barva', '02'),
(4, 'Santo Domingo', '03'),
(4, 'Santa Bárbara', '04'),
(4, 'San Rafael', '05'),
(4, 'San Isidro', '06'),
(4, 'Belén', '07'),
(4, 'Flores', '08'),
(4, 'San Pablo', '09'),
(4, 'Sarapiquí', '10'),

-- Provincia Guanacaste (5)
(5, 'Liberia', '01'),
(5, 'Nicoya', '02'),
(5, 'Santa Cruz', '03'),
(5, 'Bagaces', '04'),
(5, 'Carrillo', '05'),
(5, 'Cañas', '06'),
(5, 'Abangares', '07'),
(5, 'Tilarán', '08'),
(5, 'Nandayure', '09'),
(5, 'La Cruz', '10'),
(5, 'Hojancha', '11'),

-- Provincia Puntarenas (6)
(6, 'Puntarenas', '01'),
(6, 'Esparza', '02'),
(6, 'Buenos Aires', '03'),
(6, 'Montes de Oro', '04'),
(6, 'Osa', '05'),
(6, 'Quepos', '06'),
(6, 'Golfito', '07'),
(6, 'Coto Brus', '08'),
(6, 'Parrita', '09'),
(6, 'Corredores', '10'),
(6, 'Garabito', '11'),
(6, 'Monte Verde', '12'),

-- Provincia Limón (7)
(7, 'Limón', '01'),
(7, 'Pococí', '02'),
(7, 'Siquirres', '03'),
(7, 'Talamanca', '04'),
(7, 'Matina', '05'),
(7, 'Guácimo', '06');

-- Insertar Distritos principales de Costa Rica
-- PROVINCIA SAN JOSÉ
-- Cantón San José (1)
INSERT INTO Distritos (CantonId, Nombre, Codigo) VALUES 
(1, 'Carmen', '01'),
(1, 'Merced', '02'),
(1, 'Hospital', '03'),
(1, 'Catedral', '04'),
(1, 'Zapote', '05'),
(1, 'San Francisco de Dos Ríos', '06'),
(1, 'La Uruca', '07'),
(1, 'Mata Redonda', '08'),
(1, 'Pavas', '09'),
(1, 'Hatillo', '10'),
(1, 'San Sebastián', '11'),

-- Cantón Escazú (2)
(2, 'Escazú', '01'),
(2, 'San Antonio', '02'),
(2, 'San Rafael', '03'),

-- Cantón Desamparados (3)
(3, 'Desamparados', '01'),
(3, 'San Miguel', '02'),
(3, 'San Juan de Dios', '03'),
(3, 'San Rafael Arriba', '04'),
(3, 'San Antonio', '05'),
(3, 'Frailes', '06'),
(3, 'Patarrá', '07'),
(3, 'San Cristóbal', '08'),
(3, 'Rosario', '09'),
(3, 'Damas', '10'),
(3, 'San Rafael Abajo', '11'),
(3, 'Gravilias', '12'),
(3, 'Los Guido', '13'),

-- Cantón Pérez Zeledón (19)
(19, 'San Isidro de El General', '01'),
(19, 'El General', '02'),
(19, 'Daniel Flores', '03'),
(19, 'Rivas', '04'),
(19, 'San Pedro', '05'),
(19, 'Platanares', '06'),
(19, 'Pejibaye', '07'),
(19, 'Cajón', '08'),
(19, 'Barú', '09'),
(19, 'Río Nuevo', '10'),
(19, 'Páramo', '11'),

-- PROVINCIA ALAJUELA
-- Cantón Alajuela (21)
(21, 'Alajuela', '01'),
(21, 'San José', '02'),
(21, 'Carrizal', '03'),
(21, 'San Antonio', '04'),
(21, 'Guácima', '05'),
(21, 'San Isidro', '06'),
(21, 'Sabanilla', '07'),
(21, 'San Rafael', '08'),
(21, 'Río Segundo', '09'),
(21, 'Desamparados', '10'),
(21, 'Turrúcares', '11'),
(21, 'Tambor', '12'),
(21, 'Garita', '13'),
(21, 'Sarapiquí', '14'),

-- Cantón San Carlos (30)
(30, 'Quesada', '01'),
(30, 'Florencia', '02'),
(30, 'Buenavista', '03'),
(30, 'Aguas Zarcas', '04'),
(30, 'Venecia', '05'),
(30, 'Pital', '06'),
(30, 'La Fortuna', '07'),
(30, 'La Tigra', '08'),
(30, 'La Palmera', '09'),
(30, 'Venado', '10'),
(30, 'Cutris', '11'),
(30, 'Monterrey', '12'),
(30, 'Pocosol', '13'),

-- PROVINCIA CARTAGO
-- Cantón Cartago (37)
(37, 'Oriental', '01'),
(37, 'Occidental', '02'),
(37, 'Carmen', '03'),
(37, 'San Nicolás', '04'),
(37, 'Agua Caliente', '05'),
(37, 'Guadalupe', '06'),
(37, 'Corralillo', '07'),
(37, 'Tierra Blanca', '08'),
(37, 'Dulce Nombre', '09'),
(37, 'Llano Grande', '10'),
(37, 'Quebradilla', '11'),

-- Cantón Turrialba (41)
(41, 'Turrialba', '01'),
(41, 'La Suiza', '02'),
(41, 'Peralta', '03'),
(41, 'Santa Cruz', '04'),
(41, 'Santa Teresita', '05'),
(41, 'Pavones', '06'),
(41, 'Tuis', '07'),
(41, 'Tayutic', '08'),
(41, 'Santa Rosa', '09'),
(41, 'Tres Equis', '10'),
(41, 'La Isabel', '11'),
(41, 'Chirripó', '12'),

-- PROVINCIA HEREDIA
-- Cantón Heredia (45)
(45, 'Heredia', '01'),
(45, 'Mercedes', '02'),
(45, 'San Francisco', '03'),
(45, 'Ulloa', '04'),
(45, 'Varablanca', '05'),

-- Cantón Sarapiquí (54)
(54, 'Puerto Viejo', '01'),
(54, 'La Virgen', '02'),
(54, 'Las Horquetas', '03'),
(54, 'Llanuras del Gaspar', '04'),
(54, 'Cureña', '05'),

-- PROVINCIA GUANACASTE
-- Cantón Liberia (55)
(55, 'Liberia', '01'),
(55, 'Cañas Dulces', '02'),
(55, 'Mayorga', '03'),
(55, 'Nacascolo', '04'),
(55, 'Curubandé', '05'),

-- Cantón Nicoya (56)
(56, 'Nicoya', '01'),
(56, 'Mansión', '02'),
(56, 'San Antonio', '03'),
(56, 'Quebrada Honda', '04'),
(56, 'Sámara', '05'),
(56, 'Nosara', '06'),
(56, 'Belén de Nosarita', '07'),

-- PROVINCIA PUNTARENAS
-- Cantón Puntarenas (66)
(66, 'Puntarenas', '01'),
(66, 'Pitahaya', '02'),
(66, 'Chomes', '03'),
(66, 'Lepanto', '04'),
(66, 'Paquera', '05'),
(66, 'Manzanillo', '06'),
(66, 'Guacimal', '07'),
(66, 'Barranca', '08'),
(66, 'Monte Verde', '09'),
(66, 'Isla del Coco', '10'),
(66, 'Cóbano', '11'),
(66, 'Chacarita', '12'),
(66, 'Chira', '13'),
(66, 'Acapulco', '14'),
(66, 'El Roble', '15'),
(66, 'Arancibia', '16'),

-- Cantón Golfito (72)
(72, 'Golfito', '01'),
(72, 'Puerto Jiménez', '02'),
(72, 'Guaycará', '03'),
(72, 'Pavón', '04'),

-- PROVINCIA LIMÓN
-- Cantón Limón (78)
(78, 'Limón', '01'),
(78, 'Valle La Estrella', '02'),
(78, 'Río Blanco', '03'),
(78, 'Matama', '04'),

-- Cantón Pococí (79)
(79, 'Guápiles', '01'),
(79, 'Jiménez', '02'),
(79, 'Rita', '03'),
(79, 'Roxana', '04'),
(79, 'Cariari', '05'),
(79, 'Colorado', '06'),
(79, 'La Colonia', '07'),

-- Cantón Siquirres (80)
(80, 'Siquirres', '01'),
(80, 'Pacuarito', '02'),
(80, 'Florida', '03'),
(80, 'Germania', '04'),
(80, 'Cairo', '05'),
(80, 'Alegría', '06'),
(80, 'Reventazón', '07');

-- Insertar Instituciones Educativas de Costa Rica
INSERT INTO Instituciones (Nombre, Siglas, TipoInstitucion) VALUES 
-- Universidades Públicas
('Universidad de Costa Rica', 'UCR', 'Universidad Pública'),
('Tecnológico de Costa Rica', 'TEC', 'Universidad Pública'),
('Universidad Nacional', 'UNA', 'Universidad Pública'),
('Universidad Estatal a Distancia', 'UNED', 'Universidad Pública'),
('Universidad Técnica Nacional', 'UTN', 'Universidad Pública'),

-- Universidades Privadas principales
('Universidad Latina de Costa Rica', 'U.Latina', 'Universidad Privada'),
('Universidad Americana', 'UAM', 'Universidad Privada'),
('Universidad Fidélitas', 'UFIDELITAS', 'Universidad Privada'),
('Universidad San Marcos', 'USAM', 'Universidad Privada'),
('Universidad Metropolitana Castro Carazo', 'UMCA', 'Universidad Privada'),
('Universidad Internacional San Isidro Labrador', 'UISIL', 'Universidad Privada'),
('Universidad Hispanoamericana', 'UH', 'Universidad Privada'),
('Universidad Central', 'UC', 'Universidad Privada'),

-- Colegios Técnicos Profesionales principales
('Colegio Técnico Profesional de Alajuela', 'CTP Alajuela', 'Colegio Técnico'),
('Colegio Técnico Profesional de Cartago', 'CTP Cartago', 'Colegio Técnico'),
('Colegio Técnico Profesional de Heredia', 'CTP Heredia', 'Colegio Técnico'),
('Colegio Técnico Profesional de Puntarenas', 'CTP Puntarenas', 'Colegio Técnico'),
('Colegio Técnico Profesional de Limón', 'CTP Limón', 'Colegio Técnico'),
('Colegio Técnico Profesional de Liberia', 'CTP Liberia', 'Colegio Técnico'),

-- Institutos especializados
('Instituto Nacional de Aprendizaje', 'INA', 'Instituto'),
('Instituto Costarricense de Electricidad', 'ICE', 'Instituto'),
('Instituto de Desarrollo Profesional Uladislao Gámez Solano', 'IDP-UGS', 'Instituto');
