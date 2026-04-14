-- =====================================================================
-- SCRIPT DE DATOS DE PRUEBA PARA CUADERNO CALIFICADOR PQ2025
-- Sistema de Rúbricas - Datos completos para demostración
-- =====================================================================

-- Limpiar datos existentes (opcional - comentar si ya tienes datos)
-- DELETE FROM DetallesEvaluacion;
-- DELETE FROM Evaluaciones;
-- DELETE FROM ValoresRubrica;
-- DELETE FROM InstrumentoRubricas;
-- DELETE FROM InstrumentoMaterias;
-- DELETE FROM RubricaNiveles;
-- DELETE FROM ItemsEvaluacion;
-- DELETE FROM Rubricas;
-- DELETE FROM InstrumentosEvaluacion;
-- DELETE FROM Estudiantes;
-- DELETE FROM Materias;
-- DELETE FROM NivelesCalificacion;
-- DELETE FROM GruposCalificacion;

-- =====================================================================
-- 1. PERÍODOS ACADÉMICOS (ya deberían existir por Program.cs)
-- =====================================================================
INSERT OR IGNORE INTO PeriodosAcademicos (Id, Codigo, Nombre, Tipo, Año, NumeroPeriodo, FechaInicio, FechaFin, Activo, Estado, Descripcion, Creditos, FechaCreacion)
VALUES 
(1, 'PQ2025-1', 'Primer Cuatrimestre', 0, 2025, 1, '2025-01-15', '2025-05-15', 1, 'Activo', 'Primer cuatrimestre del año 2025', 0, datetime('now')),
(2, 'PQ2025-2', 'Segundo Cuatrimestre', 0, 2025, 2, '2025-05-16', '2025-09-15', 1, 'Activo', 'Segundo cuatrimestre del año 2025', 0, datetime('now')),
(3, 'PQ2025-3', 'Tercer Cuatrimestre', 0, 2025, 3, '2025-09-16', '2025-12-15', 1, 'Activo', 'Tercer cuatrimestre del año 2025', 0, datetime('now'));

-- =====================================================================
-- 2. GRUPOS DE CALIFICACIÓN
-- =====================================================================
INSERT OR IGNORE INTO GruposCalificacion (IdGrupo, NombreGrupo, Descripcion, Estado, FechaCreacion)
VALUES 
(1, 'Evaluación Estándar', 'Niveles de calificación estándar para evaluaciones generales', 'ACTIVO', datetime('now')),
(2, 'Evaluación Numérica', 'Niveles de calificación con puntuación numérica', 'ACTIVO', datetime('now')),
(3, 'Evaluación Cualitativa', 'Niveles de calificación descriptivos', 'ACTIVO', datetime('now'));

-- =====================================================================
-- 3. NIVELES DE CALIFICACIÓN
-- =====================================================================
INSERT OR IGNORE INTO NivelesCalificacion (IdNivel, NombreNivel, Descripcion, OrdenNivel, IdGrupo)
VALUES 
-- Grupo Estándar
(1, 'Excelente', 'Desempeño excepcional que supera las expectativas', 1, 1),
(2, 'Bueno', 'Desempeño satisfactorio que cumple las expectativas', 2, 1),
(3, 'Regular', 'Desempeño básico que necesita mejoras', 3, 1),
(4, 'Deficiente', 'Desempeño por debajo de las expectativas', 4, 1),

-- Grupo Numérico
(5, '90-100', 'Excelencia académica', 1, 2),
(6, '80-89', 'Muy bueno', 2, 2),
(7, '70-79', 'Bueno', 3, 2),
(8, '60-69', 'Suficiente', 4, 2),
(9, '0-59', 'Insuficiente', 5, 2);

-- =====================================================================
-- 4. MATERIAS
-- =====================================================================
INSERT OR IGNORE INTO Materias (MateriaId, Codigo, Nombre, Descripcion, Creditos, HorasSemanales, Tipo, Estado, Activa, FechaCreacion)
VALUES 
(1, 'MAT101', 'Matemáticas I', 'Curso fundamental de matemáticas básicas', 4, 6, 'Obligatoria', 'ACTIVO', 1, datetime('now')),
(2, 'ESP101', 'Español I', 'Fundamentos de comunicación escrita y oral', 3, 4, 'Obligatoria', 'ACTIVO', 1, datetime('now')),
(3, 'ING101', 'Inglés I', 'Inglés básico para principiantes', 3, 4, 'Obligatoria', 'ACTIVO', 1, datetime('now')),
(4, 'PROG101', 'Programación I', 'Introducción a la programación', 4, 6, 'Obligatoria', 'ACTIVO', 1, datetime('now')),
(5, 'BIO101', 'Biología General', 'Conceptos fundamentales de biología', 3, 5, 'Obligatoria', 'ACTIVO', 1, datetime('now'));

