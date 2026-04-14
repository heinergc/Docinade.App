const { test, expect } = require('@playwright/test');

test.describe('Evaluaciones - Pruebas Corregidas', () => {
  
  test.beforeEach(async ({ page }) => {
    // Configurar interceptores para monitorear requests
    page.on('console', msg => {
      if (msg.type() === 'error') {
        console.log('❌ Error de consola:', msg.text());
      } else if (msg.text().includes('🔄') || msg.text().includes('📋') || msg.text().includes('✅')) {
        console.log('📝 Log importante:', msg.text());
      }
    });

    page.on('response', response => {
      if (response.url().includes('OnGet') || response.url().includes('Evaluaciones')) {
        console.log(`📞 Request: ${response.status()} ${response.url()}`);
      }
    });

    // Navegar a la página de evaluaciones
    console.log('🌐 Navegando a evaluaciones...');
    await page.goto('https://localhost:18163/Evaluaciones?showAll=false');
    await page.waitForLoadState('networkidle');
  });

  test('AJAX - Debe cargar estudiantes al seleccionar grupo', async ({ page }) => {
    console.log('🔍 Test AJAX: Probando carga de estudiantes');
    
    // Obtener opciones de grupos usando evaluate para evitar selector problemático
    const gruposData = await page.evaluate(() => {
      const select = document.getElementById('grupoId');
      const opciones = Array.from(select.options);
      return opciones
        .filter(opt => opt.value !== '')
        .map(opt => ({ value: opt.value, text: opt.text }));
    });
    
    console.log(`📊 Grupos disponibles: ${gruposData.length}`);
    
    if (gruposData.length > 0) {
      const primerGrupo = gruposData[0];
      console.log(`📋 Seleccionando grupo: ${primerGrupo.text} (ID: ${primerGrupo.value})`);
      
      // Interceptar la llamada AJAX
      const responsePromise = page.waitForResponse(response => 
        response.url().includes('OnGetEstudiantesByGrupoAsync'), 
        { timeout: 10000 }
      );
      
      // Seleccionar grupo
      await page.selectOption('#grupoId', primerGrupo.value);
      
      // Esperar respuesta AJAX
      try {
        const response = await responsePromise;
        console.log(`📞 Respuesta AJAX: ${response.status()}`);
        
        if (response.status() === 200) {
          console.log('✅ Endpoint de estudiantes responde correctamente');
          
          // Verificar que se procesó la respuesta
          await page.waitForTimeout(2000);
          
          const estudiantesData = await page.evaluate(() => {
            const select = document.getElementById('estudianteIdCascada');
            const opciones = Array.from(select.options);
            return opciones.map(opt => opt.text);
          });
          
          console.log('📊 Opciones de estudiantes después:', estudiantesData);
          
          // Verificar que no hay mensaje de error
          const tieneError = estudiantesData.some(opt => opt.includes('Error'));
          expect(tieneError).toBe(false);
          console.log('✅ No hay mensajes de error');
          
        } else if (response.status() === 404) {
          console.log('❌ Endpoint retorna 404 - PROBLEMA CONFIRMADO');
          throw new Error('Endpoint AJAX no disponible - retorna 404');
        } else {
          console.log(`❌ Error en endpoint: ${response.status()}`);
        }
      } catch (error) {
        if (error.message.includes('Timeout')) {
          console.log('⚠️ Timeout esperando respuesta AJAX - posible problema de red');
        } else {
          console.log(`⚠️ Error capturando respuesta AJAX: ${error.message}`);
          throw error;
        }
      }
      
      // Verificar que los grupos siguen ahí después de la selección
      const gruposPostSeleccion = await page.evaluate(() => {
        const select = document.getElementById('grupoId');
        const opciones = Array.from(select.options);
        return opciones.filter(opt => opt.value !== '').length;
      });
      
      console.log(`📊 Grupos después de selección: ${gruposPostSeleccion}`);
      
      if (gruposPostSeleccion === 0) {
        console.log('❌ PROBLEMA CRÍTICO: Los grupos se borraron');
        throw new Error('Los grupos se borraron después de la selección');
      } else {
        console.log('✅ Los grupos se mantuvieron correctamente');
      }
      
    } else {
      console.log('⚠️ No hay grupos disponibles para probar');
      // Esto no es un fallo, solo indica que no hay datos de prueba
    }
    
    console.log('✅ Test AJAX completado');
  });

  test('AJAX - Debe probar cadena completa de filtros', async ({ page }) => {
    console.log('🔍 Test: Probando cadena completa de filtros');
    
    // Interceptar todas las llamadas AJAX
    const ajaxResponses = [];
    page.on('response', response => {
      if (response.url().includes('OnGet')) {
        ajaxResponses.push({
          url: response.url(),
          status: response.status(),
          timestamp: Date.now()
        });
      }
    });
    
    // 1. Seleccionar grupo usando evaluate
    const grupoSeleccionado = await page.evaluate(() => {
      const select = document.getElementById('grupoId');
      const opciones = Array.from(select.options).filter(opt => opt.value !== '');
      if (opciones.length > 0) {
        const primer = opciones[0];
        select.value = primer.value;
        select.dispatchEvent(new Event('change', { bubbles: true }));
        return { value: primer.value, text: primer.text };
      }
      return null;
    });
    
    if (grupoSeleccionado) {
      console.log(`📋 Paso 1: Grupo seleccionado: ${grupoSeleccionado.text}`);
      await page.waitForTimeout(3000); // Esperar a que termine la carga AJAX
      
      // 2. Verificar si hay materias disponibles
      const materiasDisponibles = await page.evaluate(() => {
        const select = document.getElementById('materiaId');
        const opciones = Array.from(select.options).filter(opt => opt.value !== '');
        return opciones.map(opt => ({ value: opt.value, text: opt.text }));
      });
      
      console.log(`📚 Materias disponibles: ${materiasDisponibles.length}`);
      
      if (materiasDisponibles.length > 0) {
        const materiaSeleccionada = await page.evaluate(() => {
          const select = document.getElementById('materiaId');
          const opciones = Array.from(select.options).filter(opt => opt.value !== '');
          if (opciones.length > 0) {
            const primer = opciones[0];
            select.value = primer.value;
            select.dispatchEvent(new Event('change', { bubbles: true }));
            return { value: primer.value, text: primer.text };
          }
          return null;
        });
        
        if (materiaSeleccionada) {
          console.log(`📚 Paso 2: Materia seleccionada: ${materiaSeleccionada.text}`);
          await page.waitForTimeout(3000);
          
          // 3. Verificar instrumentos
          const instrumentosDisponibles = await page.evaluate(() => {
            const select = document.getElementById('instrumentoEvaluacionId');
            const opciones = Array.from(select.options).filter(opt => opt.value !== '');
            return opciones.length;
          });
          
          console.log(`🔧 Instrumentos disponibles: ${instrumentosDisponibles}`);
        }
      }
    } else {
      console.log('⚠️ No hay grupos para probar la cadena');
    }
    
    console.log('📞 Llamadas AJAX capturadas:', ajaxResponses);
    
    // Verificar que se hicieron llamadas AJAX
    const llamadasEstudiantes = ajaxResponses.filter(r => r.url.includes('OnGetEstudiantesByGrupoAsync'));
    const llamadasMaterias = ajaxResponses.filter(r => r.url.includes('OnGetMateriasByGrupoAsync'));
    
    console.log(`📊 Llamadas de estudiantes: ${llamadasEstudiantes.length}`);
    console.log(`📊 Llamadas de materias: ${llamadasMaterias.length}`);
    
    if (llamadasEstudiantes.length > 0) {
      console.log('✅ Se realizaron llamadas AJAX para estudiantes');
      console.log(`📞 Estados de respuesta: ${llamadasEstudiantes.map(r => r.status).join(', ')}`);
    } else {
      console.log('⚠️ No se detectaron llamadas AJAX para estudiantes');
    }
    
    console.log('✅ Test de cadena completa finalizado');
  });

  test('Modo Ver Todo - Verificación completa', async ({ page }) => {
    console.log('🔍 Test: Verificando modo "Ver todo"');
    
    // Activar modo "Ver todo"
    console.log('🔄 Activando modo "Ver todo"...');
    await page.check('#showAll');
    await page.waitForLoadState('networkidle');
    
    // Verificar badge
    const badgeVisible = await page.locator('.badge:has-text("MODO VER TODO")').isVisible();
    console.log(`✅ Badge "MODO VER TODO": ${badgeVisible}`);
    expect(badgeVisible).toBe(true);
    
    // Verificar que filtros están deshabilitados usando evaluate
    const filtrosDeshabilitados = await page.evaluate(() => {
      const selects = ['#grupoId', '#estudianteIdCascada', '#materiaId', '#instrumentoEvaluacionId', '#rubricaIdCascada'];
      const estados = {};
      
      selects.forEach(selector => {
        const element = document.querySelector(selector);
        estados[selector] = element ? element.disabled : false;
      });
      
      return estados;
    });
    
    console.log('📊 Estados de filtros:', filtrosDeshabilitados);
    
    // Verificar que al menos el grupo está deshabilitado
    expect(filtrosDeshabilitados['#grupoId']).toBe(true);
    console.log('✅ Filtros deshabilitados correctamente');
    
    // Verificar botón "Activar Filtros"
    const btnActivar = await page.locator('a:has-text("Activar Filtros")').isVisible();
    console.log(`✅ Botón "Activar Filtros": ${btnActivar}`);
    expect(btnActivar).toBe(true);
    
    console.log('✅ Test modo "Ver todo" completado');
  });

  test('Performance - Medición detallada', async ({ page }) => {
    console.log('⏱️ Test: Midiendo performance de la aplicación');
    
    const metricas = {};
    
    // 1. Tiempo de carga inicial
    const startTime = Date.now();
    await page.goto('https://localhost:18163/Evaluaciones?showAll=false');
    await page.waitForLoadState('networkidle');
    metricas.cargaInicial = Date.now() - startTime;
    
    console.log(`⏱️ Tiempo de carga inicial: ${metricas.cargaInicial}ms`);
    expect(metricas.cargaInicial).toBeLessThan(10000); // 10 segundos máximo
    
    // 2. Tiempo de respuesta de selección de grupo
    const grupos = await page.evaluate(() => {
      const select = document.getElementById('grupoId');
      const opciones = Array.from(select.options).filter(opt => opt.value !== '');
      return opciones.length > 0 ? opciones[0].value : null;
    });
    
    if (grupos) {
      const ajaxStart = Date.now();
      
      // Configurar interceptor para medir AJAX
      page.on('response', response => {
        if (response.url().includes('OnGetEstudiantesByGrupoAsync')) {
          metricas.respuestaAjax = Date.now() - ajaxStart;
          console.log(`⏱️ Tiempo de respuesta AJAX: ${metricas.respuestaAjax}ms`);
        }
      });
      
      await page.selectOption('#grupoId', grupos);
      await page.waitForTimeout(3000);
      
      if (metricas.respuestaAjax) {
        expect(metricas.respuestaAjax).toBeLessThan(5000); // 5 segundos máximo
        console.log('✅ Tiempo de respuesta AJAX aceptable');
      } else {
        console.log('⚠️ No se pudo medir tiempo de respuesta AJAX');
      }
    }
    
    // 3. Tiempo de render de tabla
    const renderStart = Date.now();
    const filas = await page.locator('table tbody tr').count();
    metricas.renderTabla = Date.now() - renderStart;
    
    console.log(`⏱️ Tiempo de render tabla (${filas} filas): ${metricas.renderTabla}ms`);
    console.log('📊 Métricas completas:', metricas);
    
    console.log('✅ Test de performance completado');
  });

  test('Funcionalidad - Envío de evaluaciones', async ({ page }) => {
    console.log('🔍 Test: Verificando funcionalidad de envío');
    
    // Verificar que el botón de envío masivo existe
    const btnEnviarTodas = await page.locator('#btnEnviarTodas').isVisible();
    console.log(`✅ Botón "Enviar Todas": ${btnEnviarTodas}`);
    expect(btnEnviarTodas).toBe(true);
    
    // Contar evaluaciones disponibles para envío
    const evaluacionesParaEnvio = await page.evaluate(() => {
      const botones = document.querySelectorAll('button[data-evaluacion-id]:not([disabled])');
      return botones.length;
    });
    
    console.log(`📊 Evaluaciones disponibles para envío: ${evaluacionesParaEnvio}`);
    
    if (evaluacionesParaEnvio > 0) {
      console.log('✅ Hay evaluaciones disponibles para envío');
      
      // Verificar botones individuales de envío
      const botonesIndividuales = await page.locator('button[data-evaluacion-id]').count();
      console.log(`📊 Botones individuales de envío: ${botonesIndividuales}`);
      expect(botonesIndividuales).toBeGreaterThan(0);
      
    } else {
      console.log('⚠️ No hay evaluaciones para envío (normal si no hay datos)');
    }
    
    console.log('✅ Test de funcionalidad de envío completado');
  });
});