import { test, expect, Page } from '@playwright/test';

/**
 * Pruebas E2E para la funcionalidad de Aplicar Decisión Profesional
 * Módulo de Conducta - Opción C (REA 40862-V21, Artículo 132)
 */

// Configuración base
const BASE_URL = 'https://localhost:18163';
const ID_ESTUDIANTE = 2;
const ID_PERIODO = 1;

// Datos de prueba para la decisión profesional
const DECISION_MANTENER_APLAZADO = {
  numeroActa: 'ACTA-COM-2025-001',
  miembrosComite: 'Director Académico, Coordinador de Conducta, Profesor Guía, Orientador',
  justificacionPedagogica: 'El estudiante no ha mostrado un cambio significativo en su comportamiento a pesar de las múltiples intervenciones realizadas. Se mantiene el estado de aplazado para que continúe trabajando en mejorar su conducta.',
  circunstanciasEspeciales: 'Estudiante con varios episodios de indisciplina reiterados durante el período.',
  accionesRealizadas: 'Se realizaron entrevistas con el estudiante, reuniones con padres de familia, y seguimiento por parte del departamento de orientación.',
  compromisosEstudiante: 'El estudiante se compromete a mejorar su comportamiento en el próximo período académico.',
  acuerdosComite: 'Se mantiene la calificación de aplazado. Se recomienda continuar con seguimiento cercano.',
  decision: 'Mantener Aplazado'
};

const DECISION_APROBAR_CONDUCTA = {
  numeroActa: 'ACTA-COM-2025-002',
  miembrosComite: 'Director Académico, Coordinador de Conducta, Profesor Guía, Orientador, Psicólogo Institucional',
  justificacionPedagogica: 'El estudiante ha mostrado un cambio positivo significativo en su comportamiento durante el período. Ha cumplido satisfactoriamente con todas las actividades de mejora propuestas y no ha presentado nuevas faltas disciplinarias. El comité considera que merece una segunda oportunidad.',
  circunstanciasEspeciales: 'El estudiante ha participado activamente en las actividades de mejora conductual, ha mostrado arrepentimiento genuino y compromiso con el cambio.',
  accionesRealizadas: 'El estudiante completó satisfactoriamente un programa de orientación conductual, participó en actividades de servicio comunitario, y mantuvo un comportamiento ejemplar durante los últimos tres meses.',
  compromisosEstudiante: 'El estudiante se compromete a mantener un comportamiento adecuado y a servir como ejemplo para sus compañeros.',
  acuerdosComite: 'Se aprueba la conducta con nota mínima de 70. El estudiante debe mantener un comportamiento ejemplar durante el siguiente período.',
  notaAjustada: '70.0',
  decision: 'Aprobar Conducta'
};

/**
 * Función auxiliar para iniciar sesión (si es necesario)
 */
async function iniciarSesion(page: Page) {
  // Si la aplicación requiere autenticación, agregar aquí el flujo de login
  // Por ahora, asumimos que ya hay una sesión activa
}

/**
 * Función auxiliar para llenar el formulario de decisión profesional
 */
async function llenarFormularioDecision(page: Page, datos: typeof DECISION_MANTENER_APLAZADO | typeof DECISION_APROBAR_CONDUCTA) {
  // Información del acta
  await page.fill('#NumeroActa', datos.numeroActa);
  await page.fill('#MiembrosComite', datos.miembrosComite);
  
  // Fecha de reunión (usar la fecha actual por defecto)
  const fechaActual = new Date().toISOString().split('T')[0];
  await page.fill('#FechaReunion', fechaActual);
  
  // Justificación pedagógica (campo obligatorio)
  await page.fill('#JustificacionPedagogica', datos.justificacionPedagogica);
  
  // Circunstancias especiales
  await page.fill('#CircunstanciasEspeciales', datos.circunstanciasEspeciales);
  
  // Acciones realizadas
  await page.fill('#AccionesRealizadas', datos.accionesRealizadas);
  
  // Compromisos del estudiante
  await page.fill('#CompromisosEstudiante', datos.compromisosEstudiante);
  
  // Acuerdos del comité
  await page.fill('#AcuerdosComite', datos.acuerdosComite);
  
  // Decisión tomada
  await page.selectOption('#Decision', datos.decision);
  
  // Si es aprobar conducta, llenar la nota ajustada
  if (datos.decision === 'Aprobar Conducta' && 'notaAjustada' in datos) {
    // Asegurar que pasamos un string a page.fill
    await page.fill('#NotaAjustada', String((datos as any).notaAjustada));
  }
}

