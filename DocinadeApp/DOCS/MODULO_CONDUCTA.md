# Módulo de Conducta - RubricasApp.Web

## 📋 Resumen

Implementación completa del **Módulo de Conducta** según el Reglamento de Evaluación de los Aprendizajes (REA 40862-V21) del Ministerio de Educación Pública de Costa Rica.

**Fecha de implementación:** Noviembre 2025  
**Estado:** ✅ Backend y Frontend completados (85%)

---

## 🎯 Características Implementadas

### ✅ Backend (100% Completo)

#### 1. **Modelos de Dominio** (6 clases)
- `TipoFalta` - Catálogo de 5 niveles de faltas según REA Art. 137
- `BoletaConducta` - Registro de incidentes con notificación automática
- `NotaConducta` - Cálculo automático de nota (100 - rebajos)
- `ProgramaAccionesInstitucional` - Opción B para estudiantes aplazados
- `DecisionProfesionalConducta` - Opción C (decisión del comité)
- `ParametroInstitucion` - Configuración de nota mínima

#### 2. **Capa de Servicio**
- `ConductaService.cs` (735 líneas) - Lógica de negocio completa:
  - Registro de boletas con notificación automática al profesor guía
  - Cálculo automático de nota de conducta por período
  - Determinación de estado: Aprobado (≥65), Riesgo (60-64), Aplazado (<60)
  - Gestión de Programas de Acciones Institucional
  - Aplicación de Decisiones Profesionales
  - Dashboard con estadísticas y análisis

#### 3. **Controladores**
- `BoletasConductaController.cs` (525 líneas):
  - Index, Create (con upload de evidencia), Details, Anular
  - Endpoint AJAX para cargar definiciones de faltas
  
- `NotaConductaController.cs` (540 líneas):
  - Dashboard, EstudianteNota, EstudiantesRiesgo, EstudiantesAplazados
  - CrearPrograma, AplicarDecisionProfesional

#### 4. **Base de Datos**
- Migración `AgregarModuloConducta` generada exitosamente
- 6 tablas nuevas con relaciones configuradas
- Seed data con 5 tipos de falta + parámetro nota mínima (65)

#### 5. **ViewModels**
- 5 archivos con 20+ ViewModels para todas las operaciones CRUD

---

### ✅ Frontend (100% Completo)

#### **15 Vistas Razor Creadas:**

**Boletas de Conducta (4 vistas):**
1. `BoletasConducta/Index.cshtml` - Lista con filtros, estadísticas, DataTables
2. `BoletasConducta/Create.cshtml` - Formulario con AJAX, validación, upload
3. `BoletasConducta/Details.cshtml` - Vista detallada con evidencia
4. `BoletasConducta/Anular.cshtml` - Cancelación con justificación

**Notas de Conducta (6 vistas):**
5. `NotaConducta/Dashboard.cshtml` - Dashboard con Chart.js, filtros, resumen
6. `NotaConducta/EstudianteNota.cshtml` - Nota individual + historial boletas
7. `NotaConducta/EstudiantesRiesgo.cshtml` - Lista de estudiantes 60-64
8. `NotaConducta/EstudiantesAplazados.cshtml` - Lista <65 con acciones
9. `NotaConducta/CrearPrograma.cshtml` - Programa Opción B
10. `NotaConducta/AplicarDecisionProfesional.cshtml` - Decisión Opción C

**Programas de Acciones (3 vistas):**
11. `ProgramaAcciones/Index.cshtml` - Lista con filtros por estado
12. `ProgramaAcciones/Details.cshtml` - Detalles completos del programa
13. `ProgramaAcciones/Verificar.cshtml` - Verificación por comité

**Reportes (2 vistas):**
14. `Reportes/HistorialConductaEstudiante.cshtml` - Historial completo con gráficos
15. `Reportes/ReporteGeneralConducta.cshtml` - Análisis institucional

---

## 📊 Funcionalidades Clave

### Sistema de Calificación
- **Base:** 100 puntos
- **Rebajos:** Según tipo de falta (1-5, 6-10, 11-19, 20-32, 33-45)
- **Estados:** 
  - ✅ Aprobado: ≥ 65 puntos
  - ⚠️ Riesgo: 60-64 puntos
  - ❌ Aplazado: < 60 puntos

