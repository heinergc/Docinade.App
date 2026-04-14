-- ================================================================
-- SCRIPT PARA INSERTAR 50 ESTUDIANTES DEL MEP COSTA RICA
-- ================================================================
-- Base de datos: RubricasDb (SQL Server)
-- Tabla: Estudiantes
-- Contexto: Ministerio de Educación Pública de Costa Rica
-- Fecha de creación: 22 de septiembre de 2025
-- ================================================================

USE RubricasDb;
GO

PRINT '🏫 Iniciando inserción de 50 estudiantes del MEP Costa Rica...';

-- Verificar que existe la tabla Estudiantes
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Estudiantes')
BEGIN
    PRINT '❌ Error: La tabla Estudiantes no existe';
    RETURN;
END

-- Verificar que existe al menos un PeriodoAcademico
DECLARE @PeriodoId INT;
SELECT TOP 1 @PeriodoId = Id FROM PeriodosAcademicos WHERE Activo = 1;

IF @PeriodoId IS NULL
BEGIN
    PRINT '⚠️ No se encontró un período académico activo. Creando período por defecto...';
    
    INSERT INTO PeriodosAcademicos (Nombre, FechaInicio, FechaFin, Activo, FechaCreacion)
    VALUES ('2025 Curso Lectivo', '2025-02-12', '2025-12-06', 1, GETDATE());
    
    SELECT @PeriodoId = SCOPE_IDENTITY();
    PRINT '✅ Período académico creado con ID: ' + CAST(@PeriodoId AS VARCHAR(10));
END
ELSE
BEGIN
    PRINT '✅ Usando período académico con ID: ' + CAST(@PeriodoId AS VARCHAR(10));
END

-- Insertar los 50 estudiantes
BEGIN TRANSACTION;

