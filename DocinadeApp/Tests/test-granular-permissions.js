// Script para probar el sistema de permisos granulares en el menú de navegación
// INSTRUCCIONES:
// 1. Abre el sistema con un usuario que tenga rol "Normal" pero con permisos específicos
// 2. Abre las herramientas de desarrollador (F12)
// 3. Ve a la consola y pega este script completo
// 4. Presiona Enter para ejecutarlo

console.log('🔐 Iniciando prueba del sistema de permisos granulares...');

function probarPermisosGranulares() {
    console.log('📊 Analizando menús visibles con permisos granulares...');
    
    // Verificar menús principales
    const navbarNav = document.querySelector('.navbar-nav');
    if (!navbarNav) {
        console.error('❌ No se encontró la barra de navegación');
        return;
    }
    
    console.log('✅ Barra de navegación encontrada');
    
    // Analizar menú de Configuración
    const configMenu = document.querySelector('#navbarDropdown');
    console.log('\n📋 Menú de Configuración:');
    console.log(`   - Visible: ${configMenu ? '✅ Sí' : '❌ No'}`);
    
    if (configMenu) {
        const configDropdown = configMenu.nextElementSibling;
        if (configDropdown && configDropdown.classList.contains('dropdown-menu')) {
            const menuItems = configDropdown.querySelectorAll('li a, li button');
            console.log(`   - Total de elementos: ${menuItems.length}`);
            
            // Verificar elementos específicos por permisos
            const permissionItems = {
                'Períodos Académicos': 'periodos.ver',
                'Rúbricas': 'rubricas.ver',
                'Niveles de Calificación': 'niveles.ver',
                'Instrumentos de Evaluación': 'evaluaciones.ver',
                'Estudiantes': 'estudiantes.ver',
                'Historial de Auditoría': 'auditoria.ver'
            };
            
            Object.entries(permissionItems).forEach(([itemName, permission]) => {
                const item = Array.from(menuItems).find(item => 
                    item.textContent?.includes(itemName)
                );
                console.log(`   - ${itemName}: ${item ? '✅ Visible' : '❌ Oculto'} (requiere: ${permission})`);
            });
        }
    }
    
    // Verificar menú de Evaluaciones
    const evaluacionesMenu = document.querySelector('a[href*="Evaluaciones"]');
    console.log('\n📝 Menú de Evaluaciones:');
    console.log(`   - Visible: ${evaluacionesMenu ? '✅ Sí' : '❌ No'} (requiere: evaluaciones.ver o rol específico)`);
    
    // Verificar menú de Reportes
    const reportesMenu = document.querySelector('#reportesDropdown');
    console.log('\n📊 Menú de Reportes:');
    console.log(`   - Visible: ${reportesMenu ? '✅ Sí' : '❌ No'} (requiere: reportes.ver_basicos o rol específico)`);
    
    // Verificar menú de Administración
    const adminMenu = document.querySelector('a[href*="Admin"]');
    console.log('\n⚙️  Menú de Administración:');
    console.log(`   - Visible: ${adminMenu ? '✅ Sí' : '❌ No'} (requiere: usuarios.ver o rol admin)`);
    
    // Verificar información del usuario actual
    console.log('\n👤 Información del usuario actual:');
    const userDropdown = document.querySelector('#userDropdown');
    if (userDropdown) {
        const userName = userDropdown.textContent?.trim();
        console.log(`   - Usuario: ${userName}`);
    }
    
    return {
        success: true,
        configMenuVisible: !!configMenu,
        evaluacionesMenuVisible: !!evaluacionesMenu,
        reportesMenuVisible: !!reportesMenu,
        adminMenuVisible: !!adminMenu
    };
}

