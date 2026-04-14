import { test, expect } from '@playwright/test';

test.describe('Diagnóstico del Sistema de Usuarios', () => {
  
  test('verificar estado de la aplicación', async ({ page }) => {
    console.log('🚀 Iniciando diagnóstico del sistema...');
    
    // 1. Verificar que la aplicación esté funcionando
    await page.goto('/');
    await expect(page).toHaveTitle(/.*/); // Cualquier título indica que la app carga
    console.log('✅ Aplicación carga correctamente');
    
    // 2. Verificar la página de login
    await page.goto('/Account/Login');
    const loginForm = page.locator('form');
    await expect(loginForm).toBeVisible();
    console.log('✅ Página de login accesible');
    
    // 3. Verificar usuarios por defecto
    console.log('🔍 Verificando usuarios por defecto...');
    
    const defaultUsers = [
      { email: 'admin@rubricas.edu', password: 'Admin123!' },
      { email: 'docente@rubricas.edu', password: 'Docente123!' },
      { email: 'evaluador@rubricas.edu', password: 'Evaluador123!' }
    ];
    
    for (const user of defaultUsers) {
      await page.goto('/Account/Login');
      await page.fill('input[name="Email"], input[id="Email"]', user.email);
      await page.fill('input[name="Password"], input[id="Password"]', user.password);
      await page.click('button[type="submit"], input[type="submit"]');
      
      await page.waitForLoadState('networkidle');
      
      if (page.url().includes('/Home') || page.url().includes('/Dashboard') || !page.url().includes('/Login')) {
        console.log(`✅ Usuario ${user.email} puede hacer login`);
        
        // Hacer logout para el siguiente test
        try {
          await page.click('a[href*="Logout"], button:has-text("Cerrar"), a:has-text("Salir")');
          await page.waitForLoadState('networkidle');
        } catch (e) {
          console.log(`⚠️ No se pudo hacer logout automático para ${user.email}`);
        }
      } else {
        console.log(`❌ Usuario ${user.email} NO puede hacer login`);
        
        // Capturar errores de login
        const errorMsg = await page.locator('.alert-danger, .error, .text-danger').textContent();
        if (errorMsg) {
          console.log(`   Error: ${errorMsg}`);
        }
      }
    }
  });

  test('probar creación de usuario paso a paso', async ({ page }) => {
    console.log('👤 Probando creación de usuario paso a paso...');
    
    // Verificar configuración de registro
    await page.goto('/Account/Register');
    
    const registroCerrado = await page.locator('text=registro*cerrado, text=registro*deshabilitado, text=temporalmente*deshabilitado').isVisible();
    
    if (registroCerrado) {
      console.log('❌ PROBLEMA IDENTIFICADO: El registro está cerrado');
      
      const mensaje = await page.locator('body').textContent();
      console.log('📝 Mensaje completo:', mensaje);
      
      // Verificar si podemos habilitar el registro (como admin)
      console.log('🔧 Intentando habilitar el registro...');
      
      // Primero hacer login como admin
      await page.goto('/Account/Login');
      await page.fill('input[name="Email"], input[id="Email"]', 'admin@rubricas.edu');
      await page.fill('input[name="Password"], input[id="Password"]', 'Admin123!');
      await page.click('button[type="submit"], input[type="submit"]');
      await page.waitForLoadState('networkidle');
      
      if (!page.url().includes('/Login')) {
        console.log('✅ Login como admin exitoso');
        
        // Buscar configuración del sistema
        try {
          await page.goto('/Configuracion');
          console.log('📋 Accediendo a configuración del sistema...');
          
          // Buscar el toggle de registro abierto
          const registroToggle = page.locator('input[type="checkbox"]:near(text="Registro"), input[name*="registro"], input[id*="registro"]');
          
          if (await registroToggle.isVisible()) {
            await registroToggle.check();
            await page.click('button[type="submit"], button:has-text("Guardar")');
            console.log('✅ Registro habilitado desde configuración');
          }
        } catch (e) {
          console.log('⚠️ No se pudo acceder a la configuración automáticamente');
        }
      }
    } else {
      console.log('✅ El registro está habilitado');
      
      // Proceder con la creación del usuario
      const timestamp = Date.now();
      const testUser = {
        email: `testuser${timestamp}@test.com`,
        password: 'TestPassword123!',
        nombre: 'Usuario',
        apellidos: 'De Prueba',
        numeroIdentificacion: `ID${timestamp}`,
        institucion: 'Universidad Test',
        departamento: 'Departamento Test'
      };
      
      console.log(`👤 Creando usuario: ${testUser.email}`);
      
      // Llenar formulario paso a paso con verificaciones
      const steps = [
        { field: 'Email', value: testUser.email },
        { field: 'Password', value: testUser.password },
        { field: 'ConfirmPassword', value: testUser.password },
        { field: 'Nombre', value: testUser.nombre },
        { field: 'Apellidos', value: testUser.apellidos },
        { field: 'NumeroIdentificacion', value: testUser.numeroIdentificacion },
        { field: 'Institucion', value: testUser.institucion },
        { field: 'Departamento', value: testUser.departamento }
      ];
      
      for (const step of steps) {
        const field = page.locator(`input[name="${step.field}"], input[id="${step.field}"]`);
        
        if (await field.isVisible()) {
          await field.fill(step.value);
          console.log(`✅ Campo ${step.field} llenado`);
        } else {
          console.log(`⚠️ Campo ${step.field} no encontrado`);
        }
      }
      
      // Seleccionar rol si está disponible
      const roleSelect = page.locator('select[name="SelectedRole"], select[id="SelectedRole"]');
      if (await roleSelect.isVisible()) {
        await roleSelect.selectOption('Docente');
        console.log('✅ Rol seleccionado: Docente');
      } else {
        console.log('ℹ️ No hay selector de rol visible');
      }
      
      // Capturar estado antes del envío
      await page.screenshot({ path: 'test-results/formulario-completo.png' });
      
      console.log('📤 Enviando formulario...');
      await page.click('button[type="submit"], input[type="submit"]');
      
      // Esperar respuesta
      await page.waitForLoadState('networkidle');
      await page.screenshot({ path: 'test-results/despues-envio.png' });
      
      // Analizar resultado
      const currentUrl = page.url();
      console.log(`🔗 URL después del envío: ${currentUrl}`);
      
      const errorMessages = await page.locator('.alert-danger, .error, .field-validation-error, .text-danger').allTextContents();
      const successMessages = await page.locator('.alert-success, .success').allTextContents();
      
      if (errorMessages.length > 0) {
        console.log('❌ Errores encontrados:');
        errorMessages.forEach(msg => console.log(`   - ${msg}`));
      }
      
      if (successMessages.length > 0) {
        console.log('✅ Mensajes de éxito:');
        successMessages.forEach(msg => console.log(`   - ${msg}`));
      }
      
      if (currentUrl.includes('/Home') || currentUrl.includes('/Dashboard')) {
        console.log('✅ ¡Usuario creado exitosamente y logueado automáticamente!');
      } else if (currentUrl.includes('/Login')) {
        console.log('ℹ️ Redirigido al login - verificando si el usuario fue creado...');
        
        // Intentar login con las credenciales
        await page.fill('input[name="Email"], input[id="Email"]', testUser.email);
        await page.fill('input[name="Password"], input[id="Password"]', testUser.password);
        await page.click('button[type="submit"], input[type="submit"]');
        await page.waitForLoadState('networkidle');
        
        if (!page.url().includes('/Login')) {
          console.log('✅ Usuario creado correctamente - login manual exitoso');
        } else {
          console.log('❌ Usuario no fue creado - login falló');
        }
      } else {
        console.log('⚠️ Resultado inesperado - verificar manualmente');
      }
    }
  });

  test('verificar logs de aplicación y errores', async ({ page, context }) => {
    console.log('📋 Verificando logs y errores de la aplicación...');
    
    // Capturar errores de consola
    const consoleErrors = [];
    const networkErrors = [];
    
    page.on('console', msg => {
      if (msg.type() === 'error') {
        consoleErrors.push(msg.text());
      }
    });
    
    page.on('response', response => {
      if (response.status() >= 400) {
        networkErrors.push(`${response.status()} ${response.url()}`);
      }
    });
    
    // Navegar por las páginas principales
    const pagesToTest = [
      '/',
      '/Account/Login',
      '/Account/Register'
    ];
    
    for (const pagePath of pagesToTest) {
      console.log(`🔍 Verificando: ${pagePath}`);
      await page.goto(pagePath);
      await page.waitForLoadState('networkidle');
      
      // Esperar un momento para capturar errores
      await page.waitForTimeout(1000);
    }
    
    // Reportar errores encontrados
    if (consoleErrors.length > 0) {
      console.log('❌ Errores de consola encontrados:');
      consoleErrors.forEach(error => console.log(`   - ${error}`));
    } else {
      console.log('✅ No se encontraron errores de consola');
    }
    
    if (networkErrors.length > 0) {
      console.log('❌ Errores de red encontrados:');
      networkErrors.forEach(error => console.log(`   - ${error}`));
    } else {
      console.log('✅ No se encontraron errores de red');
    }
  });
});
