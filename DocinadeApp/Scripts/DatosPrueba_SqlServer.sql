-- =====================================================================
-- SCRIPT PARA SQL SERVER - DATOS DE PRUEBA CUADERNO CALIFICADOR
-- Migración desde SQLite a SQL Server Express
-- =====================================================================

-- =====================================================================
-- 1. VERIFICAR CONEXIÓN Y BASE DE DATOS
-- =====================================================================
USE [RubricasDb]
GO

PRINT '?? Iniciando configuración de datos de prueba en SQL Server Express'
PRINT '=================================================================='

-- =====================================================================
-- 2. LIMPIAR DATOS EXISTENTES (OPCIONAL)
-- =====================================================================
-- Descomentar esta sección si quieres limpiar datos existentes
/*
DELETE FROM [DetallesEvaluacion];
DELETE FROM [Evaluaciones];
DELETE FROM [ValoresRubrica];
DELETE FROM [InstrumentoRubricas];
DELETE FROM [InstrumentoMaterias];
DELETE FROM [RubricaNiveles];
DELETE FROM [ItemsEvaluacion];
DELETE FROM [Rubricas];
DELETE FROM [InstrumentosEvaluacion];
DELETE FROM [Estudiantes];
DELETE FROM [Materias];
DELETE FROM [NivelesCalificacion];
DELETE FROM [GruposCalificacion];
*/

-- =====================================================================
-- 3. VERIFICAR PER?ODOS ACAD?MICOS EXISTENTES
-- =====================================================================
-- Si no existen, crear el per?odo objetivo
IF NOT EXISTS (SELECT 1 FROM [PeriodosAcademicos] WHERE [Id] = 1)
BEGIN
    SET IDENTITY_INSERT [PeriodosAcademicos] ON;
    INSERT INTO [PeriodosAcademicos] ([Id], [Codigo], [Nombre], [Tipo], [Año], [NumeroPeriodo], [FechaInicio], [FechaFin], [Activo], [Estado], [Descripcion], [Creditos], [FechaCreacion])
    VALUES (1, 'PQ2025-1', 'Primer Cuatrimestre', 0, 2025, 1, '2025-01-15', '2025-05-15', 1, 'Activo', 'Primer cuatrimestre del año 2025', 0, GETDATE());
    SET IDENTITY_INSERT [PeriodosAcademicos] OFF;
END

-- =====================================================================
-- 4. GRUPOS Y NIVELES DE CALIFICACI?N
-- =====================================================================
IF NOT EXISTS (SELECT 1 FROM [GruposCalificacion] WHERE [IdGrupo] = 1)
BEGIN
    SET IDENTITY_INSERT [GruposCalificacion] ON;
    INSERT INTO [GruposCalificacion] ([IdGrupo], [NombreGrupo], [Descripcion], [Estado], [FechaCreacion])
    VALUES (1, 'Evaluación Estándar', 'Niveles de calificación estándar para evaluaciones generales', 'ACTIVO', GETDATE());
    SET IDENTITY_INSERT [GruposCalificacion] OFF;
END

-- Insertar niveles si no existen
IF NOT EXISTS (SELECT 1 FROM [NivelesCalificacion] WHERE [IdNivel] = 1)
BEGIN
    SET IDENTITY_INSERT [NivelesCalificacion] ON;
    INSERT INTO [NivelesCalificacion] ([IdNivel], [NombreNivel], [Descripcion], [OrdenNivel], [IdGrupo])
    VALUES 
    (1, 'Excelente', 'Desempeño excepcional que supera las expectativas', 1, 1),
    (2, 'Bueno', 'Desempeño satisfactorio que cumple las expectativas', 2, 1),
    (3, 'Regular', 'Desempeño básico que necesita mejoras', 3, 1),
    (4, 'Deficiente', 'Desempeño por debajo de las expectativas', 4, 1);
    SET IDENTITY_INSERT [NivelesCalificacion] OFF;
END

