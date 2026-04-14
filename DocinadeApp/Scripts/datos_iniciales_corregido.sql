-- ================================================
-- Script de Datos Iniciales - Versión Actualizada
-- Base de datos: RubricasDb.db
-- Fecha: 18 de agosto de 2025
-- Propósito: Insertar datos completos para pruebas con nueva estructura
-- ================================================

BEGIN TRANSACTION;

-- 1. PERÍODOS ACADÉMICOS (estructura actualizada)
INSERT OR IGNORE INTO PeriodosAcademicos (
    Id, Año, Anio, Ciclo, FechaInicio, FechaFin, Activo, Codigo, Nombre, Tipo, NumeroPeriodo, 
    FechaCreacion, FechaModificacion, Estado
) VALUES 
(1, 2025, 2025, 'C1', '2025-01-15', '2025-05-15', 1, '2025-C1', 'Primer Cuatrimestre 2025', 1, 1, datetime('now'), NULL, 'Activo'),
(2, 2025, 2025, 'C2', '2025-05-16', '2025-09-15', 1, '2025-C2', 'Segundo Cuatrimestre 2025', 1, 2, datetime('now'), NULL, 'Activo'),
(3, 2025, 2025, 'S1', '2025-01-15', '2025-06-15', 0, '2025-S1', 'Primer Semestre 2025', 0, 1, datetime('now'), NULL, 'Activo'),
(4, 2024, 2024, 'C1', '2024-01-15', '2024-05-15', 0, '2024-C1', 'Primer Cuatrimestre 2024', 1, 1, datetime('now'), NULL, 'Inactivo'),
(5, 2024, 2024, 'C2', '2024-05-16', '2024-09-15', 0, '2024-C2', 'Segundo Cuatrimestre 2024', 1, 2, datetime('now'), NULL, 'Inactivo');

-- 2. GRUPOS DE CALIFICACIÓN
INSERT OR IGNORE INTO GruposCalificacion (
    IdGrupo, NombreGrupo, Descripcion, Estado, FechaCreacion
) VALUES 
(1, 'Evaluación Estándar', 'Niveles de calificación estándar para evaluaciones generales', 'ACTIVO', datetime('now')),
(2, 'Evaluación Numérica', 'Niveles de calificación con puntuación numérica del 1 al 4', 'ACTIVO', datetime('now')),
(3, 'Evaluación Cualitativa', 'Niveles de calificación descriptivos para análisis cualitativo', 'ACTIVO', datetime('now')),
(4, 'Evaluación Porcentual', 'Niveles basados en porcentajes de logro', 'ACTIVO', datetime('now'));

-- 3. NIVELES DE CALIFICACIÓN (expandidos con más grupos)
INSERT OR IGNORE INTO NivelesCalificacion (
    IdNivel, NombreNivel, Descripcion, OrdenNivel, IdGrupo
) VALUES 
-- Para grupo estándar (IdGrupo = 1)
(1, 'Excelente', 'Demuestra dominio completo y aplicación innovadora de conceptos', 1, 1),
(2, 'Bueno', 'Cumple satisfactoriamente con los criterios establecidos', 2, 1),
(3, 'Regular', 'Cumple parcialmente, requiere mejoras específicas', 3, 1),
(4, 'Deficiente', 'No cumple con los criterios mínimos requeridos', 4, 1),

-- Para grupo numérico (IdGrupo = 2)  
(5, 'Nivel 4', 'Puntuación máxima - Desempeño excepcional', 1, 2),
(6, 'Nivel 3', 'Puntuación alta - Desempeño competente', 2, 2),
(7, 'Nivel 2', 'Puntuación media - Desempeño básico', 3, 2),
(8, 'Nivel 1', 'Puntuación mínima - Desempeño insuficiente', 4, 2),

-- Para grupo cualitativo (IdGrupo = 3)
(9, 'Sobresaliente', 'Supera ampliamente las expectativas', 1, 3),
(10, 'Satisfactorio', 'Cumple con las expectativas establecidas', 2, 3),
(11, 'En Desarrollo', 'Muestra progreso hacia las metas', 3, 3),
(12, 'Inicial', 'Necesita apoyo adicional para alcanzar las metas', 4, 3),

