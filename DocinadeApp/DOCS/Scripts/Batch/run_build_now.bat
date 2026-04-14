@echo off
echo Ejecutando build verification...
cd /d "C:\Users\hguido\Desktop\RubricasApp\src\RubricasApp.Web"
C:/ProgramData/anaconda3/Scripts/conda.exe run -p c:\Users\hguido\Desktop\RubricasApp\src\RubricasApp.Web\.conda --no-capture-output python build_now.py
pause