# 📚 Documentación: ViewComponent BuscadorEstudiante

## 📋 Descripción General

El **BuscadorEstudianteViewComponent** es un componente reutilizable de ASP.NET Core que proporciona funcionalidad de búsqueda y selección de estudiantes mediante un modal interactivo con búsqueda en tiempo real.

## 🏗️ Arquitectura del Componente

### Componentes Principales

1. **BuscadorEstudianteViewComponent.cs** - Lógica del ViewComponent
2. **Default.cshtml** - Vista del componente (template)
3. **buscador-estudiante.js** - JavaScript para funcionalidad del cliente
4. **BuscadorEstudianteViewModel.cs** - Modelo de datos

## 🚀 Características

- ✅ **Búsqueda en tiempo real** con debounce (300ms)
- ✅ **Modal Bootstrap 5** responsivo
- ✅ **Múltiples instancias** en la misma página
- ✅ **Paginación automática** (10 resultados por página)
- ✅ **Búsqueda por**: Nombre, Apellidos, Número de ID
- ✅ **Configuración flexible** de parámetros

## 📦 Estructura de Archivos

```
ViewComponents/
├── BuscadorEstudianteViewComponent.cs
└── BuscadorEstudiante/
    └── Default.cshtml

ViewModels/
└── BuscadorEstudianteViewModel.cs

wwwroot/js/components/
└── buscador-estudiante.js

Controllers/
└── TestController.cs (endpoint BuscarEstudiantes)
```

## 🔧 Cómo Usar el Componente

### 1. Uso Básico en una Vista

```html
<!-- En tu archivo .cshtml -->
@{
    ViewData["Title"] = "Mi Página";
}

<div class="container">
    <h2>Seleccionar Estudiante</h2>
    
    <form>
        <div class="mb-3">
            <label for="estudianteSeleccionado" class="form-label">Estudiante</label>
            <div class="input-group">
                <select id="estudianteSeleccionado" name="estudianteId" class="form-select" required>
                    <option value="">-- Seleccione un estudiante --</option>
                </select>
                <button type="button" 
                        class="btn btn-outline-primary" 
                        data-bs-toggle="modal" 
                        data-bs-target="#modalBuscadorEstudiante">
                    <i class="fas fa-search"></i>
                </button>
            </div>
        </div>
    </form>
</div>

<!-- Invocar el ViewComponent -->
@await Component.InvokeAsync("BuscadorEstudiante", new { 
    modalId = "modalBuscadorEstudiante",
    selectId = "estudianteSeleccionado",
    incluirScripts = true
})
```

### 2. Múltiples Instancias en la Misma Página

```html
@{
    ViewData["Title"] = "Múltiples Buscadores";
}

<div class="container">
    <!-- Primer buscador -->
    <div class="mb-4">
        <h3>Estudiante Principal</h3>
        <div class="input-group">
            <select id="estudiantePrincipal" class="form-select">
                <option value="">-- Seleccione estudiante principal --</option>
            </select>
            <button type="button" class="btn btn-outline-primary" 
                    data-bs-toggle="modal" data-bs-target="#modalPrincipal">
                <i class="fas fa-search"></i>
            </button>
        </div>
    </div>

    <!-- Segundo buscador -->
    <div class="mb-4">
        <h3>Estudiante Suplente</h3>
        <div class="input-group">
            <select id="estudianteSuplente" class="form-select">
                <option value="">-- Seleccione estudiante suplente --</option>
            </select>
            <button type="button" class="btn btn-outline-primary" 
                    data-bs-toggle="modal" data-bs-target="#modalSuplente">
                <i class="fas fa-search"></i>
            </button>
        </div>
    </div>
</div>

<!-- ViewComponents - TODOS con incluirScripts=false -->
@await Component.InvokeAsync("BuscadorEstudiante", new { 
    modalId = "modalPrincipal",
    selectId = "estudiantePrincipal",
    titulo = "Buscar Estudiante Principal",
    incluirScripts = false
})

@await Component.InvokeAsync("BuscadorEstudiante", new { 
    modalId = "modalSuplente",
    selectId = "estudianteSuplente",
    titulo = "Buscar Estudiante Suplente",
    incluirScripts = false
})

@section Scripts {
    <!-- Cargar JavaScript una sola vez -->
    <script src="~/js/components/buscador-estudiante.js"></script>
    <script>
        $(document).ready(function() {
            // Inicializar todas las instancias
            const buscadorPrincipal = new BuscadorEstudiante('modalPrincipal', 'estudiantePrincipal');
            const buscadorSuplente = new BuscadorEstudiante('modalSuplente', 'estudianteSuplente');
            
            console.log('Múltiples buscadores inicializados');
        });
    </script>
}
```

