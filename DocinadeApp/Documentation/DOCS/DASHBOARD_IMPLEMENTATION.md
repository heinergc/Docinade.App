# Sistema de Dashboard - Documentación de Implementación

## Descripción General

El sistema de dashboard implementado proporciona una interfaz moderna y responsive para la gestión del sistema de rúbricas académicas. Incluye navegación lateral, tarjetas modulares, y funcionalidad interactiva.

## Componentes Implementados

### 1. Archivos CSS
- **`wwwroot/css/dashboard.css`**: Framework completo de estilos para dashboard
  - Variables CSS para personalización
  - Sistema de grid responsive
  - Estilos para sidebar y navegación
  - Tarjetas modulares con animaciones
  - Breakpoints para móvil (480px), tablet (768px), desktop (1024px+)

### 2. Archivos JavaScript
- **`wwwroot/js/dashboard.js`**: Funcionalidad interactiva del dashboard
  - Clase `DashboardManager` para gestión del estado
  - Toggle de sidebar responsive
  - Sistema de notificaciones toast
  - Animaciones y efectos visuales
  - API pública para actualizar métricas

### 3. Vistas
- **`Views/Dashboard/Index.cshtml`**: Vista principal del dashboard
  - Layout completo sin dependencia de Layout.cshtml
  - Sidebar con navegación modular
  - Grid de tarjetas para módulos principales
  - Widgets de actividad reciente y estadísticas

### 4. Controlador
- **`Controllers/DashboardController.cs`**: Lógica del servidor
  - Métodos para métricas del sistema
  - Endpoints AJAX para actualizaciones
  - Datos de ejemplo para desarrollo

## Características Principales

### Diseño Responsive
- **Desktop (1024px+)**: Sidebar completo visible, grid de 3 columnas
- **Tablet (768px-1023px)**: Sidebar colapsable, grid de 2 columnas
- **Móvil (480px-767px)**: Sidebar overlay, grid de 1 columna
- **Móvil pequeño (<480px)**: Optimizado para pantallas pequeñas

### Navegación Lateral
- Sidebar de 250px de ancho fijo
- Secciones organizadas: Principal, Gestión, Sistema
- Icons de Font Awesome
- Badges de notificación
- Animaciones smooth de hover

### Sistema de Tarjetas
- Tarjetas modulares y reutilizables
- Iconos coloridos por categoría
- Métricas destacadas
- Botones de acción (ver/crear)
- Efectos hover y animaciones

### Funcionalidad JavaScript
- Toggle de sidebar para móvil
- Sistema de notificaciones toast
- Actualización dinámica de métricas
- Animaciones de entrada para elementos

## Integración con la Aplicación Existente

### 1. Referencias CSS
Agregar al `_Layout.cshtml` o a las vistas específicas:
```html
<link href="~/css/dashboard.css" rel="stylesheet" />
```

### 2. Referencias JavaScript
Agregar antes del cierre del `</body>`:
```html
<script src="~/js/dashboard.js"></script>
```

### 3. Rutas del Controlador
El controlador `DashboardController` está configurado para:
- `/Dashboard` - Vista principal
- `/Dashboard/GetMetrics` - API para métricas (AJAX)
- `/Dashboard/GetRecentActivity` - API para actividad reciente
- `/Dashboard/Estadisticas` - Vista de estadísticas detalladas

## Uso del Dashboard

### Inicialización Básica
```javascript
// El dashboard se inicializa automáticamente
// Funciones disponibles globalmente a través del objeto Dashboard

// Toggle del sidebar
Dashboard.toggleSidebar();

// Actualizar una métrica
Dashboard.updateMetric('grupos', 10);

// Mostrar notificación
Dashboard.showNotification('Operación exitosa', 'success');
```

### Personalización de Colores
El sistema usa variables CSS que pueden ser modificadas:
```css
:root {
  --dashboard-primary: #2c3e50;      /* Color principal */
  --dashboard-secondary: #34495e;    /* Color secundario */
  --dashboard-accent: #3498db;       /* Color de acento */
  --dashboard-success: #27ae60;      /* Color de éxito */
  --dashboard-warning: #f39c12;      /* Color de advertencia */
  --dashboard-danger: #e74c3c;       /* Color de peligro */
}
```

