# Ejemplo Práctico: Migración en Views/Evaluaciones/Index.cshtml

## Cambios Necesarios

### 1. Incluir Assets (en _Layout.cshtml o en la vista)

```html
@section Scripts {
    @* Incluir SearchableSelect assets *@
    @await Html.PartialAsync("_SearchableSelectAssets")
    
    @* Scripts existentes de la vista *@
    <script>
        // Todo el JavaScript existente de la vista...
    </script>
}
```

### 2. Reemplazar el Select de la Línea 156

**ANTES:**
```html
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
```

**DESPUÉS (Opción A - Lista Local):**
```html
<searchable-select name="estudianteId" 
                   id="estudianteId"
                   value="@ViewBag.EstudianteSeleccionado"
                   items="@(ViewBag.Estudiantes as List<SelectListItem>)"
                   placeholder="Buscar estudiante..."
                   allow-clear="true"
                   minimum-input-length="1"
                   disabled="@(ViewBag.ShowAll == true)"
                   css-class="form-select" />
```

**DESPUÉS (Opción B - Búsqueda Remota - RECOMENDADO):**
```html
<searchable-select name="estudianteId" 
                   id="estudianteId"
                   value="@ViewBag.EstudianteSeleccionado"
                   data-endpoint="/Estudiantes/Search"
                   placeholder="Buscar estudiante..."
                   allow-clear="true"
                   minimum-input-length="2"
                   disabled="@(ViewBag.ShowAll == true)"
                   css-class="form-select" />
```

### 3. Actualizar Otros Selects en la Vista

#### Select Estudiante Cascada (línea ~125)

**ANTES:**
```html
<select name="estudianteId" id="estudianteIdCascada" class="form-select"
        disabled="@(ViewBag.ShowAll == true)">
    <option value="">-- Todos los estudiantes --</option>
    @if (filtroVm?.Estudiantes != null)
    {
        @foreach (var estudiante in filtroVm.Estudiantes)
        {
            <option value="@estudiante.Value" selected="@(estudiante.Value == filtroVm.EstudianteId?.ToString())">
                @estudiante.Text
            </option>
        }
    }
</select>
```

**DESPUÉS:**
```html
<searchable-select name="estudianteId" 
                   id="estudianteIdCascada"
                   value="@filtroVm?.EstudianteId"
                   data-endpoint="/Evaluaciones/GetEstudiantesByGrupo"
                   placeholder="Seleccione un grupo primero"
                   allow-clear="true"
                   minimum-input-length="1"
                   disabled="@(ViewBag.ShowAll == true)"
                   css-class="form-select" />
```

#### Select Rúbrica Cascada

**ANTES:**
```html
<select name="rubricaId" id="rubricaIdCascada" class="form-select"
        disabled="@(ViewBag.ShowAll == true)">
    <option value="">-- Todas las rúbricas --</option>
    @if (filtroVm?.Rubricas != null)
    {
        @foreach (var rubrica in filtroVm.Rubricas)
        {
            <option value="@rubrica.Value" selected="@(rubrica.Value == filtroVm.RubricaId?.ToString())">
                @rubrica.Text
            </option>
        }
    }
</select>
```

**DESPUÉS:**
```html
<searchable-select name="rubricaId" 
                   id="rubricaIdCascada"
                   value="@filtroVm?.RubricaId"
                   data-endpoint="/Evaluaciones/GetRubricasByInstrumento"
                   placeholder="Seleccione un instrumento primero"
                   allow-clear="true"
                   minimum-input-length="0"
                   disabled="@(ViewBag.ShowAll == true)"
                   css-class="form-select" />
```

### 4. Actualizar JavaScript para Cascadas

En el JavaScript existente, reemplazar las funciones de carga:

```javascript
// REEMPLAZAR función loadEstudiantesByGrupo
function loadEstudiantesByGrupo(grupoId) {
    console.log('🔄 Cargando estudiantes para grupo:', grupoId);
    
    const estudianteSelect = document.getElementById('estudianteIdCascada');
    
    if (grupoId) {
        // Actualizar configuración del SearchableSelect
        estudianteSelect.dataset.searchableExtra = JSON.stringify({ grupoId: grupoId });
        estudianteSelect.dataset.searchablePlaceholder = "Buscar estudiante del grupo...";
        estudianteSelect.disabled = false;
        
        // Limpiar selección
        SearchableSelect.setValue('#estudianteIdCascada', '');
    } else {
        // Resetear a estado inicial
        estudianteSelect.dataset.searchableExtra = '{}';
        estudianteSelect.dataset.searchablePlaceholder = "Seleccione un grupo primero";
        estudianteSelect.disabled = true;
        SearchableSelect.setValue('#estudianteIdCascada', '');
    }
    
    console.log('✅ Estudiantes configurados para el grupo');
    return Promise.resolve();
}

// REEMPLAZAR función loadRubricasByInstrumento
function loadRubricasByInstrumento(instrumentoId) {
    console.log('🔄 Cargando rúbricas para instrumento:', instrumentoId);
    
    const rubricaSelect = document.getElementById('rubricaIdCascada');
    
    if (instrumentoId) {
        // Actualizar configuración
        rubricaSelect.dataset.searchableExtra = JSON.stringify({ instrumentoEvaluacionId: instrumentoId });
        rubricaSelect.dataset.searchablePlaceholder = "Buscar rúbrica del instrumento...";
        rubricaSelect.disabled = false;
        
        // Limpiar selección
        SearchableSelect.setValue('#rubricaIdCascada', '');
    } else {
        // Resetear
        rubricaSelect.dataset.searchableExtra = '{}';
        rubricaSelect.dataset.searchablePlaceholder = "Seleccione un instrumento primero";
        rubricaSelect.disabled = true;
        SearchableSelect.setValue('#rubricaIdCascada', '');
    }
    
    console.log('✅ Rúbricas configuradas para el instrumento');
    return Promise.resolve();
}
```