-- Para grupo porcentual (IdGrupo = 4)
(13, '90-100%', 'Logro excepcional del objetivo', 1, 4),
(14, '70-89%', 'Logro satisfactorio del objetivo', 2, 4),
(15, '50-69%', 'Logro parcial del objetivo', 3, 4),
(16, '0-49%', 'Logro insuficiente del objetivo', 4, 4);

-- 4. MATERIAS (estructura actualizada con nuevas propiedades)
INSERT OR IGNORE INTO Materias (
    MateriaId, Codigo, Nombre, Creditos, Activa, FechaCreacion, FechaModificacion, 
    Tipo, CicloSugerido, Descripcion, Estado
) VALUES 
(1, 'MAT101', 'Matemáticas Fundamentales', 4, 1, datetime('now'), NULL, 'Obligatoria', 1, 'Fundamentos matemáticos para carreras técnicas', 'ACTIVA'),
(2, 'ESP102', 'Español Técnico', 3, 1, datetime('now'), NULL, 'Obligatoria', 1, 'Comunicación escrita y oral en contextos técnicos', 'ACTIVA'),
(3, 'ING103', 'Inglés Técnico I', 3, 1, datetime('now'), NULL, 'Obligatoria', 1, 'Inglés básico para áreas técnicas', 'ACTIVA'),
(4, 'INF104', 'Introducción a la Informática', 4, 1, datetime('now'), NULL, 'Obligatoria', 1, 'Conceptos básicos de computación y sistemas', 'ACTIVA'),
(5, 'MAT201', 'Álgebra Lineal', 4, 1, datetime('now'), NULL, 'Obligatoria', 2, 'Matrices, vectores y transformaciones lineales', 'ACTIVA'),
(6, 'PRG201', 'Programación I', 5, 1, datetime('now'), NULL, 'Obligatoria', 2, 'Fundamentos de programación y algoritmos', 'ACTIVA'),
(7, 'EST301', 'Estadística Aplicada', 3, 1, datetime('now'), NULL, 'Electiva', 3, 'Análisis estadístico para la toma de decisiones', 'ACTIVA'),
(8, 'ING201', 'Inglés Técnico II', 3, 1, datetime('now'), NULL, 'Obligatoria', 2, 'Inglés intermedio para áreas técnicas', 'ACTIVA');

-- 5. INSTRUMENTOS DE EVALUACIÓN
INSERT OR IGNORE INTO InstrumentosEvaluacion (
    Id, Nombre, Descripcion, Activo, EstaActivo, FechaCreacion
) VALUES 
(1, 'Examen Parcial', 'Evaluación escrita de conocimientos teóricos y prácticos', 1, 1, datetime('now')),
(2, 'Proyecto Final', 'Trabajo integrador que demuestra competencias desarrolladas', 1, 1, datetime('now')),
(3, 'Laboratorio Práctico', 'Evaluación de habilidades técnicas en ambiente controlado', 1, 1, datetime('now')),
(4, 'Presentación Oral', 'Exposición y defensa de trabajos o proyectos', 1, 1, datetime('now')),
(5, 'Portafolio', 'Recopilación de evidencias de aprendizaje durante el curso', 1, 1, datetime('now')),
(6, 'Quiz Rápido', 'Evaluación breve de conceptos específicos', 1, 1, datetime('now')),
(7, 'Ensayo Reflexivo', 'Análisis crítico y reflexión sobre temas del curso', 1, 1, datetime('now')),
(8, 'Trabajo en Equipo', 'Evaluación de competencias colaborativas', 1, 1, datetime('now')),
(9, 'Simulacro Práctico', 'Simulación de situaciones reales de trabajo', 1, 1, datetime('now')),
(10, 'Autoevaluación', 'Reflexión del estudiante sobre su propio aprendizaje', 1, 1, datetime('now'));

