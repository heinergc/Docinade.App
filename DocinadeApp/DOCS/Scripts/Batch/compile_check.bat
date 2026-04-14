@echo off
cd /d "C:\Users\hguido\Desktop\RubricasApp\src\RubricasApp.Web"
echo Compilando proyecto...
dotnet build --verbosity normal > build_output.txt 2>&1
echo Resultado compilacion: %ERRORLEVEL%
if %ERRORLEVEL% neq 0 (
    echo ERROR EN COMPILACION - Ver build_output.txt
    type build_output.txt | findstr /i "error\|Error\|ERROR"
) else (
    echo COMPILACION EXITOSA
)
pause