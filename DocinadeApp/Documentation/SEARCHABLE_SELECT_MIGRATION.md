# Migración de Select Tradicional a SearchableSelect

## Ejemplo: Reemplazar Select de Estudiantes

### ANTES - Select Tradicional (línea 156 en Index.cshtml)

```html
<div class="col-md-3">
    <div class="form-group">
        <label for="estudianteId">Estudiante (Legacy):</label>
        <select name="estudianteId" id="estudianteId" class="form-select"
                disabled="@(ViewBag.ShowAll == true)">
            <option value="">-- Todos los estudiantes --</option>
            @foreach (var estudiante in ViewBag.Estudiantes as List<SelectListItem>)
            {
                <option value="@estudiante.Value" selected="@(estudiante.Value == ViewBag.EstudianteSeleccionado?.ToString())">
                    @estudiante.Text
                </option>
            }
        </select>
    </div>
</div>
```

### DESPUÉS - SearchableSelect

#### Opción 1: TagHelper con lista local

```html
<div class="col-md-3">
    <div class="form-group">
        <label for="estudianteId">Estudiante:</label>
        <searchable-select name="estudianteId" 
                           id="estudianteId"
                           value="@ViewBag.EstudianteSeleccionado"
                           items="@(ViewBag.Estudiantes as List<SelectListItem>)"
                           placeholder="Buscar estudiante..."
                           allow-clear="true"
                           minimum-input-length="1"
                           disabled="@(ViewBag.ShowAll == true)"
                           css-class="form-select" />
    </div>
</div>
```

#### Opción 2: TagHelper con búsqueda remota (Recomendado)

```html
<div class="col-md-3">
    <div class="form-group">
        <label for="estudianteId">Estudiante:</label>
        <searchable-select name="estudianteId" 
                           id="estudianteId"
                           value="@ViewBag.EstudianteSeleccionado"
                           data-endpoint="/Estudiantes/Search"
                           placeholder="Buscar estudiante..."
                           allow-clear="true"
                           minimum-input-length="2"
                           disabled="@(ViewBag.ShowAll == true)"
                           css-class="form-select" />
    </div>
</div>
```

#### Opción 3: Con ViewComponent

```html
<div class="col-md-3">
    <div class="form-group">
        <label for="estudianteId">Estudiante:</label>
        @await Component.InvokeAsync("SearchableSelect", new {
            Name = "estudianteId",
            Id = "estudianteId", 
            Value = ViewBag.EstudianteSeleccionado,
            DataEndpoint = "/Estudiantes/Search",
            Placeholder = "Buscar estudiante...",
            AllowClear = true,
            MinimumInputLength = 2,
            Disabled = ViewBag.ShowAll == true,
            CssClass = "form-select"
        })
    </div>
</div>
```

## Ejemplo: Migración Completa en Views/Evaluaciones/Index.cshtml

### 1. Agregar assets al layout o vista

```html
@section Scripts {
    @await Html.PartialAsync("_SearchableSelectAssets")
    
    <!-- Scripts existentes de la vista -->
    <script>
        // JavaScript existente...
    </script>
}
```

### 2. Reemplazar selects existentes

```html
<!-- ANTES: Select tradicional estudiante legacy -->
<div class="col-md-3">
    <div class="form-group">
        <label for="estudianteId">Estudiante (Legacy):</label>
        <select name="estudianteId" id="estudianteId" class="form-select"
                disabled="@(ViewBag.ShowAll == true)">
            <option value="">-- Todos los estudiantes --</option>
            @foreach (var estudiante in ViewBag.Estudiantes as List<SelectListItem>)
            {
                <option value="@estudiante.Value" selected="@(estudiante.Value == ViewBag.EstudianteSeleccionado?.ToString())">
                    @estudiante.Text
                </option>
            }
        </select>
    </div>
</div>

<!-- DESPUÉS: SearchableSelect con búsqueda -->
<div class="col-md-3">
    <div class="form-group">
        <label for="estudianteId">Estudiante:</label>
        <searchable-select name="estudianteId" 
                           id="estudianteId"
                           value="@ViewBag.EstudianteSeleccionado"
                           data-endpoint="/Estudiantes/Search"
                           placeholder="Buscar estudiante..."
                           allow-clear="true"
                           minimum-input-length="2"
                           disabled="@(ViewBag.ShowAll == true)"
                           css-class="form-select" />
        <small class="form-text text-muted">Escriba al menos 2 caracteres para buscar</small>
    </div>
</div>
```

