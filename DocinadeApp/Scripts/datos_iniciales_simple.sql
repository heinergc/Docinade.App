-- ================================================
-- Script de Datos Iniciales - Version Corregida
-- Base de datos: RubricasDb.db
-- Fecha: 18 de agosto de 2025
-- Proposito: Insertar datos completos para pruebas
-- ================================================

BEGIN TRANSACTION;

-- 1. PERIODOS ACADEMICOS
INSERT OR IGNORE INTO PeriodosAcademicos (
    Id, Anio, Ciclo, FechaInicio, FechaFin, Activo, Codigo, Nombre, Tipo, NumeroPeriodo, 
    FechaCreacion, Estado
) VALUES 
(1, 2025, 'C1', '2025-01-15', '2025-05-15', 1, '2025-C1', 'Primer Cuatrimestre 2025', 1, 1, datetime('now'), 'Activo'),
(2, 2025, 'C2', '2025-05-16', '2025-09-15', 1, '2025-C2', 'Segundo Cuatrimestre 2025', 1, 2, datetime('now'), 'Activo'),
(3, 2025, 'S1', '2025-01-15', '2025-06-15', 0, '2025-S1', 'Primer Semestre 2025', 0, 1, datetime('now'), 'Activo'),
(4, 2024, 'C1', '2024-01-15', '2024-05-15', 0, '2024-C1', 'Primer Cuatrimestre 2024', 1, 1, datetime('now'), 'Inactivo'),
(5, 2024, 'C2', '2024-05-16', '2024-09-15', 0, '2024-C2', 'Segundo Cuatrimestre 2024', 1, 2, datetime('now'), 'Inactivo');

-- 2. MATERIAS
INSERT OR IGNORE INTO Materias (
    Id, Codigo, Nombre, Descripcion, Creditos, Activa, Tipo, Nivel, Prerrequisitos, 
    FechaCreacion, Estado, TotalHoras, HorasTeoricas, HorasPracticas
) VALUES 
(1, 'TEST001', 'Materia de Prueba', 'Materia creada para testing', 3, 1, 'Teorica', 'Intermedio', NULL, datetime('now'), 'Activa', 60, 40, 20),
(2, 'PROG101', 'Programacion I', 'Introduccion a la programacion', 4, 1, 'Teorica', 'Basico', NULL, datetime('now'), 'Activa', 80, 60, 20),
(3, 'MAT101', 'Matematicas I', 'Fundamentos matematicos', 5, 1, 'Teorica', 'Basico', NULL, datetime('now'), 'Activa', 100, 80, 20),
(4, 'ING101', 'Ingles I', 'Ingles basico', 3, 1, 'Teorica', 'Basico', NULL, datetime('now'), 'Activa', 60, 40, 20),
(5, 'FIS101', 'Fisica I', 'Fisica general', 4, 1, 'Teorica', 'Intermedio', 'MAT101', datetime('now'), 'Activa', 80, 60, 20),
(6, 'PROG201', 'Programacion II', 'Programacion avanzada', 4, 1, 'Teorica', 'Intermedio', 'PROG101', datetime('now'), 'Activa', 80, 60, 20),
(7, 'BDD101', 'Base de Datos I', 'Fundamentos de bases de datos', 4, 1, 'Teorica', 'Intermedio', 'PROG101', datetime('now'), 'Activa', 80, 60, 20),
(8, 'WEB101', 'Desarrollo Web I', 'Desarrollo web basico', 3, 1, 'Practica', 'Intermedio', 'PROG101', datetime('now'), 'Activa', 60, 20, 40);

