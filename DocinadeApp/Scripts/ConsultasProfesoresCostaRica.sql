-- ============================================================
-- CONSULTAS Y VISTAS ÚTILES PARA PROFESORES - COSTA RICA
-- Sistema de Rúbricas Académicas
-- Fecha: 23 de octubre, 2025
-- ============================================================

-- ============================================================
-- VISTAS PRINCIPALES
-- ============================================================

-- Vista completa de Profesores con información geográfica e institucional
GO
CREATE VIEW vw_ProfesoresCompleto AS
SELECT 
    p.Id,
    p.Cedula,
    p.TipoCedula,
    CONCAT(p.Nombres, ' ', p.PrimerApellido, ' ', ISNULL(p.SegundoApellido, '')) AS NombreCompleto,
    p.Nombres,
    p.PrimerApellido,
    p.SegundoApellido,
    p.EmailPersonal,
    p.EmailInstitucional,
    p.TelefonoCelular,
    p.CodigoEmpleado,
    
    -- Información Laboral
    p.TipoContrato,
    p.CategoriaLaboral,
    p.TipoJornada,
    p.HorasLaborales,
    p.FechaIngreso,
    p.Estado,
    
    -- Información Académica
    p.GradoAcademico,
    p.TituloAcademico,
    p.InstitucionGraduacion,
    
    -- Información Geográfica
    d.Nombre AS Distrito,
    c.Nombre AS Canton,
    pr.Nombre AS Provincia,
    
    -- Información Institucional
    e.Nombre AS Escuela,
    f.Nombre AS Facultad,
    i.Nombre AS Institucion,
    i.Siglas AS SiglasInstitucion,
    
    -- Cargos y Permisos
    p.EsDirector,
    p.EsCoordinador,
    p.EsDecano,
    p.CargoAdministrativo,
    p.PuedeCrearRubricas,
    p.PuedeEvaluarEstudiantes,
    p.EsAdministradorSistema
FROM Profesores p
LEFT JOIN Distritos d ON p.DistritoId = d.Id
LEFT JOIN Cantones c ON d.CantonId = c.Id
LEFT JOIN Provincias pr ON c.ProvinciaId = pr.Id
LEFT JOIN Escuelas e ON p.EscuelaId = e.Id
LEFT JOIN Facultades f ON e.FacultadId = f.Id
LEFT JOIN Instituciones i ON f.InstitucionId = i.Id;

GO
-- Vista de Profesores con sus materias actuales
CREATE VIEW vw_ProfesoresMaterias AS
SELECT 
    p.Id AS ProfesorId,
    CONCAT(p.Nombres, ' ', p.PrimerApellido, ' ', ISNULL(p.SegundoApellido, '')) AS NombreProfesor,
    p.EmailInstitucional,
    m.Codigo AS CodigoMateria,
    m.Nombre AS NombreMateria,
    pm.Ciclo,
    pm.Anio,
    pm.Grupo,
    pm.HorarioClases,
    pm.AulaAsignada,
    pm.CantidadEstudiantes,
    e.Nombre AS Escuela,
    f.Nombre AS Facultad
FROM Profesores p
INNER JOIN ProfesorMaterias pm ON p.Id = pm.ProfesorId
INNER JOIN Materias m ON pm.MateriaId = m.Id
INNER JOIN Escuelas e ON m.EscuelaId = e.Id
INNER JOIN Facultades f ON e.FacultadId = f.Id
WHERE pm.Estado = 1 AND p.Estado = 'Activo';

GO
-- Vista de carga académica por profesor
CREATE VIEW vw_CargaAcademicaProfesores AS
SELECT 
    p.Id AS ProfesorId,
    CONCAT(p.Nombres, ' ', p.PrimerApellido) AS NombreProfesor,
    p.TipoJornada,
    p.HorasLaborales,
    COUNT(pm.Id) AS TotalMaterias,
    SUM(pm.CantidadEstudiantes) AS TotalEstudiantes,
    STRING_AGG(CONCAT(m.Codigo, ' - ', pm.Grupo), ', ') AS MateriasImpartidas
FROM Profesores p
LEFT JOIN ProfesorMaterias pm ON p.Id = pm.ProfesorId AND pm.Estado = 1
LEFT JOIN Materias m ON pm.MateriaId = m.Id
WHERE p.Estado = 'Activo'
GROUP BY p.Id, p.Nombres, p.PrimerApellido, p.TipoJornada, p.HorasLaborales;

GO
-- ============================================================
-- CONSULTAS ÚTILES
-- ============================================================