-- =====================================================================
-- 5. MATERIA DE EJEMPLO
-- =====================================================================
IF NOT EXISTS (SELECT 1 FROM [Materias] WHERE [MateriaId] = 1)
BEGIN
    SET IDENTITY_INSERT [Materias] ON;
    INSERT INTO [Materias] ([MateriaId], [Codigo], [Nombre], [Descripcion], [Creditos], [HorasSemanales], [Tipo], [Estado], [Activa], [FechaCreacion])
    VALUES (1, 'MAT101', 'Matemáticas I', 'Curso fundamental de matemáticas básicas', 4, 6, 'Obligatoria', 'ACTIVO', 1, GETDATE());
    SET IDENTITY_INSERT [Materias] OFF;
END

-- =====================================================================
-- 6. ESTUDIANTES DE PRUEBA
-- =====================================================================
IF NOT EXISTS (SELECT 1 FROM [Estudiantes] WHERE [IdEstudiante] = 1)
BEGIN
    SET IDENTITY_INSERT [Estudiantes] ON;
    INSERT INTO [Estudiantes] ([IdEstudiante], [Nombre], [Apellidos], [NumeroId], [DireccionCorreo], [Institucion], [Grupos], [Año], [PeriodoAcademicoId])
    VALUES 
    (1, 'Juan Carlos', 'Pérez Rodríguez', '2025001', 'juan.perez@estudiante.edu', 'Universidad Ejemplo', 'A1', 2025, 1),
    (2, 'María José', 'González López', '2025002', 'maria.gonzalez@estudiante.edu', 'Universidad Ejemplo', 'A1', 2025, 1),
    (3, 'Carlos Alberto', 'Martínez Silva', '2025003', 'carlos.martinez@estudiante.edu', 'Universidad Ejemplo', 'A1', 2025, 1),
    (4, 'Ana Patricia', 'Ramírez Castro', '2025004', 'ana.ramirez@estudiante.edu', 'Universidad Ejemplo', 'A1', 2025, 1),
    (5, 'Luis Fernando', 'Torres Mendoza', '2025005', 'luis.torres@estudiante.edu', 'Universidad Ejemplo', 'A1', 2025, 1);
    SET IDENTITY_INSERT [Estudiantes] OFF;
END

-- =====================================================================
-- 7. INSTRUMENTOS DE EVALUACI?N
-- =====================================================================
IF NOT EXISTS (SELECT 1 FROM [InstrumentosEvaluacion] WHERE [InstrumentoId] = 1)
BEGIN
    SET IDENTITY_INSERT [InstrumentosEvaluacion] ON;
    INSERT INTO [InstrumentosEvaluacion] ([InstrumentoId], [Nombre], [Descripcion], [Activo], [FechaCreacion])
    VALUES 
    (1, 'Tarea 1', 'Primera tarea del período académico', 1, GETDATE()),
    (2, 'Tarea 2', 'Segunda tarea del período académico', 1, GETDATE()),
    (3, 'Proyecto 1', 'Primer proyecto integral del curso', 1, GETDATE());
    SET IDENTITY_INSERT [InstrumentosEvaluacion] OFF;
END

-- =====================================================================
-- 8. R?BRICAS
-- =====================================================================
IF NOT EXISTS (SELECT 1 FROM [Rubricas] WHERE [IdRubrica] = 1)
BEGIN
    SET IDENTITY_INSERT [Rubricas] ON;
    INSERT INTO [Rubricas] ([IdRubrica], [NombreRubrica], [Descripcion], [Estado], [FechaCreacion], [EsPublica], [IdGrupo])
    VALUES 
    (1, 'Rúbrica Tarea 1', 'Evaluación para la primera tarea', 'ACTIVO', GETDATE(), 1, 1),
    (2, 'Rúbrica Tarea 2', 'Evaluación para la segunda tarea', 'ACTIVO', GETDATE(), 1, 1),
    (3, 'Rúbrica Proyecto 1', 'Evaluación integral del primer proyecto', 'ACTIVO', GETDATE(), 1, 1);
    SET IDENTITY_INSERT [Rubricas] OFF;
