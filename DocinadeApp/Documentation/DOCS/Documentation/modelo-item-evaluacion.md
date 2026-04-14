# Modelo ItemEvaluacion - Documentación

## 📋 **Descripción**

El modelo `ItemEvaluacion` representa un item específico dentro de una rúbrica de evaluación. Cada item define un criterio o aspecto que será evaluado usando la rúbrica asociada.

## 🏗️ **Estructura del Modelo**

```csharp
using System.ComponentModel.DataAnnotations;

namespace RubricasApp.Web.Models
{
    public class ItemEvaluacion
    {
        public int IdItem { get; set; }
        
        [Required(ErrorMessage = "Debe seleccionar una rúbrica")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar una rúbrica válida")]
        [Display(Name = "Rúbrica")]
        public int IdRubrica { get; set; }
        
        [Required(ErrorMessage = "El nombre del item es requerido")]
        [StringLength(200, ErrorMessage = "El nombre no puede exceder 200 caracteres")]
        [Display(Name = "Nombre del Item")]
        public string NombreItem { get; set; } = string.Empty;
        
        [Display(Name = "Descripción")]
        public string? Descripcion { get; set; }
        
        [Display(Name = "Orden")]
        public int? OrdenItem { get; set; }
        
        // Navigation properties
        public virtual Rubrica Rubrica { get; set; } = null!;
        public virtual ICollection<ValorRubrica> ValoresRubrica { get; set; } = new List<ValorRubrica>();
        public virtual ICollection<DetalleEvaluacion> DetallesEvaluacion { get; set; } = new List<DetalleEvaluacion>();
    }
}
```

## 🔧 **Propiedades del Modelo**

### **Propiedades Principales**

| Propiedad | Tipo | Descripción | Validaciones |
|-----------|------|-------------|--------------|
| `IdItem` | `int` | Identificador único del item | Clave primaria (auto-incremento) |
| `IdRubrica` | `int` | Referencia a la rúbrica padre | Required, Range(1, int.MaxValue) |
| `NombreItem` | `string` | Nombre descriptivo del item | Required, StringLength(200) |
| `Descripcion` | `string?` | Descripción detallada (opcional) | Nullable |
| `OrdenItem` | `int?` | Orden de presentación (opcional) | Nullable |

### **Navigation Properties (Propiedades de Navegación)**

| Propiedad | Tipo | Descripción | Relación |
|-----------|------|-------------|----------|
| `Rubrica` | `virtual Rubrica` | Rúbrica asociada | Many-to-One |
| `ValoresRubrica` | `virtual ICollection<ValorRubrica>` | Valores/niveles definidos para este item | One-to-Many |
| `DetallesEvaluacion` | `virtual ICollection<DetalleEvaluacion>` | Evaluaciones específicas de este item | One-to-Many |

## 📊 **Relaciones en la Base de Datos**

```
ItemEvaluacion
├── Pertenece a: Rubrica (Many-to-One)
├── Tiene: ValoresRubrica (One-to-Many)
└── Evaluado en: DetallesEvaluacion (One-to-Many)
```

### **Diagrama de Relaciones**

```
┌─────────────────┐       ┌─────────────────┐
│     Rubrica     │ 1───M │  ItemEvaluacion │
│                 │       │                 │
│ - IdRubrica     │◄──────│ - IdItem        │
│ - NombreRubrica │       │ - IdRubrica (FK)│
│ - Descripcion   │       │ - NombreItem    │
│ - Estado        │       │ - Descripcion   │
│ - FechaCreacion │       │ - OrdenItem     │
└─────────────────┘       └─────────────────┘
                                    │
                                    │ 1
                                    │
                                    M
                          ┌─────────────────┐
                          │  ValorRubrica   │
                          │                 │
                          │ - IdValor       │
                          │ - IdItem (FK)   │
                          │ - Descripcion   │
                          │ - Puntuacion    │
                          │ - OrdenNivel    │
                          └─────────────────┘
```

## 🎯 **Propósito y Uso**

### **Contexto de Uso**

Un `ItemEvaluacion` representa un criterio específico dentro de una rúbrica. Por ejemplo, en una rúbrica de "Ensayo Académico", podrías tener estos items:

- **Item 1**: "Claridad en la argumentación"
- **Item 2**: "Uso correcto de referencias"
- **Item 3**: "Estructura y organización"
- **Item 4**: "Gramática y redacción"

### **Flujo de Evaluación**

1. **Creación**: Se define el item dentro de una rúbrica
2. **Configuración**: Se establecen los valores/niveles para el item
3. **Evaluación**: Se usa en evaluaciones específicas
4. **Puntuación**: Se asignan puntuaciones según los criterios

