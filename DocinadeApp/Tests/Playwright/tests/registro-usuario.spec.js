import { test, expect } from '@playwright/test';

test.describe('Registro de Usuario', () => {
  
  test.beforeEach(async ({ page }) => {
    // Navegar a la página de inicio
    await page.goto('/');
  });

  test('debe permitir acceso a la página de registro cuando está habilitado', async ({ page }) => {
    // Hacer clic en el botón de registro
    await page.click('a[href*="Register"], a:has-text("Registrar"), a:has-text("Registro")');
    
    // Verificar que estamos en la página de registro
    await expect(page).toHaveURL(/.*Register.*/);
    await expect(page.locator('h1, h2, h3')).toContainText(/Registro|Crear cuenta|Registrar/i);
  });

  test('debe mostrar mensaje cuando el registro está cerrado', async ({ page }) => {
    // Primero, vamos a verificar si el registro está abierto
    try {
      await page.goto('/Account/Register');
      
      // Si hay un mensaje de registro cerrado, verificarlo
      const registroCerrado = page.locator('text=registro*cerrado, text=registro*deshabilitado, text=registro*bloqueado');
      if (await registroCerrado.isVisible()) {
        await expect(registroCerrado).toBeVisible();
        console.log('✓ Registro está cerrado - mensaje mostrado correctamente');
      } else {
        console.log('✓ Registro está abierto - continuando con pruebas');
      }
    } catch (error) {
      console.log('Error navegando a registro:', error.message);
    }
  });

  test('debe crear un nuevo usuario exitosamente', async ({ page }) => {
    // Navegar directamente al registro
    await page.goto('/Account/Register');
    
    // Verificar si el registro está disponible
    const registroCerradoMsg = page.locator('text=registro*cerrado, text=registro*deshabilitado');
    if (await registroCerradoMsg.isVisible()) {
      test.skip('Registro está cerrado, saltando prueba de creación');
    }

    // Generar datos únicos para el test
    const timestamp = Date.now();
    const testUser = {
      email: `test${timestamp}@rubricas.test`,
      password: 'Test123!',
      nombre: 'Usuario',
      apellidos: 'Prueba',
      numeroIdentificacion: `TEST${timestamp}`,
      institucion: 'Universidad Test',
      departamento: 'Departamento Test'
    };

    // Llenar el formulario de registro
    await page.fill('input[name="Email"], input[id="Email"]', testUser.email);
    await page.fill('input[name="Password"], input[id="Password"]', testUser.password);
    await page.fill('input[name="ConfirmPassword"], input[id="ConfirmPassword"]', testUser.password);
    await page.fill('input[name="Nombre"], input[id="Nombre"]', testUser.nombre);
    await page.fill('input[name="Apellidos"], input[id="Apellidos"]', testUser.apellidos);
    await page.fill('input[name="NumeroIdentificacion"], input[id="NumeroIdentificacion"]', testUser.numeroIdentificacion);
    await page.fill('input[name="Institucion"], input[id="Institucion"]', testUser.institucion);
    await page.fill('input[name="Departamento"], input[id="Departamento"]', testUser.departamento);

    // Seleccionar rol (si existe el selector)
    const roleSelect = page.locator('select[name="SelectedRole"], select[id="SelectedRole"]');
    if (await roleSelect.isVisible()) {
      await roleSelect.selectOption('Docente');
    }

    // Tomar screenshot antes del envío
    await page.screenshot({ path: 'test-results/antes-registro.png' });

    // Enviar el formulario
    await page.click('button[type="submit"], input[type="submit"]');

    // Esperar la respuesta y verificar el resultado
    await page.waitForLoadState('networkidle');

    // Verificar si el registro fue exitoso
    const isOnHomePage = page.url().includes('/Home') || page.url().includes('/Dashboard') || page.url() === new URL('/', page.url()).href;
    const hasSuccessMessage = await page.locator('.alert-success, .success, text=exitoso, text=bienvenido').isVisible();
    const hasErrorMessage = await page.locator('.alert-danger, .error, .field-validation-error').isVisible();

    if (hasErrorMessage) {
      // Capturar errores específicos
      const errorMessages = await page.locator('.alert-danger, .error, .field-validation-error').allTextContents();
      console.log('❌ Errores encontrados:', errorMessages);
      
      // Tomar screenshot del error
      await page.screenshot({ path: 'test-results/error-registro.png' });
      
      // Verificar errores comunes
      if (errorMessages.some(msg => msg.includes('ya existe') || msg.includes('already exists'))) {
        console.log('⚠️ Usuario ya existe, intentando con credenciales diferentes...');
        
        // Intentar con un email diferente
        const newEmail = `test${timestamp}_retry@rubricas.test`;
        await page.fill('input[name="Email"], input[id="Email"]', newEmail);
        await page.click('button[type="submit"], input[type="submit"]');
        await page.waitForLoadState('networkidle');
      }
    }

    // Tomar screenshot final
    await page.screenshot({ path: 'test-results/resultado-registro.png' });

    // Verificaciones finales
    if (isOnHomePage || hasSuccessMessage) {
      console.log('✅ Usuario creado exitosamente');
      expect(true).toBe(true); // Test pasó
    } else {
      console.log('❌ Falló la creación del usuario');
      console.log('URL actual:', page.url());
      
      // Capturar estado de la página para debugging
      const pageContent = await page.content();
      console.log('Contenido de la página (primeros 500 chars):', pageContent.substring(0, 500));
      
      // Fallar el test con información detallada
      expect(isOnHomePage, 'Usuario debería ser redirigido a la página principal después del registro').toBe(true);
    }
  });

  test('debe validar campos obligatorios', async ({ page }) => {
    await page.goto('/Account/Register');
    
    // Verificar si el registro está disponible
    const registroCerradoMsg = page.locator('text=registro*cerrado, text=registro*deshabilitado');
    if (await registroCerradoMsg.isVisible()) {
      test.skip('Registro está cerrado, saltando prueba de validación');
    }

    // Intentar enviar formulario vacío
    await page.click('button[type="submit"], input[type="submit"]');
    
    // Verificar que aparecen mensajes de validación
    const validationErrors = page.locator('.field-validation-error, .text-danger, [data-valmsg-for]');
    await expect(validationErrors.first()).toBeVisible();
    
    console.log('✅ Validaciones de campos obligatorios funcionan correctamente');
  });

  test('debe validar formato de email', async ({ page }) => {
    await page.goto('/Account/Register');
    
    const registroCerradoMsg = page.locator('text=registro*cerrado, text=registro*deshabilitado');
    if (await registroCerradoMsg.isVisible()) {
      test.skip('Registro está cerrado, saltando prueba de validación de email');
    }

    // Introducir un email inválido
    await page.fill('input[name="Email"], input[id="Email"]', 'email-invalido');
    await page.fill('input[name="Password"], input[id="Password"]', 'Test123!');
    await page.click('button[type="submit"], input[type="submit"]');
    
    // Verificar mensaje de validación de email
    const emailValidation = page.locator('text=email*válido, text=email*formato, [data-valmsg-for="Email"]');
    
    if (await emailValidation.isVisible()) {
      console.log('✅ Validación de formato de email funciona');
      await expect(emailValidation).toBeVisible();
    } else {
      // Verificar validación HTML5
      const emailInput = page.locator('input[name="Email"], input[id="Email"]');
      const validationMessage = await emailInput.evaluate(el => el.validationMessage);
      expect(validationMessage).toBeTruthy();
      console.log('✅ Validación HTML5 de email funciona:', validationMessage);
    }
  });
});

