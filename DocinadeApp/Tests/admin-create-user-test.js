// Script para probar la creación de usuarios desde el panel de administración
// Ejecutar en la consola del navegador después de hacer login como admin y navegar a /Admin/Users/Create

async function probarCreacionUsuarioAdmin() {
    console.log('🚀 Iniciando prueba de creación de usuario desde panel admin...');
    
    // Verificar que estamos en la página correcta
    if (!window.location.pathname.includes('/Admin/Users/Create')) {
        console.log('❌ No estamos en la página de creación de usuarios');
        console.log('🔗 Navegando a /Admin/Users/Create...');
        window.location.href = '/Admin/Users/Create';
        return;
    }
    
    console.log('✅ Estamos en la página de creación de usuarios');
    
    // Verificar elementos del formulario
    console.log('🔍 Verificando elementos del formulario...');
    
    const camposRequeridos = [
        'Email', 'Password', 'ConfirmPassword', 'Nombre', 'Apellido'
    ];
    
    const camposOpcionales = [
        'NumeroIdentificacion', 'Institucion', 'Departamento', 'PhoneNumber'
    ];
    
    const checkboxes = [
        'IsActive', 'EmailConfirmed', 'SendWelcomeEmail', 'TwoFactorEnabled'
    ];
    
    // Verificar campos requeridos
    for (const campo of camposRequeridos) {
        const element = document.querySelector(`input[name="${campo}"], input[id="${campo}"]`);
        if (element) {
            console.log(`✅ Campo ${campo} encontrado`);
        } else {
            console.log(`❌ Campo ${campo} NO encontrado`);
        }
    }
    
    // Verificar campos opcionales
    for (const campo of camposOpcionales) {
        const element = document.querySelector(`input[name="${campo}"], input[id="${campo}"]`);
        if (element) {
            console.log(`✅ Campo opcional ${campo} encontrado`);
        } else {
            console.log(`⚠️ Campo opcional ${campo} no encontrado`);
        }
    }
    
    // Verificar checkboxes
    for (const checkbox of checkboxes) {
        const element = document.querySelector(`input[name="${checkbox}"], input[id="${checkbox}"]`);
        if (element) {
            console.log(`✅ Checkbox ${checkbox} encontrado`);
        } else {
            console.log(`⚠️ Checkbox ${checkbox} no encontrado`);
        }
    }
    
    // Generar datos de usuario de prueba
    const timestamp = Date.now();
    const testUser = {
        Email: `admintest${timestamp}@test.com`,
        Password: 'AdminTest123!',
        ConfirmPassword: 'AdminTest123!',
        Nombre: 'Usuario',
        Apellido: 'Admin Test',
        NumeroIdentificacion: `ADMIN${timestamp}`,
        Institucion: 'Universidad Test Admin',
        Departamento: 'Departamento Test Admin',
        PhoneNumber: '8888-8888'
    };
    
    console.log('👤 Llenando formulario con datos de prueba...');
    console.log(`📧 Email: ${testUser.Email}`);
    
    // Llenar campos de texto
    for (const [key, value] of Object.entries(testUser)) {
        const element = document.querySelector(`input[name="${key}"], input[id="${key}"]`);
        if (element) {
            element.value = value;
            element.dispatchEvent(new Event('input', { bubbles: true }));
            console.log(`✅ ${key}: ${value}`);
        }
    }
    
    // Configurar checkboxes
    const activeCheckbox = document.querySelector('input[name="IsActive"], input[id="IsActive"]');
    if (activeCheckbox) {
        activeCheckbox.checked = true;
        activeCheckbox.dispatchEvent(new Event('change', { bubbles: true }));
        console.log('✅ Usuario marcado como activo');
    }
    
    const emailConfirmedCheckbox = document.querySelector('input[name="EmailConfirmed"], input[id="EmailConfirmed"]');
    if (emailConfirmedCheckbox) {
        emailConfirmedCheckbox.checked = true;
        emailConfirmedCheckbox.dispatchEvent(new Event('change', { bubbles: true }));
        console.log('✅ Email marcado como confirmado');
    }
    
    // Buscar y seleccionar roles disponibles
    console.log('🔍 Buscando roles disponibles...');
    const roleCheckboxes = document.querySelectorAll('input[type="checkbox"]:not([name="IsActive"]):not([name="EmailConfirmed"]):not([name="SendWelcomeEmail"]):not([name="TwoFactorEnabled"])');
    
    if (roleCheckboxes.length > 0) {
        console.log(`📋 Encontrados ${roleCheckboxes.length} roles/permisos disponibles`);
        
        // Buscar específicamente el rol de Docente
        const docenteRole = Array.from(roleCheckboxes).find(cb => {
            const label = cb.closest('label') || cb.nextElementSibling || cb.parentElement.querySelector('label');
            return label && label.textContent.includes('Docente');
        });
        
        if (docenteRole) {
            docenteRole.checked = true;
            docenteRole.dispatchEvent(new Event('change', { bubbles: true }));
            console.log('✅ Rol Docente asignado');
        } else {
            // Si no encontramos Docente, asignar el primer rol disponible
            roleCheckboxes[0].checked = true;
            roleCheckboxes[0].dispatchEvent(new Event('change', { bubbles: true }));
            console.log('✅ Primer rol disponible asignado');
        }
    } else {
        console.log('⚠️ No se encontraron roles disponibles para asignar');
    }
    
    console.log('📤 Formulario listo para envío');
    console.log('⚠️ IMPORTANTE: Revise los datos antes de continuar');
    console.log('🎯 Para enviar el formulario, ejecute: enviarFormularioUsuario()');
    
    // Guardar datos para función de envío
    window.testUserData = testUser;
}

