/**
 * JavaScript para la vista Create de Evaluaciones
 * Maneja la creación dinámica de formularios de evaluación
 */

class EvaluacionesCreate {
    constructor() {
        this.itemsRubrica = [];
        this.nivelesRubrica = [];
        this.baseUrl = '';
        this.init();
    }

    /**
     * Inicializa la clase y configura los event listeners
     */
    init() {
        console.log('🚀 Iniciando aplicación de evaluaciones...');
        console.log('📋 Variables inicializadas - Items:', this.itemsRubrica, 'Niveles:', this.nivelesRubrica);
        
        this.setupEventListeners();
        this.checkPreselectedRubrica();
        this.setupDebugFunction();
        
        console.log('🛠️ Función de debug disponible: window.debugEvaluacion()');
    }

    /**
     * Configura todos los event listeners necesarios
     */
    setupEventListeners() {
        // Cargar items cuando se selecciona una rúbrica
        $('#selectRubrica').change((e) => {
            const rubricaId = $(e.target).val();
            
            if (rubricaId) {
                this.cargarItemsRubrica(rubricaId);
            } else {
                $('#seccionEvaluacion').hide();
                $('#btnGuardar').prop('disabled', true);
            }
        });
        
        // Verificar validación cuando cambian los niveles
        $(document).on('change', '.nivel-select', () => {
            this.verificarValidacion();
            this.calcularPuntuacion();
        });
        
        // Validar formulario antes de enviar
        $('#formEvaluacion').submit((e) => {
            return this.validarFormulario(e);
        });
    }

    /**
     * Verifica si hay una rúbrica preseleccionada y carga sus items
     */
    checkPreselectedRubrica() {
        const rubricaPreseleccionada = $('#selectRubrica').val();
        if (rubricaPreseleccionada) {
            this.cargarItemsRubrica(rubricaPreseleccionada);
        }
    }

    /**
     * Configura la URL base para las peticiones AJAX
     * @param {string} url - URL base del controlador
     */
    setBaseUrl(url) {
        this.baseUrl = url;
    }

    /**
     * Carga los items y niveles de una rúbrica específica
     * @param {number} rubricaId - ID de la rúbrica
     */
    cargarItemsRubrica(rubricaId) {
        console.log('🔄 Cargando items para rúbrica ID:', rubricaId);
        
        $.ajax({
            url: this.baseUrl,
            type: 'GET',
            data: { rubricaId: rubricaId },
            beforeSend: () => {
                console.log('📡 Enviando petición AJAX...');
                $('#itemsEvaluacion').html('<div class="text-center"><i class="fas fa-spinner fa-spin"></i> Cargando items...</div>');
                $('#seccionEvaluacion').show();
            },
            success: (response) => {
                this.handleAjaxSuccess(response);
            },
            error: (xhr, status, error) => {
                console.error('❌ Error AJAX:', {
                    status: status,
                    error: error,
                    responseText: xhr.responseText
                });
                $('#itemsEvaluacion').html('<div class="alert alert-danger">Error al cargar los items de la rúbrica: ' + error + '</div>');
            }
        });
    }

    /**
     * Maneja la respuesta exitosa del AJAX
     * @param {object} response - Respuesta del servidor
     */
    handleAjaxSuccess(response) {
        console.log('✅ Respuesta del servidor recibida:', response);
        
        if (response.error) {
            console.error('❌ Error en la respuesta:', response.error);
            $('#itemsEvaluacion').html('<div class="alert alert-danger">Error: ' + response.error + '</div>');
            return;
        }
        
        console.log('📋 Items recibidos:', response.items);
        console.log('🎯 Niveles recibidos:', response.niveles);
        
        this.itemsRubrica = response.items;
        this.nivelesRubrica = response.niveles;
        
        console.log('📊 Items almacenados localmente:', this.itemsRubrica);
        console.log('🎚️ Niveles almacenados localmente:', this.nivelesRubrica);
        
        this.generarFormularioEvaluacion();
        this.verificarValidacion();
    }