BEGIN TRY
    -- Lote 1: Estudiantes de Colegio Científico de Costa Rica (1-15)
    INSERT INTO Estudiantes (Nombre, Apellidos, NumeroId, DireccionCorreo, Institucion, Grupos, Año, PeriodoAcademicoId)
    VALUES 
    ('José Andrés', 'Vargas Solano', '117850421', 'jose.vargas@estudiantes.mep.go.cr', 'Colegio Científico de Costa Rica', '11-A', 2025, @PeriodoId),
    ('María Fernanda', 'Jiménez Castillo', '207940312', 'maria.jimenez@estudiantes.mep.go.cr', 'Colegio Científico de Costa Rica', '11-A', 2025, @PeriodoId),
    ('Carlos Daniel', 'Morales Hernández', '305820193', 'carlos.morales@estudiantes.mep.go.cr', 'Colegio Científico de Costa Rica', '11-A', 2025, @PeriodoId),
    ('Ana Lucía', 'Rodríguez Campos', '401730264', 'ana.rodriguez@estudiantes.mep.go.cr', 'Colegio Científico de Costa Rica', '11-A', 2025, @PeriodoId),
    ('Diego Alejandro', 'Sánchez Mora', '118640375', 'diego.sanchez@estudiantes.mep.go.cr', 'Colegio Científico de Costa Rica', '11-A', 2025, @PeriodoId),
    ('Valeria Sofía', 'Quesada Villalobos', '209550186', 'valeria.quesada@estudiantes.mep.go.cr', 'Colegio Científico de Costa Rica', '11-B', 2025, @PeriodoId),
    ('Sebastián José', 'Araya Ramírez', '306460297', 'sebastian.araya@estudiantes.mep.go.cr', 'Colegio Científico de Costa Rica', '11-B', 2025, @PeriodoId),
    ('Camila Andrea', 'Alfaro González', '402370158', 'camila.alfaro@estudiantes.mep.go.cr', 'Colegio Científico de Costa Rica', '11-B', 2025, @PeriodoId),
    ('Adrián Fernando', 'Chacón Herrera', '119280349', 'adrian.chacon@estudiantes.mep.go.cr', 'Colegio Científico de Costa Rica', '11-B', 2025, @PeriodoId),
    ('Isabella María', 'Cordero Vega', '210190450', 'isabella.cordero@estudiantes.mep.go.cr', 'Colegio Científico de Costa Rica', '11-B', 2025, @PeriodoId),
    ('Esteban Andrés', 'Bolaños Picado', '307080211', 'esteban.bolanos@estudiantes.mep.go.cr', 'Colegio Científico de Costa Rica', '11-C', 2025, @PeriodoId),
    ('Natalia Paola', 'Monge Fallas', '403990322', 'natalia.monge@estudiantes.mep.go.cr', 'Colegio Científico de Costa Rica', '11-C', 2025, @PeriodoId),
    ('Leonardo José', 'Acuña Salazar', '120420533', 'leonardo.acuna@estudiantes.mep.go.cr', 'Colegio Científico de Costa Rica', '11-C', 2025, @PeriodoId),
    ('Gabriela Sofía', 'Trejos Vindas', '211330184', 'gabriela.trejos@estudiantes.mep.go.cr', 'Colegio Científico de Costa Rica', '11-C', 2025, @PeriodoId),
    ('Mauricio Daniel', 'Quirós Calderón', '308710295', 'mauricio.quiros@estudiantes.mep.go.cr', 'Colegio Científico de Costa Rica', '11-C', 2025, @PeriodoId);

    -- Lote 2: Estudiantes de Liceo de Costa Rica (16-25)
    INSERT INTO Estudiantes (Nombre, Apellidos, NumeroId, DireccionCorreo, Institucion, Grupos, Año, PeriodoAcademicoId)
    VALUES 
    ('Alejandro José', 'Mata Rojas', '405140406', 'alejandro.mata@estudiantes.mep.go.cr', 'Liceo de Costa Rica', '10-A', 2025, @PeriodoId),
    ('Stephanie María', 'Leiva Castro', '121560217', 'stephanie.leiva@estudiantes.mep.go.cr', 'Liceo de Costa Rica', '10-A', 2025, @PeriodoId),
    ('Andrés Felipe', 'Aguilar Bonilla', '212470328', 'andres.aguilar@estudiantes.mep.go.cr', 'Liceo de Costa Rica', '10-A', 2025, @PeriodoId),
    ('Priscilla Andrea', 'Varela Esquivel', '309350439', 'priscilla.varela@estudiantes.mep.go.cr', 'Liceo de Costa Rica', '10-A', 2025, @PeriodoId),
    ('Kevin Esteban', 'Montero Navarro', '406780540', 'kevin.montero@estudiantes.mep.go.cr', 'Liceo de Costa Rica', '10-A', 2025, @PeriodoId),
    ('Daniela Sofía', 'Solano Jiménez', '122690151', 'daniela.solano@estudiantes.mep.go.cr', 'Liceo de Costa Rica', '10-B', 2025, @PeriodoId),
    ('Bryan Alexander', 'Hernández Ulate', '213580262', 'bryan.hernandez@estudiantes.mep.go.cr', 'Liceo de Costa Rica', '10-B', 2025, @PeriodoId),
    ('Karla Patricia', 'Cascante Méndez', '310460373', 'karla.cascante@estudiantes.mep.go.cr', 'Liceo de Costa Rica', '10-B', 2025, @PeriodoId),
    ('Óscar Emilio', 'Barrantes Zúñiga', '407920484', 'oscar.barrantes@estudiantes.mep.go.cr', 'Liceo de Costa Rica', '10-B', 2025, @PeriodoId),
    ('Fabiola María', 'Elizondo Porras', '123830195', 'fabiola.elizondo@estudiantes.mep.go.cr', 'Liceo de Costa Rica', '10-B', 2025, @PeriodoId);

    -- Lote 3: Estudiantes de Colegio de Cartago (26-35)
    INSERT INTO Estudiantes (Nombre, Apellidos, NumeroId, DireccionCorreo, Institucion, Grupos, Año, PeriodoAcademicoId)
    VALUES 
    ('Ricardo José', 'Valverde Madrigal', '214970306', 'ricardo.valverde@estudiantes.mep.go.cr', 'Colegio de Cartago', '9-A', 2025, @PeriodoId),
    ('Michelle Andrea', 'Fonseca Carrillo', '311580417', 'michelle.fonseca@estudiantes.mep.go.cr', 'Colegio de Cartago', '9-A', 2025, @PeriodoId),
    ('Jonathan David', 'Pérez Arias', '409060528', 'jonathan.perez@estudiantes.mep.go.cr', 'Colegio de Cartago', '9-A', 2025, @PeriodoId),
    ('Kimberly Sofía', 'Rojas Sandoval', '125100239', 'kimberly.rojas@estudiantes.mep.go.cr', 'Colegio de Cartago', '9-A', 2025, @PeriodoId),
    ('Cristopher Andrés', 'Vargas Gamboa', '216210340', 'cristopher.vargas@estudiantes.mep.go.cr', 'Colegio de Cartago', '9-A', 2025, @PeriodoId),
    ('Rebeca María', 'Sibaja Carvajal', '312720451', 'rebeca.sibaja@estudiantes.mep.go.cr', 'Colegio de Cartago', '9-B', 2025, @PeriodoId),
    ('Javier Emilio', 'Salas Montoya', '410200562', 'javier.salas@estudiantes.mep.go.cr', 'Colegio de Cartago', '9-B', 2025, @PeriodoId),
    ('Ashley Nicole', 'Murillo Jiménez', '126340173', 'ashley.murillo@estudiantes.mep.go.cr', 'Colegio de Cartago', '9-B', 2025, @PeriodoId),
    ('Fernando José', 'Barboza López', '217450284', 'fernando.barboza@estudiantes.mep.go.cr', 'Colegio de Cartago', '9-B', 2025, @PeriodoId),
    ('Paola Andrea', 'Campos Rivera', '313860395', 'paola.campos@estudiantes.mep.go.cr', 'Colegio de Cartago', '9-B', 2025, @PeriodoId);

    -- Lote 4: Estudiantes de Liceo de Heredia (36-45)
    INSERT INTO Estudiantes (Nombre, Apellidos, NumeroId, DireccionCorreo, Institucion, Grupos, Año, PeriodoAcademicoId)
    VALUES 
    ('Luis Fernando', 'Cortés Badilla', '411480506', 'luis.cortes@estudiantes.mep.go.cr', 'Liceo de Heredia', '8-A', 2025, @PeriodoId),
    ('Karina Alejandra', 'Villalobos Cruz', '127590117', 'karina.villalobos@estudiantes.mep.go.cr', 'Liceo de Heredia', '8-A', 2025, @PeriodoId),
    ('Rodrigo Antonio', 'Mora Blanco', '218700228', 'rodrigo.mora@estudiantes.mep.go.cr', 'Liceo de Heredia', '8-A', 2025, @PeriodoId),
    ('Melany Sofía', 'Delgado Chaves', '315010339', 'melany.delgado@estudiantes.mep.go.cr', 'Liceo de Heredia', '8-A', 2025, @PeriodoId),
    ('Jorge Daniel', 'Ramírez Flores', '412620440', 'jorge.ramirez@estudiantes.mep.go.cr', 'Liceo de Heredia', '8-A', 2025, @PeriodoId),
    ('Tatiana María', 'Rodríguez Vega', '128730551', 'tatiana.rodriguez@estudiantes.mep.go.cr', 'Liceo de Heredia', '8-B', 2025, @PeriodoId),
    ('William José', 'Salazar Quesada', '219840162', 'william.salazar@estudiantes.mep.go.cr', 'Liceo de Heredia', '8-B', 2025, @PeriodoId),
    ('Viviana Andrea', 'Céspedes Mora', '316150273', 'viviana.cespedes@estudiantes.mep.go.cr', 'Liceo de Heredia', '8-B', 2025, @PeriodoId),
    ('Alexis David', 'Jiménez Solís', '413760384', 'alexis.jimenez@estudiantes.mep.go.cr', 'Liceo de Heredia', '8-B', 2025, @PeriodoId),
    ('Carolina Paola', 'Mesén Alfaro', '129870495', 'carolina.mesen@estudiantes.mep.go.cr', 'Liceo de Heredia', '8-B', 2025, @PeriodoId);

    -- Lote 5: Estudiantes de Colegio de Puntarenas (46-50)
    INSERT INTO Estudiantes (Nombre, Apellidos, NumeroId, DireccionCorreo, Institucion, Grupos, Año, PeriodoAcademicoId)
    VALUES 
    ('Gabriel Esteban', 'Vindas Camacho', '220980106', 'gabriel.vindas@estudiantes.mep.go.cr', 'Colegio de Puntarenas', '7-A', 2025, @PeriodoId),
    ('Sofía Valentina', 'Carranza Ugalde', '317290217', 'sofia.carranza@estudiantes.mep.go.cr', 'Colegio de Puntarenas', '7-A', 2025, @PeriodoId),
    ('Marco Antonio', 'Piedra Gamboa', '414900328', 'marco.piedra@estudiantes.mep.go.cr', 'Colegio de Puntarenas', '7-A', 2025, @PeriodoId),
    ('Alejandra Isabel', 'Centeno Vargas', '131010439', 'alejandra.centeno@estudiantes.mep.go.cr', 'Colegio de Puntarenas', '7-A', 2025, @PeriodoId),
    ('Cristian David', 'Umaña Solano', '222120540', 'cristian.umana@estudiantes.mep.go.cr', 'Colegio de Puntarenas', '7-A', 2025, @PeriodoId);

    COMMIT TRANSACTION;
    PRINT '✅ Se insertaron exitosamente 50 estudiantes del MEP Costa Rica';

