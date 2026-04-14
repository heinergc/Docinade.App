// Script para probar la creación de usuarios desde el panel admin
// INSTRUCCIONES:
// 1. Abre https://localhost:18163/Admin/Users/Create en tu navegador
// 2. Inicia sesión como administrador
// 3. Abre las herramientas de desarrollador (F12)
// 4. Ve a la consola y pega este script completo
// 5. Presiona Enter para ejecutarlo

console.log('🚀 Iniciando prueba de creación de usuario desde panel admin...');

function probarCreacionUsuarioAdmin() {
    // Verificar que estamos en la página correcta
    if (!window.location.pathname.includes('/Admin/Users/Create')) {
        console.error('❌ No estás en la página de creación de usuarios admin');
        console.log('📍 Navega a: https://localhost:18163/Admin/Users/Create');
        return;
    }
    
    console.log('✅ Página correcta detectada');
    
    // Verificar que los campos del formulario existen
    const emailField = document.querySelector('input[name="Email"]');
    const passwordField = document.querySelector('input[name="Password"]');
    const confirmPasswordField = document.querySelector('input[name="ConfirmPassword"]');
    const nombreField = document.querySelector('input[name="Nombre"]');
    const apellidoField = document.querySelector('input[name="Apellido"]');
    const phoneField = document.querySelector('input[name="PhoneNumber"]');
    const identificacionField = document.querySelector('input[name="NumeroIdentificacion"]');
    const institucionField = document.querySelector('input[name="Institucion"]');
    const departamentoField = document.querySelector('input[name="Departamento"]');
    const isActiveField = document.querySelector('input[name="IsActive"]');
    const emailConfirmedField = document.querySelector('input[name="EmailConfirmed"]');
    
    console.log('📝 Verificando campos del formulario:');
    console.log(`   - Email: ${emailField ? '✅' : '❌'}`);
    console.log(`   - Contraseña: ${passwordField ? '✅' : '❌'}`);
    console.log(`   - Confirmar Contraseña: ${confirmPasswordField ? '✅' : '❌'}`);
    console.log(`   - Nombre: ${nombreField ? '✅' : '❌'}`);
    console.log(`   - Apellido: ${apellidoField ? '✅' : '❌'}`);
    console.log(`   - Teléfono: ${phoneField ? '✅' : '❌'}`);
    console.log(`   - Identificación: ${identificacionField ? '✅' : '❌'}`);
    console.log(`   - Institución: ${institucionField ? '✅' : '❌'}`);
    console.log(`   - Departamento: ${departamentoField ? '✅' : '❌'}`);
    console.log(`   - Usuario Activo: ${isActiveField ? '✅' : '❌'}`);
    console.log(`   - Email Confirmado: ${emailConfirmedField ? '✅' : '❌'}`);
    
    // Verificar el formulario
    const form = document.querySelector('form');
    if (!form) {
        console.error('❌ No se encontró el formulario');
        return;
    }
    
    console.log('✅ Formulario encontrado');
    
    // Verificar token anti-falsificación
    const antiForgeryToken = document.querySelector('input[name="__RequestVerificationToken"]');
    console.log(`🔒 Token anti-falsificación: ${antiForgeryToken ? '✅' : '❌'}`);
    
    // Llenar el formulario con datos de prueba
    const testData = {
        email: `test.user.${Date.now()}@ejemplo.com`,
        password: 'TestPassword123!',
        nombre: 'Usuario',
        apellido: 'Prueba',
        phone: '+506 8888-8888',
        identificacion: '1-2345-6789',
        institucion: 'Universidad de Prueba',
        departamento: 'Departamento de Pruebas'
    };
    
    console.log('📝 Llenando formulario con datos de prueba...');
    
    if (emailField) {
        emailField.value = testData.email;
        emailField.dispatchEvent(new Event('input', { bubbles: true }));
        console.log(`   ✅ Email: ${testData.email}`);
    }
    
    if (passwordField) {
        passwordField.value = testData.password;
        passwordField.dispatchEvent(new Event('input', { bubbles: true }));
        console.log('   ✅ Contraseña establecida');
    }
    
    if (confirmPasswordField) {
        confirmPasswordField.value = testData.password;
        confirmPasswordField.dispatchEvent(new Event('input', { bubbles: true }));
        console.log('   ✅ Confirmación de contraseña establecida');
    }
    
    if (nombreField) {
        nombreField.value = testData.nombre;
        nombreField.dispatchEvent(new Event('input', { bubbles: true }));
        console.log(`   ✅ Nombre: ${testData.nombre}`);
    }
    
    if (apellidoField) {
        apellidoField.value = testData.apellido;
        apellidoField.dispatchEvent(new Event('input', { bubbles: true }));
        console.log(`   ✅ Apellido: ${testData.apellido}`);
    }
    
    if (phoneField) {
        phoneField.value = testData.phone;
        phoneField.dispatchEvent(new Event('input', { bubbles: true }));
        console.log(`   ✅ Teléfono: ${testData.phone}`);
    }
    
    if (identificacionField) {
        identificacionField.value = testData.identificacion;
        identificacionField.dispatchEvent(new Event('input', { bubbles: true }));
        console.log(`   ✅ Identificación: ${testData.identificacion}`);
    }
    
    if (institucionField) {
        institucionField.value = testData.institucion;
        institucionField.dispatchEvent(new Event('input', { bubbles: true }));
        console.log(`   ✅ Institución: ${testData.institucion}`);
    }
    
    if (departamentoField) {
        departamentoField.value = testData.departamento;
        departamentoField.dispatchEvent(new Event('input', { bubbles: true }));
        console.log(`   ✅ Departamento: ${testData.departamento}`);
    }
    
    if (isActiveField && !isActiveField.checked) {
        isActiveField.checked = true;
        isActiveField.dispatchEvent(new Event('change', { bubbles: true }));
        console.log('   ✅ Usuario marcado como activo');
    }
    
    if (emailConfirmedField && !emailConfirmedField.checked) {
        emailConfirmedField.checked = true;
        emailConfirmedField.dispatchEvent(new Event('change', { bubbles: true }));
        console.log('   ✅ Email marcado como confirmado');
    }
    
    console.log('🎯 Formulario llenado completamente');
    
    // Preguntar si desea enviar el formulario
    console.log('');
    console.log('⚠️  IMPORTANTE: El formulario ha sido llenado con datos de prueba.');
    console.log('   Para completar la prueba, tienes dos opciones:');
    console.log('');
    console.log('   1. ENVÍO AUTOMÁTICO (recomendado para pruebas):');
    console.log('      Ejecuta: enviarFormularioAutomatico()');
    console.log('');
    console.log('   2. ENVÍO MANUAL:');
    console.log('      Revisa los datos y haz clic en "Crear Usuario"');
    console.log('');
    
    // Crear función global para envío automático
    window.enviarFormularioAutomatico = function() {
        console.log('🚀 Enviando formulario automáticamente...');
        
        // Verificar validación del formulario
        if (form.checkValidity && !form.checkValidity()) {
            console.error('❌ El formulario contiene errores de validación');
            form.reportValidity();
            return;
        }
        
        // Enviar formulario
        form.submit();
        console.log('✅ Formulario enviado - verifica los resultados en la página');
    };
    
    return {
        success: true,
        message: 'Formulario preparado para envío',
        testData: testData
    };
}

// Ejecutar la prueba
try {
    const resultado = probarCreacionUsuarioAdmin();
    
    if (resultado.success) {
        console.log('');
        console.log('🎉 PRUEBA COMPLETADA EXITOSAMENTE');
        console.log('');
        console.log('📊 Datos de prueba utilizados:');
        console.log(`   - Email: ${resultado.testData.email}`);
        console.log(`   - Nombre: ${resultado.testData.nombre} ${resultado.testData.apellido}`);
        console.log(`   - Institución: ${resultado.testData.institucion}`);
        console.log(`   - Departamento: ${resultado.testData.departamento}`);
        console.log('');
        console.log('🔄 Para enviar el formulario automáticamente, ejecuta:');
        console.log('   enviarFormularioAutomatico()');
    }
} catch (error) {
    console.error('❌ Error durante la prueba:', error);
}
