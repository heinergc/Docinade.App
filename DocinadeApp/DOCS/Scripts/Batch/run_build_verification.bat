@echo off
echo ===========================================
echo    EJECUTANDO VERIFICACION DE BUILD
echo ===========================================
echo.

cd /d "C:\Users\hguido\Desktop\RubricasApp\src\RubricasApp.Web"

echo Usando entorno Python Conda...
echo.

C:/ProgramData/anaconda3/Scripts/conda.exe run -p c:\Users\hguido\Desktop\RubricasApp\src\RubricasApp.Web\.conda --no-capture-output python build_verification.py

echo.
echo ===========================================
echo            PROCESO COMPLETADO
echo ===========================================
echo.
echo Presiona cualquier tecla para continuar...
pause > nul