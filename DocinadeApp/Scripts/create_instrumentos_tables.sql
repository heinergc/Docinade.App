-- Crear tablas del sistema de instrumentos de evaluación

CREATE TABLE IF NOT EXISTS InstrumentosEvaluacion (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Nombre NVARCHAR(200) NOT NULL,
    Descripcion NVARCHAR(500),
    EstaActivo BOOLEAN NOT NULL DEFAULT 1,
    FechaCreacion DATETIME NOT NULL DEFAULT (datetime('now'))
);

CREATE TABLE IF NOT EXISTS InstrumentoRubricas (
    InstrumentoEvaluacionId INTEGER NOT NULL,
    RubricaId INTEGER NOT NULL,
    FechaAsignacion DATETIME NOT NULL DEFAULT (datetime('now')),
    OrdenPresentacion INTEGER,
    EsObligatorio BOOLEAN NOT NULL DEFAULT 0,
    Ponderacion DECIMAL(5,2) NOT NULL DEFAULT 0,
    PRIMARY KEY (InstrumentoEvaluacionId, RubricaId),
    FOREIGN KEY (InstrumentoEvaluacionId) REFERENCES InstrumentosEvaluacion(Id),
    FOREIGN KEY (RubricaId) REFERENCES Rubricas(Id)
);

CREATE TABLE IF NOT EXISTS InstrumentoMaterias (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    InstrumentoEvaluacionId INTEGER NOT NULL,
    MateriaId INTEGER NOT NULL,
    FechaAsignacion DATETIME NOT NULL DEFAULT (datetime('now')),
    EsObligatorio BOOLEAN NOT NULL DEFAULT 0,
    OrdenPresentacion INTEGER,
    FOREIGN KEY (InstrumentoEvaluacionId) REFERENCES InstrumentosEvaluacion(Id),
    FOREIGN KEY (MateriaId) REFERENCES Materias(Id)
);

-- Insertar algunos datos de ejemplo
INSERT OR IGNORE INTO InstrumentosEvaluacion (Id, Nombre, Descripcion, EstaActivo) VALUES
(1, 'Parcial 1', 'Primer examen parcial del periodo', 1),
(2, 'Parcial 2', 'Segundo examen parcial del periodo', 1),
(3, 'Trabajo Final', 'Proyecto final de la materia', 1),
(4, 'Participación', 'Evaluación de participación en clase', 1);

-- Relacionar instrumentos con rúbricas (ejemplo)
INSERT OR IGNORE INTO InstrumentoRubricas (InstrumentoEvaluacionId, RubricaId, Ponderacion) VALUES
(1, 1, 30.00),
(2, 1, 30.00),
(3, 1, 30.00),
(4, 1, 10.00);