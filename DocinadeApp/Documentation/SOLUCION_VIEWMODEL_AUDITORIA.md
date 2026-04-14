# Solución: Error de ViewModel en Auditoría

## Problema Identificado
```
InvalidOperationException: The model item passed into the ViewDataDictionary is of type 'RubricasApp.Web.ViewModels.Auditoria.AuditoriaIndexViewModel', but this ViewDataDictionary instance requires a model item of type 'RubricasApp.Web.ViewModels.Auditoria.HistorialAuditoriaViewModel'.
```

## Causa del Error
- La vista `Views/Auditoria/Index.cshtml` esperaba un modelo de tipo `HistorialAuditoriaViewModel`
- El controlador `AuditoriaController` estaba enviando un modelo de tipo `AuditoriaIndexViewModel`
- Incompatibilidad entre los tipos de ViewModels

## Solución Aplicada

### 1. Actualización del Controller - AuditoriaController.cs

#### Método Index corregido:
```csharp
public async Task<IActionResult> Index(int? tipoOperacion, string? usuario, DateTime? fechaDesde, DateTime? fechaHasta, int pageNumber = 1, int pageSize = 20)
{
    try
    {
        var historial = await _auditoriaService.ObtenerHistorialAsync(tipoOperacion, usuario, fechaDesde, fechaHasta, pageNumber, pageSize);
        var viewModel = new HistorialAuditoriaViewModel
        {
            TablaAfectada = "Todas",
            RegistroId = 0,
            TituloRegistro = "Historial General de Auditoría",
            Operaciones = historial.Select(MapToAuditoriaOperacionViewModel).ToList(),
            TotalOperaciones = historial.Count(),
            FechaDesde = fechaDesde,
            FechaHasta = fechaHasta
        };

        ViewBag.Filtros = new FiltrosAuditoriaViewModel
        {
            TipoOperacion = tipoOperacion?.ToString(),
            UsuarioId = usuario,
            FechaDesde = fechaDesde,
            FechaHasta = fechaHasta
        };

        return View(viewModel);
    }
    catch
    {
        ModelState.AddModelError(string.Empty, "Error al cargar el historial de auditoría");
        return View(new HistorialAuditoriaViewModel());
    }
}
```

#### Método HistorialGrupo corregido:
```csharp
public async Task<IActionResult> HistorialGrupo(int grupoId)
{
    try
    {
        var historial = await _auditoriaService.ObtenerHistorialTablaAsync("GruposEstudiantes", grupoId.ToString());
        var viewModel = new HistorialAuditoriaViewModel
        {
            TablaAfectada = "GruposEstudiantes",
            RegistroId = grupoId,
            TituloRegistro = $"Historial del Grupo ID: {grupoId}",
            Operaciones = historial.Select(MapToAuditoriaOperacionViewModel).ToList(),
            TotalOperaciones = historial.Count()
        };

        return View("Index", viewModel);
    }
    catch
    {
        ModelState.AddModelError(string.Empty, "Error al cargar el historial del grupo");
        return RedirectToAction("Index");
    }
}
```

### 2. Nuevo Método de Mapeo Agregado

```csharp
private AuditoriaOperacionViewModel MapToAuditoriaOperacionViewModel(RubricasApp.Web.Models.AuditoriaOperacion operacion)
{
    var usuario = (ApplicationUser?)operacion.Usuario;
    return new AuditoriaOperacionViewModel
    {
        Id = operacion.Id,
        TipoOperacion = operacion.TipoOperacion,
        TipoOperacionDisplay = GetTipoOperacionDisplay(operacion.TipoOperacion),
        Descripcion = operacion.Descripcion,
        Motivo = operacion.Motivo,
        FechaOperacion = operacion.FechaOperacion,
        UsuarioNombre = usuario?.Nombre ?? "Usuario desconocido",
        UsuarioEmail = usuario?.Email ?? string.Empty,
        OperacionExitosa = operacion.OperacionExitosa,
        MensajeError = operacion.MensajeError,
        DireccionIP = operacion.DireccionIP,
        UserAgent = operacion.UserAgent,
        TieneDatosAnteriores = !string.IsNullOrEmpty(operacion.DatosAnteriores),
        TieneDatosNuevos = !string.IsNullOrEmpty(operacion.DatosNuevos),
        DatosAnteriores = operacion.DatosAnteriores,
        DatosNuevos = operacion.DatosNuevos,
        IconoOperacion = GetIconoOperacion(operacion.TipoOperacion),
        ColorOperacion = GetColorOperacion(operacion.TipoOperacion)
    };
}
```

### 3. Métodos Helper Agregados

```csharp
private string GetTipoOperacionDisplay(string tipoOperacion)
private string GetIconoOperacion(string tipoOperacion)
private string GetColorOperacion(string tipoOperacion)
```

## ViewModels Involucrados

### HistorialAuditoriaViewModel (Requerido por la vista)
```csharp
public class HistorialAuditoriaViewModel
{
    public string TablaAfectada { get; set; } = string.Empty;
    public int RegistroId { get; set; }
    public string TituloRegistro { get; set; } = string.Empty;
    public List<AuditoriaOperacionViewModel> Operaciones { get; set; } = new List<AuditoriaOperacionViewModel>();
    public int TotalOperaciones { get; set; }
    public DateTime? FechaDesde { get; set; }
    public DateTime? FechaHasta { get; set; }
}
```

### AuditoriaOperacionViewModel (Para items individuales)
```csharp
public class AuditoriaOperacionViewModel
{
    public int Id { get; set; }
    public string TipoOperacion { get; set; } = string.Empty;
    public string TipoOperacionDisplay { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    // ... otras propiedades
}
```

## Estado Actual
✅ **RESUELTO**: El error de incompatibilidad de ViewModels ha sido corregido
✅ **COMPILACIÓN**: El proyecto compila sin errores
✅ **FUNCIONALIDAD**: Los métodos de auditoría ahora usan los ViewModels correctos

## Archivos Modificados
- `Controllers/AuditoriaController.cs` - Métodos Index y HistorialGrupo actualizados
- Se agregaron métodos helper para mapeo de operaciones de auditoría

## Próximos Pasos
1. Verificar que la aplicación se ejecute sin errores
2. Probar la funcionalidad de auditoría en el navegador
3. Confirmar que ambos métodos (Index y HistorialGrupo) funcionen correctamente
