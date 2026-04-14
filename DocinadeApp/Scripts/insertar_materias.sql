-- Insertar solo las 8 materias nuevas
INSERT INTO Materias (Codigo, Nombre, Descripcion, Creditos, Activa, Tipo, Nivel, Prerrequisitos, FechaCreacion, Estado, TotalHoras, HorasTeoricas, HorasPracticas) VALUES 
('TEST001', 'Materia de Prueba', 'Materia creada para testing', 3, 1, 'Teorica', 'Intermedio', NULL, datetime('now'), 'Activa', 60, 40, 20),
('PROG101', 'Programacion I', 'Introduccion a la programacion', 4, 1, 'Teorica', 'Basico', NULL, datetime('now'), 'Activa', 80, 60, 20),
('MAT101', 'Matematicas I', 'Fundamentos matematicos', 5, 1, 'Teorica', 'Basico', NULL, datetime('now'), 'Activa', 100, 80, 20),
('ING101', 'Ingles I', 'Ingles basico', 3, 1, 'Teorica', 'Basico', NULL, datetime('now'), 'Activa', 60, 40, 20),
('FIS101', 'Fisica I', 'Fisica general', 4, 1, 'Teorica', 'Intermedio', 'MAT101', datetime('now'), 'Activa', 80, 60, 20),
('PROG201', 'Programacion II', 'Programacion avanzada', 4, 1, 'Teorica', 'Intermedio', 'PROG101', datetime('now'), 'Activa', 80, 60, 20),
('BDD101', 'Base de Datos I', 'Fundamentos de bases de datos', 4, 1, 'Teorica', 'Intermedio', 'PROG101', datetime('now'), 'Activa', 80, 60, 20),
('WEB101', 'Desarrollo Web I', 'Desarrollo web basico', 3, 1, 'Practica', 'Intermedio', 'PROG101', datetime('now'), 'Activa', 60, 20, 40);

-- Mostrar las materias insertadas
SELECT COUNT(*) as TotalMaterias FROM Materias;
SELECT Codigo, Nombre, Tipo, Creditos FROM Materias ORDER BY Codigo;