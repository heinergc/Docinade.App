-- Script de Datos Iniciales para el Sistema de Rúbricas
-- Fecha: 16 de agosto de 2025
-- Propósito: Insertar datos de prueba para testing completo del sistema

-- ======================================
-- 1. PERÍODOS ACADÉMICOS
-- ======================================
INSERT OR IGNORE INTO PeriodosAcademicos (
    Año, Anio, Ciclo, FechaInicio, FechaFin, Activo, 
    Codigo, Nombre, Tipo, NumeroPeriodo, 
    FechaCreacion, FechaModificacion, Estado
) VALUES 
-- Período Académico 2025-I
(2025, 2025, 'I', '2025-02-01', '2025-06-30', 1, 
 '25-I', 'Primer Ciclo 2025', 1, 1, 
 datetime('now'), datetime('now'), 'Activo'),

-- Período Académico 2025-II
(2025, 2025, 'II', '2025-08-01', '2025-12-15', 1, 
 '25-II', 'Segundo Ciclo 2025', 1, 2, 
 datetime('now'), datetime('now'), 'Activo'),

-- Período Académico 2026-I (futuro)
(2026, 2026, 'I', '2026-02-01', '2026-06-30', 0, 
 '26-I', 'Primer Ciclo 2026', 1, 1, 
 datetime('now'), datetime('now'), 'Inactivo');

-- ======================================
-- 2. NIVELES DE CALIFICACIÓN
-- ======================================
INSERT OR IGNORE INTO NivelesCalificacion (
    NombreNivel, Descripcion, OrdenNivel, IdGrupo
) VALUES 
('Excelente', 'Desempeño excepcional que supera todas las expectativas', 1, 1),
('Muy Bueno', 'Desempeño sobresaliente que supera las expectativas', 2, 1),
('Bueno', 'Desempeño satisfactorio que cumple las expectativas', 3, 1),
('Regular', 'Desempeño básico que necesita algunas mejoras', 4, 1),
('Deficiente', 'Desempeño por debajo de las expectativas mínimas', 5, 1),
('Insuficiente', 'No cumple con los requisitos mínimos', 6, 1);

-- ======================================
-- 3. GRUPOS DE CALIFICACIÓN
-- ======================================
INSERT OR IGNORE INTO GruposCalificacion (
    IdGrupo, NombreGrupo, Descripcion, EsActivo, FechaCreacion
) VALUES 
(1, 'Estándar', 'Grupo de calificación estándar para evaluaciones generales', 1, datetime('now')),
(2, 'Simplificado', 'Grupo simplificado con menos niveles', 1, datetime('now')),
(3, 'Detallado', 'Grupo con niveles más específicos y detallados', 1, datetime('now'));

-- ======================================
-- 4. RÚBRICAS
-- ======================================
INSERT OR IGNORE INTO Rubricas (
    NombreRubrica, Descripcion, IdGrupoCalificacion, Estado, FechaCreacion
) VALUES 
('Rúbrica de Evaluación de Ensayos', 
 'Rúbrica para evaluar la calidad de ensayos académicos considerando estructura, contenido y redacción', 
 1, 'ACTIVO', datetime('now')),

('Rúbrica de Presentaciones Orales', 
 'Evaluación integral de presentaciones considerando contenido, delivery y uso de recursos', 
 1, 'ACTIVO', datetime('now')),

('Rúbrica de Proyectos de Investigación', 
 'Evaluación de proyectos de investigación académica desde planteamiento hasta conclusiones', 
 1, 'ACTIVO', datetime('now')),

('Rúbrica de Trabajo en Equipo', 
 'Evaluación de habilidades colaborativas y contribución individual en proyectos grupales', 
 1, 'ACTIVO', datetime('now'));

-- ======================================
-- 5. INSTRUMENTOS DE EVALUACIÓN
-- ======================================
INSERT OR IGNORE INTO InstrumentoEvaluacion (
    Nombre, Descripcion, Activo, FechaCreacion
) VALUES 
('Examen Parcial I', 'Primera evaluación parcial del curso', 1, datetime('now')),
('Examen Parcial II', 'Segunda evaluación parcial del curso', 1, datetime('now')),
('Examen Final', 'Evaluación final comprehensiva del curso', 1, datetime('now')),
('Proyecto Final', 'Proyecto integral de fin de curso', 1, datetime('now')),
('Ensayo Académico', 'Ensayo sobre temas específicos del curso', 1, datetime('now')),
('Presentación Oral', 'Exposición de temas asignados', 1, datetime('now')),
('Laboratorio Práctico', 'Evaluación de actividades de laboratorio', 1, datetime('now')),
('Tarea de Investigación', 'Investigación dirigida sobre temas específicos', 1, datetime('now'));