## ⚙️ Parámetros de Configuración

| Parámetro | Tipo | Requerido | Valor por Defecto | Descripción |
|-----------|------|-----------|-------------------|-------------|
| `modalId` | string | ✅ Sí | - | ID único del modal Bootstrap |
| `selectId` | string | ✅ Sí | - | ID del elemento select a poblar |
| `titulo` | string | ❌ No | "Buscar Estudiante" | Título del modal |
| `placeholder` | string | ❌ No | "Escriba nombre, apellido o número de identificación..." | Placeholder del campo de búsqueda |
| `incluirScripts` | bool | ❌ No | `true` | Si incluir los scripts JavaScript |

### Ejemplo de Configuración Completa

```csharp
@await Component.InvokeAsync("BuscadorEstudiante", new { 
    modalId = "miModalPersonalizado",
    selectId = "miSelectPersonalizado",
    titulo = "Seleccionar Participante",
    placeholder = "Busque por nombre o cédula...",
    incluirScripts = true
})
```

## 🔌 Requisitos del Endpoint

El componente requiere un endpoint que responda a búsquedas. Por defecto usa:

```
GET /Test/BuscarEstudiantes?term={termino_busqueda}&page={numero_pagina}
```

### Formato de Respuesta Esperado

```json
{
  "resultados": [
    {
      "id": 123,
      "nombre": "Juan",
      "apellidos": "Pérez García",
      "numeroId": "12345678",
      "institucion": "Universidad XYZ",
      "periodo": "2024-2025"
    }
  ],
  "totalResultados": 50,
  "paginaActual": 1,
  "totalPaginas": 5,
  "resultadosPorPagina": 10
}
```

## 🛠️ Pasos para Usar en una Nueva Vista

### Paso 1: Verificar Dependencias

Asegúrate de que tu vista tenga acceso a:

- Bootstrap 5 (CSS y JS)
- jQuery
- Font Awesome (para iconos)

```html
<!-- En _Layout.cshtml o en tu vista -->
<link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet">
<link href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css" rel="stylesheet">
<script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
<script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/js/bootstrap.bundle.min.js"></script>
```

### Paso 2: Crear el Controlador de Búsqueda (si no existe)

```csharp
[HttpGet]
public async Task<IActionResult> BuscarEstudiantes(string term, int page = 1)
{
    // Tu lógica de búsqueda aquí
    var resultados = await _context.Estudiantes
        .Where(e => e.Nombre.Contains(term) || e.Apellidos.Contains(term))
        .Skip((page - 1) * 10)
        .Take(10)
        .Select(e => new {
            id = e.IdEstudiante,
            nombre = e.Nombre,
            apellidos = e.Apellidos,
            numeroId = e.NumeroId,
            institucion = e.Institucion
        })
        .ToListAsync();

    return Json(new {
        resultados = resultados,
        totalResultados = await _context.Estudiantes.CountAsync(),
        paginaActual = page,
        totalPaginas = (int)Math.Ceiling(totalResultados / 10.0),
        resultadosPorPagina = 10
    });
}
```

### Paso 3: Agregar el Componente a tu Vista

```html
@{
    ViewData["Title"] = "Mi Nueva Vista";
}

<div class="container mt-4">
    <h2>@ViewData["Title"]</h2>
    
    <!-- Tu formulario o contenido -->
    <form method="post">
        <div class="row">
            <div class="col-md-6">
                <label for="estudianteSeleccionado" class="form-label">
                    <i class="fas fa-user me-1"></i>
                    Estudiante
                </label>
                <div class="input-group">
                    <select id="estudianteSeleccionado" 
                            name="estudianteId" 
                            class="form-select" 
                            required>
                        <option value="">-- Seleccione un estudiante --</option>
                    </select>
                    <button type="button" 
                            class="btn btn-outline-primary" 
                            data-bs-toggle="modal" 
                            data-bs-target="#modalBuscador">
                        <i class="fas fa-search"></i>
                    </button>
                </div>
            </div>
        </div>
        
        <div class="mt-3">
            <button type="submit" class="btn btn-success">
                <i class="fas fa-save me-1"></i>
                Guardar
            </button>
        </div>
    </form>
</div>

<!-- ViewComponent -->
@await Component.InvokeAsync("BuscadorEstudiante", new { 
    modalId = "modalBuscador",
    selectId = "estudianteSeleccionado",
    titulo = "Buscar Estudiante",
    incluirScripts = true
})
```

