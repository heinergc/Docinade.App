import { test, expect } from '@playwright/test';

test.describe('Basic Application Tests', () => {
  test('Debería cargar la página principal', async ({ page }) => {
    await page.goto('http://localhost:18164');
    
    // Tomar screenshot para ver qué se está mostrando
    await page.screenshot({ path: 'homepage-screenshot.png', fullPage: true });
    
    // Verificar que la página carga
    await expect(page).toHaveTitle(/Sistema de Rúbricas/);
    
    // Ver qué elementos están presentes
    const pageContent = await page.content();
    console.log('Page title:', await page.title());
    console.log('Current URL:', page.url());
    
    // Verificar si hay formulario de login
    const loginForm = await page.locator('form').count();
    console.log('Login forms found:', loginForm);
    
    // Verificar si hay menús
    const menuItems = await page.locator('nav a').allTextContents();
    console.log('Menu items:', menuItems);
    
    // Verificar si hay mensaje de "Profesores"
    const profesoresText = await page.getByText('Profesores').count();
    console.log('Profesores text found:', profesoresText);
  });

  test('Debería manejar autenticación si es necesaria', async ({ page }) => {
    await page.goto('http://localhost:18164');
    
    // Si hay un formulario de login, intentar hacer login
    const loginButton = page.getByRole('button', { name: /iniciar sesión|login/i });
    if (await loginButton.count() > 0) {
      console.log('Login required - attempting to login');
      
      // Verificar si hay campos de usuario/contraseña
      const emailField = page.locator('input[type="email"], input[name*="email"], input[name*="Email"]');
      const passwordField = page.locator('input[type="password"]');
      
      if (await emailField.count() > 0 && await passwordField.count() > 0) {
        await emailField.fill('admin@admin.com'); // Usuario por defecto común
        await passwordField.fill('Admin123!'); // Contraseña por defecto común
        await loginButton.click();
        
        // Esperar redirección
        await page.waitForURL('**/Home/**', { timeout: 5000 }).catch(() => {
          console.log('No redirect to Home after login');
        });
      }
    }
    
    // Tomar screenshot después del intento de login
    await page.screenshot({ path: 'after-login-screenshot.png', fullPage: true });
    
    // Verificar menús después del login
    const menuItems = await page.locator('nav a').allTextContents();
    console.log('Menu items after login:', menuItems);
  });

  test('Debería acceder directamente al formulario CreateStep1', async ({ page }) => {
    // Ir directamente al formulario
    await page.goto('http://localhost:18164/Profesores/CreateStep1');
    
    await page.screenshot({ path: 'direct-createstep1-screenshot.png', fullPage: true });
    
    console.log('Current URL:', page.url());
    console.log('Page title:', await page.title());
    
    // Verificar si hay errores en la página
    const errorMessages = await page.locator('.alert-danger, .text-danger').allTextContents();
    console.log('Error messages:', errorMessages);
    
    // Verificar si el formulario está presente
    const formExists = await page.locator('form').count();
    console.log('Forms found:', formExists);
    
    if (formExists > 0) {
      console.log('Form found - trying to submit with test data');
      
      // Llenar campos básicos si existen
      const nombresField = page.locator('input[name="Nombres"]');
      if (await nombresField.count() > 0) {
        await nombresField.fill('Test Name');
        
        const apellidoField = page.locator('input[name="PrimerApellido"]');
        if (await apellidoField.count() > 0) {
          await apellidoField.fill('Test Surname');
          
          const cedulaField = page.locator('input[name="Cedula"]');
          if (await cedulaField.count() > 0) {
            await cedulaField.fill('1-1234-5678');
            
            // Intentar enviar el formulario
            const submitButton = page.locator('button[type="submit"]');
            if (await submitButton.count() > 0) {
              console.log('Submitting form...');
              await submitButton.click();
              
              // Esperar la respuesta
              await page.waitForTimeout(2000);
              
              console.log('After submit - URL:', page.url());
              await page.screenshot({ path: 'after-submit-screenshot.png', fullPage: true });
              
              // Verificar errores después del submit
              const postSubmitErrors = await page.locator('.alert-danger, .text-danger').allTextContents();
              console.log('Post-submit errors:', postSubmitErrors);
            }
          }
        }
      }
    }
  });
});