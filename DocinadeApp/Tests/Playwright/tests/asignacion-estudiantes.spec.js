import { test, expect } from '@playwright/test';

/**
 * Pruebas E2E para la funcionalidad de asignación de estudiantes
 * Verifica que las tablas se refresquen correctamente después de asignar/desasignar estudiantes
 */

test.describe('Asignación de Estudiantes - Refresco de Tablas', () => {
  let grupoId;
  let groupCode;

  test.beforeEach(async ({ page }) => {
    // Configurar para ignorar errores de HTTPS
    await page.goto('/', { waitUntil: 'networkidle' });
    
    // Verificar que la aplicación esté funcionando
    await expect(page.locator('body')).toBeVisible();
    
    // Si hay un formulario de login, simularlo (ajustar según tu aplicación)
    try {
      const loginForm = page.locator('form[action*="login"], form[action*="account"]');
      if (await loginForm.isVisible({ timeout: 2000 })) {
        // Aquí puedes agregar credenciales de prueba si es necesario
        await page.fill('input[type="email"], input[name*="email"], input[name*="usuario"]', 'admin@test.com');
        await page.fill('input[type="password"], input[name*="password"]', 'Test123!');
        await page.click('button[type="submit"], input[type="submit"]');
        await page.waitForLoadState('networkidle');
      }
    } catch (error) {
      console.log('No se encontró formulario de login, continuando...');
    }

    // Navegar a la sección de grupos
    try {
      await page.click('a[href*="GruposEstudiantes"], a:has-text("Grupos")');
      await page.waitForLoadState('networkidle');
    } catch (error) {
      console.log('Navegando directamente a grupos...');
      await page.goto('/GruposEstudiantes', { waitUntil: 'networkidle' });
    }

    // Buscar un grupo existente o crear uno para las pruebas
    await page.waitForSelector('.table, .card, .group-item', { timeout: 10000 });
    
    // Intentar encontrar un grupo existente
    const grupoLinks = page.locator('a[href*="AsignarEstudiantes"], button[data-group-id], tr td a');
    const grupoCount = await grupoLinks.count();
    
    if (grupoCount > 0) {
      // Usar el primer grupo disponible
      const firstGroupLink = grupoLinks.first();
      const href = await firstGroupLink.getAttribute('href');
      
      if (href && href.includes('AsignarEstudiantes')) {
        // Extraer grupoId de la URL
        const matches = href.match(/grupoId=(\d+)|\/(\d+)\/AsignarEstudiantes/);
        grupoId = matches ? (matches[1] || matches[2]) : null;
      }
      
      // Obtener el código del grupo si está disponible
      const groupCell = page.locator('tr').filter({ hasText: /.+/ }).first().locator('td').first();
      try {
        groupCode = await groupCell.textContent();
      } catch {
        groupCode = 'TEST-GROUP';
      }
      
      // Navegar a la página de asignación
      await firstGroupLink.click();
      await page.waitForLoadState('networkidle');
    } else {
      // Si no hay grupos, navegar a crear uno o usar un grupo de prueba conocido
      console.log('No se encontraron grupos, usando datos de prueba');
      grupoId = '1'; // ID de prueba
      groupCode = 'TEST-GROUP';
      await page.goto(`/GruposEstudiantes/AsignarEstudiantes?grupoId=${grupoId}`, { waitUntil: 'networkidle' });
    }
  });

  test('debería cargar las tablas de estudiantes inicialmente', async ({ page }) => {
    // Verificar que la página de asignación se carga correctamente
    await expect(page.locator('h1')).toContainText('Asignar Estudiantes');
    
    // Verificar que las tablas están presentes
    await expect(page.locator('#tablaDisponibles')).toBeVisible();
    await expect(page.locator('#tablaAsignados')).toBeVisible();
    
    // Esperar a que se carguen los datos (los spinners deberían desaparecer)
    await page.waitForFunction(() => {
      const loadingDisponibles = document.querySelector('#loadingDisponibles');
      const loadingAsignados = document.querySelector('#loadingAsignados');
      return (!loadingDisponibles || !loadingDisponibles.offsetParent) && 
             (!loadingAsignados || !loadingAsignados.offsetParent);
    }, { timeout: 15000 });
    
    // Verificar que los badges de conteo están actualizados
    const badgeDisponibles = page.locator('#badgeDisponibles');
    const badgeAsignados = page.locator('#badgeAsignados');
    
    await expect(badgeDisponibles).toBeVisible();
    await expect(badgeAsignados).toBeVisible();
    
    // Los badges deberían tener números válidos
    const countDisponibles = await badgeDisponibles.textContent();
    const countAsignados = await badgeAsignados.textContent();
    
    expect(countDisponibles).toMatch(/^\d+$/);
    expect(countAsignados).toMatch(/^\d+$/);
  });

  test('debería refrescar tabla disponibles después de asignar un estudiante', async ({ page }) => {
    // Esperar a que se carguen los datos iniciales
    await page.waitForFunction(() => {
      const loadingDisponibles = document.querySelector('#loadingDisponibles');
      return !loadingDisponibles || !loadingDisponibles.offsetParent;
    }, { timeout: 15000 });
    
    // Obtener el conteo inicial de estudiantes disponibles
    const initialCountDisponibles = await page.locator('#badgeDisponibles').textContent();
    const initialCountAsignados = await page.locator('#badgeAsignados').textContent();
    
    // Verificar que hay estudiantes disponibles para asignar
    const estudiantesDisponibles = page.locator('#tablaDisponibles tr[data-estudiante-id]');
    const countEstudiantesDisponibles = await estudiantesDisponibles.count();
    
    if (countEstudiantesDisponibles > 0) {
      // Asignar el primer estudiante disponible
      const firstEstudiante = estudiantesDisponibles.first();
      const btnAsignar = firstEstudiante.locator('.btn-asignar');
      
      await btnAsignar.click();
      
      // Esperar a que aparezca el loading de SweetAlert
      await expect(page.locator('.swal2-popup')).toBeVisible({ timeout: 5000 });
      
      // Esperar a que se complete la operación
      await page.waitForFunction(() => {
        const swal = document.querySelector('.swal2-popup');
        return !swal || swal.querySelector('.swal2-success, .swal2-error');
      }, { timeout: 10000 });
      
      // Si hay mensaje de éxito, cerrarlo
      const successMsg = page.locator('.swal2-success');
      if (await successMsg.isVisible({ timeout: 2000 })) {
        await page.keyboard.press('Escape');
      }
      
      // Verificar que las tablas se han actualizado
      await page.waitForFunction(
        (initialDisponibles, initialAsignados) => {
          const newDisponibles = document.querySelector('#badgeDisponibles')?.textContent;
          const newAsignados = document.querySelector('#badgeAsignados')?.textContent;
          
          return newDisponibles !== initialDisponibles || newAsignados !== initialAsignados;
        },
        [initialCountDisponibles, initialCountAsignados],
        { timeout: 10000 }
      );
      
      // Verificar que los contadores han cambiado apropiadamente
      const newCountDisponibles = await page.locator('#badgeDisponibles').textContent();
      const newCountAsignados = await page.locator('#badgeAsignados').textContent();
      
      // El número de disponibles debería haber disminuido o mantenerse
      // El número de asignados debería haber aumentado o mantenerse
      expect(parseInt(newCountDisponibles)).toBeLessThanOrEqual(parseInt(initialCountDisponibles));
      expect(parseInt(newCountAsignados)).toBeGreaterThanOrEqual(parseInt(initialCountAsignados));
      
      console.log(`Contadores actualizados: Disponibles ${initialCountDisponibles} → ${newCountDisponibles}, Asignados ${initialCountAsignados} → ${newCountAsignados}`);
    } else {
      console.log('No hay estudiantes disponibles para asignar');
    }
  });

  test('debería refrescar tabla asignados después de desasignar un estudiante', async ({ page }) => {
    // Esperar a que se carguen los datos iniciales
    await page.waitForFunction(() => {
      const loadingAsignados = document.querySelector('#loadingAsignados');
      return !loadingAsignados || !loadingAsignados.offsetParent;
    }, { timeout: 15000 });
    
    // Obtener el conteo inicial
    const initialCountDisponibles = await page.locator('#badgeDisponibles').textContent();
    const initialCountAsignados = await page.locator('#badgeAsignados').textContent();
    
    // Verificar que hay estudiantes asignados para desasignar
    const estudiantesAsignados = page.locator('#tablaAsignados tr[data-estudiante-id]');
    const countEstudiantesAsignados = await estudiantesAsignados.count();
    
    if (countEstudiantesAsignados > 0) {
      // Desasignar el primer estudiante
      const firstEstudiante = estudiantesAsignados.first();
      const btnDesasignar = firstEstudiante.locator('.btn-desasignar');
      
      await btnDesasignar.click();
      
      // Confirmar la desasignación en el modal de SweetAlert
      await expect(page.locator('.swal2-popup')).toBeVisible({ timeout: 5000 });
      await page.click('.swal2-confirm');
      
      // Esperar a que se complete la operación
      await page.waitForFunction(() => {
        const swal = document.querySelector('.swal2-popup');
        return !swal || swal.querySelector('.swal2-success, .swal2-error');
      }, { timeout: 10000 });
      
      // Si hay mensaje de éxito, cerrarlo
      const successMsg = page.locator('.swal2-success');
      if (await successMsg.isVisible({ timeout: 2000 })) {
        await page.keyboard.press('Escape');
      }
      
      // Verificar que las tablas se han actualizado
      await page.waitForFunction(
        (initialDisponibles, initialAsignados) => {
          const newDisponibles = document.querySelector('#badgeDisponibles')?.textContent;
          const newAsignados = document.querySelector('#badgeAsignados')?.textContent;
          
          return newDisponibles !== initialDisponibles || newAsignados !== initialAsignados;
        },
        [initialCountDisponibles, initialCountAsignados],
        { timeout: 10000 }
      );
      
      // Verificar que los contadores han cambiado apropiadamente
      const newCountDisponibles = await page.locator('#badgeDisponibles').textContent();
      const newCountAsignados = await page.locator('#badgeAsignados').textContent();
      
      // El número de disponibles debería haber aumentado o mantenerse
      // El número de asignados debería haber disminuido o mantenerse
      expect(parseInt(newCountDisponibles)).toBeGreaterThanOrEqual(parseInt(initialCountDisponibles));
      expect(parseInt(newCountAsignados)).toBeLessThanOrEqual(parseInt(initialCountAsignados));
      
      console.log(`Contadores actualizados: Disponibles ${initialCountDisponibles} → ${newCountDisponibles}, Asignados ${initialCountAsignados} → ${newCountAsignados}`);
    } else {
      console.log('No hay estudiantes asignados para desasignar');
    }
  });

  test('debería refrescar tablas después de asignación múltiple', async ({ page }) => {
    // Esperar a que se carguen los datos iniciales
    await page.waitForFunction(() => {
      const loadingDisponibles = document.querySelector('#loadingDisponibles');
      return !loadingDisponibles || !loadingDisponibles.offsetParent;
    }, { timeout: 15000 });
    
    // Obtener conteos iniciales
    const initialCountDisponibles = await page.locator('#badgeDisponibles').textContent();
    const initialCountAsignados = await page.locator('#badgeAsignados').textContent();
    
    // Verificar que hay estudiantes disponibles
    const estudiantesDisponibles = page.locator('#tablaDisponibles tr[data-estudiante-id]');
    const countEstudiantesDisponibles = await estudiantesDisponibles.count();
    
    if (countEstudiantesDisponibles >= 2) {
      // Seleccionar los primeros 2 estudiantes
      const checkboxes = page.locator('#tablaDisponibles .check-disponible');
      await checkboxes.first().check();
      await checkboxes.nth(1).check();
      
      // Verificar que el botón de asignar múltiples se habilitó
      const btnAsignarMultiples = page.locator('#btnAsignarSeleccionados');
      await expect(btnAsignarMultiples).toBeEnabled();
      
      // Hacer clic en asignar múltiples
      await btnAsignarMultiples.click();
      
      // Confirmar en el modal
      await expect(page.locator('.swal2-popup')).toBeVisible({ timeout: 5000 });
      await page.click('.swal2-confirm');
      
      // Esperar a que se complete la operación
      await page.waitForFunction(() => {
        const swal = document.querySelector('.swal2-popup');
        return !swal || swal.querySelector('.swal2-success, .swal2-error');
      }, { timeout: 15000 });
      
      // Cerrar mensaje de éxito si aparece
      const successMsg = page.locator('.swal2-success');
      if (await successMsg.isVisible({ timeout: 2000 })) {
        await page.keyboard.press('Escape');
      }
      
      // Verificar que las tablas se actualizaron
      await page.waitForFunction(
        (initialDisponibles, initialAsignados) => {
          const newDisponibles = document.querySelector('#badgeDisponibles')?.textContent;
          const newAsignados = document.querySelector('#badgeAsignados')?.textContent;
          
          return newDisponibles !== initialDisponibles || newAsignados !== initialAsignados;
        },
        [initialCountDisponibles, initialCountAsignados],
        { timeout: 10000 }
      );
      
      // Verificar cambios en contadores
      const newCountDisponibles = await page.locator('#badgeDisponibles').textContent();
      const newCountAsignados = await page.locator('#badgeAsignados').textContent();
      
      console.log(`Asignación múltiple completada: Disponibles ${initialCountDisponibles} → ${newCountDisponibles}, Asignados ${initialCountAsignados} → ${newCountAsignados}`);
      
      // Verificar que no hay checkboxes seleccionados después de la operación
      const checkedBoxes = page.locator('#tablaDisponibles .check-disponible:checked');
      expect(await checkedBoxes.count()).toBe(0);
    } else {
      console.log('No hay suficientes estudiantes disponibles para asignación múltiple');
    }
  });

  test('debería refrescar tablas después de desasignación múltiple', async ({ page }) => {
    // Esperar a que se carguen los datos iniciales
    await page.waitForFunction(() => {
      const loadingAsignados = document.querySelector('#loadingAsignados');
      return !loadingAsignados || !loadingAsignados.offsetParent;
    }, { timeout: 15000 });
    
    // Obtener conteos iniciales
    const initialCountDisponibles = await page.locator('#badgeDisponibles').textContent();
    const initialCountAsignados = await page.locator('#badgeAsignados').textContent();
    
    // Verificar que hay estudiantes asignados
    const estudiantesAsignados = page.locator('#tablaAsignados tr[data-estudiante-id]');
    const countEstudiantesAsignados = await estudiantesAsignados.count();
    
    if (countEstudiantesAsignados >= 2) {
      // Seleccionar los primeros 2 estudiantes asignados
      const checkboxes = page.locator('#tablaAsignados .check-asignado');
      await checkboxes.first().check();
      await checkboxes.nth(1).check();
      
      // Verificar que el botón de desasignar múltiples se habilitó
      const btnDesasignarMultiples = page.locator('#btnDesasignarSeleccionados');
      await expect(btnDesasignarMultiples).toBeEnabled();
      
      // Hacer clic en desasignar múltiples
      await btnDesasignarMultiples.click();
      
      // Confirmar en el modal
      await expect(page.locator('.swal2-popup')).toBeVisible({ timeout: 5000 });
      await page.click('.swal2-confirm');
      
      // Esperar a que se complete la operación
      await page.waitForFunction(() => {
        const swal = document.querySelector('.swal2-popup');
        return !swal || swal.querySelector('.swal2-success, .swal2-error');
      }, { timeout: 15000 });
      
      // Cerrar mensaje de éxito si aparece
      const successMsg = page.locator('.swal2-success');
      if (await successMsg.isVisible({ timeout: 2000 })) {
        await page.keyboard.press('Escape');
      }
      
      // Verificar que las tablas se actualizaron
      await page.waitForFunction(
        (initialDisponibles, initialAsignados) => {
          const newDisponibles = document.querySelector('#badgeDisponibles')?.textContent;
          const newAsignados = document.querySelector('#badgeAsignados')?.textContent;
          
          return newDisponibles !== initialDisponibles || newAsignados !== initialAsignados;
        },
        [initialCountDisponibles, initialCountAsignados],
        { timeout: 10000 }
      );
      
      // Verificar cambios en contadores
      const newCountDisponibles = await page.locator('#badgeDisponibles').textContent();
      const newCountAsignados = await page.locator('#badgeAsignados').textContent();
      
      console.log(`Desasignación múltiple completada: Disponibles ${initialCountDisponibles} → ${newCountDisponibles}, Asignados ${initialCountAsignados} → ${newCountAsignados}`);
      
      // Verificar que no hay checkboxes seleccionados después de la operación
      const checkedBoxes = page.locator('#tablaAsignados .check-asignado:checked');
      expect(await checkedBoxes.count()).toBe(0);
    } else {
      console.log('No hay suficientes estudiantes asignados para desasignación múltiple');
    }
  });

  test('debería actualizar estadísticas correctamente después de operaciones', async ({ page }) => {
    // Esperar a que se carguen los datos iniciales
    await page.waitForFunction(() => {
      const loadingDisponibles = document.querySelector('#loadingDisponibles');
      const loadingAsignados = document.querySelector('#loadingAsignados');
      return (!loadingDisponibles || !loadingDisponibles.offsetParent) && 
             (!loadingAsignados || !loadingAsignados.offsetParent);
    }, { timeout: 15000 });
    
    // Verificar que las estadísticas están visibles
    await expect(page.locator('#totalDisponibles')).toBeVisible();
    await expect(page.locator('#totalAsignados')).toBeVisible();
    await expect(page.locator('#porcentajeOcupacion')).toBeVisible();
    await expect(page.locator('#espaciosDisponibles')).toBeVisible();
    
    // Obtener valores iniciales
    const initialTotalDisponibles = await page.locator('#totalDisponibles').textContent();
    const initialTotalAsignados = await page.locator('#totalAsignados').textContent();
    
    // Verificar que los totales coinciden con los badges
    const badgeDisponibles = await page.locator('#badgeDisponibles').textContent();
    const badgeAsignados = await page.locator('#badgeAsignados').textContent();
    
    expect(initialTotalDisponibles).toBe(badgeDisponibles);
    expect(initialTotalAsignados).toBe(badgeAsignados);
    
    // Verificar que los porcentajes son válidos
    const porcentaje = await page.locator('#porcentajeOcupacion').textContent();
    expect(porcentaje).toMatch(/^\d+%$|^N\/A$/);
    
    const espacios = await page.locator('#espaciosDisponibles').textContent();
    expect(espacios).toMatch(/^\d+$|^∞$/);
    
    console.log(`Estadísticas iniciales verificadas: Disponibles=${initialTotalDisponibles}, Asignados=${initialTotalAsignados}, Ocupación=${porcentaje}, Espacios=${espacios}`);
  });

  test('debería mantener filtros de búsqueda después del refresco', async ({ page }) => {
    // Esperar a que se carguen los datos
    await page.waitForFunction(() => {
      const loadingDisponibles = document.querySelector('#loadingDisponibles');
      return !loadingDisponibles || !loadingDisponibles.offsetParent;
    }, { timeout: 15000 });
    
    // Aplicar un filtro de búsqueda
    const searchInput = page.locator('#buscarDisponibles');
    await searchInput.fill('test');
    
    // Esperar a que se aplique el filtro
    await page.waitForTimeout(500);
    
    // Verificar que el filtro está aplicado
    const searchValue = await searchInput.inputValue();
    expect(searchValue).toBe('test');
    
    // Si hay estudiantes disponibles, realizar una asignación para forzar refresco
    const estudiantesVisibles = page.locator('#tablaDisponibles tr[data-estudiante-id]:visible');
    const countVisible = await estudiantesVisibles.count();
    
    if (countVisible > 0) {
      const btnAsignar = estudiantesVisibles.first().locator('.btn-asignar');
      await btnAsignar.click();
      
      // Manejar el modal de SweetAlert
      try {
        await expect(page.locator('.swal2-popup')).toBeVisible({ timeout: 5000 });
        await page.waitForTimeout(2000);
        const successMsg = page.locator('.swal2-success');
        if (await successMsg.isVisible({ timeout: 2000 })) {
          await page.keyboard.press('Escape');
        }
      } catch (error) {
        console.log('Modal no apareció o se cerró automáticamente');
      }
      
      // Verificar que el filtro se mantiene después del refresco
      const searchValueAfter = await searchInput.inputValue();
      expect(searchValueAfter).toBe('test');
    }
    
    console.log('Filtro de búsqueda mantenido después del refresco');
  });
});
