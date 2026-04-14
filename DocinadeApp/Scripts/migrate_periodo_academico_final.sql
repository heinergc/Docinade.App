-- Migración manual para convertir PeriodoAcademico de string a foreign key
-- Fecha: 17 Julio 2025

BEGIN TRANSACTION;

-- 1. Crear tabla temporal para respaldar datos existentes
CREATE TABLE InstrumentoMaterias_Backup AS 
SELECT InstrumentoId, MateriaId, PeriodoAcademico
FROM InstrumentoMaterias;

-- 2. Ver datos actuales antes de la migración
SELECT 'Datos antes de migración:' AS Info;
SELECT * FROM InstrumentoMaterias;
SELECT 'Períodos académicos disponibles:' AS Info;
SELECT Id, Año, Anio, Ciclo FROM PeriodosAcademicos;

-- 3. Crear nueva tabla con la estructura correcta
CREATE TABLE InstrumentoMaterias_New (
    InstrumentoId INTEGER NOT NULL,
    MateriaId INTEGER NOT NULL,
    PeriodoAcademicoId INTEGER NOT NULL,
    PRIMARY KEY (InstrumentoId, MateriaId),
    FOREIGN KEY (InstrumentoId) REFERENCES InstrumentosEvaluacion(InstrumentoId) ON DELETE CASCADE,
    FOREIGN KEY (MateriaId) REFERENCES Materias(MateriaId) ON DELETE CASCADE,
    FOREIGN KEY (PeriodoAcademicoId) REFERENCES PeriodosAcademicos(Id) ON DELETE RESTRICT
);

-- 4. Migrar datos existentes - mapear "2024-2" al primer período académico activo (Id=1)
INSERT INTO InstrumentoMaterias_New (InstrumentoId, MateriaId, PeriodoAcademicoId)
SELECT 
    InstrumentoId, 
    MateriaId, 
    1 AS PeriodoAcademicoId  -- Usar el primer período académico (2025-C1)
FROM InstrumentoMaterias;

-- 5. Verificar datos migrados
SELECT 'Datos migrados:' AS Info;
SELECT im.*, p.Año, p.Ciclo
FROM InstrumentoMaterias_New im
JOIN PeriodosAcademicos p ON im.PeriodoAcademicoId = p.Id;

-- 6. Reemplazar la tabla original
DROP TABLE InstrumentoMaterias;
ALTER TABLE InstrumentoMaterias_New RENAME TO InstrumentoMaterias;

-- 7. Verificación final
SELECT 'Migración completada. Registros en InstrumentoMaterias:' AS Info, COUNT(*) AS Cantidad
FROM InstrumentoMaterias;

SELECT 'Verificación final de datos:' AS Info;
SELECT im.*, p.Año, p.Ciclo, p.Activo
FROM InstrumentoMaterias im
JOIN PeriodosAcademicos p ON im.PeriodoAcademicoId = p.Id;

COMMIT;