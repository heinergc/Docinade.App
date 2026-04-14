-- Migración manual para convertir PeriodoAcademico de string a foreign key
-- Fecha: $(Get-Date)

BEGIN TRANSACTION;

-- 1. Crear tabla temporal para respaldar datos existentes
CREATE TABLE InstrumentoMaterias_Backup AS 
SELECT InstrumentoId, MateriaId, PeriodoAcademico
FROM InstrumentoMaterias;

-- 2. Agregar nueva columna PeriodoAcademicoId
ALTER TABLE InstrumentoMaterias ADD COLUMN PeriodoAcademicoId INTEGER;

-- 3. Intentar mapear los períodos académicos existentes a IDs
-- Esto asume que los valores en PeriodoAcademico corresponden a NombreCompleto en PeriodosAcademicos
UPDATE InstrumentoMaterias 
SET PeriodoAcademicoId = (
    SELECT p.Id 
    FROM PeriodosAcademicos p 
    WHERE p.NombreCompleto = InstrumentoMaterias.PeriodoAcademico
    OR p.Año || '-' || p.Ciclo = InstrumentoMaterias.PeriodoAcademico
    LIMIT 1
);

-- 4. Para registros que no pudieron mapearse, usar el primer período académico activo
UPDATE InstrumentoMaterias 
SET PeriodoAcademicoId = (
    SELECT Id FROM PeriodosAcademicos WHERE Activo = 1 ORDER BY Id LIMIT 1
)
WHERE PeriodoAcademicoId IS NULL;

-- 5. Verificar que todos los registros tienen PeriodoAcademicoId
SELECT 'Registros sin PeriodoAcademicoId:' AS Info, COUNT(*) AS Cantidad
FROM InstrumentoMaterias 
WHERE PeriodoAcademicoId IS NULL;

-- 6. Eliminar la columna antigua PeriodoAcademico
-- En SQLite necesitamos recrear la tabla para eliminar columnas
CREATE TABLE InstrumentoMaterias_New (
    InstrumentoId INTEGER NOT NULL,
    MateriaId INTEGER NOT NULL,
    PeriodoAcademicoId INTEGER NOT NULL,
    PRIMARY KEY (InstrumentoId, MateriaId),
    FOREIGN KEY (InstrumentoId) REFERENCES InstrumentosEvaluacion(InstrumentoId) ON DELETE CASCADE,
    FOREIGN KEY (MateriaId) REFERENCES Materias(MateriaId) ON DELETE CASCADE,
    FOREIGN KEY (PeriodoAcademicoId) REFERENCES PeriodosAcademicos(Id) ON DELETE RESTRICT
);

-- 7. Copiar datos a la nueva tabla
INSERT INTO InstrumentoMaterias_New (InstrumentoId, MateriaId, PeriodoAcademicoId)
SELECT InstrumentoId, MateriaId, PeriodoAcademicoId
FROM InstrumentoMaterias
WHERE PeriodoAcademicoId IS NOT NULL;

-- 8. Reemplazar la tabla original
DROP TABLE InstrumentoMaterias;
ALTER TABLE InstrumentoMaterias_New RENAME TO InstrumentoMaterias;

-- 9. Verificación final
SELECT 'Migración completada. Registros en InstrumentoMaterias:' AS Info, COUNT(*) AS Cantidad
FROM InstrumentoMaterias;

SELECT 'Verificación de foreign keys:' AS Info;
SELECT im.*, p.NombreCompleto as PeriodoNombre
FROM InstrumentoMaterias im
JOIN PeriodosAcademicos p ON im.PeriodoAcademicoId = p.Id
LIMIT 5;

COMMIT;