-- =====================================================================
-- 5. ESTUDIANTES
-- =====================================================================
INSERT OR IGNORE INTO Estudiantes (IdEstudiante, Nombre, Apellidos, NumeroId, DireccionCorreo, Institucion, Grupos, Año, PeriodoAcademicoId)
VALUES 
(1, 'Juan Carlos', 'Pérez Rodríguez', '2025001', 'juan.perez@estudiante.edu', 'Universidad Ejemplo', 'A1', 2025, 1),
(2, 'María José', 'González López', '2025002', 'maria.gonzalez@estudiante.edu', 'Universidad Ejemplo', 'A1', 2025, 1),
(3, 'Carlos Alberto', 'Martínez Silva', '2025003', 'carlos.martinez@estudiante.edu', 'Universidad Ejemplo', 'A1', 2025, 1),
(4, 'Ana Patricia', 'Ramírez Castro', '2025004', 'ana.ramirez@estudiante.edu', 'Universidad Ejemplo', 'A1', 2025, 1),
(5, 'Luis Fernando', 'Torres Mendoza', '2025005', 'luis.torres@estudiante.edu', 'Universidad Ejemplo', 'A1', 2025, 1),
(6, 'Sandra Milena', 'Vargas Ruiz', '2025006', 'sandra.vargas@estudiante.edu', 'Universidad Ejemplo', 'A2', 2025, 1),
(7, 'Diego Alejandro', 'Morales Jiménez', '2025007', 'diego.morales@estudiante.edu', 'Universidad Ejemplo', 'A2', 2025, 1),
(8, 'Claudia Patricia', 'Herrera Vega', '2025008', 'claudia.herrera@estudiante.edu', 'Universidad Ejemplo', 'A2', 2025, 1),
(9, 'Roberto Carlos', 'Delgado Núñez', '2025009', 'roberto.delgado@estudiante.edu', 'Universidad Ejemplo', 'A2', 2025, 1),
(10, 'Paola Andrea', 'Guerrero Sánchez', '2025010', 'paola.guerrero@estudiante.edu', 'Universidad Ejemplo', 'A2', 2025, 1);

-- =====================================================================
-- 6. INSTRUMENTOS DE EVALUACIÓN
-- =====================================================================
INSERT OR IGNORE INTO InstrumentosEvaluacion (InstrumentoId, Nombre, Descripcion, Activo, FechaCreacion)
VALUES 
(1, 'Tarea 1', 'Primera tarea del período académico', 1, datetime('now')),
(2, 'Tarea 2', 'Segunda tarea del período académico', 1, datetime('now')),
(3, 'Proyecto 1', 'Primer proyecto integral del curso', 1, datetime('now')),
(4, 'Examen Parcial', 'Evaluación parcial del período', 1, datetime('now')),
(5, 'Laboratorio 1', 'Primera práctica de laboratorio', 1, datetime('now')),
(6, 'Ensayo', 'Ensayo reflexivo sobre el tema', 1, datetime('now'));

-- =====================================================================
-- 7. RÚBRICAS
-- =====================================================================
INSERT OR IGNORE INTO Rubricas (IdRubrica, NombreRubrica, Descripcion, Estado, FechaCreacion, EsPublica, IdGrupo)
VALUES 
(1, 'Rúbrica Tarea 1', 'Evaluación para la primera tarea', 'ACTIVO', datetime('now'), 1, 1),
(2, 'Rúbrica Tarea 2', 'Evaluación para la segunda tarea', 'ACTIVO', datetime('now'), 1, 1),
(3, 'Rúbrica Proyecto 1', 'Evaluación integral del primer proyecto', 'ACTIVO', datetime('now'), 1, 1),
(4, 'Rúbrica Examen Parcial', 'Evaluación del examen parcial', 'ACTIVO', datetime('now'), 1, 2),
(5, 'Rúbrica Laboratorio', 'Evaluación de prácticas de laboratorio', 'ACTIVO', datetime('now'), 1, 1),
(6, 'Rúbrica Ensayo', 'Evaluación de ensayos escritos', 'ACTIVO', datetime('now'), 1, 1);