## 💡 **Validaciones y Reglas de Negocio**

### **Validaciones del Modelo**

```csharp
[Required(ErrorMessage = "Debe seleccionar una rúbrica")]
[Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar una rúbrica válida")]
public int IdRubrica { get; set; }
```

- **Propósito**: Garantiza que el item esté asociado a una rúbrica válida
- **Validación**: Debe ser un número entero positivo mayor a 0

```csharp
[Required(ErrorMessage = "El nombre del item es requerido")]
[StringLength(200, ErrorMessage = "El nombre no puede exceder 200 caracteres")]
public string NombreItem { get; set; } = string.Empty;
```

- **Propósito**: Nombre descriptivo obligatorio con longitud limitada
- **Validación**: Requerido, máximo 200 caracteres

### **Reglas de Negocio**

1. **Orden de Items**: Si no se especifica `OrdenItem`, se ordenan alfabéticamente
2. **Unicidad**: El nombre debe ser único dentro de la misma rúbrica
3. **Dependencias**: No se puede eliminar si tiene evaluaciones asociadas
4. **Estado**: Hereda el estado de la rúbrica padre

## 🔍 **Ejemplos de Uso**

### **Creación de un Item**

```csharp
var item = new ItemEvaluacion
{
    IdRubrica = 1,
    NombreItem = "Claridad en la argumentación",
    Descripcion = "Evalúa la claridad y coherencia de los argumentos presentados",
    OrdenItem = 1
};
```

### **Consulta con Navigation Properties**

```csharp
var items = await _context.ItemsEvaluacion
    .Include(i => i.Rubrica)
    .Include(i => i.ValoresRubrica)
    .Where(i => i.IdRubrica == rubricaId)
    .OrderBy(i => i.OrdenItem ?? int.MaxValue)
    .ThenBy(i => i.NombreItem)
    .ToListAsync();
```

### **Validación en Controlador**

```csharp
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Create([Bind("IdRubrica,NombreItem,Descripcion,OrdenItem")] ItemEvaluacion item)
{
    // Eliminar error de validación de la propiedad de navegación
    ModelState.Remove("Rubrica");
    
    if (ModelState.IsValid)
    {
        _context.Add(item);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
    
    // Recargar datos para la vista
    ViewData["IdRubrica"] = new SelectList(_context.Rubricas, "IdRubrica", "NombreRubrica", item.IdRubrica);
    return View(item);
}
```

## 🚨 **Problemas Comunes y Soluciones**

### **Error: "The Rubrica field is required"**

**Problema**: La navigation property `Rubrica` causa errores de validación en el model binding.

**Solución**:
```csharp
// En el controlador POST
ModelState.Remove("Rubrica");
```

### **Error: Over-posting Attack**

**Problema**: Propiedades no deseadas se pueden modificar desde el formulario.

**Solución**:
```csharp
[HttpPost]
public async Task<IActionResult> Create([Bind("IdRubrica,NombreItem,Descripcion,OrdenItem")] ItemEvaluacion item)
```

### **Error: Lazy Loading no funciona**

**Problema**: Navigation properties aparecen como `null`.

**Solución**:
```csharp
// Usar Include para carga explícita
var items = await _context.ItemsEvaluacion
    .Include(i => i.Rubrica)
    .ToListAsync();
```

## 📈 **Mejores Prácticas**

1. **Usar [Bind]** en métodos POST para controlar model binding
2. **Remover navigation properties** del ModelState cuando sea necesario
3. **Validar foreign keys** antes de guardar
4. **Usar Include()** para cargar navigation properties
5. **Ordenar por OrdenItem** con fallback a nombre alfabético
6. **Validar longitudes** apropiadas para campos de texto
7. **Documentar dependencias** entre entidades

## 🔄 **Migraciones y Actualizaciones**

### **Comando para Migración**

```bash
dotnet ef migrations add AddItemEvaluacionModel
dotnet ef database update
```

### **Índices Recomendados**

```csharp
// En DbContext.OnModelCreating
modelBuilder.Entity<ItemEvaluacion>()
    .HasIndex(i => i.IdRubrica)
    .HasDatabaseName("IX_ItemEvaluacion_IdRubrica");

modelBuilder.Entity<ItemEvaluacion>()
    .HasIndex(i => new { i.IdRubrica, i.NombreItem })
    .IsUnique()
    .HasDatabaseName("IX_ItemEvaluacion_IdRubrica_NombreItem");
```

---

**Fecha de documentación:** 16 de julio de 2025  
**Versión del modelo:** 1.0  
**Autor:** GitHub Copilot  
**Contexto:** Sistema de Rúbricas - Gestión de Items de Evaluación
