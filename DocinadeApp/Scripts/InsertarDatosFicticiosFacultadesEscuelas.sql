-- Script para insertar datos ficticios en las tablas Facultades y Escuelas
-- Ejecutar en la base de datos RubricasDb

PRINT 'Iniciando inserción de datos ficticios en Facultades y Escuelas...'

-- ==================================================
-- INSERTAR FACULTADES FICTICIAS
-- ==================================================

-- Universidad de Costa Rica (Id = 1)
INSERT INTO Facultades (InstitucionId, Nombre, Codigo, Decano, Email, Telefono, Estado, FechaCreacion)
VALUES 
(1, 'Facultad de Ingeniería', 'ING-UCR', 'Dr. Carlos Méndez Rojas', 'decano.ingenieria@ucr.ac.cr', '2511-6000', 1, GETDATE()),
(1, 'Facultad de Ciencias Sociales', 'CS-UCR', 'Dra. María Elena Vargas', 'decano.sociales@ucr.ac.cr', '2511-6100', 1, GETDATE()),
(1, 'Facultad de Educación', 'EDU-UCR', 'Dr. Luis Alberto Chinchilla', 'decano.educacion@ucr.ac.cr', '2511-6200', 1, GETDATE()),
(1, 'Facultad de Ciencias Exactas y Naturales', 'CEN-UCR', 'Dra. Ana Lucía Mora', 'decano.exactas@ucr.ac.cr', '2511-6300', 1, GETDATE());

-- Tecnológico de Costa Rica (Id = 2)
INSERT INTO Facultades (InstitucionId, Nombre, Codigo, Decano, Email, Telefono, Estado, FechaCreacion)
VALUES 
(2, 'Escuela de Ingeniería en Computación', 'EIC-TEC', 'Dr. Roberto Solís Mora', 'decano.computacion@tec.ac.cr', '2550-2225', 1, GETDATE()),
(2, 'Escuela de Ingeniería Industrial', 'EII-TEC', 'Dr. Fernando Alvarado Castro', 'decano.industrial@tec.ac.cr', '2550-2230', 1, GETDATE()),
(2, 'Escuela de Administración de Empresas', 'EAE-TEC', 'Dra. Patricia Mora Castellanos', 'decano.administracion@tec.ac.cr', '2550-2240', 1, GETDATE());

-- Universidad Nacional (Id = 3)
INSERT INTO Facultades (InstitucionId, Nombre, Codigo, Decano, Email, Telefono, Estado, FechaCreacion)
VALUES 
(3, 'Facultad de Ciencias Exactas y Naturales', 'CEN-UNA', 'Dr. José Manuel Arroyo', 'decano.exactas@una.ac.cr', '2277-3000', 1, GETDATE()),
(3, 'Facultad de Educación', 'EDU-UNA', 'Dra. Silvia Charpentier Mora', 'decano.educacion@una.ac.cr', '2277-3100', 1, GETDATE()),
(3, 'Facultad de Ciencias Sociales', 'CS-UNA', 'Dr. Rolando Bolaños Garita', 'decano.sociales@una.ac.cr', '2277-3200', 1, GETDATE());

-- Universidad Latina (Id = 6)
INSERT INTO Facultades (InstitucionId, Nombre, Codigo, Decano, Email, Telefono, Estado, FechaCreacion)
VALUES 
(6, 'Facultad de Ingeniería y Tecnología', 'ING-ULATINA', 'Dr. Mario Rodríguez Vega', 'decano.ingenieria@ulatina.ac.cr', '2203-7000', 1, GETDATE()),
(6, 'Facultad de Ciencias Empresariales', 'CE-ULATINA', 'Dra. Carmen Solís Jiménez', 'decano.empresariales@ulatina.ac.cr', '2203-7100', 1, GETDATE()),
(6, 'Facultad de Educación', 'EDU-ULATINA', 'Dr. Alejandro Venegas Cruz', 'decano.educacion@ulatina.ac.cr', '2203-7200', 1, GETDATE());

PRINT 'Facultades insertadas correctamente.'

