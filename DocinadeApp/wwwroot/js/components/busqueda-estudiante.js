/**
 * Componente de Búsqueda Avanzada de Estudiantes
 * Autor: Sistema de Rúbricas
 * Versión: 1.0
 */

class BusquedaEstudiante {
    constructor(config) {
        this.config = {
            selectId: 'estudianteId',
            modalId: 'modalBusquedaEstudiante',
            controllerUrl: '/Evaluaciones/BuscarEstudiantes',
            debounceDelay: 300,
            minCharacters: 2,
            ...config
        };

        this.debounceTimer = null;
        this.selectedRowId = null;
        this.estudiantes = [];
        
        this.init();
    }

    init() {
        console.log('?? Inicializando componente de búsqueda de estudiantes:', this.config);
        
        this.setupElements();
        this.setupEventListeners();
        this.showInitialMessage();
    }

    setupElements() {
        // Elementos principales
        this.modal = document.getElementById(this.config.modalId);
        this.select = document.getElementById(this.config.selectId);
        
        // Elementos del modal (usando el ID único)
        const suffix = this.config.modalId.replace('modal', '');
        this.txtBusqueda = document.getElementById(`txtBusqueda${suffix}`);
        this.btnLimpiar = document.getElementById(`btnLimpiar${suffix}`);
        this.tablaResultados = document.getElementById(`tablaResultados${suffix}`);
        this.tbody = this.tablaResultados?.querySelector('tbody');
        this.loading = document.getElementById(`loading${suffix}`);
        this.mensajeBusqueda = document.getElementById(`mensajeBusqueda${suffix}`);
        this.mensajeSinResultados = document.getElementById(`mensajeSinResultados${suffix}`);
        this.mensajeError = document.getElementById(`mensajeError${suffix}`);
        this.btnSeleccionar = document.getElementById(`btnSeleccionar${suffix}`);

        // Verificar que todos los elementos existen
        const elementos = {
            modal: this.modal,
            select: this.select,
            txtBusqueda: this.txtBusqueda,
            tablaResultados: this.tablaResultados,
            tbody: this.tbody
        };

        for (const [nombre, elemento] of Object.entries(elementos)) {
            if (!elemento) {
                console.error(`? No se encontró el elemento: ${nombre}`);
            }
        }
    }

    setupEventListeners() {
        if (!this.txtBusqueda || !this.tbody) {
            console.error('? No se pueden configurar los event listeners: elementos faltantes');
            return;
        }

        // Búsqueda con debounce
        this.txtBusqueda.addEventListener('input', (e) => {
            this.handleSearchInput(e.target.value.trim());
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

        // Limpiar al abrir modal
        this.modal?.addEventListener('show.bs.modal', () => {
            this.clearSearch();
            this.showInitialMessage();
        });

        // Teclas en el campo de búsqueda
        this.txtBusqueda.addEventListener('keydown', (e) => {
            this.handleKeyDown(e);
        });
    }

    handleSearchInput(texto) {
        clearTimeout(this.debounceTimer);
        
        if (texto.length < this.config.minCharacters) {
            this.showInitialMessage();
            return;
        }

        this.debounceTimer = setTimeout(() => {
            this.buscarEstudiantes(texto);
        }, this.config.debounceDelay);
    }

    async buscarEstudiantes(textoBusqueda) {
        console.log(`?? Buscando estudiantes: "${textoBusqueda}"`);
        
        this.showLoading();
        
        try {
            const response = await fetch(this.config.controllerUrl, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value || ''
                },
                body: JSON.stringify({ textoBusqueda: textoBusqueda })
            });

            if (!response.ok) {
                throw new Error(`Error HTTP: ${response.status}`);
            }

            const data = await response.json();
            console.log(`? Recibidos ${data.length} estudiantes`);
            
            this.estudiantes = data;
            this.renderResultados(data);

        } catch (error) {
            console.error('? Error al buscar estudiantes:', error);
            this.showError('Error al buscar estudiantes. Intente nuevamente.');
        }
    }

    renderResultados(estudiantes) {
        this.hideAllMessages();
        
        if (!estudiantes || estudiantes.length === 0) {
            this.showNoResults();
            return;
        }

        this.tbody.innerHTML = '';
        
        estudiantes.forEach((estudiante, index) => {
            const fila = this.createRow(estudiante, index);
            this.tbody.appendChild(fila);
        });

        this.showTable();
    }

    createRow(estudiante, index) {
        const fila = document.createElement('tr');
        fila.className = 'estudiante-row';
        fila.setAttribute('data-id', estudiante.idEstudiante);
        fila.setAttribute('data-index', index);
        fila.style.cursor = 'pointer';
        fila.title = 'Clic para seleccionar este estudiante';
        
        fila.innerHTML = `
            <td class="fw-bold text-primary">${this.escapeHtml(estudiante.idEstudiante)}</td>
            <td>${this.escapeHtml(estudiante.apellidos)}</td>
            <td>${this.escapeHtml(estudiante.nombre)}</td>
            <td><span class="badge bg-secondary">${this.escapeHtml(estudiante.numeroId)}</span></td>
            <td><small class="text-muted">${this.escapeHtml(estudiante.institucion)}</small></td>
        `;

        return fila;
    }

