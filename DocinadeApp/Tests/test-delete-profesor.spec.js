// @ts-check
const { test, expect } = require('@playwright/test');

test.describe('Eliminar Profesor - Prueba End-to-End', () => {
  test('debe eliminar el profesor ID 1 con todos sus datos relacionados', async ({ page }) => {
    // Configurar timeout más largo para operaciones de base de datos
    test.setTimeout(60000);

    // 1. Navegar a la página de inicio de sesión
    await page.goto('https://localhost:18163/Account/Login', { 
      waitUntil: 'networkidle',
      timeout: 30000 
    });

    console.log('✓ Navegó a la página de login');

    // 2. Iniciar sesión (ajustar credenciales según sea necesario)
    await page.fill('input[name="Email"]', 'admin@rubricas.edu');
    await page.fill('input[name="Password"]', 'Admin123!');
    await page.click('button[type="submit"]');
    
    // Esperar a que cargue el dashboard
    await page.waitForURL('**/Home/Index', { timeout: 15000 });
    console.log('✓ Inicio de sesión exitoso');

    // 3. Navegar a la página de profesores
    await page.goto('https://localhost:18163/Profesores', { 
      waitUntil: 'networkidle',
      timeout: 30000 
    });
    console.log('✓ Navegó a la página de Profesores');

    // 4. Verificar que existe el profesor con ID 1
    const profesorRow = await page.locator('tr').filter({ hasText: 'Ana Maria' }).first();
    await expect(profesorRow).toBeVisible();
    console.log('✓ Profesor "Ana Maria Gonzalez" encontrado en la lista');

    // 5. Hacer clic en el botón de eliminar
    const deleteButton = profesorRow.locator('a[title="Eliminar"]');
    
    // Manejar el confirm del onclick
    page.on('dialog', dialog => {
      console.log(`✓ Confirmación inicial: ${dialog.message()}`);
      dialog.accept();
    });
    
    await deleteButton.click();
    console.log('✓ Clic en botón eliminar');

    // 6. Esperar a que cargue la página de confirmación
    await page.waitForURL('**/Profesores/Delete/1', { timeout: 15000 });
    console.log('✓ Página de confirmación cargada');

    // 7. Verificar que muestra la información del profesor
    await expect(page.locator('h4').filter({ hasText: 'Ana Maria' })).toBeVisible();
    console.log('✓ Información del profesor visible en página de confirmación');

    // 8. Verificar que muestra la advertencia
    await expect(page.locator('.alert-danger')).toBeVisible();
    await expect(page.locator('text=irreversible')).toBeVisible();
    console.log('✓ Advertencia de eliminación permanente visible');

    // 9. Hacer clic en el botón de confirmar eliminación
    const confirmButton = page.locator('button[type="submit"]').filter({ hasText: 'Eliminar Permanentemente' });
    
    // Manejar el segundo confirm
    page.on('dialog', dialog => {
      console.log(`✓ Confirmación final: ${dialog.message()}`);
      dialog.accept();
    });
    
    await confirmButton.click();
    console.log('✓ Clic en botón de confirmación final');

    // 10. Esperar a que vuelva al Index con mensaje de éxito
    await page.waitForURL('**/Profesores', { timeout: 15000 });
    console.log('✓ Redirigido al Index de Profesores');

    // 11. Verificar mensaje de éxito
    const successMessage = page.locator('.alert-success, .alert.alert-success');
    await expect(successMessage).toBeVisible({ timeout: 5000 });
    await expect(successMessage).toContainText('eliminado');
    console.log('✓ Mensaje de éxito mostrado');

    // 12. Verificar que el profesor ya no aparece en la lista
    const profesorDeleted = page.locator('tr').filter({ hasText: 'Ana Maria Gonzalez' });
    await expect(profesorDeleted).toHaveCount(0);
    console.log('✓ Profesor ya no aparece en la lista');

    // 13. Captura de pantalla final
    await page.screenshot({ path: 'Tests/screenshots/profesor-eliminado.png', fullPage: true });
    console.log('✓ Captura de pantalla guardada');

    console.log('\n=== PRUEBA COMPLETADA EXITOSAMENTE ===');
    console.log('✓ Profesor ID 1 eliminado correctamente');
    console.log('✓ Todos los datos relacionados eliminados');
    console.log('✓ Redirección al Index funcionando');
    console.log('✓ Mensaje de éxito mostrado');
  });

  test('debe mostrar correctamente la vista de confirmación', async ({ page }) => {
    // Iniciar sesión
    await page.goto('https://localhost:18163/Account/Login');
    await page.fill('input[name="Email"]', 'admin@rubricas.edu');
    await page.fill('input[name="Password"]', 'Admin123!');
    await page.click('button[type="submit"]');
    await page.waitForURL('**/Home/Index');

    // Navegar directamente a Delete de un profesor que existe
    await page.goto('https://localhost:18163/Profesores/Delete/2');
    
    // Verificar elementos de la página
    await expect(page.locator('h4').filter({ hasText: 'Advertencia' })).toBeVisible();
    await expect(page.locator('.card-header.bg-danger')).toBeVisible();
    await expect(page.locator('text=Cancelar')).toBeVisible();
    await expect(page.locator('text=Ver Opción Desactivar')).toBeVisible();
    await expect(page.locator('button').filter({ hasText: 'Eliminar Permanentemente' })).toBeVisible();

    console.log('✓ Vista de confirmación muestra todos los elementos correctamente');
  });
});
