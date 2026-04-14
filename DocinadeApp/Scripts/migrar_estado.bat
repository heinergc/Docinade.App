@echo off
echo Agregando columna Estado a la tabla Evaluaciones...

REM Cambiar al directorio del proyecto
cd /d "C:\Users\hguido\Desktop\RubricasApp\src\RubricasApp.Web"

REM Verificar si existe sqlite3.exe en el PATH
where sqlite3 >nul 2>nul
if %ERRORLEVEL% NEQ 0 (
    echo SQLite3 no encontrado en el PATH.
    echo Instalando SQLite3...
    
    REM Crear directorio temporal
    if not exist "temp" mkdir temp
    
    REM Descargar SQLite3 (esto requiere curl o PowerShell)
    powershell -Command "Invoke-WebRequest -Uri 'https://www.sqlite.org/2023/sqlite-tools-win32-x86-3420000.zip' -OutFile 'temp\sqlite.zip'"
    powershell -Command "Expand-Archive -Path 'temp\sqlite.zip' -DestinationPath 'temp\sqlite' -Force"
    
    REM Usar SQLite3 desde temp
    set SQLITE_PATH=temp\sqlite\sqlite3.exe
) else (
    set SQLITE_PATH=sqlite3
)

echo Ejecutando migración...

REM Verificar estructura actual
echo PRAGMA table_info(Evaluaciones); | %SQLITE_PATH% RubricasDbNueva.db

REM Agregar columna Estado
echo ALTER TABLE Evaluaciones ADD COLUMN Estado TEXT DEFAULT 'BORRADOR'; | %SQLITE_PATH% RubricasDbNueva.db

REM Actualizar registros existentes
echo UPDATE Evaluaciones SET Estado = 'COMPLETADA' WHERE Estado IS NULL OR Estado = ''; | %SQLITE_PATH% RubricasDbNueva.db

echo Verificando resultado...
echo PRAGMA table_info(Evaluaciones); | %SQLITE_PATH% RubricasDbNueva.db

echo.
echo Migración completada!
echo Presione cualquier tecla para continuar...
pause