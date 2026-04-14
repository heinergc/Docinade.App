/**
 * Buscador Avanzado de Estudiantes
 * Sistema de busqueda modal con debounce y selecci�n inteligente
 * Compatible con Bootstrap 5 y ASP.NET Core MVC 8.0
 * 
 * @author Sistema de Rubricas
 * @version 2.0
 */

class BuscadorEstudiante {
    constructor(options = {}) {
        // Configuración por defecto
        this.config = {
            selectId: 'estudianteId',
            modalId: 'modalBusquedaEstudiante',
            buscarUrl: '/Evaluaciones/BuscarEstudiantes',
            debounceDelay: 300,
            minCharacters: 2,
            maxResults: 50,
            debug: false,
            ...options
        };

        // Estado interno
        this.debounceTimer = null;
        this.selectedRow = null;
        this.currentResults = [];
        this.isSearching = false;

        // Inicializar
        this.init();
    }

    /**
     * Inicializa el componente
     */
    init() {
        this.log('?? Inicializando Buscador de Estudiantes', this.config);
        
        try {
            this.setupElements();
            this.setupEventListeners();
            this.log('? Buscador inicializado correctamente');
        } catch (error) {
            this.error('? Error al inicializar el buscador:', error);
        }
    }

    /**
     * Configura las referencias a elementos DOM
     */
    setupElements() {
        // Elementos principales
        this.modal = document.getElementById(this.config.modalId);
        this.select = document.getElementById(this.config.selectId);
        
        if (!this.modal) {
            throw new Error(`Modal con ID '${this.config.modalId}' no encontrado`);
        }
        
        if (!this.select) {
            throw new Error(`Select con ID '${this.config.selectId}' no encontrado`);
        }

        // Elementos del modal (usando sufijo único basado en el modalId)
        const suffix = this.config.modalId.replace('modal', '');
        this.txtBusqueda = document.getElementById(`txtBusqueda${this.config.modalId}`);
        this.btnLimpiar = document.getElementById(`btnLimpiar${this.config.modalId}`);
        this.loading = document.getElementById(`loading${this.config.modalId}`);
        this.mensajeInicial = document.getElementById(`mensajeInicial${this.config.modalId}`);
        this.tablaContainer = document.getElementById(`tablaContainer${this.config.modalId}`);
        this.tbody = document.getElementById(`tbodyResultados${this.config.modalId}`);
        this.sinResultados = document.getElementById(`sinResultados${this.config.modalId}`);
        this.mensajeError = document.getElementById(`mensajeError${this.config.modalId}`);
        this.detalleError = document.getElementById(`detalleError${this.config.modalId}`);
        this.btnSeleccionar = document.getElementById(`btnSeleccionar${this.config.modalId}`);

        // Verificar elementos críticos
        const elementosCriticos = ['txtBusqueda', 'tbody'];
        elementosCriticos.forEach(elemento => {
            if (!this[elemento]) {
                throw new Error(`Elemento '${elemento}' no encontrado en el modal`);
            }
        });

        // Configurar instancia de Bootstrap Modal
        this.bootstrapModal = new bootstrap.Modal(this.modal);
    }

    /**
     * Configura los event listeners
     */
    setupEventListeners() {
        // Búsqueda con debounce
        this.txtBusqueda.addEventListener('input', (e) => {
            this.handleSearchInput(e.target.value);
        });

        // Limpiar búsqueda
        this.btnLimpiar?.addEventListener('click', () => {
            this.clearSearch();
        });

        // Selección en tabla
        this.tbody.addEventListener('click', (e) => {
            this.handleRowClick(e);
        });

        // Botón seleccionar
        this.btnSeleccionar?.addEventListener('click', () => {
            this.selectCurrentStudent();
        });

        // Teclas especiales
        this.txtBusqueda.addEventListener('keydown', (e) => {
            this.handleKeyDown(e);
        });

        // Eventos del modal
        this.modal.addEventListener('show.bs.modal', () => {
            this.onModalShow();
        });

        this.modal.addEventListener('hidden.bs.modal', () => {
            this.onModalHide();
        });
    }

