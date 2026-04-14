-- ============================================================
-- DATOS DE EJEMPLO PARA PROFESORES - COSTA RICA
-- Sistema de Rúbricas Académicas
-- Fecha: 23 de octubre, 2025
-- ============================================================

-- Completar datos geográficos adicionales

-- Más cantones de Alajuela
INSERT INTO Cantones (ProvinciaId, Nombre, Codigo) VALUES 
(2, 'Alajuela', '01'),
(2, 'San Ramón', '02'),
(2, 'Grecia', '03'),
(2, 'San Mateo', '04'),
(2, 'Atenas', '05'),
(2, 'Naranjo', '06'),
(2, 'Palmares', '07'),
(2, 'Poás', '08'),
(2, 'Orotina', '09'),
(2, 'San Carlos', '10');

-- Más cantones de Cartago
INSERT INTO Cantones (ProvinciaId, Nombre, Codigo) VALUES 
(3, 'Cartago', '01'),
(3, 'Paraíso', '02'),
(3, 'La Unión', '03'),
(3, 'Jiménez', '04'),
(3, 'Turrialba', '05'),
(3, 'Alvarado', '06'),
(3, 'Oreamuno', '07'),
(3, 'El Guarco', '08');

-- Distritos adicionales importantes
INSERT INTO Distritos (CantonId, Nombre, Codigo) VALUES 
-- San José
(2, 'Escazú', '01'),
(2, 'San Antonio', '02'),
(2, 'San Rafael', '03'),
-- Alajuela
(21, 'Alajuela', '01'),
(21, 'San José', '02'),
(21, 'Carrizal', '03'),
-- Cartago
(29, 'Oriental', '01'),
(29, 'Occidental', '02'),
(29, 'Carmen', '03');

-- Insertar más instituciones educativas
INSERT INTO Instituciones (Nombre, Siglas, TipoInstitucion, CodigoMEP, Telefono, Email, SitioWeb, DistritoId) VALUES 
('Universidad Latina de Costa Rica', 'ULatina', 'Universidad Privada', 'UL001', '2523-4000', 'info@ulatina.ac.cr', 'https://www.ulatina.ac.cr', 1),
('Universidad Veritas', 'Veritas', 'Universidad Privada', 'UV001', '2283-4747', 'info@veritas.cr', 'https://www.veritas.cr', 1),
('Universidad San Judas Tadeo', 'USJT', 'Universidad Privada', 'USJT001', '2296-3643', 'info@usanjudas.ac.cr', 'https://www.usanjudas.ac.cr', 1),
('CENFOTEC', 'CENFOTEC', 'Universidad Privada', 'CF001', '2281-1555', 'info@ucenfotec.ac.cr', 'https://www.ucenfotec.ac.cr', 1),
('Universidad Castro Carazo', 'UCC', 'Universidad Privada', 'UCC001', '2280-9090', 'info@ucc.ac.cr', 'https://www.ucc.ac.cr', 1);

-- Insertar Facultades
INSERT INTO Facultades (InstitucionId, Nombre, Codigo, Decano, Email, Telefono) VALUES 
-- UCR
(1, 'Facultad de Ingeniería', 'FI', 'Dr. Carlos Rodríguez Jiménez', 'decanato.ingenieria@ucr.ac.cr', '2511-4000'),
(1, 'Facultad de Ciencias Sociales', 'FCS', 'Dra. María Elena Vargas', 'decanato.sociales@ucr.ac.cr', '2511-4100'),
(1, 'Facultad de Educación', 'FE', 'Dr. Luis Alberto Morales', 'decanato.educacion@ucr.ac.cr', '2511-4200'),
(1, 'Facultad de Ciencias', 'FC', 'Dra. Ana Patricia Solís', 'decanato.ciencias@ucr.ac.cr', '2511-4300'),
-- TEC
(2, 'Escuela de Ingeniería en Computación', 'EIC', 'Dr. Roberto Marín Castro', 'director.computacion@tec.ac.cr', '2550-2225'),
(2, 'Escuela de Ingeniería Industrial', 'EII', 'Ing. Patricia Vega Rojas', 'director.industrial@tec.ac.cr', '2550-2226'),
(2, 'Escuela de Administración de Empresas', 'EAE', 'MBA. Carlos Fernández León', 'director.administracion@tec.ac.cr', '2550-2227');

