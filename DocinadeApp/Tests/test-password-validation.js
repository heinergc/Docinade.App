// Script para probar la validación de contraseñas en español
// INSTRUCCIONES:
// 1. Abre https://localhost:18163/Admin/Users/Create en tu navegador
// 2. Inicia sesión como administrador
// 3. Abre las herramientas de desarrollador (F12)
// 4. Ve a la consola y pega este script completo
// 5. Presiona Enter para ejecutarlo

console.log('🔐 Iniciando prueba de validación de contraseñas en español...');

function probarValidacionContrasenas() {
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
    const form = document.querySelector('form');
    
    if (!emailField || !passwordField || !confirmPasswordField || !nombreField || !apellidoField || !form) {
        console.error('❌ No se encontraron todos los campos necesarios');
        return;
    }
    
    console.log('📝 Formulario encontrado - iniciando pruebas de validación...');
    
    // Casos de prueba para contraseñas
    const passwordTests = [
        {
            name: 'Contraseña muy corta (menos de 6 caracteres)',
            password: '123',
            expectedError: 'debe tener al menos 6 caracteres'
        },
        {
            name: 'Contraseña sin mayúsculas',
            password: 'password123',
            expectedError: 'debe tener al menos una letra mayúscula'
        },
        {
            name: 'Contraseña sin minúsculas',
            password: 'PASSWORD123',
            expectedError: 'debe tener al menos una letra minúscula'
        },
        {
            name: 'Contraseña sin números',
            password: 'Password',
            expectedError: 'debe tener al menos un número'
        },
        {
            name: 'Contraseña válida',
            password: 'Password123',
            expectedError: null
        }
    ];
    
    let testIndex = 0;
    
    function runNextTest() {
        if (testIndex >= passwordTests.length) {
            console.log('');
            console.log('🎉 TODAS LAS PRUEBAS COMPLETADAS');
            console.log('');
            console.log('📊 Resumen:');
            passwordTests.forEach((test, i) => {
                const icon = test.expectedError ? '🔍' : '✅';
                console.log(`   ${icon} ${test.name}: ${test.expectedError ? 'Validación requerida' : 'Contraseña válida'}`);
            });
            console.log('');
            console.log('ℹ️  Para ver los mensajes de error en acción:');
            console.log('   1. Llena los campos requeridos (Email, Nombre, Apellido)');
            console.log('   2. Usa una contraseña que no cumpla los requisitos');
            console.log('   3. Intenta enviar el formulario');
            console.log('   4. Observa los mensajes de error en español');
            return;
        }
        
        const test = passwordTests[testIndex];
        console.log(`\n🧪 Prueba ${testIndex + 1}: ${test.name}`);
        console.log(`   Contraseña: "${test.password}"`);
        
        // Llenar campos básicos
        emailField.value = 'test@ejemplo.com';
        nombreField.value = 'Nombre';
        apellidoField.value = 'Apellido';
        passwordField.value = test.password;
        confirmPasswordField.value = test.password;
        
        // Disparar eventos
        [emailField, nombreField, apellidoField, passwordField, confirmPasswordField].forEach(field => {
            field.dispatchEvent(new Event('input', { bubbles: true }));
            field.dispatchEvent(new Event('change', { bubbles: true }));
        });
        
        setTimeout(() => {
            // Verificar validación HTML5
            const isValid = passwordField.checkValidity() && form.checkValidity();
            console.log(`   HTML5 Validación: ${isValid ? '✅ Válido' : '❌ Inválido'}`);
            
            if (test.expectedError) {
                console.log(`   ⚠️  Se espera error: "${test.expectedError}"`);
                console.log(`   💡 Para ver el error, intenta enviar el formulario ahora`);
            } else {
                console.log(`   ✅ Contraseña válida - no debería mostrar errores`);
            }
            
            testIndex++;
            setTimeout(runNextTest, 1000);
        }, 500);
    }
    
    // Iniciar las pruebas
    runNextTest();
}