    /**
     * Maneja la entrada de texto con debounce
     */
    handleSearchInput(texto) {
        clearTimeout(this.debounceTimer);
        
        const textoLimpio = texto.trim();
        
        if (textoLimpio.length < this.config.minCharacters) {
            this.showInitialState();
            return;
        }

        this.debounceTimer = setTimeout(() => {
            this.buscarEstudiantes(textoLimpio);
        }, this.config.debounceDelay);
    }

    /**
     * Realiza la búsqueda de estudiantes
     */
    async buscarEstudiantes(textoBusqueda) {
        if (this.isSearching) return;

        this.log(`?? Buscando estudiantes: "${textoBusqueda}"`);
        this.isSearching = true;
        
        try {
            this.showLoading();

            const response = await fetch(`${this.config.buscarUrl}?term=${encodeURIComponent(textoBusqueda)}`, {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json'
                }
            });

            if (!response.ok) {
                throw new Error(`Error HTTP: ${response.status} ${response.statusText}`);
            }

            const data = await response.json();
            
            if (data.error) {
                throw new Error(data.error);
            }

            // El controlador devuelve {results: [], pagination: {...}, total: 0}
            this.currentResults = Array.isArray(data.results) ? data.results : [];
            this.renderResults(this.currentResults);

            this.log(`? Encontrados ${this.currentResults.length} estudiantes`);

        } catch (error) {
            this.error('? Error en b�squeda:', error);
            this.showError(error.message || 'Error al realizar la b�squeda');
        } finally {
            this.isSearching = false;
        }
    }

    /**
     * Renderiza los resultados en la tabla
     */
    renderResults(estudiantes) {
        this.hideAllStates();

        if (!estudiantes || estudiantes.length === 0) {
            this.showNoResults();
            return;
        }

        // Generar filas de la tabla
        this.tbody.innerHTML = '';
        
        estudiantes.forEach((estudiante, index) => {
            const fila = this.createStudentRow(estudiante, index);
            this.tbody.appendChild(fila);
        });

        this.showResults();
    }

    /**
     * Crea una fila para un estudiante
     */
    createStudentRow(estudiante, index) {
        const fila = document.createElement('tr');
        fila.className = 'estudiante-row';
        fila.setAttribute('data-id', estudiante.id);
        fila.setAttribute('data-index', index);
        fila.setAttribute('data-nombre-completo', `${estudiante.apellidos}, ${estudiante.nombre}`);
        fila.setAttribute('data-texto-select', `${estudiante.apellidos}, ${estudiante.nombre} (${estudiante.numeroId})`);
        fila.title = 'Clic para seleccionar este estudiante';
        
        fila.innerHTML = `
            <td class="fw-bold text-primary">${this.escapeHtml(estudiante.id)}</td>
            <td>${this.escapeHtml(estudiante.apellidos)}</td>
            <td>${this.escapeHtml(estudiante.nombre)}</td>
            <td>
                <span class="badge bg-secondary">${this.escapeHtml(estudiante.numeroId)}</span>
            </td>
            <td>
                <small class="text-muted">${this.escapeHtml(estudiante.institucion || 'Sin especificar')}</small>
            </td>
        `;

        return fila;
    }

    /**
     * Maneja el clic en una fila
     */
    handleRowClick(event) {
        const fila = event.target.closest('tr.estudiante-row');
        if (!fila) return;

        // Remover selección previa
        this.tbody.querySelectorAll('tr').forEach(row => {
            row.classList.remove('selected');
        });

        // Seleccionar nueva fila
        fila.classList.add('selected');
        this.selectedRow = fila;

        // Mostrar botón seleccionar
        if (this.btnSeleccionar) {
            this.btnSeleccionar.style.display = 'inline-block';
        }

        // Doble clic para seleccionar automáticamente
        if (event.detail === 2) {
            this.selectCurrentStudent();
        }
    }

    /**
     * Maneja las teclas especiales
     */
    handleKeyDown(event) {
        switch (event.key) {
            case 'Enter':
                event.preventDefault();
                if (this.selectedRow) {
                    this.selectCurrentStudent();
                } else {
                    // Seleccionar el primer resultado si existe
                    const primerResultado = this.tbody.querySelector('tr.estudiante-row');
                    if (primerResultado) {
                        primerResultado.click();
                        setTimeout(() => this.selectCurrentStudent(), 100);
                    }
                }
                break;
            case 'Escape':
                this.bootstrapModal.hide();
                break;
            case 'ArrowDown':
            case 'ArrowUp':
                event.preventDefault();
                this.navigateResults(event.key === 'ArrowDown' ? 1 : -1);
                break;
        }
    }

    /**
     * Navega por los resultados con el teclado
     */
    navigateResults(direction) {
        const filas = this.tbody.querySelectorAll('tr.estudiante-row');
        if (filas.length === 0) return;

        let currentIndex = -1;
        if (this.selectedRow) {
            currentIndex = parseInt(this.selectedRow.getAttribute('data-index'));
        }

        const newIndex = Math.max(0, Math.min(filas.length - 1, currentIndex + direction));
        const nuevaFila = filas[newIndex];
        
        if (nuevaFila) {
            nuevaFila.click();
            nuevaFila.scrollIntoView({ behavior: 'smooth', block: 'nearest' });
        }
    }

    /**
     * Selecciona el estudiante actual
     */
    selectCurrentStudent() {
        if (!this.selectedRow) {
            this.showAlert('warning', 'Por favor seleccione un estudiante de la lista');
            return;
        }

        const estudianteId = this.selectedRow.getAttribute('data-id');
        const textoSelect = this.selectedRow.getAttribute('data-texto-select');
        const nombreCompleto = this.selectedRow.getAttribute('data-nombre-completo');

        if (!estudianteId) {
            this.error('? ID de estudiante no encontrado');
            return;
        }

        try {
            // Verificar si la opción ya existe en el select
            let option = this.select.querySelector(`option[value="${estudianteId}"]`);
            
            if (!option) {
                // Crear nueva opción si no existe
                option = document.createElement('option');
                option.value = estudianteId;
                option.textContent = textoSelect;
                this.select.appendChild(option);
                this.log(`? Nueva opci�n agregada al select: ${textoSelect}`);
            }

            // Seleccionar la opción
            this.select.value = estudianteId;

            // Disparar eventos
            this.select.dispatchEvent(new Event('change', { bubbles: true }));
            this.select.dispatchEvent(new CustomEvent('estudiante-seleccionado', { 
                detail: { 
                    id: estudianteId, 
                    texto: textoSelect,
                    nombre: nombreCompleto
                } 
            }));

            // Cerrar modal y mostrar confirmaci�n
            this.bootstrapModal.hide();
            this.showSuccess(`Estudiante seleccionado: ${nombreCompleto}`);

            this.log(`? Estudiante seleccionado: ${nombreCompleto} (ID: ${estudianteId})`);

        } catch (error) {
            this.error('? Error al seleccionar estudiante:', error);
            this.showAlert('error', 'Error al seleccionar el estudiante');
        }
    }

    /**
     * Limpia la b�squeda
     */
    clearSearch() {
        this.txtBusqueda.value = '';
        this.selectedRow = null;
        this.currentResults = [];
        clearTimeout(this.debounceTimer);
        
        if (this.btnSeleccionar) {
            this.btnSeleccionar.style.display = 'none';
        }
        
        this.showInitialState();
    }

    // =============== MÉTODOS DE ESTADOS VISUALES ===============

    showLoading() {
        this.hideAllStates();
        if (this.loading) this.loading.style.display = 'block';
    }

    showInitialState() {
        this.hideAllStates();
        if (this.mensajeInicial) this.mensajeInicial.style.display = 'block';
    }

    showResults() {
        this.hideAllStates();
        if (this.tablaContainer) this.tablaContainer.style.display = 'block';
    }

    showNoResults() {
        this.hideAllStates();
        if (this.sinResultados) this.sinResultados.style.display = 'block';
    }

    showError(mensaje) {
        this.hideAllStates();
        if (this.mensajeError) {
            this.mensajeError.style.display = 'block';
            if (this.detalleError) {
                this.detalleError.textContent = mensaje;
            }
        }
    }

    hideAllStates() {
        const elementos = [this.loading, this.mensajeInicial, this.tablaContainer, 
                          this.sinResultados, this.mensajeError];
        elementos.forEach(el => {
            if (el) el.style.display = 'none';
        });
    }

    // =============== EVENTOS DEL MODAL ===============

    onModalShow() {
        this.clearSearch();
        this.showInitialState();
        
        // Focus en el campo de b�squeda
        setTimeout(() => {
            if (this.txtBusqueda) {
                this.txtBusqueda.focus();
            }
        }, 300);

        this.log('?? Modal de b�squeda abierto');
    }

    onModalHide() {
        this.clearSearch();
        this.log('? Modal de b�squeda cerrado');
    }

    // =============== MÉTODOS DE UTILIDAD ===============

    /**
     * Obtiene el token antiforgery de ASP.NET Core
     */
    getAntiForgeryToken() {
        const token = document.querySelector('input[name="__RequestVerificationToken"]');
        return token ? token.value : '';
    }

    /**
     * Escapa HTML para prevenir XSS
     */
    escapeHtml(text) {
        if (!text) return '';
        const div = document.createElement('div');
        div.textContent = text;
        return div.innerHTML;
    }

    /**
     * Muestra notificaciones de �xito
     */
    showSuccess(mensaje) {
        this.showToast('success', mensaje);
    }

    /**
     * Muestra alertas
     */
    showAlert(tipo, mensaje) {
        this.showToast(tipo, mensaje);
    }

    /**
     * Sistema de notificaciones toast
     */
    showToast(tipo, mensaje) {
        // Crear elemento toast
        const toast = document.createElement('div');
        toast.className = `alert alert-${tipo === 'success' ? 'success' : tipo === 'warning' ? 'warning' : 'danger'} alert-dismissible fade show position-fixed`;
        toast.style.cssText = 'top: 20px; right: 20px; z-index: 9999; min-width: 300px; box-shadow: 0 4px 6px rgba(0,0,0,0.1);';
        
        const icon = tipo === 'success' ? 'check-circle' : tipo === 'warning' ? 'exclamation-triangle' : 'exclamation-circle';
        
        toast.innerHTML = `
            <i class="fas fa-${icon} me-2"></i>
            ${this.escapeHtml(mensaje)}
            <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
        `;
        
        document.body.appendChild(toast);
        
        // Auto-remover despu�s de 4 segundos
        setTimeout(() => {
            if (toast.parentNode) {
                toast.parentNode.removeChild(toast);
            }
        }, 4000);
    }

    /**
     * Logger condicional
     */
    log(...args) {
        if (this.config.debug) {
            console.log('[BuscadorEstudiante]', ...args);
        }
    }

    /**
     * Error logger
     */
    error(...args) {
        console.error('[BuscadorEstudiante]', ...args);
    }
}

// Hacer la clase disponible globalmente
window.BuscadorEstudiante = BuscadorEstudiante;

// Auto-inicialización para compatibilidad con implementaciones existentes
document.addEventListener('DOMContentLoaded', function() {
    // Buscar elementos que necesiten inicializaci�n autom�tica
    const selectores = document.querySelectorAll('[data-buscador-estudiante]');
    selectores.forEach(select => {
        const config = {
            selectId: select.id,
            modalId: select.getAttribute('data-modal-id') || 'modalBusquedaEstudiante',
            buscarUrl: select.getAttribute('data-buscar-url') || '/Evaluaciones/BuscarEstudiantes'
        };
        new BuscadorEstudiante(config);
    });
});

// Exportar para módulos ES6
if (typeof module !== 'undefined' && module.exports) {
    module.exports = BuscadorEstudiante;
}