-- =====================================================================
-- 8. ITEMS DE EVALUACIÓN
-- =====================================================================
INSERT OR IGNORE INTO ItemsEvaluacion (IdItem, NombreItem, Descripcion, Peso, IdRubrica)
VALUES 
-- Items para Rúbrica Tarea 1
(1, 'Cumplimiento de objetivos', 'Alcanza los objetivos planteados', 25.0, 1),
(2, 'Calidad del contenido', 'Contenido relevante y bien estructurado', 30.0, 1),
(3, 'Presentación', 'Formato y presentación adecuados', 20.0, 1),
(4, 'Entrega puntual', 'Cumple con los tiempos establecidos', 25.0, 1),

-- Items para Rúbrica Tarea 2
(5, 'Análisis crítico', 'Demuestra pensamiento crítico', 40.0, 2),
(6, 'Uso de fuentes', 'Utiliza fuentes confiables y actuales', 30.0, 2),
(7, 'Coherencia', 'Ideas organizadas y coherentes', 30.0, 2),

-- Items para Rúbrica Proyecto 1
(8, 'Planificación', 'Evidencia planificación adecuada', 20.0, 3),
(9, 'Desarrollo', 'Desarrollo completo del proyecto', 40.0, 3),
(10, 'Innovación', 'Aportes creativos e innovadores', 20.0, 3),
(11, 'Presentación final', 'Comunicación efectiva de resultados', 20.0, 3),

-- Items para Examen Parcial
(12, 'Conocimiento teórico', 'Dominio de conceptos teóricos', 50.0, 4),
(13, 'Aplicación práctica', 'Aplicación de conocimientos', 30.0, 4),
(14, 'Claridad en respuestas', 'Respuestas claras y precisas', 20.0, 4),

-- Items para Laboratorio
(15, 'Preparación previa', 'Llega preparado al laboratorio', 25.0, 5),
(16, 'Ejecución', 'Realiza correctamente los procedimientos', 40.0, 5),
(17, 'Reporte', 'Entrega reporte completo y preciso', 35.0, 5),

-- Items para Ensayo
(18, 'Tesis clara', 'Presenta una tesis bien definida', 30.0, 6),
(19, 'Argumentación', 'Argumentos sólidos y bien respaldados', 40.0, 6),
(20, 'Redacción', 'Excelente calidad de redacción', 30.0, 6);

-- =====================================================================
-- 9. VALORES DE RÚBRICA (Puntuaciones por Nivel e Item)
-- =====================================================================
INSERT OR IGNORE INTO ValoresRubrica (IdValor, IdRubrica, IdItem, IdNivel, ValorPuntos)
VALUES 
-- Valores para Rúbrica Tarea 1 - Items 1-4
-- Item 1: Cumplimiento de objetivos
(1, 1, 1, 1, 25.0),  -- Excelente
(2, 1, 1, 2, 20.0),  -- Bueno
(3, 1, 1, 3, 15.0),  -- Regular
(4, 1, 1, 4, 10.0),  -- Deficiente

-- Item 2: Calidad del contenido
(5, 1, 2, 1, 30.0),  -- Excelente
(6, 1, 2, 2, 24.0),  -- Bueno
(7, 1, 2, 3, 18.0),  -- Regular
(8, 1, 2, 4, 12.0),  -- Deficiente

-- Item 3: Presentación
(9, 1, 3, 1, 20.0),  -- Excelente
(10, 1, 3, 2, 16.0), -- Bueno
(11, 1, 3, 3, 12.0), -- Regular
(12, 1, 3, 4, 8.0),  -- Deficiente

-- Item 4: Entrega puntual
(13, 1, 4, 1, 25.0), -- Excelente
(14, 1, 4, 2, 20.0), -- Bueno
(15, 1, 4, 3, 15.0), -- Regular
(16, 1, 4, 4, 0.0),  -- Deficiente

-- Valores para Rúbrica Tarea 2 - Items 5-7
-- Item 5: Análisis crítico
(17, 2, 5, 1, 40.0), -- Excelente
(18, 2, 5, 2, 32.0), -- Bueno
(19, 2, 5, 3, 24.0), -- Regular
(20, 2, 5, 4, 16.0), -- Deficiente

