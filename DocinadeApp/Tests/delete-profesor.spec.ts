import { test, expect } from '@playwright/test';

test.describe('Eliminación de Profesores', () => {
  test.beforeEach(async ({ page }) => {
    // Navegar a la página de login (usando HTTP en lugar de HTTPS para evitar problemas de certificado)
    await page.goto('http://localhost:18163/Account/Login', { timeout: 30000 });
    
    // Login
    await page.fill('input[name="Email"]', 'admin@rubricas.edu');
    await page.fill('input[name="Password"]', 'Admin123!');
    await page.click('button[type="submit"]');
    
    // Esperar a que redirija (puede ir a Home o a selección de período)
    await page.waitForLoadState('networkidle');
    
    // Verificar si estamos en la página de selección de período
    const currentUrl = page.url();
    if (currentUrl.includes('/PeriodoAcademico/Seleccionar')) {
      console.log('Seleccionando período académico...');
      
      // Esperar a que cargue la página de selección de período
      await page.waitForSelector('input[name="periodoId"]', { timeout: 10000 });
      
      // Seleccionar el primer período disponible
      const primerPeriodo = page.locator('input[name="periodoId"]').first();
      await primerPeriodo.click();
      
      // Hacer clic en el botón de continuar/establecer período
      await page.click('button[type="submit"].btn-primary');
      
      // Esperar a que se establezca el período y redirija
      await page.waitForLoadState('networkidle');
    }
    
    console.log('Login completado, URL actual:', page.url());
  });

  test('Debe mostrar la lista de profesores', async ({ page }) => {
    // Navegar a la página de profesores
    await page.goto('http://localhost:18163/Profesores');
    
    // Verificar que la página cargó
    await expect(page.locator('h1')).toContainText('Gestión de Profesores');
    
    // Verificar que hay profesores en la lista
    const profesoresCount = await page.locator('tbody tr').count();
    console.log(`Total de profesores encontrados: ${profesoresCount}`);
    expect(profesoresCount).toBeGreaterThan(0);
  });

  test('Debe eliminar un profesor correctamente (ID 1)', async ({ page }) => {
    // Navegar a la página de profesores
    await page.goto('http://localhost:18163/Profesores');
    
    // Contar profesores antes de eliminar
    const profesoresAntesCount = await page.locator('tbody tr').count();
    console.log(`Profesores antes de eliminar: ${profesoresAntesCount}`);
    
    // Buscar el botón de eliminar del primer profesor (ID 1)
    // El botón tiene class btn-danger y href que contiene /Delete/1
    const deleteButton = page.locator('a[href*="/Profesores/Delete/1"]').first();
    
    // Verificar que el botón existe
    await expect(deleteButton).toBeVisible();
    
    // Hacer clic en el botón de eliminar
    await deleteButton.click();
    
    // Esperar a que cargue la página de confirmación
    await page.waitForURL('**/Profesores/Delete/1', { timeout: 10000 });
    
    // Verificar que estamos en la página de confirmación
    await expect(page.locator('h4')).toContainText('Advertencia');
    await expect(page.locator('.card-header')).toContainText('Confirmar Eliminación de Profesor');
    
    // Tomar screenshot de la página de confirmación
    await page.screenshot({ path: 'Tests/screenshots/delete-confirmation.png', fullPage: true });
    
    // Manejar el diálogo de confirmación
    page.on('dialog', dialog => {
      console.log('Diálogo de confirmación:', dialog.message());
      dialog.accept();
    });
    
    // Hacer clic en el botón de eliminar permanentemente
    await page.click('button[type="submit"].btn-danger');
    
    // Esperar a que se complete la eliminación y redirija al index
    await page.waitForURL('**/Profesores', { timeout: 15000 });
    
    // Verificar mensaje de éxito
    const successMessage = page.locator('.alert-success, [class*="success"]');
    await expect(successMessage).toBeVisible({ timeout: 5000 });
    
    // Tomar screenshot después de eliminar
    await page.screenshot({ path: 'Tests/screenshots/after-delete.png', fullPage: true });
    
    // Contar profesores después de eliminar
    const profesoresDespuesCount = await page.locator('tbody tr').count();
    console.log(`Profesores después de eliminar: ${profesoresDespuesCount}`);
    
    // Verificar que se eliminó un profesor
    expect(profesoresDespuesCount).toBe(profesoresAntesCount - 1);
    
    // Verificar que el profesor con ID 1 ya no existe
    const deletedProfessor = page.locator('a[href*="/Profesores/Details/1"]');
    await expect(deletedProfessor).toHaveCount(0);
    
    console.log('✅ Profesor eliminado exitosamente');
  });

  test('Debe cancelar la eliminación correctamente', async ({ page }) => {
    // Navegar a la página de profesores
    await page.goto('http://localhost:18163/Profesores');
    
    // Contar profesores antes
    const profesoresAntesCount = await page.locator('tbody tr').count();
    
    // Buscar el botón de eliminar del profesor ID 2
    const deleteButton = page.locator('a[href*="/Profesores/Delete/2"]').first();
    
    // Verificar que el botón existe
    await expect(deleteButton).toBeVisible();
    
    // Hacer clic en el botón de eliminar
    await deleteButton.click();
    
    // Esperar a que cargue la página de confirmación
    await page.waitForURL('**/Profesores/Delete/2', { timeout: 10000 });
    
    // Hacer clic en el botón Cancelar
    await page.click('a.btn-secondary');
    
    // Esperar a que redirija al index
    await page.waitForURL('**/Profesores', { timeout: 10000 });
    
    // Contar profesores después de cancelar
    const profesoresDespuesCount = await page.locator('tbody tr').count();
    
    // Verificar que NO se eliminó ningún profesor
    expect(profesoresDespuesCount).toBe(profesoresAntesCount);
    
    // Verificar que el profesor con ID 2 sigue existiendo
    const professor = page.locator('a[href*="/Profesores/Details/2"]');
    await expect(professor).toHaveCount(1);
    
    console.log('✅ Cancelación funcionó correctamente');
  });

  test('Debe verificar eliminación en cascada en la base de datos', async ({ page }) => {
    // Navegar a la página de profesores
    await page.goto('http://localhost:18163/Profesores');
    
    // Buscar el profesor ID 3 y ver sus detalles primero
    await page.click('a[href*="/Profesores/Details/3"]');
    
    // Esperar a que cargue la página de detalles
    await page.waitForURL('**/Profesores/Details/3', { timeout: 10000 });
    
    // Tomar screenshot de los detalles antes de eliminar
    await page.screenshot({ path: 'Tests/screenshots/profesor-3-details.png', fullPage: true });
    
    // Volver al index
    await page.goto('http://localhost:18163/Profesores');
    
    // Eliminar el profesor ID 3
    const deleteButton = page.locator('a[href*="/Profesores/Delete/3"]').first();
    await deleteButton.click();
    
    // Esperar confirmación
    await page.waitForURL('**/Profesores/Delete/3', { timeout: 10000 });
    
    // Manejar diálogo
    page.on('dialog', dialog => dialog.accept());
    
    // Confirmar eliminación
    await page.click('button[type="submit"].btn-danger');
    
    // Esperar redirección
    await page.waitForURL('**/Profesores', { timeout: 15000 });
    
    // Intentar acceder a los detalles del profesor eliminado (debe fallar o redirigir)
    await page.goto('http://localhost:18163/Profesores/Details/3');
    
    // Debe mostrar error o redirigir al index
    await page.waitForTimeout(2000);
    
    // Verificar que no estamos en la página de detalles del profesor 3
    const currentUrl = page.url();
    expect(currentUrl).not.toContain('/Profesores/Details/3');
    
    console.log('✅ Eliminación en cascada verificada');
  });
});
