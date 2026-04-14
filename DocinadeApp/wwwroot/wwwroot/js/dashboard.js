/**
 * DASHBOARD FUNCTIONALITY
 * JavaScript para la funcionalidad del sistema dashboard
 * Compatible con dashboard.css
 * Autor: Sistema de Rúbricas
 * Fecha: 12 de septiembre de 2025
 */

class DashboardManager {
    constructor() {
        this.sidebar = document.querySelector('.dashboard-sidebar');
        this.content = document.querySelector('.dashboard-content');
        this.toggleBtn = document.querySelector('.sidebar-toggle');
        this.isCollapsed = false;
        this.isMobile = window.innerWidth <= 768;
        
        this.init();
    }

    init() {
        this.setupEventListeners();
        this.setupResponsive();
        this.animateCards();
        console.log('Dashboard inicializado correctamente');
    }

    setupEventListeners() {
        // Toggle sidebar
        if (this.toggleBtn) {
            this.toggleBtn.addEventListener('click', () => this.toggleSidebar());
        }

        // Click fuera del sidebar en móvil
        document.addEventListener('click', (e) => {
            if (this.isMobile && this.sidebar && !this.sidebar.contains(e.target) && !this.toggleBtn.contains(e.target)) {
                this.closeSidebar();
            }
        });

        // Tecla Escape para cerrar sidebar
        document.addEventListener('keydown', (e) => {
            if (e.key === 'Escape' && this.isMobile) {
                this.closeSidebar();
            }
        });

        // Smooth scroll para enlaces internos
        document.querySelectorAll('a[href^="#"]').forEach(link => {
            link.addEventListener('click', (e) => {
                e.preventDefault();
                const target = document.querySelector(link.getAttribute('href'));
                if (target) {
                    target.scrollIntoView({ behavior: 'smooth' });
                }
            });
        });

        // Auto-actualización de badges de notificación
        this.setupNotificationBadges();
    }

    setupResponsive() {
        window.addEventListener('resize', () => {
            this.isMobile = window.innerWidth <= 768;
            
            if (!this.isMobile && this.sidebar) {
                this.sidebar.classList.remove('open');
                this.sidebar.classList.remove('collapsed');
            }
        });
    }

    toggleSidebar() {
        if (this.isMobile) {
            if (this.sidebar) {
                this.sidebar.classList.toggle('open');
            }
        } else {
            if (this.sidebar) {
                this.sidebar.classList.toggle('collapsed');
                this.isCollapsed = !this.isCollapsed;
            }
        }
    }

    closeSidebar() {
        if (this.sidebar) {
            this.sidebar.classList.remove('open');
        }
    }

    animateCards() {
        // Animación en cascada para las tarjetas
        const cards = document.querySelectorAll('.dashboard-card');
        cards.forEach((card, index) => {
            card.style.animationDelay = `${index * 0.1}s`;
        });

        // Intersection Observer para animaciones al hacer scroll
        if ('IntersectionObserver' in window) {
            const observer = new IntersectionObserver((entries) => {
                entries.forEach(entry => {
                    if (entry.isIntersecting) {
                        entry.target.style.animation = 'dashboardFadeIn 0.5s ease-out';
                    }
                });
            }, { threshold: 0.1 });

            document.querySelectorAll('.dashboard-widget').forEach(widget => {
                observer.observe(widget);
            });
        }
    }

    setupNotificationBadges() {
        // Simular actualización de badges (en producción vendría del servidor)
        setInterval(() => {
            const badges = document.querySelectorAll('.nav-badge');
            badges.forEach(badge => {
                // Agregar efecto de pulso si hay notificaciones
                if (parseInt(badge.textContent) > 0) {
                    badge.style.animation = 'pulse 2s infinite';
                }
            });
        }, 5000);
    }

    // Método público para actualizar métricas
    updateMetric(cardId, newValue, animated = true) {
        const card = document.querySelector(`[data-card-id="${cardId}"]`);
        if (card) {
            const metric = card.querySelector('.card-metric');
            if (metric && animated) {
                this.animateNumber(metric, parseInt(metric.textContent) || 0, newValue);
            } else if (metric) {
                metric.textContent = newValue;
            }
        }
    }

    animateNumber(element, from, to, duration = 1000) {
        const startTime = performance.now();
        const animate = (currentTime) => {
            const elapsed = currentTime - startTime;
            const progress = Math.min(elapsed / duration, 1);
            
            const value = Math.floor(from + (to - from) * this.easeOutQuart(progress));
            element.textContent = value.toLocaleString();
            
            if (progress < 1) {
                requestAnimationFrame(animate);
            }
        };
        requestAnimationFrame(animate);
    }

    easeOutQuart(t) {
        return 1 - Math.pow(1 - t, 4);
    }

