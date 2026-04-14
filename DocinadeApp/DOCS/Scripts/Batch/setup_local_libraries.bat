@echo off
echo ================================================
echo   CONFIGURACIÓN DE LIBRERÍAS LOCALES
echo ================================================
cd /d "C:\Users\hguido\Desktop\RubricasApp\src\RubricasApp.Web"

echo.
echo === 1. LIMPIANDO LIBRERÍAS ANTERIORES ===
if exist "wwwroot\lib" (
    echo Eliminando directorio lib anterior...
    rmdir /s /q "wwwroot\lib"
    echo Directorio lib eliminado.
) else (
    echo Directorio lib no existe - OK
)

echo.
echo === 2. RESTAURANDO LIBRERÍAS CON LIBMAN ===
echo Ejecutando libman restore...
libman restore

if %ERRORLEVEL% EQU 0 (
    echo ✅ LibMan restore exitoso
) else (
    echo ❌ Error en LibMan restore
    echo.
    echo Instalando LibMan globalmente...
    dotnet tool install -g Microsoft.Web.LibraryManager.Cli
    echo.
    echo Reintentando restore...
    libman restore
)

echo.
echo === 3. VERIFICANDO ESTRUCTURA FINAL ===
echo.
echo jQuery:
if exist "wwwroot\lib\jquery\dist\jquery.min.js" (
    echo ✅ jQuery OK - wwwroot\lib\jquery\dist\jquery.min.js
    dir "wwwroot\lib\jquery\dist\jquery.min.js" | find ".js"
) else (
    echo ❌ jQuery NO encontrado
    if exist "wwwroot\lib\jquery" (
        echo Contenido de wwwroot\lib\jquery:
        dir "wwwroot\lib\jquery" /s /b
    )
)

echo.
echo Bootstrap CSS:
if exist "wwwroot\lib\bootstrap\dist\css\bootstrap.min.css" (
    echo ✅ Bootstrap CSS OK - wwwroot\lib\bootstrap\dist\css\bootstrap.min.css
    dir "wwwroot\lib\bootstrap\dist\css\bootstrap.min.css" | find ".css"
) else (
    echo ❌ Bootstrap CSS NO encontrado
    if exist "wwwroot\lib\bootstrap" (
        echo Contenido de wwwroot\lib\bootstrap:
        dir "wwwroot\lib\bootstrap" /s /b
    )
)

echo.
echo Bootstrap JS:
if exist "wwwroot\lib\bootstrap\dist\js\bootstrap.bundle.min.js" (
    echo ✅ Bootstrap JS OK - wwwroot\lib\bootstrap\dist\js\bootstrap.bundle.min.js
    dir "wwwroot\lib\bootstrap\dist\js\bootstrap.bundle.min.js" | find ".js"
) else (
    echo ❌ Bootstrap JS NO encontrado
    if exist "wwwroot\lib\bootstrap" (
        echo Contenido de wwwroot\lib\bootstrap:
        dir "wwwroot\lib\bootstrap" /s /b
    )
)

echo.
echo === 4. RESUMEN ===
if exist "wwwroot\lib\jquery\dist\jquery.min.js" if exist "wwwroot\lib\bootstrap\dist\css\bootstrap.min.css" if exist "wwwroot\lib\bootstrap\dist\js\bootstrap.bundle.min.js" (
    echo.
    echo 🎉 ¡CONFIGURACIÓN EXITOSA!
    echo ✅ Todas las librerías están instaladas correctamente
    echo ✅ Layout.cshtml configurado para usar librerías locales
    echo.
    echo SIGUIENTE PASO:
    echo 1. Ejecutar: dotnet run
    echo 2. Abrir navegador y presionar Ctrl+F5
    echo.
) else (
    echo.
    echo ❌ FALTAN ARCHIVOS
    echo Revisar errores arriba e intentar solución manual
    echo.
)

echo ================================================
echo   CONFIGURACIÓN COMPLETADA
echo ================================================
echo.
echo Presiona cualquier tecla para salir...
pause > nul