-- 1. Listado de todos los profesores activos por escuela
SELECT 
    e.Nombre AS Escuela,
    f.Nombre AS Facultad,
    COUNT(*) AS CantidadProfesores,
    SUM(CASE WHEN p.TipoJornada = 'Tiempo Completo' THEN 1 ELSE 0 END) AS TiempoCompleto,
    SUM(CASE WHEN p.TipoJornada = 'Medio Tiempo' THEN 1 ELSE 0 END) AS MedioTiempo,
    SUM(CASE WHEN p.TipoJornada = 'Cuarto Tiempo' THEN 1 ELSE 0 END) AS CuartoTiempo
FROM Profesores p
INNER JOIN Escuelas e ON p.EscuelaId = e.Id
INNER JOIN Facultades f ON e.FacultadId = f.Id
WHERE p.Estado = 'Activo'
GROUP BY e.Nombre, f.Nombre
ORDER BY f.Nombre, e.Nombre;

-- 2. Profesores con mayor carga académica (más estudiantes)
SELECT TOP 10
    CONCAT(p.Nombres, ' ', p.PrimerApellido, ' ', ISNULL(p.SegundoApellido, '')) AS NombreCompleto,
    p.EmailInstitucional,
    e.Nombre AS Escuela,
    COUNT(pm.Id) AS TotalMaterias,
    SUM(pm.CantidadEstudiantes) AS TotalEstudiantes
FROM Profesores p
INNER JOIN ProfesorMaterias pm ON p.Id = pm.ProfesorId
INNER JOIN Escuelas e ON p.EscuelaId = e.Id
WHERE p.Estado = 'Activo' AND pm.Estado = 1
GROUP BY p.Id, p.Nombres, p.PrimerApellido, p.SegundoApellido, p.EmailInstitucional, e.Nombre
ORDER BY TotalEstudiantes DESC;

-- 3. Profesores por grado académico
SELECT 
    p.GradoAcademico,
    COUNT(*) AS Cantidad,
    ROUND(COUNT(*) * 100.0 / (SELECT COUNT(*) FROM Profesores WHERE Estado = 'Activo'), 2) AS Porcentaje
FROM Profesores p
WHERE p.Estado = 'Activo' AND p.GradoAcademico IS NOT NULL
GROUP BY p.GradoAcademico
ORDER BY Cantidad DESC;

-- 4. Profesores próximos a cumplir años de servicio (25 años)
SELECT 
    CONCAT(p.Nombres, ' ', p.PrimerApellido, ' ', ISNULL(p.SegundoApellido, '')) AS NombreCompleto,
    p.FechaIngreso,
    DATEDIFF(YEAR, p.FechaIngreso, GETDATE()) AS AniosServicio,
    e.Nombre AS Escuela,
    p.EmailInstitucional,
    p.TelefonoCelular
FROM Profesores p
INNER JOIN Escuelas e ON p.EscuelaId = e.Id
WHERE p.Estado = 'Activo' 
  AND DATEDIFF(YEAR, p.FechaIngreso, GETDATE()) >= 24
ORDER BY p.FechaIngreso;

-- 5. Distribución de profesores por provincia
SELECT 
    pr.Nombre AS Provincia,
    COUNT(*) AS CantidadProfesores
FROM Profesores p
INNER JOIN Distritos d ON p.DistritoId = d.Id
INNER JOIN Cantones c ON d.CantonId = c.Id
INNER JOIN Provincias pr ON c.ProvinciaId = pr.Id
WHERE p.Estado = 'Activo'
GROUP BY pr.Nombre
ORDER BY CantidadProfesores DESC;

GO
-- ============================================================
-- PROCEDIMIENTOS ALMACENADOS ÚTILES
-- ============================================================

-- Procedimiento para buscar profesores por múltiples criterios
CREATE PROCEDURE sp_BuscarProfesores
    @Nombre NVARCHAR(100) = NULL,
    @Cedula NVARCHAR(20) = NULL,
    @Escuela NVARCHAR(200) = NULL,
    @Estado NVARCHAR(20) = 'Activo',
    @TipoContrato NVARCHAR(50) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT * FROM vw_ProfesoresCompleto
    WHERE (@Nombre IS NULL OR (Nombres LIKE '%' + @Nombre + '%' OR PrimerApellido LIKE '%' + @Nombre + '%'))
      AND (@Cedula IS NULL OR Cedula = @Cedula)
      AND (@Escuela IS NULL OR Escuela LIKE '%' + @Escuela + '%')
      AND Estado = @Estado
      AND (@TipoContrato IS NULL OR TipoContrato = @TipoContrato)
    ORDER BY PrimerApellido, Nombres;
END;