### 5. Verificar Endpoints en EvaluacionesController

Asegurarse de que estos métodos existan:

```csharp
[HttpGet]
public async Task<IActionResult> GetEstudiantesByGrupo(int? grupoId, string q, int? id)
{
    try
    {
        var query = _context.Estudiantes.Include(e => e.PeriodoAcademico).AsQueryable();
        
        if (id.HasValue)
        {
            var estudiante = await query
                .Where(e => e.IdEstudiante == id.Value)
                .Select(e => new { id = e.IdEstudiante, text = $"{e.Apellidos}, {e.Nombre}" })
                .FirstOrDefaultAsync();
            return Json(estudiante != null ? new[] { estudiante } : new object[0]);
        }

        if (grupoId.HasValue)
        {
            var estudiantesEnGrupo = await _context.EstudianteGrupos
                .Where(eg => eg.GrupoId == grupoId.Value)
                .Select(eg => eg.EstudianteId)
                .ToListAsync();
            query = query.Where(e => estudiantesEnGrupo.Contains(e.IdEstudiante));
        }

        if (!string.IsNullOrWhiteSpace(q))
        {
            q = q.ToLowerInvariant();
            query = query.Where(e => 
                e.Nombre.ToLower().Contains(q) || 
                e.Apellidos.ToLower().Contains(q));
        }

        var estudiantes = await query
            .Take(20)
            .Select(e => new { id = e.IdEstudiante, text = $"{e.Apellidos}, {e.Nombre}" })
            .ToListAsync();

        return Json(estudiantes);
    }
    catch (Exception ex)
    {
        return Json(new { error = ex.Message });
    }
}

[HttpGet]
public async Task<IActionResult> GetRubricasByInstrumento(int? instrumentoEvaluacionId, string q, int? id)
{
    try
    {
        var query = _context.InstrumentoRubricas
            .Include(ir => ir.Rubrica)
            .AsQueryable();
        
        if (id.HasValue)
        {
            var rubrica = await query
                .Where(ir => ir.RubricaId == id.Value)
                .Select(ir => new { id = ir.RubricaId, text = ir.Rubrica.Titulo })
                .FirstOrDefaultAsync();
            return Json(rubrica != null ? new[] { rubrica } : new object[0]);
        }

        if (instrumentoEvaluacionId.HasValue)
        {
            query = query.Where(ir => ir.InstrumentoEvaluacionId == instrumentoEvaluacionId.Value);
        }

        if (!string.IsNullOrWhiteSpace(q))
        {
            q = q.ToLowerInvariant();
            query = query.Where(ir => ir.Rubrica.Titulo.ToLower().Contains(q));
        }

        var rubricas = await query
            .Take(20)
            .Select(ir => new { id = ir.RubricaId, text = ir.Rubrica.Titulo })
            .ToListAsync();

        return Json(rubricas);
    }
    catch (Exception ex)
    {
        return Json(new { error = ex.Message });
    }
}
```

## Resultado Final

### Beneficios Inmediatos

1. **Mejor UX**: Los usuarios pueden buscar estudiantes escribiendo nombre o apellido
2. **Performance**: No se cargan todos los estudiantes al inicio de la página
3. **Escalabilidad**: Funciona con miles de estudiantes sin problemas
4. **Consistencia**: Comportamiento uniforme en toda la aplicación

### Vista Actualizada

La vista tendrá selects con búsqueda que:
- Buscan en tiempo real mientras el usuario escribe
- Muestran resultados relevantes inmediatamente
- Mantienen el estado de filtros durante navegación
- Proporcionan mejor feedback visual

### Implementación Gradual

1. **Inmediato**: Cambiar solo el select de la línea 156
2. **Fase 2**: Migrar selects en cascada
3. **Fase 3**: Aplicar a otras vistas de la aplicación

### Código de Prueba

Para probar rápidamente, solo cambiar la línea 156:

```html
<!-- Reemplazar esta línea -->
<select name="estudianteId" id="estudianteId" class="form-select"

<!-- Por esta -->
<searchable-select name="estudianteId" id="estudianteId" 
                   data-endpoint="/Estudiantes/Search"
                   placeholder="Buscar estudiante..."
                   allow-clear="true"
                   minimum-input-length="2"
                   css-class="form-select"
```

Y agregar los assets al final de la vista:

```html
@section Scripts {
    @await Html.PartialAsync("_SearchableSelectAssets")
    <!-- Todo el JavaScript existente -->
}
```