-- ==================================================
-- INSERTAR ESCUELAS FICTICIAS
-- ==================================================

-- Escuelas para Facultad de Ingeniería UCR (Id = 1)
INSERT INTO Escuelas (FacultadId, Nombre, Codigo, Director, Email, Telefono, Estado, FechaCreacion)
VALUES 
(1, 'Escuela de Ingeniería en Sistemas y Computación', 'EISC-UCR', 'Dr. Gabriel Aguilera Serrano', 'director.sistemas@ucr.ac.cr', '2511-6001', 1, GETDATE()),
(1, 'Escuela de Ingeniería Civil', 'EIC-UCR', 'Ing. María José Ramírez', 'director.civil@ucr.ac.cr', '2511-6002', 1, GETDATE()),
(1, 'Escuela de Ingeniería Eléctrica', 'EIE-UCR', 'Dr. Carlos Morera Beita', 'director.electrica@ucr.ac.cr', '2511-6003', 1, GETDATE()),
(1, 'Escuela de Ingeniería Mecánica', 'EIM-UCR', 'Ing. Rafael Castro Vindas', 'director.mecanica@ucr.ac.cr', '2511-6004', 1, GETDATE());

-- Escuelas para Facultad de Ciencias Sociales UCR (Id = 2)
INSERT INTO Escuelas (FacultadId, Nombre, Codigo, Director, Email, Telefono, Estado, FechaCreacion)
VALUES 
(2, 'Escuela de Psicología', 'EPSI-UCR', 'Dra. Vanessa Smith Castro', 'director.psicologia@ucr.ac.cr', '2511-6101', 1, GETDATE()),
(2, 'Escuela de Trabajo Social', 'ETS-UCR', 'MSc. Ana Cecilia Escalante', 'director.trabajosocial@ucr.ac.cr', '2511-6102', 1, GETDATE()),
(2, 'Escuela de Sociología', 'ESOC-UCR', 'Dr. Manuel Solís Avendaño', 'director.sociologia@ucr.ac.cr', '2511-6103', 1, GETDATE());

-- Escuelas para Facultad de Educación UCR (Id = 3)
INSERT INTO Escuelas (FacultadId, Nombre, Codigo, Director, Email, Telefono, Estado, FechaCreacion)
VALUES 
(3, 'Escuela de Formación Docente', 'EFD-UCR', 'Dra. Isabel Román Vega', 'director.formacion@ucr.ac.cr', '2511-6201', 1, GETDATE()),
(3, 'Escuela de Orientación y Educación Especial', 'EOEE-UCR', 'MSc. Jorge Cardona Andujar', 'director.orientacion@ucr.ac.cr', '2511-6202', 1, GETDATE()),
(3, 'Escuela de Educación Física y Deportes', 'EEFD-UCR', 'Dr. Walter Salazar Rojas', 'director.educfisica@ucr.ac.cr', '2511-6203', 1, GETDATE());

-- Escuelas para Facultad de Ciencias Exactas UCR (Id = 4)
INSERT INTO Escuelas (FacultadId, Nombre, Codigo, Director, Email, Telefono, Estado, FechaCreacion)
VALUES 
(4, 'Escuela de Matemática', 'EMAT-UCR', 'Dr. Edwin Chaves Esquivel', 'director.matematica@ucr.ac.cr', '2511-6301', 1, GETDATE()),
(4, 'Escuela de Física', 'EFIS-UCR', 'Dra. Cristina Arguedas Matarrita', 'director.fisica@ucr.ac.cr', '2511-6302', 1, GETDATE()),
(4, 'Escuela de Química', 'EQ-UCR', 'Dr. José Vega Baudrit', 'director.quimica@ucr.ac.cr', '2511-6303', 1, GETDATE()),
(4, 'Escuela de Biología', 'EB-UCR', 'Dra. Giselle Tamayo Castillo', 'director.biologia@ucr.ac.cr', '2511-6304', 1, GETDATE());

