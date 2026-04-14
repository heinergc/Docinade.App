@echo off
echo Restaurando paquetes y compilando...
cd /d "C:\Users\hguido\Desktop\RubricasApp\src\RubricasApp.Web"

echo.
echo === DOTNET RESTORE ===
dotnet restore

echo.
echo === DOTNET BUILD ===
dotnet build --no-restore

echo.
echo Compilacion completa.
pause