    /**
     * Genera el formulario dinámico de evaluación
     */
    generarFormularioEvaluacion() {
        console.log('🎨 Iniciando generación del formulario...');
        console.log('📁 Items disponibles:', this.itemsRubrica);
        console.log('🎆 Niveles disponibles:', this.nivelesRubrica);
        
        let html = '';
       
        if (Array.isArray(this.itemsRubrica) && this.itemsRubrica.length === 0) {
            console.warn('⚠️ No hay items en la rúbrica');
            html = '<div class="alert alert-warning">Esta rúbrica no tiene items de evaluación configurados.</div>';
        } else {
            html = this.buildTableHTML();
        }
        
        console.log('🎉 HTML generado:', html.substring(0, 200) + '...');
        $('#itemsEvaluacion').html(html);
        console.log('✅ Formulario insertado en el DOM');
    }

    /**
     * Construye el HTML de la tabla de evaluación
     * @returns {string} HTML de la tabla
     */
    buildTableHTML() {
        console.log('📝 Generando tabla con', this.itemsRubrica.length, 'items');
        
        let html = '<div class="table-responsive"><table class="table table-bordered">';
        html += '<thead class="table-dark">';
        html += '<tr><th>Item de Evaluación</th><th>Peso</th><th>Nivel de Calificación</th><th>Puntos</th></tr>';
        html += '</thead><tbody>';
        
        this.itemsRubrica.forEach((item, index) => {
            html += this.buildItemRow(item, index);
        });
        
        html += '</tbody></table></div>';
        return html;
    }

    /**
     * Construye una fila de la tabla para un item específico
     * @param {object} item - Item de evaluación
     * @param {number} index - Índice del item
     * @returns {string} HTML de la fila
     */
    buildItemRow(item, index) {
        console.log(`🔍 Procesando item ${index + 1}:`, item);
        
        // El controlador envía con PascalCase pero puede llegar como camelCase por serialización JSON
        const itemId = item.IdItem || item.idItem;
        const descripcion = item.Descripcion || item.descripcion || 'Sin descripción';
        const peso = item.Peso || item.peso || '0';
        const ordenItem = item.OrdenItem || item.ordenItem;
        
        console.log('  - IdItem:', itemId);
        console.log('  - Descripcion:', descripcion);
        console.log('  - Peso:', peso);
        console.log('  - OrdenItem:', ordenItem);
        
        const nivelesItem = this.nivelesRubrica.filter(n => 
            (n.IdItem || n.idItem) === itemId
        );
        console.log(`  - Niveles para este item (${nivelesItem.length}):`, nivelesItem);
        
        let html = '<tr>';
        html += '<td><strong>' + descripcion + '</strong></td>';
        html += '<td>' + peso + '%</td>';
        html += '<td>' + this.buildSelectHTML(nivelesItem, index, itemId) + '</td>';
        html += '<td><span class="puntos-item badge bg-secondary" data-item-id="' + itemId + '">0</span></td>';
        html += '</tr>';
        
        return html;
    }

    /**
     * Construye el HTML del select de niveles
     * @param {array} nivelesItem - Niveles disponibles para el item
     * @param {number} index - Índice del item
     * @param {number} itemId - ID del item
     * @returns {string} HTML del select
     */
    buildSelectHTML(nivelesItem, index, itemId) {
        let html = '<select name="DetallesEvaluacion[' + index + '].IdNivel" class="form-select nivel-select" data-item-id="' + itemId + '" required>';
        html += '<option value="">-- Seleccionar nivel --</option>';
        
        nivelesItem.forEach((nivel) => {
            // El controlador envía con PascalCase pero puede llegar como camelCase
            const idNivel = nivel.IdNivel || nivel.idNivel;
            const nombreNivel = nivel.NombreNivel || nivel.nombreNivel;
            const valor = nivel.Valor || nivel.valor;
            
            console.log('    * Agregando nivel:', {
                IdNivel: idNivel,
                NombreNivel: nombreNivel,
                Valor: valor
            });
            html += '<option value="' + idNivel + '" data-valor="' + valor + '">' + nombreNivel + ' (' + valor + ' pts)</option>';
        });
        
        html += '</select>';
        html += '<input type="hidden" name="DetallesEvaluacion[' + index + '].IdItem" value="' + itemId + '" />';
        
        return html;
    }

    /**
     * Verifica si el formulario está completamente validado
     */
    verificarValidacion() {
        let todosCompletos = true;
        const estudiante = $('#selectEstudiante').val();
        const rubrica = $('#selectRubrica').val();
        
        if (!estudiante || !rubrica) {
            todosCompletos = false;
        }
        
        $('.nivel-select').each(function() {
            if (!$(this).val()) {
                todosCompletos = false;
            }
        });
        
        $('#btnGuardar').prop('disabled', !todosCompletos);
        
        if (todosCompletos && this.itemsRubrica.length > 0) {
            $('#resumenPuntuacion').show();
        } else {
            $('#resumenPuntuacion').hide();
        }
    }

