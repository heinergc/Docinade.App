@echo off
echo Ejecutando verificacion de errores de compilacion...
cd /d "C:\Users\hguido\Desktop\RubricasApp\src\RubricasApp.Web"
C:/ProgramData/anaconda3/Scripts/conda.exe run -p c:\Users\hguido\Desktop\RubricasApp\src\RubricasApp.Web\.conda --no-capture-output python check_build_errors.py
echo.
echo Presiona cualquier tecla para continuar...
pause > nul