    handleRowClick(e) {
        const fila = e.target.closest('tr.estudiante-row');
        if (!fila) return;

        // Remover selección anterior
        this.tbody.querySelectorAll('tr').forEach(row => {
            row.classList.remove('table-active', 'selected-row');
        });

        // Seleccionar nueva fila
        fila.classList.add('table-active', 'selected-row');
        this.selectedRowId = fila.getAttribute('data-id');
        
        // Mostrar botón seleccionar
        if (this.btnSeleccionar) {
            this.btnSeleccionar.style.display = 'inline-block';
        }

        // Doble clic para seleccionar automáticamente
        if (e.detail === 2) {
            this.selectCurrentStudent();
        }
    }

    handleKeyDown(e) {
        if (e.key === 'Enter') {
            e.preventDefault();
            const primerResultado = this.tbody.querySelector('tr.estudiante-row');
            if (primerResultado) {
                this.selectedRowId = primerResultado.getAttribute('data-id');
                this.selectCurrentStudent();
            }
        }
    }

    selectCurrentStudent() {
        if (!this.selectedRowId) {
            console.warn('?? No hay estudiante seleccionado');
            return;
        }

        const estudiante = this.estudiantes.find(e => e.idEstudiante.toString() === this.selectedRowId);
        if (!estudiante) {
            console.error('? No se encontró el estudiante seleccionado');
            return;
        }

        console.log('? Seleccionando estudiante:', estudiante);

        // Verificar si ya existe la opción en el select
        let option = this.select.querySelector(`option[value="${estudiante.idEstudiante}"]`);
        
        if (!option) {
            // Crear nueva opción si no existe
            option = document.createElement('option');
            option.value = estudiante.idEstudiante;
            option.textContent = `${estudiante.apellidos}, ${estudiante.nombre} (${estudiante.numeroId})`;
            this.select.appendChild(option);
        }

        // Seleccionar la opción
        this.select.value = estudiante.idEstudiante;

        // Disparar evento change
        this.select.dispatchEvent(new Event('change', { bubbles: true }));

        // Cerrar modal
        const modalInstance = bootstrap.Modal.getInstance(this.modal);
        if (modalInstance) {
            modalInstance.hide();
        }

        // Mensaje de confirmación
        this.showSuccessMessage(`Estudiante seleccionado: ${estudiante.apellidos}, ${estudiante.nombre}`);
    }

    clearSearch() {
        if (this.txtBusqueda) {
            this.txtBusqueda.value = '';
        }
        this.selectedRowId = null;
        this.estudiantes = [];
        
        if (this.tbody) {
            this.tbody.innerHTML = '';
        }
        
        if (this.btnSeleccionar) {
            this.btnSeleccionar.style.display = 'none';
        }
        
        clearTimeout(this.debounceTimer);
    }

    // Métodos de visualización
    showLoading() {
        this.hideAllMessages();
        if (this.loading) {
            this.loading.style.display = 'block';
        }
        if (this.tbody) {
            this.tbody.innerHTML = '<tr><td colspan="5" class="text-center text-muted">Buscando estudiantes...</td></tr>';
        }
    }

    showInitialMessage() {
        this.hideAllMessages();
        if (this.mensajeBusqueda) {
            this.mensajeBusqueda.style.display = 'block';
        }
        if (this.tbody) {
            this.tbody.innerHTML = '';
        }
    }

    showNoResults() {
        this.hideAllMessages();
        if (this.mensajeSinResultados) {
            this.mensajeSinResultados.style.display = 'block';
        }
        if (this.tbody) {
            this.tbody.innerHTML = '<tr><td colspan="5" class="text-center text-muted"><i class="fas fa-search me-2"></i>No se encontraron estudiantes</td></tr>';
        }
    }

    showError(mensaje) {
        this.hideAllMessages();
        if (this.mensajeError) {
            this.mensajeError.style.display = 'block';
            const errorSpan = this.mensajeError.querySelector('.error-message');
            if (errorSpan) {
                errorSpan.textContent = mensaje;
            }
        }
        if (this.tbody) {
            this.tbody.innerHTML = `<tr><td colspan="5" class="text-center text-danger"><i class="fas fa-exclamation-triangle me-2"></i>${this.escapeHtml(mensaje)}</td></tr>`;
        }
    }

    showTable() {
        this.hideAllMessages();
    }

    hideAllMessages() {
        [this.loading, this.mensajeBusqueda, this.mensajeSinResultados, this.mensajeError].forEach(element => {
            if (element) {
                element.style.display = 'none';
            }
        });
    }

    showSuccessMessage(mensaje) {
        // Crear una notificación temporal
        const alert = document.createElement('div');
        alert.className = 'alert alert-success alert-dismissible fade show position-fixed';
        alert.style.cssText = 'top: 20px; right: 20px; z-index: 9999; min-width: 300px;';
        alert.innerHTML = `
            <i class="fas fa-check-circle me-2"></i>
            ${this.escapeHtml(mensaje)}
            <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
        `;
        
        document.body.appendChild(alert);
        
        // Remover después de 3 segundos
        setTimeout(() => {
            if (alert.parentNode) {
                alert.parentNode.removeChild(alert);
            }
        }, 3000);
    }

    // Utilidades
    escapeHtml(text) {
        if (!text) return '';
        const div = document.createElement('div');
        div.textContent = text;
        return div.innerHTML;
    }
}

// Función global para inicializar el componente
function initBusquedaEstudiante(config) {
    return new BusquedaEstudiante(config);
}

// Hacer disponible globalmente
window.BusquedaEstudiante = BusquedaEstudiante;
window.initBusquedaEstudiante = initBusquedaEstudiante;