    /**
     * Calcula y muestra la puntuación total
     */
    calcularPuntuacion() {
        let totalPuntos = 0;
        const detalles = [];
        
        $('.nivel-select').each((index, element) => {
            const select = $(element);
            const itemId = select.data('item-id');
            const opcionSeleccionada = select.find('option:selected');
            const valor = parseFloat(opcionSeleccionada.data('valor') || 0);
            const nombreNivel = opcionSeleccionada.text();
            
            // Actualizar puntos del item
            $('.puntos-item[data-item-id="' + itemId + '"]').text(valor.toFixed(2));
            
            if (valor > 0) {
                totalPuntos += valor;
                const item = this.itemsRubrica.find(i => 
                    (i.IdItem || i.idItem) === itemId
                );
                if (item) {
                    detalles.push({
                        descripcion: item.Descripcion || item.descripcion || 'Sin descripción',
                        nivel: nombreNivel,
                        puntos: valor
                    });
                }
            }
        });
        
        $('#totalPuntos').text(totalPuntos.toFixed(2));
        this.mostrarDetallesPuntuacion(detalles);
    }

    /**
     * Muestra los detalles de la puntuación
     * @param {array} detalles - Array con los detalles de puntuación
     */
    mostrarDetallesPuntuacion(detalles) {
        let htmlDetalles = '';
        detalles.forEach((detalle) => {
            htmlDetalles += '<div class="row"><div class="col-8">' + detalle.descripcion + '</div>';
            htmlDetalles += '<div class="col-2">' + detalle.nivel.split('(')[0].trim() + '</div>';
            htmlDetalles += '<div class="col-2 text-end">' + detalle.puntos.toFixed(2) + ' pts</div></div>';
        });
        
        $('#detallesPuntuacion').html(htmlDetalles);
    }

    /**
     * Valida el formulario antes de enviarlo
     * @param {Event} e - Evento del formulario
     * @returns {boolean} True si es válido, false en caso contrario
     */
    validarFormulario(e) {
        const estudianteId = $('#selectEstudiante').val();
        const rubricaId = $('#selectRubrica').val();
        
        if (!estudianteId || !rubricaId) {
            e.preventDefault();
            alert('Debe seleccionar un estudiante y una rúbrica.');
            return false;
        }
        
        const nivelesSinSeleccionar = $('.nivel-select').filter(function() {
            return !$(this).val();
        }).length;
        
        if (nivelesSinSeleccionar > 0) {
            e.preventDefault();
            alert('Debe seleccionar un nivel de calificación para todos los items.');
            return false;
        }
        
        return true;
    }

    /**
     * Configura la función de debug global
     */
    setupDebugFunction() {
        window.debugEvaluacion = () => {
            console.log('🔧 DEBUG - Estado actual de la aplicación:');
            console.log('  📋 Items en memoria:', this.itemsRubrica);
            console.log('  🎆 Niveles en memoria:', this.nivelesRubrica);
            console.log('  🎯 Rúbrica seleccionada:', $('#selectRubrica').val());
            console.log('  👥 Estudiante seleccionado:', $('#selectEstudiante').val());
            console.log('  📝 Elementos .nivel-select encontrados:', $('.nivel-select').length);
            console.log('  🌐 URL del controlador:', this.baseUrl);
            
            // Mostrar estructura de datos recibidos
            if (this.itemsRubrica.length > 0) {
                console.log('  📄 Estructura de un item:', this.itemsRubrica[0]);
            }
            if (this.nivelesRubrica.length > 0) {
                console.log('  🎚️ Estructura de un nivel:', this.nivelesRubrica[0]);
            }
            
            return {
                items: this.itemsRubrica,
                niveles: this.nivelesRubrica,
                rubricaId: $('#selectRubrica').val(),
                estudianteId: $('#selectEstudiante').val(),
                url: this.baseUrl
            };
        };
    }
}

// Inicializar cuando el documento esté listo
$(document).ready(function() {
    window.evaluacionesCreate = new EvaluacionesCreate();
});