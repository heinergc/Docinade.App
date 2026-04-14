# Guía de Estilos Genéricos para Sistema de Rúbricas

## Descripción
Este documento describe el sistema de estilos genéricos implementado para la aplicación Sistema de Rúbricas. Los estilos están diseñados para ser modulares, reutilizables y mantener consistencia visual en todas las vistas.

## Archivo de Estilos
**Ubicación:** `/wwwroot/css/estilos_genericos.css`

## Paleta de Colores

### Colores Principales
- **Primario:** `#3b82f6` (azul)
- **Primario Hover:** `#2563eb` (azul más oscuro)
- **Secundario:** `#6b7280` (gris)
- **Texto Principal:** `#1f2937` (gris muy oscuro)
- **Fondo de Página:** `#f3f4f6` (gris muy claro)
- **Fondo de Tarjetas:** `#ffffff` (blanco)
- **Fondo del Pie de Página:** `#2a599b` (azul oscuro)
- **Bordes:** `#e5e7eb` (gris claro)

### Colores de Estado
- **Éxito:** `#22c55e` (verde)
- **Advertencia:** `#f59e0b` (amarillo)
- **Peligro:** `#ef4444` (rojo)
- **Información:** `#3b82f6` (azul)

## Componentes Disponibles

### 1. Tarjetas (.card)

#### Uso Básico
```html
<div class="card">
    <div class="card-header">
        <h5 class="card-title">Título de la Tarjeta</h5>
    </div>
    <div class="card-body">
        Contenido de la tarjeta
    </div>
    <div class="card-footer">
        Pie de la tarjeta
    </div>
</div>
```

#### Variantes de Color
```html
<div class="card card-primary">...</div>
<div class="card card-success">...</div>
<div class="card card-warning">...</div>
<div class="card card-danger">...</div>
```

### 2. Botones (.btn)

#### Tipos de Botones
```html
<!-- Botón primario -->
<button class="btn btn-primary">
    <i class="fas fa-save"></i> Guardar
</button>

<!-- Botón secundario -->
<button class="btn btn-secondary">Cancelar</button>

<!-- Botón pequeño para tablas -->
<button class="btn btn-small btn-outline-info">
    <i class="fas fa-eye"></i>
</button>
```

#### Botones de Estado
```html
<button class="btn btn-success">Éxito</button>
<button class="btn btn-warning">Advertencia</button>
<button class="btn btn-danger">Peligro</button>
<button class="btn btn-info">Información</button>
```

#### Botones Outline
```html
<button class="btn btn-outline-primary">Primario</button>
<button class="btn btn-outline-success">Éxito</button>
<button class="btn btn-outline-warning">Advertencia</button>
<button class="btn btn-outline-danger">Peligro</button>
```

#### Grupo de Botones
```html
<div class="btn-group" role="group">
    <button class="btn btn-outline-info"><i class="fas fa-eye"></i></button>
    <button class="btn btn-outline-warning"><i class="fas fa-edit"></i></button>
    <button class="btn btn-outline-danger"><i class="fas fa-trash"></i></button>
</div>
```

### 3. Tablas (.table)

#### Estructura Completa
```html
<div class="table-container">
    <div class="table-responsive">
        <table class="table">
            <thead>
                <tr>
                    <th><i class="fas fa-code"></i> Código</th>
                    <th><i class="fas fa-book"></i> Nombre</th>
                    <th class="table-actions"><i class="fas fa-cogs"></i> Acciones</th>
                </tr>
            </thead>
            <tbody>
                <tr>
                    <td><span class="badge badge-primary">ESP001</span></td>
                    <td>Español</td>
                    <td class="table-actions">
                        <div class="btn-group">
                            <button class="btn btn-small btn-outline-info">
                                <i class="fas fa-eye"></i>
                            </button>
                        </div>
                    </td>
                </tr>
            </tbody>
        </table>
    </div>
</div>
```

### 4. Formularios (.form-group)

#### Estructura de Formulario
```html
<div class="form-group">
    <label class="form-label">
        <i class="fas fa-user"></i> Nombre
    </label>
    <input type="text" class="form-control" placeholder="Ingrese el nombre" />
    <span class="invalid-feedback">Error de validación</span>
</div>

<div class="form-group">
    <label class="form-label">
        <i class="fas fa-list"></i> Categoría
    </label>
    <select class="form-control form-select">
        <option>Opción 1</option>
        <option>Opción 2</option>
    </select>
</div>
```

### 5. Badges (.badge)

#### Tipos de Badge
```html
<span class="badge badge-primary">Primario</span>
<span class="badge badge-success">Éxito</span>
<span class="badge badge-warning">Advertencia</span>
<span class="badge badge-danger">Peligro</span>
<span class="badge badge-info">Información</span>
<span class="badge badge-secondary">Secundario</span>
```

#### Badge de Estado
```html
<span class="status-badge">Activo</span>
<span class="status-badge inactive">Inactivo</span>
<span class="status-badge warning">Advertencia</span>
<span class="status-badge danger">Peligro</span>
```

### 6. Sección de Encabezado (.header-section)

