-- ================================================================
-- SCRIPT COMPACTO: 10 ESTUDIANTES DE PRUEBA
-- ================================================================
-- Para uso rápido en desarrollo y pruebas simples
-- ================================================================

USE RubricasDb;
GO

PRINT '📚 Insertando 10 estudiantes de prueba (versión compacta)...';

-- Obtener período académico activo
DECLARE @PeriodoId INT;
SELECT TOP 1 @PeriodoId = Id FROM PeriodosAcademicos WHERE Activo = 1;

-- Crear período si no existe
IF @PeriodoId IS NULL
BEGIN
    INSERT INTO PeriodosAcademicos (Nombre, FechaInicio, FechaFin, Activo, FechaCreacion)
    VALUES ('2025-II', '2025-08-01', '2025-12-15', 1, GETDATE());
    SELECT @PeriodoId = SCOPE_IDENTITY();
END

-- Insertar 10 estudiantes básicos
INSERT INTO Estudiantes (Nombre, Apellidos, NumeroId, DireccionCorreo, Institucion, Grupos, Año, PeriodoAcademicoId)
VALUES 
('Juan Carlos', 'Pérez García', 'EST001', 'juan.perez@unal.edu.co', 'Universidad Nacional', 'A1', 2025, @PeriodoId),
('María José', 'López Martínez', 'EST002', 'maria.lopez@unal.edu.co', 'Universidad Nacional', 'A1', 2025, @PeriodoId),
('Carlos Alberto', 'González Rivera', 'EST003', 'carlos.gonzalez@unal.edu.co', 'Universidad Nacional', 'A1', 2025, @PeriodoId),
('Ana Sofía', 'Rodríguez Cruz', 'EST004', 'ana.rodriguez@unal.edu.co', 'Universidad Nacional', 'A2', 2025, @PeriodoId),
('Diego Fernando', 'Martínez Silva', 'EST005', 'diego.martinez@unal.edu.co', 'Universidad Nacional', 'A2', 2025, @PeriodoId),
('Laura Melissa', 'Hernández Torres', 'EST006', 'laura.hernandez@unal.edu.co', 'Universidad Nacional', 'A2', 2025, @PeriodoId),
('Andrés Felipe', 'Vargas Morales', 'EST007', 'andres.vargas@unal.edu.co', 'Universidad Nacional', 'B1', 2025, @PeriodoId),
('Camila Andrea', 'Castro Jiménez', 'EST008', 'camila.castro@unal.edu.co', 'Universidad Nacional', 'B1', 2025, @PeriodoId),
('Sebastián', 'Ramos Flores', 'EST009', 'sebastian.ramos@unal.edu.co', 'Universidad Nacional', 'B1', 2025, @PeriodoId),
('Valentina', 'Sánchez Ruiz', 'EST010', 'valentina.sanchez@unal.edu.co', 'Universidad Nacional', 'B1', 2025, @PeriodoId);

-- Verificar inserción
SELECT COUNT(*) as EstudiantesInsertados FROM Estudiantes WHERE NumeroId LIKE 'EST%';

PRINT '✅ Script compacto completado - 10 estudiantes insertados';
GO