END

-- =====================================================================
-- 9. ITEMS DE EVALUACI?N
-- =====================================================================
IF NOT EXISTS (SELECT 1 FROM [ItemsEvaluacion] WHERE [IdItem] = 1)
BEGIN
    SET IDENTITY_INSERT [ItemsEvaluacion] ON;
    INSERT INTO [ItemsEvaluacion] ([IdItem], [NombreItem], [Descripcion], [Peso], [IdRubrica])
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
    (11, 'Presentación final', 'Comunicación efectiva de resultados', 20.0, 3);
    SET IDENTITY_INSERT [ItemsEvaluacion] OFF;
END

-- =====================================================================
-- 10. VALORES DE R?BRICA
-- =====================================================================
IF NOT EXISTS (SELECT 1 FROM [ValoresRubrica] WHERE [IdValor] = 1)
BEGIN
    SET IDENTITY_INSERT [ValoresRubrica] ON;

    -- Valores para Rúbrica Tarea 1
    INSERT INTO [ValoresRubrica] ([IdValor], [IdRubrica], [IdItem], [IdNivel], [ValorPuntos]) VALUES (1, 1, 1, 1, 25.0);
    INSERT INTO [ValoresRubrica] ([IdValor], [IdRubrica], [IdItem], [IdNivel], [ValorPuntos]) VALUES (2, 1, 1, 2, 20.0);
    INSERT INTO [ValoresRubrica] ([IdValor], [IdRubrica], [IdItem], [IdNivel], [ValorPuntos]) VALUES (3, 1, 1, 3, 15.0);
    INSERT INTO [ValoresRubrica] ([IdValor], [IdRubrica], [IdItem], [IdNivel], [ValorPuntos]) VALUES (4, 1, 1, 4, 10.0);

    INSERT INTO [ValoresRubrica] ([IdValor], [IdRubrica], [IdItem], [IdNivel], [ValorPuntos]) VALUES (5, 1, 2, 1, 30.0);
    INSERT INTO [ValoresRubrica] ([IdValor], [IdRubrica], [IdItem], [IdNivel], [ValorPuntos]) VALUES (6, 1, 2, 2, 24.0);
    INSERT INTO [ValoresRubrica] ([IdValor], [IdRubrica], [IdItem], [IdNivel], [ValorPuntos]) VALUES (7, 1, 2, 3, 18.0);
    INSERT INTO [ValoresRubrica] ([IdValor], [IdRubrica], [IdItem], [IdNivel], [ValorPuntos]) VALUES (8, 1, 2, 4, 12.0);

    INSERT INTO [ValoresRubrica] ([IdValor], [IdRubrica], [IdItem], [IdNivel], [ValorPuntos]) VALUES (9, 1, 3, 1, 20.0);
    INSERT INTO [ValoresRubrica] ([IdValor], [IdRubrica], [IdItem], [IdNivel], [ValorPuntos]) VALUES (10, 1, 3, 2, 16.0);
    INSERT INTO [ValoresRubrica] ([IdValor], [IdRubrica], [IdItem], [IdNivel], [ValorPuntos]) VALUES (11, 1, 3, 3, 12.0);
    INSERT INTO [ValoresRubrica] ([IdValor], [IdRubrica], [IdItem], [IdNivel], [ValorPuntos]) VALUES (12, 1, 3, 4, 8.0);

    INSERT INTO [ValoresRubrica] ([IdValor], [IdRubrica], [IdItem], [IdNivel], [ValorPuntos]) VALUES (13, 1, 4, 1, 25.0);
    INSERT INTO [ValoresRubrica] ([IdValor], [IdRubrica], [IdItem], [IdNivel], [ValorPuntos]) VALUES (14, 1, 4, 2, 20.0);
    INSERT INTO [ValoresRubrica] ([IdValor], [IdRubrica], [IdItem], [IdNivel], [ValorPuntos]) VALUES (15, 1, 4, 3, 15.0);
    INSERT INTO [ValoresRubrica] ([IdValor], [IdRubrica], [IdItem], [IdNivel], [ValorPuntos]) VALUES (16, 1, 4, 4, 0.0);

    -- Valores para Rúbrica Tarea 2
    INSERT INTO [ValoresRubrica] ([IdValor], [IdRubrica], [IdItem], [IdNivel], [ValorPuntos]) VALUES (17, 2, 5, 1, 40.0);
    INSERT INTO [ValoresRubrica] ([IdValor], [IdRubrica], [IdItem], [IdNivel], [ValorPuntos]) VALUES (18, 2, 5, 2, 32.0);
    INSERT INTO [ValoresRubrica] ([IdValor], [IdRubrica], [IdItem], [IdNivel], [ValorPuntos]) VALUES (19, 2, 5, 3, 24.0);
    INSERT INTO [ValoresRubrica] ([IdValor], [IdRubrica], [IdItem], [IdNivel], [ValorPuntos]) VALUES (20, 2, 5, 4, 16.0);

    INSERT INTO [ValoresRubrica] ([IdValor], [IdRubrica], [IdItem], [IdNivel], [ValorPuntos]) VALUES (21, 2, 6, 1, 30.0);
    INSERT INTO [ValoresRubrica] ([IdValor], [IdRubrica], [IdItem], [IdNivel], [ValorPuntos]) VALUES (22, 2, 6, 2, 24.0);
    INSERT INTO [ValoresRubrica] ([IdValor], [IdRubrica], [IdItem], [IdNivel], [ValorPuntos]) VALUES (23, 2, 6, 3, 18.0);
    INSERT INTO [ValoresRubrica] ([IdValor], [IdRubrica], [IdItem], [IdNivel], [ValorPuntos]) VALUES (24, 2, 6, 4, 12.0);

    INSERT INTO [ValoresRubrica] ([IdValor], [IdRubrica], [IdItem], [IdNivel], [ValorPuntos]) VALUES (25, 2, 7, 1, 30.0);
    INSERT INTO [ValoresRubrica] ([IdValor], [IdRubrica], [IdItem], [IdNivel], [ValorPuntos]) VALUES (26, 2, 7, 2, 24.0);
    INSERT INTO [ValoresRubrica] ([IdValor], [IdRubrica], [IdItem], [IdNivel], [ValorPuntos]) VALUES (27, 2, 7, 3, 18.0);
    INSERT INTO [ValoresRubrica] ([IdValor], [IdRubrica], [IdItem], [IdNivel], [ValorPuntos]) VALUES (28, 2, 7, 4, 12.0);

    -- Valores para Rúbrica Proyecto 1
    INSERT INTO [ValoresRubrica] ([IdValor], [IdRubrica], [IdItem], [IdNivel], [ValorPuntos]) VALUES (29, 3, 8, 1, 20.0);
    INSERT INTO [ValoresRubrica] ([IdValor], [IdRubrica], [IdItem], [IdNivel], [ValorPuntos]) VALUES (30, 3, 8, 2, 16.0);
    INSERT INTO [ValoresRubrica] ([IdValor], [IdRubrica], [IdItem], [IdNivel], [ValorPuntos]) VALUES (31, 3, 8, 3, 12.0);
    INSERT INTO [ValoresRubrica] ([IdValor], [IdRubrica], [IdItem], [IdNivel], [ValorPuntos]) VALUES (32, 3, 8, 4, 8.0);

    INSERT INTO [ValoresRubrica] ([IdValor], [IdRubrica], [IdItem], [IdNivel], [ValorPuntos]) VALUES (33, 3, 9, 1, 40.0);
    INSERT INTO [ValoresRubrica] ([IdValor], [IdRubrica], [IdItem], [IdNivel], [ValorPuntos]) VALUES (34, 3, 9, 2, 32.0);
    INSERT INTO [ValoresRubrica] ([IdValor], [IdRubrica], [IdItem], [IdNivel], [ValorPuntos]) VALUES (35, 3, 9, 3, 24.0);
    INSERT INTO [ValoresRubrica] ([IdValor], [IdRubrica], [IdItem], [IdNivel], [ValorPuntos]) VALUES (36, 3, 9, 4, 16.0);

    INSERT INTO [ValoresRubrica] ([IdValor], [IdRubrica], [IdItem], [IdNivel], [ValorPuntos]) VALUES (37, 3, 10, 1, 20.0);
    INSERT INTO [ValoresRubrica] ([IdValor], [IdRubrica], [IdItem], [IdNivel], [ValorPuntos]) VALUES (38, 3, 10, 2, 16.0);
    INSERT INTO [ValoresRubrica] ([IdValor], [IdRubrica], [IdItem], [IdNivel], [ValorPuntos]) VALUES (39, 3, 10, 3, 12.0);
    INSERT INTO [ValoresRubrica] ([IdValor], [IdRubrica], [IdItem], [IdNivel], [ValorPuntos]) VALUES (40, 3, 10, 4, 8.0);

    INSERT INTO [ValoresRubrica] ([IdValor], [IdRubrica], [IdItem], [IdNivel], [ValorPuntos]) VALUES (41, 3, 11, 1, 20.0);
    INSERT INTO [ValoresRubrica] ([IdValor], [IdRubrica], [IdItem], [IdNivel], [ValorPuntos]) VALUES (42, 3, 11, 2, 16.0);
    INSERT INTO [ValoresRubrica] ([IdValor], [IdRubrica], [IdItem], [IdNivel], [ValorPuntos]) VALUES (43, 3, 11, 3, 12.0);
    INSERT INTO [ValoresRubrica] ([IdValor], [IdRubrica], [IdItem], [IdNivel], [ValorPuntos]) VALUES (44, 3, 11, 4, 8.0);

    SET IDENTITY_INSERT [ValoresRubrica] OFF;
