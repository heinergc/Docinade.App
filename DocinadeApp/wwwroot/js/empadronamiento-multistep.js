/**
 * JavaScript para el formulario multistep de empadronamiento
 * Maneja la navegación, validación y estado del formulario
 */

class EmpadronamientoMultistep {
    constructor() {
        this.currentStep = 1;
        this.totalSteps = 5;
        this.formData = {};
        this.validation = {};
        
        this.init();
    }

    init() {
        this.bindEvents();
        this.updateProgress();
        this.updateButtons(); // Solo actualizar botones, NO validar
        this.setupDatePickers();
        this.setupPhoneMasks();
        this.initializeFormValidation();
    }

    bindEvents() {
        // Navegación por pasos
        document.querySelectorAll('.step-item').forEach(step => {
            step.addEventListener('click', (e) => {
                const stepNumber = parseInt(e.currentTarget.dataset.step);
                if (this.canNavigateToStep(stepNumber)) {
                    this.goToStep(stepNumber);
                }
            });
        });

        // Botones de navegación
        document.getElementById('btnAnterior')?.addEventListener('click', () => {
            this.previousStep();
        });

        document.getElementById('btnSiguiente')?.addEventListener('click', () => {
            this.nextStep();
        });

        document.getElementById('btnGuardarBorrador')?.addEventListener('click', () => {
            this.saveDraft();
        });

        // Botón Finalizar (en la vista Edit tiene id="btnFinalizar")
        const btnFinalizar = document.getElementById('btnFinalizar');
        if (btnFinalizar) {
            btnFinalizar.addEventListener('click', (e) => {
                e.preventDefault();
                this.saveForm();
            });
        }
        
        // Botón Guardar (por compatibilidad con otras vistas)
        const btnGuardar = document.getElementById('btnGuardar');
        if (btnGuardar) {
            btnGuardar.addEventListener('click', (e) => {
                e.preventDefault();
                this.saveForm();
            });
        }

        // Limpiar errores cuando el usuario escribe
        document.querySelectorAll('input, select, textarea').forEach(field => {
            field.addEventListener('input', () => {
                this.clearFieldError(field);
            });

            field.addEventListener('change', () => {
                this.clearFieldError(field);
            });
        });

        // Prevenir envío accidental del formulario
        document.getElementById('formEmpadronamiento')?.addEventListener('submit', (e) => {
            e.preventDefault();
            this.saveForm();
        });
    }

    goToStep(stepNumber) {
        if (stepNumber < 1 || stepNumber > this.totalSteps) {
            return;
        }

        // Ocultar todas las secciones
        document.querySelectorAll('.form-section').forEach(section => {
            section.classList.add('d-none');
        });

        // Mostrar la sección actual (intentar ambos formatos de ID)
        let currentSection = document.getElementById(`section-${stepNumber}`);
        if (!currentSection) {
            currentSection = document.getElementById(`seccion${stepNumber}`);
        }
        if (currentSection) {
            currentSection.classList.remove('d-none');
        }

        // Actualizar navegación
        this.updateStepNavigation(stepNumber);
        this.currentStep = stepNumber;
        this.updateProgress();
        this.updateButtons();
        this.updateSectionTitle(stepNumber);
        
        // Scroll al inicio del formulario
        document.querySelector('.form-section:not(.d-none)')?.scrollIntoView({ 
            behavior: 'smooth' 
        });
    }

    updateStepNavigation(activeStep) {
        document.querySelectorAll('.step-item').forEach((step, index) => {
            if (!step) return; // Verificar que el elemento existe
            
            const stepNumber = index + 1;
            step.classList.remove('step-active');
            
            if (stepNumber === activeStep) {
                step.classList.add('step-active');
            }
            
            // Marcar pasos completados
            if (this.isStepValid(stepNumber) && stepNumber < activeStep) {
                step.classList.add('step-valid');
            } else {
                step.classList.remove('step-valid');
            }
        });
    }

    updateSectionTitle(stepNumber) {
        const titleElement = document.getElementById('sectionTitle');
        if (!titleElement) return;

        const titles = {
            1: '<i class="mdi mdi-account-outline me-2"></i>Datos Personales',
            2: '<i class="mdi mdi-map-marker me-2"></i>Contacto',
            3: '<i class="mdi mdi-account-group-outline me-2"></i>Responsables',
            4: '<i class="mdi mdi-medical-bag me-2"></i>Salud',
            5: '<i class="mdi mdi-school me-2"></i>Académico'
        };

        if (titles[stepNumber]) {
            titleElement.innerHTML = titles[stepNumber];
        }
    }

