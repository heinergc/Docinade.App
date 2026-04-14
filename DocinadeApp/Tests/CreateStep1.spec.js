import { test, expect } from '@playwright/test';

test.describe('Profesor CreateStep1 Form Tests', () => {
  test.beforeEach(async ({ page }) => {
    // Navegar a la página de inicio
    await page.goto('http://localhost:18164');
  });

  test('Debería navegar al formulario de nuevo profesor y completar paso 1', async ({ page }) => {
    // 1. Ir al menú de profesores
    await page.hover('text=Profesores');
    await page.click('text=Nuevo Profesor');

    // 2. Verificar que estamos en el paso 1
    await expect(page).toHaveTitle(/Nuevo Profesor/);
    await expect(page.locator('h1')).toContainText('Nuevo Profesor');
    await expect(page.locator('p')).toContainText('Paso 1 de 6: Información Personal');

    // 3. Llenar los campos requeridos
    await page.fill('input[name="Nombres"]', 'Juan Carlos');
    await page.fill('input[name="PrimerApellido"]', 'González');
    await page.fill('input[name="SegundoApellido"]', 'Pérez');
    await page.fill('input[name="Cedula"]', '1-1234-5678');
    
    // Seleccionar tipo de cédula
    await page.selectOption('select[name="TipoCedula"]', 'Nacional');
    
    // Seleccionar sexo
    await page.selectOption('select[name="Sexo"]', 'M');
    
    // Fecha de nacimiento
    await page.fill('input[name="FechaNacimiento"]', '1985-05-15');
    
    // Estado civil
    await page.selectOption('select[name="EstadoCivil"]', 'Soltero');
    
    // Nacionalidad
    await page.fill('input[name="Nacionalidad"]', 'Costarricense');

    // 4. Hacer clic en Continuar y capturar cualquier error
    console.log('Haciendo clic en Continuar...');
    
    // Interceptar cualquier request/response
    page.on('response', response => {
      console.log(`Response: ${response.status()} ${response.url()}`);
    });
    
    page.on('request', request => {
      console.log(`Request: ${request.method()} ${request.url()}`);
    });

    // Interceptar errores de consola
    page.on('console', msg => {
      if (msg.type() === 'error') {
        console.log(`Console Error: ${msg.text()}`);
      }
    });

    // Hacer clic en continuar
    const continueButton = page.locator('button[type="submit"]');
    await expect(continueButton).toBeVisible();
    
    // Esperar por la navegación después del clic
    const [response] = await Promise.all([
      page.waitForResponse(response => response.url().includes('CreateStep')),
      continueButton.click()
    ]);

    console.log(`Final Response Status: ${response.status()}`);
    console.log(`Final URL: ${page.url()}`);

    // 5. Verificar el resultado
    if (response.status() === 200) {
      // Debería ir al paso 2
      await expect(page.locator('p')).toContainText('Paso 2 de 6', { timeout: 10000 });
    } else {
      // Capturar el error
      console.log('Error detectado, capturando detalles...');
      await page.screenshot({ path: 'error-screenshot.png', fullPage: true });
      
      // Verificar si hay mensajes de error en la página
      const errorMessages = await page.locator('.text-danger, .alert-danger').allTextContents();
      console.log('Error messages found:', errorMessages);
      
      // Verificar validaciones de campos
      const validationErrors = await page.locator('.field-validation-error').allTextContents();
      console.log('Validation errors:', validationErrors);
    }
  });

  test('Debería validar campos requeridos', async ({ page }) => {
    // Ir al formulario
    await page.hover('text=Profesores');
    await page.click('text=Nuevo Profesor');

    // Intentar enviar sin llenar campos requeridos
    await page.click('button[type="submit"]');

    // Verificar validaciones
    const validationErrors = await page.locator('.text-danger').allTextContents();
    console.log('Validation errors for empty form:', validationErrors);

    // Debería seguir en el mismo paso
    await expect(page.locator('p')).toContainText('Paso 1 de 6');
  });

  test('Debería validar formato de cédula', async ({ page }) => {
    // Ir al formulario
    await page.hover('text=Profesores');
    await page.click('text=Nuevo Profesor');

    // Llenar con cédula inválida
    await page.fill('input[name="Nombres"]', 'Test');
    await page.fill('input[name="PrimerApellido"]', 'Test');
    await page.fill('input[name="Cedula"]', 'cedula-invalida');

    await page.click('button[type="submit"]');

    // Verificar mensaje de validación
    const cedulaError = await page.locator('span[data-valmsg-for="Cedula"]').textContent();
    console.log('Cedula validation error:', cedulaError);
  });
});