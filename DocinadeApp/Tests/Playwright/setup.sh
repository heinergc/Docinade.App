#!/bin/bash

# Script para instalar y ejecutar pruebas de Playwright
# Asegúrate de tener Node.js instalado

echo "🚀 Configurando pruebas de Playwright para RubricasApp..."

# Navegar al directorio de pruebas
cd "$(dirname "$0")"

# Verificar si Node.js está instalado
if ! command -v node &> /dev/null; then
    echo "❌ Node.js no está instalado. Por favor instala Node.js primero."
    echo "Descarga desde: https://nodejs.org/"
    exit 1
fi

# Verificar si npm está instalado
if ! command -v npm &> /dev/null; then
    echo "❌ npm no está instalado. Por favor instala npm primero."
    exit 1
fi

echo "✅ Node.js y npm detectados"

# Instalar dependencias
echo "📦 Instalando dependencias..."
npm install

# Instalar navegadores de Playwright
echo "🌐 Instalando navegadores de Playwright..."
npx playwright install

# Verificar instalación
echo "🔍 Verificando instalación de Playwright..."
npx playwright --version

echo ""
echo "🎉 ¡Configuración completada!"
echo ""
echo "Para ejecutar las pruebas:"
echo "  npm test                     # Ejecutar todas las pruebas"
echo "  npm run test:headed          # Ejecutar con navegador visible"
echo "  npm run test:debug           # Ejecutar en modo debug"
echo "  npm run test:asignacion      # Ejecutar solo pruebas de asignación"
echo "  npm run report               # Ver reporte de resultados"
echo ""
echo "Nota: Asegúrate de que la aplicación ASP.NET esté ejecutándose en https://localhost:18163"
