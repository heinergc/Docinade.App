@echo off
echo ========================================
echo COMPILACION DIAGNOSTICA
echo ========================================

cd /d "C:\Users\hguido\Desktop\RubricasApp\src\RubricasApp.Web"

echo Limpiando...
dotnet clean > nul 2>&1

echo Compilando...
dotnet build --no-restore > build_output.txt 2>&1

echo.
echo === PRIMEROS 10 ERRORES ===
findstr /n /i "error CS" build_output.txt | head -10

echo.
echo === RESUMEN ===
set /a error_count=0
for /f %%i in ('findstr /c:"error CS" build_output.txt') do set /a error_count+=1
echo Total de errores: %error_count%

echo.
echo === ARCHIVO COMPLETO ===
echo Output completo guardado en: build_output.txt
echo Para ver todos los errores: type build_output.txt
echo.
pause