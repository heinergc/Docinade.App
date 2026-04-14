const { chromium } = require('playwright');

async function verificarEndpoints() {
  console.log('🔍 Verificando endpoints AJAX corregidos...');
  
  const browser = await chromium.launch({ headless: true });
  const context = await browser.newContext({ ignoreHTTPSErrors: true });
  const page = await context.newPage();
  
  try {
    // Probar endpoints corregidos
    const endpoints = [
      'https://localhost:18163/Evaluaciones/GetEstudiantesByGrupo?grupoId=1',
      'https://localhost:18163/Evaluaciones/GetMateriasByGrupo?grupoId=1',
      'https://localhost:18163/Evaluaciones/GetInstrumentosByMateria?materiaId=1',
      'https://localhost:18163/Evaluaciones/GetRubricasByInstrumento?instrumentoEvaluacionId=1'
    ];
    
    for (const endpoint of endpoints) {
      console.log(`\n📞 Probando: ${endpoint}`);
      
      try {
        const response = await page.goto(endpoint, { 
          waitUntil: 'networkidle',
          timeout: 5000 
        });
        
        console.log(`   Status: ${response.status()}`);
        
        if (response.status() === 200) {
          const content = await page.content();
          if (content.includes('[') || content.includes('{')) {
            console.log('   ✅ Respuesta JSON válida');
            
            // Intentar parsear el JSON para verificar estructura
            try {
              const jsonMatch = content.match(/\[.*\]/);
              if (jsonMatch) {
                const data = JSON.parse(jsonMatch[0]);
                console.log(`   📊 ${Array.isArray(data) ? data.length : 'No-array'} elementos`);
              }
            } catch (parseError) {
              console.log('   ⚠️ JSON válido pero no parseable en preview');
            }
          } else {
            console.log('   ⚠️ Respuesta no parece JSON');
          }
        } else if (response.status() === 404) {
          console.log('   ❌ Endpoint no encontrado (404)');
        } else {
          console.log('   ⚠️ Respuesta inesperada');
        }
        
      } catch (error) {
        console.log(`   💥 Error: ${error.message}`);
      }
    }

    // También probar endpoints legacy para compatibilidad
    console.log('\n🔄 Probando endpoints legacy...');
    const legacyEndpoints = [
      'https://localhost:18163/Evaluaciones/OnGetEstudiantesByGrupoAsync?grupoId=1',
      'https://localhost:18163/Evaluaciones/OnGetMateriasByGrupoAsync?grupoId=1'
    ];

    for (const endpoint of legacyEndpoints) {
      console.log(`\n📞 Legacy: ${endpoint}`);
      
      try {
        const response = await page.goto(endpoint, { 
          waitUntil: 'networkidle',
          timeout: 5000 
        });
        
        console.log(`   Status: ${response.status()}`);
        
        if (response.status() === 200) {
          console.log('   ✅ Legacy endpoint funciona');
        } else {
          console.log('   ❌ Legacy endpoint falla');
        }
        
      } catch (error) {
        console.log(`   💥 Error legacy: ${error.message}`);
      }
    }
    
  } finally {
    await browser.close();
  }
}

// Ejecutar verificación
verificarEndpoints().catch(console.error);