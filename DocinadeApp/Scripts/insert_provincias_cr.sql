-- Script para insertar las provincias de Costa Rica en la base de datos
-- Este script debe ejecutarse si la tabla de Provincias está vacía

-- Insertar Provincias
INSERT OR IGNORE INTO Provincias (Id, Nombre, Codigo, Estado) VALUES
(1, 'San José', 'SJ', 1),
(2, 'Alajuela', 'AJ', 1),
(3, 'Cartago', 'CT', 1),
(4, 'Heredia', 'HR', 1),
(5, 'Guanacaste', 'GC', 1),
(6, 'Puntarenas', 'PT', 1),
(7, 'Limón', 'LM', 1);

-- Insertar algunos cantones principales de San José
INSERT OR IGNORE INTO Cantones (Id, ProvinciaId, Nombre, Codigo, Estado) VALUES
(1, 1, 'San José', '01', 1),
(2, 1, 'Escazú', '02', 1),
(3, 1, 'Desamparados', '03', 1),
(4, 1, 'Puriscal', '04', 1),
(5, 1, 'Tarrazú', '05', 1),
(6, 1, 'Aserrí', '06', 1),
(7, 1, 'Mora', '07', 1),
(8, 1, 'Goicoechea', '08', 1),
(9, 1, 'Santa Ana', '09', 1),
(10, 1, 'Alajuelita', '10', 1),
(11, 1, 'Vásquez de Coronado', '11', 1),
(12, 1, 'Acosta', '12', 1),
(13, 1, 'Tibás', '13', 1),
(14, 1, 'Moravia', '14', 1),
(15, 1, 'Montes de Oca', '15', 1),
(16, 1, 'Turrubares', '16', 1),
(17, 1, 'Dota', '17', 1),
(18, 1, 'Curridabat', '18', 1),
(19, 1, 'Pérez Zeledón', '19', 1),
(20, 1, 'León Cortés Castro', '20', 1);

-- Insertar algunos cantones principales de Alajuela
INSERT OR IGNORE INTO Cantones (Id, ProvinciaId, Nombre, Codigo, Estado) VALUES
(21, 2, 'Alajuela', '01', 1),
(22, 2, 'San Ramón', '02', 1),
(23, 2, 'Grecia', '03', 1),
(24, 2, 'San Mateo', '04', 1),
(25, 2, 'Atenas', '05', 1),
(26, 2, 'Naranjo', '06', 1),
(27, 2, 'Palmares', '07', 1),
(28, 2, 'Poás', '08', 1),
(29, 2, 'Orotina', '09', 1),
(30, 2, 'San Carlos', '10', 1);

-- Insertar algunos distritos de San José (cantón)
INSERT OR IGNORE INTO Distritos (Id, CantonId, Nombre, Codigo, Estado) VALUES
(1, 1, 'Carmen', '01', 1),
(2, 1, 'Merced', '02', 1),
(3, 1, 'Hospital', '03', 1),
(4, 1, 'Catedral', '04', 1),
(5, 1, 'Zapote', '05', 1),
(6, 1, 'San Francisco de Dos Ríos', '06', 1),
(7, 1, 'La Uruca', '07', 1),
(8, 1, 'Mata Redonda', '08', 1),
(9, 1, 'Pavas', '09', 1),
(10, 1, 'Hatillo', '10', 1),
(11, 1, 'San Sebastián', '11', 1);

-- Insertar algunos distritos de Escazú
INSERT OR IGNORE INTO Distritos (Id, CantonId, Nombre, Codigo, Estado) VALUES
(12, 2, 'Escazú', '01', 1),
(13, 2, 'San Antonio', '02', 1),
(14, 2, 'San Rafael', '03', 1);

-- Insertar algunos distritos de Alajuela (cantón)
INSERT OR IGNORE INTO Distritos (Id, CantonId, Nombre, Codigo, Estado) VALUES
(15, 21, 'Alajuela', '01', 1),
(16, 21, 'San José', '02', 1),
(17, 21, 'Carrizal', '03', 1),
(18, 21, 'San Antonio', '04', 1);

-- Verificar que se insertaron correctamente
SELECT 'Provincias insertadas:' as Mensaje, COUNT(*) as Total FROM Provincias;
SELECT 'Cantones insertados:' as Mensaje, COUNT(*) as Total FROM Cantones;  
SELECT 'Distritos insertados:' as Mensaje, COUNT(*) as Total FROM Distritos;