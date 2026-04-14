# Análisis de Arquitectura: N-Capas vs Monolito Actual

**Fecha:** 13 de abril de 2026  
**Contexto:** ~300 estudiantes, ~40 administrativos

---

## Decisión: Mantener la arquitectura actual

**El proyecto no requiere migración a n-capas en este momento.**  
La inversión de 130–200h no está justificada para la escala actual.

---

## Estado Actual del Proyecto

| Componente | Cantidad | Observación |
|---|---|---|
| Controladores | 53 | Algunos con archivos `-bk.cs` y `.backup` |
| Modelos | 57 | Mezclados: entidades + ViewModels |
| Servicios | 47 | Usan `DbContext` directamente, sin repositorios |
| Vistas | 242 | Razor Views + Areas |
| Migraciones | 30 | Acopladas al proyecto principal |
| DTOs | 3 | Muy pocos para el tamaño del proyecto |
| Interfaces | ~14 | 2 en `/Interfaces`, resto dispersas en `/Services` |

**Ventajas que ya tiene el monolito actual:**
- AutoMapper configurado
- Interfaces + Servicios con inyección de dependencias
- Organización por módulos dentro de `/Services`

---

## Por qué NO vale migrar a N-Capas ahora

| Problema que resuelve n-layers | ¿Lo tiene este proyecto? |
|---|---|
| Equipos de 5+ devs trabajando en paralelo | No — proyecto personal/pequeño equipo |
| Necesidad de cambiar la BD sin tocar lógica | No — SQL Server fijo |
| Múltiples frontends (Web + API + Mobile) | No — solo MVC |
| Pruebas unitarias a gran escala | No — `Tests/` prácticamente vacío |
| Microservicios en el futuro | Sin señales de eso |

> **Regla práctica:** Si un solo desarrollador puede entender el proyecto completo en un día, la arquitectura es suficientemente buena.

---

## Referencia: Arquitectura N-Capas (si se necesitara en el futuro)

```
RubricasApp.Domain          ← Entidades puras, interfaces de dominio
RubricasApp.Application     ← Servicios de aplicación, DTOs, interfaces
RubricasApp.Infrastructure  ← EF Core, DbContext, Email, PDF, migraciones
RubricasApp.Web             ← Controllers, Views, ViewModels (presentación)
```

**Costo estimado de migración:**

| Fase | Horas mínimas | Horas máximas |
|---|---|---|
| Setup solución (3 nuevos .csproj) | 4 | 8 |
| Domain layer (~30 entidades) | 16 | 24 |
| Application layer (~35 servicios + DTOs) | 40 | 60 |
| Infrastructure layer (DbContext, Email, PDF) | 20 | 30 |
| Actualizar Presentación (~53 controladores) | 20 | 32 |
| Testing y resolución de dependencias rotas | 30 | 50 |
| **TOTAL** | **130 h** | **204 h** |

Con CQRS/MediatR adicional: sumar +40–80 horas.

---

## Plan de Mejoras Recomendadas (~20–30h total)

Estas mejoras dan el 80% del beneficio con el 15% del costo de una migración completa.

### Tarea 1 — Limpiar archivos residuales (~2h)

Eliminar los siguientes archivos que ya no se compilan pero generan ruido:

- [ ] `Controllers/EvaluacionesController-bk.cs`
- [ ] `Controllers/EvaluacionesController_Clean.cs`
- [ ] `Controllers/ConfiguracionController.cs.backup`
- [ ] `Controllers/ImportarRubricaController.cs.backup`
- [ ] `Controllers/ImportarRubricaController_updated.cs`
- [ ] `Models/Rubrica.cs.bak`
- [ ] `Models/Materia.cs.bak`
- [ ] `Data/RubricasDbContext.cs.bak`

> Verificar que ninguno tenga código útil antes de eliminar.

---

### Tarea 2 — Completar DTOs donde faltan (~8h)

Actualmente solo hay 3 DTOs para 57 modelos. Los módulos críticos deben tener DTOs propios:

- [ ] `RubricaDto` / `RubricaCreateDto` / `RubricaUpdateDto`
- [ ] `EvaluacionDto` / `EvaluacionCreateDto`
- [ ] `EstudianteDto`
- [ ] `GrupoEstudianteDto`
- [ ] `PeriodoAcademicoDto`

Los controladores de Evaluaciones y Rúbricas son los más urgentes.

---

### Tarea 3 — Consolidar interfaces (~3h)

Las interfaces están en dos lugares distintos:
- `/Interfaces/IAuditable.cs`, `IUserOwned.cs`
- `/Services/Academic/IServices.cs`, `/Services/Grupos/IGrupoEstudianteService.cs`, etc.

**Acción:** Mantener el patrón actual de interfaz junto a su implementación por módulo (ya funciona bien en `Academic/`, `Grupos/`, `CuadernoCalificador/`). Aplicarlo donde falta.

---

### Tarea 4 — Agregar interfaces a servicios que no las tienen (~5h)

Los siguientes servicios no tienen interfaz, lo que dificulta reemplazarlos o testearlos:

- [ ] `ConductaService.cs` → crear `IConductaService`
- [ ] `HorarioService.cs` → crear `IHorarioService`
- [ ] `EstudianteImportService.cs` → crear `IEstudianteImportService`
- [ ] `AsistenciaLeccionService.cs` → crear `IAsistenciaLeccionService`
- [ ] `CedulaCostaRicaService.cs` → crear `ICedulaCostaRicaService`

---

## Riesgos a tener presentes (si en algún momento se migra)

1. **ASP.NET Identity acoplado** — `ApplicationUser` vive en `/Models/Identity` y `RubricasDbContext` hereda de `IdentityDbContext`. Es el mayor riesgo técnico de una migración.

2. **DbContext sin repositorios** — Los 47 servicios inyectan `RubricasDbContext` directamente. Requiere introducir `IRepository<T>` o `IUnitOfWork` antes de separar proyectos.

3. **Sin cobertura de tests** — Refactorizar sin tests unitarios es riesgoso. La carpeta `Tests/` existe pero está prácticamente vacía.

---

## Prioridad de ejecución

```
[Hoy]    Tarea 1: Limpiar archivos residuales        ~2h
[Corto]  Tarea 4: Interfaces a servicios sin ellas   ~5h
[Medio]  Tarea 2: Completar DTOs críticos            ~8h
[Medio]  Tarea 3: Consolidar interfaces              ~3h
```

---

*Última revisión: 13 de abril de 2026*
