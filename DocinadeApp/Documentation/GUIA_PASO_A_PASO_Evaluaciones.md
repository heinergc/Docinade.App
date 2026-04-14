# ?? GUÍA PASO A PASO - Resolver "No cargan items de rúbrica"

## ?? **PROBLEMA**
Los items de las rúbricas no se cargan en `https://localhost:18163/Evaluaciones/Create`

**ACTUALIZACIÓN**: También se resolvió el error de headers duplicados:
```
ArgumentException: An item with the same key has already been added.
Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http.HttpHeaders.ThrowDuplicateKeyException()
```

## ? **SOLUCIÓN RÁPIDA (5 minutos)**

### **Paso 1: Ejecutar Script de Datos**
```powershell
# Navegar al directorio del proyecto
cd C:\Users\hguido\Desktop\RubricasApp\src\RubricasApp.Web

# Ejecutar script de configuración
.\Scripts\ConfigurarDatos.ps1
```

### **Paso 2: Reiniciar Aplicación**
```bash
# Detener aplicación (Ctrl+C si está corriendo)
# Luego ejecutar:
dotnet run
```

### **Paso 3: Probar Evaluaciones**
1. Ir a: `https://localhost:18163/Evaluaciones/Create`
2. Seleccionar cualquier estudiante
3. Seleccionar **"Rúbrica Tarea 1"**
4. ? **Verificar**: Deben aparecer 4 items con dropdowns de niveles
5. ? **Sin errores**: No debe aparecer el error de headers duplicados

---

## ?? **PROBLEMAS TÉCNICOS RESUELTOS**

### **?? Error de Headers Duplicados SOLUCIONADO**

**Causa**: En los métodos del controlador se agregaba manualmente el header `Content-Type` cuando ASP.NET Core ya lo configuraba automáticamente.

**Corrección aplicada:**
```csharp
// ? ANTES (causaba error):
Response.Headers.Add("Content-Type", "application/json; charset=utf-8");
return Json(new { items, niveles });

// ? DESPUÉS (corregido):
return Json(new { items, niveles }, new System.Text.Json.JsonSerializerOptions
{
    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    PropertyNamingPolicy = null
});
```

**Métodos corregidos:**
- `ObtenerItemsPorRubrica` en EvaluacionesController
- `DiagnosticarRubrica` en EvaluacionesController

---

## ?? **DIAGNÓSTICO MANUAL**

### **Si la solución rápida no funciona:**

#### **1. Verificar Datos en Base de Datos**
```sql
-- Abrir base de datos con herramienta de tu preferencia
-- Ejecutar estas consultas:

SELECT COUNT(*) as TotalRubricas FROM Rubricas;
-- Esperado: Al menos 3

SELECT COUNT(*) as TotalItems FROM ItemsEvaluacion;  
-- Esperado: Al menos 11

SELECT COUNT(*) as TotalValores FROM ValoresRubrica;
-- Esperado: Al menos 16

-- Si alguno es 0, ejecuta DatosPrueba_Simplificado.sql
```

#### **2. Probar URL de Diagnóstico**
```
https://localhost:18163/Evaluaciones/DiagnosticarRubrica?rubricaId=1
```

**Respuesta esperada:**
```json
{
  "rubricaExiste": true,
  "totalItems": 4,
  "nivelesConValores": 4,
  "itemsSinValores": [],
  "timestamp": "2024-12-19T..."
}
```

#### **3. Verificar Console del Navegador**
1. Abrir **Developer Tools** (F12)
2. Ir a **Console** tab
3. Seleccionar una rúbrica
4. ? **Verificar**: NO debe haber errores de headers duplicados
5. ? **Verificar**: NO debe haber errores de jQuery/JSON

---

## ?? **ESTRUCTURA DE DATOS ESPERADA**

### **Rúbricas Disponibles:**
- `ID: 1` - **Rúbrica Tarea 1** 
- `ID: 2` - **Rúbrica Tarea 2**
- `ID: 3` - **Rúbrica Proyecto 1**

### **Items por Rúbrica (Ejemplo Rúbrica ID: 1):**
```
ID  | Nombre                    | Peso
----|---------------------------|-----
1   | Cumplimiento de objetivos | 25%
2   | Calidad del contenido     | 30%
3   | Presentación              | 20%
4   | Entrega puntual           | 25%
```

### **Niveles de Calificación:**
```
ID  | Nombre      | Orden
----|-------------|------
1   | Excelente   | 1
2   | Bueno       | 2
3   | Regular     | 3
4   | Deficiente  | 4
```