### Agregar Nuevas Tarjetas
Estructura básica de una tarjeta:
```html
<div class="dashboard-card" data-card-id="mi-modulo">
    <div class="card-header-dashboard">
        <div class="card-icon success">
            <i class="fas fa-icon"></i>
        </div>
        <h3 class="card-title">Título del Módulo</h3>
    </div>
    <div class="card-metric">42</div>
    <p class="card-subtitle">Descripción de la métrica</p>
    <div class="card-actions">
        <a href="/MiModulo" class="dashboard-btn btn-primary">
            <i class="fas fa-eye"></i>
            Ver Todos
        </a>
        <a href="/MiModulo/Create" class="dashboard-btn btn-outline">
            <i class="fas fa-plus"></i>
            Crear Nuevo
        </a>
    </div>
</div>
```

## Migración desde Vistas Existentes

### Opción 1: Layout Específico para Dashboard
Crear `_DashboardLayout.cshtml` para usar con vistas específicas:
```csharp
@{
    Layout = "_DashboardLayout";
}
```

### Opción 2: Integración con Layout Existente
Modificar el `_Layout.cshtml` existente para incluir:
- Referencia a `dashboard.css`
- Estructura condicional para mostrar sidebar
- JavaScript de dashboard

### Opción 3: Vistas Híbridas
Combinar elementos del dashboard con el layout existente:
```html
<!-- En _Layout.cshtml -->
@if (ViewContext.RouteData.Values["controller"]?.ToString() == "Dashboard")
{
    <!-- Incluir estilos y estructura de dashboard -->
}
```

## Módulos Actuales Integrados

El dashboard incluye enlaces a los módulos existentes:
- **Grupos de Estudiantes** (`/GruposEstudiantes`)
- **Evaluaciones** (`/Evaluaciones`)
- **Rúbricas** (`/Rubricas`)
- **Instrumentos** (`/Instrumentos`)
- **Configuración** (`/Configuracion`)
- **Reportes** (`/Reportes`)
- **Usuarios** (`/Usuarios`)

## Testing y Validación

### Responsividad
El sistema ha sido diseñado para funcionar en:
- Chrome, Firefox, Safari, Edge
- Dispositivos móviles iOS y Android
- Tablets en orientación vertical y horizontal

### Pruebas Recomendadas
1. **Funcionalidad del sidebar**: Toggle en diferentes resoluciones
2. **Responsive design**: Validar en breakpoints principales
3. **Navegación**: Verificar enlaces a módulos existentes
4. **JavaScript**: Confirmar notificaciones y actualizaciones
5. **Accesibilidad**: Navegación por teclado y lectores de pantalla

## Próximos Pasos

### Fase 1: Integración Básica
- [ ] Agregar referencias CSS/JS al layout principal
- [ ] Configurar ruta en el menú de navegación
- [ ] Probar funcionalidad básica

### Fase 2: Datos Reales
- [ ] Conectar métricas con la base de datos
- [ ] Implementar queries reales en `DashboardController`
- [ ] Agregar cache para mejorar performance

### Fase 3: Funcionalidad Avanzada
- [ ] Gráficos y visualizaciones
- [ ] Filtros de fecha para métricas
- [ ] Exportación de reportes
- [ ] Notificaciones en tiempo real

### Fase 4: Personalización
- [ ] Configuración de dashboard por usuario
- [ ] Widgets personalizables
- [ ] Temas de color alternativos
- [ ] Configuración de idioma

## Notas Técnicas

### Compatibilidad
- Bootstrap 5.3.0+
- Font Awesome 6.4.0+
- Navegadores modernos (ES6+)
- ASP.NET Core 6.0+

### Performance
- CSS optimizado con custom properties
- JavaScript vanilla sin dependencias externas
- Lazy loading para widgets de estadísticas
- Compresión automática de assets en producción

### Seguridad
- Controlador con `[Authorize]`
- Validación de entrada en endpoints AJAX
- Escape automático de contenido dinámico
- CSRF protection en formularios