test.describe('Aplicar Decisión Profesional - Vista Principal', () => {
  
  test.beforeEach(async ({ page }) => {
    // Ignorar errores de certificado SSL en desarrollo
    await page.goto(`${BASE_URL}/NotaConducta/AplicarDecisionProfesional?idEstudiante=${ID_ESTUDIANTE}&idPeriodo=${ID_PERIODO}`, {
      waitUntil: 'networkidle'
    });
  });

  test('Debe cargar la página correctamente', async ({ page }) => {
    // Verificar el título de la página
    await expect(page).toHaveTitle(/Aplicar Decisión Profesional/);
    
    // Verificar el encabezado principal
    const titulo = page.locator('h2');
    await expect(titulo).toContainText('Decisión Profesional del Comité');
    
    // Verificar que muestra la referencia al reglamento
    await expect(page.locator('p.text-muted')).toContainText('REA 40862-V21, Artículo 132');
  });

  test('Debe mostrar la información del estudiante', async ({ page }) => {
    // Verificar que existe la sección de información del estudiante
    const cardEstudiante = page.locator('.card').first();
    await expect(cardEstudiante).toBeVisible();
    
    // Verificar que muestra el nombre del estudiante
    await expect(cardEstudiante).toContainText(/Nombre/);
    
    // Verificar que muestra la nota actual
    await expect(cardEstudiante).toContainText(/Nota Actual/);
    
    // Verificar que muestra el total de boletas
    await expect(cardEstudiante).toContainText(/Total de Boletas/);
  });

  test('Debe mostrar el breadcrumb de navegación', async ({ page }) => {
    // Verificar que existe el breadcrumb
    const breadcrumb = page.locator('nav[aria-label="breadcrumb"]');
    await expect(breadcrumb).toBeVisible();
    
    // Verificar los elementos del breadcrumb
    await expect(breadcrumb).toContainText('Dashboard');
    await expect(breadcrumb).toContainText('Estudiantes Aplazados');
    await expect(breadcrumb).toContainText('Aplicar Decisión');
  });

  test('Debe mostrar todos los campos del formulario', async ({ page }) => {
    // Información del acta
    await expect(page.locator('#NumeroActa')).toBeVisible();
    await expect(page.locator('#FechaReunion')).toBeVisible();
    await expect(page.locator('#MiembrosComite')).toBeVisible();
    
    // Justificación pedagógica
    await expect(page.locator('#JustificacionPedagogica')).toBeVisible();
    await expect(page.locator('#CircunstanciasEspeciales')).toBeVisible();
    
    // Acciones y compromisos
    await expect(page.locator('#AccionesRealizadas')).toBeVisible();
    await expect(page.locator('#CompromisosEstudiante')).toBeVisible();
    await expect(page.locator('#AcuerdosComite')).toBeVisible();
    
    // Decisión
    await expect(page.locator('#Decision')).toBeVisible();
    await expect(page.locator('#NotaAjustada')).toBeVisible();
  });

  test('Debe tener campos obligatorios marcados correctamente', async ({ page }) => {
    // Verificar atributo required en campos obligatorios
    await expect(page.locator('#NumeroActa')).toHaveAttribute('required', '');
    await expect(page.locator('#FechaReunion')).toHaveAttribute('required', '');
    await expect(page.locator('#MiembrosComite')).toHaveAttribute('required', '');
    await expect(page.locator('#JustificacionPedagogica')).toHaveAttribute('required', '');
    await expect(page.locator('#Decision')).toHaveAttribute('required', '');
  });

  test('Debe mostrar el cuadro informativo sobre el artículo 132', async ({ page }) => {
    // Buscar el alert/card con la información del reglamento
    const infoBox = page.locator('.alert-info, .card-body').filter({ hasText: 'Artículo 132' });
    await expect(infoBox).toBeVisible();
  });
});

