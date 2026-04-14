import { test, expect } from '@playwright/test';

test('Reproduce error and check logs', async ({ page }) => {
  // Ir a la página de login
  await page.goto('https://localhost:18163/Account/Login');
  
  // Hacer login
  await page.fill('input[name="Email"]', 'admin@rubricas.edu');
  await page.fill('input[name="Password"]', 'Admin123!');
  await page.click('button[type="submit"]');
  
  // Esperar a que complete el login
  await page.waitForLoadState('networkidle');
  
  // Ir directamente al formulario CreateStep1
  await page.goto('https://localhost:18163/Profesores/CreateStep1');
  
  console.log('=== REPRODUCIENDO ERROR ===');
  
  // Llenar el formulario
  await page.fill('input[name="Nombres"]', 'Juan Carlos');
  await page.fill('input[name="PrimerApellido"]', 'González');
  await page.fill('input[name="Cedula"]', '1-1234-5678');
  
  console.log('Form filled, submitting...');
  
  // Enviar el formulario
  await page.click('button:has-text("Continuar")');
  
  // Esperar la respuesta
  await page.waitForTimeout(3000);
  
  console.log('Form submitted, checking result...');
  console.log('Final URL:', page.url());
  
  // Revisar errores
  const errors = await page.locator('.alert-danger, .text-danger').allTextContents();
  console.log('Errors found:', errors);
});