END TRY
BEGIN CATCH
    ROLLBACK TRANSACTION;
    PRINT '❌ Error al insertar estudiantes:';
    PRINT ERROR_MESSAGE();
    RETURN;
END CATCH

-- Verificar la inserción
PRINT '';
PRINT '📊 Verificación de datos insertados:';

-- Contar estudiantes por institución y nivel
SELECT 
    Institucion,
    Grupos,
    COUNT(*) as CantidadEstudiantes
FROM Estudiantes 
WHERE PeriodoAcademicoId = @PeriodoId
GROUP BY Institucion, Grupos
ORDER BY Institucion, Grupos;

-- Mostrar estadísticas generales
PRINT '';
PRINT '📈 Estadísticas generales del MEP:';

SELECT 
    'Total de estudiantes' as Descripcion,
    COUNT(*) as Cantidad
FROM Estudiantes 
WHERE PeriodoAcademicoId = @PeriodoId

UNION ALL

SELECT 
    'Colegios diferentes' as Descripcion,
    COUNT(DISTINCT Institucion) as Cantidad
FROM Estudiantes 
WHERE PeriodoAcademicoId = @PeriodoId

UNION ALL

SELECT 
    'Niveles educativos' as Descripcion,
    COUNT(DISTINCT SUBSTRING(Grupos, 1, CHARINDEX('-', Grupos + '-') - 1)) as Cantidad
