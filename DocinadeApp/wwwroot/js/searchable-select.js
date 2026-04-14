/**
 * SearchableSelect - Componente reutilizable para select con búsqueda
 * Soporta Tom Select y Select2 con búsqueda local y remota
 * Compatible con ASP.NET Core MVC y validación cliente
 * 
 * @version 1.0.0
 * @author RubricasApp
 */

(function(window, document) {
    'use strict';

    // Verificar si ya está cargado
    if (window.SearchableSelect) {
        console.warn('SearchableSelect ya está cargado');
        return;
    }

    // Cache de instancias inicializadas
    const instances = new Map();
    
    // Configuraciones por defecto
    const defaults = {
        tomselect: {
            create: false,
            sortField: 'text',
            highlight: true,
            closeAfterSelect: true,
            loadThrottle: 250,
            placeholder: 'Buscar...',
            render: {
                no_results: function() {
                    return '<div class="no-results">No se encontraron resultados</div>';
                },
                loading: function() {
                    return '<div class="loading">Buscando...</div>';
                }
            }
        },
        select2: {
            theme: 'default',
            width: '100%',
            placeholder: 'Buscar...',
            allowClear: true,
            language: {
                noResults: function() {
                    return 'No se encontraron resultados';
                },
                searching: function() {
                    return 'Buscando...';
                },
                loadingMore: function() {
                    return 'Cargando más resultados...';
                }
            }
        }
    };

    /**
     * Inicializa un select con funcionalidad de búsqueda
     * @param {string|HTMLElement} selector - Selector CSS o elemento DOM
     * @param {Object} options - Opciones adicionales de configuración
     */
    function init(selector, options = {}) {
        const element = typeof selector === 'string' ? document.querySelector(selector) : selector;
        
        if (!element) {
            console.warn('SearchableSelect: Elemento no encontrado:', selector);
            return null;
        }

        // Evitar doble inicialización
        if (instances.has(element)) {
            console.warn('SearchableSelect: Elemento ya inicializado:', element);
            return instances.get(element);
        }

        try {
            const config = { ...getConfigFromElement(element), ...options };
            const instance = createInstance(element, config);
            
            if (instance) {
                instances.set(element, instance);
                element.classList.add('searchable-select-initialized');
                console.log('SearchableSelect: Inicializado correctamente:', element.id || element.name);
                return instance;
            }
        } catch (error) {
            console.error('SearchableSelect: Error al inicializar:', error);
            // Fallback: mantener select nativo
            element.classList.add('searchable-select-fallback');
        }
        
        return null;
    }

    /**
     * Obtiene la configuración desde los data attributes del elemento
     */
    function getConfigFromElement(element) {
        const dataset = element.dataset;
        
        return {
            library: dataset.searchableLibrary || 'tomselect',
            endpoint: dataset.searchableEndpoint || null,
            placeholder: dataset.searchablePlaceholder || 'Buscar...',
            allowClear: dataset.searchableAllowClear === 'true',
            minimumInputLength: parseInt(dataset.searchableMinimumInput) || 0,
            maxOptions: dataset.searchableMaxOptions ? parseInt(dataset.searchableMaxOptions) : null,
            debounce: parseInt(dataset.searchableDebounce) || 250,
            extra: dataset.searchableExtra ? JSON.parse(dataset.searchableExtra) : {}
        };
    }

    /**
     * Crea una instancia según la librería especificada
     */
    function createInstance(element, config) {
        if (config.library === 'select2' && window.jQuery && window.jQuery.fn.select2) {
            return createSelect2Instance(element, config);
        } else if (window.TomSelect) {
            return createTomSelectInstance(element, config);
        } else {
            console.warn('SearchableSelect: Librerías no disponibles, usando select nativo');
            return null;
        }
    }

    /**
     * Crea instancia de Tom Select
     */
    function createTomSelectInstance(element, config) {
        const options = {
            ...defaults.tomselect,
            placeholder: config.placeholder,
            loadThrottle: config.debounce
        };

        // Configurar según tipo de búsqueda
        if (config.endpoint) {
            // Búsqueda remota
            options.load = function(query, callback) {
                if (query.length < config.minimumInputLength) {
                    callback();
                    return;
                }

                element.classList.add('searchable-select-loading');
                
                const url = new URL(config.endpoint, window.location.origin);
                url.searchParams.set('q', query);
                
                // Agregar parámetros extra
                Object.entries(config.extra).forEach(([key, value]) => {
                    url.searchParams.set(key, value);
                });

                fetch(url)
                    .then(response => response.json())
                    .then(data => {
                        element.classList.remove('searchable-select-loading');
                        
                        // Formatear datos para Tom Select
                        const items = Array.isArray(data) ? data : data.items || [];
                        const formatted = items.map(item => ({
                            value: item.id || item.value,
                            text: item.text || item.name || item.title
                        }));
                        
                        callback(formatted);
                    })
                    .catch(error => {
                        element.classList.remove('searchable-select-loading');
                        console.error('SearchableSelect: Error en búsqueda remota:', error);
                        callback();
                    });
            };
        } else {
            // Búsqueda local
            if (config.minimumInputLength > 0) {
                options.searchField = ['text'];
                options.score = function(search) {
                    return function(item) {
                        return search.length >= config.minimumInputLength ? 
                            this.getScoreFunction(search)(item) : 0;
                    };
                };
            }
        }

        // Configuraciones adicionales
        if (config.maxOptions) {
            options.maxOptions = config.maxOptions;
        }

        if (!config.allowClear) {
            options.allowEmptyOption = false;
        }

        // Precargar valor seleccionado si es remoto
        if (config.endpoint && element.value) {
            preloadSelectedOption(element, config);
        }

        return new TomSelect(element, options);
    }

    /**
     * Crea instancia de Select2
     */
    function createSelect2Instance(element, config) {
        const $ = window.jQuery;
        const options = {
            ...defaults.select2,
            placeholder: config.placeholder,
            allowClear: config.allowClear,
            minimumInputLength: config.minimumInputLength
        };

        // Configurar según tipo de búsqueda
        if (config.endpoint) {
            // Búsqueda remota
            options.ajax = {
                url: config.endpoint,
                delay: config.debounce,
                data: function(params) {
                    const query = {
                        q: params.term,
                        page: params.page || 1
                    };
                    
                    // Agregar parámetros extra
                    return { ...query, ...config.extra };
                },
                processResults: function(data) {
                    // Formatear datos para Select2
                    const items = Array.isArray(data) ? data : data.items || [];
                    const results = items.map(item => ({
                        id: item.id || item.value,
                        text: item.text || item.name || item.title
                    }));
                    
                    return { results };
                },
                cache: true
            };

            // Precargar valor seleccionado
            if (element.value) {
                preloadSelectedOption(element, config);
            }
        }

        return $(element).select2(options).data('select2');
    }

    /**
     * Precarga la opción seleccionada para búsqueda remota
     */
    function preloadSelectedOption(element, config) {
        if (!element.value || !config.endpoint) return;

        const url = new URL(config.endpoint, window.location.origin);
        url.searchParams.set('id', element.value);
        
        fetch(url)
            .then(response => response.json())
            .then(data => {
                const item = Array.isArray(data) ? data[0] : data;
                if (item) {
                    const option = element.querySelector(`option[value="${element.value}"]`);
                    if (option) {
                        option.textContent = item.text || item.name || item.title;
                    }
                }
            })
            .catch(error => {
                console.warn('SearchableSelect: Error precargando opción seleccionada:', error);
            });
    }

    /**
     * Destruye una instancia
     */
    function destroy(selector) {
        const element = typeof selector === 'string' ? document.querySelector(selector) : selector;
        
        if (!element || !instances.has(element)) {
            return false;
        }

        const instance = instances.get(element);
        
        try {
            if (instance && typeof instance.destroy === 'function') {
                instance.destroy();
            } else if (window.jQuery && element.classList.contains('select2-hidden-accessible')) {
                window.jQuery(element).select2('destroy');
            }
            
            instances.delete(element);
            element.classList.remove('searchable-select-initialized', 'searchable-select-fallback');
            
            return true;
        } catch (error) {
            console.error('SearchableSelect: Error al destruir instancia:', error);
            return false;
        }
    }

    /**
     * Inicializa todos los selects con clase searchable-select
     */
    function initAll() {
        const elements = document.querySelectorAll('select.searchable-select:not(.searchable-select-initialized)');
        elements.forEach(element => init(element));
    }

    /**
     * Actualiza las opciones de un select (útil para dependencias)
     */
    function updateOptions(selector, newOptions) {
        const element = typeof selector === 'string' ? document.querySelector(selector) : selector;
        
        if (!element || !instances.has(element)) {
            console.warn('SearchableSelect: Elemento no inicializado:', selector);
            return false;
        }

        const instance = instances.get(element);
        const config = getConfigFromElement(element);

        try {
            if (config.library === 'select2' && window.jQuery) {
                const $element = window.jQuery(element);
                $element.empty();
                
                newOptions.forEach(option => {
                    const newOption = new Option(option.text, option.value, false, option.selected);
                    $element.append(newOption);
                });
                
                $element.trigger('change');
            } else if (window.TomSelect) {
                instance.clearOptions();
                instance.addOptions(newOptions);
                if (newOptions.some(opt => opt.selected)) {
                    const selectedValues = newOptions.filter(opt => opt.selected).map(opt => opt.value);
                    instance.setValue(selectedValues);
                }
            }
            
            return true;
        } catch (error) {
            console.error('SearchableSelect: Error actualizando opciones:', error);
            return false;
        }
    }

    /**
     * Obtiene el valor actual del select
     */
    function getValue(selector) {
        const element = typeof selector === 'string' ? document.querySelector(selector) : selector;
        
        if (!element) {
            return null;
        }

        if (instances.has(element)) {
            const instance = instances.get(element);
            const config = getConfigFromElement(element);
            
            if (config.library === 'select2' && window.jQuery) {
                return window.jQuery(element).val();
            } else if (window.TomSelect) {
                return instance.getValue();
            }
        }
        
        return element.value;
    }

    /**
     * Establece el valor del select
     */
    function setValue(selector, value) {
        const element = typeof selector === 'string' ? document.querySelector(selector) : selector;
        
        if (!element) {
            return false;
        }

        if (instances.has(element)) {
            const instance = instances.get(element);
            const config = getConfigFromElement(element);
            
            try {
                if (config.library === 'select2' && window.jQuery) {
                    window.jQuery(element).val(value).trigger('change');
                } else if (window.TomSelect) {
                    instance.setValue(value);
                }
                return true;
            } catch (error) {
                console.error('SearchableSelect: Error estableciendo valor:', error);
            }
        }
        
        element.value = value;
        return true;
    }

    // Auto-inicialización cuando el DOM esté listo
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', initAll);
    } else {
        // Si el DOM ya está listo, ejecutar inmediatamente
        setTimeout(initAll, 0);
    }

    // Exponer API pública
    window.SearchableSelect = {
        init,
        destroy,
        initAll,
        updateOptions,
        getValue,
        setValue,
        instances
    };

    // Para compatibilidad con módulos
    if (typeof module !== 'undefined' && module.exports) {
        module.exports = window.SearchableSelect;
    }

})(window, document);
