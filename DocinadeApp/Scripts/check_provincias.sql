-- Verificar si existen provincias en la base de datos
SELECT COUNT(*) as TotalProvincias FROM Provincias;
SELECT * FROM Provincias LIMIT 5;

-- Verificar si existen cantones
SELECT COUNT(*) as TotalCantones FROM Cantones;
SELECT * FROM Cantones LIMIT 5;

-- Verificar si existen distritos
SELECT COUNT(*) as TotalDistritos FROM Distritos;
SELECT * FROM Distritos LIMIT 5;