FROM Estudiantes 
WHERE PeriodoAcademicoId = @PeriodoId;

-- Mostrar algunos registros para verificación
PRINT '';
PRINT '🔍 Muestra de estudiantes del MEP insertados:';

SELECT TOP 10
    IdEstudiante,
    NombreCompleto = Nombre + ' ' + Apellidos,
    NumeroId,
    DireccionCorreo,
    Institucion,
    Grupos
FROM Estudiantes 
WHERE PeriodoAcademicoId = @PeriodoId
ORDER BY IdEstudiante;

PRINT '';
PRINT '🎉 Script completado exitosamente';
PRINT '🏫 Se crearon 50 estudiantes del MEP Costa Rica distribuidos así:';
PRINT '   • Colegio Científico de Costa Rica (11°): 15 estudiantes';
PRINT '   • Liceo de Costa Rica (10°): 10 estudiantes';  
PRINT '   • Colegio de Cartago (9°): 10 estudiantes';
PRINT '   • Liceo de Heredia (8°): 10 estudiantes';
PRINT '   • Colegio de Puntarenas (7°): 5 estudiantes';
PRINT '';
PRINT '✨ Los estudiantes están listos para evaluaciones del MEP';
PRINT '🇨🇷 Todos con cédulas costarricenses y correos @estudiantes.mep.go.cr';

GO
