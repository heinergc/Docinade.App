// Script para probar la funcionalidad de logout desde el panel Admin
// INSTRUCCIONES:
// 1. Abre cualquier página del admin como: https://localhost:18163/Admin/Users
// 2. Inicia sesión como administrador
// 3. Abre las herramientas de desarrollador (F12)
// 4. Ve a la consola y pega este script completo
// 5. Presiona Enter para ejecutarlo

console.log('🚪 Iniciando prueba de funcionalidad de logout desde panel Admin...');

function probarLogoutAdmin() {
    // Verificar que estamos en el área admin
    if (!window.location.pathname.includes('/Admin/')) {
        console.error('❌ No estás en el área de administración');
        console.log('📍 Navega a: https://localhost:18163/Admin/Users o similar');
        return;
    }
    
    console.log('✅ Página del área Admin detectada');
    console.log(`📍 URL actual: ${window.location.href}`);
    
    // Buscar el formulario de logout
    const logoutForm = document.querySelector('form[action*="Logout"]');
    const logoutButton = document.querySelector('button[type="submit"]:has(i.fa-sign-out-alt)') || 
                        document.querySelector('button:contains("Cerrar Sesión")') ||
                        document.querySelector('form[action*="Logout"] button[type="submit"]');
    
    console.log('🔍 Verificando elementos de logout:');
    console.log(`   - Formulario de logout: ${logoutForm ? '✅ Encontrado' : '❌ No encontrado'}`);
    console.log(`   - Botón de logout: ${logoutButton ? '✅ Encontrado' : '❌ No encontrado'}`);
    
    if (logoutForm) {
        const action = logoutForm.getAttribute('action') || logoutForm.action;
        console.log(`   - Acción del formulario: ${action}`);
        
        // Verificar el método
        const method = logoutForm.getAttribute('method') || logoutForm.method;
        console.log(`   - Método: ${method}`);
        
        // Verificar token anti-falsificación
        const antiForgeryToken = logoutForm.querySelector('input[name="__RequestVerificationToken"]');
        console.log(`   - Token anti-falsificación: ${antiForgeryToken ? '✅ Presente' : '❌ Ausente'}`);
    }
    
    // Verificar enlaces en el menú de usuario
    const userMenu = document.querySelector('.dropdown-menu') || document.querySelector('[role="menu"]');
    if (userMenu) {
        console.log('📋 Menú de usuario encontrado');
        const menuItems = userMenu.querySelectorAll('a, button');
        console.log(`   - Total de elementos en menú: ${menuItems.length}`);
        
        menuItems.forEach((item, index) => {
            const text = item.textContent?.trim();
            const href = item.getAttribute('href');
            if (text) {
                console.log(`   ${index + 1}. "${text}" ${href ? `→ ${href}` : '(botón)'}`);
            }
        });
    }
    
    // Verificar dropdown del usuario
    const userDropdown = document.querySelector('[data-bs-toggle="dropdown"]') || 
                        document.querySelector('.dropdown-toggle');
    
    console.log('');
    console.log('👤 Estado del usuario:');
    console.log(`   - Dropdown de usuario: ${userDropdown ? '✅ Encontrado' : '❌ No encontrado'}`);
    
    if (userDropdown) {
        const userText = userDropdown.textContent?.trim();
        console.log(`   - Texto del usuario: "${userText}"`);
    }
    
    // Función para probar el logout
    console.log('');
    console.log('🎯 Funciones de prueba disponibles:');
    console.log('   - simularLogout() - Simula clic en logout (¡cerrará la sesión!)');
    console.log('   - verificarRutaLogout() - Verifica la URL del logout');
    
    // Crear funciones globales
    window.simularLogout = function() {
        console.log('⚠️  ADVERTENCIA: Esto cerrará tu sesión actual');
        console.log('💡 Asegúrate de guardar cualquier trabajo antes de continuar');
        
        if (confirm('¿Estás seguro de que quieres cerrar sesión?')) {
            if (logoutButton) {
                console.log('🚪 Haciendo clic en el botón de logout...');
                logoutButton.click();
            } else if (logoutForm) {
                console.log('🚪 Enviando formulario de logout...');
                logoutForm.submit();
            } else {
                console.error('❌ No se encontró método para hacer logout');
            }
        } else {
            console.log('👍 Logout cancelado por el usuario');
        }
    };
    
    window.verificarRutaLogout = function() {
        if (logoutForm) {
            const action = logoutForm.getAttribute('action') || logoutForm.action;
            console.log('🔍 Análisis de la ruta de logout:');
            console.log(`   - Acción del formulario: "${action}"`);
            
            // Construir URL completa
            const baseUrl = window.location.origin;
            let fullUrl;
            
            if (action.startsWith('/')) {
                fullUrl = baseUrl + action;
            } else if (action.startsWith('http')) {
                fullUrl = action;
            } else {
                fullUrl = window.location.href.replace(/\/[^\/]*$/, '/') + action;
            }
            
            console.log(`   - URL completa: "${fullUrl}"`);
            
            // Verificar si la ruta es correcta
            if (action.includes('/Account/Logout')) {
                console.log('✅ La ruta de logout parece correcta');
            } else {
                console.log('⚠️  La ruta de logout puede no ser la esperada');
            }
        } else {
            console.error('❌ No se encontró el formulario de logout');
        }
    };
    
    return {
        success: true,
        logoutForm: logoutForm ? 'Encontrado' : 'No encontrado',
        logoutButton: logoutButton ? 'Encontrado' : 'No encontrado',
        userDropdown: userDropdown ? 'Encontrado' : 'No encontrado'
    };
}

// Ejecutar la prueba
try {
    const resultado = probarLogoutAdmin();
    
    if (resultado.success) {
        console.log('');
        console.log('🎉 VERIFICACIÓN COMPLETADA');
        console.log('');
        console.log('📊 Resumen de elementos:');
        console.log(`   - Formulario de logout: ${resultado.logoutForm}`);
        console.log(`   - Botón de logout: ${resultado.logoutButton}`);
        console.log(`   - Menú de usuario: ${resultado.userDropdown}`);
        console.log('');
        console.log('🔧 Para probar el logout:');
        console.log('   1. Ejecuta: verificarRutaLogout()');
        console.log('   2. Luego: simularLogout() (¡esto cerrará tu sesión!)');
    }
} catch (error) {
    console.error('❌ Error durante la verificación:', error);
}
