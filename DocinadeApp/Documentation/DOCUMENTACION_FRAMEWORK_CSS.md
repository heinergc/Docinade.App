# 📋 Framework CSS Genérico - Sistema de Rúbricas

## 🎯 Objetivo
Este documento describe el framework CSS genérico y modular implementado para el Sistema de Rúbricas, diseñado para proporcionar una interfaz de usuario consistente, moderna y profesional a través de todas las vistas de la aplicación.

## 🎨 Especificaciones de Diseño

### Paleta de Colores
```css
/* Colores Principales */
--color-primary: #3b82f6          /* Azul primario */
--color-primary-hover: #2563eb    /* Azul primario hover */
--color-secondary: #6b7280        /* Gris secundario */
--color-text-primary: #1f2937     /* Texto principal */
--color-background-page: #f3f4f6  /* Fondo de página */
--color-background-card: #ffffff  /* Fondo de tarjetas */
--color-background-footer: #2a599b /* Fondo del pie de página */
--color-border: #e5e7eb           /* Bordes */

/* Colores de Estado */
--color-success: #22c55e          /* Verde éxito */
--color-warning: #f59e0b          /* Amarillo advertencia */
--color-danger: #ef4444           /* Rojo peligro */
--color-info: #3b82f6            /* Azul información */
```

### Tipografía
- **Fuente Principal**: Inter (Google Fonts)
- **Iconografía**: Font Awesome 6
- **Pesos**: 300, 400, 500, 600, 700

## 🧩 Componentes Reutilizables

### 1. Estructura Base
```css
.main-container /* Contenedor principal con max-width: 1200px */
```

### 2. Tarjetas (.card)
#### Tarjeta Básica
```css
.card              /* Tarjeta básica */
.card-header       /* Encabezado de tarjeta */
.card-body         /* Cuerpo de tarjeta */
.card-footer       /* Pie de tarjeta */
.card-title        /* Título de tarjeta */
```

#### Variantes de Color
```css
.card-primary      /* Tarjeta azul */
.card-success      /* Tarjeta verde */
.card-warning      /* Tarjeta amarilla */
.card-danger       /* Tarjeta roja */
.card-modern       /* Tarjeta con estilos modernos mejorados */
```

### 3. Botones (.btn)
#### Tipos Básicos
```css
.btn               /* Botón base */
.btn-primary       /* Botón azul principal */
.btn-secondary     /* Botón gris transparente */
.btn-small         /* Botón pequeño para tablas */
```

#### Botones de Estado
```css
.btn-success       /* Botón verde */
.btn-warning       /* Botón amarillo */
.btn-danger        /* Botón rojo */
.btn-info          /* Botón azul información */
```

#### Botones Outline
```css
.btn-outline-primary    /* Borde azul, fondo transparente */
.btn-outline-secondary  /* Borde gris, fondo transparente */
.btn-outline-success    /* Borde verde, fondo transparente */
.btn-outline-warning    /* Borde amarillo, fondo transparente */
.btn-outline-danger     /* Borde rojo, fondo transparente */
.btn-outline-info       /* Borde azul info, fondo transparente */
```

#### Grupos de Botones
```css
.btn-group         /* Grupo básico de botones */
.btn-group-modern  /* Grupo moderno con espaciado mejorado */
```

### 4. Tablas (.table)
```css
.table-container   /* Contenedor principal de tabla */
.table-header      /* Encabezado con título y acciones */
.table-title       /* Título de la tabla */
.table-actions     /* Acciones de la tabla */
.table-content     /* Contenido scrolleable */
.table-modern      /* Tabla con estilos modernos */
```

### 5. Formularios (.form-group)
```css
.form-group        /* Grupo de campo de formulario */
.form-label        /* Etiqueta de campo */
.form-control      /* Campo de entrada */
.form-select       /* Campo de selección */
.is-invalid        /* Estado de error */
.invalid-feedback  /* Mensaje de error */
```

### 6. Badges (.badge)
#### Tipos Básicos
```css
.badge             /* Badge base */
.badge-primary     /* Badge azul */
.badge-secondary   /* Badge gris */
.badge-success     /* Badge verde */
.badge-warning     /* Badge amarillo */
.badge-danger      /* Badge rojo */
.badge-info        /* Badge azul información */
```

#### Badge de Estado
```css
.status-badge      /* Badge de estado genérico */
.status-badge.inactive  /* Estado inactivo */
.status-badge.warning   /* Estado de advertencia */
.status-badge.danger    /* Estado de peligro */
```

### 7. Sección de Encabezado (.header-section)
```css
.header-section    /* Contenedor principal del encabezado */
.header-title      /* Contenedor del título */
.page-title        /* Título principal de la página */
.header-subtitle   /* Subtítulo descriptivo */
.header-actions    /* Contenedor de acciones */
```

### 8. Componentes Modernos Específicos
#### Grid de Estadísticas
```css
.stats-grid        /* Grid responsive para estadísticas */
.stats-icon        /* Icono de estadística */
.stats-number      /* Número principal */
.stats-label       /* Etiqueta descriptiva */
```

#### Estado Vacío
```css
.empty-state       /* Contenedor de estado vacío */
.empty-state-icon  /* Icono de estado vacío */
.empty-state-title /* Título de estado vacío */
.empty-state-text  /* Texto descriptivo */
```

#### Paginación
```css
.pagination-container  /* Contenedor de paginación */
.pagination           /* Lista de paginación */
.page-item            /* Elemento de página */
.page-link            /* Enlace de página */
```

## 🛠️ Utilidades CSS