-- Item 6: Uso de fuentes
(21, 2, 6, 1, 30.0), -- Excelente
(22, 2, 6, 2, 24.0), -- Bueno
(23, 2, 6, 3, 18.0), -- Regular
(24, 2, 6, 4, 12.0), -- Deficiente

-- Item 7: Coherencia
(25, 2, 7, 1, 30.0), -- Excelente
(26, 2, 7, 2, 24.0), -- Bueno
(27, 2, 7, 3, 18.0), -- Regular
(28, 2, 7, 4, 12.0), -- Deficiente

-- Valores para Rúbrica Proyecto 1 - Items 8-11
-- Item 8: Planificación
(29, 3, 8, 1, 20.0), -- Excelente
(30, 3, 8, 2, 16.0), -- Bueno
(31, 3, 8, 3, 12.0), -- Regular
(32, 3, 8, 4, 8.0),  -- Deficiente

-- Item 9: Desarrollo
(33, 3, 9, 1, 40.0), -- Excelente
(34, 3, 9, 2, 32.0), -- Bueno
(35, 3, 9, 3, 24.0), -- Regular
(36, 3, 9, 4, 16.0), -- Deficiente

-- Item 10: Innovación
(37, 3, 10, 1, 20.0), -- Excelente
(38, 3, 10, 2, 16.0), -- Bueno
(39, 3, 10, 3, 12.0), -- Regular
(40, 3, 10, 4, 8.0),  -- Deficiente

-- Item 11: Presentación final
(41, 3, 11, 1, 20.0), -- Excelente
(42, 3, 11, 2, 16.0), -- Bueno
(43, 3, 11, 3, 12.0), -- Regular
(44, 3, 11, 4, 8.0);  -- Deficiente

-- =====================================================================
-- 10. RELACIONES INSTRUMENTO-MATERIA
-- =====================================================================
INSERT OR IGNORE INTO InstrumentoMaterias (InstrumentoEvaluacionId, MateriaId, PeriodoAcademicoId, FechaAsignacion, EsObligatorio)
VALUES 
-- Matemáticas I - Primer Cuatrimestre 2025
(1, 1, 1, datetime('now'), 1), -- Tarea 1
(2, 1, 1, datetime('now'), 1), -- Tarea 2
(3, 1, 1, datetime('now'), 1), -- Proyecto 1
(4, 1, 1, datetime('now'), 0), -- Examen Parcial

-- Español I - Primer Cuatrimestre 2025
(6, 2, 1, datetime('now'), 1), -- Ensayo
(1, 2, 1, datetime('now'), 1), -- Tarea 1
(2, 2, 1, datetime('now'), 1), -- Tarea 2

-- Programación I - Primer Cuatrimestre 2025
(3, 4, 1, datetime('now'), 1), -- Proyecto 1
(5, 4, 1, datetime('now'), 1), -- Laboratorio 1
(4, 4, 1, datetime('now'), 1); -- Examen Parcial

-- =====================================================================
-- 11. RELACIONES INSTRUMENTO-RÚBRICA CON PONDERACIONES
-- =====================================================================
INSERT OR IGNORE INTO InstrumentoRubricas (InstrumentoEvaluacionId, RubricaId, FechaAsignacion, EsObligatorio, Ponderacion, OrdenPresentacion)
VALUES 
-- Matemáticas I - Ponderaciones: Tarea1(30%), Tarea2(30%), Proyecto1(40%)
(1, 1, datetime('now'), 1, 30.0, 1), -- Tarea 1 ? Rúbrica Tarea 1 (30%)
(2, 2, datetime('now'), 1, 30.0, 2), -- Tarea 2 ? Rúbrica Tarea 2 (30%)
(3, 3, datetime('now'), 1, 40.0, 3), -- Proyecto 1 ? Rúbrica Proyecto 1 (40%)

-- Español I - Ponderaciones distribuidas
(6, 6, datetime('now'), 1, 40.0, 1), -- Ensayo ? Rúbrica Ensayo (40%)
(1, 1, datetime('now'), 1, 30.0, 2), -- Tarea 1 ? Rúbrica Tarea 1 (30%)
(2, 2, datetime('now'), 1, 30.0, 3), -- Tarea 2 ? Rúbrica Tarea 2 (30%)

-- Programación I
(3, 3, datetime('now'), 1, 50.0, 1), -- Proyecto 1 ? Rúbrica Proyecto 1 (50%)
(5, 5, datetime('now'), 1, 30.0, 2), -- Laboratorio ? Rúbrica Laboratorio (30%)
(4, 4, datetime('now'), 1, 20.0, 3); -- Examen ? Rúbrica Examen (20%)

