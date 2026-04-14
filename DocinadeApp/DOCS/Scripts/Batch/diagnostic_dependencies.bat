@echo off
echo ================================================
echo   DIAGNÓSTICO COMPLETO DE DEPENDENCIAS
echo ================================================
cd /d "C:\Users\hguido\Desktop\RubricasApp\src\RubricasApp.Web"

echo.
echo === 1. VERIFICANDO ARCHIVOS LIBMAN ===
if exist "libman.json" (
    echo [OK] libman.json existe
    echo Contenido:
    type "libman.json"
) else (
    echo [ERROR] libman.json NO encontrado
)

echo.
echo === 2. VERIFICANDO ESTRUCTURA DE LIBRERÍAS ===
echo Estructura de wwwroot/lib/:
if exist "wwwroot\lib" (
    echo [OK] Directorio lib existe
    dir "wwwroot\lib" /b
    
    echo.
    echo jQuery:
    if exist "wwwroot\lib\jquery" (
        dir "wwwroot\lib\jquery" /b
        echo Tamaño del archivo:
        dir "wwwroot\lib\jquery\jquery.min.js" | find "jquery.min.js"
    ) else (
        echo [ERROR] Directorio jquery NO existe
    )
    
    echo.
    echo Bootstrap:
    if exist "wwwroot\lib\bootstrap" (
        echo CSS:
        dir "wwwroot\lib\bootstrap\css" /b 2>nul || echo "  [ERROR] Directorio css no existe"
        echo JS:
        dir "wwwroot\lib\bootstrap\js" /b 2>nul || echo "  [ERROR] Directorio js no existe"
    ) else (
        echo [ERROR] Directorio bootstrap NO existe
    )
) else (
    echo [ERROR] Directorio lib NO existe
)

echo.
echo === 3. VERIFICANDO LAYOUT.CSHTML ===
if exist "Views\Shared\_Layout.cshtml" (
    echo [OK] _Layout.cshtml existe
    echo.
    echo Referencias encontradas:
    findstr /I "jquery\|bootstrap" "Views\Shared\_Layout.cshtml"
) else (
    echo [ERROR] _Layout.cshtml NO encontrado
)

echo.
echo === 4. VERIFICANDO SITE.JS ===
if exist "wwwroot\js\site.js" (
    echo [OK] site.js existe
    echo Tamaño:
    dir "wwwroot\js\site.js" | find "site.js"
) else (
    echo [ERROR] site.js NO encontrado
)

echo.
echo === 5. RECOMENDACIONES ===
echo Si hay errores 404:
echo 1. Ejecutar: libman restore
echo 2. Limpiar caché del navegador (Ctrl+F5)
echo 3. Verificar que el servidor esté sirviendo archivos estáticos
echo 4. Verificar permisos de archivos

echo.
echo ================================================
echo   DIAGNÓSTICO COMPLETADO
echo ================================================
echo.
echo Presiona cualquier tecla para salir...
pause > nul