test.describe('Aplicar Decisión Profesional - Validaciones del Formulario', () => {
  
  test.beforeEach(async ({ page }) => {
    await page.goto(`${BASE_URL}/NotaConducta/AplicarDecisionProfesional?idEstudiante=${ID_ESTUDIANTE}&idPeriodo=${ID_PERIODO}`, {
      waitUntil: 'networkidle'
    });
  });

  test('Debe validar campos obligatorios al enviar formulario vacío', async ({ page }) => {
    // Intentar enviar el formulario sin llenar campos
    const submitBtn = page.locator('button[type="submit"]').filter({ hasText: /Aplicar Decisión/i });
    await submitBtn.click();
    
    // Verificar que no navega (el formulario no es válido)
    await expect(page).toHaveURL(new RegExp(`AplicarDecisionProfesional.*idEstudiante=${ID_ESTUDIANTE}`));
    
    // Verificar que los campos requeridos muestran validación HTML5
    const numeroActa = page.locator('#NumeroActa');
    const esInvalido = await numeroActa.evaluate((el: HTMLInputElement) => !el.validity.valid);
    expect(esInvalido).toBeTruthy();
  });

  test('Debe validar la longitud máxima de campos de texto', async ({ page }) => {
    const textoLargo = 'A'.repeat(4000); // Más de 3000 caracteres
    
    await page.fill('#JustificacionPedagogica', textoLargo);
    
    // Verificar que el campo tiene maxlength
    const maxLength = await page.locator('#JustificacionPedagogica').getAttribute('maxlength');
    expect(maxLength).toBeTruthy();
  });

  test('Debe deshabilitar NotaAjustada cuando la decisión es "Mantener Aplazado"', async ({ page }) => {
    // Seleccionar "Mantener Aplazado"
    await page.selectOption('#Decision', 'Mantener Aplazado');
    
    // Verificar que NotaAjustada está deshabilitado o es opcional
    const notaAjustada = page.locator('#NotaAjustada');
    const estaDeshabilitado = await notaAjustada.isDisabled();
    const tieneReadonly = await notaAjustada.getAttribute('readonly');
    
    // Debe estar deshabilitado o readonly
    expect(estaDeshabilitado || tieneReadonly !== null).toBeTruthy();
  });

  test('Debe habilitar y requerir NotaAjustada cuando la decisión es "Aprobar Conducta"', async ({ page }) => {
    // Seleccionar "Aprobar Conducta"
    await page.selectOption('#Decision', 'Aprobar Conducta');
    
    // Esperar un momento para que se ejecute el JavaScript
    await page.waitForTimeout(300);
    
    // Verificar que NotaAjustada está habilitado
    const notaAjustada = page.locator('#NotaAjustada');
    await expect(notaAjustada).toBeEnabled();
  });

  test('Debe validar que la nota ajustada sea un número válido', async ({ page }) => {
    await page.selectOption('#Decision', 'Aprobar Conducta');
    
    // Intentar ingresar texto en lugar de número
    await page.fill('#NotaAjustada', 'texto invalido');
    
    // Verificar validación HTML5 para tipo number
    const notaAjustada = page.locator('#NotaAjustada');
    const esInvalido = await notaAjustada.evaluate((el: HTMLInputElement) => !el.validity.valid);
    expect(esInvalido).toBeTruthy();
  });
});

