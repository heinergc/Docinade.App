// Script simple para verificar el registro de usuarios
// Ejecutar desde la consola del navegador en http://localhost:18163

async function verificarRegistro() {
    console.log('🔍 Verificando estado del registro de usuarios...');
    
    try {
        // 1. Verificar si podemos acceder a la página de registro
        const response = await fetch('/Account/Register');
        
        if (response.ok) {
            const html = await response.text();
            
            // Verificar si el registro está cerrado
            if (html.includes('registro') && (html.includes('cerrado') || html.includes('deshabilitado'))) {
                console.log('❌ PROBLEMA: El registro está cerrado/deshabilitado');
                console.log('📝 Texto encontrado:', html.match(/registro.*?(cerrado|deshabilitado|temporalmente)/i)?.[0]);
                
                // Buscar el mensaje específico
                const mensajeMatch = html.match(/<[^>]*class[^>]*mensaje[^>]*>([^<]+)</i);
                if (mensajeMatch) {
                    console.log('💬 Mensaje específico:', mensajeMatch[1]);
                }
                
                return { registroAbierto: false, mensaje: 'Registro cerrado' };
            }
            
            // Verificar si hay un formulario de registro
            if (html.includes('<form') && html.includes('Email') && html.includes('Password')) {
                console.log('✅ Formulario de registro disponible');
                
                // Extraer campos del formulario
                const campos = [];
                const inputMatches = html.matchAll(/<input[^>]*name=['""]([^'""]+)['""][^>]*>/gi);
                for (const match of inputMatches) {
                    campos.push(match[1]);
                }
                
                console.log('📋 Campos encontrados:', campos);
                return { registroAbierto: true, campos: campos };
            }
            
        } else {
            console.log(`❌ Error al acceder a /Account/Register: ${response.status}`);
            return { registroAbierto: false, error: response.status };
        }
        
    } catch (error) {
        console.log('❌ Error:', error.message);
        return { registroAbierto: false, error: error.message };
    }
}

async function probarCreacionUsuario() {
    console.log('👤 Probando creación de usuario...');
    
    const timestamp = Date.now();
    const usuario = {
        Email: `test${timestamp}@test.com`,
        Password: 'Test123!',
        ConfirmPassword: 'Test123!',
        Nombre: 'Usuario',
        Apellidos: 'Prueba',
        NumeroIdentificacion: `TEST${timestamp}`,
        Institucion: 'Universidad Test',
        Departamento: 'Departamento Test',
        SelectedRole: 'Docente'
    };
    
    try {
        // Crear FormData
        const formData = new FormData();
        Object.keys(usuario).forEach(key => {
            formData.append(key, usuario[key]);
        });
        
        // Enviar solicitud POST
        const response = await fetch('/Account/Register', {
            method: 'POST',
            body: formData,
            credentials: 'same-origin'
        });
        
        console.log(`📤 Respuesta: ${response.status} ${response.statusText}`);
        
        if (response.ok) {
            const resultHtml = await response.text();
            
            // Verificar si fue exitoso (redirigido)
            if (response.redirected || resultHtml.includes('Dashboard') || resultHtml.includes('Bienvenido')) {
                console.log('✅ ¡Usuario creado exitosamente!');
                return { success: true, usuario: usuario.Email };
            }
            
            // Buscar errores en el HTML
            const errorPattern = /(error|validation|required|invalid)/i;
            if (errorPattern.test(resultHtml)) {
                console.log('❌ Errores en el formulario detectados');
                
                // Extraer mensajes de error específicos
                const errorMatches = resultHtml.matchAll(/<[^>]*(?:error|validation)[^>]*>([^<]+)</gi);
                for (const match of errorMatches) {
                    console.log('🔴 Error:', match[1].trim());
                }
            }
            
        } else {
            console.log(`❌ Error HTTP: ${response.status}`);
        }
        
    } catch (error) {
        console.log('❌ Error en la solicitud:', error.message);
    }
}

async function verificarUsuariosExistentes() {
    console.log('👥 Verificando usuarios existentes...');
    
    const usuariosPrueba = [
        { email: 'admin@rubricas.edu', password: 'Admin123!' },
        { email: 'docente@rubricas.edu', password: 'Docente123!' }
    ];
    
    for (const user of usuariosPrueba) {
        try {
            const formData = new FormData();
            formData.append('Email', user.email);
            formData.append('Password', user.password);
            
            const response = await fetch('/Account/Login', {
                method: 'POST',
                body: formData,
                credentials: 'same-origin'
            });
            
            if (response.ok && !response.url.includes('/Login')) {
                console.log(`✅ Usuario ${user.email} existe y puede loguearse`);
            } else {
                console.log(`❌ Usuario ${user.email} no puede loguearse`);
            }
            
        } catch (error) {
            console.log(`❌ Error probando ${user.email}:`, error.message);
        }
    }
}

// Función principal
async function diagnosticoCompleto() {
    console.log('🚀 Iniciando diagnóstico completo...');
    
    await verificarRegistro();
    await new Promise(resolve => setTimeout(resolve, 1000)); // Pausa
    
    await verificarUsuariosExistentes();
    await new Promise(resolve => setTimeout(resolve, 1000)); // Pausa
    
    await probarCreacionUsuario();
    
    console.log('✨ Diagnóstico completo terminado');
}

// Ejecutar automáticamente
diagnosticoCompleto();
