@echo off
echo ========================================
echo Compilando proyecto para detectar errores
echo ========================================

cd /d "C:\Users\hguido\Desktop\RubricasApp\src\RubricasApp.Web"

echo Limpiando proyecto...
dotnet clean

echo.
echo Restaurando paquetes NuGet...
dotnet restore

echo.
echo Compilando proyecto...
dotnet build --verbosity normal --no-restore > compilation_output.txt 2>&1

echo.
echo Mostrando errores de compilación:
echo ========================================
type compilation_output.txt | findstr /i "error"

echo.
echo ========================================
echo Compilación completada. Revisa compilation_output.txt para detalles completos.
echo ========================================
pause