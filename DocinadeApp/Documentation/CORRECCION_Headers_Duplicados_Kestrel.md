# ?? CORRECCIÓN - Error Headers Duplicados en Kestrel

## ? **PROBLEMA RESUELTO**
```
ArgumentException: An item with the same key has already been added.
Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http.HttpHeaders.ThrowDuplicateKeyException()
```

## ?? **ANÁLISIS DEL PROBLEMA**

### **Causa Principal**
El error ocurría porque en los métodos del controlador se intentaba agregar manualmente el header `Content-Type` cuando ASP.NET Core ya lo configuraba automáticamente al devolver JSON con el método `Json()`.

### **Métodos Afectados**
- `ObtenerItemsPorRubrica` en `EvaluacionesController`
- `DiagnosticarRubrica` en `EvaluacionesController`

---

## ? **SOLUCIÓN IMPLEMENTADA**

### **Código Problemático (ANTES):**
```csharp
[HttpGet]
public async Task<IActionResult> ObtenerItemsPorRubrica(int rubricaId)
{
    try
    {
        // ... lógica del método ...
        
        // ? PROBLEMA: Agregar header manualmente
        Response.Headers.Add("Content-Type", "application/json; charset=utf-8");

        return Json(new { items, niveles }, new System.Text.Json.JsonSerializerOptions
        {
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            PropertyNamingPolicy = null
        });
    }
    catch (Exception ex)
    {
        // ? PROBLEMA: Agregar header manualmente también en catch
        Response.Headers.Add("Content-Type", "application/json; charset=utf-8");
        return Json(new { error = ex.Message });
    }
}
```

### **Código Corregido (DESPUÉS):**
```csharp
[HttpGet]
public async Task<IActionResult> ObtenerItemsPorRubrica(int rubricaId)
{
    try
    {
        // ... lógica del método ...
        
        // ? CORRECCIÓN: Sin header manual, ASP.NET Core lo maneja automáticamente
        return Json(new { items, niveles }, new System.Text.Json.JsonSerializerOptions
        {
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            PropertyNamingPolicy = null // Mantener nombres exactos
        });
    }
    catch (Exception ex)
    {
        // ? CORRECCIÓN: Sin header manual también en catch
        return Json(new { error = ex.Message });
    }
}
```

---

## ?? **ARCHIVOS MODIFICADOS**

### **1. EvaluacionesController.cs**
- **Método**: `ObtenerItemsPorRubrica`
- **Cambio**: Eliminadas líneas `Response.Headers.Add("Content-Type", "application/json; charset=utf-8");`

- **Método**: `DiagnosticarRubrica`
- **Cambio**: Eliminadas líneas `Response.Headers.Add("Content-Type", "application/json; charset=utf-8");`

---

## ?? **¿POR QUÉ OCURRÍA ESTE ERROR?**

### **Flujo del Problema:**
1. **Paso 1**: Se ejecuta el método del controlador
2. **Paso 2**: Se ejecuta `Response.Headers.Add("Content-Type", "application/json; charset=utf-8")`
3. **Paso 3**: Se ejecuta `return Json(...)` que internamente también configura el Content-Type
4. **Paso 4**: Kestrel detecta el header duplicado y lanza la excepción

### **Comportamiento Esperado de ASP.NET Core:**
- El método `Json()` **automáticamente** configura:
  - `Content-Type: application/json; charset=utf-8`
  - Serialización apropiada del objeto
  - Headers de respuesta correctos

### **Regla General:**
**Nunca agregar manualmente headers que ASP.NET Core configura automáticamente**

---

## ?? **VALIDACIÓN DE LA CORRECCIÓN**

### **Antes de la Corrección:**
```bash
# Error en la consola del servidor:
ArgumentException: An item with the same key has already been added.
Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http.HttpHeaders.ThrowDuplicateKeyException()

# Error en el navegador:
500 Internal Server Error
```

### **Después de la Corrección:**
```bash
# Consola del servidor:
Sin errores ?

# Navegador - Network Tab:
Status: 200 OK ?
Content-Type: application/json; charset=utf-8 ?
Response: JSON válido con items y niveles ?
```

---

## ??? **PREVENCIÓN DE ERRORES SIMILARES**

### **Buenas Prácticas:**

#### **? HACER:**
```csharp
// Dejar que ASP.NET Core maneje los headers automáticamente
return Json(data);

// Si necesitas opciones especiales de JSON:
return Json(data, new JsonSerializerOptions { ... });

// Para otros tipos de content:
return Content("texto", "text/plain");
return File(bytes, "application/pdf");
```

#### **? NO HACER:**
```csharp
// No agregar headers que ASP.NET Core maneja automáticamente
Response.Headers.Add("Content-Type", "application/json");
return Json(data); // Causa conflicto

// No forzar headers en métodos que ya los configuran
Response.ContentType = "application/json";
return Json(data); // También puede causar problemas
```

### **Headers Seguros para Agregar Manualmente:**
```csharp
// ? Headers personalizados (seguros):
Response.Headers.Add("X-Custom-Header", "mi-valor");
Response.Headers.Add("X-API-Version", "1.0");
Response.Headers.Add("Cache-Control", "no-cache");

// ? CORS headers (si no usas middleware):
Response.Headers.Add("Access-Control-Allow-Origin", "*");
```

---

## ?? **RESULTADO FINAL**

### **Estado Actual:**
- ? **Error de headers duplicados**: RESUELTO
- ? **Carga de items de rúbrica**: FUNCIONAL
- ? **Respuestas JSON**: CORRECTAS
- ? **UTF-8 encoding**: CONFIGURADO
- ? **Compatibilidad con navegadores**: COMPLETA

### **Funcionalidades Verificadas:**
1. **Crear Evaluación**: Items se cargan dinámicamente sin errores
2. **Diagnóstico de rúbricas**: URL de diagnóstico funciona correctamente
3. **Respuestas AJAX**: JSON válido sin problemas de encoding
4. **Headers HTTP**: Configurados automáticamente sin duplicación

---

## ?? **PRÓXIMOS PASOS**

Con este error resuelto, puedes:

1. **Ejecutar el script de datos**: `.\Scripts\ConfigurarDatos.ps1`
2. **Probar evaluaciones**: Crear evaluaciones sin errores técnicos
3. **Usar todas las funcionalidades**: Sistema completamente estable

---

## ?? **LECCIÓN APRENDIDA**

> **Regla de Oro**: Confía en las convenciones de ASP.NET Core. El framework maneja automáticamente muchos aspectos técnicos como headers HTTP, serialización JSON, encoding UTF-8, etc. Solo intervenir manualmente cuando sea estrictamente necesario y no interfiera con el comportamiento automático.

**¡Error de headers duplicados completamente resuelto! ??**