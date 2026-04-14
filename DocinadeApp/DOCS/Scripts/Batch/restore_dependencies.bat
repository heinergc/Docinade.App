@echo off
echo === Restaurando dependencias de LibMan ===
cd /d "C:\Users\hguido\Desktop\RubricasApp\src\RubricasApp.Web"

echo.
echo Restaurando librerías...
libman restore

if %ERRORLEVEL% EQU 0 (
    echo.
    echo === Verificando archivos ===
    if exist "wwwroot\lib\jquery\jquery.min.js" (
        echo [OK] jQuery encontrado
    ) else (
        echo [ERROR] jQuery NO encontrado
    )
    
    if exist "wwwroot\lib\bootstrap\css\bootstrap.min.css" (
        echo [OK] Bootstrap CSS encontrado
    ) else (
        echo [ERROR] Bootstrap CSS NO encontrado
    )
    
    if exist "wwwroot\lib\bootstrap\js\bootstrap.bundle.min.js" (
        echo [OK] Bootstrap JS encontrado
    ) else (
        echo [ERROR] Bootstrap JS NO encontrado
    )
    
    echo.
    echo === Listando contenido de las librerías ===
    echo jQuery:
    dir "wwwroot\lib\jquery" /b
    echo.
    echo Bootstrap CSS:
    dir "wwwroot\lib\bootstrap\css" /b
    echo.
    echo Bootstrap JS:
    dir "wwwroot\lib\bootstrap\js" /b
    
    echo.
    echo ✅ LibMan restore completado exitosamente
) else (
    echo ❌ Error al ejecutar LibMan restore
)

echo.
echo Presiona cualquier tecla para continuar...
pause > nul