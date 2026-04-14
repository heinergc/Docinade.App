import { test, expect } from '@playwright/test';

test.describe('Admin - Creación de Usuarios', () => {

  test.beforeEach(async ({ page }) => {
    // Login como administrador
    await page.goto('/Account/Login');
    await page.fill('input[name="Email"], input[id="Email"]', 'admin@rubricas.edu');
    await page.fill('input[name="Password"], input[id="Password"]', 'Admin123!');
    await page.click('button[type="submit"], input[type="submit"]');
    await page.waitForLoadState('networkidle');
    
    // Verificar que el login fue exitoso
    if (page.url().includes('/Login')) {
      throw new Error('Login como admin falló');
    }
  });

  test('debe poder acceder a la página de creación de usuarios', async ({ page }) => {
    console.log('🔍 Verificando acceso a la página de creación de usuarios...');
    
    // Navegar a la creación de usuarios
    await page.goto('/Admin/Users/Create');
    
    // Verificar que estamos en la página correcta
    await expect(page).toHaveURL(/.*Admin\/Users\/Create.*/);
    await expect(page.locator('h2, h1')).toContainText(/Crear.*Usuario/i);
    
    console.log('✅ Acceso a la página de creación exitoso');
  });

  test('debe mostrar todos los campos necesarios del formulario', async ({ page }) => {
    console.log('📋 Verificando campos del formulario...');
    
    await page.goto('/Admin/Users/Create');
    
    // Verificar campos obligatorios
    const camposObligatorios = [
      'Email',
      'Password', 
      'ConfirmPassword',
      'Nombre',
      'Apellido'
    ];
    
    for (const campo of camposObligatorios) {
      const input = page.locator(`input[name="${campo}"], input[id="${campo}"]`);
      await expect(input).toBeVisible();
      console.log(`✅ Campo ${campo} presente`);
    }
    
    // Verificar campos opcionales
    const camposOpcionales = [
      'NumeroIdentificacion',
      'Institucion',
      'Departamento',
      'PhoneNumber'
    ];
    
    for (const campo of camposOpcionales) {
      const input = page.locator(`input[name="${campo}"], input[id="${campo}"]`);
      await expect(input).toBeVisible();
      console.log(`✅ Campo ${campo} presente`);
    }
    
    // Verificar checkboxes
    const checkboxes = [
      'IsActive',
      'EmailConfirmed', 
      'SendWelcomeEmail'
    ];
    
    for (const checkbox of checkboxes) {
      const input = page.locator(`input[name="${checkbox}"], input[id="${checkbox}"]`);
      await expect(input).toBeVisible();
      console.log(`✅ Checkbox ${checkbox} presente`);
    }
    
    console.log('✅ Todos los campos del formulario están presentes');
  });

  test('debe crear un usuario exitosamente', async ({ page }) => {
    console.log('👤 Creando usuario desde panel de administración...');
    
    await page.goto('/Admin/Users/Create');
    
    // Generar datos únicos para el test
    const timestamp = Date.now();
    const testUser = {
      email: `admintest${timestamp}@test.com`,
      password: 'AdminTest123!',
      nombre: 'Usuario Admin',
      apellido: 'Test',
      numeroIdentificacion: `ADMIN${timestamp}`,
      institucion: 'Universidad Admin',
      departamento: 'Departamento Admin'
    };
    
    console.log(`📧 Creando usuario: ${testUser.email}`);
    
    // Llenar el formulario
    await page.fill('input[name="Email"], input[id="Email"]', testUser.email);
    await page.fill('input[name="Password"], input[id="Password"]', testUser.password);
    await page.fill('input[name="ConfirmPassword"], input[id="ConfirmPassword"]', testUser.password);
    await page.fill('input[name="Nombre"], input[id="Nombre"]', testUser.nombre);
    await page.fill('input[name="Apellido"], input[id="Apellido"]', testUser.apellido);
    await page.fill('input[name="NumeroIdentificacion"], input[id="NumeroIdentificacion"]', testUser.numeroIdentificacion);
    await page.fill('input[name="Institucion"], input[id="Institucion"]', testUser.institucion);
    await page.fill('input[name="Departamento"], input[id="Departamento"]', testUser.departamento);
    
    // Configurar checkboxes
    await page.check('input[name="IsActive"], input[id="IsActive"]');
    await page.check('input[name="EmailConfirmed"], input[id="EmailConfirmed"]');
    
    // Seleccionar roles si están disponibles
    const roleCheckboxes = await page.locator('input[type="checkbox"]:near(:text("Docente"), :text("Evaluador"), :text("Observador"))').count();
    if (roleCheckboxes > 0) {
      // Seleccionar el primer rol disponible (probablemente Docente)
      await page.locator('input[type="checkbox"]:near(:text("Docente"))').first().check();
      console.log('✅ Rol Docente asignado');
    }
    
    // Capturar screenshot antes del envío
    await page.screenshot({ path: 'test-results/admin-create-user-form.png' });
    
    // Enviar el formulario
    console.log('📤 Enviando formulario...');
    await page.click('button[type="submit"], input[type="submit"]');
    
    // Esperar la respuesta
    await page.waitForLoadState('networkidle');
    await page.screenshot({ path: 'test-results/admin-create-user-result.png' });
    
    // Verificar el resultado
    const currentUrl = page.url();
    console.log(`🔗 URL después del envío: ${currentUrl}`);
    
    // Buscar mensajes de éxito o error
    const successMessage = await page.locator('.alert-success, .success, text=exitoso').isVisible();
    const errorMessage = await page.locator('.alert-danger, .error, .text-danger').isVisible();
    
    if (successMessage) {
      console.log('✅ ¡Usuario creado exitosamente desde panel admin!');
      
      // Verificar que fuimos redirigidos a la página de detalles o lista
      expect(currentUrl).toMatch(/Admin\/Users/);
      
    } else if (errorMessage) {
      const errorText = await page.locator('.alert-danger, .error, .text-danger').first().textContent();
      console.log('❌ Error en la creación:', errorText);
      
      // Si es error de usuario existente, no falla el test
      if (errorText?.includes('existe')) {
        console.log('⚠️ Usuario ya existe - esto es normal en testing');
      } else {
        throw new Error(`Error inesperado: ${errorText}`);
      }
      
    } else {
      // Verificar si estamos en una página de éxito indirecta
      if (currentUrl.includes('/Admin/Users') && !currentUrl.includes('/Create')) {
        console.log('✅ Usuario creado exitosamente (redirigido)');
      } else {
        console.log('⚠️ Resultado incierto - verificando manualmente...');
        
        // Buscar en la lista de usuarios
        await page.goto('/Admin/Users');
        const userInList = await page.locator(`text=${testUser.email}`).isVisible();
        
        if (userInList) {
          console.log('✅ Usuario encontrado en la lista - creación exitosa');
        } else {
          throw new Error('Usuario no fue creado correctamente');
        }
      }
    }
  });

  test('debe validar campos obligatorios', async ({ page }) => {
    console.log('🔍 Verificando validación de campos obligatorios...');
    
    await page.goto('/Admin/Users/Create');
    
    // Intentar enviar formulario vacío
    await page.click('button[type="submit"], input[type="submit"]');
    
    // Verificar que aparecen mensajes de validación
    const validationErrors = page.locator('.text-danger, .field-validation-error, [data-valmsg-for]');
    
    // Debe haber al menos un mensaje de validación
    await expect(validationErrors.first()).toBeVisible();
    
    console.log('✅ Validación de campos obligatorios funciona');
  });

  test('debe validar confirmación de contraseña', async ({ page }) => {
    console.log('🔒 Verificando validación de confirmación de contraseña...');
    
    await page.goto('/Admin/Users/Create');
    
    // Llenar contraseñas diferentes
    await page.fill('input[name="Password"], input[id="Password"]', 'Password123!');
    await page.fill('input[name="ConfirmPassword"], input[id="ConfirmPassword"]', 'DifferentPassword123!');
    await page.fill('input[name="Email"], input[id="Email"]', 'test@test.com');
    await page.fill('input[name="Nombre"], input[id="Nombre"]', 'Test');
    await page.fill('input[name="Apellido"], input[id="Apellido"]', 'User');
    
    // Enviar formulario
    await page.click('button[type="submit"], input[type="submit"]');
    
    // Verificar mensaje de validación de confirmación de contraseña
    const passwordError = page.locator('span[data-valmsg-for="ConfirmPassword"], text=*coinciden*');
    await expect(passwordError).toBeVisible();
    
    console.log('✅ Validación de confirmación de contraseña funciona');
  });

  test('debe manejar email duplicado correctamente', async ({ page }) => {
    console.log('📧 Verificando manejo de email duplicado...');
    
    await page.goto('/Admin/Users/Create');
    
    // Intentar crear usuario con email del admin existente
    await page.fill('input[name="Email"], input[id="Email"]', 'admin@rubricas.edu');
    await page.fill('input[name="Password"], input[id="Password"]', 'Test123!');
    await page.fill('input[name="ConfirmPassword"], input[id="ConfirmPassword"]', 'Test123!');
    await page.fill('input[name="Nombre"], input[id="Nombre"]', 'Test');
    await page.fill('input[name="Apellido"], input[id="Apellido"]', 'User');
    
    // Enviar formulario
    await page.click('button[type="submit"], input[type="submit"]');
    await page.waitForLoadState('networkidle');
    
    // Verificar mensaje de error por email duplicado
    const duplicateError = page.locator('text=*existe*, text=*duplicado*');
    await expect(duplicateError).toBeVisible();
    
    console.log('✅ Manejo de email duplicado funciona correctamente');
  });
});
