---
applyTo: '**'
---
# Instrucciones Básicas de Codificación

## Reglas de Caracteres y Codificación

### ❌ NO usar emoticones en el código
- **Prohibido**: Emoticones (🎉, ✅, 🔧, ⚠️, etc.) en código C#, SQL, JavaScript, HTML
- **Razón**: Causan problemas de codificación UTF-8, errores de compilación, incompatibilidad con algunos editores
- **Permitido**: Emoticones SOLO en comentarios de commits Git, documentación Markdown, y archivos README

### ✅ Alternativas aceptadas
```csharp
// ❌ MAL
var mensaje = "🎉 Sistema inicializado";
Console.WriteLine("✅ Proceso completado");

// ✅ BIEN
var mensaje = "[OK] Sistema inicializado";
Console.WriteLine("SUCCESS: Proceso completado");
Console.WriteLine("INFO: Cargando configuración...");
Console.WriteLine("ERROR: Falló la conexión");
```

### Prefijos de log recomendados
- `[INFO]`, `[SUCCESS]`, `[WARNING]`, `[ERROR]`, `[DEBUG]`
- `OK:`, `FAIL:`, `WARN:`, `ERROR:`
- Usar colores de consola cuando sea apropiado: `Console.ForegroundColor`

## Convenciones Generales
- Encoding: UTF-8 sin BOM para archivos de código
- Indentación: 4 espacios (C#), 2 espacios (HTML/JS)
- Nombres en español para entidades de dominio MEP (Estudiante, Materia, etc.)
- Nombres en inglés para infraestructura técnica (Service, Repository, etc.)