// Script de prueba para debuggear el buscador de estudiantes

console.log('✅ Script de prueba cargado');

document.addEventListener('DOMContentLoaded', function() {
    console.log('✅ DOM cargado');
    
    // Verificar si el BuscadorEstudiante existe
    if (typeof BuscadorEstudiante !== 'undefined') {
        console.log('✅ Clase BuscadorEstudiante encontrada');
        
        // Probar inicialización básica
        try {
            const testBuscador = new BuscadorEstudiante({
                selectId: 'estudiante1',
                modalId: 'modalBasico',
                buscarUrl: '/Test/BuscarEstudiantes',
                debug: true
            });
            console.log('✅ BuscadorEstudiante inicializado correctamente');
        } catch (error) {
            console.error('❌ Error al inicializar BuscadorEstudiante:', error);
        }
    } else {
        console.error('❌ Clase BuscadorEstudiante NO encontrada');
    }
    
    // Verificar elementos del DOM
    const modal1 = document.getElementById('modalBasico');
    const select1 = document.getElementById('estudiante1');
    const txtBusqueda1 = document.getElementById('txtBusquedamodalBasico');
    
    console.log('DOM elements check:');
    console.log('Modal modalBasico:', modal1 ? '✅ Encontrado' : '❌ No encontrado');
    console.log('Select estudiante1:', select1 ? '✅ Encontrado' : '❌ No encontrado');
    console.log('Input búsqueda:', txtBusqueda1 ? '✅ Encontrado' : '❌ No encontrado');
    
    // Probar búsqueda manual
    if (txtBusqueda1) {
        txtBusqueda1.addEventListener('input', function() {
            console.log('📝 Input detectado:', this.value);
            if (this.value.length >= 2) {
                console.log('🔍 Iniciando búsqueda manual...');
                
                fetch('/Test/BuscarEstudiantes?term=' + encodeURIComponent(this.value))
                    .then(response => {
                        console.log('📡 Respuesta recibida:', response.status);
                        return response.json();
                    })
                    .then(data => {
                        console.log('📊 Datos recibidos:', data);
                    })
                    .catch(error => {
                        console.error('❌ Error en búsqueda:', error);
                    });
            }
        });
    }
});