END

-- =====================================================================
-- 11. RELACIONES INSTRUMENTO-MATERIA
-- =====================================================================
IF NOT EXISTS (SELECT 1 FROM [InstrumentoMaterias] WHERE [InstrumentoEvaluacionId] = 1 AND [MateriaId] = 1)
BEGIN
    INSERT INTO [InstrumentoMaterias] ([InstrumentoEvaluacionId], [MateriaId], [PeriodoAcademicoId], [FechaAsignacion], [EsObligatorio])
    VALUES 
    (1, 1, 1, GETDATE(), 1),
    (2, 1, 1, GETDATE(), 1),
    (3, 1, 1, GETDATE(), 1);
END

-- =====================================================================
-- 12. RELACIONES INSTRUMENTO-R?BRICA CON PONDERACIONES
-- =====================================================================
IF NOT EXISTS (SELECT 1 FROM [InstrumentoRubricas] WHERE [InstrumentoEvaluacionId] = 1 AND [RubricaId] = 1)
BEGIN
    INSERT INTO [InstrumentoRubricas] ([InstrumentoEvaluacionId], [RubricaId], [FechaAsignacion], [EsObligatorio], [Ponderacion], [OrdenPresentacion])
    VALUES 
    (1, 1, GETDATE(), 1, 30.0, 1),
    (2, 2, GETDATE(), 1, 30.0, 2),
    (3, 3, GETDATE(), 1, 40.0, 3);