-- Escuelas para TEC - Ingeniería en Computación (Id = 5)
INSERT INTO Escuelas (FacultadId, Nombre, Codigo, Director, Email, Telefono, Estado, FechaCreacion)
VALUES 
(5, 'Área Académica de Ingeniería en Computadores', 'AAIC-TEC', 'Dr. Francisco Torres Rojas', 'director.computadores@tec.ac.cr', '2550-2226', 1, GETDATE()),
(5, 'Área Académica de Ingeniería en Software', 'AAIS-TEC', 'MSc. Gabriela Marín Raventós', 'director.software@tec.ac.cr', '2550-2227', 1, GETDATE()),
(5, 'Área Académica de Administración de TI', 'AAATI-TEC', 'Dr. Mauricio Araya Vargas', 'director.adminti@tec.ac.cr', '2550-2228', 1, GETDATE());

-- Escuelas para TEC - Ingeniería Industrial (Id = 6)
INSERT INTO Escuelas (FacultadId, Nombre, Codigo, Director, Email, Telefono, Estado, FechaCreacion)
VALUES 
(6, 'Área Académica de Gestión de la Producción', 'AAGP-TEC', 'Ing. Sandra Cauffman Jiménez', 'director.produccion@tec.ac.cr', '2550-2231', 1, GETDATE()),
(6, 'Área Académica de Gestión de la Calidad', 'AAGC-TEC', 'Dr. Luis Paulino Vargas', 'director.calidad@tec.ac.cr', '2550-2232', 1, GETDATE());

-- Escuelas para TEC - Administración (Id = 7)
INSERT INTO Escuelas (FacultadId, Nombre, Codigo, Director, Email, Telefono, Estado, FechaCreacion)
VALUES 
(7, 'Área Académica de Gestión Empresarial', 'AAGE-TEC', 'MBA. Carlos Hernández López', 'director.empresarial@tec.ac.cr', '2550-2241', 1, GETDATE()),
(7, 'Área Académica de Contaduría Pública', 'AACP-TEC', 'CPA. Ana Lorena Vargas', 'director.contaduria@tec.ac.cr', '2550-2242', 1, GETDATE());

-- Escuelas para UNA - Ciencias Exactas (Id = 8)
INSERT INTO Escuelas (FacultadId, Nombre, Codigo, Director, Email, Telefono, Estado, FechaCreacion)
VALUES 
(8, 'Escuela de Informática', 'EI-UNA', 'Dr. Franklin Hernández Castro', 'director.informatica@una.ac.cr', '2277-3001', 1, GETDATE()),
(8, 'Escuela de Matemática', 'EM-UNA', 'Dra. Jessica Ramírez González', 'director.matematica@una.ac.cr', '2277-3002', 1, GETDATE()),
(8, 'Escuela de Química', 'EQ-UNA', 'Dr. José Pablo Sibaja Ballestero', 'director.quimica@una.ac.cr', '2277-3003', 1, GETDATE());

-- Escuelas para UNA - Educación (Id = 9)
INSERT INTO Escuelas (FacultadId, Nombre, Codigo, Director, Email, Telefono, Estado, FechaCreacion)
VALUES 
(9, 'División de Educación Básica', 'DEB-UNA', 'Dra. Jacqueline García Fallas', 'director.basica@una.ac.cr', '2277-3101', 1, GETDATE()),
(9, 'División de Educación para el Trabajo', 'DET-UNA', 'MSc. Magaly Zúñiga Céspedes', 'director.trabajo@una.ac.cr', '2277-3102', 1, GETDATE()),
(9, 'División de Educología', 'DEDUC-UNA', 'Dr. Gilberto Alfaro Varela', 'director.educologia@una.ac.cr', '2277-3103', 1, GETDATE());

-- Escuelas para UNA - Ciencias Sociales (Id = 10)
INSERT INTO Escuelas (FacultadId, Nombre, Codigo, Director, Email, Telefono, Estado, FechaCreacion)
VALUES 
(10, 'Escuela de Planificación y Promoción Social', 'EPPS-UNA', 'Dr. Carlos Sandoval García', 'director.planificacion@una.ac.cr', '2277-3201', 1, GETDATE()),
(10, 'Escuela de Relaciones Internacionales', 'ERI-UNA', 'Dra. Arodys Robles Soto', 'director.internacionales@una.ac.cr', '2277-3202', 1, GETDATE());