-- =====================================================================
-- 12. EVALUACIONES REALIZADAS (Estados: BORRADOR, COMPLETADA, FINALIZADA)
-- =====================================================================
INSERT OR IGNORE INTO Evaluaciones (IdEvaluacion, IdEstudiante, IdRubrica, FechaEvaluacion, TotalPuntos, Observaciones, Estado, EvaluadoPorId, FechaFinalizacion)
VALUES 
-- Evaluaciones para Juan Carlos Pérez (IdEstudiante = 1) - MATEMÁTICAS I
-- Caso de ejemplo del README: 100, 80, 90 ? Total = (100×0.30) + (80×0.30) + (90×0.40) = 90
(1, 1, 1, datetime('now', '-20 days'), 100.0, 'Excelente trabajo en la tarea 1', 'FINALIZADA', NULL, datetime('now', '-20 days')),
(2, 1, 2, datetime('now', '-15 days'), 80.0, 'Buen desarrollo, puede mejorar en análisis', 'FINALIZADA', NULL, datetime('now', '-15 days')),
(3, 1, 3, datetime('now', '-10 days'), 90.0, 'Proyecto muy bien ejecutado', 'FINALIZADA', NULL, datetime('now', '-10 days')),

-- Evaluaciones para María José González (IdEstudiante = 2) - MATEMÁTICAS I
(4, 2, 1, datetime('now', '-20 days'), 85.0, 'Muy buen trabajo', 'FINALIZADA', NULL, datetime('now', '-20 days')),
(5, 2, 2, datetime('now', '-15 days'), 75.0, 'Trabajo satisfactorio', 'FINALIZADA', NULL, datetime('now', '-15 days')),
(6, 2, 3, datetime('now', '-10 days'), 88.0, 'Excelente proyecto', 'FINALIZADA', NULL, datetime('now', '-10 days')),

-- Evaluaciones para Carlos Alberto Martínez (IdEstudiante = 3) - MATEMÁTICAS I
(7, 3, 1, datetime('now', '-20 days'), 92.0, 'Trabajo sobresaliente', 'FINALIZADA', NULL, datetime('now', '-20 days')),
(8, 3, 2, datetime('now', '-15 days'), 78.0, 'Buen esfuerzo', 'FINALIZADA', NULL, datetime('now', '-15 days')),
(9, 3, 3, datetime('now', '-10 days'), 95.0, 'Proyecto excepcional', 'FINALIZADA', NULL, datetime('now', '-10 days')),

-- Evaluaciones para Ana Patricia Ramírez (IdEstudiante = 4) - MATEMÁTICAS I
(10, 4, 1, datetime('now', '-20 days'), 88.0, 'Muy buen desempeño', 'FINALIZADA', NULL, datetime('now', '-20 days')),
(11, 4, 2, datetime('now', '-15 days'), 82.0, 'Trabajo consistente', 'FINALIZADA', NULL, datetime('now', '-15 days')),
(12, 4, 3, datetime('now', '-10 days'), 86.0, 'Buen proyecto', 'FINALIZADA', NULL, datetime('now', '-10 days')),

-- Evaluaciones para Luis Fernando Torres (IdEstudiante = 5) - MATEMÁTICAS I  
(13, 5, 1, datetime('now', '-20 days'), 76.0, 'Trabajo aceptable', 'FINALIZADA', NULL, datetime('now', '-20 days')),
(14, 5, 2, datetime('now', '-15 days'), 70.0, 'Cumple requisitos básicos', 'FINALIZADA', NULL, datetime('now', '-15 days')),
-- Luis no tiene proyecto aún (pendiente)

-- Evaluaciones para Sandra Milena Vargas (IdEstudiante = 6) - MATEMÁTICAS I
(15, 6, 1, datetime('now', '-20 days'), 94.0, 'Excelente calidad', 'FINALIZADA', NULL, datetime('now', '-20 days')),
(16, 6, 2, datetime('now', '-15 days'), 89.0, 'Muy buen análisis', 'FINALIZADA', NULL, datetime('now', '-15 days')),
(17, 6, 3, datetime('now', '-10 days'), 92.0, 'Proyecto destacado', 'FINALIZADA', NULL, datetime('now', '-10 days')),