-- Insertar Escuelas/Departamentos
INSERT INTO Escuelas (FacultadId, Nombre, Codigo, Director, Email, Telefono) VALUES 
-- Facultad de Ingeniería UCR
(1, 'Escuela de Ingeniería en Sistemas', 'EIS', 'Dr. Juan Carlos Méndez', 'director.sistemas@ucr.ac.cr', '2511-4010'),
(1, 'Escuela de Ingeniería Civil', 'EIC', 'Ing. Ana Lucía Vargas', 'director.civil@ucr.ac.cr', '2511-4011'),
(1, 'Escuela de Ingeniería Eléctrica', 'EIE', 'Dr. Roberto Silva Mata', 'director.electrica@ucr.ac.cr', '2511-4012'),
-- Facultad de Educación UCR
(3, 'Escuela de Formación Docente', 'EFD', 'Dra. Carmen Lyra Jiménez', 'director.docente@ucr.ac.cr', '2511-4210'),
(3, 'Escuela de Orientación y Educación Especial', 'EOEE', 'M.Ed. Laura Mora Coto', 'director.orientacion@ucr.ac.cr', '2511-4211'),
-- TEC
(5, 'Área de Ingeniería de Software', 'AIS', 'Dr. Miguel Ángel Rojas', 'area.software@tec.ac.cr', '2550-2230'),
(6, 'Área de Gestión de Proyectos', 'AGP', 'MBA. Silvia Castro Vega', 'area.proyectos@tec.ac.cr', '2550-2240');

-- Insertar Materias
INSERT INTO Materias (EscuelaId, Codigo, Nombre, Creditos, HorasTeoricas, HorasPracticas, Modalidad, Nivel) VALUES 
-- Ingeniería en Sistemas
(1, 'CI0101', 'Programación I', 4, 2, 4, 'Presencial', 'Bachillerato'),
(1, 'CI0102', 'Programación II', 4, 2, 4, 'Presencial', 'Bachillerato'),
(1, 'CI0201', 'Estructuras de Datos', 4, 3, 2, 'Presencial', 'Bachillerato'),
(1, 'CI0301', 'Bases de Datos I', 4, 3, 2, 'Presencial', 'Bachillerato'),
(1, 'CI0401', 'Ingeniería de Software I', 4, 3, 2, 'Presencial', 'Licenciatura'),
(1, 'CI0501', 'Sistemas Distribuidos', 4, 3, 2, 'Presencial', 'Licenciatura'),

-- Formación Docente
(4, 'FD0101', 'Fundamentos de Pedagogía', 3, 3, 0, 'Presencial', 'Bachillerato'),
(4, 'FD0201', 'Psicología Educativa', 3, 3, 0, 'Presencial', 'Bachillerato'),
(4, 'FD0301', 'Didáctica General', 4, 2, 4, 'Presencial', 'Bachillerato'),
(4, 'FD0401', 'Evaluación Educativa', 3, 2, 2, 'Bimodal', 'Bachillerato'),
(4, 'FD0501', 'Práctica Profesional', 6, 1, 10, 'Presencial', 'Licenciatura'),

-- TEC - Computación
(6, 'CE1101', 'Introducción a la Programación', 4, 2, 4, 'Presencial', 'Bachillerato'),
(6, 'CE2103', 'Algoritmos y Estructuras de Datos I', 4, 3, 2, 'Presencial', 'Bachillerato'),
(6, 'CE3101', 'Bases de Datos I', 4, 3, 2, 'Presencial', 'Bachillerato'),
(6, 'CE4101', 'Proyecto de Graduación I', 3, 1, 4, 'Presencial', 'Licenciatura');

-- ============================================================
-- INSERTAR PROFESORES DE EJEMPLO
-- ============================================================

