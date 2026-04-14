const { test, expect } = require('@playwright/test');

test.describe('Evaluaciones - Pruebas Completas', () => {
  
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

  test('01 - Debe cargar la página correctamente', async ({ page }) => {
    console.log('🔍 Test 01: Verificando carga de página');
    
    // Verificar título
    await expect(page).toHaveTitle(/Gestión de Evaluaciones/);
    
    // Verificar elementos principales
    await expect(page.locator('h1')).toContainText('Gestión de Evaluaciones');
    await expect(page.locator('#filtrosForm')).toBeVisible();
    
    // Verificar botones principales
    await expect(page.locator('a[href*="Create"]')).toBeVisible();
    await expect(page.locator('#btnEnviarTodas')).toBeVisible();
    
    console.log('✅ Test 01: Página cargada correctamente');
  });

  test('02 - Debe mostrar los filtros en cascada', async ({ page }) => {
    console.log('🔍 Test 02: Verificando filtros en cascada');
    
    // Verificar que todos los selects existen
    const selects = [
      '#grupoId',
      '#estudianteIdCascada', 
      '#materiaId',
      '#instrumentoEvaluacionId',
      '#rubricaIdCascada'
    ];
    
    for (const selector of selects) {
      await expect(page.locator(selector)).toBeVisible();
      console.log(`✅ Select ${selector} visible`);
    }
    
    // Verificar estado inicial
    const grupoOptions = await page.locator('#grupoId option').count();
    console.log(`📊 Opciones de grupos: ${grupoOptions}`);
    expect(grupoOptions).toBeGreaterThan(1); // Al menos la opción por defecto + grupos reales
    
    console.log('✅ Test 02: Filtros en cascada verificados');
  });

  test('03 - Debe cargar estudiantes al seleccionar grupo', async ({ page }) => {
    console.log('🔍 Test 03: Probando carga de estudiantes');
    
    // Obtener primer grupo disponible
    const primerGrupo = await page.locator('#grupoId option[value!=""]').first();
    if (await primerGrupo.count() > 0) {
      const grupoValue = await primerGrupo.getAttribute('value');
      const grupoText = await primerGrupo.textContent();
      
      console.log(`📋 Seleccionando grupo: ${grupoText} (ID: ${grupoValue})`);
      
      // Interceptar la llamada AJAX
      const responsePromise = page.waitForResponse(response => 
        response.url().includes('OnGetEstudiantesByGrupoAsync')
      );
      
      // Seleccionar grupo
      await page.selectOption('#grupoId', grupoValue);
      
      // Esperar respuesta AJAX
      try {
        const response = await responsePromise;
        console.log(`📞 Respuesta AJAX: ${response.status()}`);
        
        if (response.status() === 200) {
          console.log('✅ Endpoint de estudiantes responde correctamente');
          
          // Verificar que se cargaron opciones en estudiantes
          await page.waitForTimeout(1000); // Dar tiempo para que se procese la respuesta
          
          const estudianteOptions = await page.locator('#estudianteIdCascada option').count();
          console.log(`📊 Opciones de estudiantes después: ${estudianteOptions}`);
          
          // Verificar que no hay mensaje de error
          const tieneError = await page.locator('#estudianteIdCascada option:has-text("Error")').count();
          expect(tieneError).toBe(0);
          
        } else {
          console.log(`❌ Error en endpoint: ${response.status()}`);
        }
      } catch (error) {
        console.log(`⚠️ No se pudo capturar respuesta AJAX: ${error.message}`);
      }
      
      // Verificar que los grupos siguen ahí después de la selección
      const gruposPostSeleccion = await page.locator('#grupoId option[value!=""]').count();
      console.log(`📊 Grupos después de selección: ${gruposPostSeleccion}`);
      
      if (gruposPostSeleccion === 0) {
        console.log('❌ PROBLEMA: Los grupos se borraron');
        throw new Error('Los grupos se borraron después de la selección');
      } else {
        console.log('✅ Los grupos se mantuvieron correctamente');
      }
      
    } else {
      console.log('⚠️ No hay grupos disponibles para probar');
    }
    
    console.log('✅ Test 03: Carga de estudiantes verificada');
  });

  test('04 - Debe manejar errores AJAX correctamente', async ({ page }) => {
    console.log('🔍 Test 04: Probando manejo de errores AJAX');
    
    // Simular error 404 interceptando requests
    await page.route('**/OnGetEstudiantesByGrupoAsync*', route => {
      route.fulfill({
        status: 404,
        contentType: 'text/html',
        body: 'Not Found'
      });
    });
    
    // Seleccionar un grupo para disparar el error
    const primerGrupo = await page.locator('#grupoId option[value!=""]').first();
    if (await primerGrupo.count() > 0) {
      const grupoValue = await primerGrupo.getAttribute('value');
      
      console.log(`📋 Seleccionando grupo para probar error: ${grupoValue}`);
      await page.selectOption('#grupoId', grupoValue);
      
      // Esperar a que se procese el error
      await page.waitForTimeout(2000);
      
      // Verificar que se muestra mensaje de error
      const mensajeError = await page.locator('#estudianteIdCascada option:has-text("Error")').count();
      expect(mensajeError).toBeGreaterThan(0);
      console.log('✅ Mensaje de error mostrado correctamente');
      
      // Verificar que el select no queda disabled
      const isDisabled = await page.locator('#estudianteIdCascada').getAttribute('disabled');
      expect(isDisabled).toBeNull();
      console.log('✅ Select no queda bloqueado después del error');
    }
    
    console.log('✅ Test 04: Manejo de errores verificado');
  });

  test('05 - Debe validar el modo "Ver todo"', async ({ page }) => {
    console.log('🔍 Test 05: Probando modo "Ver todo"');
    
    // Activar modo "Ver todo"
    await page.check('#showAll');
    await page.waitForLoadState('networkidle');
    
    // Verificar que se muestra el badge de modo "Ver todo"
    await expect(page.locator('.badge:has-text("MODO VER TODO")')).toBeVisible();
    console.log('✅ Badge de modo "Ver todo" visible');
    
    // Verificar que los filtros están deshabilitados
    const filtrosDeshabilitados = await page.locator('#grupoId[disabled]').count();
    expect(filtrosDeshabilitados).toBe(1);
    console.log('✅ Filtros deshabilitados en modo "Ver todo"');
    
    // Verificar botón de activar filtros
    await expect(page.locator('a:has-text("Activar Filtros")')).toBeVisible();
    console.log('✅ Botón "Activar Filtros" visible');
    
    console.log('✅ Test 05: Modo "Ver todo" verificado');
  });

  test('06 - Debe mostrar estadísticas si hay evaluaciones', async ({ page }) => {
    console.log('🔍 Test 06: Verificando estadísticas');
    
    // Verificar si hay evaluaciones
    const hayEvaluaciones = await page.locator('table tbody tr').count() > 0;
    
    if (hayEvaluaciones) {
      console.log('📊 Hay evaluaciones, verificando estadísticas');
      
      // Verificar que las estadísticas están visibles
      await expect(page.locator('#estadisticasEnvio')).toBeVisible();
      await expect(page.locator('#totalEvaluaciones')).toBeVisible();
      await expect(page.locator('#evaluacionesCompletadas')).toBeVisible();
      
      // Verificar que los números son válidos
      const totalText = await page.locator('#totalEvaluaciones').textContent();
      const total = parseInt(totalText.trim());
      expect(total).toBeGreaterThan(0);
      console.log(`📊 Total de evaluaciones: ${total}`);
      
    } else {
      console.log('📊 No hay evaluaciones, verificando mensaje vacío');
      await expect(page.locator('.text-center:has-text("No hay evaluaciones")')).toBeVisible();
    }
    
    console.log('✅ Test 06: Estadísticas verificadas');
  });

  test('07 - Debe validar la funcionalidad de botones', async ({ page }) => {
    console.log('🔍 Test 07: Probando botones principales');
    
    // Verificar botón "Nueva Evaluación"
    const btnNueva = page.locator('a:has-text("Nueva Evaluación")');
    await expect(btnNueva).toBeVisible();
    console.log('✅ Botón "Nueva Evaluación" visible');
    
    // Verificar botón "Reportes"
    const btnReportes = page.locator('a:has-text("Reportes")');
    await expect(btnReportes).toBeVisible();
    console.log('✅ Botón "Reportes" visible');
    
    // Verificar botón "Filtrar"
    const btnFiltrar = page.locator('button:has-text("Filtrar")');
    await expect(btnFiltrar).toBeVisible();
    console.log('✅ Botón "Filtrar" visible');
    
    // Verificar botón "Limpiar Filtros"
    const btnLimpiar = page.locator('a:has-text("Limpiar Filtros")');
    await expect(btnLimpiar).toBeVisible();
    console.log('✅ Botón "Limpiar Filtros" visible');
    
    console.log('✅ Test 07: Botones verificados');
  });

  test('08 - Debe validar parámetros en la URL', async ({ page }) => {
    console.log('🔍 Test 08: Verificando parámetros URL');
    
    const url = page.url();
    console.log(`🌐 URL actual: ${url}`);
    
    // Verificar que no hay parámetros duplicados
    const urlObj = new URL(url);
    const params = Array.from(urlObj.searchParams.entries());
    const paramNames = params.map(([name]) => name);
    const duplicados = paramNames.filter((name, index) => paramNames.indexOf(name) !== index);
    
    console.log('📋 Parámetros encontrados:', params.map(([name, value]) => `${name}=${value}`));
    
    if (duplicados.length > 0) {
      console.log('❌ Parámetros duplicados encontrados:', [...new Set(duplicados)]);
      throw new Error(`Parámetros duplicados en URL: ${duplicados.join(', ')}`);
    } else {
      console.log('✅ No hay parámetros duplicados');
    }
    
    console.log('✅ Test 08: URL validada');
  });

  test('09 - Debe probar la cadena completa de filtros', async ({ page }) => {
    console.log('🔍 Test 09: Probando cadena completa de filtros');
    
    // Interceptar todas las llamadas AJAX
    const ajaxResponses = [];
    page.on('response', response => {
      if (response.url().includes('OnGet')) {
        ajaxResponses.push({
          url: response.url(),
          status: response.status()
        });
      }
    });
    
    // 1. Seleccionar grupo
    const primerGrupo = await page.locator('#grupoId option[value!=""]').first();
    if (await primerGrupo.count() > 0) {
      const grupoValue = await primerGrupo.getAttribute('value');
      console.log(`📋 Paso 1: Seleccionando grupo ${grupoValue}`);
      
      await page.selectOption('#grupoId', grupoValue);
      await page.waitForTimeout(2000);
      
      // 2. Seleccionar materia si está disponible
      const materiaOptions = await page.locator('#materiaId option[value!=""]').count();
      if (materiaOptions > 0) {
        const primerMateria = await page.locator('#materiaId option[value!=""]').first();
        const materiaValue = await primerMateria.getAttribute('value');
        console.log(`📚 Paso 2: Seleccionando materia ${materiaValue}`);
        
        await page.selectOption('#materiaId', materiaValue);
        await page.waitForTimeout(2000);
        
        // 3. Seleccionar instrumento si está disponible
        const instrumentoOptions = await page.locator('#instrumentoEvaluacionId option[value!=""]').count();
        if (instrumentoOptions > 0) {
          const primerInstrumento = await page.locator('#instrumentoEvaluacionId option[value!=""]').first();
          const instrumentoValue = await primerInstrumento.getAttribute('value');
          console.log(`🔧 Paso 3: Seleccionando instrumento ${instrumentoValue}`);
          
          await page.selectOption('#instrumentoEvaluacionId', instrumentoValue);
          await page.waitForTimeout(2000);
        }
      }
    }
    
    console.log('📞 Llamadas AJAX realizadas:', ajaxResponses);
    console.log('✅ Test 09: Cadena de filtros completada');
  });

  test('10 - Debe verificar la tabla de evaluaciones', async ({ page }) => {
    console.log('🔍 Test 10: Verificando tabla de evaluaciones');
    
    // Verificar que la tabla existe
    await expect(page.locator('table')).toBeVisible();
    
    // Verificar headers de la tabla
    const expectedHeaders = [
      'Grupo', 'Estudiante', 'Materia', 'Instrumento', 
      'Rúbrica', 'Período', 'Estado', 'Fecha', 
      'Total Puntos', 'Items Evaluados', 'Acciones', 'Enviar'
    ];
    
    for (const header of expectedHeaders) {
      await expect(page.locator(`th:has-text("${header}")`)).toBeVisible();
      console.log(`✅ Header "${header}" presente`);
    }
    
    // Contar filas de datos
    const filas = await page.locator('table tbody tr').count();
    console.log(`📊 Filas de evaluaciones: ${filas}`);
    
    if (filas > 0) {
      // Verificar que las filas tienen el contenido esperado
      const primeraFila = page.locator('table tbody tr').first();
      await expect(primeraFila.locator('td')).toHaveCount(12); // 12 columnas
      console.log('✅ Estructura de filas correcta');
      
      // Verificar botones de acción en la primera fila
      await expect(primeraFila.locator('a[title="Ver detalles"]')).toBeVisible();
      await expect(primeraFila.locator('a[title="Editar"]')).toBeVisible();
      await expect(primeraFila.locator('a[title="Eliminar"]')).toBeVisible();
      console.log('✅ Botones de acción presentes');
    }
    
    console.log('✅ Test 10: Tabla verificada');
  });
});

