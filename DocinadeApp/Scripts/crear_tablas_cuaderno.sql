-- Script para crear tablas del Cuaderno Calificador

-- Tabla Materias (si no existe)
CREATE TABLE IF NOT EXISTS Materias (
    MateriaId INTEGER PRIMARY KEY AUTOINCREMENT,
    Codigo NVARCHAR(20) NOT NULL UNIQUE,
    Nombre NVARCHAR(120) NOT NULL,
    Tipo NVARCHAR(50),
    Descripcion NVARCHAR(500),
    Creditos INTEGER NOT NULL DEFAULT 0,
    HorasSemanales INTEGER NOT NULL DEFAULT 0,
    Activo BOOLEAN NOT NULL DEFAULT 1,
    FechaCreacion DATETIME NOT NULL DEFAULT (datetime('now'))
);

-- Tabla CuadernosCalificadores
CREATE TABLE IF NOT EXISTS CuadernosCalificadores (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    MateriaId INTEGER NOT NULL,
    PeriodoAcademicoId INTEGER NOT NULL,
    Nombre NVARCHAR(200) NOT NULL,
    FechaCreacion DATETIME NOT NULL DEFAULT (datetime('now')),
    Estado NVARCHAR(20) NOT NULL DEFAULT 'ACTIVO',
    FechaCierre DATETIME NULL,
    FOREIGN KEY (MateriaId) REFERENCES Materias(MateriaId) ON DELETE RESTRICT,
    FOREIGN KEY (PeriodoAcademicoId) REFERENCES PeriodosAcademicos(Id) ON DELETE RESTRICT
);

-- Tabla CuadernoInstrumentos
CREATE TABLE IF NOT EXISTS CuadernoInstrumentos (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    CuadernoCalificadorId INTEGER NOT NULL,
    RubricaId INTEGER NOT NULL,
    PonderacionPorcentaje DECIMAL(5,2) NOT NULL,
    Orden INTEGER NOT NULL DEFAULT 0,
    EsObligatorio BOOLEAN NOT NULL DEFAULT 1,
    FOREIGN KEY (CuadernoCalificadorId) REFERENCES CuadernosCalificadores(Id) ON DELETE CASCADE,
    FOREIGN KEY (RubricaId) REFERENCES Rubricas(IdRubrica) ON DELETE RESTRICT
);

-- Índices para mejorar rendimiento
CREATE INDEX IF NOT EXISTS IX_CuadernosCalificadores_MateriaId ON CuadernosCalificadores(MateriaId);
CREATE INDEX IF NOT EXISTS IX_CuadernosCalificadores_PeriodoAcademicoId ON CuadernosCalificadores(PeriodoAcademicoId);
CREATE INDEX IF NOT EXISTS IX_CuadernoInstrumentos_CuadernoCalificadorId ON CuadernoInstrumentos(CuadernoCalificadorId);
CREATE INDEX IF NOT EXISTS IX_CuadernoInstrumentos_RubricaId ON CuadernoInstrumentos(RubricaId);

-- Insertar materias de ejemplo
INSERT OR IGNORE INTO Materias (Codigo, Nombre, Tipo, Descripcion, Creditos, HorasSemanales) VALUES
('ING101', 'Inglés Técnico I', 'Obligatoria', 'Curso básico de inglés técnico para ingeniería', 3, 4),
('MAT101', 'Matemáticas I', 'Obligatoria', 'Álgebra y geometría analítica', 4, 6),
('FIS101', 'Física I', 'Obligatoria', 'Mecánica clásica y termodinámica', 4, 6),
('QUI101', 'Química General', 'Obligatoria', 'Fundamentos de química general', 3, 4),
('INF101', 'Introducción a la Programación', 'Obligatoria', 'Fundamentos de programación', 4, 6),
('COM101', 'Comunicación Oral y Escrita', 'Obligatoria', 'Técnicas de comunicación efectiva', 2, 3);

-- Insertar cuaderno de ejemplo
INSERT OR IGNORE INTO CuadernosCalificadores (MateriaId, PeriodoAcademicoId, Nombre) 
SELECT 1, 1, 'Cuaderno - Inglés Técnico I - 2025 C1'
WHERE EXISTS (SELECT 1 FROM Materias WHERE MateriaId = 1) 
  AND EXISTS (SELECT 1 FROM PeriodosAcademicos WHERE Id = 1);

-- Verificar las tablas creadas
.schema CuadernosCalificadores
.schema CuadernoInstrumentos
.schema Materias