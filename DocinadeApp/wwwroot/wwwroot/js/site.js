// Site.js - Scripts generales del sitio
// Asegurar que jQuery esté cargado antes de continuar
(function() {
    'use strict';
    
    // Función para inicializar cuando jQuery esté disponible
    function initializeApp() {
        if (typeof $ === 'undefined' || typeof jQuery === 'undefined') {
            console.warn('jQuery no está disponible. Reintentando en 100ms...');
            setTimeout(initializeApp, 100);
            return;
        }
        
        console.log('jQuery disponible. Inicializando aplicación...');
        
        $(document).ready(function () {
            console.log('DOM listo. Inicializando funcionalidades...');
            
            // Inicializar toastr
            if (typeof toastr !== 'undefined') {
                toastr.options = {
                    "closeButton": true,
                    "debug": false,
                    "newestOnTop": true,
                    "progressBar": true,
                    "positionClass": "toast-top-right",
                    "preventDuplicates": true,
                    "onclick": null,
                    "showDuration": "300",
                    "hideDuration": "1000",
                    "timeOut": "5000",
                    "extendedTimeOut": "1000",
                    "showEasing": "swing",
                    "hideEasing": "linear",
                    "showMethod": "fadeIn",
                    "hideMethod": "fadeOut"
                };
                console.log('Toastr configurado correctamente');
            } else {
                console.warn('Toastr no está disponible');
            }
            
            // Inicializar dropdowns de Bootstrap
            if (typeof bootstrap !== 'undefined') {
                // Inicializar todos los dropdowns
                var dropdownElementList = [].slice.call(document.querySelectorAll('.dropdown-toggle'));
                var dropdownList = dropdownElementList.map(function (dropdownToggleEl) {
                    return new bootstrap.Dropdown(dropdownToggleEl);
                });

                // Inicializar tooltips si están disponibles
                if (bootstrap.Tooltip) {
                    var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
                    var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
                        return new bootstrap.Tooltip(tooltipTriggerEl);
                    });
                }
            } else {
                // Fallback para cuando Bootstrap no esté disponible
                $('.dropdown-toggle').on('click', function (e) {
                    e.preventDefault();
                    var dropdown = $(this).next('.dropdown-menu');

                    // Cerrar otros dropdowns
                    $('.dropdown-menu').not(dropdown).removeClass('show').slideUp();

                    // Toggle current dropdown
                    dropdown.toggleClass('show');
                    if (dropdown.hasClass('show')) {
                        dropdown.slideDown();
                    } else {
                        dropdown.slideUp();
                    }
                });

                // Cerrar dropdown al hacer click fuera
                $(document).on('click', function (e) {
                    if (!$(e.target).closest('.dropdown').length) {
                        $('.dropdown-menu').removeClass('show').slideUp();
                    }
                });
            }

            // Manejar alertas dismissible
            $('.alert .btn-close').on('click', function () {
                $(this).closest('.alert').fadeOut();
            });

            // Auto-hide alerts después de 5 segundos
            setTimeout(function () {
                $('.alert:not(.alert-permanent)').fadeOut();
            }, 5000);

            // VALIDACIÓN Y LOADING SPINNER PARA FORMULARIOS (CONSOLIDADO)
            var clickedButton = null;

            // Guardar textos originales al cargar la página
            $('form button[type="submit"]').each(function () {
                $(this).data('original-text', $(this).html());
            });

            // Capturar qué botón fue clickeado antes del submit
            $('form button[type="submit"]').on('click', function (e) {
                clickedButton = $(this);
                console.log('Botón clickeado:', $(this).attr('name'), $(this).attr('value'), $(this).data('action'));
            });

            // Manejar el submit del formulario (ÚNICO MANEJADOR)
            $('form').on('submit', function (e) {
                var form = this;
                console.log('Form submit iniciado');

                // Detectar qué botón disparó el submit
                var submitButton = clickedButton || $(document.activeElement);
                var action = $(submitButton).attr('value') || $(submitButton).data('action') || $(submitButton).attr('name') || '';

                console.log('Action detectada:', action);

                // Si es guardarBorrador o actualizar, omitir validación estricta
                var skipValidation = (action === 'guardarBorrador' || action === 'actualizar' || action.toLowerCase().includes('actualizar'));

                if (!skipValidation) {
                    var valid = true;

                    $(form).find('input[required], select[required], textarea[required]').each(function () {
                        var value = $(this).val();
                        if (!value || !String(value).trim()) {
                            $(this).addClass('is-invalid');
                            valid = false;
                        } else {
                            $(this).removeClass('is-invalid');
                        }
                    });

                    if (!valid) {
                        e.preventDefault();
                        alert('Por favor complete todos los campos requeridos.');
                        return false;
                    }
                }

                // LOADING SPINNER - Solo después de pasar validación
                if (!clickedButton) {
                    clickedButton = $(this).find('button[type="submit"]').first();
                }

                // Guardar el texto original del botón clickeado
                var originalText = clickedButton.data('original-text') || clickedButton.html();

                // Obtener el texto de loading personalizado según el botón
                var loadingText = getLoadingTextForButton(clickedButton);

                // Cambiar texto y desactivar solo el botón clickeado
                clickedButton.html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> ' + loadingText);
                clickedButton.prop('disabled', true);

                // Función para restaurar el botón
                function restoreButton() {
                    if (clickedButton) {
                        clickedButton.html(originalText);
                        clickedButton.prop('disabled', false);
                        clickedButton = null; // Reset para la próxima vez
                    }
                }

                // Restaurar después de 10 segundos como fallback
                setTimeout(restoreButton, 10000);

                console.log('Form submit permitido para:', action);
                return true;
            });

            // Limpiar validación al escribir
            $('input, select, textarea').on('input change', function () {
                $(this).removeClass('is-invalid');
            });

            // Animaciones para cards
            $('.card').hover(
                function () {
                    $(this).addClass('shadow-lg').css('transform', 'translateY(-5px)');
                },
                function () {
                    $(this).removeClass('shadow-lg').css('transform', 'translateY(0)');
                }
            );

            // Manejo del navbar collapse para móviles
            $('.navbar-toggler').on('click', function () {
                var target = $(this).data('bs-target') || $(this).attr('data-target');
                if (target) {
                    $(target).toggleClass('show');
                } else {
                    $('.navbar-collapse').toggleClass('show');
                }
            });

            console.log('Sistema de Rúbricas - Scripts cargados correctamente');
        });
    }
    
    // Inicializar cuando el DOM esté listo
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', initializeApp);
    } else {
        initializeApp();
    }
})();

