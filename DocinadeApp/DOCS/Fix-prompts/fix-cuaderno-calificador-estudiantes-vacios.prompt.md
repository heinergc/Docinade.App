---
mode: agent
description: Corrige el bug del Cuaderno Calificador que muestra tabla vacía aunque existen evaluaciones. Aplica el fix a CuadernoCalificadorDinamicoService.
---

# Fix: Cuaderno Calificador no muestra estudiantes

## Problema

El Cuaderno Calificador (`/CuadernoCalificador?materiaId=X&periodoAcademicoId=Y`) muestra
"No se encontraron evaluaciones" aunque existen evaluaciones, estudiantes e instrumentos asignados.

**Causa raíz**: `CuadernoCalificadorDinamicoViewModel` declara `EstudiantesRegulares = new List<>()` 
(lista vacía, NO null). La vista usa `??` para fallback:

```csharp
var estudiantesRegulares = Model.EstudiantesRegulares ?? Model.EstudiantesCalificaciones.Where(...);
```

El operador `??` solo actúa si el valor es `null`. Como es lista vacía, nunca cae al
fallback y siempre renderiza la tabla vacía aunque `EstudiantesCalificaciones` tenga datos.

## Solución

Aplica estos dos cambios en `Services/CuadernoCalificador/CuadernoCalificadorDinamicoService.cs`:

### Cambio 1 — Método `ProcesarCalificacionesEstudiantesAsync`

Localiza la inicialización del objeto `EstudianteCalificacionInfo` y agrega las
propiedades `RequiereACS` y `TipoAdecuacion`:

```csharp
// ANTES:
var estudianteInfo = new EstudianteCalificacionInfo
{
    EstudianteId = estudiante.IdEstudiante,
    NumeroIdentificacion = estudiante.NumeroId,
    NombreCompleto = $"{estudiante.Apellidos}, {estudiante.Nombre}",
    CorreoElectronico = estudiante.DireccionCorreo ?? ""
};

// DESPUES:
var estudianteInfo = new EstudianteCalificacionInfo
{
    EstudianteId = estudiante.IdEstudiante,
    NumeroIdentificacion = estudiante.NumeroId,
    NombreCompleto = $"{estudiante.Apellidos}, {estudiante.Nombre}",
    CorreoElectronico = estudiante.DireccionCorreo ?? "",
    RequiereACS = estudiante.TipoAdecuacion == "Significativa",
    TipoAdecuacion = estudiante.TipoAdecuacion ?? "NoPresenta"
};
```

### Cambio 2 — Método `GenerarCuadernoCalificadorAsync`

Después de la llamada a `ProcesarCalificacionesEstudiantesAsync`, agrega la
distribución en las dos listas separadas:

```csharp
// ANTES:
viewModel.EstudiantesCalificaciones = await ProcesarCalificacionesEstudiantesAsync(
    estudiantes, viewModel.Instrumentos, evaluaciones);

viewModel.EstudiantesConEvaluaciones = viewModel.EstudiantesCalificaciones.Count(e => e.NotasPorInstrumento.Any());

// DESPUES:
viewModel.EstudiantesCalificaciones = await ProcesarCalificacionesEstudiantesAsync(
    estudiantes, viewModel.Instrumentos, evaluaciones);

viewModel.EstudiantesRegulares = viewModel.EstudiantesCalificaciones
    .Where(e => !e.RequiereACS).ToList();
viewModel.EstudiantesACS = viewModel.EstudiantesCalificaciones
    .Where(e => e.RequiereACS).ToList();

viewModel.EstudiantesConEvaluaciones = viewModel.EstudiantesCalificaciones.Count(e => e.NotasPorInstrumento.Any());
```

## Verificación

Después de aplicar el fix:

1. Compilar: `dotnet build`
2. Ejecutar y abrir: `/CuadernoCalificador?materiaId=2&periodoAcademicoId=3`
3. La tabla debe mostrar todos los estudiantes del periodo con sus notas por instrumento.

## Archivos modificados

- `Services/CuadernoCalificador/CuadernoCalificadorDinamicoService.cs`