INSERT INTO Profesores (
    Nombres, PrimerApellido, SegundoApellido, Cedula, TipoCedula, Sexo, FechaNacimiento, 
    EstadoCivil, EmailPersonal, EmailInstitucional, TelefonoCelular, DistritoId,
    GradoAcademico, TituloAcademico, InstitucionGraduacion, CodigoEmpleado, 
    FechaIngreso, TipoContrato, CategoriaLaboral, TipoJornada, HorasLaborales,
    EscuelaId, AreasEspecializacion, ExperienciaDocente, IdiomasHabla, NivelIngles
) VALUES 

-- Profesores de Ingeniería en Sistemas (UCR)
('Juan Carlos', 'Rodríguez', 'Jiménez', '1-1234-5678', 'Nacional', 'M', '1975-03-15',
 'Casado', 'juan.rodriguez@gmail.com', 'juan.rodriguez@ucr.ac.cr', '8888-1234', 1,
 'Doctorado', 'Doctor en Ciencias de la Computación', 'Universidad de Costa Rica', 'EMP001',
 '2010-02-01', 'Propiedad', 'Catedrático', 'Tiempo Completo', 40.00,
 1, 'Inteligencia Artificial, Machine Learning, Programación', 15, 'Español, Inglés', 'Avanzado'),

('María Elena', 'Vargas', 'Castro', '2-2345-6789', 'Nacional', 'F', '1980-07-22',
 'Soltera', 'maria.vargas@hotmail.com', 'maria.vargas@ucr.ac.cr', '8777-5678', 1,
 'Maestría', 'Maestría en Ingeniería de Software', 'Tecnológico de Costa Rica', 'EMP002',
 '2015-01-15', 'Propiedad', 'Profesor Asociado', 'Tiempo Completo', 40.00,
 1, 'Ingeniería de Software, Bases de Datos, Gestión de Proyectos', 10, 'Español, Inglés, Portugués', 'Intermedio'),

('Roberto', 'Silva', 'Mata', '1-3456-7890', 'Nacional', 'M', '1978-11-08',
 'Casado', 'roberto.silva@yahoo.com', 'roberto.silva@ucr.ac.cr', '8666-9012', 1,
 'Maestría', 'Maestría en Ciencias de la Computación', 'Universidad Nacional', 'EMP003',
 '2012-08-01', 'Propiedad', 'Profesor Asistente', 'Medio Tiempo', 20.00,
 1, 'Programación Web, Desarrollo Móvil, UI/UX', 8, 'Español, Inglés', 'Intermedio'),

-- Profesores de Formación Docente (UCR)
('Carmen', 'Lyra', 'Jiménez', '3-4567-8901', 'Nacional', 'F', '1972-05-30',
 'Divorciada', 'carmen.lyra@gmail.com', 'carmen.lyra@ucr.ac.cr', '8555-3456', 1,
 'Doctorado', 'Doctora en Educación', 'Universidad de Costa Rica', 'EMP004',
 '2008-03-01', 'Propiedad', 'Catedrática', 'Tiempo Completo', 40.00,
 4, 'Pedagogía, Didáctica, Evaluación Educativa', 17, 'Español, Inglés, Francés', 'Intermedio'),

('Luis Alberto', 'Morales', 'Vega', '1-5678-9012', 'Nacional', 'M', '1976-12-18',
 'Casado', 'luis.morales@outlook.com', 'luis.morales@ucr.ac.cr', '8444-7890', 1,
 'Maestría', 'Maestría en Psicología Educativa', 'Universidad Nacional', 'EMP005',
 '2011-01-15', 'Propiedad', 'Profesor Asociado', 'Tiempo Completo', 40.00,
 4, 'Psicología Educativa, Desarrollo Humano, Inclusión Educativa', 12, 'Español, Inglés', 'Básico'),

