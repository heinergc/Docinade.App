import { test, expect } from '@playwright/test';

test.describe('Direct Form Submission Test', () => {
  let page;

  test.beforeEach(async ({ browser }) => {
    page = await browser.newPage();
    
    // Ir a la página de login
    await page.goto('https://localhost:18163/Account/Login');
    
    // Hacer login con credenciales de administrador
    await page.fill('input[name="Email"]', 'admin@rubricas.edu');
    await page.fill('input[name="Password"]', 'Admin123!');
    await page.click('button[type="submit"]');
    
    // Esperar a que complete el login
    await page.waitForLoadState('networkidle');
    
    console.log('After login URL:', page.url());
  });

  test('Debería encontrar el error específico al enviar el formulario CreateStep1', async () => {
    // Ir directamente al formulario CreateStep1
    await page.goto('https://localhost:18163/Profesores/CreateStep1');
    
    // Verificar que estamos en la página correcta
    console.log('Current URL:', page.url());
    
    // Tomar screenshot inicial
    await page.screenshot({ path: 'initial-form.png', fullPage: true });
    
    // Llenar TODOS los campos disponibles en Step1
    await page.fill('input[name="Nombres"]', 'Juan Carlos');
    await page.fill('input[name="PrimerApellido"]', 'González');
    await page.fill('input[name="SegundoApellido"]', 'Ramírez');
    await page.fill('input[name="Cedula"]', '1-1234-5678');
    
    // Seleccionar fecha de nacimiento
    await page.fill('input[name="FechaNacimiento"]', '1980-01-15');
    
    // Seleccionar género
    await page.selectOption('select[name="Sexo"]', 'M');
    
    // Seleccionar estado civil
    await page.selectOption('select[name="EstadoCivil"]', 'Soltero');
    
    // Llenar nacionalidad
    await page.fill('input[name="Nacionalidad"]', 'Costarricense');
    
    // Tomar screenshot antes del envío
    await page.screenshot({ path: 'before-submit.png', fullPage: true });
    
    // Configurar listener para capturar errores de red
    const responses = [];
    page.on('response', response => {
      responses.push({
        url: response.url(),
        status: response.status(),
        statusText: response.statusText()
      });
    });
    
    // Configurar listener para capturar errores de consola
    const consoleMessages = [];
    page.on('console', msg => {
      consoleMessages.push({
        type: msg.type(),
        text: msg.text()
      });
    });
    
    console.log('Submitting form...');
    
    // Enviar el formulario
    await page.click('button:has-text("Continuar")');
    
    // Esperar a que procese la request
    await page.waitForTimeout(3000);
    
    // Tomar screenshot después del envío
    await page.screenshot({ path: 'after-submit.png', fullPage: true });
    
    console.log('After submit URL:', page.url());
    
    // Revisar si hay errores de validación en la página
    const validationErrors = await page.locator('.field-validation-error, .alert-danger, .text-danger').allTextContents();
    if (validationErrors.length > 0) {
      console.log('Validation errors found:', validationErrors);
    }
    
    // Revisar respuestas HTTP
    console.log('HTTP Responses:', responses.filter(r => r.status >= 400));
    
    // Revisar mensajes de consola
    console.log('Console messages:', consoleMessages.filter(m => m.type === 'error'));
    
    // Verificar el título de la página para entender dónde estamos
    console.log('Page title:', await page.title());
    
    // Verificar si avanzó al siguiente paso o si hay algún error
    const currentUrl = page.url();
    if (currentUrl.includes('CreateStep2')) {
      console.log('✅ Successfully moved to Step 2');
    } else if (currentUrl.includes('CreateStep1')) {
      console.log('⚠️ Still on Step 1, checking for errors...');
      
      // Buscar posibles mensajes de error en la página
      const errorSelectors = [
        '.alert',
        '.error',
        '.validation-summary-errors',
        '[data-valmsg-summary="true"]',
        '.field-validation-error'
      ];
      
      for (const selector of errorSelectors) {
        const elements = await page.locator(selector).count();
        if (elements > 0) {
          const texts = await page.locator(selector).allTextContents();
          console.log(`Found errors in ${selector}:`, texts);
        }
      }
    } else {
      console.log(`⚠️ Unexpected redirect to: ${currentUrl}`);
    }
    
    // Revisar si hay scripts de validación que fallen
    const scriptErrors = await page.evaluate(() => {
      return window.__validationErrors || [];
    });
    if (scriptErrors.length > 0) {
      console.log('Script validation errors:', scriptErrors);
    }
  });
});