// Función para determinar el texto de loading según el botón
function getLoadingTextForButton(button) {
    var action = button.data('action') || button.attr('value') || button.attr('name') || '';

    var loadingMessages = {
        'guardarCompleta': 'Completando...',
        'guardarBorrador': 'Guardando borrador...',
        'actualizar': 'Actualizando...',
        'eliminar': 'Eliminando...',
        'enviar': 'Enviando...',
        'crear': 'Creando...',
        'editar': 'Editando...',
        'buscar': 'Buscando...',
        'exportar': 'Exportando...',
        'importar': 'Importando...',
        'default': 'Procesando...'
    };

    // Buscar por coincidencia parcial si no hay coincidencia exacta
    for (var key in loadingMessages) {
        if (action.toLowerCase().includes(key.toLowerCase())) {
            return loadingMessages[key];
        }
    }

    return loadingMessages['default'];
}

// Funciones utilitarias
function showAlert(message, type = 'info') {
    var alertClass = 'alert-' + type;
    var alertHtml = '<div class="alert ' + alertClass + ' alert-dismissible fade show" role="alert">' +
        '<i class="fas fa-info-circle"></i> ' + message +
        '<button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>' +
        '</div>';

    $('.container-fluid').prepend(alertHtml);

    // Auto-hide después de 5 segundos
    setTimeout(function () {
        $('.alert').first().fadeOut();
    }, 5000);
}

function formatDate(dateString) {
    var date = new Date(dateString);
    return date.toLocaleDateString('es-ES', {
        year: 'numeric',
        month: '2-digit',
        day: '2-digit'
    });
}

function formatDateTime(dateString) {
    var date = new Date(dateString);
    return date.toLocaleDateString('es-ES', {
        year: 'numeric',
        month: '2-digit',
        day: '2-digit',
        hour: '2-digit',
        minute: '2-digit'
    });
}

// Función para validar formularios
function validateForm(formSelector) {
    var form = $(formSelector);
    var valid = true;

    form.find('input[required], select[required], textarea[required]').each(function () {
        if (!$(this).val().trim()) {
            $(this).addClass('is-invalid');
            valid = false;
        } else {
            $(this).removeClass('is-invalid');
        }
    });

    return valid;
}

// Función para mostrar loading
function showLoading(element) {
    $(element).html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Cargando...');
    $(element).prop('disabled', true);
}

function hideLoading(element, originalText) {
    $(element).html(originalText);
    $(element).prop('disabled', false);
}

// Función para restaurar manualmente todos los botones submit (utilidad global)
function restoreSubmitButtons() {
    $('form button[type="submit"]').each(function () {
        var $btn = $(this);
        if ($btn.prop('disabled')) {
            var originalText = $btn.data('original-text');

            if (originalText) {
                $btn.html(originalText);
                $btn.prop('disabled', false);
            }
        }
    });
}