-- 3. ESTUDIANTES
INSERT OR IGNORE INTO Estudiantes (
    Id, NumeroEstudiante, Nombres, Apellidos, Email, Telefono, FechaNacimiento, 
    Activo, FechaIngreso, Estado, Carrera, NumeroId, DireccionCorreo, Institucion, 
    Grupos, PeriodoAcademicoId
) VALUES 
(1, 'EST001', 'Juan Carlos', 'Perez Rodriguez', 'juan.perez@email.com', '555-1234', '2000-05-15', 1, '2023-01-15', 'Activo', 'Ingenieria de Sistemas', 'ID001', 'Calle 123', 'Universidad XYZ', 'Grupo A', 1),
(2, 'EST002', 'Maria Elena', 'Gonzalez Lopez', 'maria.gonzalez@email.com', '555-5678', '1999-08-22', 1, '2023-01-15', 'Activo', 'Ingenieria de Sistemas', 'ID002', 'Avenida 456', 'Universidad XYZ', 'Grupo A', 1),
(3, 'EST003', 'Carlos Andres', 'Martinez Vargas', 'carlos.martinez@email.com', '555-9012', '2001-03-10', 1, '2023-01-15', 'Activo', 'Ingenieria de Sistemas', 'ID003', 'Carrera 789', 'Universidad XYZ', 'Grupo B', 1),
(4, 'EST004', 'Ana Sofia', 'Ramirez Castro', 'ana.ramirez@email.com', '555-3456', '2000-11-08', 1, '2023-01-15', 'Activo', 'Ingenieria de Sistemas', 'ID004', 'Diagonal 321', 'Universidad XYZ', 'Grupo B', 1),
(5, 'EST005', 'Luis Fernando', 'Torres Silva', 'luis.torres@email.com', '555-7890', '1999-12-25', 1, '2023-01-15', 'Activo', 'Ingenieria de Sistemas', 'ID005', 'Transversal 654', 'Universidad XYZ', 'Grupo A', 2),
(6, 'EST006', 'Diana Patricia', 'Moreno Jimenez', 'diana.moreno@email.com', '555-2345', '2000-07-18', 1, '2023-01-15', 'Activo', 'Ingenieria de Sistemas', 'ID006', 'Circular 987', 'Universidad XYZ', 'Grupo A', 2),
(7, 'EST007', 'Alejandro', 'Gutierrez Herrera', 'alejandro.gutierrez@email.com', '555-6789', '2001-01-30', 1, '2023-08-15', 'Activo', 'Ingenieria de Sistemas', 'ID007', 'Autopista 147', 'Universidad XYZ', 'Grupo B', 2),
(8, 'EST008', 'Valentina', 'Sanchez Rojas', 'valentina.sanchez@email.com', '555-0123', '2000-09-14', 1, '2023-08-15', 'Activo', 'Ingenieria de Sistemas', 'ID008', 'Boulevard 258', 'Universidad XYZ', 'Grupo B', 2),
(9, 'EST009', 'Santiago', 'Ospina Mendez', 'santiago.ospina@email.com', '555-4567', '1999-04-03', 1, '2023-08-15', 'Activo', 'Ingenieria de Sistemas', 'ID009', 'Pasaje 369', 'Universidad XYZ', 'Grupo A', 3),
(10, 'EST010', 'Camila', 'Vargas Quintero', 'camila.vargas@email.com', '555-8901', '2001-06-21', 1, '2023-08-15', 'Activo', 'Ingenieria de Sistemas', 'ID010', 'Glorieta 741', 'Universidad XYZ', 'Grupo A', 3),
(11, 'EST011', 'Andres Felipe', 'Castro Mejia', 'andres.castro@email.com', '555-2468', '2000-02-12', 1, '2024-01-15', 'Activo', 'Ingenieria de Sistemas', 'ID011', 'Rotonda 852', 'Universidad XYZ', 'Grupo B', 4),
(12, 'EST012', 'Isabella', 'Restrepo Aguilar', 'isabella.restrepo@email.com', '555-1357', '1999-10-05', 1, '2024-01-15', 'Activo', 'Ingenieria de Sistemas', 'ID012', 'Puente 963', 'Universidad XYZ', 'Grupo B', 4);