-- 6. RÚBRICAS (con más variedad)
INSERT OR IGNORE INTO Rubricas (
    IdRubrica, NombreRubrica, Descripcion, FechaCreacion, FechaModificacion, Estado, 
    EsPublica, IdGrupo, Titulo, Vigente, CreadoPorId, ModificadoPorId
) VALUES 
(1, 'Evaluación de Proyectos Técnicos', 'Rúbrica para evaluar proyectos integradores en áreas técnicas', datetime('now'), NULL, 'ACTIVO', 1, 1, 'Proyectos Técnicos', 1, NULL, NULL),
(2, 'Competencias de Comunicación', 'Evaluación de habilidades comunicativas orales y escritas', datetime('now'), NULL, 'ACTIVO', 1, 1, 'Comunicación Efectiva', 1, NULL, NULL),
(3, 'Análisis y Resolución de Problemas', 'Evaluación de capacidades analíticas y de resolución', datetime('now'), NULL, 'ACTIVO', 1, 2, 'Pensamiento Crítico', 1, NULL, NULL),
(4, 'Trabajo Colaborativo', 'Evaluación de competencias de trabajo en equipo', datetime('now'), NULL, 'ACTIVO', 1, 1, 'Colaboración y Liderazgo', 1, NULL, NULL),
(5, 'Programación y Desarrollo', 'Evaluación de habilidades de programación', datetime('now'), NULL, 'ACTIVO', 1, 2, 'Desarrollo de Software', 1, NULL, NULL),
(6, 'Investigación Académica', 'Evaluación de competencias investigativas', datetime('now'), NULL, 'ACTIVO', 1, 3, 'Investigación y Análisis', 1, NULL, NULL);

-- 7. ÍTEMS DE EVALUACIÓN PARA CADA RÚBRICA
INSERT OR IGNORE INTO ItemsEvaluacion (
    IdItem, IdRubrica, NombreItem, Descripcion, OrdenItem, Peso
) VALUES 
-- Items para Rúbrica 1: Proyectos Técnicos
(1, 1, 'Planificación y Diseño', 'Calidad del plan de trabajo y diseño técnico propuesto', 1, 25.0),
(2, 1, 'Implementación Técnica', 'Ejecución y calidad técnica de la solución desarrollada', 2, 35.0),
(3, 1, 'Innovación y Creatividad', 'Originalidad y creatividad en el enfoque y solución', 3, 20.0),
(4, 1, 'Documentación', 'Calidad y completitud de la documentación técnica', 4, 20.0),

-- Items para Rúbrica 2: Comunicación
(5, 2, 'Claridad Expositiva', 'Capacidad de expresar ideas de forma clara y coherente', 1, 30.0),
(6, 2, 'Uso del Lenguaje Técnico', 'Apropiado uso de terminología técnica y profesional', 2, 25.0),
(7, 2, 'Organización del Contenido', 'Estructura lógica y secuencial de la información', 3, 25.0),
(8, 2, 'Interacción con Audiencia', 'Habilidad para responder preguntas y mantener engagement', 4, 20.0),

-- Items para Rúbrica 3: Análisis y Problemas
(9, 3, 'Identificación del Problema', 'Capacidad para definir y analizar el problema central', 1, 25.0),
(10, 3, 'Investigación y Análisis', 'Profundidad en la investigación y análisis de información', 2, 30.0),
(11, 3, 'Propuesta de Solución', 'Calidad y viabilidad de las soluciones propuestas', 3, 30.0),
(12, 3, 'Evaluación de Resultados', 'Capacidad para evaluar críticamente los resultados', 4, 15.0),

-- Items para Rúbrica 4: Trabajo Colaborativo
(13, 4, 'Participación Activa', 'Nivel de participación y contribución al equipo', 1, 25.0),
(14, 4, 'Comunicación Interpersonal', 'Habilidades de comunicación dentro del equipo', 2, 25.0),
(15, 4, 'Resolución de Conflictos', 'Capacidad para mediar y resolver conflictos', 3, 25.0),
(16, 4, 'Liderazgo y Coordinación', 'Habilidades de liderazgo y coordinación de actividades', 4, 25.0),

-- Items para Rúbrica 5: Programación
(17, 5, 'Lógica de Programación', 'Correcta aplicación de conceptos de programación', 1, 30.0),
(18, 5, 'Calidad del Código', 'Legibilidad, estructura y buenas prácticas', 2, 25.0),
(19, 5, 'Funcionalidad', 'El programa cumple con los requisitos especificados', 3, 30.0),
(20, 5, 'Manejo de Errores', 'Gestión apropiada de errores y excepciones', 4, 15.0),

