import { test, expect } from '@playwright/test';

const adminCredentials = {
  email: 'admin@rubricas.edu',
  password: 'Admin123!'
};

const estudianteId = 33;

async function loginAsAdmin(page) {
  await page.goto('/Account/Login');

  await page.fill('#Email', adminCredentials.email);
  await page.fill('#Password', adminCredentials.password);

  // Esperar a que el botón esté listo antes de hacer clic
  await page.waitForSelector('button[type="submit"]', { state: 'visible' });
  
  // Hacer clic y esperar la navegación
  await page.click('button[type="submit"]');
  
  // Esperar a que la navegación termine
  await page.waitForURL(/\/(Home|PeriodoAcademico\/Seleccionar|Empadronamiento)(\/|$)/, { timeout: 10000 });
}

async function openEmpadronamientoForm(page) {
  await page.goto(`/Empadronamiento/Create/${estudianteId}`);
  await expect(page.locator('#formEmpadronamiento')).toBeVisible();
}

test.describe('Empadronamiento multistep', () => {
  test.beforeEach(async ({ page }) => {
    await loginAsAdmin(page);
    await openEmpadronamientoForm(page);
  });

  test('completa el flujo y finaliza el empadronamiento', async ({ page }) => {
    const uniqueNumber = Date.now().toString().slice(-8);
    const numeroId = `9${uniqueNumber}`;
    const telefonoAlterno = `22${uniqueNumber.slice(-6)}`;
    const telefonoEmergencia = `87${uniqueNumber.slice(-6)}`;
    const correoAlterno = `empadronamiento${uniqueNumber}@example.com`;

    // Paso 1: Datos personales
    await page.fill('#DatosEmpadronamiento_NumeroId', numeroId);
    await page.fill('#DatosEmpadronamiento_FechaNacimiento', '1990-05-15');
    await page.selectOption('#DatosEmpadronamiento_Genero', { label: 'Masculino' });
    await page.selectOption('#DatosEmpadronamiento_Nacionalidad', { label: 'Costarricense' });
    await page.selectOption('#DatosEmpadronamiento_EstadoCivil', { label: 'Soltero/a' });

    const botonSiguiente = page.locator('#btnSiguiente');
    await expect(botonSiguiente).toBeEnabled();
    await botonSiguiente.click();

    // Paso 2: Contacto
    await expect(page.locator('#seccion2')).toBeVisible();
    await page.selectOption('#DatosEmpadronamiento_Provincia', { label: 'San José' });
    await page.fill('#DatosEmpadronamiento_Canton', `Central ${uniqueNumber.slice(-2)}`);
    await page.fill('#DatosEmpadronamiento_Distrito', 'Carmen');
    await page.fill('#DatosEmpadronamiento_Barrio', 'Centro');
    await page.fill('#DatosEmpadronamiento_Senas', 'Referencia de prueba generada por Playwright.');
    await page.fill('#DatosEmpadronamiento_TelefonoAlterno', telefonoAlterno);
    await page.fill('#DatosEmpadronamiento_CorreoAlterno', correoAlterno);
    await botonSiguiente.click();

    // Paso 3: Responsables
    await expect(page.locator('#seccion3')).toBeVisible();
    await page.fill('#DatosEmpadronamiento_NombrePadre', 'Carlos Automatizado');
    await page.fill('#DatosEmpadronamiento_NombreMadre', 'María Automatizada');
    await page.fill('#DatosEmpadronamiento_ContactoEmergencia', 'Ana Emergencias');
    await page.fill('#DatosEmpadronamiento_TelefonoEmergencia', telefonoEmergencia);
    await page.selectOption('#DatosEmpadronamiento_RelacionEmergencia', { label: 'Hermano/a' });
    await botonSiguiente.click();

    // Paso 4: Salud
    await expect(page.locator('#seccion4')).toBeVisible();
    await page.fill('#DatosEmpadronamiento_Alergias', 'Sin alergias registradas');
    await page.fill('#DatosEmpadronamiento_CondicionesMedicas', 'Ninguna condición relevante');
    await page.fill('#DatosEmpadronamiento_Medicamentos', 'No aplica');
    await page.selectOption('#DatosEmpadronamiento_SeguroMedico', { label: 'CCSS - Caja Costarricense de Seguro Social' });
    await page.fill('#DatosEmpadronamiento_CentroMedicoHabitual', 'Clínica Central');

    // Verificar navegación hacia atrás antes de continuar
    await page.click('#btnAnterior');
    await expect(page.locator('#seccion3')).toBeVisible();
    await expect(page.locator('#DatosEmpadronamiento_ContactoEmergencia')).toHaveValue('Ana Emergencias');
    await botonSiguiente.click();
    await expect(page.locator('#seccion4')).toBeVisible();

    await page.check('#autorizacionMedica');
    await botonSiguiente.click();

    // Paso 5: Académico
    await expect(page.locator('#seccion5')).toBeVisible();
    await expect(botonSiguiente).toBeHidden();
    const botonGuardar = page.locator('#btnGuardar');
    await expect(botonGuardar).toBeVisible();

    await page.fill('#DatosEmpadronamiento_InstitucionProcedencia', 'Colegio Automatizado');
    await page.selectOption('#DatosEmpadronamiento_UltimoNivelCursado', { label: 'Bachillerato en Educación Media' });
    await page.fill('#DatosEmpadronamiento_PromedioAnterior', '87.5');
    await page.fill('#DatosEmpadronamiento_AdaptacionesPrevias', 'Sin adaptaciones registradas.');
    await page.check('#certNotas');
    await page.check('#constConducta');

    const saveResponsePromise = page.waitForResponse((response) => {
      return response.url().includes('/Empadronamiento/Create') && response.request().method() === 'POST';
    });

    await botonGuardar.click();

    const saveResponse = await saveResponsePromise;
    expect(saveResponse.ok()).toBeTruthy();
    const saveResult = await saveResponse.json();
    expect(saveResult.success).toBeTruthy();

    const successToast = page.locator('#toast-container .toast-body', { hasText: 'Empadronamiento guardado exitosamente' });
    await expect(successToast).toBeVisible();

    await page.waitForURL(/\/Empadronamiento(\/|$)/, { timeout: 15000 });
    await expect(page.locator('#tablaEstudiantes')).toBeVisible();

    const rowConNuevoNumero = page.locator('#tablaEstudiantes tbody tr', { hasText: numeroId });
    await expect(rowConNuevoNumero).toBeVisible();
  });

  test('permite guardar borrador y regresar de un paso posterior', async ({ page }) => {
    const uniqueNumber = Date.now().toString().slice(-6);
    const numeroId = `98${uniqueNumber}`;

    // Paso 1: completar datos mínimos
    await page.fill('#DatosEmpadronamiento_NumeroId', numeroId);
    await page.fill('#DatosEmpadronamiento_FechaNacimiento', '1995-10-10');
    await page.selectOption('#DatosEmpadronamiento_Genero', { label: 'Femenino' });
    await page.selectOption('#DatosEmpadronamiento_Nacionalidad', { label: 'Costarricense' });
    await page.selectOption('#DatosEmpadronamiento_EstadoCivil', { label: 'Soltero/a' });

    const botonSiguiente = page.locator('#btnSiguiente');
    await botonSiguiente.click();
    await expect(page.locator('#seccion2')).toBeVisible();

    // Navegar hacia atrás para validar multistep
    await page.click('#btnAnterior');
    await expect(page.locator('#seccion1')).toBeVisible();
    await expect(page.locator('#DatosEmpadronamiento_NumeroId')).toHaveValue(numeroId);

    // Volver a avanzar al paso 2
    await botonSiguiente.click();
    await expect(page.locator('#seccion2')).toBeVisible();

    // Guardar borrador y verificar notificación
    const toastPromise = page.waitForSelector('#toast-container .toast-body', { timeout: 5000 });
    await page.click('#btnGuardarBorrador');
    const toast = await toastPromise;
    await expect(toast).toHaveText(/Borrador guardado/i);

    const storedDraft = await page.evaluate(() => localStorage.getItem('empadronamiento_draft'));
    expect(storedDraft).not.toBeNull();

    const draftData = storedDraft ? JSON.parse(storedDraft) : {};
    expect(draftData['DatosEmpadronamiento.NumeroId']).toBe(numeroId);
  });
});