-- 4. RUBRICAS
INSERT OR IGNORE INTO Rubricas (
    Id, Nombre, Descripcion, FechaCreacion, Activa, TipoRubrica, PesoTotal, 
    Estado, Observaciones, MateriaId
) VALUES 
(1, 'Rubrica Programacion I', 'Evaluacion integral de competencias en programacion basica', datetime('now'), 1, 'Formativa', 100.0, 'Activa', 'Rubrica principal para PROG101', 2),
(2, 'Rubrica Matematicas I', 'Evaluacion de conceptos matematicos fundamentales', datetime('now'), 1, 'Formativa', 100.0, 'Activa', 'Rubrica principal para MAT101', 3),
(3, 'Rubrica Ingles I', 'Evaluacion de competencias linguisticas basicas', datetime('now'), 1, 'Formativa', 100.0, 'Activa', 'Rubrica principal para ING101', 4),
(4, 'Rubrica Fisica I', 'Evaluacion de conceptos fisicos fundamentales', datetime('now'), 1, 'Formativa', 100.0, 'Activa', 'Rubrica principal para FIS101', 5),
(5, 'Rubrica Programacion II', 'Evaluacion de programacion avanzada', datetime('now'), 1, 'Formativa', 100.0, 'Activa', 'Rubrica principal para PROG201', 6),
(6, 'Rubrica Base de Datos I', 'Evaluacion de fundamentos de bases de datos', datetime('now'), 1, 'Formativa', 100.0, 'Activa', 'Rubrica principal para BDD101', 7);

-- 5. ITEMS DE EVALUACION
INSERT OR IGNORE INTO ItemsEvaluacion (
    Id, Nombre, Descripcion, Peso, RubricaId, Orden, TipoItem, 
    Estado, CriteriosEvaluacion
) VALUES 
-- Items para Programacion I
(1, 'Logica de Programacion', 'Capacidad para resolver problemas usando logica de programacion', 25.0, 1, 1, 'Competencia', 'Activo', 'Analiza problemas, diseña algoritmos, implementa soluciones'),
(2, 'Sintaxis y Semantica', 'Dominio de la sintaxis del lenguaje de programacion', 20.0, 1, 2, 'Conocimiento', 'Activo', 'Sintaxis correcta, uso apropiado de estructuras'),
(3, 'Estructuras de Control', 'Uso correcto de estructuras de control', 25.0, 1, 3, 'Competencia', 'Activo', 'Condicionales, bucles, funciones'),
(4, 'Documentacion y Estilo', 'Calidad de documentacion y estilo de codigo', 30.0, 1, 4, 'Actitud', 'Activo', 'Comentarios claros, indentacion, nombres descriptivos'),

-- Items para Matematicas I  
(5, 'Algebra Basica', 'Operaciones algebraicas fundamentales', 30.0, 2, 1, 'Conocimiento', 'Activo', 'Ecuaciones, factorizacion, simplificacion'),
(6, 'Funciones', 'Comprension y manejo de funciones matematicas', 25.0, 2, 2, 'Competencia', 'Activo', 'Dominio, rango, composicion, inversas'),
(7, 'Calculo Diferencial', 'Conceptos basicos de derivadas', 25.0, 2, 3, 'Competencia', 'Activo', 'Limites, derivadas, aplicaciones'),
(8, 'Resolucion de Problemas', 'Aplicacion de conceptos a problemas reales', 20.0, 2, 4, 'Competencia', 'Activo', 'Modelado, analisis, interpretacion'),

-- Items para Ingles I
(9, 'Comprension Lectora', 'Capacidad de comprension de textos en ingles', 25.0, 3, 1, 'Competencia', 'Activo', 'Vocabulario, contexto, inferencias'),
(10, 'Expresion Oral', 'Habilidades de comunicacion oral', 25.0, 3, 2, 'Competencia', 'Activo', 'Pronunciacion, fluidez, coherencia'),
(11, 'Gramatica', 'Conocimiento de estructuras gramaticales', 25.0, 3, 3, 'Conocimiento', 'Activo', 'Tiempos verbales, estructuras, sintaxis'),
(12, 'Expresion Escrita', 'Habilidades de escritura en ingles', 25.0, 3, 4, 'Competencia', 'Activo', 'Coherencia, cohesion, vocabulario'),

