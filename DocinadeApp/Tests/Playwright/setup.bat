@echo off
REM Script para instalar y ejecutar pruebas de Playwright en Windows
REM Asegúrate de tener Node.js instalado

echo 🚀 Configurando pruebas de Playwright para RubricasApp...

REM Navegar al directorio de pruebas
cd /d "%~dp0"

REM Verificar si Node.js está instalado
node --version >nul 2>&1
if %errorlevel% neq 0 (
    echo ❌ Node.js no está instalado. Por favor instala Node.js primero.
    echo Descarga desde: https://nodejs.org/
    pause
    exit /b 1
)

REM Verificar si npm está instalado
npm --version >nul 2>&1
if %errorlevel% neq 0 (
    echo ❌ npm no está instalado. Por favor instala npm primero.
    pause
    exit /b 1
)

echo ✅ Node.js y npm detectados

REM Instalar dependencias
echo 📦 Instalando dependencias...
npm install

REM Instalar navegadores de Playwright
echo 🌐 Instalando navegadores de Playwright...
npx playwright install

REM Verificar instalación
echo 🔍 Verificando instalación de Playwright...
npx playwright --version

echo.
echo 🎉 ¡Configuración completada!
echo.
echo Para ejecutar las pruebas:
echo   npm test                     # Ejecutar todas las pruebas
echo   npm run test:headed          # Ejecutar con navegador visible
echo   npm run test:debug           # Ejecutar en modo debug
echo   npm run test:asignacion      # Ejecutar solo pruebas de asignación
echo   npm run report               # Ver reporte de resultados
echo.
echo Nota: Asegúrate de que la aplicación ASP.NET esté ejecutándose en https://localhost:18163
pause