-- Evaluaciones para Diego Alejandro Morales (IdEstudiante = 7) - MATEMÁTICAS I
(18, 7, 1, datetime('now', '-20 days'), 72.0, 'Trabajo satisfactorio', 'FINALIZADA', NULL, datetime('now', '-20 days')),
(19, 7, 2, datetime('now', '-15 days'), 74.0, 'Puede mejorar', 'FINALIZADA', NULL, datetime('now', '-15 days')),
(20, 7, 3, datetime('now', '-10 days'), 79.0, 'Proyecto aceptable', 'FINALIZADA', NULL, datetime('now', '-10 days')),

-- Evaluaciones para Claudia Patricia Herrera (IdEstudiante = 8) - MATEMÁTICAS I
(21, 8, 1, datetime('now', '-20 days'), 96.0, 'Trabajo excepcional', 'FINALIZADA', NULL, datetime('now', '-20 days')),
(22, 8, 2, datetime('now', '-15 days'), 91.0, 'Excelente análisis', 'FINALIZADA', NULL, datetime('now', '-15 days')),
(23, 8, 3, datetime('now', '-10 days'), 97.0, 'Proyecto sobresaliente', 'FINALIZADA', NULL, datetime('now', '-10 days')),

-- Roberto Carlos Delgado (IdEstudiante = 9) - Solo tiene algunas evaluaciones
(24, 9, 1, datetime('now', '-20 days'), 68.0, 'Trabajo básico', 'FINALIZADA', NULL, datetime('now', '-20 days')),
(25, 9, 2, datetime('now', '-15 days'), 65.0, 'Necesita mejorar', 'FINALIZADA', NULL, datetime('now', '-15 days')),
-- Roberto no tiene proyecto (pendiente)

-- Paola Andrea Guerrero (IdEstudiante = 10) - Evaluaciones completas
(26, 10, 1, datetime('now', '-20 days'), 87.0, 'Muy buen trabajo', 'FINALIZADA', NULL, datetime('now', '-20 days')),
(27, 10, 2, datetime('now', '-15 days'), 84.0, 'Trabajo consistente', 'FINALIZADA', NULL, datetime('now', '-15 days')),
(28, 10, 3, datetime('now', '-10 days'), 89.0, 'Excelente proyecto', 'FINALIZADA', NULL, datetime('now', '-10 days'));

-- =====================================================================
-- 13. DETALLES DE EVALUACIÓN (Calificaciones por Item)
-- =====================================================================
INSERT OR IGNORE INTO DetallesEvaluacion (IdDetalle, IdEvaluacion, IdItem, IdNivel, PuntosObtenidos)
VALUES 
-- Detalles para Juan Carlos Pérez - Tarea 1 (Evaluación 1) - Total: 100 puntos
(1, 1, 1, 1, 25.0),  -- Cumplimiento: Excelente
(2, 1, 2, 1, 30.0),  -- Calidad: Excelente
(3, 1, 3, 1, 20.0),  -- Presentación: Excelente
(4, 1, 4, 1, 25.0),  -- Puntualidad: Excelente

-- Detalles para Juan Carlos Pérez - Tarea 2 (Evaluación 2) - Total: 80 puntos
(5, 2, 5, 2, 32.0),  -- Análisis crítico: Bueno
(6, 2, 6, 1, 30.0),  -- Uso de fuentes: Excelente
(7, 2, 7, 3, 18.0),  -- Coherencia: Regular

-- Detalles para Juan Carlos Pérez - Proyecto 1 (Evaluación 3) - Total: 90 puntos
(8, 3, 8, 2, 16.0),  -- Planificación: Bueno
(9, 3, 9, 1, 40.0),  -- Desarrollo: Excelente
(10, 3, 10, 2, 16.0), -- Innovación: Bueno
(11, 3, 11, 3, 18.0), -- Presentación final: Bueno (ajustado a 18 para total 90)

-- Detalles para María José González - Tarea 1 (Evaluación 4) - Total: 85 puntos
(12, 4, 1, 1, 25.0), -- Cumplimiento: Excelente
(13, 4, 2, 2, 24.0), -- Calidad: Bueno
(14, 4, 3, 2, 16.0), -- Presentación: Bueno
(15, 4, 4, 2, 20.0), -- Puntualidad: Bueno