### Paso 4: Configurar la Ruta del Endpoint (si es diferente)

Si tu endpoint está en un controlador diferente, actualiza la URL en el JavaScript:

```javascript
// Modificar en buscador-estudiante.js
const url = `/MiControlador/BuscarEstudiantes?term=${term}&page=${this.currentPage}`;
```

## 🎯 Casos de Uso Comunes

### Caso 1: Formulario de Inscripción

```html
<!-- Formulario con buscador de estudiante -->
<form method="post" action="/Inscripciones/Crear">
    <div class="row">
        <div class="col-md-6">
            <label class="form-label">Estudiante a Inscribir</label>
            <div class="input-group">
                <select id="estudiante" name="EstudianteId" class="form-select" required>
                    <option value="">-- Seleccione --</option>
                </select>
                <button type="button" class="btn btn-outline-primary" 
                        data-bs-toggle="modal" data-bs-target="#modalEstudiante">
                    <i class="fas fa-search"></i>
                </button>
            </div>
        </div>
        <div class="col-md-6">
            <label class="form-label">Curso</label>
            <select name="CursoId" class="form-select" required>
                <option value="">-- Seleccione --</option>
                <!-- Opciones de cursos -->
            </select>
        </div>
    </div>
    <button type="submit" class="btn btn-success mt-3">Inscribir</button>
</form>

@await Component.InvokeAsync("BuscadorEstudiante", new { 
    modalId = "modalEstudiante",
    selectId = "estudiante",
    incluirScripts = true
})
```

### Caso 2: Asignación de Grupos de Trabajo

```html
<div class="card">
    <div class="card-header">
        <h5>Formar Grupo de Trabajo</h5>
    </div>
    <div class="card-body">
        <!-- Líder del grupo -->
        <div class="mb-3">
            <label class="form-label">Líder del Grupo</label>
            <div class="input-group">
                <select id="lider" name="LiderId" class="form-select" required>
                    <option value="">-- Seleccione líder --</option>
                </select>
                <button type="button" class="btn btn-outline-primary" 
                        data-bs-toggle="modal" data-bs-target="#modalLider">
                    <i class="fas fa-search"></i>
                </button>
            </div>
        </div>

        <!-- Miembros del grupo -->
        <div class="mb-3">
            <label class="form-label">Miembro 1</label>
            <div class="input-group">
                <select id="miembro1" name="Miembro1Id" class="form-select">
                    <option value="">-- Seleccione miembro --</option>
                </select>
                <button type="button" class="btn btn-outline-secondary" 
                        data-bs-toggle="modal" data-bs-target="#modalMiembro1">
                    <i class="fas fa-search"></i>
                </button>
            </div>
        </div>

        <div class="mb-3">
            <label class="form-label">Miembro 2</label>
            <div class="input-group">
                <select id="miembro2" name="Miembro2Id" class="form-select">
                    <option value="">-- Seleccione miembro --</option>
                </select>
                <button type="button" class="btn btn-outline-secondary" 
                        data-bs-toggle="modal" data-bs-target="#modalMiembro2">
                    <i class="fas fa-search"></i>
                </button>
            </div>
        </div>
    </div>
</div>

<!-- ViewComponents - Múltiples instancias -->
@await Component.InvokeAsync("BuscadorEstudiante", new { 
    modalId = "modalLider", selectId = "lider", 
    titulo = "Seleccionar Líder", incluirScripts = false
})

@await Component.InvokeAsync("BuscadorEstudiante", new { 
    modalId = "modalMiembro1", selectId = "miembro1", 
    titulo = "Seleccionar Miembro 1", incluirScripts = false
})

@await Component.InvokeAsync("BuscadorEstudiante", new { 
    modalId = "modalMiembro2", selectId = "miembro2", 
    titulo = "Seleccionar Miembro 2", incluirScripts = false
})

@section Scripts {
    <script src="~/js/components/buscador-estudiante.js"></script>
    <script>
        $(document).ready(function() {
            new BuscadorEstudiante('modalLider', 'lider');
            new BuscadorEstudiante('modalMiembro1', 'miembro1');
            new BuscadorEstudiante('modalMiembro2', 'miembro2');
        });
    </script>
}
```