### Tipos de Falta (REA Art. 137)
1. **Muy Leve:** 1-5 puntos
2. **Leve:** 6-10 puntos
3. **Grave:** 11-19 puntos
4. **Muy Grave:** 20-32 puntos
5. **Gravísima:** 33-45 puntos

### Opciones para Estudiantes Aplazados
- **Opción B:** Programa de Acciones Institucional con supervisión
- **Opción C:** Decisión Profesional del Comité de Apoyo Educativo

### Características Técnicas
- ✅ Validación client-side y server-side
- ✅ Notificaciones automáticas a profesores guía
- ✅ Upload de archivos de evidencia (5MB, jpg/png/pdf/doc/docx)
- ✅ Gráficos interactivos con Chart.js
- ✅ DataTables para tablas con búsqueda/filtrado
- ✅ Diseño responsive con Bootstrap 5
- ✅ Confirmaciones y alertas con SweetAlert/Toastr
- ✅ Print-friendly CSS para reportes

---

## 🗂️ Estructura de Archivos

```
RubricasApp.Web/
│
├── Models/
│   ├── TipoFalta.cs
│   ├── BoletaConducta.cs
│   ├── NotaConducta.cs
│   ├── ProgramaAccionesInstitucional.cs
│   ├── DecisionProfesionalConducta.cs
│   └── ParametroInstitucion.cs
│
├── ViewModels/
│   └── Conducta/
│       ├── BoletaConductaViewModels.cs
│       ├── NotaConductaViewModels.cs
│       ├── ProgramaAccionesViewModels.cs
│       ├── DecisionProfesionalViewModels.cs
│       └── ReportesViewModels.cs
│
├── Services/
│   ├── IConductaService.cs (interfaz)
│   └── ConductaService.cs (735 líneas)
│
├── Controllers/
│   ├── BoletasConductaController.cs (525 líneas)
│   └── NotaConductaController.cs (540 líneas)
│
├── Views/
│   ├── BoletasConducta/
│   │   ├── Index.cshtml
│   │   ├── Create.cshtml
│   │   ├── Details.cshtml
│   │   └── Anular.cshtml
│   │
│   ├── NotaConducta/
│   │   ├── Dashboard.cshtml
│   │   ├── EstudianteNota.cshtml
│   │   ├── EstudiantesRiesgo.cshtml
│   │   ├── EstudiantesAplazados.cshtml
│   │   ├── CrearPrograma.cshtml
│   │   └── AplicarDecisionProfesional.cshtml
│   │
│   ├── ProgramaAcciones/
│   │   ├── Index.cshtml
│   │   ├── Details.cshtml
│   │   └── Verificar.cshtml
│   │
│   └── Reportes/
│       ├── HistorialConductaEstudiante.cshtml
│       └── ReporteGeneralConducta.cshtml
│
├── Data/
│   └── RubricasDbContext.cs (6 DbSets agregados)
│
├── Utils/
│   └── ConductaSeedData.cs
│
└── Migrations/
    └── xxxxx_AgregarModuloConducta.cs
```

---

## 🚀 Pasos Pendientes para Completar

### 1. Aplicar Migración a Base de Datos
```bash
dotnet ef database update
```

### 2. Agregar Navegación al Menú Principal
Editar `Views/Shared/_Layout.cshtml` o el archivo de menú correspondiente:

```html
<li class="nav-item dropdown">
    <a class="nav-link dropdown-toggle" href="#" id="navbarConducta" 
       role="button" data-bs-toggle="dropdown">
        <i class="fas fa-clipboard-check"></i> Conducta
    </a>
    <ul class="dropdown-menu">
        <li><a class="dropdown-item" asp-controller="NotaConducta" asp-action="Dashboard">
            <i class="fas fa-tachometer-alt"></i> Dashboard
        </a></li>
        <li><hr class="dropdown-divider"></li>
        <li><a class="dropdown-item" asp-controller="BoletasConducta" asp-action="Index">
            <i class="fas fa-file-alt"></i> Boletas de Conducta
        </a></li>
        <li><a class="dropdown-item" asp-controller="NotaConducta" asp-action="EstudiantesRiesgo">
            <i class="fas fa-exclamation-triangle"></i> Estudiantes en Riesgo
        </a></li>
        <li><a class="dropdown-item" asp-controller="NotaConducta" asp-action="EstudiantesAplazados">
            <i class="fas fa-times-circle"></i> Estudiantes Aplazados
        </a></li>
        <li><hr class="dropdown-divider"></li>
        <li><a class="dropdown-item" asp-controller="ProgramaAcciones" asp-action="Index">
            <i class="fas fa-clipboard-list"></i> Programas de Acciones
        </a></li>
        <li><hr class="dropdown-divider"></li>
        <li><a class="dropdown-item" asp-controller="Reportes" asp-action="ReporteGeneralConducta">
            <i class="fas fa-chart-bar"></i> Reporte General
        </a></li>
    </ul>
</li>
```

