@echo off
echo =====================================================
echo  COMPILACION Y EJECUCION - RubricasApp
echo =====================================================

cd /d "C:\Users\hguido\Desktop\RubricasApp\src\RubricasApp.Web"

echo.
echo 1. Limpiando solucion...
dotnet clean

echo.
echo 2. Restaurando paquetes NuGet...
dotnet restore

echo.
echo 3. Compilando proyecto...
dotnet build

if %ERRORLEVEL% NEQ 0 (
    echo.
    echo ERROR: Fallos en la compilacion
    echo Presione cualquier tecla para salir...
    pause
    exit /b 1
)

echo.
echo =====================================================
echo  COMPILACION EXITOSA!
echo =====================================================
echo.
echo El proyecto se ha compilado correctamente.
echo La columna Estado se agregara automaticamente al ejecutar.
echo.
echo Para ejecutar la aplicacion:
echo   dotnet run
echo.
echo O navegue a: https://localhost:5001 o http://localhost:5000
echo.
echo Presione cualquier tecla para continuar...
pause