-- Profesores del TEC
('Miguel Ángel', 'Rojas', 'Fernández', '2-6789-0123', 'Nacional', 'M', '1983-09-25',
 'Soltero', 'miguel.rojas@gmail.com', 'mrojas@tec.ac.cr', '8333-2468', 21,
 'Doctorado', 'Doctor en Ingeniería de Software', 'Tecnológico de Costa Rica', 'TEC001',
 '2018-02-01', 'Propiedad', 'Profesor Asistente', 'Tiempo Completo', 40.00,
 6, 'Desarrollo de Software, Metodologías Ágiles, DevOps', 7, 'Español, Inglés', 'Avanzado'),

('Ana Lucía', 'Chacón', 'Solís', '1-7890-1234', 'Nacional', 'F', '1985-04-12',
 'Casada', 'ana.chacon@hotmail.com', 'achacon@tec.ac.cr', '8222-1357', 21,
 'Maestría', 'Maestría en Administración de Tecnologías de Información', 'Universidad Latina', 'TEC002',
 '2019-08-01', 'Interino', 'Instructor', 'Medio Tiempo', 20.00,
 7, 'Gestión de Proyectos TI, Análisis de Sistemas, Consultoría', 5, 'Español, Inglés', 'Intermedio'),

-- Professor extranjero
('James', 'Patterson', 'Smith', '184756291', 'Residencia', 'M', '1979-01-20',
 'Casado', 'james.patterson@gmail.com', 'jpatterson@ucr.ac.cr', '8111-9876', 1,
 'Doctorado', 'PhD in Computer Science', 'Stanford University', 'EMP006',
 '2020-01-15', 'Propiedad', 'Profesor Asociado', 'Tiempo Completo', 40.00,
 1, 'Artificial Intelligence, Data Science, Machine Learning', 12, 'Inglés, Español', 'Nativo');

-- ============================================================
-- ASIGNACIÓN DE MATERIAS A PROFESORES (CICLO ACTUAL)
-- ============================================================

INSERT INTO ProfesorMaterias (ProfesorId, MateriaId, Ciclo, Anio, Grupo, HorarioClases, AulaAsignada, CantidadEstudiantes) VALUES 
-- Juan Carlos Rodríguez
(1, 3, 'II Ciclo 2025', 2025, '01', 'Lunes y Miércoles 08:00-10:00', 'Aula 101', 35),
(1, 5, 'II Ciclo 2025', 2025, '01', 'Martes y Jueves 10:00-12:00', 'Lab 201', 25),

-- María Elena Vargas
(2, 2, 'II Ciclo 2025', 2025, '01', 'Lunes y Miércoles 14:00-16:00', 'Lab 102', 30),
(2, 4, 'II Ciclo 2025', 2025, '01', 'Martes y Jueves 08:00-10:00', 'Aula 205', 28),
(2, 6, 'II Ciclo 2025', 2025, '01', 'Viernes 14:00-18:00', 'Lab 301', 20),

-- Roberto Silva
(3, 1, 'II Ciclo 2025', 2025, '02', 'Martes y Jueves 18:00-20:00', 'Aula 103', 40),

-- Carmen Lyra
(4, 7, 'II Ciclo 2025', 2025, '01', 'Lunes 08:00-11:00', 'Aula 301', 45),
(4, 9, 'II Ciclo 2025', 2025, '01', 'Miércoles y Viernes 10:00-12:00', 'Aula 302', 35),
(4, 10, 'II Ciclo 2025', 2025, '01', 'Martes y Jueves 14:00-16:00', 'Aula 303', 30),

-- Luis Alberto Morales
(5, 8, 'II Ciclo 2025', 2025, '01', 'Lunes y Miércoles 16:00-18:00', 'Aula 304', 38),
(5, 11, 'II Ciclo 2025', 2025, '01', 'Jueves y Viernes 08:00-11:00', 'Campo', 15),

-- Miguel Ángel Rojas (TEC)
(6, 12, 'II Ciclo 2025', 2025, '01', 'Lunes y Miércoles 08:00-10:00', 'Lab TEC-101', 32),
(6, 13, 'II Ciclo 2025', 2025, '01', 'Martes y Jueves 14:00-16:00', 'Lab TEC-102', 28),