-- ======================================
-- 6. ITEMS DE EVALUACIÓN (dentro de las rúbricas)
-- ======================================
INSERT OR IGNORE INTO ItemsEvaluacion (
    IdRubrica, NombreItem, Descripcion, OrdenItem, Peso
) VALUES 
-- Items para Rúbrica de Ensayos (IdRubrica = 1)
(1, 'Estructura del ensayo', 'Organización lógica, introducción, desarrollo y conclusión', 1, 25.00),
(1, 'Calidad del contenido', 'Profundidad de análisis, uso de fuentes y argumentación', 2, 35.00),
(1, 'Redacción y estilo', 'Claridad, coherencia y corrección gramatical', 3, 25.00),
(1, 'Formato y presentación', 'Cumplimiento de normas de citación y formato académico', 4, 15.00),

-- Items para Rúbrica de Presentaciones Orales (IdRubrica = 2)
(2, 'Contenido y conocimiento', 'Dominio del tema y calidad de la información presentada', 1, 40.00),
(2, 'Comunicación verbal', 'Claridad, volumen y ritmo de la presentación', 2, 25.00),
(2, 'Comunicación no verbal', 'Lenguaje corporal, contacto visual y presencia escénica', 3, 20.00),
(2, 'Recursos audiovisuales', 'Efectividad y calidad de materiales de apoyo', 4, 15.00),

-- Items para Rúbrica de Proyectos de Investigación (IdRubrica = 3)
(3, 'Planteamiento del problema', 'Claridad en la formulación del problema de investigación', 1, 20.00),
(3, 'Marco teórico', 'Revisión bibliográfica y fundamentación teórica', 2, 25.00),
(3, 'Metodología', 'Diseño metodológico apropiado para el tipo de investigación', 3, 25.00),
(3, 'Análisis de resultados', 'Interpretación y discusión de hallazgos', 4, 20.00),
(3, 'Conclusiones', 'Síntesis y recomendaciones basadas en los resultados', 5, 10.00),

-- Items para Rúbrica de Trabajo en Equipo (IdRubrica = 4)
(4, 'Participación activa', 'Contribución consistente y proactiva en las actividades del equipo', 1, 30.00),
(4, 'Comunicación efectiva', 'Capacidad de expresar ideas y escuchar a los demás', 2, 25.00),
(4, 'Responsabilidad compartida', 'Cumplimiento de compromisos y distribución equitativa del trabajo', 3, 25.00),
(4, 'Resolución de conflictos', 'Habilidad para manejar diferencias y encontrar soluciones', 4, 20.00);

-- ======================================
-- 7. VALORES DE RÚBRICA (matriz de evaluación)
-- ======================================
-- Valores para Items de Rúbrica de Ensayos
INSERT OR IGNORE INTO ValoresRubrica (
    IdRubrica, IdItem, IdNivel, ValorPuntos, DescripcionNivel
) VALUES 
-- Estructura del ensayo (IdItem = 1)
(1, 1, 1, 100, 'Estructura excepcional con transiciones fluidas y organización impecable'),
(1, 1, 2, 85, 'Estructura muy buena con organización clara y transiciones apropiadas'),
(1, 1, 3, 75, 'Estructura buena con organización básica apropiada'),
(1, 1, 4, 60, 'Estructura regular con algunos problemas de organización'),
(1, 1, 5, 40, 'Estructura deficiente con problemas significativos de organización'),
(1, 1, 6, 20, 'Estructura insuficiente o ausente'),

-- Calidad del contenido (IdItem = 2)
(1, 2, 1, 100, 'Análisis excepcional con fuentes diversas y argumentación sólida'),
(1, 2, 2, 85, 'Análisis muy bueno con buenas fuentes y argumentación clara'),
(1, 2, 3, 75, 'Análisis bueno con fuentes apropiadas y argumentación básica'),
(1, 2, 4, 60, 'Análisis regular con pocas fuentes y argumentación débil'),
(1, 2, 5, 40, 'Análisis deficiente con fuentes inadecuadas'),
(1, 2, 6, 20, 'Análisis insuficiente o ausente');

