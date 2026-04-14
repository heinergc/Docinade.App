const { chromium } = require('playwright');

async function verificarServidor() {
  console.log('🔍 Verificando si la aplicación está ejecutándose...');
  
  const browser = await chromium.launch({ headless: true });
  const context = await browser.newContext({ ignoreHTTPSErrors: true });
  const page = await context.newPage();
  
  try {
    const response = await page.goto('https://localhost:18163/Evaluaciones?showAll=false', {
      waitUntil: 'domcontentloaded',
      timeout: 10000
    });
    
    if (response.ok()) {
      console.log('✅ Aplicación disponible en https://localhost:18163');
      console.log(`📊 Status: ${response.status()}`);
      
      // Verificar algunos elementos básicos
      const titulo = await page.title();
      console.log(`📄 Título: ${titulo}`);
      
      const hasTable = await page.locator('table').isVisible();
      const hasFilters = await page.locator('#filtrosForm').isVisible();
      
      console.log(`📋 Tabla presente: ${hasTable}`);
      console.log(`🔧 Filtros presentes: ${hasFilters}`);
      
      return true;
    } else {
      console.log(`❌ Aplicación responde con error: ${response.status()}`);
      return false;
    }
  } catch (error) {
    console.log(`❌ Error conectando a la aplicación: ${error.message}`);
    return false;
  } finally {
    await browser.close();
  }
}

// Ejecutar verificación
verificarServidor().then(disponible => {
  if (disponible) {
    console.log('🚀 La aplicación está lista para las pruebas');
    console.log('👉 Ejecute: npx playwright test tests/evaluaciones-comprehensive.spec.js --headed');
  } else {
    console.log('⚠️ La aplicación no está disponible');
    console.log('👉 Inicie la aplicación primero con: dotnet run');
  }
}).catch(console.error);