    updateProgress() {
        const progress = (this.currentStep / this.totalSteps) * 100;
        const progressBar = document.querySelector('.progress-bar');
        if (progressBar) {
            progressBar.style.width = `${progress}%`;
            progressBar.textContent = `${Math.round(progress)}%`;
        }

        // Actualizar contador
        const counter = document.getElementById('currentStep');
        if (counter) {
            counter.textContent = this.currentStep;
        }
    }

    updateButtons() {
        const btnAnterior = document.getElementById('btnAnterior');
        const btnSiguiente = document.getElementById('btnSiguiente');
        const btnFinalizar = document.getElementById('btnFinalizar');
        const btnGuardar = document.getElementById('btnGuardar'); // Por compatibilidad

        // El botón anterior se muestra solo si no estamos en el primer paso
        if (btnAnterior) {
            btnAnterior.style.display = this.currentStep === 1 ? 'none' : 'inline-block';
            btnAnterior.disabled = false; // Siempre habilitado
        }

        // El botón siguiente siempre está habilitado (validación al hacer clic)
        if (btnSiguiente) {
            btnSiguiente.style.display = this.currentStep === this.totalSteps ? 'none' : 'inline-block';
            btnSiguiente.disabled = false; // Siempre habilitado
        }

        // El botón finalizar se muestra solo en el último paso
        if (btnFinalizar) {
            btnFinalizar.style.display = this.currentStep === this.totalSteps ? 'inline-block' : 'none';
            btnFinalizar.classList.remove('d-none'); // Remover clase d-none si existe
        }
        
        // El botón guardar (por compatibilidad) se muestra solo en el último paso
        if (btnGuardar) {
            btnGuardar.style.display = this.currentStep === this.totalSteps ? 'inline-block' : 'none';
        }
    }

    nextStep() {
        // Validar el paso actual antes de avanzar
        if (!this.validateCurrentStep()) {
            // Mostrar notificación de error
            this.showNotification('error', 'Por favor complete todos los campos requeridos antes de continuar.');
            
            // Scroll al primer campo con error
            const firstError = document.querySelector('.form-section:not(.d-none) .is-invalid');
            if (firstError) {
                firstError.scrollIntoView({ behavior: 'smooth', block: 'center' });
                firstError.focus();
            }
            return;
        }

        // Si la validación pasó, avanzar al siguiente paso
        if (this.currentStep < this.totalSteps) {
            this.goToStep(this.currentStep + 1);
        }
    }

    previousStep() {
        if (this.currentStep > 1) {
            this.goToStep(this.currentStep - 1);
        }
    }

    canNavigateToStep(stepNumber) {
        // Permitir navegación libre entre todos los pasos
        // La validación solo se hace al intentar avanzar con el botón "Siguiente"
        return true;
    }

    validateCurrentStep() {
        return this.isStepValid(this.currentStep);
    }

    isStepValid(stepNumber) {
        let section = document.getElementById(`section-${stepNumber}`);
        if (!section) {
            section = document.getElementById(`seccion${stepNumber}`);
        }
        if (!section) return false;

        let isValid = true;

        // Validaciones específicas por sección (sin validar campos required del HTML)
        switch (stepNumber) {
            case 1:
                isValid = this.validateDatosPersonales();
                break;
            case 2:
                isValid = this.validateContactoResidencia();
                break;
            case 3:
                isValid = this.validateResponsables();
                break;
            case 4:
                isValid = this.validateSalud();
                break;
            case 5:
                isValid = this.validateAcademico();
                break;
        }

        return isValid;
    }

    validateField(field) {
        if (!field) return true;

        const value = field.value.trim();
        let isValid = true;
        let errorMessage = '';

        // Validar campos requeridos
        if (field.hasAttribute('required') && !value) {
            isValid = false;
            errorMessage = 'Este campo es requerido';
        }

        // Validaciones específicas por tipo
        if (isValid && value) {
            switch (field.type) {
                case 'email':
                    if (!this.isValidEmail(value)) {
                        isValid = false;
                        errorMessage = 'Ingrese un email válido';
                    }
                    break;
                case 'tel':
                    if (!this.isValidPhone(value)) {
                        isValid = false;
                        errorMessage = 'Ingrese un teléfono válido';
                    }
                    break;
                case 'date':
                    if (!this.isValidDate(value)) {
                        isValid = false;
                        errorMessage = 'Ingrese una fecha válida';
                    }
                    break;
            }

            // Validaciones personalizadas
            if (field.classList.contains('cedula')) {
                if (!this.isValidCedula(value)) {
                    isValid = false;
                    errorMessage = 'Ingrese una cédula válida';
                }
            }
        }

        // Mostrar/ocultar error
        this.showFieldError(field, isValid ? '' : errorMessage);
        
        return isValid;
    }