-- Ana Lucía Chacón (TEC)
(7, 14, 'II Ciclo 2025', 2025, '01', 'Miércoles 18:00-21:00', 'Aula TEC-201', 25),

-- James Patterson
(8, 5, 'II Ciclo 2025', 2025, '02', 'Lunes y Miércoles 10:00-12:00', 'Lab 202', 22),
(8, 6, 'II Ciclo 2025', 2025, '02', 'Viernes 08:00-12:00', 'Lab 302', 18);

-- ============================================================
-- FORMACIÓN ACADÉMICA DE LOS PROFESORES
-- ============================================================

INSERT INTO ProfesorFormacionAcademica (ProfesorId, TipoFormacion, TituloObtenido, InstitucionEducativa, PaisInstitucion, AnioInicio, AnioFinalizacion, PromedioGeneral, EsTituloReconocidoCONARE) VALUES 
-- Juan Carlos Rodríguez
(1, 'Bachillerato', 'Bachiller en Ingeniería en Sistemas', 'Universidad de Costa Rica', 'Costa Rica', 1993, 1997, 8.5, 1),
(1, 'Licenciatura', 'Licenciado en Ingeniería en Sistemas', 'Universidad de Costa Rica', 'Costa Rica', 1997, 1999, 8.8, 1),
(1, 'Maestría', 'Maestría en Ciencias de la Computación', 'Universidad de Costa Rica', 'Costa Rica', 2000, 2003, 9.1, 1),
(1, 'Doctorado', 'Doctor en Ciencias de la Computación', 'Universidad de Costa Rica', 'Costa Rica', 2004, 2009, 9.3, 1),

-- María Elena Vargas
(2, 'Bachillerato', 'Bachiller en Ingeniería en Computación', 'Tecnológico de Costa Rica', 'Costa Rica', 1998, 2002, 8.7, 1),
(2, 'Licenciatura', 'Licenciada en Ingeniería en Computación', 'Tecnológico de Costa Rica', 'Costa Rica', 2002, 2004, 9.0, 1),
(2, 'Maestría', 'Maestría en Ingeniería de Software', 'Tecnológico de Costa Rica', 'Costa Rica', 2010, 2013, 9.2, 1),

-- Carmen Lyra
(4, 'Bachillerato', 'Bachiller en Ciencias de la Educación', 'Universidad Nacional', 'Costa Rica', 1990, 1994, 8.9, 1),
(4, 'Licenciatura', 'Licenciada en Educación Primaria', 'Universidad Nacional', 'Costa Rica', 1994, 1996, 9.1, 1),
(4, 'Maestría', 'Maestría en Educación', 'Universidad de Costa Rica', 'Costa Rica', 1997, 2000, 9.3, 1),
(4, 'Doctorado', 'Doctora en Educación', 'Universidad de Costa Rica', 'Costa Rica', 2002, 2007, 9.4, 1),

-- James Patterson
(8, 'Bachillerato', 'Bachelor in Computer Science', 'University of California', 'Estados Unidos', 1997, 2001, NULL, 1),
(8, 'Maestría', 'Master in Computer Science', 'Stanford University', 'Estados Unidos', 2001, 2003, NULL, 1),
(8, 'Doctorado', 'PhD in Computer Science', 'Stanford University', 'Estados Unidos', 2003, 2008, NULL, 1);

-- ============================================================
-- EXPERIENCIA LABORAL DE LOS PROFESORES
-- ============================================================

INSERT INTO ProfesorExperienciaLaboral (ProfesorId, NombreInstitucion, CargoDesempenado, TipoInstitucion, FechaInicio, FechaFin, DescripcionFunciones, TipoContrato) VALUES 
-- Juan Carlos Rodríguez
(1, 'Empresa de Software ABC', 'Desarrollador Senior', 'Empresa Privada', '1999-06-01', '2010-01-31', 'Desarrollo de aplicaciones empresariales, liderazgo de equipos técnicos', 'Tiempo Completo'),
(1, 'Universidad de Costa Rica', 'Profesor Catedrático', 'Universidad', '2010-02-01', NULL, 'Docencia e investigación en el área de sistemas computacionales', 'Propiedad'),

