import { test, expect, Page } from '@playwright/test';

/**
 * Pruebas E2E para la funcionalidad de Experiencia Laboral Previa
 * Paso 5 del wizard de creación de profesores
 */

// Datos de prueba
const DATOS_PROFESOR_BASE = {
  cedula: '112340000',
  nombres: 'Juan',
  primerApellido: 'Pérez',
  segundoApellido: 'González',
  email: 'juan.perez@test.com',
  telefono: '88887777'
};

const EXPERIENCIA_1 = {
  nombreInstitucion: 'Universidad Nacional',
  cargoDesempenado: 'Profesor de Matemáticas',
  tipoInstitucion: 'Pública',
  fechaInicio: '2020-01',
  fechaFin: '2023-12',
  trabajandoActualmente: false,
  descripcionFunciones: 'Impartir clases de cálculo y álgebra',
  tipoContrato: 'Tiempo Completo',
  jornadaLaboral: 'Diurna'
};

const EXPERIENCIA_2 = {
  nombreInstitucion: 'Colegio San José',
  cargoDesempenado: 'Director Académico',
  tipoInstitucion: 'Privada',
  fechaInicio: '2024-01',
  fechaFin: '', // Vacío porque trabaja actualmente
  trabajandoActualmente: true,
  descripcionFunciones: 'Coordinar programas académicos',
  tipoContrato: 'Tiempo Completo',
  jornadaLaboral: 'Mixta'
};

/**
 * Navegar y completar los primeros pasos del wizard
 */
async function navegarAStep5(page: Page) {
  // Navegar a la página de inicio
  await page.goto('/Profesores/Create');
  
  // Step 1: Información Personal
  await page.fill('#Cedula', DATOS_PROFESOR_BASE.cedula);
  await page.fill('#Nombres', DATOS_PROFESOR_BASE.nombres);
  await page.fill('#PrimerApellido', DATOS_PROFESOR_BASE.primerApellido);
  await page.fill('#SegundoApellido', DATOS_PROFESOR_BASE.segundoApellido);
  await page.click('button[type="submit"]');
  
  // Step 2: Información de Contacto
  await page.waitForURL('**/CreateStep2');
  await page.fill('#Email', DATOS_PROFESOR_BASE.email);
  await page.fill('#Telefono', DATOS_PROFESOR_BASE.telefono);
  await page.click('button[type="submit"]');
  
  // Step 3: Información de Dirección
  await page.waitForURL('**/CreateStep3');
  await page.selectOption('#provinciaSelect', { index: 1 });
  await page.waitForTimeout(500); // Esperar carga de cantones
  await page.selectOption('#cantonSelect', { index: 1 });
  await page.waitForTimeout(500); // Esperar carga de distritos
  await page.selectOption('#distritoSelect', { index: 1 });
  await page.click('button[type="submit"]');
  
  // Step 4: Información Académica
  await page.waitForURL('**/CreateStep4');
  await page.click('button[type="submit"]');
  
  // Step 5: Experiencia Laboral
  await page.waitForURL('**/CreateStep5');
}

/**
 * Llenar el formulario modal de experiencia laboral
 */
async function llenarModalExperiencia(page: Page, experiencia: typeof EXPERIENCIA_1) {
  await page.fill('#nombreInstitucion', experiencia.nombreInstitucion);
  await page.fill('#cargoDesempenado', experiencia.cargoDesempenado);
  await page.selectOption('#tipoInstitucion', experiencia.tipoInstitucion);
  await page.fill('#fechaInicio', experiencia.fechaInicio);
  
  if (experiencia.trabajandoActualmente) {
    await page.check('#trabajandoActualmente');
  } else {
    await page.uncheck('#trabajandoActualmente');
    if (experiencia.fechaFin) {
      await page.fill('#fechaFin', experiencia.fechaFin);
    }
  }
  
  await page.fill('#descripcionFunciones', experiencia.descripcionFunciones);
  await page.selectOption('#tipoContrato', experiencia.tipoContrato);
  await page.selectOption('#jornadaLaboral', experiencia.jornadaLaboral);
}