END

-- =====================================================================
-- 13. EVALUACIONES DE EJEMPLO
-- =====================================================================
IF NOT EXISTS (SELECT 1 FROM [Evaluaciones] WHERE [IdEvaluacion] = 1)
BEGIN
    SET IDENTITY_INSERT [Evaluaciones] ON;

    -- Juan Carlos: 100, 80, 90 ? Total esperado: 90.00
    INSERT INTO [Evaluaciones] ([IdEvaluacion], [IdEstudiante], [IdRubrica], [FechaEvaluacion], [TotalPuntos], [Observaciones], [Estado])
    VALUES 
    (1, 1, 1, DATEADD(day, -10, GETDATE()), 100.0, 'Excelente trabajo', 'FINALIZADA'),
    (2, 1, 2, DATEADD(day, -8, GETDATE()), 80.0, 'Buen trabajo', 'FINALIZADA'),
    (3, 1, 3, DATEADD(day, -5, GETDATE()), 90.0, 'Muy buen proyecto', 'FINALIZADA');

    -- María José: 85, 75, 88 ? Total esperado: 83.20
    INSERT INTO [Evaluaciones] ([IdEvaluacion], [IdEstudiante], [IdRubrica], [FechaEvaluacion], [TotalPuntos], [Observaciones], [Estado])
    VALUES 
    (4, 2, 1, DATEADD(day, -10, GETDATE()), 85.0, 'Muy buen trabajo', 'FINALIZADA'),
    (5, 2, 2, DATEADD(day, -8, GETDATE()), 75.0, 'Trabajo satisfactorio', 'FINALIZADA'),
    (6, 2, 3, DATEADD(day, -5, GETDATE()), 88.0, 'Excelente proyecto', 'FINALIZADA');

    -- Carlos: 92, 78, 95 ? Total esperado: 88.60
    INSERT INTO [Evaluaciones] ([IdEvaluacion], [IdEstudiante], [IdRubrica], [FechaEvaluacion], [TotalPuntos], [Observaciones], [Estado])
    VALUES 
    (7, 3, 1, DATEADD(day, -10, GETDATE()), 92.0, 'Trabajo sobresaliente', 'FINALIZADA'),
    (8, 3, 2, DATEADD(day, -8, GETDATE()), 78.0, 'Buen esfuerzo', 'FINALIZADA'),
    (9, 3, 3, DATEADD(day, -5, GETDATE()), 95.0, 'Proyecto excepcional', 'FINALIZADA');

    -- Ana: Solo tiene 2 evaluaciones (para probar valores por defecto)
    INSERT INTO [Evaluaciones] ([IdEvaluacion], [IdEstudiante], [IdRubrica], [FechaEvaluacion], [TotalPuntos], [Observaciones], [Estado])
    VALUES 
    (10, 4, 1, DATEADD(day, -10, GETDATE()), 88.0, 'Muy buen desempeño', 'FINALIZADA'),
    (11, 4, 2, DATEADD(day, -8, GETDATE()), 82.0, 'Trabajo consistente', 'FINALIZADA');

    -- Luis: Solo tiene 1 evaluación (para probar más valores por defecto)
    INSERT INTO [Evaluaciones] ([IdEvaluacion], [IdEstudiante], [IdRubrica], [FechaEvaluacion], [TotalPuntos], [Observaciones], [Estado])
    VALUES 
    (12, 5, 1, DATEADD(day, -10, GETDATE()), 76.0, 'Trabajo aceptable', 'FINALIZADA');

    SET IDENTITY_INSERT [Evaluaciones] OFF;