-- Carmen Lyra
(4, 'Escuela República de Honduras', 'Maestra de Primaria', 'Escuela', '1996-02-01', '2008-02-28', 'Docencia en educación primaria, coordinación académica', 'Propiedad'),
(4, 'Universidad de Costa Rica', 'Profesora Catedrática', 'Universidad', '2008-03-01', NULL, 'Formación de docentes, investigación educativa', 'Propiedad'),

-- James Patterson
(8, 'Google Inc.', 'Software Engineer', 'Empresa Privada', '2008-09-01', '2015-12-31', 'Development of AI algorithms and machine learning systems', 'Tiempo Completo'),
(8, 'Stanford University', 'Research Associate', 'Universidad', '2016-01-01', '2019-12-31', 'Research in artificial intelligence and data science', 'Temporal'),
(8, 'Universidad de Costa Rica', 'Profesor Asociado', 'Universidad', '2020-01-15', NULL, 'Teaching and research in computer science and AI', 'Propiedad');

-- ============================================================
-- CAPACITACIONES RECIENTES
-- ============================================================

INSERT INTO ProfesorCapacitaciones (ProfesorId, NombreCapacitacion, InstitucionOrganizadora, TipoCapacitacion, Modalidad, FechaInicio, FechaFin, HorasCapacitacion, CertificadoObtenido, AreaConocimiento) VALUES 
-- Capacitaciones 2024-2025
(1, 'Machine Learning Avanzado', 'Coursera - Stanford', 'Curso', 'Virtual', '2024-01-15', '2024-03-15', 40, 1, 'Inteligencia Artificial'),
(1, 'Metodologías Ágiles en Educación Superior', 'Universidad Nacional', 'Taller', 'Presencial', '2024-06-10', '2024-06-12', 16, 1, 'Educación'),

(2, 'DevOps para Educadores', 'Tecnológico de Costa Rica', 'Seminario', 'Bimodal', '2024-08-20', '2024-08-22', 20, 1, 'Tecnología'),
(2, 'Gestión de Proyectos Educativos', 'CONARE', 'Diplomado', 'Virtual', '2025-02-01', '2025-06-01', 120, 0, 'Administración'),

(4, 'Neuroeducación y Aprendizaje', 'Universidad Latina', 'Congreso', 'Presencial', '2024-11-15', '2024-11-17', 24, 1, 'Educación'),
(4, 'Tecnologías Educativas Emergentes', 'UNED', 'Curso', 'Virtual', '2025-01-10', '2025-04-10', 60, 0, 'Tecnología Educativa'),

(8, 'Advanced AI Ethics', 'MIT OpenCourseWare', 'Curso', 'Virtual', '2024-09-01', '2024-12-01', 80, 1, 'Ética en IA');

GO

-- ============================================================
-- CONSULTAS DE VERIFICACIÓN
-- ============================================================

-- Verificar que los datos se insertaron correctamente
SELECT 'Profesores insertados:' AS Descripcion, COUNT(*) AS Cantidad FROM Profesores;
SELECT 'Materias insertadas:' AS Descripcion, COUNT(*) AS Cantidad FROM Materias;
SELECT 'Asignaciones de materias:' AS Descripcion, COUNT(*) AS Cantidad FROM ProfesorMaterias;

-- Mostrar un resumen de profesores por institución
SELECT 
    i.Nombre AS Institucion,
    COUNT(p.Id) AS CantidadProfesores
FROM Profesores p
INNER JOIN Escuelas e ON p.EscuelaId = e.Id
INNER JOIN Facultades f ON e.FacultadId = f.Id
INNER JOIN Instituciones i ON f.InstitucionId = i.Id
WHERE p.Estado = 'Activo'
GROUP BY i.Nombre;

-- Mostrar carga académica actual
SELECT * FROM vw_CargaAcademicaProfesores ORDER BY TotalEstudiantes DESC;