### 3. Actualizar selects en cascada

```html
<!-- Select estudiante en cascada -->
<div class="col-md-2">
    <div class="form-group">
        <label for="estudianteIdCascada">Estudiante:</label>
        <searchable-select name="estudianteId" 
                           id="estudianteIdCascada"
                           data-endpoint="/Evaluaciones/GetEstudiantesByGrupo"
                           placeholder="Seleccione un grupo primero"
                           allow-clear="true"
                           minimum-input-length="1"
                           disabled="@(ViewBag.ShowAll == true)"
                           css-class="form-select" />
    </div>
</div>

<!-- Select rúbrica en cascada -->
<div class="col-md-3">
    <div class="form-group">
        <label for="rubricaIdCascada">Rúbrica:</label>
        <searchable-select name="rubricaId" 
                           id="rubricaIdCascada"
                           data-endpoint="/Evaluaciones/GetRubricasByInstrumento"
                           placeholder="Seleccione un instrumento primero"
                           allow-clear="true"
                           minimum-input-length="0"
                           disabled="@(ViewBag.ShowAll == true)"
                           css-class="form-select" />
    </div>
</div>
```

### 4. Actualizar JavaScript para manejar dependencias

```javascript
// Reemplazar función loadEstudiantesByGrupo existente
function loadEstudiantesByGrupo(grupoId) {
    console.log('🔄 Cargando estudiantes para grupo:', grupoId);
    
    const estudianteSelect = document.getElementById('estudianteIdCascada');
    
    if (grupoId) {
        // Actualizar endpoint con filtro de grupo
        estudianteSelect.dataset.searchableExtra = JSON.stringify({ grupoId: grupoId });
        estudianteSelect.dataset.searchablePlaceholder = "Buscar estudiante del grupo...";
        
        // Limpiar selección actual
        SearchableSelect.setValue('#estudianteIdCascada', '');
        
        // Habilitar el select
        estudianteSelect.disabled = false;
    } else {
        // Limpiar y deshabilitar
        estudianteSelect.dataset.searchableExtra = '{}';
        estudianteSelect.dataset.searchablePlaceholder = "Seleccione un grupo primero";
        SearchableSelect.setValue('#estudianteIdCascada', '');
        estudianteSelect.disabled = true;
    }
    
    return Promise.resolve();
}

// Actualizar evento cuando cambia el grupo
$('#grupoId').on('change.cascade', function() {
    const grupoId = $(this).val();
    console.log('📋 Grupo seleccionado:', grupoId);
    
    window.cascadeLoadingInProgress = true;
    
    // Usar la nueva función
    loadEstudiantesByGrupo(grupoId)
        .then(() => {
            window.cascadeLoadingInProgress = false;
            console.log('✅ Estudiantes actualizados para el grupo');
        });
});
```

## Ejemplo: Form con Binding

### ViewModel

```csharp
public class EvaluacionCreateViewModel
{
    [Display(Name = "Estudiante")]
    [Required(ErrorMessage = "Debe seleccionar un estudiante")]
    public int EstudianteId { get; set; }
    
    [Display(Name = "Rúbrica")]
    [Required(ErrorMessage = "Debe seleccionar una rúbrica")]
    public int RubricaId { get; set; }
    
    [Display(Name = "Materias")]
    public List<int> MateriasIds { get; set; } = new List<int>();
}
```

### Vista Create