test.describe('Experiencia Laboral Previa - CreateStep5', () => {
  
  test.beforeEach(async ({ page }) => {
    // Navegar al paso 5
    await navegarAStep5(page);
  });

  test('Debe mostrar mensaje de alerta cuando no hay experiencias', async ({ page }) => {
    // Verificar que existe el mensaje de alerta
    const alerta = page.locator('#alertSinExperiencias');
    await expect(alerta).toBeVisible();
    await expect(alerta).toContainText('No se ha agregado experiencia laboral previa');
    
    // Verificar que la tabla está oculta
    const contenedorTabla = page.locator('#contenedorTablaExperiencias');
    await expect(contenedorTabla).toHaveClass(/d-none/);
  });

  test('Debe abrir el modal al hacer clic en "Agregar Experiencia Laboral"', async ({ page }) => {
    // Click en el botón
    await page.click('#btnAgregarExperiencia');
    
    // Verificar que el modal está visible
    const modal = page.locator('#modalExperiencia');
    await expect(modal).toBeVisible();
    
    // Verificar que el título del modal es correcto
    await expect(modal.locator('.modal-title')).toContainText('Agregar Experiencia Laboral Previa');
  });

  test('Debe agregar una experiencia laboral correctamente', async ({ page }) => {
    // Abrir modal
    await page.click('#btnAgregarExperiencia');
    await page.waitForSelector('#modalExperiencia', { state: 'visible' });
    
    // Llenar formulario
    await llenarModalExperiencia(page, EXPERIENCIA_1);
    
    // Hacer clic en agregar
    await page.click('#btnAgregarExperienciaModal');
    
    // Esperar que el modal se cierre
    await page.waitForSelector('#modalExperiencia', { state: 'hidden' });
    
    // Verificar que la alerta desapareció
    const alerta = page.locator('#alertSinExperiencias');
    await expect(alerta).toHaveClass(/d-none/);
    
    // Verificar que la tabla está visible
    const contenedorTabla = page.locator('#contenedorTablaExperiencias');
    await expect(contenedorTabla).not.toHaveClass(/d-none/);
    
    // Verificar que la experiencia aparece en la tabla
    const tabla = page.locator('#tablaExperiencias tbody');
    await expect(tabla.locator('tr')).toHaveCount(1);
    await expect(tabla).toContainText(EXPERIENCIA_1.nombreInstitucion);
    await expect(tabla).toContainText(EXPERIENCIA_1.cargoDesempenado);
    await expect(tabla).toContainText(EXPERIENCIA_1.tipoInstitucion);
  });

  test('Debe agregar múltiples experiencias laborales', async ({ page }) => {
    // Agregar primera experiencia
    await page.click('#btnAgregarExperiencia');
    await page.waitForSelector('#modalExperiencia', { state: 'visible' });
    await llenarModalExperiencia(page, EXPERIENCIA_1);
    await page.click('#btnAgregarExperienciaModal');
    await page.waitForSelector('#modalExperiencia', { state: 'hidden' });
    
    // Agregar segunda experiencia
    await page.click('#btnAgregarExperiencia');
    await page.waitForSelector('#modalExperiencia', { state: 'visible' });
    await llenarModalExperiencia(page, EXPERIENCIA_2);
    await page.click('#btnAgregarExperienciaModal');
    await page.waitForSelector('#modalExperiencia', { state: 'hidden' });
    
    // Verificar que hay 2 filas en la tabla
    const tabla = page.locator('#tablaExperiencias tbody');
    await expect(tabla.locator('tr')).toHaveCount(2);
    
    // Verificar que ambas experiencias están presentes
    await expect(tabla).toContainText(EXPERIENCIA_1.nombreInstitucion);
    await expect(tabla).toContainText(EXPERIENCIA_2.nombreInstitucion);
  });

  test('Debe calcular correctamente la duración de la experiencia', async ({ page }) => {
    // Agregar experiencia (2020-01 a 2023-12 = 4 años)
    await page.click('#btnAgregarExperiencia');
    await page.waitForSelector('#modalExperiencia', { state: 'visible' });
    await llenarModalExperiencia(page, EXPERIENCIA_1);
    await page.click('#btnAgregarExperienciaModal');
    await page.waitForSelector('#modalExperiencia', { state: 'hidden' });
    
    // Verificar que aparece la duración calculada
    const tabla = page.locator('#tablaExperiencias tbody');
    const duracion = tabla.locator('tr td').nth(4); // Columna de experiencia
    await expect(duracion).toContainText(/\d+\s+(años?|meses?)/);
  });

  test('Debe mostrar "Actualidad" para experiencias actuales', async ({ page }) => {
    // Agregar experiencia actual
    await page.click('#btnAgregarExperiencia');
    await page.waitForSelector('#modalExperiencia', { state: 'visible' });
    await llenarModalExperiencia(page, EXPERIENCIA_2);
    await page.click('#btnAgregarExperienciaModal');
    await page.waitForSelector('#modalExperiencia', { state: 'hidden' });
    
    // Verificar que aparece "Actualidad"
    const tabla = page.locator('#tablaExperiencias tbody');
    await expect(tabla).toContainText('Actualidad');
  });

  test('Debe eliminar una experiencia al hacer clic en el botón de eliminar', async ({ page }) => {
    // Agregar dos experiencias
    await page.click('#btnAgregarExperiencia');
    await page.waitForSelector('#modalExperiencia', { state: 'visible' });
    await llenarModalExperiencia(page, EXPERIENCIA_1);
    await page.click('#btnAgregarExperienciaModal');
    await page.waitForSelector('#modalExperiencia', { state: 'hidden' });
    
    await page.click('#btnAgregarExperiencia');
    await page.waitForSelector('#modalExperiencia', { state: 'visible' });
    await llenarModalExperiencia(page, EXPERIENCIA_2);
    await page.click('#btnAgregarExperienciaModal');
    await page.waitForSelector('#modalExperiencia', { state: 'hidden' });
    
    // Verificar que hay 2 experiencias
    let tabla = page.locator('#tablaExperiencias tbody');
    await expect(tabla.locator('tr')).toHaveCount(2);
    
    // Configurar el diálogo de confirmación
    page.on('dialog', async dialog => {
      expect(dialog.message()).toContain('¿Está seguro');
      await dialog.accept();
    });
    
    // Hacer clic en el botón de eliminar de la primera fila
    await page.click('#tablaExperiencias tbody tr:first-child button[title*="Eliminar"]');
    
    // Esperar a que se actualice la tabla
    await page.waitForTimeout(500);
    
    // Verificar que ahora solo hay 1 experiencia
    tabla = page.locator('#tablaExperiencias tbody');
    await expect(tabla.locator('tr')).toHaveCount(1);
  });

  test('Debe mostrar alerta nuevamente al eliminar todas las experiencias', async ({ page }) => {
    // Agregar una experiencia
    await page.click('#btnAgregarExperiencia');
    await page.waitForSelector('#modalExperiencia', { state: 'visible' });
    await llenarModalExperiencia(page, EXPERIENCIA_1);
    await page.click('#btnAgregarExperienciaModal');
    await page.waitForSelector('#modalExperiencia', { state: 'hidden' });
    
    // Configurar el diálogo de confirmación
    page.on('dialog', async dialog => {
      await dialog.accept();
    });
    
    // Eliminar la experiencia
    await page.click('#tablaExperiencias tbody tr button[title*="Eliminar"]');
    
    // Esperar a que se actualice
    await page.waitForTimeout(500);
    
    // Verificar que la alerta vuelve a aparecer
    const alerta = page.locator('#alertSinExperiencias');
    await expect(alerta).toBeVisible();
    await expect(alerta).not.toHaveClass(/d-none/);
    
    // Verificar que la tabla está oculta
    const contenedorTabla = page.locator('#contenedorTablaExperiencias');
    await expect(contenedorTabla).toHaveClass(/d-none/);
  });

  test('Debe validar campos requeridos en el modal', async ({ page }) => {
    // Abrir modal
    await page.click('#btnAgregarExperiencia');
    await page.waitForSelector('#modalExperiencia', { state: 'visible' });
    
    // Intentar agregar sin llenar campos
    await page.click('#btnAgregarExperienciaModal');
    
    // El modal debe seguir abierto (no se cierra por validación)
    await expect(page.locator('#modalExperiencia')).toBeVisible();
    
    // Verificar mensajes de validación (atributo required en HTML5)
    const nombreInstitucion = page.locator('#nombreInstitucion');
    const esInvalido = await nombreInstitucion.evaluate((el: HTMLInputElement) => !el.validity.valid);
    expect(esInvalido).toBeTruthy();
  });

  test('Debe deshabilitar fecha fin cuando se marca "Trabajando Actualmente"', async ({ page }) => {
    // Abrir modal
    await page.click('#btnAgregarExperiencia');
    await page.waitForSelector('#modalExperiencia', { state: 'visible' });
    
    // Verificar que fechaFin está habilitado inicialmente
    const fechaFin = page.locator('#fechaFin');
    await expect(fechaFin).toBeEnabled();
    
    // Marcar checkbox
    await page.check('#trabajandoActualmente');
    
    // Verificar que fechaFin está deshabilitado
    await expect(fechaFin).toBeDisabled();
    
    // Desmarcar checkbox
    await page.uncheck('#trabajandoActualmente');
    
    // Verificar que fechaFin vuelve a estar habilitado
    await expect(fechaFin).toBeEnabled();
  });

  test('Debe persistir las experiencias al continuar al siguiente paso', async ({ page }) => {
    // Agregar una experiencia
    await page.click('#btnAgregarExperiencia');
    await page.waitForSelector('#modalExperiencia', { state: 'visible' });
    await llenarModalExperiencia(page, EXPERIENCIA_1);
    await page.click('#btnAgregarExperienciaModal');
    await page.waitForSelector('#modalExperiencia', { state: 'hidden' });
    
    // Continuar al siguiente paso
    await page.click('button[type="submit"]');
    await page.waitForURL('**/CreateStep6');
    
    // Volver al paso 5
    await page.click('a[href*="CreateStep5"]');
    await page.waitForURL('**/CreateStep5');
    
    // Verificar que la experiencia sigue ahí
    const tabla = page.locator('#tablaExperiencias tbody');
    await expect(tabla.locator('tr')).toHaveCount(1);
    await expect(tabla).toContainText(EXPERIENCIA_1.nombreInstitucion);
  });

  test('Debe mantener el orden de las experiencias', async ({ page }) => {
    // Agregar experiencias en orden específico
    const experiencias = [EXPERIENCIA_1, EXPERIENCIA_2];
    
    for (const exp of experiencias) {
      await page.click('#btnAgregarExperiencia');
      await page.waitForSelector('#modalExperiencia', { state: 'visible' });
      await llenarModalExperiencia(page, exp);
      await page.click('#btnAgregarExperienciaModal');
      await page.waitForSelector('#modalExperiencia', { state: 'hidden' });
    }
    
    // Verificar el orden
    const filas = page.locator('#tablaExperiencias tbody tr');
    const primeraFila = filas.nth(0);
    const segundaFila = filas.nth(1);
    
    await expect(primeraFila).toContainText(EXPERIENCIA_1.nombreInstitucion);
    await expect(segundaFila).toContainText(EXPERIENCIA_2.nombreInstitucion);
  });

  test('Debe mostrar tooltips en el botón de eliminar', async ({ page }) => {
    // Agregar una experiencia
    await page.click('#btnAgregarExperiencia');
    await page.waitForSelector('#modalExperiencia', { state: 'visible' });
    await llenarModalExperiencia(page, EXPERIENCIA_1);
    await page.click('#btnAgregarExperienciaModal');
    await page.waitForSelector('#modalExperiencia', { state: 'hidden' });
    
    // Verificar el tooltip del botón eliminar
    const btnEliminar = page.locator('#tablaExperiencias tbody tr button[title*="Eliminar"]');
    await expect(btnEliminar).toHaveAttribute('title', 'Eliminar experiencia');
  });

  test('Debe aplicar estilos visuales a la tabla', async ({ page }) => {
    // Agregar una experiencia
    await page.click('#btnAgregarExperiencia');
    await page.waitForSelector('#modalExperiencia', { state: 'visible' });
    await llenarModalExperiencia(page, EXPERIENCIA_1);
    await page.click('#btnAgregarExperienciaModal');
    await page.waitForSelector('#modalExperiencia', { state: 'hidden' });
    
    // Verificar clases CSS de la tabla
    const tabla = page.locator('#tablaExperiencias');
    await expect(tabla).toHaveClass(/table-bordered/);
    await expect(tabla).toHaveClass(/table-hover/);
    
    // Verificar el wrapper responsive
    const wrapper = page.locator('#contenedorTablaExperiencias');
    await expect(wrapper).toHaveClass(/table-responsive/);
  });
});

test.describe('Navegación entre pasos del wizard', () => {
  
  test('Debe poder navegar entre pasos haciendo clic en los indicadores', async ({ page }) => {
    // Ir al paso 1
    await page.goto('/Profesores/CreateStep1');
    
    // Click en el indicador del paso 5
    await page.click('a[href*="CreateStep5"]');
    await page.waitForURL('**/CreateStep5');
    
    // Verificar que estamos en el paso 5
    await expect(page.locator('h1')).toContainText('Paso 5 de 6');
    
    // Click en el indicador del paso 2
    await page.click('a[href*="CreateStep2"]');
    await page.waitForURL('**/CreateStep2');
    
    // Verificar que estamos en el paso 2
    await expect(page.locator('h1')).toContainText('Paso 2 de 6');
  });

  test('Los indicadores deben tener efectos hover', async ({ page }) => {
    await page.goto('/Profesores/CreateStep1');
    
    // Obtener el enlace de navegación
    const stepLink = page.locator('a[href*="CreateStep2"]');
    
    // Hacer hover
    await stepLink.hover();
    
    // Verificar que tiene la clase step-link (el CSS se aplicará)
    await expect(stepLink).toHaveClass(/step-link/);
  });
});