-- Items para Rúbrica 6: Investigación
(21, 6, 'Marco Teórico', 'Fundamentación teórica sólida y actualizada', 1, 25.0),
(22, 6, 'Metodología', 'Apropiada selección y aplicación de métodos', 2, 25.0),
(23, 6, 'Análisis de Datos', 'Interpretación correcta y profunda de los datos', 3, 30.0),
(24, 6, 'Conclusiones', 'Coherencia entre hallazgos y conclusiones', 4, 20.0);

-- 8. RELACIONES RÚBRICA-NIVELES
INSERT OR IGNORE INTO RubricaNiveles (
    IdRubrica, IdNivel, OrdenEnRubrica
) VALUES 
-- Rúbrica 1 con niveles estándar (1-4)
(1, 1, 1), (1, 2, 2), (1, 3, 3), (1, 4, 4),
-- Rúbrica 2 con niveles estándar (1-4)
(2, 1, 1), (2, 2, 2), (2, 3, 3), (2, 4, 4),
-- Rúbrica 3 con niveles numéricos (5-8)
(3, 5, 1), (3, 6, 2), (3, 7, 3), (3, 8, 4),
-- Rúbrica 4 con niveles estándar (1-4)
(4, 1, 1), (4, 2, 2), (4, 3, 3), (4, 4, 4),
-- Rúbrica 5 con niveles numéricos (5-8)
(5, 5, 1), (5, 6, 2), (5, 7, 3), (5, 8, 4),
-- Rúbrica 6 con niveles cualitativos (9-12)
(6, 9, 1), (6, 10, 2), (6, 11, 3), (6, 12, 4);

-- 9. VALORES DE RÚBRICA (puntuaciones para cada item-nivel)
INSERT OR IGNORE INTO ValoresRubrica (
    IdValor, IdRubrica, IdItem, IdNivel, ValorPuntos
) VALUES 
-- Valores para Rúbrica 1 (Proyectos Técnicos) - Items 1-4, Niveles 1-4
(1, 1, 1, 1, 4.0), (2, 1, 1, 2, 3.0), (3, 1, 1, 3, 2.0), (4, 1, 1, 4, 1.0),
(5, 1, 2, 1, 4.0), (6, 1, 2, 2, 3.0), (7, 1, 2, 3, 2.0), (8, 1, 2, 4, 1.0),
(9, 1, 3, 1, 4.0), (10, 1, 3, 2, 3.0), (11, 1, 3, 3, 2.0), (12, 1, 3, 4, 1.0),
(13, 1, 4, 1, 4.0), (14, 1, 4, 2, 3.0), (15, 1, 4, 3, 2.0), (16, 1, 4, 4, 1.0),

-- Valores para Rúbrica 2 (Comunicación) - Items 5-8, Niveles 1-4
(17, 2, 5, 1, 4.0), (18, 2, 5, 2, 3.0), (19, 2, 5, 3, 2.0), (20, 2, 5, 4, 1.0),
(21, 2, 6, 1, 4.0), (22, 2, 6, 2, 3.0), (23, 2, 6, 3, 2.0), (24, 2, 6, 4, 1.0),
(25, 2, 7, 1, 4.0), (26, 2, 7, 2, 3.0), (27, 2, 7, 3, 2.0), (28, 2, 7, 4, 1.0),
(29, 2, 8, 1, 4.0), (30, 2, 8, 2, 3.0), (31, 2, 8, 3, 2.0), (32, 2, 8, 4, 1.0),

-- Valores para Rúbrica 3 (Análisis) - Items 9-12, Niveles 5-8
(33, 3, 9, 5, 4.0), (34, 3, 9, 6, 3.0), (35, 3, 9, 7, 2.0), (36, 3, 9, 8, 1.0),
(37, 3, 10, 5, 4.0), (38, 3, 10, 6, 3.0), (39, 3, 10, 7, 2.0), (40, 3, 10, 8, 1.0),
(41, 3, 11, 5, 4.0), (42, 3, 11, 6, 3.0), (43, 3, 11, 7, 2.0), (44, 3, 11, 8, 1.0),
(45, 3, 12, 5, 4.0), (46, 3, 12, 6, 3.0), (47, 3, 12, 7, 2.0), (48, 3, 12, 8, 1.0);

