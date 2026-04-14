-- Agregar columna Estado a la tabla Evaluaciones
-- Si la columna ya existe, esto fallará silenciosamente en SQLite

-- Verificar estructura actual de la tabla
PRAGMA table_info(Evaluaciones);

-- Agregar la columna Estado si no existe
ALTER TABLE Evaluaciones ADD COLUMN Estado TEXT DEFAULT 'BORRADOR';

-- Actualizar registros existentes para que tengan estado COMPLETADA
UPDATE Evaluaciones SET Estado = 'COMPLETADA' WHERE Estado IS NULL OR Estado = '';

-- Verificar que la columna se agregó correctamente
PRAGMA table_info(Evaluaciones);

-- Mostrar algunos registros para verificar
SELECT IdEvaluacion, IdEstudiante, IdRubrica, Estado, FechaEvaluacion 
FROM Evaluaciones 
LIMIT 5;