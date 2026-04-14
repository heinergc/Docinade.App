-- =============================================
-- Script: Seed Profesores Completo
-- Descripcion: Elimina todos los profesores existentes e inserta 5 registros de prueba
--              con todos sus datos relacionados (Formacion Academica, Experiencia Laboral,
--              Capacitaciones, Grupos, etc.)
-- Fecha: 2025-10-27
-- =============================================

BEGIN TRANSACTION;

BEGIN TRY
    PRINT '========================================';
    PRINT 'Iniciando limpieza de datos de profesores...';
    PRINT '========================================';

    -- Eliminar datos relacionados primero (por foreign keys)
    DELETE FROM [dbo].[ProfesorGuia];
    PRINT 'ProfesorGuia eliminados: ' + CAST(@@ROWCOUNT AS VARCHAR(10));

    DELETE FROM [dbo].[ProfesorGrupo];
    PRINT 'ProfesorGrupo eliminados: ' + CAST(@@ROWCOUNT AS VARCHAR(10));

    DELETE FROM [dbo].[ProfesorCapacitacion];
    PRINT 'ProfesorCapacitacion eliminados: ' + CAST(@@ROWCOUNT AS VARCHAR(10));

    DELETE FROM [dbo].[ProfesorExperienciaLaboral];
    PRINT 'ProfesorExperienciaLaboral eliminados: ' + CAST(@@ROWCOUNT AS VARCHAR(10));

    DELETE FROM [dbo].[ProfesorFormacionAcademica];
    PRINT 'ProfesorFormacionAcademica eliminados: ' + CAST(@@ROWCOUNT AS VARCHAR(10));

    DELETE FROM [dbo].[Profesores];
    PRINT 'Profesores eliminados: ' + CAST(@@ROWCOUNT AS VARCHAR(10));

    -- Reiniciar autonumericos
    DBCC CHECKIDENT ('[dbo].[Profesores]', RESEED, 0);
    DBCC CHECKIDENT ('[dbo].[ProfesorFormacionAcademica]', RESEED, 0);
    DBCC CHECKIDENT ('[dbo].[ProfesorExperienciaLaboral]', RESEED, 0);
    DBCC CHECKIDENT ('[dbo].[ProfesorCapacitacion]', RESEED, 0);
    DBCC CHECKIDENT ('[dbo].[ProfesorGrupo]', RESEED, 0);
    DBCC CHECKIDENT ('[dbo].[ProfesorGuia]', RESEED, 0);
    PRINT 'Autonumericos reiniciados correctamente';

    PRINT '';
    PRINT '========================================';
    PRINT 'Insertando 5 profesores de prueba...';
    PRINT '========================================';

    -- =============================================
    -- PROFESOR 1: Ana Maria Gonzalez Rodriguez
    -- =============================================
    INSERT INTO [dbo].[Profesores] (
        [Nombres], [PrimerApellido], [SegundoApellido], [Cedula], [TipoCedula],
        [Sexo], [FechaNacimiento], [EstadoCivil], [Nacionalidad],
        [EmailPersonal], [EmailInstitucional], [TelefonoFijo], [TelefonoCelular], [TelefonoOficina], [Extension],
        [DireccionExacta], [ProvinciaId], [CantonId], [DistritoId], [CodigoPostal],
        [GradoAcademico], [TituloAcademico], [InstitucionGraduacion], [PaisGraduacion], [AnioGraduacion],
        [NumeroColegiadoProfesional], [EscuelaId], [CodigoEmpleado], [FechaIngreso], [TipoContrato],
        [RegimenLaboral], [CategoriaLaboral], [TipoJornada], [HorasLaborales],
        [AreasEspecializacion], [IdiomasHabla], [NivelIngles], [ExperienciaDocente],
        [ContactoEmergenciaNombre], [ContactoEmergenciaParentesco], [ContactoEmergenciaTelefono],
        [NotificacionesEmail], [NotificacionesSMS], [Estado], [FechaCreacion], [CreadoPor], [Version]
    ) VALUES (
        'Ana Maria', 'Gonzalez', 'Rodriguez', '101230456', 'Nacional',
        'F', '1985-03-15', 'Casada', 'Costarricense',
        'ana.gonzalez@gmail.com', 'ana.gonzalez@utn.ac.cr', '2233-4455', '8888-1111', '2222-3333', '101',
        'San Jose, Curridabat, 200 metros norte de la iglesia', 1, 1, 1, '10101',
        'Doctorado', 'Doctora en Educacion', 'Universidad de Costa Rica', 'Costa Rica', 2018,
        'COL-12345', 1, 'PROF-001', '2019-01-15', 'Propiedad',
        'Dedicacion Exclusiva', 'Catedratico', 'Tiempo Completo', 40,
        'Pedagogia, Didactica, Evaluacion Educativa, Tecnologia Educativa', 'Espanol, Ingles, Frances', 'Avanzado', 15,
        'Carlos Gonzalez Mora', 'Esposo', '8877-6655',
        1, 1, 1, GETDATE(), 'admin@rubricas.edu', 1
    );
    DECLARE @ProfesorId1 INT = SCOPE_IDENTITY();
    PRINT 'Profesor 1 insertado: Ana Maria Gonzalez Rodriguez (ID: ' + CAST(@ProfesorId1 AS VARCHAR(10)) + ')';

    -- Formacion Academica - Profesor 1
    INSERT INTO [dbo].[ProfesorFormacionAcademica] (
        [ProfesorId], [TipoFormacion], [TituloObtenido], [InstitucionEducativa], [PaisInstitucion],
        [AnioInicio], [AnioFinalizacion], [EnCurso], [PromedioGeneral], [EsTituloReconocidoCONARE],
        [NumeroReconocimiento], [FechaCreacion]
    ) VALUES 
    (@ProfesorId1, 'Doctorado', 'Doctorado en Educacion', 'Universidad de Costa Rica', 'Costa Rica', 2015, 2018, 0, 9.5, 1, 'CONARE-2018-001', GETDATE()),
    (@ProfesorId1, 'Maestria', 'Maestria en Docencia Universitaria', 'Universidad Nacional', 'Costa Rica', 2010, 2012, 0, 9.2, 1, 'CONARE-2012-123', GETDATE()),
    (@ProfesorId1, 'Licenciatura', 'Licenciatura en Educacion Primaria', 'Universidad de Costa Rica', 'Costa Rica', 2003, 2007, 0, 8.8, 1, 'CONARE-2007-456', GETDATE());

    -- Experiencia Laboral - Profesor 1
    INSERT INTO [dbo].[ProfesorExperienciaLaboral] (
        [ProfesorId], [NombreInstitucion], [CargoDesempenado], [TipoInstitucion],
        [FechaInicio], [FechaFin], [TrabajandoActualmente], [DescripcionFunciones],
        [TipoContrato], [JornadaLaboral], [FechaCreacion]
    ) VALUES
    (@ProfesorId1, 'Universidad Tecnica Nacional', 'Profesora Titular', 'Educacion Superior', '2019-01-15', NULL, 1, 'Docencia en cursos de pedagogia y didactica. Investigacion en tecnologias educativas. Coordinacion de proyectos de extension.', 'Propiedad', 'Tiempo Completo', GETDATE()),
    (@ProfesorId1, 'Ministerio de Educacion Publica', 'Asesora Nacional', 'Educacion Publica', '2015-03-01', '2018-12-31', 0, 'Asesoria a docentes en metodologias activas. Diseno de material didactico. Capacitacion docente.', 'Servicios Profesionales', 'Medio Tiempo', GETDATE()),
    (@ProfesorId1, 'Colegio Tecnico Profesional', 'Docente', 'Educacion Secundaria', '2008-02-01', '2015-02-28', 0, 'Impartir lecciones de matematicas y ciencias. Tutoria de estudiantes. Coordinacion de proyectos.', 'Interino', 'Tiempo Completo', GETDATE());

    -- Capacitaciones - Profesor 1
    INSERT INTO [dbo].[ProfesorCapacitacion] (
        [ProfesorId], [NombreCapacitacion], [InstitucionOrganizadora], [TipoCapacitacion], [Modalidad],
        [FechaInicio], [FechaFin], [HorasCapacitacion], [CertificadoObtenido], [CalificacionObtenida],
        [AreaConocimiento], [FechaCreacion]
    ) VALUES
    (@ProfesorId1, 'Inteligencia Artificial en la Educacion', 'Universidad de Stanford', 'Curso', 'Virtual', '2024-01-15', '2024-03-15', 60, 1, 95.0, 'Tecnologia Educativa', GETDATE()),
    (@ProfesorId1, 'Evaluacion por Competencias', 'CONARE', 'Taller', 'Presencial', '2023-06-10', '2023-06-12', 16, 1, 90.0, 'Evaluacion', GETDATE()),
    (@ProfesorId1, 'Diseno Universal para el Aprendizaje', 'UNESCO', 'Seminario', 'Hibrido', '2023-09-01', '2023-11-30', 40, 1, 92.0, 'Pedagogia', GETDATE());

    -- =============================================
    -- PROFESOR 2: Carlos Alberto Ramirez Solis
    -- =============================================
    INSERT INTO [dbo].[Profesores] (
        [Nombres], [PrimerApellido], [SegundoApellido], [Cedula], [TipoCedula],
        [Sexo], [FechaNacimiento], [EstadoCivil], [Nacionalidad],
        [EmailPersonal], [EmailInstitucional], [TelefonoFijo], [TelefonoCelular], [TelefonoOficina], [Extension],
        [DireccionExacta], [ProvinciaId], [CantonId], [DistritoId], [CodigoPostal],
        [GradoAcademico], [TituloAcademico], [InstitucionGraduacion], [PaisGraduacion], [AnioGraduacion],
        [NumeroColegiadoProfesional], [EscuelaId], [CodigoEmpleado], [FechaIngreso], [TipoContrato],
        [RegimenLaboral], [CategoriaLaboral], [TipoJornada], [HorasLaborales],
        [AreasEspecializacion], [IdiomasHabla], [NivelIngles], [ExperienciaDocente],
        [ContactoEmergenciaNombre], [ContactoEmergenciaParentesco], [ContactoEmergenciaTelefono],
        [NotificacionesEmail], [NotificacionesSMS], [Estado], [FechaCreacion], [CreadoPor], [Version]
    ) VALUES (
        'Carlos Alberto', 'Ramirez', 'Solis', '201340567', 'Nacional',
        'M', '1980-07-22', 'Soltero', 'Costarricense',
        'carlos.ramirez@hotmail.com', 'carlos.ramirez@utn.ac.cr', '2244-5566', '8899-2222', '2222-4444', '102',
        'Alajuela, Centro, frente al parque central', 2, 36, 66, '20101',
        'Maestria', 'Master en Ingenieria de Software', 'Instituto Tecnologico de Costa Rica', 'Costa Rica', 2015,
        'COL-23456', 1, 'PROF-002', '2016-02-01', 'Interinato',
        'Tiempo Parcial', 'Profesor', 'Medio Tiempo', 20,
        'Programacion, Bases de Datos, Desarrollo Web, Arquitectura de Software', 'Espanol, Ingles', 'Intermedio', 10,
        'Maria Ramirez Vega', 'Madre', '8866-5544',
        1, 0, 1, GETDATE(), 'admin@rubricas.edu', 1
    );
    DECLARE @ProfesorId2 INT = SCOPE_IDENTITY();
    PRINT 'Profesor 2 insertado: Carlos Alberto Ramirez Solis (ID: ' + CAST(@ProfesorId2 AS VARCHAR(10)) + ')';

    -- Formacion Academica - Profesor 2
    INSERT INTO [dbo].[ProfesorFormacionAcademica] (
        [ProfesorId], [TipoFormacion], [TituloObtenido], [InstitucionEducativa], [PaisInstitucion],
        [AnioInicio], [AnioFinalizacion], [EnCurso], [PromedioGeneral], [EsTituloReconocidoCONARE],
        [NumeroReconocimiento], [FechaCreacion]
    ) VALUES 
    (@ProfesorId2, 'Maestria', 'Master en Ingenieria de Software', 'Instituto Tecnologico de Costa Rica', 'Costa Rica', 2013, 2015, 0, 9.0, 1, 'CONARE-2015-789', GETDATE()),
    (@ProfesorId2, 'Bachillerato', 'Bachillerato en Ingenieria en Computacion', 'Universidad de Costa Rica', 'Costa Rica', 2003, 2007, 0, 8.5, 1, 'CONARE-2007-234', GETDATE());

    -- Experiencia Laboral - Profesor 2
    INSERT INTO [dbo].[ProfesorExperienciaLaboral] (
        [ProfesorId], [NombreInstitucion], [CargoDesempenado], [TipoInstitucion],
        [FechaInicio], [FechaFin], [TrabajandoActualmente], [DescripcionFunciones],
        [TipoContrato], [JornadaLaboral], [FechaCreacion]
    ) VALUES
    (@ProfesorId2, 'Universidad Tecnica Nacional', 'Profesor Instructor', 'Educacion Superior', '2016-02-01', NULL, 1, 'Impartir cursos de programacion y desarrollo web. Asesoria de proyectos finales de graduacion.', 'Interino', 'Medio Tiempo', GETDATE()),
    (@ProfesorId2, 'Tech Solutions S.A.', 'Desarrollador Senior', 'Empresa Privada', '2010-06-01', '2016-01-31', 0, 'Desarrollo de aplicaciones web. Liderazgo tecnico de equipo. Analisis y diseno de sistemas.', 'Planilla', 'Tiempo Completo', GETDATE());

    -- Capacitaciones - Profesor 2
    INSERT INTO [dbo].[ProfesorCapacitacion] (
        [ProfesorId], [NombreCapacitacion], [InstitucionOrganizadora], [TipoCapacitacion], [Modalidad],
        [FechaInicio], [FechaFin], [HorasCapacitacion], [CertificadoObtenido], [CalificacionObtenida],
        [AreaConocimiento], [FechaCreacion]
    ) VALUES
    (@ProfesorId2, 'Cloud Computing con AWS', 'Amazon Web Services', 'Certificacion', 'Virtual', '2024-03-01', '2024-05-15', 80, 1, 88.0, 'Tecnologia', GETDATE()),
    (@ProfesorId2, 'Scrum Master Professional', 'Scrum.org', 'Certificacion', 'Virtual', '2023-08-01', '2023-09-30', 40, 1, 92.0, 'Gestion de Proyectos', GETDATE()),
    (@ProfesorId2, 'Docker y Kubernetes', 'Linux Foundation', 'Curso', 'Virtual', '2023-01-15', '2023-03-15', 50, 1, 85.0, 'DevOps', GETDATE());

    -- =============================================
    -- PROFESOR 3: Maria Fernanda Mora Castro
    -- =============================================
    INSERT INTO [dbo].[Profesores] (
        [Nombres], [PrimerApellido], [SegundoApellido], [Cedula], [TipoCedula],
        [Sexo], [FechaNacimiento], [EstadoCivil], [Nacionalidad],
        [EmailPersonal], [EmailInstitucional], [TelefonoFijo], [TelefonoCelular], [TelefonoOficina], [Extension],
        [DireccionExacta], [ProvinciaId], [CantonId], [DistritoId], [CodigoPostal],
        [GradoAcademico], [TituloAcademico], [InstitucionGraduacion], [PaisGraduacion], [AnioGraduacion],
        [NumeroColegiadoProfesional], [EscuelaId], [CodigoEmpleado], [FechaIngreso], [TipoContrato],
        [RegimenLaboral], [CategoriaLaboral], [TipoJornada], [HorasLaborales],
        [AreasEspecializacion], [IdiomasHabla], [NivelIngles], [ExperienciaDocente],
        [ContactoEmergenciaNombre], [ContactoEmergenciaParentesco], [ContactoEmergenciaTelefono],
        [NotificacionesEmail], [NotificacionesSMS], [Estado], [FechaCreacion], [CreadoPor], [Version]
    ) VALUES (
        'Maria Fernanda', 'Mora', 'Castro', '303620067', 'Nacional',
        'F', '1988-11-05', 'Casada', 'Costarricense',
        'maria.mora@yahoo.com', 'maria.mora@utn.ac.cr', '2255-6677', '8855-3333', '2222-5555', '103',
        'Cartago, Cartago, diagonal a la Basilica', 3, 41, 77, '30101',
        'Licenciatura', 'Licenciada en Administracion de Empresas', 'Universidad Nacional', 'Costa Rica', 2012,
        'COL-34567', 1, 'PROF-003', '2013-03-15', 'Propiedad',
        'Dedicacion Parcial', 'Profesor Asistente', 'Tiempo Completo', 40,
        'Administracion, Recursos Humanos, Gestion de Proyectos, Emprendimiento', 'Espanol, Ingles, Portugues', 'Avanzado', 12,
        'Roberto Mora Jimenez', 'Esposo', '8844-3322',
        1, 1, 1, GETDATE(), 'admin@rubricas.edu', 1
    );
    DECLARE @ProfesorId3 INT = SCOPE_IDENTITY();
    PRINT 'Profesor 3 insertado: Maria Fernanda Mora Castro (ID: ' + CAST(@ProfesorId3 AS VARCHAR(10)) + ')';

    -- Formacion Academica - Profesor 3
    INSERT INTO [dbo].[ProfesorFormacionAcademica] (
        [ProfesorId], [TipoFormacion], [TituloObtenido], [InstitucionEducativa], [PaisInstitucion],
        [AnioInicio], [AnioFinalizacion], [EnCurso], [PromedioGeneral], [EsTituloReconocidoCONARE],
        [NumeroReconocimiento], [FechaCreacion]
    ) VALUES 
    (@ProfesorId3, 'Maestria', 'Maestria en Administracion de Negocios (MBA)', 'INCAE Business School', 'Costa Rica', 2020, 2022, 0, 9.3, 1, 'CONARE-2022-567', GETDATE()),
    (@ProfesorId3, 'Licenciatura', 'Licenciatura en Administracion de Empresas', 'Universidad Nacional', 'Costa Rica', 2007, 2012, 0, 8.7, 1, 'CONARE-2012-890', GETDATE()),
    (@ProfesorId3, 'Diplomado', 'Diplomado en Contabilidad', 'Universidad Nacional', 'Costa Rica', 2005, 2007, 0, 8.4, 1, 'CONARE-2007-345', GETDATE());

    -- Experiencia Laboral - Profesor 3
    INSERT INTO [dbo].[ProfesorExperienciaLaboral] (
        [ProfesorId], [NombreInstitucion], [CargoDesempenado], [TipoInstitucion],
        [FechaInicio], [FechaFin], [TrabajandoActualmente], [DescripcionFunciones],
        [TipoContrato], [JornadaLaboral], [FechaCreacion]
    ) VALUES
    (@ProfesorId3, 'Universidad Tecnica Nacional', 'Profesora Asociada', 'Educacion Superior', '2013-03-15', NULL, 1, 'Docencia en administracion y gestion empresarial. Coordinacion de proyectos de emprendimiento. Investigacion en gestion de recursos humanos.', 'Propiedad', 'Tiempo Completo', GETDATE()),
    (@ProfesorId3, 'Banco Nacional de Costa Rica', 'Analista de Recursos Humanos', 'Empresa Publica', '2010-01-10', '2013-02-28', 0, 'Reclutamiento y seleccion de personal. Capacitacion y desarrollo. Evaluacion del desempeno.', 'Planilla', 'Tiempo Completo', GETDATE());

    -- Capacitaciones - Profesor 3
    INSERT INTO [dbo].[ProfesorCapacitacion] (
        [ProfesorId], [NombreCapacitacion], [InstitucionOrganizadora], [TipoCapacitacion], [Modalidad],
        [FechaInicio], [FechaFin], [HorasCapacitacion], [CertificadoObtenido], [CalificacionObtenida],
        [AreaConocimiento], [FechaCreacion]
    ) VALUES
    (@ProfesorId3, 'Liderazgo Transformacional', 'Harvard Business School', 'Curso', 'Virtual', '2024-02-01', '2024-04-30', 60, 1, 94.0, 'Liderazgo', GETDATE()),
    (@ProfesorId3, 'Design Thinking para Innovacion', 'Stanford University', 'Taller', 'Virtual', '2023-07-15', '2023-09-15', 40, 1, 91.0, 'Innovacion', GETDATE()),
    (@ProfesorId3, 'Coaching Empresarial', 'ICF Costa Rica', 'Certificacion', 'Presencial', '2023-03-01', '2023-06-30', 80, 1, 89.0, 'Desarrollo Organizacional', GETDATE()),
    (@ProfesorId3, 'Excel Avanzado para Negocios', 'Microsoft', 'Curso', 'Virtual', '2022-11-01', '2022-12-15', 30, 1, 87.0, 'Herramientas Ofimaticas', GETDATE());

    -- =============================================
    -- PROFESOR 4: Jose Daniel Vargas Hernandez
    -- =============================================
    INSERT INTO [dbo].[Profesores] (
        [Nombres], [PrimerApellido], [SegundoApellido], [Cedula], [TipoCedula],
        [Sexo], [FechaNacimiento], [EstadoCivil], [Nacionalidad],
        [EmailPersonal], [EmailInstitucional], [TelefonoFijo], [TelefonoCelular], [TelefonoOficina], [Extension],
        [DireccionExacta], [ProvinciaId], [CantonId], [DistritoId], [CodigoPostal],
        [GradoAcademico], [TituloAcademico], [InstitucionGraduacion], [PaisGraduacion], [AnioGraduacion],
        [NumeroColegiadoProfesional], [EscuelaId], [CodigoEmpleado], [FechaIngreso], [TipoContrato],
        [RegimenLaboral], [CategoriaLaboral], [TipoJornada], [HorasLaborales],
        [AreasEspecializacion], [IdiomasHabla], [NivelIngles], [ExperienciaDocente],
        [ContactoEmergenciaNombre], [ContactoEmergenciaParentesco], [ContactoEmergenciaTelefono],
        [NotificacionesEmail], [NotificacionesSMS], [Estado], [FechaCreacion], [CreadoPor], [Version]
    ) VALUES (
        'Jose Daniel', 'Vargas', 'Hernandez', '401450678', 'Nacional',
        'M', '1975-04-18', 'Casado', 'Costarricense',
        'jose.vargas@gmail.com', 'jose.vargas@utn.ac.cr', '2266-7788', '8877-4444', '2222-6666', '104',
        'Heredia, Santo Domingo, 300 metros sur del supermercado', 4, 48, 88, '40101',
        'Doctorado', 'Doctor en Ciencias de la Computacion', 'Universidad de Stanford', 'Estados Unidos', 2010,
        'COL-45678', 1, 'PROF-004', '2011-01-10', 'Propiedad',
        'Dedicacion Exclusiva', 'Catedratico', 'Tiempo Completo', 40,
        'Inteligencia Artificial, Machine Learning, Redes Neuronales, Big Data', 'Espanol, Ingles', 'Nativo', 20,
        'Ana Vargas Solano', 'Esposa', '8833-2211',
        1, 0, 1, GETDATE(), 'admin@rubricas.edu', 1
    );
    DECLARE @ProfesorId4 INT = SCOPE_IDENTITY();
    PRINT 'Profesor 4 insertado: Jose Daniel Vargas Hernandez (ID: ' + CAST(@ProfesorId4 AS VARCHAR(10)) + ')';

    -- Formacion Academica - Profesor 4
    INSERT INTO [dbo].[ProfesorFormacionAcademica] (
        [ProfesorId], [TipoFormacion], [TituloObtenido], [InstitucionEducativa], [PaisInstitucion],
        [AnioInicio], [AnioFinalizacion], [EnCurso], [PromedioGeneral], [EsTituloReconocidoCONARE],
        [NumeroReconocimiento], [FechaCreacion]
    ) VALUES 
    (@ProfesorId4, 'Doctorado', 'Doctor en Ciencias de la Computacion', 'Universidad de Stanford', 'Estados Unidos', 2006, 2010, 0, 9.8, 1, 'CONARE-2010-111', GETDATE()),
    (@ProfesorId4, 'Maestria', 'Master en Inteligencia Artificial', 'MIT', 'Estados Unidos', 2002, 2004, 0, 9.5, 1, 'CONARE-2004-222', GETDATE()),
    (@ProfesorId4, 'Licenciatura', 'Licenciatura en Ingenieria en Sistemas', 'Instituto Tecnologico de Costa Rica', 'Costa Rica', 1996, 2001, 0, 9.0, 1, 'CONARE-2001-333', GETDATE());

    -- Experiencia Laboral - Profesor 4
    INSERT INTO [dbo].[ProfesorExperienciaLaboral] (
        [ProfesorId], [NombreInstitucion], [CargoDesempenado], [TipoInstitucion],
        [FechaInicio], [FechaFin], [TrabajandoActualmente], [DescripcionFunciones],
        [TipoContrato], [JornadaLaboral], [FechaCreacion]
    ) VALUES
    (@ProfesorId4, 'Universidad Tecnica Nacional', 'Profesor Catedratico', 'Educacion Superior', '2011-01-10', NULL, 1, 'Docencia en ciencias de la computacion e inteligencia artificial. Direccion de proyectos de investigacion. Tutoria de tesis doctorales.', 'Propiedad', 'Tiempo Completo', GETDATE()),
    (@ProfesorId4, 'Google Inc.', 'Research Scientist', 'Empresa Privada', '2010-06-01', '2010-12-31', 0, 'Investigacion en algoritmos de machine learning. Desarrollo de modelos predictivos. Publicacion de papers cientificos.', 'Consultor', 'Tiempo Completo', GETDATE()),
    (@ProfesorId4, 'Stanford University', 'Investigador Doctoral', 'Educacion Superior', '2006-01-01', '2010-05-31', 0, 'Investigacion en redes neuronales y deep learning. Asistente de investigacion. Docencia de laboratorios.', 'Beca Doctoral', 'Tiempo Completo', GETDATE());

    -- Capacitaciones - Profesor 4
    INSERT INTO [dbo].[ProfesorCapacitacion] (
        [ProfesorId], [NombreCapacitacion], [InstitucionOrganizadora], [TipoCapacitacion], [Modalidad],
        [FechaInicio], [FechaFin], [HorasCapacitacion], [CertificadoObtenido], [CalificacionObtenida],
        [AreaConocimiento], [FechaCreacion]
    ) VALUES
    (@ProfesorId4, 'Advanced Deep Learning', 'DeepLearning.AI', 'Especializacion', 'Virtual', '2024-01-10', '2024-06-30', 120, 1, 98.0, 'Inteligencia Artificial', GETDATE()),
    (@ProfesorId4, 'Quantum Computing', 'IBM', 'Curso', 'Virtual', '2023-09-01', '2023-11-30', 60, 1, 95.0, 'Computacion Cuantica', GETDATE()),
    (@ProfesorId4, 'Natural Language Processing', 'Stanford University', 'Curso', 'Virtual', '2023-03-01', '2023-05-31', 80, 1, 96.0, 'NLP', GETDATE());

    -- =============================================
    -- PROFESOR 5: Laura Patricia Jimenez Salas
    -- =============================================
    INSERT INTO [dbo].[Profesores] (
        [Nombres], [PrimerApellido], [SegundoApellido], [Cedula], [TipoCedula],
        [Sexo], [FechaNacimiento], [EstadoCivil], [Nacionalidad],
        [EmailPersonal], [EmailInstitucional], [TelefonoFijo], [TelefonoCelular], [TelefonoOficina], [Extension],
        [DireccionExacta], [ProvinciaId], [CantonId], [DistritoId], [CodigoPostal],
        [GradoAcademico], [TituloAcademico], [InstitucionGraduacion], [PaisGraduacion], [AnioGraduacion],
        [NumeroColegiadoProfesional], [EscuelaId], [CodigoEmpleado], [FechaIngreso], [TipoContrato],
        [RegimenLaboral], [CategoriaLaboral], [TipoJornada], [HorasLaborales],
        [AreasEspecializacion], [IdiomasHabla], [NivelIngles], [ExperienciaDocente],
        [ContactoEmergenciaNombre], [ContactoEmergenciaParentesco], [ContactoEmergenciaTelefono],
        [NotificacionesEmail], [NotificacionesSMS], [Estado], [FechaCreacion], [CreadoPor], [Version]
    ) VALUES (
        'Laura Patricia', 'Jimenez', 'Salas', '501560789', 'Nacional',
        'F', '1992-09-28', 'Soltera', 'Costarricense',
        'laura.jimenez@outlook.com', 'laura.jimenez@utn.ac.cr', '2277-8899', '8866-5555', '2222-7777', '105',
        'Puntarenas, Puntarenas, detras del mercado municipal', 5, 57, 102, '50101',
        'Maestria', 'Master en Gestion de Tecnologias de Informacion', 'Universidad Latina', 'Costa Rica', 2020,
        'COL-56789', 1, 'PROF-005', '2021-02-15', 'Interinato',
        'Tiempo Parcial', 'Instructor', 'Tres Cuartos Tiempo', 30,
        'Redes de Computadoras, Seguridad Informatica, Gestion de TI, Telecomunicaciones', 'Espanol, Ingles, Aleman', 'Intermedio', 5,
        'Carmen Jimenez Rojas', 'Madre', '8855-4433',
        1, 1, 1, GETDATE(), 'admin@rubricas.edu', 1
    );
    DECLARE @ProfesorId5 INT = SCOPE_IDENTITY();
    PRINT 'Profesor 5 insertado: Laura Patricia Jimenez Salas (ID: ' + CAST(@ProfesorId5 AS VARCHAR(10)) + ')';

    -- Formacion Academica - Profesor 5
    INSERT INTO [dbo].[ProfesorFormacionAcademica] (
        [ProfesorId], [TipoFormacion], [TituloObtenido], [InstitucionEducativa], [PaisInstitucion],
        [AnioInicio], [AnioFinalizacion], [EnCurso], [PromedioGeneral], [EsTituloReconocidoCONARE],
        [NumeroReconocimiento], [FechaCreacion]
    ) VALUES 
    (@ProfesorId5, 'Maestria', 'Master en Gestion de Tecnologias de Informacion', 'Universidad Latina', 'Costa Rica', 2018, 2020, 0, 8.9, 1, 'CONARE-2020-444', GETDATE()),
    (@ProfesorId5, 'Bachillerato', 'Bachillerato en Ingenieria en Sistemas de Informacion', 'Universidad de Costa Rica', 'Costa Rica', 2010, 2015, 0, 8.6, 1, 'CONARE-2015-555', GETDATE());

    -- Experiencia Laboral - Profesor 5
    INSERT INTO [dbo].[ProfesorExperienciaLaboral] (
        [ProfesorId], [NombreInstitucion], [CargoDesempenado], [TipoInstitucion],
        [FechaInicio], [FechaFin], [TrabajandoActualmente], [DescripcionFunciones],
        [TipoContrato], [JornadaLaboral], [FechaCreacion]
    ) VALUES
    (@ProfesorId5, 'Universidad Tecnica Nacional', 'Profesora Instructora', 'Educacion Superior', '2021-02-15', NULL, 1, 'Impartir cursos de redes y seguridad informatica. Asesoria tecnica a estudiantes. Coordinacion de laboratorios.', 'Interino', 'Tres Cuartos Tiempo', GETDATE()),
    (@ProfesorId5, 'Cisco Systems', 'Ingeniera de Redes', 'Empresa Privada', '2016-03-01', '2021-01-31', 0, 'Diseno e implementacion de redes corporativas. Soporte tecnico nivel 3. Capacitacion a clientes.', 'Planilla', 'Tiempo Completo', GETDATE());

    -- Capacitaciones - Profesor 5
    INSERT INTO [dbo].[ProfesorCapacitacion] (
        [ProfesorId], [NombreCapacitacion], [InstitucionOrganizadora], [TipoCapacitacion], [Modalidad],
        [FechaInicio], [FechaFin], [HorasCapacitacion], [CertificadoObtenido], [CalificacionObtenida],
        [AreaConocimiento], [FechaCreacion]
    ) VALUES
    (@ProfesorId5, 'CCNA: Cisco Certified Network Associate', 'Cisco', 'Certificacion', 'Presencial', '2024-01-20', '2024-04-20', 100, 1, 90.0, 'Redes', GETDATE()),
    (@ProfesorId5, 'Ethical Hacking y Ciberseguridad', 'EC-Council', 'Certificacion', 'Virtual', '2023-08-01', '2023-10-30', 80, 1, 88.0, 'Seguridad', GETDATE()),
    (@ProfesorId5, 'CompTIA Security+', 'CompTIA', 'Certificacion', 'Virtual', '2023-02-01', '2023-04-30', 60, 1, 85.0, 'Seguridad Informatica', GETDATE()),
    (@ProfesorId5, 'Administracion de Servidores Linux', 'Linux Professional Institute', 'Curso', 'Virtual', '2022-09-01', '2022-11-30', 50, 1, 87.0, 'Sistemas Operativos', GETDATE());

    PRINT '';
    PRINT '========================================';
    PRINT 'Resumen de insercion:';
    -- Obtener contadores
    DECLARE @CountFormaciones INT = (SELECT COUNT(*) FROM [dbo].[ProfesorFormacionAcademica]);
    DECLARE @CountExperiencias INT = (SELECT COUNT(*) FROM [dbo].[ProfesorExperienciaLaboral]);
    DECLARE @CountCapacitaciones INT = (SELECT COUNT(*) FROM [dbo].[ProfesorCapacitacion]);
    
    PRINT '========================================';
    PRINT 'Total Profesores insertados: 5';
    PRINT 'Total Formaciones Academicas: ' + CAST(@CountFormaciones AS VARCHAR(10));
    PRINT 'Total Experiencias Laborales: ' + CAST(@CountExperiencias AS VARCHAR(10));
    PRINT 'Total Capacitaciones: ' + CAST(@CountCapacitaciones AS VARCHAR(10));
    PRINT '';
    PRINT 'Script ejecutado exitosamente!';
    PRINT '========================================';

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    ROLLBACK TRANSACTION;
    
    PRINT '';
    PRINT '========================================';
    PRINT 'ERROR EN LA EJECUCION DEL SCRIPT';
    PRINT '========================================';
    PRINT 'Error Number: ' + CAST(ERROR_NUMBER() AS VARCHAR(10));
    PRINT 'Error Message: ' + ERROR_MESSAGE();
    PRINT 'Error Line: ' + CAST(ERROR_LINE() AS VARCHAR(10));
    
    -- Re-lanzar el error
    THROW;
END CATCH;