-- Escuelas para Universidad Latina - Ingeniería (Id = 11)
INSERT INTO Escuelas (FacultadId, Nombre, Codigo, Director, Email, Telefono, Estado, FechaCreacion)
VALUES 
(11, 'Escuela de Ingeniería en Sistemas', 'EIS-ULATINA', 'Ing. Ricardo Vargas Mora', 'director.sistemas@ulatina.ac.cr', '2203-7001', 1, GETDATE()),
(11, 'Escuela de Ingeniería Industrial', 'EII-ULATINA', 'Ing. Ana Patricia Jiménez', 'director.industrial@ulatina.ac.cr', '2203-7002', 1, GETDATE()),
(11, 'Escuela de Arquitectura', 'EA-ULATINA', 'Arq. José Miguel Zeledón', 'director.arquitectura@ulatina.ac.cr', '2203-7003', 1, GETDATE());

-- Escuelas para Universidad Latina - Ciencias Empresariales (Id = 12)
INSERT INTO Escuelas (FacultadId, Nombre, Codigo, Director, Email, Telefono, Estado, FechaCreacion)
VALUES 
(12, 'Escuela de Administración de Empresas', 'EAE-ULATINA', 'MBA. Jorge Vásquez Alfaro', 'director.administracion@ulatina.ac.cr', '2203-7101', 1, GETDATE()),
(12, 'Escuela de Mercadeo', 'EM-ULATINA', 'MSc. Laura Chacón Elizondo', 'director.mercadeo@ulatina.ac.cr', '2203-7102', 1, GETDATE()),
(12, 'Escuela de Contaduría Pública', 'ECP-ULATINA', 'CPA. Manuel Obando Quesada', 'director.contaduria@ulatina.ac.cr', '2203-7103', 1, GETDATE());

-- Escuelas para Universidad Latina - Educación (Id = 13)
INSERT INTO Escuelas (FacultadId, Nombre, Codigo, Director, Email, Telefono, Estado, FechaCreacion)
VALUES 
(13, 'Escuela de Educación Preescolar', 'EEP-ULATINA', 'MSc. Silvia Mora Esquivel', 'director.preescolar@ulatina.ac.cr', '2203-7201', 1, GETDATE()),
(13, 'Escuela de Educación Primaria', 'EEPR-ULATINA', 'Dr. Alberto González Herrera', 'director.primaria@ulatina.ac.cr', '2203-7202', 1, GETDATE()),
(13, 'Escuela de Educación Especial', 'EEE-ULATINA', 'MSc. Carmen Iris Ruiz', 'director.especial@ulatina.ac.cr', '2203-7203', 1, GETDATE());

PRINT 'Escuelas insertadas correctamente.'

-- ==================================================
-- VERIFICAR INSERCIÓN
-- ==================================================

PRINT 'Verificando la inserción de datos...'

SELECT 'Facultades insertadas: ' + CAST(COUNT(*) AS VARCHAR(10))
FROM Facultades WHERE FechaCreacion >= CAST(GETDATE() AS DATE);

SELECT 'Escuelas insertadas: ' + CAST(COUNT(*) AS VARCHAR(10))
FROM Escuelas WHERE FechaCreacion >= CAST(GETDATE() AS DATE);

-- Mostrar resumen por institución
SELECT 
    i.Nombre AS Institucion,
    COUNT(DISTINCT f.Id) AS TotalFacultades,
    COUNT(e.Id) AS TotalEscuelas
FROM Instituciones i
LEFT JOIN Facultades f ON i.Id = f.InstitucionId AND f.FechaCreacion >= CAST(GETDATE() AS DATE)
LEFT JOIN Escuelas e ON f.Id = e.FacultadId AND e.FechaCreacion >= CAST(GETDATE() AS DATE)
WHERE i.Id IN (1, 2, 3, 6)
GROUP BY i.Id, i.Nombre
ORDER BY i.Nombre;

PRINT 'Script completado exitosamente!'