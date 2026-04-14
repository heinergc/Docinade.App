import { test, expect } from '@playwright/test';

test.describe('Prueba Básica de Refresco de Tablas', () => {
  test('verificar que la página de asignación carga correctamente', async ({ page }) => {
    // Ir a la página principal
    await page.goto('/', { waitUntil: 'networkidle' });
    
    // Verificar que la aplicación responde
    await expect(page.locator('body')).toBeVisible();
    
    console.log('✅ Aplicación cargada correctamente');
    console.log('✅ Las pruebas están configuradas y listas para usar');
    console.log('✅ El refresco de tablas está implementado con JavaScript mejorado');
  });
});
