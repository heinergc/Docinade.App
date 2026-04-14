import { test, expect } from '@playwright/test';

/**
 * Prueba simplificada del formulario de empadronamiento
 * Verifica específicamente el botón "Finalizar" en la última pestaña
 */
test.describe('Prueba del Botón Finalizar en Empadronamiento', () => {
  
  test('verificar botón finalizar en última pestaña', async ({ page }) => {
    // Configurar timeout
    test.setTimeout(60000);
    
    console.log('🚀 Iniciando prueba del botón Finalizar...');
    
    try {
      // 1. Navegar directamente al formulario (asumiendo login previo)
      console.log('📝 Navegando al formulario de empadronamiento...');
      await page.goto('http://localhost:18164/Empadronamiento/Create/33');
      
      // Esperar a que el formulario se cargue
      await page.waitForSelector('#formEmpadronamiento', { timeout: 10000 });
      console.log('✅ Formulario cargado');
      
      // 2. Verificar que estamos en el paso 1
      await expect(page.locator('.step-item[data-step="1"]')).toHaveClass(/active/);
      console.log('✅ Paso 1 activo');
      
      // 3. Llenar campos mínimos del paso 1
      await page.fill('#NumeroIdentificacion', '118570823');
      await page.fill('#Nombre', 'Juan Test');
      await page.fill('#PrimerApellido', 'González');
      await page.selectOption('#Nacionalidad', { index: 1 }); // Seleccionar primera opción
      await page.fill('#FechaNacimiento', '1990-05-15');
      await page.selectOption('#Sexo', 'M');
      console.log('✅ Campos del paso 1 completados');
      
      // 4. Avanzar al paso 2
      await page.click('#btnSiguiente');
      await expect(page.locator('#step-2')).toBeVisible();
      console.log('✅ Paso 2 activo');
      
      // 5. Llenar campos mínimos del paso 2
      await page.fill('#CorreoElectronico', 'test@ejemplo.com');
      await page.fill('#ConfirmarCorreo', 'test@ejemplo.com');
      await page.fill('#TelefonoResidencia', '22501234');
      await page.fill('#TelefonoCelular', '87654321');
      
      // 6. Avanzar al paso 3
      await page.click('#btnSiguiente');
      await expect(page.locator('#step-3')).toBeVisible();
      console.log('✅ Paso 3 activo');
      
      // 7. Llenar campos mínimos del paso 3
      await page.fill('#NombrePadre', 'Carlos González');
      await page.fill('#CedulaPadre', '105670823');
      await page.fill('#NombreMadre', 'María Rodríguez');
      await page.fill('#CedulaMadre', '206780934');
      await page.fill('#NombreEmergencia', 'Ana González');
      await page.fill('#TelefonoEmergencia', '89012347');
      await page.fill('#RelacionEmergencia', 'Hermana');
      
      // 8. Avanzar al paso 4
      await page.click('#btnSiguiente');
      await expect(page.locator('#step-4')).toBeVisible();
      console.log('✅ Paso 4 activo');
      
      // 9. Llenar campos mínimos del paso 4
      await page.selectOption('#TipoSangre', 'O+');
      await page.fill('#Alergias', 'Ninguna');
      await page.fill('#Medicamentos', 'Ninguno');
      await page.selectOption('#TipoSeguro', 'CCSS');
      
      // 10. Avanzar al paso 5 (Académico)
      await page.click('#btnSiguiente');
      await expect(page.locator('#step-5')).toBeVisible();
      console.log('✅ Paso 5 (Académico) activo');
      
      // 11. VERIFICAR EL BOTÓN FINALIZAR
      console.log('🎯 Verificando botón "Finalizar"...');
      
      // Verificar que el botón "Siguiente" esté oculto
      const btnSiguiente = page.locator('#btnSiguiente');
      await expect(btnSiguiente).toBeHidden();
      console.log('✅ Botón "Siguiente" oculto correctamente');
      
      // Verificar que el botón "Finalizar" esté visible
      const btnFinalizar = page.locator('#btnGuardar');
      await expect(btnFinalizar).toBeVisible();
      console.log('✅ Botón "Finalizar Empadronamiento" visible correctamente');
      
      // Verificar el texto del botón
      await expect(btnFinalizar).toContainText('Finalizar Empadronamiento');
      console.log('✅ Texto del botón correcto');
      
      // 12. Llenar campos académicos mínimos
      await page.fill('#InstitucionAnterior', 'Colegio Test');
      await page.selectOption('#NivelEducativo', { index: 1 });
      await page.fill('#PromedioGeneral', '85.5');
      
      // 13. Verificar que el botón esté habilitado para hacer clic
      await expect(btnFinalizar).toBeEnabled();
      console.log('✅ Botón "Finalizar" habilitado');
      
      // 14. Tomar screenshot del estado final
      await page.screenshot({ 
        path: 'test-results/boton-finalizar-verificado.png', 
        fullPage: true 
      });
      
      console.log('🎉 Prueba del botón "Finalizar" completada exitosamente');
      
      // OPCIONAL: Hacer clic en finalizar si se quiere probar el guardado
      // await page.click('#btnGuardar');
      // await page.waitForURL(/\/Empadronamiento/);
      // console.log('✅ Formulario enviado correctamente');
      
    } catch (error) {
      console.error('❌ Error en la prueba:', error.message);
      await page.screenshot({ path: 'test-results/error-screenshot.png', fullPage: true });
      throw error;
    }
  });
  
  test('probar funcionalidad de navegación entre pasos', async ({ page }) => {
    console.log('🧭 Probando navegación entre pasos...');
    
    await page.goto('http://localhost:18164/Empadronamiento/Create/33');
    await page.waitForSelector('#formEmpadronamiento', { timeout: 10000 });
    
    // Completar paso 1 mínimamente
    await page.fill('#NumeroIdentificacion', '118570823');
    await page.fill('#Nombre', 'Test Navigation');
    await page.fill('#PrimerApellido', 'User');
    await page.selectOption('#Nacionalidad', { index: 1 });
    await page.fill('#FechaNacimiento', '1990-01-01');
    await page.selectOption('#Sexo', 'M');
    
    // Avanzar al paso 2
    await page.click('#btnSiguiente');
    await expect(page.locator('#step-2')).toBeVisible();
    
    // Probar botón "Anterior"
    await page.click('#btnAnterior');
    await expect(page.locator('#step-1')).toBeVisible();
    
    console.log('✅ Navegación funcionando correctamente');
  });
});