## 🎨 Personalización de Estilos

### Modificar Apariencia del Modal

```css
/* En tu archivo CSS personalizado */
.buscador-estudiante-modal .modal-dialog {
    max-width: 800px; /* Hacer el modal más ancho */
}

.buscador-estudiante-modal .list-group-item {
    transition: background-color 0.2s ease;
}

.buscador-estudiante-modal .list-group-item:hover {
    background-color: #f8f9fa;
    cursor: pointer;
}

/* Personalizar colores */
.buscador-estudiante-modal .btn-primary {
    background-color: #your-color;
    border-color: #your-color;
}
```

### Agregar Clase CSS Personalizada

```html
@await Component.InvokeAsync("BuscadorEstudiante", new { 
    modalId = "modalCustom",
    selectId = "estudiante",
    titulo = "Mi Buscador Personalizado",
    cssClass = "mi-modal-personalizado",
    incluirScripts = true
})
```

## 🐛 Solución de Problemas Comunes

### Problema 1: "BuscadorEstudiante is not defined"

**Causa**: JavaScript no se está cargando correctamente.

**Solución**:
```html
@section Scripts {
    <script src="~/js/components/buscador-estudiante.js"></script>
    <script>
        $(document).ready(function() {
            if (typeof BuscadorEstudiante !== 'undefined') {
                new BuscadorEstudiante('modalId', 'selectId');
            } else {
                console.error('BuscadorEstudiante no está disponible');
            }
        });
    </script>
}
```

### Problema 2: Multiple instancias no funcionan

**Causa**: `incluirScripts=true` en múltiples componentes.

**Solución**: Usar `incluirScripts=false` en todos excepto en el primero, o mejor aún, usar inicialización centralizada:

```html
<!-- TODOS los componentes con incluirScripts=false -->
@await Component.InvokeAsync("BuscadorEstudiante", new { 
    modalId = "modal1", selectId = "select1", incluirScripts = false
})
@await Component.InvokeAsync("BuscadorEstudiante", new { 
    modalId = "modal2", selectId = "select2", incluirScripts = false
})

@section Scripts {
    <script src="~/js/components/buscador-estudiante.js"></script>
    <script>
        $(document).ready(function() {
            new BuscadorEstudiante('modal1', 'select1');
            new BuscadorEstudiante('modal2', 'select2');
        });
    </script>
}
```

### Problema 3: Error HTTP 405 (Method Not Allowed)

**Causa**: El endpoint no acepta peticiones GET.

**Solución**: Verificar que el endpoint esté decorado con `[HttpGet]`:

```csharp
[HttpGet] // ✅ Importante
public async Task<IActionResult> BuscarEstudiantes(string term, int page = 1)
{
    // ...
}
```

### Problema 4: No se muestran resultados

**Causa**: Formato de respuesta incorrecto o datos vacíos.

**Solución**: Verificar formato JSON de respuesta:

```csharp
// ✅ Formato correcto
return Json(new {
    resultados = estudiantes,
    totalResultados = total,
    paginaActual = page,
    totalPaginas = totalPaginas,
    resultadosPorPagina = 10
});
```

## 📝 Notas Importantes

1. **IDs únicos**: Cada instancia debe tener `modalId` y `selectId` únicos
2. **Bootstrap 5**: El componente requiere Bootstrap 5 para funcionar correctamente
3. **jQuery**: Dependencia requerida para la funcionalidad
4. **Endpoint**: Debe seguir el formato de respuesta especificado
5. **Múltiples instancias**: Usar inicialización centralizada de JavaScript

## 🔄 Actualizaciones y Mantenimiento

Para actualizar el componente:

1. Modificar `BuscadorEstudianteViewComponent.cs` para nuevas funcionalidades
2. Actualizar `Default.cshtml` para cambios de UI
3. Actualizar `buscador-estudiante.js` para nueva lógica de cliente
4. Actualizar esta documentación

## 📞 Soporte

Para problemas o mejoras, revisar:

- Logs de la aplicación en la consola del navegador
- Logs del servidor ASP.NET Core
- Network tab en las herramientas de desarrollo del navegador

---

*Documentación generada el 26 de septiembre de 2025*