@echo off
echo Eliminando migraciones existentes y creando nueva migracion inicial...

echo.
echo 1. Eliminando base de datos existente...
if exist RubricasDbNueva.db del RubricasDbNueva.db
if exist RubricasDbNueva.db-shm del RubricasDbNueva.db-shm
if exist RubricasDbNueva.db-wal del RubricasDbNueva.db-wal

echo.
echo 2. Eliminando carpeta de migraciones...
if exist Migrations rmdir /s /q Migrations

echo.
echo 3. Creando nueva migracion inicial...
dotnet ef migrations add InitialCreateWithIdentity --verbose

echo.
echo 4. Aplicando migraciones a la base de datos...
dotnet ef database update --verbose

echo.
echo 5. Proceso completado. Presione cualquier tecla para continuar...
pause