    // Método para mostrar notificaciones toast
    showToast(message, type = 'info', duration = 5000) {
        const toast = document.createElement('div');
        toast.className = `dashboard-toast toast-${type}`;
        toast.innerHTML = `
            <div class="toast-content">
                <i class="fas fa-${this.getToastIcon(type)}"></i>
                <span>${message}</span>
            </div>
            <button class="toast-close" onclick="this.parentElement.remove()">
                <i class="fas fa-times"></i>
            </button>
        `;

        // Crear contenedor si no existe
        let toastContainer = document.querySelector('.toast-container');
        if (!toastContainer) {
            toastContainer = document.createElement('div');
            toastContainer.className = 'toast-container';
            document.body.appendChild(toastContainer);
        }

        toastContainer.appendChild(toast);

        // Auto-remover después del tiempo especificado
        setTimeout(() => {
            if (toast.parentElement) {
                toast.style.animation = 'slideOut 0.3s ease-in';
                setTimeout(() => toast.remove(), 300);
            }
        }, duration);
    }

    getToastIcon(type) {
        const icons = {
            'success': 'check-circle',
            'error': 'exclamation-circle',
            'warning': 'exclamation-triangle',
            'info': 'info-circle'
        };
        return icons[type] || 'info-circle';
    }
}

// Estilos adicionales para toasts
const toastStyles = `
.toast-container {
    position: fixed;
    top: 1rem;
    right: 1rem;
    z-index: 9999;
    display: flex;
    flex-direction: column;
    gap: 0.5rem;
}

.dashboard-toast {
    background: white;
    border-radius: 8px;
    padding: 1rem;
    box-shadow: 0 4px 12px rgba(0,0,0,0.15);
    border-left: 4px solid var(--dashboard-info);
    display: flex;
    align-items: center;
    justify-content: space-between;
    min-width: 300px;
    animation: slideIn 0.3s ease-out;
}

.dashboard-toast.toast-success {
    border-left-color: var(--dashboard-success);
}

.dashboard-toast.toast-error {
    border-left-color: var(--dashboard-danger);
}

.dashboard-toast.toast-warning {
    border-left-color: var(--dashboard-warning);
}

.toast-content {
    display: flex;
    align-items: center;
    gap: 0.75rem;
}

.toast-content i {
    font-size: 1.2rem;
    color: var(--dashboard-info);
}

.toast-success .toast-content i {
    color: var(--dashboard-success);
}

.toast-error .toast-content i {
    color: var(--dashboard-danger);
}

.toast-warning .toast-content i {
    color: var(--dashboard-warning);
}

.toast-close {
    background: none;
    border: none;
    color: #6c757d;
    cursor: pointer;
    padding: 0.25rem;
    border-radius: 4px;
    transition: color 0.2s ease;
}

.toast-close:hover {
    color: #495057;
    background: #f8f9fa;
}

@keyframes slideIn {
    from {
        transform: translateX(100%);
        opacity: 0;
    }
    to {
        transform: translateX(0);
        opacity: 1;
    }
}

@keyframes slideOut {
    from {
        transform: translateX(0);
        opacity: 1;
    }
    to {
        transform: translateX(100%);
        opacity: 0;
    }
}

@keyframes pulse {
    0% { transform: scale(1); }
    50% { transform: scale(1.1); }
    100% { transform: scale(1); }
}
`;

// Agregar estilos al documento
const styleSheet = document.createElement('style');
styleSheet.textContent = toastStyles;
document.head.appendChild(styleSheet);

// Utility functions globales
window.Dashboard = {
    // Función para inicializar dashboard
    init: function() {
        if (typeof window.dashboardManager === 'undefined') {
            window.dashboardManager = new DashboardManager();
        }
    },

    // Función para actualizar métricas
    updateMetric: function(cardId, newValue, animated = true) {
        if (window.dashboardManager) {
            window.dashboardManager.updateMetric(cardId, newValue, animated);
        }
    },

    // Función para mostrar notificaciones
    showNotification: function(message, type = 'info', duration = 5000) {
        if (window.dashboardManager) {
            window.dashboardManager.showToast(message, type, duration);
        }
    },

    // Función para toggle del sidebar
    toggleSidebar: function() {
        if (window.dashboardManager) {
            window.dashboardManager.toggleSidebar();
        }
    }
};

// Auto-inicialización cuando el DOM esté listo
document.addEventListener('DOMContentLoaded', function() {
    // Verificar si estamos en una página que usa dashboard
    if (document.querySelector('.dashboard-container')) {
        Dashboard.init();
        console.log('Dashboard System - Cargado correctamente');
    }
});

// Exportar para módulos ES6 si es necesario
if (typeof module !== 'undefined' && module.exports) {
    module.exports = { DashboardManager, Dashboard };
}