// Función para probar envío con contraseña inválida
function probarEnvioConPasswordInvalida() {
    console.log('🚨 Probando envío con contraseña inválida...');
    
    const emailField = document.querySelector('input[name="Email"]');
    const passwordField = document.querySelector('input[name="Password"]');
    const confirmPasswordField = document.querySelector('input[name="ConfirmPassword"]');
    const nombreField = document.querySelector('input[name="Nombre"]');
    const apellidoField = document.querySelector('input[name="Apellido"]');
    const form = document.querySelector('form');
    
    if (!emailField || !passwordField || !confirmPasswordField || !nombreField || !apellidoField || !form) {
        console.error('❌ No se encontraron los campos del formulario');
        return;
    }
    
    // Llenar con datos válidos excepto la contraseña
    emailField.value = `test.${Date.now()}@ejemplo.com`;
    nombreField.value = 'Usuario';
    apellidoField.value = 'Prueba';
    passwordField.value = 'invalida'; // Sin mayúscula ni número
    confirmPasswordField.value = 'invalida';
    
    // Disparar eventos
    [emailField, nombreField, apellidoField, passwordField, confirmPasswordField].forEach(field => {
        field.dispatchEvent(new Event('input', { bubbles: true }));
        field.dispatchEvent(new Event('change', { bubbles: true }));
    });
    
    console.log('📝 Formulario llenado con contraseña inválida: "invalida"');
    console.log('⚠️  Esta contraseña NO cumple los requisitos:');
    console.log('   - Falta una letra mayúscula');
    console.log('   - Falta un número');
    console.log('');
    console.log('🚀 Enviando formulario para ver los mensajes de error en español...');
    
    // Intentar enviar el formulario
    setTimeout(() => {
        form.submit();
    }, 1000);
}

// Función para llenar formulario con contraseña válida
function llenarFormularioValido() {
    console.log('✅ Llenando formulario con datos válidos...');
    
    const emailField = document.querySelector('input[name="Email"]');
    const passwordField = document.querySelector('input[name="Password"]');
    const confirmPasswordField = document.querySelector('input[name="ConfirmPassword"]');
    const nombreField = document.querySelector('input[name="Nombre"]');
    const apellidoField = document.querySelector('input[name="Apellido"]');
    const isActiveField = document.querySelector('input[name="IsActive"]');
    const emailConfirmedField = document.querySelector('input[name="EmailConfirmed"]');
    
    const validPassword = 'Password123!';
    const testEmail = `test.${Date.now()}@ejemplo.com`;
    
    // Llenar todos los campos
    if (emailField) emailField.value = testEmail;
    if (nombreField) nombreField.value = 'Usuario';
    if (apellidoField) apellidoField.value = 'Prueba';
    if (passwordField) passwordField.value = validPassword;
    if (confirmPasswordField) confirmPasswordField.value = validPassword;
    
    // Activar checkboxes
    if (isActiveField && !isActiveField.checked) isActiveField.checked = true;
    if (emailConfirmedField && !emailConfirmedField.checked) emailConfirmedField.checked = true;
    
    // Disparar eventos
    [emailField, nombreField, apellidoField, passwordField, confirmPasswordField].forEach(field => {
        if (field) {
            field.dispatchEvent(new Event('input', { bubbles: true }));
            field.dispatchEvent(new Event('change', { bubbles: true }));
        }
    });
    
    console.log(`📧 Email: ${testEmail}`);
    console.log('👤 Nombre: Usuario Prueba');
    console.log(`🔐 Contraseña: ${validPassword} (cumple todos los requisitos)`);
    console.log('✅ Usuario Activo: Sí');
    console.log('✅ Email Confirmado: Sí');
    console.log('');
    console.log('🎯 Formulario listo para envío exitoso');
}

// Ejecutar la prueba principal
try {
    probarValidacionContrasenas();
    
    // Crear funciones globales para usar después
    window.probarEnvioConPasswordInvalida = probarEnvioConPasswordInvalida;
    window.llenarFormularioValido = llenarFormularioValido;
    
    console.log('');
    console.log('🔧 Funciones adicionales disponibles:');
    console.log('   probarEnvioConPasswordInvalida() - Envía formulario con contraseña inválida para ver errores');
    console.log('   llenarFormularioValido() - Llena formulario con datos válidos para envío exitoso');
    
} catch (error) {
    console.error('❌ Error durante la prueba:', error);
}