-- 10. ESTUDIANTES (con PeriodoAcademicoId correcto)
INSERT OR IGNORE INTO Estudiantes (
    IdEstudiante, Nombre, Apellidos, NumeroId, DireccionCorreo, Institucion, Grupos, Año, PeriodoAcademicoId
) VALUES 
(1, 'Ana', 'García López', 'EST001', 'ana.garcia@estudiantes.edu', 'Instituto Técnico Superior', 'A1', 2025, 1),
(2, 'Carlos', 'Rodríguez Pérez', 'EST002', 'carlos.rodriguez@estudiantes.edu', 'Instituto Técnico Superior', 'A1', 2025, 1),
(3, 'María', 'Fernández Silva', 'EST003', 'maria.fernandez@estudiantes.edu', 'Instituto Técnico Superior', 'A2', 2025, 1),
(4, 'José', 'Martín González', 'EST004', 'jose.martin@estudiantes.edu', 'Instituto Técnico Superior', 'A2', 2025, 1),
(5, 'Laura', 'Sánchez Torres', 'EST005', 'laura.sanchez@estudiantes.edu', 'Instituto Técnico Superior', 'B1', 2025, 2),
(6, 'Pedro', 'Jiménez Ruiz', 'EST006', 'pedro.jimenez@estudiantes.edu', 'Instituto Técnico Superior', 'B1', 2025, 2),
(7, 'Carmen', 'López Herrera', 'EST007', 'carmen.lopez@estudiantes.edu', 'Instituto Técnico Superior', 'B2', 2025, 2),
(8, 'Miguel', 'Torres Morales', 'EST008', 'miguel.torres@estudiantes.edu', 'Instituto Técnico Superior', 'B2', 2025, 2),
(9, 'Isabel', 'Ramos Jiménez', 'EST009', 'isabel.ramos@estudiantes.edu', 'Instituto Técnico Superior', 'C1', 2025, 3),
(10, 'Daniel', 'Moreno Castro', 'EST010', 'daniel.moreno@estudiantes.edu', 'Instituto Técnico Superior', 'C1', 2025, 3),
(11, 'Sofía', 'Vega Ramírez', 'EST011', 'sofia.vega@estudiantes.edu', 'Instituto Técnico Superior', 'A3', 2025, 1),
(12, 'Roberto', 'Cruz Mendoza', 'EST012', 'roberto.cruz@estudiantes.edu', 'Instituto Técnico Superior', 'B3', 2025, 2);

-- 11. RELACIONES MATERIA-PERÍODO (Ofertas de materias)
INSERT OR IGNORE INTO MateriaPeriodos (
    Id, MateriaId, PeriodoAcademicoId, Cupo, Estado, FechaCreacion, FechaPublicacion, Observaciones
) VALUES 
(1, 1, 1, 30, 'Abierta', datetime('now'), datetime('now'), 'Sección A - Horario matutino'),
(2, 1, 2, 25, 'Abierta', datetime('now'), datetime('now'), 'Sección B - Horario vespertino'),
(3, 2, 1, 35, 'Abierta', datetime('now'), datetime('now'), 'Grupo único - Salón 101'),
(4, 3, 1, 30, 'Abierta', datetime('now'), datetime('now'), 'Laboratorio de idiomas'),
(5, 4, 1, 20, 'Abierta', datetime('now'), datetime('now'), 'Laboratorio de cómputo A'),
(6, 5, 2, 25, 'Abierta', datetime('now'), datetime('now'), 'Sección avanzada'),
(7, 6, 2, 18, 'Abierta', datetime('now'), datetime('now'), 'Laboratorio de cómputo B'),
(8, 7, 3, 22, 'Abierta', datetime('now'), datetime('now'), 'Electiva - Estadística'),
(9, 8, 2, 28, 'Abierta', datetime('now'), datetime('now'), 'Continuación de Inglés I');

