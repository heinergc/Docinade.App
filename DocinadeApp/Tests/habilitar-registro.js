// Script para habilitar el registro de usuarios automáticamente
// Ejecutar en la consola del navegador después de hacer login como admin

async function habilitarRegistro() {
    console.log('🔧 Habilitando registro de usuarios...');
    
    try {
        // 1. Primero hacer login como admin si no estamos logueados
        console.log('🔑 Verificando sesión admin...');
        
        // Verificar si ya estamos logueados como admin
        let currentPage = await fetch('/Admin/Configuracion');
        
        if (!currentPage.ok || currentPage.url.includes('/Login')) {
            console.log('🔐 Haciendo login como admin...');
            
            // Login como admin
            const loginData = new FormData();
            loginData.append('Email', 'admin@rubricas.edu');
            loginData.append('Password', 'Admin123!');
            
            const loginResponse = await fetch('/Account/Login', {
                method: 'POST',
                body: loginData,
                credentials: 'same-origin'
            });
            
            if (loginResponse.ok && !loginResponse.url.includes('/Login')) {
                console.log('✅ Login admin exitoso');
            } else {
                console.log('❌ Error en login admin');
                return false;
            }
        } else {
            console.log('✅ Ya logueado como admin');
        }
        
        // 2. Acceder a la página de configuración
        console.log('📋 Accediendo a configuración...');
        const configPage = await fetch('/Admin/Configuracion');
        
        if (!configPage.ok) {
            console.log('❌ No se pudo acceder a la configuración');
            return false;
        }
        
        // 3. Actualizar el modo de registro a "Abierto"
        console.log('🔓 Cambiando modo de registro a Abierto...');
        
        const updateData = new FormData();
        updateData.append('ModoRegistro', 'Abierto');
        updateData.append('MensajeRegistroCerrado', 'El registro está habilitado.');
        
        // Obtener el token anti-falsificación si es necesario
        const configHtml = await configPage.text();
        const tokenMatch = configHtml.match(/name="__RequestVerificationToken".*?value="([^"]+)"/);
        if (tokenMatch) {
            updateData.append('__RequestVerificationToken', tokenMatch[1]);
        }
        
        const updateResponse = await fetch('/Admin/Configuracion/ActualizarModoRegistro', {
            method: 'POST',
            body: updateData,
            credentials: 'same-origin'
        });
        
        if (updateResponse.ok) {
            console.log('✅ ¡Modo de registro actualizado exitosamente!');
            
            // 4. Verificar que el cambio se aplicó
            console.log('🔍 Verificando cambio...');
            const testRegistro = await fetch('/Account/Register');
            const registroHtml = await testRegistro.text();
            
            if (registroHtml.includes('Email') && registroHtml.includes('Password') && 
                !registroHtml.includes('cerrado') && !registroHtml.includes('deshabilitado')) {
                console.log('✅ ¡Registro ahora está habilitado!');
                return true;
            } else {
                console.log('⚠️ El cambio no se reflejó inmediatamente');
                return false;
            }
        } else {
            console.log('❌ Error al actualizar modo de registro');
            return false;
        }
        
    } catch (error) {
        console.log('❌ Error:', error.message);
        return false;
    }
}

async function probarRegistroCompleto() {
    console.log('🎯 Iniciando prueba completa de registro...');
    
    // 1. Habilitar registro
    const habilitado = await habilitarRegistro();
    
    if (!habilitado) {
        console.log('❌ No se pudo habilitar el registro');
        return;
    }
    
    // 2. Esperar un momento
    await new Promise(resolve => setTimeout(resolve, 2000));
    
    // 3. Crear usuario de prueba
    console.log('👤 Creando usuario de prueba...');
    
    const timestamp = Date.now();
    const testUser = {
        Email: `testuser${timestamp}@test.com`,
        Password: 'Test123!',
        ConfirmPassword: 'Test123!',
        Nombre: 'Usuario',
        Apellidos: 'De Prueba',
        NumeroIdentificacion: `TEST${timestamp}`,
        Institucion: 'Universidad Test',
        Departamento: 'Departamento Test',
        SelectedRole: 'Docente'
    };
    
    // Ir a la página de registro
    const registroPage = await fetch('/Account/Register');
    const registroHtml = await registroPage.text();
    
    // Obtener token anti-falsificación
    const tokenMatch = registroHtml.match(/name="__RequestVerificationToken".*?value="([^"]+)"/);
    
    const formData = new FormData();
    Object.keys(testUser).forEach(key => {
        formData.append(key, testUser[key]);
    });
    
    if (tokenMatch) {
        formData.append('__RequestVerificationToken', tokenMatch[1]);
    }
    
    // Enviar formulario de registro
    const registroResponse = await fetch('/Account/Register', {
        method: 'POST',
        body: formData,
        credentials: 'same-origin'
    });
    
    if (registroResponse.ok) {
        const resultUrl = registroResponse.url;
        
        if (resultUrl.includes('/Home') || resultUrl.includes('/Dashboard')) {
            console.log('✅ ¡ÉXITO! Usuario creado y logueado automáticamente');
            console.log(`📧 Email: ${testUser.Email}`);
            console.log(`🔑 Password: ${testUser.Password}`);
            return true;
        } else {
            console.log('⚠️ Usuario posiblemente creado, verificando...');
            
            // Intentar login manual
            const loginData = new FormData();
            loginData.append('Email', testUser.Email);
            loginData.append('Password', testUser.Password);
            
            const loginTest = await fetch('/Account/Login', {
                method: 'POST',
                body: loginData,
                credentials: 'same-origin'
            });
            
            if (loginTest.ok && !loginTest.url.includes('/Login')) {
                console.log('✅ ¡Usuario creado exitosamente! (login manual exitoso)');
                return true;
            } else {
                console.log('❌ Error en la creación del usuario');
                return false;
            }
        }
    } else {
        console.log('❌ Error al enviar formulario de registro');
        return false;
    }
}

// Ejecutar la prueba completa
probarRegistroCompleto();