function verificarPermisosEnConsola() {
    console.log('🔍 Verificando permisos del usuario actual...');
    
    // Esta función simularía la verificación de permisos
    // En un entorno real, esto se haría en el servidor
    const permisosEsperados = [
        'auditoria.ver',
        'configuracion.ver', 
        'estudiantes.ver',
        'evaluaciones.ver',
        'niveles.ver',
        'periodos.ver',
        'reportes.ver_basicos',
        'rubricas.ver',
        'usuarios.ver'
    ];
    
    console.log('📝 Permisos que deberían dar acceso a menús:');
    permisosEsperados.forEach((permiso, index) => {
        console.log(`   ${index + 1}. ${permiso}`);
    });
    
    console.log('\n💡 Para verificar permisos reales, revisa:');
    console.log('   1. Panel de administración → Usuarios');
    console.log('   2. Editar el usuario con rol "Normal"');
    console.log('   3. Verificar permisos asignados en la sección "Permisos Directos"');
    
    return permisosEsperados;
}

function simularUsuarioConPermisos() {
    console.log('🎭 Simulando comportamiento esperado para usuario Normal con permisos...');
    
    const escenarios = [
        {
            nombre: 'Usuario Normal sin permisos',
            permisos: [],
            menusVisibles: ['Inicio'],
            descripcion: 'Solo debería ver el menú de Inicio'
        },
        {
            nombre: 'Usuario Normal con permiso de auditoría',
            permisos: ['auditoria.ver'],
            menusVisibles: ['Inicio', 'Configuración (solo Historial de Auditoría)'],
            descripcion: 'Debería ver Configuración con acceso solo a Auditoría'
        },
        {
            nombre: 'Usuario Normal con permisos de evaluación',
            permisos: ['evaluaciones.ver'],
            menusVisibles: ['Inicio', 'Evaluaciones', 'Configuración (Instrumentos)'],
            descripcion: 'Debería ver menú de Evaluaciones y sección de Instrumentos'
        },
        {
            nombre: 'Usuario Normal con múltiples permisos de lectura',
            permisos: ['rubricas.ver', 'estudiantes.ver', 'reportes.ver_basicos', 'usuarios.ver'],
            menusVisibles: ['Inicio', 'Configuración (completo)', 'Reportes', 'Admin'],
            descripcion: 'Debería ver casi todos los menús pero solo en modo lectura'
        }
    ];
    
    console.log('📋 Escenarios de permisos:');
    escenarios.forEach((escenario, index) => {
        console.log(`\n${index + 1}. ${escenario.nombre}:`);
        console.log(`   Permisos: ${escenario.permisos.join(', ') || 'Ninguno'}`);
        console.log(`   Menús visibles: ${escenario.menusVisibles.join(', ')}`);
        console.log(`   Descripción: ${escenario.descripcion}`);
    });
    
    return escenarios;
}

// Ejecutar las pruebas
try {
    const resultado = probarPermisosGranulares();
    verificarPermisosEnConsola();
    simularUsuarioConPermisos();
    
    console.log('\n🎉 ANÁLISIS COMPLETADO');
    console.log('\n📊 Resumen de visibilidad actual:');
    console.log(`   - Menú Configuración: ${resultado.configMenuVisible ? '✅ Visible' : '❌ Oculto'}`);
    console.log(`   - Menú Evaluaciones: ${resultado.evaluacionesMenuVisible ? '✅ Visible' : '❌ Oculto'}`);
    console.log(`   - Menú Reportes: ${resultado.reportesMenuVisible ? '✅ Visible' : '❌ Oculto'}`);
    console.log(`   - Menú Admin: ${resultado.adminMenuVisible ? '✅ Visible' : '❌ Oculto'}`);
    
    console.log('\n🔧 Para probar con diferentes permisos:');
    console.log('   1. Ve al Panel de Administración');
    console.log('   2. Crea un usuario con rol "Normal"');
    console.log('   3. Asigna permisos específicos de lectura');
    console.log('   4. Inicia sesión con ese usuario');
    console.log('   5. Verifica que solo vea los menús correspondientes a sus permisos');
    
} catch (error) {
    console.error('❌ Error durante el análisis:', error);
}
