@echo off
echo 🔨 EJECUTANDO DOTNET BUILD
echo ========================================

cd /d "C:\Users\hguido\Desktop\RubricasApp\src\RubricasApp.Web"

echo.
echo Ejecutando dotnet build...
dotnet build

echo.
if %ERRORLEVEL% EQU 0 (
    echo ✅ BUILD EXITOSO!
    echo Los errores del UsersController han sido corregidos.
) else (
    echo ❌ BUILD FALLÓ
    echo Codigo de error: %ERRORLEVEL%
)

echo.
echo ========================================
pause