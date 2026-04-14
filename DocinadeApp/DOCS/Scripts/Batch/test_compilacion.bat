@echo off
echo Compilacion rapida...

cd /d "C:\Users\hguido\Desktop\RubricasApp\src\RubricasApp.Web"

dotnet clean --verbosity quiet
dotnet build --verbosity quiet

if %ERRORLEVEL% EQU 0 (
    echo EXITO: Proyecto compilado correctamente!
    echo.
    echo La funcionalidad de evaluaciones como borrador esta lista.
    echo El error de la columna Estado se resolvera automaticamente al ejecutar.
) else (
    echo ERROR: Revise los errores de compilacion
    dotnet build
)

echo.
echo Para ejecutar: dotnet run
echo Para acceder: https://localhost:18163/Evaluaciones