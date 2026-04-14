-- Insertar las 8 materias con la estructura correcta
INSERT INTO Materias (Codigo, Nombre, Descripcion, Creditos, Activa, Tipo, CicloSugerido, FechaCreacion, Estado) VALUES 
('TEST001', 'Materia de Prueba', 'Materia creada para testing', 3, 1, 'Teorica', 1, datetime('now'), 'Activa'),
('PROG101', 'Programacion I', 'Introduccion a la programacion', 4, 1, 'Teorica', 1, datetime('now'), 'Activa'),
('MAT101', 'Matematicas I', 'Fundamentos matematicos', 5, 1, 'Teorica', 1, datetime('now'), 'Activa'),
('ING101', 'Ingles I', 'Ingles basico', 3, 1, 'Teorica', 1, datetime('now'), 'Activa'),
('FIS101', 'Fisica I', 'Fisica general', 4, 1, 'Teorica', 2, datetime('now'), 'Activa'),
('PROG201', 'Programacion II', 'Programacion avanzada', 4, 1, 'Teorica', 2, datetime('now'), 'Activa'),
('BDD101', 'Base de Datos I', 'Fundamentos de bases de datos', 4, 1, 'Teorica', 2, datetime('now'), 'Activa'),
('WEB101', 'Desarrollo Web I', 'Desarrollo web basico', 3, 1, 'Practica', 2, datetime('now'), 'Activa');

-- Mostrar las materias insertadas
SELECT COUNT(*) as TotalMaterias FROM Materias;
SELECT Codigo, Nombre, Tipo, Creditos, CicloSugerido FROM Materias ORDER BY Codigo;