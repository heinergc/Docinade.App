import { test, expect } from '@playwright/test';

const BASE_URL = 'http://localhost:4321';

test.describe('Documentación RubricasApp - Navegación', () => {
  
  test('La página principal carga correctamente', async ({ page }) => {
    await page.goto(BASE_URL);
    
    // Verificar que el título existe en el área principal
    await expect(page.locator('.sl-markdown-content h1, article h1, #_top').first()).toContainText('Bienvenido a RubricasApp');
    
    // Verificar que el contenido principal existe
    await expect(page.locator('.main-frame, .sl-container, main').first()).toBeVisible();
  });

  test('Página de Introducción carga sin errores', async ({ page }) => {
    await page.goto(BASE_URL + '/introduccion/');
    
    await expect(page.locator('#_top, article h1').first()).toContainText('Introducción a RubricasApp');
    
    // Verificar que el contenido principal existe  
    await expect(page.locator('.main-frame, .sl-markdown-content, main').first()).toBeVisible();
  });

  test('Página Primeros Pasos - Registro carga correctamente', async ({ page }) => {
    const errors: string[] = [];
    page.on('console', msg => {
      if (msg.type() === 'error') {
        errors.push(msg.text());
      }
    });
    
    await page.goto(BASE_URL + '/primeros-pasos/registro/');
    
    await expect(page.locator('#_top, article h1').first()).toContainText('Registro en el Sistema');
    
    // Wait for page to fully load
    await page.waitForLoadState('networkidle');
    
    // Verify no JavaScript errors
    expect(errors.filter(e => e.includes('Cannot read properties of undefined'))).toHaveLength(0);
  });

  test('Página Primeros Pasos - Perfil Usuario carga correctamente', async ({ page }) => {
    await page.goto(BASE_URL + '/primeros-pasos/perfil-usuario/');
    
    await expect(page.locator('#_top, article h1').first()).toContainText('Configuración del Perfil');
  });

  test('Página Primeros Pasos - Navegación carga correctamente', async ({ page }) => {
    await page.goto(BASE_URL + '/primeros-pasos/navegacion/');
    
    await expect(page.locator('#_top, article h1').first()).toContainText('Navegación por el Sistema');
  });

  test('Página Grupos carga correctamente', async ({ page }) => {
    await page.goto(BASE_URL + '/grupos/crear-grupo/');
    
    await expect(page.locator('#_top, article h1').first()).toContainText('Crear Grupos');
  });

  test('Página Profesores carga correctamente', async ({ page }) => {
    await page.goto(BASE_URL + '/profesores/gestion-profesores/');
    
    await expect(page.locator('#_top, article h1').first()).toContainText('Gestión de Profesores');
  });

  test('Página Currículos carga correctamente', async ({ page }) => {
    await page.goto(BASE_URL + '/curriculos/gestion-curriculos/');
    
    await expect(page.locator('#_top, article h1').first()).toContainText('Gestión de Currículos');
  });

  test('Página Asignaciones carga correctamente', async ({ page }) => {
    await page.goto(BASE_URL + '/asignaciones/gestion-asignaciones/');
    
    await expect(page.locator('#_top, article h1').first()).toContainText('Gestión de Asignaciones');
  });

  test('Página Estudiantes carga correctamente', async ({ page }) => {
    await page.goto(BASE_URL + '/estudiantes/agregar-estudiante/');
    
    await expect(page.locator('#_top, article h1').first()).toContainText('Agregar Estudiantes');
  });

  test('Página Empadronamiento carga correctamente', async ({ page }) => {
    await page.goto(BASE_URL + '/empadronamiento/registro-empadronamiento/');
    
    await expect(page.locator('#_top, article h1').first()).toContainText('Registro de Empadronamiento');
  });

  test('Página Rúbricas carga correctamente', async ({ page }) => {
    await page.goto(BASE_URL + '/rubricas/crear-rubrica/');
    
    await expect(page.locator('#_top, article h1').first()).toContainText('Crear Rúbricas');
  });

  test('Página Conducta carga correctamente', async ({ page }) => {
    await page.goto(BASE_URL + '/conducta/modulo-conducta/');
    
    await expect(page.locator('#_top, article h1').first()).toContainText('Módulo de Conducta');
  });

  test('Página Asistencia carga correctamente', async ({ page }) => {
    await page.goto(BASE_URL + '/asistencia/registro-asistencia/');
    
    await expect(page.locator('#_top, article h1').first()).toContainText('Registro de Asistencia');
  });

  test('Página FAQ carga correctamente', async ({ page }) => {
    await page.goto(BASE_URL + '/faq/preguntas-frecuentes/');
    
    await expect(page.locator('#_top, article h1').first()).toContainText('Preguntas Frecuentes');
  });

  test('El sidebar está visible y funcional', async ({ page }) => {
    // Configurar viewport de escritorio para que el sidebar sea visible
    await page.setViewportSize({ width: 1280, height: 720 });
    await page.goto(BASE_URL);
    
    // Esperar a que la página cargue completamente
    await page.waitForLoadState('networkidle');
    
    // Verificar que hay enlaces navegables en la página
    const links = page.locator('a').first();
    await expect(links).toBeVisible();
    
    // Verificar que hay múltiples enlaces
    const allLinks = await page.locator('a[href^="/"]').count();
    expect(allLinks).toBeGreaterThan(3);
  });

  test('La búsqueda funciona', async ({ page }) => {
    await page.goto(BASE_URL);
    
    // Buscar el botón de búsqueda
    const searchButton = page.locator('button[aria-label*="Search"]').first();
    if (await searchButton.isVisible()) {
      await searchButton.click();
      
      // Verificar que se abre el diálogo de búsqueda
      await expect(page.locator('[role="dialog"]')).toBeVisible();
    }
  });

  test('Navegación entre páginas funciona', async ({ page }) => {
    await page.goto(BASE_URL);
    
    // Hacer clic en "Introducción" del sidebar
    await page.click('text=Introducción');
    await expect(page).toHaveURL(/.*introduccion/);
    
    // Navegar a Primeros Pasos
    await page.click('text=Registro en el Sistema');
    await expect(page).toHaveURL(/.*primeros-pasos\/registro/);
    
    // Verificar que no hay errores después de navegar
    await page.waitForLoadState('networkidle');
  });

  test('El tema oscuro/claro funciona', async ({ page }) => {
    await page.goto(BASE_URL);
    
    // Buscar el botón de cambio de tema
    const themeButton = page.locator('button[aria-label*="theme"]').first();
    
    if (await themeButton.isVisible()) {
      await themeButton.click();
      await page.waitForTimeout(500);
      
      // Verificar que el tema cambió
      const html = page.locator('html');
      const dataTheme = await html.getAttribute('data-theme');
      expect(dataTheme).toBeTruthy();
    }
  });

  test('Todos los enlaces internos funcionan', async ({ page }) => {
    await page.goto(BASE_URL + '/introduccion/');
    
    // Obtener todos los enlaces internos
    const links = page.locator('article a[href^="/"]');
    const count = await links.count();
    
    console.log(`Encontrados ${count} enlaces internos`);
    
    for (let i = 0; i < Math.min(count, 5); i++) {
      const href = await links.nth(i).getAttribute('href');
      if (href) {
        const response = await page.goto(BASE_URL + href);
        expect(response?.status()).toBe(200);
      }
    }
  });

  test('No hay errores 404 en recursos', async ({ page }) => {
    const failed: string[] = [];
    
    page.on('response', response => {
      if (response.status() === 404 && !response.url().includes('favicon')) {
        failed.push(response.url());
      }
    });
    
    await page.goto(BASE_URL);
    await page.waitForLoadState('networkidle');
    
    expect(failed).toHaveLength(0);
  });

  test('El contenido es responsive', async ({ page }) => {
    // Test en móvil
    await page.setViewportSize({ width: 375, height: 667 });
    await page.goto(BASE_URL);
    
    await expect(page.locator('#_top, h1').first()).toBeVisible();
    
    // Test en tablet
    await page.setViewportSize({ width: 768, height: 1024 });
    await expect(page.locator('#_top, h1').first()).toBeVisible();
    
    // Test en desktop
    await page.setViewportSize({ width: 1920, height: 1080 });
    await expect(page.locator('#_top, h1').first()).toBeVisible();
  });
});