-- Items para Fisica I
(13, 'Mecanica Clasica', 'Conceptos de movimiento y fuerzas', 30.0, 4, 1, 'Conocimiento', 'Activo', 'Cinematica, dinamica, energia'),
(14, 'Termodinamica', 'Principios termodinamicos basicos', 25.0, 4, 2, 'Conocimiento', 'Activo', 'Calor, temperatura, leyes termodinamicas'),
(15, 'Ondas y Vibraciones', 'Fenomenos ondulatorios', 25.0, 4, 3, 'Competencia', 'Activo', 'Frecuencia, amplitud, propagacion'),
(16, 'Laboratorio', 'Habilidades experimentales', 20.0, 4, 4, 'Competencia', 'Activo', 'Medicion, analisis, interpretacion'),

-- Items para Programacion II
(17, 'POO Conceptos', 'Programacion Orientada a Objetos', 30.0, 5, 1, 'Conocimiento', 'Activo', 'Clases, objetos, herencia, polimorfismo'),
(18, 'Estructuras de Datos', 'Manejo de estructuras de datos avanzadas', 25.0, 5, 2, 'Competencia', 'Activo', 'Listas, pilas, colas, arboles'),
(19, 'Algoritmos', 'Diseño y analisis de algoritmos', 25.0, 5, 3, 'Competencia', 'Activo', 'Complejidad, ordenamiento, busqueda'),
(20, 'Proyecto Final', 'Desarrollo de aplicacion completa', 20.0, 5, 4, 'Proyecto', 'Activo', 'Diseño, implementacion, documentacion'),

-- Items para Base de Datos I
(21, 'Modelo Relacional', 'Conceptos del modelo relacional', 25.0, 6, 1, 'Conocimiento', 'Activo', 'Tablas, relaciones, normalizacion'),
(22, 'SQL Basico', 'Consultas SQL fundamentales', 30.0, 6, 2, 'Competencia', 'Activo', 'SELECT, INSERT, UPDATE, DELETE'),
(23, 'Diseño de BD', 'Diseño de bases de datos', 25.0, 6, 3, 'Competencia', 'Activo', 'ER, normalizacion, integridad'),
(24, 'Proyecto BD', 'Implementacion de base de datos', 20.0, 6, 4, 'Proyecto', 'Activo', 'Diseño completo, implementacion, consultas');

-- 6. NIVELES DE CALIFICACION
INSERT OR IGNORE INTO NivelesCalificacion (
    Id, Nombre, Descripcion, ValorMinimo, ValorMaximo, ItemEvaluacionId, 
    Orden, Estado, Color, EsAprobatorio
) VALUES 
-- Niveles para items de Programacion I (4 items x 4 niveles = 16 registros)
(1, 'Excelente', 'Demuestra dominio excepcional', 90, 100, 1, 1, 'Activo', '#4CAF50', 1),
(2, 'Bueno', 'Demuestra buen dominio', 70, 89, 1, 2, 'Activo', '#8BC34A', 1),
(3, 'Regular', 'Demuestra dominio basico', 50, 69, 1, 3, 'Activo', '#FFC107', 1),
(4, 'Deficiente', 'No demuestra dominio suficiente', 0, 49, 1, 4, 'Activo', '#F44336', 0),

(5, 'Excelente', 'Sintaxis perfecta y uso apropiado', 90, 100, 2, 1, 'Activo', '#4CAF50', 1),
(6, 'Bueno', 'Sintaxis correcta con errores menores', 70, 89, 2, 2, 'Activo', '#8BC34A', 1),
(7, 'Regular', 'Sintaxis basica con algunos errores', 50, 69, 2, 3, 'Activo', '#FFC107', 1),
(8, 'Deficiente', 'Sintaxis incorrecta frecuentemente', 0, 49, 2, 4, 'Activo', '#F44336', 0),

(9, 'Excelente', 'Uso magistral de estructuras', 90, 100, 3, 1, 'Activo', '#4CAF50', 1),
(10, 'Bueno', 'Uso correcto de estructuras', 70, 89, 3, 2, 'Activo', '#8BC34A', 1),
(11, 'Regular', 'Uso basico de estructuras', 50, 69, 3, 3, 'Activo', '#FFC107', 1),
(12, 'Deficiente', 'Uso incorrecto de estructuras', 0, 49, 3, 4, 'Activo', '#F44336', 0),