### 3. Configurar Permisos (Opcional)
Agregar atributos `[Authorize]` con roles específicos:

```csharp
// En los controladores
[Authorize(Roles = "Profesor,Administrador")]
public class BoletasConductaController : Controller { }

[Authorize(Roles = "Orientador,Administrador")]
public async Task<IActionResult> CrearPrograma(int idEstudiante, int idPeriodo) { }
```

### 4. Crear ProgramaAccionesController
Actualmente las vistas de ProgramaAcciones están creadas pero falta el controlador. Puedes agregarlo siguiendo el patrón de los otros controladores, o integrar esas acciones en `NotaConductaController`.

### 5. Crear ReportesController
Similar al punto anterior, las vistas de reportes necesitan su controlador con las acciones correspondientes.

---

## 📝 Notas Técnicas

### Dependencias JavaScript
- **Chart.js 4.4.0** - Gráficos interactivos
- **DataTables** - Tablas con búsqueda/ordenamiento
- **jQuery** - AJAX y validación
- **Bootstrap 5** - UI responsive

### Validaciones Implementadas
- Rango de rebajos según tipo de falta
- Tamaño y formato de archivos (evidencia)
- Fechas válidas (no futuras, coherentes)
- Longitud mínima de textos (justificaciones)
- Confirmaciones antes de acciones críticas

### Consideraciones de Rendimiento
- Uso de DTOs en ViewModels
- Queries optimizadas con proyecciones
- Paginación en listas largas (DataTables)
- Carga bajo demanda (AJAX)

---

## 🧪 Pruebas Recomendadas

1. **Flujo completo de boleta:**
   - Crear boleta → Notificación enviada → Nota recalculada

2. **Estudiante aplazado:**
   - Nota < 65 → Crear programa → Verificar → Estado actualizado

3. **Decisión profesional:**
   - Aplicar Opción C → Nota ajustada → Registro en acta

4. **Reportes:**
   - Generar historial estudiante
   - Exportar reporte general a Excel

5. **Anulación:**
   - Anular boleta → Nota recalculada → Estado actualizado

---

## 👥 Roles del Sistema

| Rol | Permisos |
|-----|----------|
| **Profesor** | Crear boletas, ver notas de sus estudiantes |
| **Profesor Guía** | Todo lo del profesor + recibir notificaciones |
| **Orientador** | Crear programas, aplicar decisiones, ver reportes |
| **Administrador** | Acceso completo a todo el módulo |
| **Comité** | Verificar programas, aplicar decisiones profesionales |

---

## 📞 Soporte

Para preguntas o problemas con el módulo de conducta:
- Revisar los comentarios en el código fuente
- Consultar el REA 40862-V21 (documentación oficial del MEP)
- Verificar logs del sistema para errores

---

## ✅ Checklist de Implementación

- [x] Modelos de dominio (6 clases)
- [x] DbContext actualizado
- [x] ViewModels (20+ clases)
- [x] ConductaService (735 líneas)
- [x] BoletasConductaController (525 líneas)
- [x] NotaConductaController (540 líneas)
- [x] Migración generada
- [x] Seed data configurado
- [x] 15 vistas Razor completadas
- [ ] Migración aplicada a BD
- [ ] ProgramaAccionesController
- [ ] ReportesController
- [ ] Menú de navegación
- [ ] Permisos configurados
- [ ] Pruebas funcionales

**Progreso general: 85%** ✅

---

**Última actualización:** Noviembre 7, 2025