-- ======================================
-- 8. ASIGNACIONES INSTRUMENTO-RÚBRICA
-- ======================================
INSERT OR IGNORE INTO InstrumentoRubricas (
    InstrumentoId, RubricaId, Ponderacion
) VALUES 
-- Ensayo Académico usa Rúbrica de Ensayos al 100%
(5, 1, 100.00),

-- Presentación Oral usa Rúbrica de Presentaciones al 100%
(6, 2, 100.00),

-- Proyecto Final usa múltiples rúbricas
(4, 3, 70.00),  -- 70% Investigación
(4, 2, 20.00),  -- 20% Presentación
(4, 4, 10.00),  -- 10% Trabajo en equipo

-- Examen Final usa Rúbrica de Ensayos (para preguntas abiertas)
(3, 1, 100.00);

-- ======================================
-- 9. ESTUDIANTES
-- ======================================
INSERT OR IGNORE INTO Estudiantes (
    Nombre, Apellidos, NumeroId, DireccionCorreo, Institucion, 
    Grupos, Año, PeriodoAcademicoId
) VALUES 
-- Estudiantes para el período 2025-I
('Ana María', 'García López', '2021001', 'ana.garcia@universidad.edu', 'Universidad Nacional', 'Grupo A', 2025, 1),
('Carlos Eduardo', 'Rodríguez Méndez', '2021002', 'carlos.rodriguez@universidad.edu', 'Universidad Nacional', 'Grupo A', 2025, 1),
('María Elena', 'Fernández Castro', '2021003', 'maria.fernandez@universidad.edu', 'Universidad Nacional', 'Grupo B', 2025, 1),
('José Manuel', 'Torres Vargas', '2021004', 'jose.torres@universidad.edu', 'Universidad Nacional', 'Grupo B', 2025, 1),
('Laura Patricia', 'Morales Jiménez', '2021005', 'laura.morales@universidad.edu', 'Universidad Nacional', 'Grupo A', 2025, 1),

-- Estudiantes para el período 2025-II
('Diego Alejandro', 'Ramírez Solano', '2021006', 'diego.ramirez@universidad.edu', 'Universidad Nacional', 'Grupo C', 2025, 2),
('Sandra Milena', 'Herrera Quesada', '2021007', 'sandra.herrera@universidad.edu', 'Universidad Nacional', 'Grupo C', 2025, 2),
('Fernando José', 'Castillo Montero', '2021008', 'fernando.castillo@universidad.edu', 'Universidad Nacional', 'Grupo D', 2025, 2),
('Gabriela', 'Chaves Badilla', '2021009', 'gabriela.chaves@universidad.edu', 'Universidad Nacional', 'Grupo C', 2025, 2),
('Ricardo Antonio', 'Vega Villalobos', '2021010', 'ricardo.vega@universidad.edu', 'Universidad Nacional', 'Grupo D', 2025, 2);

-- ======================================
-- 10. EVALUACIONES DE EJEMPLO
-- ======================================
INSERT OR IGNORE INTO Evaluaciones (
    IdEstudiante, IdRubrica, FechaEvaluacion, PuntuacionTotal, 
    Observaciones, IdEvaluador
) VALUES 
-- Evaluaciones para Ana María García
(1, 1, '2025-03-15', 85.50, 'Excelente estructura y buena argumentación. Mejorar el uso de fuentes.', 'admin@rubricas.edu'),
(1, 2, '2025-04-20', 92.00, 'Presentación muy clara y bien organizada. Excelente manejo del tema.', 'admin@rubricas.edu'),

-- Evaluaciones para Carlos Eduardo Rodríguez
(2, 1, '2025-03-15', 78.25, 'Buena estructura pero necesita mejorar la profundidad del análisis.', 'admin@rubricas.edu'),
(2, 4, '2025-05-10', 88.75, 'Excelente colaborador, contribuye activamente al equipo.', 'admin@rubricas.edu'),

-- Evaluaciones para María Elena Fernández
(3, 3, '2025-05-25', 89.50, 'Investigación sólida con metodología apropiada. Buenas conclusiones.', 'admin@rubricas.edu');