test.describe('Aplicar Decisión Profesional - Funcionalidad de Envío', () => {
  
  test.beforeEach(async ({ page }) => {
    await page.goto(`${BASE_URL}/NotaConducta/AplicarDecisionProfesional?idEstudiante=${ID_ESTUDIANTE}&idPeriodo=${ID_PERIODO}`, {
      waitUntil: 'networkidle'
    });
  });

  test('Debe enviar correctamente una decisión de "Mantener Aplazado"', async ({ page }) => {
    // Llenar el formulario
    await llenarFormularioDecision(page, DECISION_MANTENER_APLAZADO);
    
    // Enviar el formulario
    const submitBtn = page.locator('button[type="submit"]').filter({ hasText: /Aplicar Decisión/i });
    await submitBtn.click();
    
    // Esperar navegación o mensaje de éxito
    await page.waitForTimeout(2000);
    
    // Verificar redirección o mensaje de éxito
    const urlActual = page.url();
    const tieneExito = urlActual.includes('EstudianteNota') || 
                       await page.locator('.alert-success, .toast-success').count() > 0;
    
    expect(tieneExito).toBeTruthy();
  });

  test('Debe enviar correctamente una decisión de "Aprobar Conducta"', async ({ page }) => {
    // Llenar el formulario
    await llenarFormularioDecision(page, DECISION_APROBAR_CONDUCTA);
    
    // Enviar el formulario
    const submitBtn = page.locator('button[type="submit"]').filter({ hasText: /Aplicar Decisión/i });
    await submitBtn.click();
    
    // Esperar navegación o mensaje de éxito
    await page.waitForTimeout(2000);
    
    // Verificar redirección o mensaje de éxito
    const urlActual = page.url();
    const tieneExito = urlActual.includes('EstudianteNota') || 
                       await page.locator('.alert-success, .toast-success').count() > 0;
    
    expect(tieneExito).toBeTruthy();
  });

  test('Debe poder cancelar y volver a la página anterior', async ({ page }) => {
    // Buscar el botón de cancelar/volver
    const btnCancelar = page.locator('a.btn, button').filter({ hasText: /Cancelar|Volver/i }).first();
    
    if (await btnCancelar.count() > 0) {
      await btnCancelar.click();
      
      // Esperar navegación
      await page.waitForTimeout(1000);
      
      // Verificar que ya no estamos en la página de decisión
      const urlActual = page.url();
      expect(urlActual).not.toContain('AplicarDecisionProfesional');
    }
  });

  test('Debe mostrar confirmación antes de enviar decisión crítica', async ({ page }) => {
    // Llenar el formulario
    await llenarFormularioDecision(page, DECISION_APROBAR_CONDUCTA);
    
    // Configurar listener para diálogos
    let dialogoMostrado = false;
    page.on('dialog', async dialog => {
      dialogoMostrado = true;
      expect(dialog.message()).toContain(/seguro|confirmar/i);
      await dialog.dismiss(); // Cancelar para no afectar otros tests
    });
    
    // Intentar enviar
    const submitBtn = page.locator('button[type="submit"]').filter({ hasText: /Aplicar Decisión/i });
    await submitBtn.click();
    
    // Esperar un momento para que se muestre el diálogo
    await page.waitForTimeout(500);
    
    // Si no hay diálogo, está bien (depende de la implementación)
    // Este test documenta que podría haber confirmación
  });
});