END

-- =====================================================================
-- 14. CONSULTA DE VERIFICACI?N
-- =====================================================================
PRINT 'VERIFICACIÓN DE DATOS INSERTADOS'

SELECT 'Materias' as Tabla, COUNT(*) as Total FROM [Materias] WHERE [MateriaId] = 1;
SELECT 'Estudiantes' as Tabla, COUNT(*) as Total FROM [Estudiantes] WHERE [PeriodoAcademicoId] = 1;
SELECT 'Instrumentos' as Tabla, COUNT(*) as Total FROM [InstrumentosEvaluacion] WHERE [InstrumentoId] IN (1,2,3);
SELECT 'Rúbricas' as Tabla, COUNT(*) as Total FROM [Rubricas] WHERE [IdRubrica] IN (1,2,3);
SELECT 'Items' as Tabla, COUNT(*) as Total FROM [ItemsEvaluacion] WHERE [IdRubrica] IN (1,2,3);
SELECT 'Valores' as Tabla, COUNT(*) as Total FROM [ValoresRubrica] WHERE [IdRubrica] IN (1,2,3);
SELECT 'Relaciones I-M' as Tabla, COUNT(*) as Total FROM [InstrumentoMaterias] WHERE [MateriaId] = 1;
SELECT 'Relaciones I-R' as Tabla, COUNT(*) as Total FROM [InstrumentoRubricas] WHERE [InstrumentoEvaluacionId] IN (1,2,3);
SELECT 'Evaluaciones' as Tabla, COUNT(*) as Total FROM [Evaluaciones] WHERE [IdEstudiante] IN (1,2,3,4,5);

