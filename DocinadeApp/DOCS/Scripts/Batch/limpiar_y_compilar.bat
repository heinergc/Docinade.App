@echo off
echo ========================================
echo     LIMPIEZA COMPLETA DEL PROYECTO
echo ========================================
echo.

echo 1. Limpiando configuraciones Debug y Release...
dotnet clean --configuration Debug
dotnet clean --configuration Release

echo.
echo 2. Eliminando directorios de archivos compilados...
if exist bin (
    rmdir /s /q bin
    echo    - bin eliminado
)

if exist obj (
    rmdir /s /q obj
    echo    - obj eliminado
)

echo.
echo 3. Restaurando paquetes NuGet...
dotnet restore

echo.
echo 4. Compilando el proyecto...
dotnet build --no-restore --verbosity minimal

echo.
if %ERRORLEVEL% EQU 0 (
    echo ========================================
    echo     COMPILACION EXITOSA!
    echo ========================================
) else (
    echo ========================================
    echo     ERROR EN LA COMPILACION
    echo ========================================
)

echo.
pause