```html
<form asp-action="Create" method="post">
    <div class="row">
        <div class="col-md-6">
            <div class="form-group">
                <label asp-for="EstudianteId" class="form-label"></label>
                <searchable-select asp-for="EstudianteId"
                                   data-endpoint="/Estudiantes/Search"
                                   placeholder="Buscar y seleccionar estudiante..."
                                   allow-clear="true"
                                   minimum-input-length="2"
                                   required="true" />
                <span asp-validation-for="EstudianteId" class="text-danger"></span>
            </div>
        </div>
        
        <div class="col-md-6">
            <div class="form-group">
                <label asp-for="RubricaId" class="form-label"></label>
                <searchable-select asp-for="RubricaId"
                                   data-endpoint="/Rubricas/Search"
                                   placeholder="Buscar rúbrica..."
                                   allow-clear="true"
                                   minimum-input-length="1" />
                <span asp-validation-for="RubricaId" class="text-danger"></span>
            </div>
        </div>
    </div>
    
    <div class="form-group">
        <label asp-for="MateriasIds" class="form-label"></label>
        <searchable-select asp-for="MateriasIds"
                           data-endpoint="/Materias/Search"
                           multiple="true"
                           placeholder="Seleccionar materias..."
                           minimum-input-length="1" />
        <span asp-validation-for="MateriasIds" class="text-danger"></span>
    </div>
    
    <button type="submit" class="btn btn-primary">Crear Evaluación</button>
</form>
```

### Controlador

```csharp
[HttpGet]
public async Task<IActionResult> Create()
{
    var model = new EvaluacionCreateViewModel();
    return View(model);
}

[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Create(EvaluacionCreateViewModel model)
{
    if (!ModelState.IsValid)
    {
        return View(model);
    }
    
    // Procesar el modelo con valores seleccionados
    var evaluacion = new Evaluacion
    {
        IdEstudiante = model.EstudianteId,
        IdRubrica = model.RubricaId,
        // Procesar MateriasIds si es necesario
    };
    
    _context.Evaluaciones.Add(evaluacion);
    await _context.SaveChangesAsync();
    
    return RedirectToAction(nameof(Index));
}
```

## Beneficios de la Migración

### Ventajas del SearchableSelect

1. **Mejor UX**: Búsqueda en tiempo real vs scroll manual
2. **Performance**: Carga bajo demanda vs cargar todas las opciones
3. **Accesibilidad**: Mejor soporte para screen readers
4. **Escalabilidad**: Maneja miles de registros sin problemas
5. **Consistencia**: Mismo comportamiento en toda la app
6. **Mantenibilidad**: Componente reutilizable vs código duplicado

### Comparación

| Aspecto | Select Tradicional | SearchableSelect |
|---------|-------------------|------------------|
| Opciones | Limitado (~100) | Ilimitado |
| Búsqueda | No | Sí (local/remota) |
| Performance | Baja (muchas opciones) | Alta |
| UX | Básica | Avanzada |
| Accesibilidad | Limitada | Completa |
| Mantenimiento | Alto | Bajo |
| Código | Duplicado | Reutilizable |

### Migración Gradual

1. **Fase 1**: Implementar componente y documentación
2. **Fase 2**: Migrar formularios nuevos
3. **Fase 3**: Migrar formularios existentes uno por uno
4. **Fase 4**: Deprecar selects tradicionales
5. **Fase 5**: Eliminar código legacy

## Checklist de Migración

### Por Vista/Formulario

- [ ] Identificar todos los `<select>` a migrar
- [ ] Verificar si necesita búsqueda local o remota  
- [ ] Crear/verificar endpoint de búsqueda si es remoto
- [ ] Reemplazar HTML con TagHelper/ViewComponent
- [ ] Actualizar JavaScript si hay dependencias
- [ ] Probar validación cliente y servidor
- [ ] Probar preselección de valores
- [ ] Verificar accesibilidad
- [ ] Documentar cambios

### Global

- [ ] Incluir assets en layout principal
- [ ] Configurar TagHelper en _ViewImports.cshtml
- [ ] Crear endpoints de búsqueda para entidades principales
- [ ] Documentar patrones y ejemplos
- [ ] Capacitar al equipo
- [ ] Monitorear performance post-migración
