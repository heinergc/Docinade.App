@echo off
echo ========================================
echo Verificando errores de compilación
echo ========================================

echo Compilando proyecto...
dotnet build --verbosity normal

echo.
echo ========================================
echo Compilación completada
echo ========================================
pause