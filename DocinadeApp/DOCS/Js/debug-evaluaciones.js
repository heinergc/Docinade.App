const { chromium } = require('playwright');

async function debugEvaluaciones() {
  console.log('🚀 Debugeando filtros en cascada...');
  
  const browser = await chromium.launch({ 
    headless: false,
    devtools: true
  });
  
  const context = await browser.newContext({
    ignoreHTTPSErrors: true
  });
  
  const page = await context.newPage();
  
  // Interceptar respuestas AJAX
  page.on('response', response => {
    const url = response.url();
    if (url.includes('OnGetEstudiantesByGrupo') || url.includes('OnGetMateriasByGrupo')) {
      console.log(`📞 AJAX ${response.status()}: ${url}`);
    }
  });
  
  // Interceptar logs de consola del navegador
  page.on('console', msg => {
    const text = msg.text();
    if (text.includes('🔄') || text.includes('📋') || text.includes('❌') || text.includes('✅') || text.includes('Grupo seleccionado')) {
      console.log(`🌐 BROWSER: ${text}`);
    }
  });
  
  try {
    console.log('🌐 Navegando a la página...');
    await page.goto('https://localhost:18163/Evaluaciones?showAll=false');
    await page.waitForLoadState('networkidle');
    
    console.log('✅ Página cargada');
    console.log('🌐 URL actual:', page.url());
    
    // Verificar que los elementos básicos existen
    const grupoSelect = await page.$('#grupoId');
    const estudianteSelect = await page.$('#estudianteIdCascada');
    
    if (!grupoSelect) {
      console.log('❌ Select de grupos no encontrado');
      return;
    }
    
    if (!estudianteSelect) {
      console.log('❌ Select de estudiantes no encontrado');
      return;
    }
    
    console.log('✅ Elementos encontrados');
    
    // Obtener información sobre los grupos de forma más simple
    const grupoInfo = await page.evaluate(() => {
      const select = document.getElementById('grupoId');
      if (!select) return { error: 'Select no encontrado' };
      
      const options = Array.from(select.options);
      const gruposConValor = options.filter(opt => opt.value !== '');
      
      return {
        total: options.length,
        conValor: gruposConValor.length,
        grupos: gruposConValor.map(opt => ({ value: opt.value, text: opt.text }))
      };
    });
    
    console.log('📊 Información de grupos:', grupoInfo);
    
    // Obtener estado inicial de estudiantes
    const estudianteInfo = await page.evaluate(() => {
      const select = document.getElementById('estudianteIdCascada');
      if (!select) return { error: 'Select no encontrado' };
      
      const options = Array.from(select.options);
      return {
        total: options.length,
        opciones: options.map(opt => opt.text)
      };
    });
    
    console.log('📋 Estado inicial de estudiantes:', estudianteInfo);
    
    // Probar selección de grupo si hay grupos disponibles
    if (grupoInfo.conValor > 0) {
      const primerGrupo = grupoInfo.grupos[0];
      console.log(`🎯 Probando con grupo: ${primerGrupo.text} (ID: ${primerGrupo.value})`);
      
      // Interceptar específicamente la llamada AJAX que vamos a generar
      let ajaxResponse = null;
      page.on('response', response => {
        if (response.url().includes('OnGetEstudiantesByGrupoAsync')) {
          response.json().then(data => {
            ajaxResponse = data;
            console.log(`📊 Respuesta AJAX recibida:`, data);
          }).catch(() => {
            console.log(`📊 Respuesta AJAX no-JSON recibida`);
          });
        }
      });
      
      // Seleccionar el grupo
      await page.selectOption('#grupoId', primerGrupo.value);
      console.log('✅ Grupo seleccionado, esperando respuesta...');
      
      // Esperar a que complete las operaciones AJAX
      await page.waitForTimeout(5000);
      
      // Verificar estado después de la selección
      const estudianteInfoPost = await page.evaluate(() => {
        const select = document.getElementById('estudianteIdCascada');
        if (!select) return { error: 'Select no encontrado' };
        
        const options = Array.from(select.options);
        return {
          total: options.length,
          opciones: options.map(opt => opt.text),
          disabled: select.disabled
        };
      });
      
      console.log('📋 Estado de estudiantes DESPUÉS de selección:', estudianteInfoPost);
      
      // Verificar si los grupos siguen existiendo
      const grupoInfoPost = await page.evaluate(() => {
        const select = document.getElementById('grupoId');
        if (!select) return { error: 'Select no encontrado' };
        
        const options = Array.from(select.options);
        const gruposConValor = options.filter(opt => opt.value !== '');
        
        return {
          total: options.length,
          conValor: gruposConValor.length,
          valorSeleccionado: select.value
        };
      });
      
      console.log('📊 Estado de grupos DESPUÉS de selección:', grupoInfoPost);
      
      if (grupoInfoPost.conValor === 0) {
        console.log('❌ PROBLEMA DETECTADO: Los grupos se borraron después de la selección');
      } else {
        console.log('✅ Los grupos se mantuvieron correctamente');
      }
      
      // Verificar URL después del cambio
      console.log('🌐 URL después del cambio:', page.url());
      
      // Verificar información sobre la llamada AJAX
      if (ajaxResponse) {
        console.log(`📊 Datos AJAX procesados: ${Array.isArray(ajaxResponse) ? ajaxResponse.length : 'No es array'} elementos`);
      } else {
        console.log('⚠️ No se capturó respuesta AJAX');
      }
    } else {
      console.log('⚠️ No hay grupos disponibles para probar');
    }
    
    console.log('⏸️ Pausa para inspección manual (15 segundos)...');
    await page.waitForTimeout(15000);
    
  } catch (error) {
    console.log('💥 Error durante la ejecución:', error.message);
    console.log(error.stack);
  } finally {
    console.log('🔚 Cerrando navegador...');
    await browser.close();
  }
}

// Ejecutar el debug
debugEvaluaciones().catch(error => {
  console.error('💥 Error fatal:', error);
  process.exit(1);
});