    showFieldError(field, message) {
        if (!field) return; // Verificar que el campo existe
        
        field.classList.toggle('is-invalid', !!message);
        field.classList.toggle('is-valid', !message && field.value.trim());

        // Buscar o crear elemento de error
        let errorElement = field.parentElement.querySelector('.invalid-feedback');
        if (!errorElement) {
            errorElement = document.createElement('div');
            errorElement.className = 'invalid-feedback';
            field.parentElement.appendChild(errorElement);
        }

        errorElement.textContent = message;
        errorElement.style.display = message ? 'block' : 'none';
    }

    clearFieldError(field) {
        if (!field) return; // Verificar que el campo existe
        
        field.classList.remove('is-invalid');
        const errorElement = field.parentElement.querySelector('.invalid-feedback');
        if (errorElement) {
            errorElement.style.display = 'none';
        }
    }

    // Validaciones específicas por sección
    validateDatosPersonales() {
        let isValid = true;

        // Validar campos requeridos de la sección 1
        const numeroIdField = document.getElementById('DatosEmpadronamiento_NumeroId');
        const fechaNacimientoField = document.getElementById('DatosEmpadronamiento_FechaNacimiento');
        const generoField = document.getElementById('DatosEmpadronamiento_Genero');
        const nacionalidadField = document.getElementById('DatosEmpadronamiento_Nacionalidad');

        // Validar Número ID
        if (numeroIdField) {
            const numeroId = numeroIdField.value?.trim();
            if (!numeroId) {
                this.showFieldError(numeroIdField, 'El número de ID es requerido');
                isValid = false;
            } else {
                this.clearFieldError(numeroIdField);
            }
        }

        // Validar Fecha de Nacimiento
        if (fechaNacimientoField) {
            const fechaNacimiento = fechaNacimientoField.value?.trim();
            if (!fechaNacimiento) {
                this.showFieldError(fechaNacimientoField, 'La fecha de nacimiento es requerida');
                isValid = false;
            } else {
                this.clearFieldError(fechaNacimientoField);
            }
        }

        // Validar Género
        if (generoField) {
            const genero = generoField.value?.trim();
            if (!genero) {
                this.showFieldError(generoField, 'El género es requerido');
                isValid = false;
            } else {
                this.clearFieldError(generoField);
            }
        }

        // Validar Nacionalidad
        if (nacionalidadField) {
            const nacionalidad = nacionalidadField.value?.trim();
            if (!nacionalidad) {
                this.showFieldError(nacionalidadField, 'La nacionalidad es requerida');
                isValid = false;
            } else {
                this.clearFieldError(nacionalidadField);
            }
        }

        return isValid;
    }

    validateContactoResidencia() {
        // Para la sección de contacto, no hay campos obligatorios específicos
        // Los teléfonos y correos son opcionales en esta versión
        return true;
    }

