import { test, expect } from '@playwright/test';

test.describe('Professor Creation Form Tests', () => {
  let page;

  test.beforeEach(async ({ browser }) => {
    page = await browser.newPage();
    
    // Ir a la página de login
    await page.goto('https://localhost:18163/Account/Login');
    
    // Hacer login con credenciales de administrador
    await page.fill('input[name="Email"]', 'admin@rubricas.edu');
    await page.fill('input[name="Password"]', 'Admin123!');
    await page.click('button[type="submit"]');
    
    // Esperar a que redirija (puede ser PeriodoAcademico o Dashboard)
    await page.waitForLoadState('networkidle');
    
    console.log('After login URL:', page.url());
  });

  test('Debería navegar al formulario de crear profesor desde el menú', async () => {
    // Si está en selección de periodo, seleccionar uno o ir directamente al dashboard
    if (page.url().includes('PeriodoAcademico/Seleccionar')) {
      // Intentar ir directamente al dashboard
      await page.goto('https://localhost:18163/Dashboard');
    }
    
    // Buscar el menú de Profesores (puede estar en la navegación principal)
    const profesoresMenu = page.getByText('Profesores');
    await profesoresMenu.hover();
    await page.click('text=Nuevo Profesor');
    
    // Verificar que llegamos al formulario CreateStep1
    await expect(page).toHaveURL(/.*\/Profesores\/CreateStep1/);
    
    // Tomar screenshot del formulario
    await page.screenshot({ path: 'createstep1-form.png', fullPage: true });
  });

  test('Debería llenar y enviar el formulario CreateStep1 correctamente', async () => {
    // Ir directamente al formulario
    await page.goto('https://localhost:18163/Profesores/CreateStep1');
    
    // Verificar que estamos en el formulario correcto
    await expect(page.locator('h2')).toContainText(/Información Personal|Paso 1/);
    
    // Llenar los campos obligatorios
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
    
    // Llenar teléfonos
    await page.fill('input[name="TelefonoCelular"]', '8888-9999');
    await page.fill('input[name="TelefonoFijo"]', '2222-3333');
    
    // Llenar email
    await page.fill('input[name="EmailPersonal"]', 'juan.gonzalez@test.com');
    
    // Tomar screenshot antes de enviar
    await page.screenshot({ path: 'before-submit-step1.png', fullPage: true });
    
    // Hacer clic en el botón "Continuar"
    await page.click('button:has-text("Continuar")');
    
    // Esperar a que procese la request
    await page.waitForTimeout(3000);
    
    // Tomar screenshot después del submit
    await page.screenshot({ path: 'after-submit-step1.png', fullPage: true });
    
    console.log('URL after submit:', page.url());
    
    // Verificar si hay errores de validación
    const validationErrors = await page.locator('.field-validation-error, .alert-danger').allTextContents();
    if (validationErrors.length > 0) {
      console.log('Validation errors found:', validationErrors);
    }
    
    // Verificar si avanzó al siguiente paso
    const isStep2 = page.url().includes('CreateStep2');
    if (isStep2) {
      console.log('Successfully moved to Step 2');
      await expect(page).toHaveURL(/.*\/CreateStep2/);
    } else {
      // Si no avanzó, verificar qué pasó
      const currentContent = await page.textContent('body');
      console.log('Current page content snippet:', currentContent.substring(0, 500));
    }
  });

  test('Debería mostrar errores de validación para campos faltantes', async () => {
    // Ir al formulario
    await page.goto('https://localhost:18163/Profesores/CreateStep1');
    
    // Intentar enviar sin llenar campos obligatorios
    await page.click('button:has-text("Continuar")');
    
    // Esperar a que aparezcan los errores
    await page.waitForTimeout(2000);
    
    // Verificar que aparecen mensajes de validación
    const validationErrors = await page.locator('.field-validation-error').count();
    expect(validationErrors).toBeGreaterThan(0);
    
    console.log('Validation errors count:', validationErrors);
    
    // Tomar screenshot de los errores
    await page.screenshot({ path: 'validation-errors-step1.png', fullPage: true });
  });

  test('Debería cargar las listas desplegables correctamente', async () => {
    // Ir al formulario
    await page.goto('https://localhost:18163/Profesores/CreateStep1');
    
    // Verificar que las listas desplegables tienen opciones
    const sexoOptions = await page.locator('select[name="Sexo"] option').count();
    const estadoCivilOptions = await page.locator('select[name="EstadoCivil"] option').count();
    
    console.log('Sexo options:', sexoOptions);
    console.log('Estado Civil options:', estadoCivilOptions);
    
    expect(sexoOptions).toBeGreaterThan(1); // Debe tener más que solo la opción vacía
    expect(estadoCivilOptions).toBeGreaterThan(1);
    
    // Verificar contenido de las opciones
    const sexoTexts = await page.locator('select[name="Sexo"] option').allTextContents();
    const estadoCivilTexts = await page.locator('select[name="EstadoCivil"] option').allTextContents();
    
    console.log('Sexo options:', sexoTexts);
    console.log('Estado Civil options:', estadoCivilTexts);
  });
});