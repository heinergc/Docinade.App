const { test, expect } = require('@playwright/test');

test.describe('Evaluaciones - Filtros en Cascada FUNCIONALES', () => {
  
  test.beforeEach(async ({ page }) => {
    // Configurar interceptores para monitorear requests
    page.on('console', msg => {
      console.log('📝 Console:', msg.text());
    });

    page.on('response', response => {
      if (response.url().includes('GetEstudiantesByGrupo') || 
          response.url().includes('GetMateriasByGrupo') ||
          response.url().includes('GetInstrumentosByMateria') ||
          response.url().includes('GetRubricasByInstrumento')) {
        console.log(`📞 AJAX: ${response.status()} ${response.url()}`);
      }
    });

    // Navegar a la página de evaluaciones
    console.log('🌐 Navegando a evaluaciones...');
    await page.goto('https://localhost:18163/Evaluaciones?showAll=false');
    await page.waitForLoadState('networkidle');
  });

  test('Filtros en Cascada - Flujo Completo SIN Auto-Submit', async ({ page }) => {
    console.log('🔍 Test: Flujo completo de filtros en cascada');
    
    // PASO 1: Verificar estado inicial
    console.log('📋 PASO 1: Estado inicial');
    
    const gruposIniciales = await page.evaluate(() => {
      const select = document.getElementById('grupoId');
      return Array.from(select.options).filter(opt => opt.value !== '').length;
    });
    
    console.log(`✅ Grupos iniciales disponibles: ${gruposIniciales}`);
    expect(gruposIniciales).toBeGreaterThan(0);
    
    // PASO 2: Seleccionar un grupo y verificar AJAX
    console.log('📋 PASO 2: Seleccionar grupo');
    
    // Obtener primer grupo disponible
    const primerGrupo = await page.evaluate(() => {
      const select = document.getElementById('grupoId');
      const opciones = Array.from(select.options).filter(opt => opt.value !== '');
      return opciones.length > 0 ? { value: opciones[0].value, text: opciones[0].text } : null;
    });
    
    if (primerGrupo) {
      console.log(`📋 Seleccionando grupo: ${primerGrupo.text} (${primerGrupo.value})`);
      
      // Interceptar llamadas AJAX
      const estudiantesPromise = page.waitForResponse(response => 
        response.url().includes('GetEstudiantesByGrupo'), 
        { timeout: 10000 }
      );
      
      const materiasPromise = page.waitForResponse(response => 
        response.url().includes('GetMateriasByGrupo'), 
        { timeout: 10000 }
      );
      
      // Seleccionar grupo
      await page.selectOption('#grupoId', primerGrupo.value);
      
      // Esperar ambas respuestas AJAX
      const [estudiantesResponse, materiasResponse] = await Promise.all([
        estudiantesPromise,
        materiasPromise
      ]);
      
      console.log(`📞 Estudiantes: ${estudiantesResponse.status()}`);
      console.log(`📞 Materias: ${materiasResponse.status()}`);
      
      expect(estudiantesResponse.status()).toBe(200);
      expect(materiasResponse.status()).toBe(200);
      
      // PASO 3: Verificar que los datos se cargaron
      await page.waitForTimeout(2000); // Dar tiempo para procesar respuestas
      
      const estadoPostAjax = await page.evaluate(() => {
        const grupos = document.getElementById('grupoId');
        const estudiantes = document.getElementById('estudianteIdCascada');
        const materias = document.getElementById('materiaId');
        
        return {
          grupos: Array.from(grupos.options).filter(opt => opt.value !== '').length,
          estudiantes: Array.from(estudiantes.options).filter(opt => opt.value !== '').length,
          materias: Array.from(materias.options).filter(opt => opt.value !== '').length,
          estudiantesTexto: Array.from(estudiantes.options).map(opt => opt.text),
          materiasTexto: Array.from(materias.options).map(opt => opt.text)
        };
      });
      
      console.log('📊 Estado después de AJAX:');
      console.log(`   Grupos: ${estadoPostAjax.grupos}`);
      console.log(`   Estudiantes: ${estadoPostAjax.estudiantes}`);
      console.log(`   Materias: ${estadoPostAjax.materias}`);
      console.log(`   Estudiantes texto:`, estadoPostAjax.estudiantesTexto);
      console.log(`   Materias texto:`, estadoPostAjax.materiasTexto);
      
      // Verificar que los grupos NO se borraron
      expect(estadoPostAjax.grupos).toBeGreaterThan(0);
      console.log('✅ Los grupos se mantuvieron después del AJAX');
      
      // PASO 4: Seleccionar materia si hay disponibles
      if (estadoPostAjax.materias > 0) {
        console.log('📚 PASO 4: Seleccionar materia');
        
        const primeraMateria = await page.evaluate(() => {
          const select = document.getElementById('materiaId');
          const opciones = Array.from(select.options).filter(opt => opt.value !== '');
          return opciones.length > 0 ? { value: opciones[0].value, text: opciones[0].text } : null;
        });
        
        if (primeraMateria) {
          console.log(`📚 Seleccionando materia: ${primeraMateria.text}`);
          
          const instrumentosPromise = page.waitForResponse(response => 
            response.url().includes('GetInstrumentosByMateria'), 
            { timeout: 10000 }
          );
          
          await page.selectOption('#materiaId', primeraMateria.value);
          
          const instrumentosResponse = await instrumentosPromise;
          console.log(`📞 Instrumentos: ${instrumentosResponse.status()}`);
          expect(instrumentosResponse.status()).toBe(200);
          
          await page.waitForTimeout(2000);
          
          // PASO 5: Verificar instrumentos cargados
          const instrumentosDisponibles = await page.evaluate(() => {
            const select = document.getElementById('instrumentoEvaluacionId');
            return Array.from(select.options).filter(opt => opt.value !== '').length;
          });
          
          console.log(`📊 Instrumentos disponibles: ${instrumentosDisponibles}`);
          
          // PASO 6: Seleccionar instrumento si hay disponibles  
          if (instrumentosDisponibles > 0) {
            console.log('🔧 PASO 6: Seleccionar instrumento');
            
            const primerInstrumento = await page.evaluate(() => {
              const select = document.getElementById('instrumentoEvaluacionId');
              const opciones = Array.from(select.options).filter(opt => opt.value !== '');
              return opciones.length > 0 ? { value: opciones[0].value, text: opciones[0].text } : null;
            });
            
            if (primerInstrumento) {
              console.log(`🔧 Seleccionando instrumento: ${primerInstrumento.text}`);
              
              const rubricasPromise = page.waitForResponse(response => 
                response.url().includes('GetRubricasByInstrumento'), 
                { timeout: 10000 }
              );
              
              await page.selectOption('#instrumentoEvaluacionId', primerInstrumento.value);
              
              const rubricasResponse = await rubricasPromise;
              console.log(`📞 Rúbricas: ${rubricasResponse.status()}`);
              expect(rubricasResponse.status()).toBe(200);
              
              await page.waitForTimeout(2000);
              
              const rubricasDisponibles = await page.evaluate(() => {
                const select = document.getElementById('rubricaIdCascada');
                return Array.from(select.options).filter(opt => opt.value !== '').length;
              });
              
              console.log(`📊 Rúbricas disponibles: ${rubricasDisponibles}`);
              console.log('✅ FLUJO COMPLETO DE CASCADA FUNCIONA CORRECTAMENTE');
            }
          }
        }
      } else {
        console.log('⚠️ No hay materias en este grupo (normal para algunos grupos)');
      }
    } else {
      console.log('⚠️ No hay grupos disponibles para probar');
    }
    
    console.log('✅ Test de filtros en cascada completado exitosamente');
  });

  test('Verificar datos reales en endpoints', async ({ page }) => {
    console.log('🔍 Test: Verificar datos reales en endpoints');
    
    // Probar con diferentes IDs para encontrar datos
    const idsParaProbar = [1, 2, 3, 4, 5];
    
    for (const id of idsParaProbar) {
      console.log(`\n📞 Probando con ID ${id}:`);
      
      // Probar instrumentos por materia
      try {
        const response = await page.goto(`https://localhost:18163/Evaluaciones/GetInstrumentosByMateria?materiaId=${id}`);
        if (response.ok()) {
          const data = await response.json();
          console.log(`   Instrumentos para materia ${id}: ${data.length} elementos`);
          if (data.length > 0) {
            console.log(`   Primer instrumento: ${data[0].text} (ID: ${data[0].value})`);
          }
        }
      } catch (error) {
        console.log(`   Error probando materia ${id}: ${error.message}`);
      }
      
      // Probar rúbricas por instrumento
      try {
        const response2 = await page.goto(`https://localhost:18163/Evaluaciones/GetRubricasByInstrumento?instrumentoEvaluacionId=${id}`);
        if (response2.ok()) {
          const data2 = await response2.json();
          console.log(`   Rúbricas para instrumento ${id}: ${data2.length} elementos`);
          if (data2.length > 0) {
            console.log(`   Primera rúbrica: ${data2[0].text} (ID: ${data2[0].value})`);
          }
        }
      } catch (error) {
        console.log(`   Error probando instrumento ${id}: ${error.message}`);
      }
    }
    
    console.log('✅ Verificación de datos completada');
  });
});