    validateResponsables() {
        let isValid = true;

        // Buscar campos por múltiples métodos para asegurar compatibilidad
        const getFieldValue = (fieldId) => {
            let element = document.getElementById(fieldId);
            if (!element) {
                // Intentar con punto en lugar de guión bajo
                const altId = fieldId.replace(/_/g, '.');
                element = document.getElementById(altId);
            }
            if (!element) {
                // Intentar buscar por name
                element = document.querySelector(`[name="${fieldId}"]`);
            }
            if (!element) {
                const altName = fieldId.replace(/_/g, '.');
                element = document.querySelector(`[name="${altName}"]`);
            }
            return element ? element.value.trim() : null;
        };

        const getFieldElement = (fieldId) => {
            let element = document.getElementById(fieldId);
            if (!element) {
                const altId = fieldId.replace(/_/g, '.');
                element = document.getElementById(altId);
            }
            if (!element) {
                element = document.querySelector(`[name="${fieldId}"]`);
            }
            if (!element) {
                const altName = fieldId.replace(/_/g, '.');
                element = document.querySelector(`[name="${altName}"]`);
            }
            return element;
        };

        // Validar campos obligatorios de emergencia
        const contactoEmergencia = getFieldValue('DatosEmpadronamiento_ContactoEmergencia');
        const telefonoEmergencia = getFieldValue('DatosEmpadronamiento_TelefonoEmergencia');
        const relacionEmergencia = getFieldValue('DatosEmpadronamiento_RelacionEmergencia');

        // Validar contacto de emergencia
        const contactoElement = getFieldElement('DatosEmpadronamiento_ContactoEmergencia');
        if (!contactoEmergencia) {
            this.showFieldError(contactoElement, 'El contacto de emergencia es requerido');
            isValid = false;
        } else {
            this.clearFieldError(contactoElement);
        }

        // Validar teléfono de emergencia
        const telefonoElement = getFieldElement('DatosEmpadronamiento_TelefonoEmergencia');
        if (!telefonoEmergencia) {
            this.showFieldError(telefonoElement, 'El teléfono de emergencia es requerido');
            isValid = false;
        } else {
            this.clearFieldError(telefonoElement);
        }

        // Validar relación de emergencia
        const relacionElement = getFieldElement('DatosEmpadronamiento_RelacionEmergencia');
        if (!relacionEmergencia || relacionEmergencia === '') {
            this.showFieldError(relacionElement, 'La relación de emergencia es requerida');
            isValid = false;
        } else {
            this.clearFieldError(relacionElement);
        }

        return isValid;
    }

    validateSalud() {
        return true; // Los campos de salud son opcionales
    }

    validateAcademico() {
        return true; // Los campos académicos son opcionales en esta etapa
    }

    // Utilidades de validación
    isValidEmail(email) {
        const regex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        return regex.test(email);
    }

    isValidPhone(phone) {
        const cleaned = phone.replace(/\D/g, '');
        return cleaned.length >= 8;
    }

    isValidDate(date) {
        const dateObj = new Date(date);
        return dateObj instanceof Date && !isNaN(dateObj);
    }

    isValidCedula(cedula) {
        // Validación básica de cédula costarricense
        const cleaned = cedula.replace(/\D/g, '');
        return cleaned.length >= 9;
    }

    // Configuración de componentes
    setupDatePickers() {
        document.querySelectorAll('input[type="date"]').forEach(input => {
            // Configurar límites de fecha si es necesario
            if (input.classList.contains('fecha-nacimiento')) {
                const today = new Date();
                const maxDate = new Date(today.getFullYear() - 5, today.getMonth(), today.getDate());
                input.max = maxDate.toISOString().split('T')[0];
            }
        });
    }

    setupPhoneMasks() {
        document.querySelectorAll('input[type="tel"]').forEach(input => {
            input.addEventListener('input', (e) => {
                let value = e.target.value.replace(/\D/g, '');
                if (value.length > 0) {
                    value = value.replace(/(\d{4})(\d{4})/, '$1-$2');
                }
                e.target.value = value;
            });
        });
    }

    initializeFormValidation() {
        // Configurar validación de Bootstrap si está disponible
        if (typeof bootstrap !== 'undefined') {
            const forms = document.querySelectorAll('.needs-validation');
            forms.forEach(form => {
                form.addEventListener('submit', (event) => {
                    if (!form.checkValidity()) {
                        event.preventDefault();
                        event.stopPropagation();
                    }
                    form.classList.add('was-validated');
                });
            });
        }
    }

    // Guardar borrador
    async saveDraft() {
        const form = document.getElementById('formEmpadronamiento');
        if (!form) return;

        const formData = new FormData(form);
        const submitButton = document.getElementById('btnGuardarBorrador');

        try {
            this.setButtonLoading(submitButton, true);
            
            // Guardar en localStorage como respaldo
            this.saveFormData();

            // Intentar guardar en el servidor (opcional para borradores)
            // Aquí podrías implementar un endpoint específico para borradores
            
            this.showNotification('success', 'Borrador guardado exitosamente');
        } catch (error) {
            console.error('Error al guardar borrador:', error);
            this.showNotification('warning', 'Borrador guardado localmente');
        } finally {
            this.setButtonLoading(submitButton, false);
        }
    }