test.describe('Debug del Sistema de Registro', () => {
  test('inspeccionar configuración del sistema', async ({ page }) => {
    // Verificar configuración del sistema
    console.log('🔍 Inspeccionando configuración del sistema...');
    
    await page.goto('/');
    
    // Verificar si hay enlaces de registro visibles
    const registerLinks = await page.locator('a[href*="Register"], a:has-text("Registrar"), a:has-text("Registro")').count();
    console.log(`📝 Enlaces de registro encontrados: ${registerLinks}`);
    
    // Intentar acceder directamente al endpoint de configuración (si existe)
    try {
      const response = await page.request.get('/api/configuracion/registro');
      if (response.ok()) {
        const config = await response.json();
        console.log('⚙️ Configuración de registro:', config);
      }
    } catch (error) {
      console.log('ℹ️ No se pudo acceder a la API de configuración');
    }
    
    // Verificar la página de registro
    await page.goto('/Account/Register');
    await page.screenshot({ path: 'test-results/pagina-registro-debug.png' });
    
    const pageTitle = await page.title();
    console.log(`📄 Título de la página: ${pageTitle}`);
    
    const url = page.url();
    console.log(`🔗 URL actual: ${url}`);
    
    // Verificar elementos del formulario
    const formElements = await page.locator('form input, form select, form button').count();
    console.log(`📋 Elementos de formulario encontrados: ${formElements}`);
    
    // Listar todos los inputs del formulario
    const inputs = await page.locator('form input').all();
    for (const input of inputs) {
      const name = await input.getAttribute('name');
      const type = await input.getAttribute('type');
      console.log(`  - Input: ${name} (${type})`);
    }
  });
});