```html
<div class="header-section">
    <div>
        <h2><i class="fas fa-book"></i> Gestión de Materias</h2>
        <p class="text-muted mb-0">Administra las materias del sistema</p>
    </div>
    <div class="header-actions">
        <a href="#" class="btn btn-primary">
            <i class="fas fa-plus"></i> Nueva Materia
        </a>
    </div>
</div>
```

## Utilidades de CSS

### Espaciado
```html
<!-- Márgenes -->
<div class="mt-1 mt-2 mt-3 mt-4 mt-5"></div>
<div class="mb-1 mb-2 mb-3 mb-4 mb-5"></div>

<!-- Padding -->
<div class="p-1 p-2 p-3 p-4 p-5"></div>
```

### Flexbox
```html
<div class="d-flex justify-content-between align-items-center">
    <div>Contenido izquierdo</div>
    <div>Contenido derecho</div>
</div>

<div class="d-flex gap-3">
    <button class="btn btn-primary">Botón 1</button>
    <button class="btn btn-secondary">Botón 2</button>
</div>
```

### Texto
```html
<p class="text-center text-primary fw-bold">Texto centrado, azul y en negrita</p>
<p class="text-muted">Texto secundario</p>
<p class="text-success">Texto de éxito</p>
<p class="text-warning">Texto de advertencia</p>
<p class="text-danger">Texto de peligro</p>
```

### Fondos y Bordes
```html
<div class="bg-light border rounded p-3">
    Contenido con fondo claro, borde y esquinas redondeadas
</div>

<div class="shadow-lg">Elemento con sombra grande</div>
<div class="shadow">Elemento con sombra media</div>
<div class="shadow-sm">Elemento con sombra pequeña</div>
```

## Alertas

```html
<div class="alert alert-success">
    <i class="fas fa-check-circle me-2"></i>
    Operación exitosa
</div>

<div class="alert alert-warning">
    <i class="fas fa-exclamation-triangle me-2"></i>
    Advertencia importante
</div>

<div class="alert alert-danger">
    <i class="fas fa-times-circle me-2"></i>
    Error crítico
</div>

<div class="alert alert-info">
    <i class="fas fa-info-circle me-2"></i>
    Información adicional
</div>
```

## Paginación

```html
<nav aria-label="Paginación">
    <ul class="pagination">
        <li class="page-item">
            <a class="page-link" href="#"><i class="fas fa-chevron-left"></i> Anterior</a>
        </li>
        <li class="page-item active">
            <a class="page-link" href="#">1</a>
        </li>
        <li class="page-item">
            <a class="page-link" href="#">2</a>
        </li>
        <li class="page-item">
            <a class="page-link" href="#">Siguiente <i class="fas fa-chevron-right"></i></a>
        </li>
    </ul>
</nav>
```

## Responsividad

Los estilos incluyen breakpoints responsivos:

- **Desktop:** `> 768px` - Comportamiento estándar
- **Tablet:** `<= 768px` - Header apilado, botones adaptados
- **Mobile:** `<= 480px` - Padding reducido, botones más pequeños

## Integración con Font Awesome

Todos los ejemplos incluyen iconos de Font Awesome 6. Los iconos se pueden usar con cualquier componente:

```html
<button class="btn btn-primary">
    <i class="fas fa-save"></i> Guardar
</button>

<h2><i class="fas fa-book"></i> Título con Icono</h2>

<span class="badge badge-success">
    <i class="fas fa-check"></i> Completado
</span>
```

## Aplicación en Vistas de Materias

### Archivos de Ejemplo Creados

1. **IndexNew.cshtml** - Vista de listado con estilos genéricos
2. **DetailsNew.cshtml** - Vista de detalles con layout moderno
3. **EditNew.cshtml** - Vista de edición con formulario estructurado

### Para Implementar los Nuevos Estilos

1. Asegúrate de que `estilos_genericos.css` esté incluido en el layout
2. Reemplaza las vistas existentes con las versiones "New" o adapta el código
3. Usa las clases genéricas en lugar de estilos específicos
4. Mantén la consistencia usando la paleta de colores definida

## Variables CSS Personalizadas

El archivo usa CSS Custom Properties (variables) para facilitar el mantenimiento:

```css
:root {
  --color-primary: #3b82f6;
  --color-primary-hover: #2563eb;
  --shadow-md: 0 4px 6px -1px rgba(0, 0, 0, 0.1);
  --radius-lg: 0.75rem;
}
```

Esto permite cambios globales fáciles modificando solo las variables.

## Notas de Implementación

1. **Compatibilidad:** Estilos compatibles con Bootstrap 5
2. **Performance:** CSS optimizado con selectores eficientes
3. **Mantenimiento:** Variables CSS para fácil personalización
4. **Accesibilidad:** Contrastes y tamaños accesibles
5. **Mobile-First:** Diseño responsivo desde mobile hacia desktop

## Próximos Pasos

1. Aplicar los estilos a otras vistas del sistema
2. Crear componentes adicionales según necesidades
3. Documentar patrones de uso específicos por módulo
4. Implementar tema oscuro usando las mismas variables