test.describe('Evaluaciones - Pruebas de Performance', () => {
  
  test('Performance - Tiempo de carga inicial', async ({ page }) => {
    console.log('⏱️ Midiendo tiempo de carga inicial');
    
    const startTime = Date.now();
    await page.goto('https://localhost:18163/Evaluaciones?showAll=false');
    await page.waitForLoadState('networkidle');
    const endTime = Date.now();
    
    const loadTime = endTime - startTime;
    console.log(`⏱️ Tiempo de carga: ${loadTime}ms`);
    
    // Verificar que la página carga en menos de 5 segundos
    expect(loadTime).toBeLessThan(5000);
    console.log('✅ Tiempo de carga aceptable');
  });

  test('Performance - Tiempo de respuesta AJAX', async ({ page }) => {
    console.log('⏱️ Midiendo tiempo de respuesta AJAX');
    
    await page.goto('https://localhost:18163/Evaluaciones?showAll=false');
    await page.waitForLoadState('networkidle');
    
    const primerGrupo = await page.locator('#grupoId option[value!=""]').first();
    if (await primerGrupo.count() > 0) {
      const grupoValue = await primerGrupo.getAttribute('value');
      
      const startTime = Date.now();
      
      // Configurar interceptor para medir tiempo de respuesta
      page.on('response', response => {
        if (response.url().includes('OnGetEstudiantesByGrupoAsync')) {
          const responseTime = Date.now() - startTime;
          console.log(`⏱️ Tiempo de respuesta AJAX: ${responseTime}ms`);
          expect(responseTime).toBeLessThan(3000); // Menos de 3 segundos
        }
      });
      
      await page.selectOption('#grupoId', grupoValue);
      await page.waitForTimeout(3000);
    }
    
    console.log('✅ Performance AJAX verificada');
  });
});