    // Guardar formulario
    async saveForm() {
        if (!this.validateAllSteps()) {
            this.showNotification('error', 'Por favor complete todos los campos requeridos');
            return;
        }

        const form = document.getElementById('formEmpadronamiento');
        if (!form) return;

        // Buscar el botón de submit (puede ser btnFinalizar o btnGuardar)
        const submitButton = document.getElementById('btnFinalizar') || document.getElementById('btnGuardar');

        try {
            if (submitButton) {
                this.setButtonLoading(submitButton, true);
            }

            // Para el formulario de edición, simplemente enviarlo normalmente
            // ya que el controlador hace redirect con TempData
            this.showNotification('success', 'Guardando cambios...');
            
            // Enviar el formulario de manera tradicional (no AJAX)
            form.submit();

        } catch (error) {
            console.error('Error al guardar:', error);
            this.showNotification('error', 'Error al guardar el formulario. Por favor, intente nuevamente.');
            if (submitButton) {
                this.setButtonLoading(submitButton, false);
            }
        }
    }

    validateAllSteps() {
        for (let i = 1; i <= this.totalSteps; i++) {
            if (!this.isStepValid(i)) {
                this.goToStep(i);
                return false;
            }
        }
        return true;
    }

    setButtonLoading(button, loading) {
        if (!button) return;

        if (loading) {
            button.classList.add('loading');
            button.disabled = true;
        } else {
            button.classList.remove('loading');
            button.disabled = false;
        }
    }

    showNotification(type, message) {
        // Crear notificación toast
        const toastHtml = `
            <div class="toast align-items-center text-white bg-${type === 'success' ? 'success' : type === 'error' ? 'danger' : 'warning'} border-0" role="alert">
                <div class="d-flex">
                    <div class="toast-body">
                        ${message}
                    </div>
                    <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast"></button>
                </div>
            </div>
        `;

        // Buscar o crear contenedor de toasts
        let toastContainer = document.getElementById('toast-container');
        if (!toastContainer) {
            toastContainer = document.createElement('div');
            toastContainer.id = 'toast-container';
            toastContainer.className = 'toast-container position-fixed top-0 end-0 p-3';
            toastContainer.style.zIndex = '9999';
            document.body.appendChild(toastContainer);
        }

        // Agregar toast
        toastContainer.insertAdjacentHTML('beforeend', toastHtml);
        const toastElement = toastContainer.lastElementChild;

        // Mostrar toast
        if (typeof bootstrap !== 'undefined') {
            const toast = new bootstrap.Toast(toastElement);
            toast.show();
        } else {
            // Fallback sin Bootstrap
            toastElement.style.display = 'block';
            setTimeout(() => {
                toastElement.remove();
            }, 3000);
        }
    }

    // Autoguardado (opcional)
    enableAutoSave() {
        setInterval(() => {
            this.saveFormData();
        }, 30000); // Cada 30 segundos
    }

    saveFormData() {
        const form = document.getElementById('formEmpadronamiento');
        if (!form) return;

        const formData = new FormData(form);
        const data = {};
        
        for (let [key, value] of formData.entries()) {
            data[key] = value;
        }

        localStorage.setItem('empadronamiento_draft', JSON.stringify(data));
    }

    getEstudianteId() {
        const hiddenInput = document.querySelector('input[name="IdEstudiante"]');
        return hiddenInput ? hiddenInput.value : null;
    }

    loadFormData() {
        const saved = localStorage.getItem('empadronamiento_draft');
        if (saved) {
            const data = JSON.parse(saved);
            Object.keys(data).forEach(key => {
                const field = document.querySelector(`[name="${key}"]`);
                if (field) {
                    field.value = data[key];
                }
            });
        }
    }
}

// Inicializar cuando el DOM esté listo
document.addEventListener('DOMContentLoaded', () => {
    // Solo inicializar en páginas de empadronamiento
    if (document.getElementById('formEmpadronamiento')) {
        window.empadronamientoMultistep = new EmpadronamientoMultistep();
    }
});

// Prevenir pérdida de datos al salir
window.addEventListener('beforeunload', (e) => {
    if (document.getElementById('formEmpadronamiento')) {
        const form = document.getElementById('formEmpadronamiento');
        const formData = new FormData(form);
        let hasData = false;
        
        for (let [key, value] of formData.entries()) {
            if (value.trim()) {
                hasData = true;
                break;
            }
        }
        
        if (hasData) {
            e.preventDefault();
            e.returnValue = '';
        }
    }
});

// Inicializar la clase cuando el DOM esté cargado
document.addEventListener('DOMContentLoaded', () => {
    if (document.getElementById('formEmpadronamiento')) {
        const empadronamiento = new EmpadronamientoMultistep();
    }
});