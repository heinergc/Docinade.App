// Funcionalidades modernas para la aplicación
document.addEventListener('DOMContentLoaded', function() {
    
    // Inicializar efectos de animación
    initializeAnimations();
    
    // Configurar funcionalidades de formularios
    setupFormEnhancements();
    
    // Configurar efectos hover mejorados
    setupHoverEffects();
    
    // Configurar funcionalidades de filtros
    setupFilterFunctionality();
});

// Animaciones de entrada
function initializeAnimations() {
    // Agregar clase fade-in a elementos que la necesiten
    const animatedElements = document.querySelectorAll('.modern-card, .page-header, .table-container, .filters-container');
    
    animatedElements.forEach((element, index) => {
        element.style.opacity = '0';
        element.style.transform = 'translateY(20px)';
        
        setTimeout(() => {
            element.style.transition = 'all 0.5s ease';
            element.style.opacity = '1';
            element.style.transform = 'translateY(0)';
        }, index * 100);
    });
}

// Mejoras en formularios
function setupFormEnhancements() {
    // Mejorar focus en inputs
    const inputs = document.querySelectorAll('.form-control, .form-select');
    
    inputs.forEach(input => {
        input.addEventListener('focus', function() {
            this.parentElement.classList.add('form-group-focused');
        });
        
        input.addEventListener('blur', function() {
            this.parentElement.classList.remove('form-group-focused');
        });
    });
    
    // Mejorar botones de filtro
    const filterForm = document.querySelector('form[method=\"get\"]');
    if (filterForm) {
        filterForm.addEventListener('submit', function(e) {
            const submitBtn = this.querySelector('.btn-modern.primary');
            if (submitBtn) {
                showLoadingState(submitBtn);
            }
        });
    }
}

// Efectos hover mejorados
function setupHoverEffects() {
    // Efectos para tarjetas
    const cards = document.querySelectorAll('.modern-card');
    cards.forEach(card => {
        card.addEventListener('mouseenter', function() {
            this.style.transform = 'translateY(-4px)';
            this.style.boxShadow = '0 8px 25px rgba(0,0,0,0.15)';
        });
        
        card.addEventListener('mouseleave', function() {
            this.style.transform = 'translateY(-2px)';
            this.style.boxShadow = '0 2px 8px rgba(0,0,0,0.1)';
        });
    });
    
    // Efectos para filas de tabla
    const tableRows = document.querySelectorAll('.table-modern tbody tr');
    tableRows.forEach(row => {
        row.addEventListener('mouseenter', function() {
            this.style.backgroundColor = 'rgba(0,123,255,0.08)';
            this.style.transform = 'translateX(2px)';
        });
        
        row.addEventListener('mouseleave', function() {
            this.style.backgroundColor = '';
            this.style.transform = 'translateX(0)';
        });
    });
}

// Funcionalidades de filtros
function setupFilterFunctionality() {
    // Auto-submit en algunos casos
    const selectFilters = document.querySelectorAll('.filters-container select');
    
    selectFilters.forEach(select => {
        select.addEventListener('change', function() {
            // Agregar indicador visual de que algo cambió
            this.style.borderColor = 'var(--primary-color)';
            this.style.boxShadow = '0 0 0 3px rgba(0,123,255,0.1)';
            
            setTimeout(() => {
                this.style.borderColor = '';
                this.style.boxShadow = '';
            }, 1000);
        });
    });
    
    // Limpiar filtros con animación
    const clearBtn = document.querySelector('.btn-modern.secondary');
    if (clearBtn && clearBtn.textContent.includes('Limpiar')) {
        clearBtn.addEventListener('click', function(e) {
            e.preventDefault();
            
            // Animación de limpieza
            const inputs = document.querySelectorAll('.filters-container select, .filters-container input');
            inputs.forEach((input, index) => {
                setTimeout(() => {
                    input.style.transform = 'scale(0.95)';
                    input.value = '';
                    
                    setTimeout(() => {
                        input.style.transform = 'scale(1)';
                    }, 100);
                }, index * 50);
            });
            
            // Redirigir después de la animación
            setTimeout(() => {
                window.location.href = this.href;
            }, 500);
        });
    }
}