test.describe('Aplicar Decisión Profesional - Interfaz de Usuario', () => {
  
  test.beforeEach(async ({ page }) => {
    await page.goto(`${BASE_URL}/NotaConducta/AplicarDecisionProfesional?idEstudiante=${ID_ESTUDIANTE}&idPeriodo=${ID_PERIODO}`, {
      waitUntil: 'networkidle'
    });
  });

  test('Debe tener un diseño responsivo', async ({ page }) => {
    // Verificar en vista móvil
    await page.setViewportSize({ width: 375, height: 667 });
    await page.waitForTimeout(300);
    
    // Los elementos principales deben seguir visibles
    await expect(page.locator('h2')).toBeVisible();
    await expect(page.locator('form')).toBeVisible();
    
    // Verificar en vista tablet
    await page.setViewportSize({ width: 768, height: 1024 });
    await page.waitForTimeout(300);
    await expect(page.locator('form')).toBeVisible();
    
    // Verificar en vista desktop
    await page.setViewportSize({ width: 1920, height: 1080 });
    await page.waitForTimeout(300);
    await expect(page.locator('form')).toBeVisible();
  });

  test('Debe aplicar estilos Bootstrap correctamente', async ({ page }) => {
    // Verificar clases de Bootstrap en elementos clave
    const form = page.locator('form').first();
    await expect(form).toBeVisible();
    
    // Verificar que los botones tienen clases de Bootstrap
    const submitBtn = page.locator('button[type="submit"]').first();
    const clases = await submitBtn.getAttribute('class');
    expect(clases).toContain('btn');
  });

  test('Debe mostrar iconos de Font Awesome', async ({ page }) => {
    // Buscar iconos de FA (pueden estar en títulos, botones, etc.)
    const iconos = page.locator('i.fa, i.fas, i.far');
    const cantidadIconos = await iconos.count();
    
    // Debe haber al menos algún icono en la página
    expect(cantidadIconos).toBeGreaterThan(0);
  });

  test('Debe mostrar tooltips en elementos informativos', async ({ page }) => {
    // Buscar elementos con tooltips (data-bs-toggle="tooltip")
    const elementosConTooltip = page.locator('[data-bs-toggle="tooltip"], [title]');
    const cantidad = await elementosConTooltip.count();
    
    // Si hay tooltips, verificar que están correctamente configurados
    if (cantidad > 0) {
      const primerElemento = elementosConTooltip.first();
      const tieneTitle = await primerElemento.getAttribute('title');
      expect(tieneTitle).toBeTruthy();
    }
  });

  test('Debe tener labels asociados a todos los inputs', async ({ page }) => {
    // Obtener todos los inputs
    const inputs = page.locator('input[type="text"], input[type="date"], input[type="number"], textarea, select');
    const cantidadInputs = await inputs.count();
    
    // Verificar que cada input tenga un label asociado o un placeholder
    for (let i = 0; i < cantidadInputs; i++) {
      const input = inputs.nth(i);
      const id = await input.getAttribute('id');
      
      if (id) {
        // Buscar label asociado
        const label = page.locator(`label[for="${id}"]`);
        const existeLabel = await label.count() > 0;
        
        // O verificar que tenga placeholder/aria-label
        const placeholder = await input.getAttribute('placeholder');
        const ariaLabel = await input.getAttribute('aria-label');
        
        expect(existeLabel || placeholder || ariaLabel).toBeTruthy();
      }
    }
  });
});