-- =====================================================================
-- 15. CONSULTA DE PRUEBA DEL CUADERNO
-- =====================================================================
PRINT 'CONSULTA DE PRUEBA DEL CUADERNO CALIFICADOR'

SELECT 
    e.[Nombre] + ' ' + e.[Apellidos] as Estudiante,
    ie.[Nombre] as Instrumento,
    r.[NombreRubrica] as Rubrica,
    ISNULL(ev.[TotalPuntos], 0) as Calificacion,
    ir.[Ponderacion] as 'Ponderacion(%)'
FROM [Estudiantes] e
CROSS JOIN (
    SELECT DISTINCT ie.*, ir.[Ponderacion], ir.[OrdenPresentacion]
    FROM [InstrumentosEvaluacion] ie
    JOIN [InstrumentoMaterias] im ON ie.[InstrumentoId] = im.[InstrumentoEvaluacionId]
    JOIN [InstrumentoRubricas] ir ON ie.[InstrumentoId] = ir.[InstrumentoEvaluacionId]
    WHERE im.[MateriaId] = 1 AND im.[PeriodoAcademicoId] = 1
) ie
JOIN [InstrumentoRubricas] ir ON ie.[InstrumentoId] = ir.[InstrumentoEvaluacionId]
JOIN [Rubricas] r ON ir.[RubricaId] = r.[IdRubrica]
LEFT JOIN [Evaluaciones] ev ON e.[IdEstudiante] = ev.[IdEstudiante] 
    AND r.[IdRubrica] = ev.[IdRubrica] 
    AND ev.[Estado] = 'FINALIZADA'
WHERE e.[PeriodoAcademicoId] = 1
ORDER BY e.[Apellidos], e.[Nombre], ir.[OrdenPresentacion];

PRINT '?? ¡Datos de prueba configurados exitosamente en SQL Server!'
PRINT '====================================================================='

-- =====================================================================
-- INSTRUCCIONES DE PRUEBA
-- =====================================================================
/*
DESPUÉS DE EJECUTAR ESTE SCRIPT:

1. Ejecutar la aplicación: dotnet run

2. Ir a la aplicación web: https://localhost:18163/CalificadorPQ2025

3. Seleccionar:
   - Materia: Matemáticas I
   - Período Académico: Primer Cuatrimestre 2025

4. Hacer clic en "Generar Cuaderno"

5. Verificar resultados esperados:
   - Juan Carlos Pérez: (100×0.30) + (80×0.30) + (90×0.40) = 90.00
   - María José González: (85×0.30) + (75×0.30) + (88×0.40) = 83.20  
   - Carlos Alberto Martínez: (92×0.30) + (78×0.30) + (95×0.40) = 88.60
   - Ana Patricia Ramírez: (88×0.30) + (82×0.30) + (0×0.40) = 51.00 (proyecto pendiente)
   - Luis Fernando Torres: (76×0.30) + (0×0.30) + (0×0.40) = 22.80 (dos pendientes)

6. Probar exportación a CSV

7. Verificar que las columnas se generen dinámicamente:
   - Tarea 1 ? Rúbrica Tarea 1 (30%)
   - Tarea 2 ? Rúbrica Tarea 2 (30%)
   - Proyecto 1 ? Rúbrica Proyecto 1 (40%)
*/