(13, 'Excelente', 'Documentacion y estilo impecables', 90, 100, 4, 1, 'Activo', '#4CAF50', 1),
(14, 'Bueno', 'Buena documentacion y estilo', 70, 89, 4, 2, 'Activo', '#8BC34A', 1),
(15, 'Regular', 'Documentacion basica', 50, 69, 4, 3, 'Activo', '#FFC107', 1),
(16, 'Deficiente', 'Falta documentacion y estilo', 0, 49, 4, 4, 'Activo', '#F44336', 0);

-- 7. EVALUACIONES
INSERT OR IGNORE INTO Evaluaciones (
    Id, Nombre, Descripcion, FechaEvaluacion, EstudianteId, RubricaId, 
    Estado, TipoEvaluacion, Observaciones, CalificacionTotal, 
    FechaCreacion, EvaluadorId
) VALUES 
(1, 'Evaluacion Parcial 1 - PROG101', 'Primera evaluacion parcial de Programacion I', '2025-03-15', 1, 1, 'Completada', 'Parcial', 'Buen desempeño general', 85.5, datetime('now'), 1),
(2, 'Evaluacion Parcial 1 - PROG101', 'Primera evaluacion parcial de Programacion I', '2025-03-15', 2, 1, 'Completada', 'Parcial', 'Excelente trabajo', 92.0, datetime('now'), 1),
(3, 'Evaluacion Parcial 1 - MAT101', 'Primera evaluacion parcial de Matematicas I', '2025-03-20', 1, 2, 'Completada', 'Parcial', 'Necesita reforzar algunos conceptos', 75.0, datetime('now'), 1),
(4, 'Evaluacion Parcial 1 - MAT101', 'Primera evaluacion parcial de Matematicas I', '2025-03-20', 3, 2, 'Completada', 'Parcial', 'Muy buen dominio', 88.5, datetime('now'), 1),
(5, 'Evaluacion Parcial 1 - ING101', 'Primera evaluacion parcial de Ingles I', '2025-03-25', 2, 3, 'Completada', 'Parcial', 'Excelente nivel de ingles', 95.0, datetime('now'), 1),
(6, 'Evaluacion Parcial 1 - ING101', 'Primera evaluacion parcial de Ingles I', '2025-03-25', 4, 3, 'Completada', 'Parcial', 'Buen progreso', 82.0, datetime('now'), 1),
(7, 'Evaluacion Parcial 1 - FIS101', 'Primera evaluacion parcial de Fisica I', '2025-04-01', 5, 4, 'Completada', 'Parcial', 'Conceptos claros', 87.5, datetime('now'), 1),
(8, 'Evaluacion Parcial 1 - PROG201', 'Primera evaluacion parcial de Programacion II', '2025-04-05', 6, 5, 'Completada', 'Parcial', 'Dominio avanzado', 90.0, datetime('now'), 1),
(9, 'Evaluacion Parcial 1 - BDD101', 'Primera evaluacion parcial de Base de Datos I', '2025-04-10', 7, 6, 'Completada', 'Parcial', 'Buen entendimiento', 83.5, datetime('now'), 1),
(10, 'Evaluacion Final - PROG101', 'Evaluacion final de Programacion I', '2025-05-15', 1, 1, 'Programada', 'Final', 'Evaluacion comprehensiva', NULL, datetime('now'), 1);

COMMIT;

-- Mostrar resumen de datos insertados
SELECT 'Periodos Academicos' as Tabla, COUNT(*) as Registros FROM PeriodosAcademicos
UNION ALL
SELECT 'Materias', COUNT(*) FROM Materias
UNION ALL  
SELECT 'Estudiantes', COUNT(*) FROM Estudiantes
UNION ALL
SELECT 'Rubricas', COUNT(*) FROM Rubricas
UNION ALL
SELECT 'Items Evaluacion', COUNT(*) FROM ItemsEvaluacion
UNION ALL
SELECT 'Niveles Calificacion', COUNT(*) FROM NivelesCalificacion
UNION ALL
SELECT 'Evaluaciones', COUNT(*) FROM Evaluaciones;