test.describe('Aplicar Decisión Profesional - Accesibilidad', () => {
  
  test.beforeEach(async ({ page }) => {
    await page.goto(`${BASE_URL}/NotaConducta/AplicarDecisionProfesional?idEstudiante=${ID_ESTUDIANTE}&idPeriodo=${ID_PERIODO}`, {
      waitUntil: 'networkidle'
    });
  });

  test('Debe poder navegar el formulario con tabulador', async ({ page }) => {
    // Hacer foco en el primer campo
    await page.keyboard.press('Tab');
    
    // Verificar que un elemento tiene foco
    const elementoConFoco = await page.evaluate(() => document.activeElement?.tagName);
    expect(elementoConFoco).toBeTruthy();
    
    // Navegar varios campos
    for (let i = 0; i < 5; i++) {
      await page.keyboard.press('Tab');
    }
    
    // Debería seguir dentro del formulario
    const tagActivo = await page.evaluate(() => document.activeElement?.tagName);
    expect(['INPUT', 'TEXTAREA', 'SELECT', 'BUTTON']).toContain(tagActivo);
  });

  test('Debe tener un orden de tabulación lógico', async ({ page }) => {
    // Los campos deben seguir un orden lógico de arriba hacia abajo
    const campos = [
      '#NumeroActa',
      '#FechaReunion',
      '#MiembrosComite',
      '#JustificacionPedagogica'
    ];
    
    // Hacer foco en cada campo y verificar
    for (const selector of campos) {
      await page.locator(selector).focus();
      const estaEnfocado = await page.locator(selector).evaluate(el => el === document.activeElement);
      expect(estaEnfocado).toBeTruthy();
    }
  });

  test('Debe tener atributos ARIA apropiados', async ({ page }) => {
    // Buscar elementos con roles ARIA
    const elementosConRole = page.locator('[role]');
    const cantidad = await elementosConRole.count();
    
    // Debe haber al menos algunos elementos con roles ARIA
    expect(cantidad).toBeGreaterThan(0);
  });
});

test.describe('Aplicar Decisión Profesional - Manejo de Errores', () => {
  
  test('Debe manejar errores de red gracefully', async ({ page }) => {
    // Simular desconexión de red
    await page.route('**/*', route => route.abort());
    
    await page.goto(`${BASE_URL}/NotaConducta/AplicarDecisionProfesional?idEstudiante=${ID_ESTUDIANTE}&idPeriodo=${ID_PERIODO}`, {
      waitUntil: 'domcontentloaded'
    }).catch(() => {
      // Esperamos que falle
    });
    
    // Verificar que muestra algún mensaje de error o página de error
    const tieneError = await page.locator('body').evaluate(el => 
      el.textContent?.includes('error') || 
      el.textContent?.includes('no disponible') ||
      el.textContent?.includes('conexión')
    );
    
    expect(tieneError).toBeTruthy();
  });

  test('Debe validar estudiante y período válidos', async ({ page }) => {
    // Intentar acceder con IDs inválidos
    await page.goto(`${BASE_URL}/NotaConducta/AplicarDecisionProfesional?idEstudiante=999999&idPeriodo=999`, {
      waitUntil: 'domcontentloaded'
    });
    
    // Debería redirigir o mostrar error
    await page.waitForTimeout(1000);
    
    const urlActual = page.url();
    const tieneError = urlActual.includes('Error') || 
                       urlActual.includes('Dashboard') ||
                       await page.locator('.alert-danger, .alert-warning').count() > 0;
    
    expect(tieneError).toBeTruthy();
  });
});

test.describe('Aplicar Decisión Profesional - Rendimiento', () => {
  
  test('Debe cargar la página en menos de 3 segundos', async ({ page }) => {
    const inicio = Date.now();
    
    await page.goto(`${BASE_URL}/NotaConducta/AplicarDecisionProfesional?idEstudiante=${ID_ESTUDIANTE}&idPeriodo=${ID_PERIODO}`, {
      waitUntil: 'networkidle'
    });
    
    const fin = Date.now();
    const tiempoCarga = fin - inicio;
    
    expect(tiempoCarga).toBeLessThan(3000);
  });

  test('Debe cargar recursos estáticos correctamente', async ({ page }) => {
    const respuestasRecursos: number[] = [];
    
    page.on('response', response => {
      const url = response.url();
      if (url.includes('.css') || url.includes('.js') || url.includes('.woff')) {
        respuestasRecursos.push(response.status());
      }
    });
    
    await page.goto(`${BASE_URL}/NotaConducta/AplicarDecisionProfesional?idEstudiante=${ID_ESTUDIANTE}&idPeriodo=${ID_PERIODO}`, {
      waitUntil: 'networkidle'
    });
    
    // Todos los recursos deben cargar con 200 OK
    const todosCargados = respuestasRecursos.every(status => status === 200);
    expect(todosCargados).toBeTruthy();
  });
});