---

## ??? **TROUBLESHOOTING AVANZADO**

### **Problema: Script PowerShell no funciona**

**Solución Manual:**
1. Abrir tu herramienta de base de datos (DB Browser for SQLite, etc.)
2. Conectar a la base de datos del proyecto
3. Ejecutar manualmente: `src/RubricasApp.Web/Scripts/DatosPrueba_Simplificado.sql`

### **Problema: Datos insertados pero no aparecen**

**Verificar conexión string:**
```csharp
// En appsettings.json o Program.cs
// Verificar que apunta a la BD correcta
"ConnectionStrings": {
  "DefaultConnection": "Data Source=ruta/correcta/a/bd.db"
}
```

### **Problema: Error de AJAX**

**Revisar URL del método:**
```javascript
// En Create.cshtml, línea ~124
url: '@Url.Action("ObtenerItemsPorRubrica", "Evaluaciones")'

// Debe generar: /Evaluaciones/ObtenerItemsPorRubrica
```

### **Problema: Response vacía del servidor**

**Verificar método del controlador:**
```csharp
// EvaluacionesController.cs
// Método: ObtenerItemsPorRubrica debe existir y ser [HttpGet]
[HttpGet]
public async Task<IActionResult> ObtenerItemsPorRubrica(int rubricaId)
```

### **? NUEVO: Problema de Headers Duplicados**

**Si aparece el error:**
```
ArgumentException: An item with the same key has already been added.
Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http.HttpHeaders.ThrowDuplicateKeyException()
```

**Verificar que NO se agreguen headers manualmente:**
```csharp
// ? NO hacer esto en métodos que devuelven JSON:
Response.Headers.Add("Content-Type", "application/json; charset=utf-8");

// ? En su lugar, usar las opciones del Json():
return Json(data, new System.Text.Json.JsonSerializerOptions { ... });
```

---

## ?? **VERIFICACIÓN FINAL**

### **Cuando Todo Esté Funcionando:**

#### **1. Crear Evaluación debe mostrar:**
```
???????????????????????????????????????????????????????????
? ? Dropdown estudiantes: 5 opciones                     ?
? ? Dropdown rúbricas: 3 opciones                        ?
? ? Al seleccionar rúbrica: Items aparecen automático    ?
? ? Cada item: 4 niveles en dropdown                     ?
? ? Cálculo puntos: Automático al seleccionar niveles    ?
? ? Botón guardar: Se habilita cuando todo está lleno    ?
? ? Sin errores: NO aparecen errores de headers          ?
???????????????????????????????????????????????????????????
```

#### **2. Debug desde Console:**
```javascript
// Ejecutar en Console del navegador:
window.debugEvaluacion()

// Debe retornar:
{
  items: [Array con 4 elementos],
  niveles: [Array con 16 elementos], 
  rubricaId: "1",
  estudianteId: "algún_id"
}
```

#### **3. Network Tab debe mostrar:**
```
? GET /Evaluaciones/ObtenerItemsPorRubrica?rubricaId=1
? Status: 200 OK
? Response: JSON con items y niveles
? Content-Type: application/json; charset=utf-8
? Sin errores de headers duplicados
```

---

## ?? **SIGUIENTE PASO**

Una vez que las evaluaciones funcionen:

1. **Crear algunas evaluaciones de prueba**
2. **Probar el Cuaderno Calificador**: `https://localhost:18163/CalificadorPQ2025`
3. **Verificar exportación a CSV**
4. **Revisar estadísticas y reportes**

---

## ?? **CONTACTO/SOPORTE**

Si el problema persiste después de seguir todos estos pasos:

1. **Verificar versión .NET**: `dotnet --version` (debe ser 8.0+)
2. **Revisar logs de aplicación** en la consola donde ejecutas `dotnet run`
3. **Comprobar archivos de migración** en `/Data/Migrations/`
4. **Verificar que todas las tablas existen** en la base de datos
5. **Reiniciar aplicación completamente**: detener con Ctrl+C y volver a ejecutar `dotnet run`

---

## ?? **¡PROBLEMAS RESUELTOS!**

Siguiendo estos pasos, tanto el módulo de Evaluaciones como el error de headers duplicados deberían estar completamente solucionados. Los items se cargarán dinámicamente y podrás crear evaluaciones sin problemas técnicos.

**¡A evaluar sin errores! ???**