async function enviarFormularioUsuario() {
    console.log('📤 Enviando formulario de creación de usuario...');
    
    const form = document.querySelector('form');
    if (!form) {
        console.log('❌ No se encontró el formulario');
        return;
    }
    
    // Verificar que tenemos datos
    const email = document.querySelector('input[name="Email"], input[id="Email"]').value;
    if (!email) {
        console.log('❌ El formulario está vacío. Ejecute primero: probarCreacionUsuarioAdmin()');
        return;
    }
    
    console.log(`👤 Creando usuario: ${email}`);
    
    // Enviar el formulario
    const submitButton = form.querySelector('button[type="submit"], input[type="submit"]');
    if (submitButton) {
        submitButton.click();
        console.log('✅ Formulario enviado');
        
        // Esperar un momento y verificar el resultado
        setTimeout(() => {
            verificarResultadoCreacion();
        }, 2000);
    } else {
        console.log('❌ No se encontró el botón de envío');
    }
}

function verificarResultadoCreacion() {
    console.log('🔍 Verificando resultado de la creación...');
    
    const currentUrl = window.location.href;
    console.log(`🔗 URL actual: ${currentUrl}`);
    
    // Buscar mensajes de éxito o error
    const successMessages = document.querySelectorAll('.alert-success, .success');
    const errorMessages = document.querySelectorAll('.alert-danger, .error, .text-danger');
    
    if (successMessages.length > 0) {
        console.log('✅ ¡Usuario creado exitosamente!');
        successMessages.forEach(msg => {
            console.log(`📝 Mensaje: ${msg.textContent.trim()}`);
        });
    } else if (errorMessages.length > 0) {
        console.log('❌ Errores encontrados:');
        errorMessages.forEach(msg => {
            console.log(`🔴 Error: ${msg.textContent.trim()}`);
        });
    } else if (currentUrl.includes('/Admin/Users') && !currentUrl.includes('/Create')) {
        console.log('✅ Redirigido a la lista de usuarios - creación probablemente exitosa');
    } else {
        console.log('⚠️ Resultado no claro - verificar manualmente');
    }
}

// Ejecutar automáticamente la verificación
probarCreacionUsuarioAdmin();
