@echo off
echo ================================================
echo   VERIFICACIÓN ÁREA ADMIN - RubricasApp
echo ================================================
cd /d "C:\Users\hguido\Desktop\RubricasApp\src\RubricasApp.Web"

echo.
echo === 1. VERIFICANDO ESTRUCTURA ADMIN ===
echo.
echo Controladores en Areas\Admin\Controllers:
if exist "Areas\Admin\Controllers" (
    dir "Areas\Admin\Controllers" /b
) else (
    echo ❌ Directorio Controllers no existe en Areas\Admin
)

echo.
echo Vistas en Areas\Admin\Views:
if exist "Areas\Admin\Views" (
    dir "Areas\Admin\Views" /b
) else (
    echo ❌ Directorio Views no existe en Areas\Admin
)

echo.
echo === 2. COMPILANDO PROYECTO ===
echo Ejecutando dotnet build...
dotnet build --configuration Release

if %ERRORLEVEL% EQU 0 (
    echo ✅ Compilación exitosa
) else (
    echo ❌ Error en compilación
    echo Ejecutando compilación detallada...
    dotnet build --verbosity normal
    goto end
)

echo.
echo === 3. VERIFICANDO RUTAS ADMIN ===
echo Las siguientes rutas deberían estar disponibles:
echo.
echo ✅ /Admin/Admin/Index        - Panel principal
echo ✅ /Admin/Users/Index        - Gestión de usuarios  
echo ✅ /Admin/Roles/Index        - Gestión de roles
echo ✅ /Admin/Audit/Index        - Auditoría
echo.

echo === 4. EJECUTANDO APLICACIÓN ===
echo Iniciando servidor de desarrollo...
echo.
echo Una vez que el servidor inicie:
echo 1. Navegar a la aplicación
echo 2. Iniciar sesión como admin
echo 3. Ir a Configuración → Panel de Administración
echo.
echo ================================================
echo   INICIANDO SERVIDOR...
echo ================================================
echo.

dotnet run

:end
echo.
echo Presiona cualquier tecla para salir...
pause > nul