### Espaciado
```css
.mt-1, .mt-2, .mt-3, .mt-4, .mt-5    /* Margin top */
.mb-1, .mb-2, .mb-3, .mb-4, .mb-5    /* Margin bottom */
.p-1, .p-2, .p-3, .p-4, .p-5         /* Padding */
```

### Flexbox
```css
.d-flex                    /* Display flex */
.d-inline-flex            /* Display inline-flex */
.justify-content-center   /* Justify center */
.justify-content-between  /* Justify space-between */
.justify-content-end      /* Justify end */
.align-items-center       /* Align center */
.align-items-start        /* Align start */
.align-items-end          /* Align end */
.flex-wrap                /* Flex wrap */
.gap-1, .gap-2, .gap-3    /* Gap spacing */
```

### Texto
```css
.text-center, .text-left, .text-right           /* Alineación */
.text-primary, .text-secondary, .text-success   /* Colores */
.text-warning, .text-danger, .text-muted        /* Colores */
.fw-normal, .fw-medium, .fw-semibold, .fw-bold  /* Peso de fuente */
```

### Fondo y Bordes
```css
.bg-primary, .bg-secondary, .bg-success         /* Fondos */
.bg-warning, .bg-danger, .bg-light, .bg-white  /* Fondos */
.border, .border-primary, .border-secondary     /* Bordes */
.rounded, .rounded-sm, .rounded-lg              /* Bordes redondeados */
.shadow-sm, .shadow, .shadow-lg                 /* Sombras */
```

### Ancho
```css
.w-25, .w-50, .w-75, .w-100  /* Anchos en porcentaje */
```

## 🎨 Alertas y Mensajes
```css
.alert             /* Alerta base */
.alert-primary     /* Alerta azul */
.alert-success     /* Alerta verde */
.alert-warning     /* Alerta amarilla */
.alert-danger      /* Alerta roja */
.alert-info        /* Alerta azul información */
```

## 📱 Diseño Responsivo

### Breakpoints
- **Desktop**: > 1200px
- **Tablet**: 768px - 1200px
- **Mobile Large**: 576px - 768px
- **Mobile Small**: < 576px

### Comportamientos Responsivos
1. **Header Section**: Se apila verticalmente en móvil
2. **Botones**: Se expanden al 100% del ancho en móvil
3. **Stats Grid**: Se adapta de 4 columnas a 2 en móvil
4. **Tablas**: Se vuelven scrolleables horizontalmente
5. **Texto de Botones**: Se oculta en pantallas muy pequeñas

## 🚀 Implementación

### 1. Integración en Layout
El archivo `estilos_genericos.css` está integrado en `_Layout.cshtml`:
```html
<link rel="stylesheet" href="~/css/estilos_genericos.css" asp-append-version="true" />
```

### 2. Uso en Vistas
```html
<!-- Ejemplo de Header Section -->
<div class="header-section">
    <div class="header-title">
        <h1 class="page-title">
            <i class="fas fa-users-cog text-primary"></i>
            Título de la Página
        </h1>
        <p class="header-subtitle">Descripción de la funcionalidad</p>
    </div>
    <div class="header-actions">
        <div class="btn-group-modern">
            <a href="#" class="btn btn-primary">
                <i class="fas fa-plus"></i> Crear
            </a>
            <a href="#" class="btn btn-secondary">
                <i class="fas fa-download"></i> Exportar
            </a>
        </div>
    </div>
</div>
```

### 3. Ejemplo de Tarjeta Moderna
```html
<div class="card-modern">
    <div class="card-header">
        <h5 class="card-title">Título de la Tarjeta</h5>
    </div>
    <div class="card-body">
        Contenido de la tarjeta
    </div>
</div>
```

### 4. Ejemplo de Tabla Moderna
```html
<div class="table-container">
    <div class="table-header">
        <h5 class="table-title">
            <i class="fas fa-list"></i> Lista de Elementos
        </h5>
        <div class="table-actions">
            <button class="btn btn-outline-secondary">
                <i class="fas fa-filter"></i>
            </button>
        </div>
    </div>
    <div class="table-content">
        <table class="table-modern">
            <!-- Contenido de la tabla -->
        </table>
    </div>
</div>
```

## ✅ Ventajas del Framework

1. **Consistencia**: Todos los elementos mantienen el mismo estilo visual
2. **Modularidad**: Los componentes son reutilizables
3. **Responsividad**: Diseño adaptable a cualquier dispositivo
4. **Mantenibilidad**: Fácil de modificar y extender
5. **Performance**: CSS optimizado con variables personalizadas
6. **Accesibilidad**: Cumple con estándares de accesibilidad web

## 🔧 Mantenimiento

### Modificación de Colores
Para cambiar la paleta de colores, editar las variables CSS en la sección `:root`:
```css
:root {
  --color-primary: #nuevo-color;
  /* Otros colores... */
}
```

### Añadir Nuevos Componentes
1. Seguir la estructura de comentarios existente
2. Usar las variables CSS definidas
3. Mantener la nomenclatura consistente
4. Incluir estados responsive

### Extensión del Framework
Para añadir nuevas utilidades o componentes:
1. Añadir al final del archivo CSS
2. Documentar en este archivo
3. Seguir las convenciones de nomenclatura
4. Probar en diferentes dispositivos

## 📝 Notas de Implementación

- Compatible con Bootstrap 5
- Requiere Font Awesome 6 para iconos
- Utiliza Google Fonts (Inter)
- CSS Variables para máxima flexibilidad
- Transiciones suaves para mejor UX
- Optimizado para performance

---

**Autor**: Sistema de Rúbricas  
**Versión**: 1.0  
**Última actualización**: Septiembre 2025  