-- 12. RELACIONES MATERIA-RÚBRICA
INSERT OR IGNORE INTO MateriaRubricas (
    MateriaId, RubricaId, FechaAsignacion, EsObligatoria, Observaciones
) VALUES 
(1, 1, datetime('now'), 1, 'Rúbrica principal para proyectos matemáticos'),
(1, 3, datetime('now'), 0, 'Rúbrica adicional para resolución de problemas'),
(2, 2, datetime('now'), 1, 'Evaluación de comunicación escrita y oral'),
(3, 2, datetime('now'), 1, 'Evaluación de competencias comunicativas en inglés'),
(4, 1, datetime('now'), 1, 'Proyectos tecnológicos y de desarrollo'),
(4, 5, datetime('now'), 1, 'Competencias específicas de programación'),
(5, 1, datetime('now'), 1, 'Proyectos de álgebra aplicada'),
(6, 5, datetime('now'), 1, 'Evaluación de proyectos de programación'),
(7, 3, datetime('now'), 1, 'Análisis estadístico y resolución de problemas'),
(7, 6, datetime('now'), 0, 'Investigación aplicada en estadística');

-- 13. RELACIONES INSTRUMENTO-RÚBRICA
INSERT OR IGNORE INTO InstrumentoRubricas (
    InstrumentoEvaluacionId, RubricaId, Ponderacion, FechaAsignacion, EsObligatorio
) VALUES 
(1, 1, 40.0, datetime('now'), 1), -- Examen Parcial con Proyectos Técnicos
(2, 1, 60.0, datetime('now'), 1), -- Proyecto Final con Proyectos Técnicos
(4, 2, 100.0, datetime('now'), 1), -- Presentación Oral con Comunicación
(3, 1, 30.0, datetime('now'), 0), -- Laboratorio con Proyectos Técnicos
(5, 2, 50.0, datetime('now'), 0), -- Portafolio con Comunicación
(8, 4, 100.0, datetime('now'), 1), -- Trabajo en Equipo con Colaborativo
(7, 3, 100.0, datetime('now'), 1), -- Ensayo con Análisis y Problemas
(6, 3, 25.0, datetime('now'), 0), -- Quiz con Análisis y Problemas
(2, 5, 80.0, datetime('now'), 1), -- Proyecto Final con Programación
(3, 5, 20.0, datetime('now'), 1), -- Laboratorio con Programación
(7, 6, 70.0, datetime('now'), 1), -- Ensayo con Investigación
(5, 6, 30.0, datetime('now'), 0); -- Portafolio con Investigación

-- 14. RELACIONES INSTRUMENTO-MATERIA
INSERT OR IGNORE INTO InstrumentoMaterias (
    InstrumentoEvaluacionId, MateriaId, FechaAsignacion, EsObligatorio
) VALUES 
(1, 1, datetime('now'), 1), -- Examen Parcial en Matemáticas
(2, 1, datetime('now'), 1), -- Proyecto Final en Matemáticas
(4, 2, datetime('now'), 1), -- Presentación Oral en Español
(7, 2, datetime('now'), 1), -- Ensayo en Español
(4, 3, datetime('now'), 1), -- Presentación Oral en Inglés
(6, 3, datetime('now'), 1), -- Quiz en Inglés
(2, 4, datetime('now'), 1), -- Proyecto Final en Informática
(3, 4, datetime('now'), 1), -- Laboratorio en Informática
(1, 5, datetime('now'), 1), -- Examen en Álgebra
(2, 5, datetime('now'), 1), -- Proyecto en Álgebra
(2, 6, datetime('now'), 1), -- Proyecto en Programación
(3, 6, datetime('now'), 1), -- Laboratorio en Programación
(1, 7, datetime('now'), 1), -- Examen en Estadística
(7, 7, datetime('now'), 1), -- Ensayo en Estadística
(4, 8, datetime('now'), 1), -- Presentación en Inglés II
(6, 8, datetime('now'), 1); -- Quiz en Inglés II

