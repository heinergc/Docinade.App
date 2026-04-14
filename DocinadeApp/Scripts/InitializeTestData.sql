-- Script para inicializar datos de prueba
-- Ejecutar este script después de crear la base de datos

-- Insertar un período académico
INSERT INTO PeriodosAcademicos (NombrePeriodo, NumeroPeriodo, Año, FechaInicio, FechaFin, Estado)
VALUES ('Primer Cuatrimestre', 1, 2025, '2025-01-01', '2025-04-30', 'ACTIVO');

-- Insertar un grupo de calificación
INSERT INTO GrupoCalificacion (NombreGrupo, Descripcion, Estado, FechaCreacion)
VALUES ('Proyecto IO 2025', 'Grupo de calificación para Proyecto de Investigación de Operaciones 2025', 'ACTIVO', datetime('now'));

-- Insertar una rúbrica
INSERT INTO Rubricas (NombreRubrica, Descripcion, Estado, FechaCreacion, IdGrupo)
VALUES ('Proyecto 1 IO 2025', 'Evaluación del primer proyecto de Investigación de Operaciones 2025', 'ACTIVO', datetime('now'), 1);

-- Insertar niveles de calificación
INSERT INTO NivelesCalificacion (NombreNivel, Descripcion, ValorNumerico, ColorHex, IdGrupo)
VALUES 
('Excelente', 'Desempeño excelente', 4, '#28a745', 1),
('Bueno', 'Desempeño bueno', 3, '#17a2b8', 1),
('Regular', 'Desempeño regular', 2, '#ffc107', 1),
('Deficiente', 'Desempeño deficiente', 1, '#dc3545', 1);

-- Insertar ítems de evaluación
INSERT INTO ItemsEvaluacion (Descripcion, IdRubrica, OrdenItem, Peso)
VALUES 
('Análisis del problema', 1, 1, 25.0),
('Formulación matemática', 1, 2, 25.0),
('Solución e interpretación', 1, 3, 25.0),
('Presentación y documentación', 1, 4, 25.0);

-- Insertar valores de rúbrica (relación entre ítems y niveles)
INSERT INTO ValoresRubrica (IdItem, IdNivel, IdRubrica, Valor, Descripcion)
VALUES 
-- Item 1: Análisis del problema
(1, 1, 1, 4.0, 'Análisis completo y profundo del problema'),
(1, 2, 1, 3.0, 'Análisis adecuado del problema'),
(1, 3, 1, 2.0, 'Análisis básico del problema'),
(1, 4, 1, 1.0, 'Análisis deficiente del problema'),

-- Item 2: Formulación matemática  
(2, 1, 1, 4.0, 'Formulación matemática correcta y completa'),
(2, 2, 1, 3.0, 'Formulación matemática adecuada'),
(2, 3, 1, 2.0, 'Formulación matemática básica'),
(2, 4, 1, 1.0, 'Formulación matemática incorrecta'),

-- Item 3: Solución e interpretación
(3, 1, 1, 4.0, 'Solución correcta y interpretación excelente'),
(3, 2, 1, 3.0, 'Solución correcta y interpretación adecuada'),
(3, 3, 1, 2.0, 'Solución parcial y interpretación básica'),
(3, 4, 1, 1.0, 'Solución incorrecta y interpretación deficiente'),

-- Item 4: Presentación y documentación
(4, 1, 1, 4.0, 'Presentación excelente y documentación completa'),
(4, 2, 1, 3.0, 'Presentación buena y documentación adecuada'),
(4, 3, 1, 2.0, 'Presentación básica y documentación incompleta'),
(4, 4, 1, 1.0, 'Presentación deficiente y documentación inadecuada');

-- Insertar RubricaNiveles (tabla intermedia)
INSERT INTO RubricaNiveles (IdRubrica, IdNivel)
VALUES 
(1, 1), (1, 2), (1, 3), (1, 4);

-- Insertar un estudiante de prueba
INSERT INTO Estudiantes (Nombre, Apellidos, NumeroId, DireccionCorreo, Institucion, Grupos, Año, PeriodoAcademicoId)
VALUES ('LADY YARENIS', 'ABARCA BRENES', '0208390339', 'lady.abarca@uned.cr', 'PALMARES (06)', 'Grupo 02: Tutor Heiner Guido Cambronero', 2025, 1);