GO
-- Procedimiento para obtener el historial académico completo de un profesor
CREATE PROCEDURE sp_HistorialAcademicoProfesor
    @ProfesorId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Información básica del profesor
    SELECT * FROM vw_ProfesoresCompleto WHERE Id = @ProfesorId;
    
    -- Formación académica
    SELECT 
        TipoFormacion,
        TituloObtenido,
        InstitucionEducativa,
        PaisInstitucion,
        AnioInicio,
        AnioFinalizacion,
        EnCurso,
        PromedioGeneral
    FROM ProfesorFormacionAcademica 
    WHERE ProfesorId = @ProfesorId 
    ORDER BY AnioFinalizacion DESC;
    
    -- Experiencia laboral
    SELECT 
        NombreInstitucion,
        CargoDesempenado,
        TipoInstitucion,
        FechaInicio,
        FechaFin,
        TrabajandoActualmente,
        DATEDIFF(MONTH, FechaInicio, ISNULL(FechaFin, GETDATE())) AS MesesExperiencia
    FROM ProfesorExperienciaLaboral 
    WHERE ProfesorId = @ProfesorId 
    ORDER BY FechaInicio DESC;
    
    -- Capacitaciones recientes (últimos 3 años)
    SELECT 
        NombreCapacitacion,
        InstitucionOrganizadora,
        TipoCapacitacion,
        FechaInicio,
        FechaFin,
        HorasCapacitacion,
        CertificadoObtenido
    FROM ProfesorCapacitaciones 
    WHERE ProfesorId = @ProfesorId 
      AND FechaInicio >= DATEADD(YEAR, -3, GETDATE())
    ORDER BY FechaInicio DESC;
END;

GO
-- ============================================================
-- TRIGGERS PARA AUDITORÍA
-- ============================================================

-- Trigger para actualizar fecha de modificación
CREATE TRIGGER tr_Profesores_UpdateModificacion
ON Profesores
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE p
    SET FechaModificacion = GETDATE(),
        Version = Version + 1
    FROM Profesores p
    INNER JOIN inserted i ON p.Id = i.Id;
END;

GO
-- ============================================================
-- FUNCIONES ÚTILES
-- ============================================================

-- Función para calcular años de experiencia docente
CREATE FUNCTION fn_CalcularExperienciaDocente(@ProfesorId INT)
RETURNS INT
AS
BEGIN
    DECLARE @AniosExperiencia INT;
    
    SELECT @AniosExperiencia = SUM(
        CASE 
            WHEN TrabajandoActualmente = 1 THEN DATEDIFF(YEAR, FechaInicio, GETDATE())
            ELSE DATEDIFF(YEAR, FechaInicio, FechaFin)
        END
    )
    FROM ProfesorExperienciaLaboral
    WHERE ProfesorId = @ProfesorId
      AND TipoInstitucion IN ('Universidad', 'Colegio', 'Escuela');
    
    RETURN ISNULL(@AniosExperiencia, 0);
END;

GO
-- Función para obtener el nombre completo formateado
CREATE FUNCTION fn_NombreCompletoProfesor(@ProfesorId INT)
RETURNS NVARCHAR(300)
AS
BEGIN
    DECLARE @NombreCompleto NVARCHAR(300);
    
    SELECT @NombreCompleto = CONCAT(
        CASE 
            WHEN GradoAcademico = 'Doctorado' THEN 'Dr. '
            WHEN GradoAcademico = 'Maestría' THEN 'M.Sc. '
            WHEN GradoAcademico = 'Licenciatura' THEN 'Lic. '
            ELSE ''
        END,
        Nombres, ' ', PrimerApellido, 
        CASE WHEN SegundoApellido IS NOT NULL THEN ' ' + SegundoApellido ELSE '' END
    )
    FROM Profesores
    WHERE Id = @ProfesorId;
    
    RETURN @NombreCompleto;
END;

GO

-- ============================================================
-- EJEMPLOS DE USO
-- ============================================================

/*
-- Buscar profesores por nombre
EXEC sp_BuscarProfesores @Nombre = 'Juan';

-- Obtener historial completo
EXEC sp_HistorialAcademicoProfesor @ProfesorId = 1;

-- Calcular experiencia docente
SELECT dbo.fn_CalcularExperienciaDocente(1) AS AniosExperiencia;

-- Obtener nombre completo con título
SELECT dbo.fn_NombreCompletoProfesor(1) AS NombreCompleto;

-- Ver todos los profesores con su información completa
SELECT * FROM vw_ProfesoresCompleto WHERE Estado = 'Activo';

-- Ver carga académica actual
SELECT * FROM vw_CargaAcademicaProfesores ORDER BY TotalEstudiantes DESC;
*/