// Estado de carga para botones
function showLoadingState(button) {
    const originalText = button.innerHTML;
    const loadingIcon = '<i class=\"fas fa-spinner fa-spin\"></i>';
    
    button.innerHTML = loadingIcon + ' Cargando...';
    button.style.pointerEvents = 'none';
    button.style.opacity = '0.8';
    
    // No restaurar automáticamente - se hará con la navegación
}

// Función para mostrar notificaciones
function showNotification(message, type = 'info') {
    const notification = document.createElement('div');
    notification.className = `alert-modern ${type}`;
    notification.style.position = 'fixed';
    notification.style.top = '20px';
    notification.style.right = '20px';
    notification.style.zIndex = '9999';
    notification.style.minWidth = '300px';
    notification.style.opacity = '0';
    notification.style.transform = 'translateX(100%)';
    notification.style.transition = 'all 0.3s ease';
    
    const icon = type === 'success' ? 'check-circle' : 
                 type === 'error' ? 'exclamation-circle' : 
                 type === 'warning' ? 'exclamation-triangle' : 'info-circle';
    
    notification.innerHTML = `
        <i class=\"fas fa-${icon}\"></i>
        <span>${message}</span>
        <button type=\"button\" onclick=\"this.parentElement.remove()\" style=\"background: none; border: none; color: inherit; font-size: 1.2em; margin-left: auto;\">&times;</button>
    `;
    
    document.body.appendChild(notification);
    
    // Animar entrada
    setTimeout(() => {
        notification.style.opacity = '1';
        notification.style.transform = 'translateX(0)';
    }, 100);
    
    // Auto-remover después de 5 segundos
    setTimeout(() => {
        notification.style.opacity = '0';
        notification.style.transform = 'translateX(100%)';
        setTimeout(() => {
            notification.remove();
        }, 300);
    }, 5000);
}

// Mejorar botones de acción de tabla
document.addEventListener('click', function(e) {
    if (e.target.closest('.btn-outline-info, .btn-outline-warning, .btn-outline-danger')) {
        const btn = e.target.closest('.btn-outline-info, .btn-outline-warning, .btn-outline-danger');
        
        // Efecto de ripple
        const ripple = document.createElement('span');
        ripple.style.position = 'absolute';
        ripple.style.borderRadius = '50%';
        ripple.style.background = 'rgba(255,255,255,0.6)';
        ripple.style.width = ripple.style.height = '10px';
        ripple.style.left = (e.offsetX - 5) + 'px';
        ripple.style.top = (e.offsetY - 5) + 'px';
        ripple.style.animation = 'ripple 0.6s linear';
        ripple.style.pointerEvents = 'none';
        
        btn.style.position = 'relative';
        btn.style.overflow = 'hidden';
        btn.appendChild(ripple);
        
        setTimeout(() => {
            ripple.remove();
        }, 600);
    }
});

// Agregar estilos CSS dinámicos
const dynamicStyles = `
    <style>
        @keyframes ripple {
            to {
                transform: scale(4);
                opacity: 0;
            }
        }
        
        .form-group-focused {
            transform: scale(1.02);
        }
        
        .btn-modern:active {
            transform: translateY(-1px) scale(0.98);
        }
        
        .table-modern tbody tr {
            transition: all 0.2s ease;
        }
        
        .modern-card {
            transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
        }
        
        .filters-container select,
        .filters-container input {
            transition: all 0.2s ease;
        }
        
        /* Mejoras responsive adicionales */
        @media (max-width: 768px) {
            .alert-modern {
                margin: 0 10px;
            }
            
            .modern-card {
                margin: 0 10px 2rem 10px;
            }
            
            .page-header {
                margin: 0 10px 2rem 10px;
            }
        }
        
        /* Estados de carga */
        .loading-overlay {
            position: fixed;
            top: 0;
            left: 0;
            right: 0;
            bottom: 0;
            background: rgba(255,255,255,0.8);
            display: flex;
            align-items: center;
            justify-content: center;
            z-index: 9999;
        }
        
        .loading-spinner {
            width: 40px;
            height: 40px;
            border: 4px solid #e9ecef;
            border-top: 4px solid var(--primary-color);
            border-radius: 50%;
            animation: spin 1s linear infinite;
        }
        
        @keyframes spin {
            0% { transform: rotate(0deg); }
            100% { transform: rotate(360deg); }
        }
    </style>
`;

document.head.insertAdjacentHTML('beforeend', dynamicStyles);