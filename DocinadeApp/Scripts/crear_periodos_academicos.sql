-- Script para crear períodos académicos de prueba para el sistema de rúbricas
-- Fecha: 18 de agosto de 2025

-- Insertar períodos académicos para 2025
INSERT INTO PeriodosAcademicos (
    Año, Anio, Ciclo, FechaInicio, FechaFin, Activo, 
    Codigo, Nombre, Tipo, NumeroPeriodo, FechaCreacion, Estado
) VALUES 
-- Primer Cuatrimestre 2025
(2025, 2025, 'C1', '2025-01-15', '2025-05-15', 1, 
 'C1', 'Primer Cuatrimestre', 0, 1, '2025-08-18 10:00:00', 'Activo'),

-- Segundo Cuatrimestre 2025  
(2025, 2025, 'C2', '2025-05-16', '2025-09-15', 1, 
 'C2', 'Segundo Cuatrimestre', 0, 2, '2025-08-18 10:00:00', 'Activo'),

-- Tercer Cuatrimestre 2025
(2025, 2025, 'C3', '2025-09-16', '2025-12-15', 1, 
 'C3', 'Tercer Cuatrimestre', 0, 3, '2025-08-18 10:00:00', 'Activo'),

-- Primer Semestre 2025
(2025, 2025, 'S1', '2025-01-15', '2025-06-15', 1, 
 'S1', 'Primer Semestre', 1, 1, '2025-08-18 10:00:00', 'Activo'),

-- Segundo Semestre 2025
(2025, 2025, 'S2', '2025-06-16', '2025-12-15', 1, 
 'S2', 'Segundo Semestre', 1, 2, '2025-08-18 10:00:00', 'Activo'),

-- Períodos para 2024 (históricos)
(2024, 2024, 'C1', '2024-01-15', '2024-05-15', 0, 
 'C1', 'Primer Cuatrimestre', 0, 1, '2024-01-15 10:00:00', 'Finalizado'),

(2024, 2024, 'C2', '2024-05-16', '2024-09-15', 0, 
 'C2', 'Segundo Cuatrimestre', 0, 2, '2024-05-16 10:00:00', 'Finalizado'),

(2024, 2024, 'C3', '2024-09-16', '2024-12-15', 0, 
 'C3', 'Tercer Cuatrimestre', 0, 3, '2024-09-16 10:00:00', 'Finalizado');

-- Verificar los datos insertados
SELECT 'Períodos creados:' as mensaje;
SELECT Id, Anio, Ciclo, 
       strftime('%Y-%m-%d', FechaInicio) as FechaInicio,
       strftime('%Y-%m-%d', FechaFin) as FechaFin,
       CASE WHEN Activo = 1 THEN 'SÍ' ELSE 'NO' END as Activo,
       Estado
FROM PeriodosAcademicos 
ORDER BY Anio DESC, Ciclo;