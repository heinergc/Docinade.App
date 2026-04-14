@echo off
echo =====================================================
echo  LIMPIEZA Y RECOMPILACION - RubricasApp
echo =====================================================

cd /d "C:\Users\hguido\Desktop\RubricasApp\src\RubricasApp.Web"

echo.
echo 1. Limpiando directorios bin y obj...
if exist bin rmdir /s /q bin
if exist obj rmdir /s /q obj

echo.
echo 2. Limpiando solucion...
dotnet clean

echo.
echo 3. Restaurando paquetes NuGet...
dotnet restore

echo.
echo 4. Recompilando proyecto...
dotnet build --configuration Release

if %ERRORLEVEL% EQU 0 (
    echo.
    echo =====================================================
    echo  COMPILACION EXITOSA!
    echo =====================================================
    echo.
    echo El proyecto se ha compilado correctamente.
    echo Los archivos de Scripts han sido excluidos de la compilacion.
    echo La columna Estado se agregara automaticamente al ejecutar.
    echo.
    echo Para ejecutar la aplicacion:
    echo   dotnet run
    echo.
    echo Presione cualquier tecla para continuar...
    pause
) else (
    echo.
    echo =====================================================
    echo  ERROR EN LA COMPILACION
    echo =====================================================
    echo.
    echo Por favor revise los errores mostrados arriba.
    echo.
    echo Presione cualquier tecla para salir...
    pause
    exit /b 1
)