-- Detalles para María José González - Tarea 2 (Evaluación 5) - Total: 75 puntos
(16, 5, 5, 3, 24.0), -- Análisis crítico: Regular
(17, 5, 6, 2, 24.0), -- Uso de fuentes: Bueno
(18, 5, 7, 2, 27.0), -- Coherencia: Bueno (ajustado para total 75)

-- Detalles para María José González - Proyecto 1 (Evaluación 6) - Total: 88 puntos
(19, 6, 8, 1, 20.0), -- Planificación: Excelente
(20, 6, 9, 2, 32.0), -- Desarrollo: Bueno
(21, 6, 10, 1, 20.0), -- Innovación: Excelente
(22, 6, 11, 2, 16.0); -- Presentación final: Bueno

-- =====================================================================
-- 14. VERIFICACIÓN DE DATOS
-- =====================================================================

-- Consulta de verificación - Total de registros creados
SELECT 'Períodos Académicos' as Tabla, COUNT(*) as Total FROM PeriodosAcademicos
UNION ALL
SELECT 'Grupos Calificación', COUNT(*) FROM GruposCalificacion
UNION ALL
SELECT 'Niveles Calificación', COUNT(*) FROM NivelesCalificacion
UNION ALL
SELECT 'Materias', COUNT(*) FROM Materias
UNION ALL
SELECT 'Estudiantes', COUNT(*) FROM Estudiantes
UNION ALL
SELECT 'Instrumentos Evaluación', COUNT(*) FROM InstrumentosEvaluacion
UNION ALL
SELECT 'Rúbricas', COUNT(*) FROM Rubricas
UNION ALL
SELECT 'Items Evaluación', COUNT(*) FROM ItemsEvaluacion
UNION ALL
SELECT 'Valores Rúbrica', COUNT(*) FROM ValoresRubrica
UNION ALL
SELECT 'Instrumento-Materias', COUNT(*) FROM InstrumentoMaterias
UNION ALL
SELECT 'Instrumento-Rúbricas', COUNT(*) FROM InstrumentoRubricas
UNION ALL
SELECT 'Evaluaciones', COUNT(*) FROM Evaluaciones
UNION ALL
SELECT 'Detalles Evaluación', COUNT(*) FROM DetallesEvaluacion;

-- =====================================================================
-- 15. CONSULTA DE PRUEBA PARA CUADERNO CALIFICADOR
-- =====================================================================

-- Verificar datos para Matemáticas I en Primer Cuatrimestre 2025
SELECT 
    e.Nombre + ' ' + e.Apellidos as Estudiante,
    ie.Nombre as Instrumento,
    r.NombreRubrica as Rubrica,
    ev.TotalPuntos as Calificacion,
    ir.Ponderacion as Ponderacion
FROM Estudiantes e
LEFT JOIN Evaluaciones ev ON e.IdEstudiante = ev.IdEstudiante
LEFT JOIN Rubricas r ON ev.IdRubrica = r.IdRubrica
LEFT JOIN InstrumentoRubricas ir ON r.IdRubrica = ir.RubricaId
LEFT JOIN InstrumentosEvaluacion ie ON ir.InstrumentoEvaluacionId = ie.InstrumentoId
LEFT JOIN InstrumentoMaterias im ON ie.InstrumentoId = im.InstrumentoEvaluacionId
WHERE im.MateriaId = 1 -- Matemáticas I
  AND im.PeriodoAcademicoId = 1 -- Primer Cuatrimestre 2025
ORDER BY e.Apellidos, e.Nombre, ir.OrdenPresentacion;

-- =====================================================================
-- INSTRUCCIONES DE USO
-- =====================================================================
/*
1. Ejecutar este script en la base de datos SQLite
2. Verificar los datos con las consultas de verificación
3. Probar el Cuaderno Calificador con:
   - Materia: Matemáticas I (ID: 1)
   - Período: Primer Cuatrimestre 2025 (ID: 1)
   
4. Casos de prueba esperados:
   - Juan Carlos Pérez: (100×0.30) + (80×0.30) + (90×0.40) = 90.00
   - María José González: (85×0.30) + (75×0.30) + (88×0.40) = 83.20
   - Algunos estudiantes con evaluaciones pendientes para probar valores por defecto
   
5. El sistema debería generar 3 columnas dinámicas:
   - Tarea 1 ? Rúbrica Tarea 1 (30%)
   - Tarea 2 ? Rúbrica Tarea 2 (30%) 
   - Proyecto 1 ? Rúbrica Proyecto 1 (40%)
*/