-- 15. CUADERNOS CALIFICADORES
INSERT OR IGNORE INTO CuadernosCalificadores (
    Id, Nombre, Descripcion, MateriaId, PeriodoAcademicoId, Estado, FechaCreacion
) VALUES 
(1, 'Cuaderno MAT101 - C1 2025', 'Calificaciones de Matemáticas Fundamentales', 1, 1, 'ACTIVO', datetime('now')),
(2, 'Cuaderno ESP102 - C1 2025', 'Calificaciones de Español Técnico', 2, 1, 'ACTIVO', datetime('now')),
(3, 'Cuaderno ING103 - C1 2025', 'Calificaciones de Inglés Técnico I', 3, 1, 'ACTIVO', datetime('now')),
(4, 'Cuaderno INF104 - C1 2025', 'Calificaciones de Introducción a la Informática', 4, 1, 'ACTIVO', datetime('now')),
(5, 'Cuaderno PRG201 - C2 2025', 'Calificaciones de Programación I', 6, 2, 'ACTIVO', datetime('now'));

-- 16. INSTRUMENTOS EN CUADERNOS
INSERT OR IGNORE INTO CuadernoInstrumentos (
    Id, CuadernoCalificadorId, RubricaId, PonderacionPorcentaje, EsObligatorio, OrdenEvaluacion
) VALUES 
(1, 1, 1, 60.0, 1, 1), -- Proyectos Técnicos en MAT101
(2, 1, 3, 40.0, 1, 2), -- Análisis y Problemas en MAT101
(3, 2, 2, 100.0, 1, 1), -- Comunicación en ESP102
(4, 3, 2, 100.0, 1, 1), -- Comunicación en ING103
(5, 4, 1, 50.0, 1, 1), -- Proyectos Técnicos en INF104
(6, 4, 5, 50.0, 1, 2), -- Programación en INF104
(7, 5, 5, 80.0, 1, 1), -- Programación en PRG201
(8, 5, 4, 20.0, 1, 2); -- Trabajo Colaborativo en PRG201

-- 17. EVALUACIONES DE MUESTRA
INSERT OR IGNORE INTO Evaluaciones (
    IdEvaluacion, IdEstudiante, IdRubrica, FechaEvaluacion, TotalPuntos, Observaciones, 
    Estado, TiempoEvaluacionMinutos, EvaluadoPorId
) VALUES 
(1, 1, 1, datetime('now', '-5 days'), 14.5, 'Excelente trabajo en planificación, buena implementación técnica', 'FINALIZADA', 45, NULL),
(2, 1, 2, datetime('now', '-3 days'), 13.0, 'Muy buena presentación, clara exposición de ideas', 'FINALIZADA', 20, NULL),
(3, 2, 1, datetime('now', '-4 days'), 11.5, 'Buen proyecto, podría mejorar en documentación', 'FINALIZADA', 50, NULL),
(4, 3, 2, datetime('now', '-2 days'), 12.5, 'Excelente uso del lenguaje técnico y organización', 'FINALIZADA', 25, NULL),
(5, 4, 3, datetime('now', '-1 day'), 10.0, 'Análisis profundo, soluciones viables propuestas', 'FINALIZADA', 60, NULL),
(6, 5, 4, datetime('now'), 15.0, 'Evaluación en progreso', 'BORRADOR', NULL, NULL),
(7, 2, 5, datetime('now', '-2 days'), 13.5, 'Excelente lógica de programación y código limpio', 'FINALIZADA', 90, NULL),
(8, 6, 1, datetime('now', '-3 days'), 12.0, 'Buen proyecto técnico, documentación completa', 'FINALIZADA', 40, NULL),
(9, 7, 2, datetime('now', '-1 day'), 14.0, 'Comunicación clara y efectiva', 'FINALIZADA', 30, NULL),
(10, 8, 3, datetime('now'), 9.5, 'Análisis inicial, requiere más desarrollo', 'BORRADOR', NULL, NULL);

-- 18. DETALLES DE EVALUACIÓN
INSERT OR IGNORE INTO DetallesEvaluacion (
    IdDetalle, IdEvaluacion, IdItem, IdNivel, PuntosObtenidos
) VALUES 
-- Evaluación 1 (Ana - Proyectos Técnicos)
(1, 1, 1, 1, 4.0), -- Planificación: Excelente
(2, 1, 2, 2, 3.0), -- Implementación: Bueno  
(3, 1, 3, 1, 4.0), -- Innovación: Excelente
(4, 1, 4, 2, 3.5), -- Documentación: Bueno+

