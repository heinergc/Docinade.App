// Test directo de endpoints de AsignarEstudiantes
console.log('🧪 Iniciando pruebas de endpoints de AsignarEstudiantes...');

// Función para probar un endpoint
async function testEndpoint(url, description) {
    try {
        console.log(`\n🔍 Probando: ${description}`);
        console.log(`📍 URL: ${url}`);
        
        const response = await fetch(url, {
            method: 'GET',
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json'
            }
        });
        
        console.log(`📊 Status: ${response.status} ${response.statusText}`);
        
        if (response.ok) {
            const data = await response.json();
            console.log('✅ Respuesta exitosa:', data);
            return data;
        } else {
            const errorText = await response.text();
            console.log('❌ Error en respuesta:', errorText);
            return null;
        }
    } catch (error) {
        console.log('💥 Error de red:', error.message);
        return null;
    }
}

// Probar endpoints
async function runTests() {
    const baseUrl = window.location.origin;
    
    console.log(`🌐 Base URL: ${baseUrl}`);
    
    // Test 0: Debug endpoint 
    await testEndpoint(
        `${baseUrl}/GruposEstudiantes/DebugEstudiantes`, 
        'Debug: Verificar datos en base de datos'
    );
    
    // Test 1: Estudiantes disponibles (sin grupo)
    await testEndpoint(
        `${baseUrl}/GruposEstudiantes/GetEstudiantesDisponibles`, 
        'Obtener TODOS los estudiantes disponibles'
    );
    
    // Test 2: Estudiantes disponibles para un grupo específico
    await testEndpoint(
        `${baseUrl}/GruposEstudiantes/GetEstudiantesDisponibles?grupoId=1`, 
        'Obtener estudiantes disponibles para grupo 1'
    );
    
    // Test 3: Estudiantes asignados (necesita un GrupoId válido)
    await testEndpoint(
        `${baseUrl}/GruposEstudiantes/GetEstudiantesAsignados?grupoId=1`, 
        'Obtener estudiantes asignados al grupo 1'
    );
    
    console.log('\n🏁 Pruebas completadas');
}

// Función específica para verificar datos
async function verificarDatos() {
    const baseUrl = window.location.origin;
    console.log('📊 === VERIFICACIÓN DE DATOS ===');
    
    const debug = await testEndpoint(
        `${baseUrl}/GruposEstudiantes/DebugEstudiantes`, 
        'Verificar estado de la base de datos'
    );
    
    if (debug) {
        console.log(`\n📈 RESUMEN:`);
        console.log(`   👥 Total estudiantes: ${debug.totalEstudiantes}`);
        console.log(`   🏫 Total grupos: ${debug.totalGrupos}`);
        console.log(`   🔗 Total asignaciones: ${debug.totalAsignaciones}`);
        console.log(`   📅 Período activo: ${debug.periodoActivo ? debug.periodoActivo.nombre : 'NINGUNO'}`);
        
        if (debug.totalEstudiantes === 0) {
            console.log('\n⚠️ NO HAY ESTUDIANTES EN LA BASE DE DATOS');
            console.log('💡 Necesitas ejecutar el script de datos de prueba');
        } else {
            console.log('\n✅ Datos encontrados en la base de datos');
            console.log('🔍 Primeros estudiantes:', debug.primerosEstudiantes);
        }
    }
}

// Ejecutar las pruebas
console.log('Usa runTests() para probar todos los endpoints');
console.log('Usa verificarDatos() para ver el estado de la base de datos');

// Auto-ejecutar verificación
verificarDatos();