-- ======================================
-- 11. DETALLES DE EVALUACIÓN
-- ======================================
INSERT OR IGNORE INTO DetallesEvaluacion (
    IdEvaluacion, IdItem, IdNivel, Puntos, Comentarios
) VALUES 
-- Detalles para evaluación de Ana María (IdEvaluacion = 1)
(1, 1, 2, 85, 'Muy buena estructura con transiciones claras'),
(1, 2, 3, 75, 'Buen contenido pero podría usar más fuentes académicas'),
(1, 3, 2, 85, 'Redacción clara y fluida'),
(1, 4, 1, 100, 'Formato impecable siguiendo normas APA'),

-- Detalles para evaluación de Carlos Eduardo (IdEvaluacion = 2)
(2, 1, 3, 75, 'Estructura básica apropiada'),
(2, 2, 4, 60, 'Análisis superficial, necesita mayor profundidad'),
(2, 3, 3, 75, 'Redacción aceptable con algunos errores menores'),
(2, 4, 2, 85, 'Buen formato general');

-- ======================================
-- 12. CONFIGURACIONES DEL SISTEMA
-- ======================================
INSERT OR IGNORE INTO ConfiguracionesSistema (
    Clave, Valor, Descripcion, Categoria, FechaCreacion
) VALUES 
('sistema.nombre', 'Sistema de Rúbricas Académicas', 'Nombre del sistema', 'General', datetime('now')),
('sistema.version', '1.0.0', 'Versión actual del sistema', 'General', datetime('now')),
('evaluacion.escala_maxima', '100', 'Puntuación máxima en evaluaciones', 'Evaluación', datetime('now')),
('evaluacion.escala_minima', '0', 'Puntuación mínima en evaluaciones', 'Evaluación', datetime('now')),
('notificaciones.activas', 'true', 'Activar notificaciones del sistema', 'Notificaciones', datetime('now')),
('backup.frecuencia_dias', '7', 'Frecuencia de respaldos automáticos en días', 'Mantenimiento', datetime('now'));

-- ======================================
-- VERIFICACIÓN DE DATOS INSERTADOS
-- ======================================
-- Mostrar resumen de datos insertados
SELECT 'Períodos Académicos' as Tabla, COUNT(*) as Total FROM PeriodosAcademicos
UNION ALL
SELECT 'Niveles de Calificación', COUNT(*) FROM NivelesCalificacion  
UNION ALL
SELECT 'Rúbricas', COUNT(*) FROM Rubricas
UNION ALL
SELECT 'Instrumentos de Evaluación', COUNT(*) FROM InstrumentoEvaluacion
UNION ALL
SELECT 'Items de Evaluación', COUNT(*) FROM ItemsEvaluacion
UNION ALL
SELECT 'Asignaciones Instrumento-Rúbrica', COUNT(*) FROM InstrumentoRubricas
UNION ALL
SELECT 'Estudiantes', COUNT(*) FROM Estudiantes
UNION ALL
SELECT 'Evaluaciones', COUNT(*) FROM Evaluaciones
UNION ALL
SELECT 'Detalles de Evaluación', COUNT(*) FROM DetallesEvaluacion;

-- ======================================
-- COMENTARIOS Y NOTAS
-- ======================================
/*
DATOS CREADOS:
- 3 Períodos académicos (2025-I activo, 2025-II activo, 2026-I inactivo)
- 6 Niveles de calificación estándar
- 3 Grupos de calificación
- 4 Rúbricas completas con diferentes propósitos
- 8 Instrumentos de evaluación variados
- 17 Items de evaluación distribuidos en las rúbricas
- Valores de rúbrica para demostrar la matriz de evaluación
- 6 Asignaciones instrumento-rúbrica con diferentes ponderaciones
- 10 Estudiantes distribuidos en dos períodos académicos
- 3 Evaluaciones de ejemplo con diferentes rúbricas
- 8 Detalles de evaluación mostrando puntuaciones específicas
- 6 Configuraciones básicas del sistema

CASOS DE PRUEBA HABILITADOS:
1. Crear y gestionar períodos académicos
2. Asignar estudiantes a períodos
3. Crear rúbricas con múltiples items
4. Definir instrumentos de evaluación
5. Asignar rúbricas a instrumentos con ponderaciones
6. Realizar evaluaciones completas
7. Consultar reportes y estadísticas
8. Probar todas las funcionalidades CRUD

USUARIOS DE PRUEBA RECOMENDADOS:
- admin@rubricas.edu / Admin123! (Super Administrador)
- docente@rubricas.edu / Docente123! (Docente)
- evaluador@rubricas.edu / Evaluador123! (Evaluador)
*/