-- Evaluación 2 (Ana - Comunicación)
(5, 2, 5, 1, 4.0), -- Claridad: Excelente
(6, 2, 6, 2, 3.0), -- Lenguaje Técnico: Bueno
(7, 2, 7, 1, 4.0), -- Organización: Excelente
(8, 2, 8, 3, 2.0), -- Interacción: Regular

-- Evaluación 3 (Carlos - Proyectos Técnicos)
(9, 3, 1, 2, 3.0), -- Planificación: Bueno
(10, 3, 2, 2, 3.0), -- Implementación: Bueno
(11, 3, 3, 3, 2.5), -- Innovación: Regular+
(12, 3, 4, 3, 3.0), -- Documentación: Regular+

-- Evaluación 7 (Carlos - Programación)
(13, 7, 17, 1, 4.0), -- Lógica: Excelente
(14, 7, 18, 1, 4.0), -- Calidad: Excelente
(15, 7, 19, 2, 3.0), -- Funcionalidad: Bueno
(16, 7, 20, 2, 2.5); -- Manejo de Errores: Bueno

-- 19. CONFIGURACIONES DEL SISTEMA
INSERT OR IGNORE INTO ConfiguracionesSistema (
    Id, Clave, Valor, Descripcion, FechaCreacion, FechaModificacion, UsuarioModificacion
) VALUES 
(1, 'SISTEMA_NOMBRE', 'Sistema de Rúbricas Académicas', 'Nombre del sistema', datetime('now'), datetime('now'), 'System'),
(2, 'VERSION_SISTEMA', '2.0.0', 'Versión actual del sistema', datetime('now'), datetime('now'), 'System'),
(3, 'EVALUACION_MAXIMA_DURACION', '120', 'Duración máxima de evaluación en minutos', datetime('now'), datetime('now'), 'System'),
(4, 'BACKUP_AUTOMATICO', 'true', 'Activar backup automático diario', datetime('now'), datetime('now'), 'System'),
(5, 'NOTIFICACIONES_EMAIL', 'true', 'Enviar notificaciones por correo', datetime('now'), datetime('now'), 'System'),
(6, 'MODO_REGISTRO', 'CERRADO', 'Modo de registro de usuarios', datetime('now'), datetime('now'), 'System'),
(7, 'CALIFICACION_MINIMA_APROBACION', '2.0', 'Calificación mínima para aprobar', datetime('now'), datetime('now'), 'System'),
(8, 'MOSTRAR_ESTADISTICAS', 'true', 'Mostrar estadísticas en dashboard', datetime('now'), datetime('now'), 'System'),
(9, 'PERMITIR_AUTOEVALUACION', 'true', 'Permitir que estudiantes se autoevalúen', datetime('now'), datetime('now'), 'System'),
(10, 'TIEMPO_SESION_MINUTOS', '60', 'Tiempo de sesión antes de logout automático', datetime('now'), datetime('now'), 'System');

COMMIT;

-- ================================================
-- Script completado exitosamente
-- Base de datos actualizada con nueva estructura
-- Total aproximado de registros insertados: 200+
-- Tablas populadas: 16 tablas principales
-- ================================================

-- Verificación rápida de los datos insertados
SELECT 'Períodos' as Tabla, COUNT(*) as Registros FROM PeriodosAcademicos
UNION ALL
SELECT 'Materias', COUNT(*) FROM Materias  
UNION ALL
SELECT 'Estudiantes', COUNT(*) FROM Estudiantes
UNION ALL
SELECT 'Rúbricas', COUNT(*) FROM Rubricas
UNION ALL
SELECT 'Items Evaluación', COUNT(*) FROM ItemsEvaluacion
UNION ALL
SELECT 'Niveles Calificación', COUNT(*) FROM NivelesCalificacion
UNION ALL
SELECT 'Evaluaciones', COUNT(*) FROM Evaluaciones
UNION ALL
SELECT 'Instrumentos', COUNT(*) FROM InstrumentosEvaluacion
UNION ALL
SELECT 'Materia-Períodos', COUNT(*) FROM MateriaPeriodos
UNION ALL
SELECT 'Cuadernos', COUNT(*) FROM CuadernosCalificadores;