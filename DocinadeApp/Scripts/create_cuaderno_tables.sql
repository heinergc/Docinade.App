-- Crear tablas del sistema Cuaderno Calificador

CREATE TABLE IF NOT EXISTS CuadernosCalificadores (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Nombre NVARCHAR(200) NOT NULL,
    MateriaId INTEGER NOT NULL,
    PeriodoAcademicoId INTEGER NOT NULL,
    FechaCreacion DATETIME NOT NULL DEFAULT (datetime('now')),
    FechaCierre DATETIME NULL,
    Estado NVARCHAR(20) NOT NULL DEFAULT 'ACTIVO',
    Observaciones NVARCHAR(500) NULL,
    FOREIGN KEY (MateriaId) REFERENCES Materias(Id),
    FOREIGN KEY (PeriodoAcademicoId) REFERENCES PeriodosAcademicos(Id)
);

CREATE TABLE IF NOT EXISTS CuadernoInstrumentos (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    CuadernoCalificadorId INTEGER NOT NULL,
    RubricaId INTEGER NOT NULL,
    PonderacionPorcentaje DECIMAL(5,2) NOT NULL,
    EsObligatorio BOOLEAN NOT NULL DEFAULT 1,
    Orden INTEGER NOT NULL DEFAULT 1,
    FechaAsignacion DATETIME NOT NULL DEFAULT (datetime('now')),
    FOREIGN KEY (CuadernoCalificadorId) REFERENCES CuadernosCalificadores(Id) ON DELETE CASCADE,
    FOREIGN KEY (RubricaId) REFERENCES Rubricas(Id)
);

-- Insertar algunos datos de ejemplo
INSERT OR IGNORE INTO CuadernosCalificadores (Id, Nombre, MateriaId, PeriodoAcademicoId, Estado) VALUES
(1, 'Cuaderno Matemáticas I - Período 2024-1', 1, 1, 'ACTIVO'),
(2, 'Cuaderno Física I - Período 2024-1', 2, 1, 'ACTIVO');

-- Asociar instrumentos con el cuaderno
INSERT OR IGNORE INTO CuadernoInstrumentos (CuadernoCalificadorId, RubricaId, PonderacionPorcentaje, EsObligatorio, Orden) VALUES
(1, 1, 40.00, 1, 1),
(1, 2, 35.00, 1, 2),
(1, 3, 25.00, 1, 3),
(2, 1, 50.00, 1, 1),
(2, 2, 50.00, 1, 2);