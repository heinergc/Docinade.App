# ?? SOLUCIÓN - No se cargan los items de la rúbrica en Evaluaciones

## ?? **Problema Identificado**

La página `https://localhost:18163/Evaluaciones/Create` no carga los items de las rúbricas cuando se selecciona una rúbrica del dropdown.

## ?? **Diagnóstico del Problema**

### **Posibles Causas:**
1. **Datos faltantes**: No hay datos de prueba en la base de datos
2. **Configuración incorrecta**: Faltan relaciones entre rúbricas e items
3. **Valores de rúbrica faltantes**: No hay valores configurados en la tabla `ValoresRubrica`
4. **Error en el flujo AJAX**: Problema en la comunicación cliente-servidor

---

## ??? **Pasos de Diagnóstico**

### **1. Verificar Conexión AJAX**

Abre las **Developer Tools** del navegador y verifica:

1. **Console Tab**: Busca errores de JavaScript
2. **Network Tab**: Verifica si la petición AJAX se está enviando
3. **Response**: Revisa qué respuesta está devolviendo el servidor

### **2. Método de Diagnóstico Automático**

He agregado un método de diagnóstico. Ejecuta esta URL en tu navegador:

```
https://localhost:18163/Evaluaciones/DiagnosticarRubrica?rubricaId=1
```

Esto te mostrará:
- ? Si la rúbrica existe
- ?? Cuántos items tiene
- ?? Si tiene valores configurados
- ?? Qué niveles están disponibles

### **3. Verificar Datos en la Base de Datos**

Ejecuta estas consultas SQL:

```sql
-- Verificar rúbricas disponibles
SELECT IdRubrica, NombreRubrica FROM Rubricas;

-- Verificar items por rúbrica (ejemplo rúbrica ID = 1)
SELECT IdItem, NombreItem, IdRubrica FROM ItemsEvaluacion WHERE IdRubrica = 1;

-- Verificar valores de rúbrica
SELECT COUNT(*) as TotalValores FROM ValoresRubrica WHERE IdRubrica = 1;

-- Verificar niveles disponibles
SELECT DISTINCT nc.IdNivel, nc.NombreNivel 
FROM NivelesCalificacion nc
JOIN ValoresRubrica vr ON nc.IdNivel = vr.IdNivel 
WHERE vr.IdRubrica = 1;
```

---

## ? **Solución Más Probable: Ejecutar Datos de Prueba**

El problema más común es que **no hay datos de prueba** en la base de datos.

### **Opción 1: Script Automático (Recomendado)**

Ejecuta el script PowerShell:

```powershell
.\Scripts\ConfigurarDatos.ps1
```

### **Opción 2: Script Manual**

Ejecuta el archivo SQL:

```sql
-- Ubicación: src/RubricasApp.Web/Scripts/DatosPrueba_Simplificado.sql
```

**Pasos:**
1. Abre tu herramienta de base de datos (SQLite Browser, etc.)
2. Conecta a la base de datos del proyecto
3. Ejecuta el script completo `DatosPrueba_Simplificado.sql`

### **Opción 3: Verificación Rápida**

Ejecuta estas consultas rápidas para verificar datos:

```sql
-- 1. Verificar datos básicos
SELECT 'Rubricas' as Tabla, COUNT(*) as Total FROM Rubricas;
SELECT 'Items' as Tabla, COUNT(*) as Total FROM ItemsEvaluacion;
SELECT 'Valores' as Tabla, COUNT(*) as Total FROM ValoresRubrica;

-- 2. Si los totales son 0, ejecuta el script de datos de prueba
```

---

## ?? **Datos de Prueba Incluidos**

El script crea automáticamente:

### **?? Rúbricas:**
- **Rúbrica Tarea 1** (ID: 1)
- **Rúbrica Tarea 2** (ID: 2)  
- **Rúbrica Proyecto 1** (ID: 3)

### **?? Items por Rúbrica:**

**Rúbrica Tarea 1:**
- Cumplimiento de objetivos (25%)
- Calidad del contenido (30%)
- Presentación (20%)
- Entrega puntual (25%)

**Rúbrica Tarea 2:**
- Análisis crítico (40%)
- Uso de fuentes (30%)
- Coherencia (30%)

**Rúbrica Proyecto 1:**
- Planificación (20%)
- Desarrollo (40%)
- Innovación (20%)
- Presentación final (20%)

### **??? Niveles de Calificación:**
- **Excelente** (Orden 1)
- **Bueno** (Orden 2)
- **Regular** (Orden 3)
- **Deficiente** (Orden 4)

---

## ?? **Pruebas Después de Ejecutar los Datos**

### **1. Crear Nueva Evaluación**
1. Ir a: `https://localhost:18163/Evaluaciones/Create`
2. Seleccionar cualquier estudiante
3. Seleccionar **"Rúbrica Tarea 1"**
4. **Verificar**: Deben aparecer 4 items con niveles de calificación

### **2. Debug desde Console**
En Developer Tools, ejecuta:
```javascript
// Función de debug disponible
window.debugEvaluacion()
```

### **3. Verificar Response del Servidor**
La respuesta AJAX debe incluir:
```json
{
  "items": [
    {
      "idItem": 1,
      "nombreItem": "Cumplimiento de objetivos",
      "descripcion": "Cumplimiento de objetivos",
      "peso": 25.0,
      "ordenItem": 0
    }
    // ... más items
  ],
  "niveles": [
    {
      "idNivel": 1,
      "nombreNivel": "Excelente",
      "idItem": 1,
      "valor": 25.0
    }
    // ... más niveles
  ]
}
```

---

## ?? **Si el Problema Persiste**

### **1. Verificar Logs del Servidor**
Revisa la consola donde ejecutas `dotnet run` para errores.

### **2. Verificar Base de Datos**
```sql
-- Verificar que la base de datos existe y tiene tablas
.tables

-- Verificar conexión a datos
SELECT COUNT(*) FROM Rubricas;
```

### **3. Reiniciar Aplicación**
```bash
# Detener la aplicación (Ctrl+C)
# Reiniciar
dotnet run
```

### **4. Limpiar y Reconstruir**
```bash
dotnet clean
dotnet build
dotnet run
```

---

## ?? **Resultado Esperado**

Después de ejecutar los datos de prueba:

1. ? **Dropdown de rúbricas** muestra 3 opciones
2. ? **Al seleccionar rúbrica** aparecen los items automáticamente
3. ? **Cada item** tiene 4 niveles de calificación disponibles
4. ? **Cálculo automático** de puntuación funciona
5. ? **Validación** permite guardar la evaluación

### **?? Vista Esperada:**
```
???????????????????????????????????????????????????????????
? Evaluación de Items                                     ?
???????????????????????????????????????????????????????????
? Item de Evaluación     ? Peso ? Nivel          ? Puntos ?
???????????????????????????????????????????????????????????
? Cumplimiento objetivos ? 25%  ? [Dropdown]     ? 0      ?
? Calidad del contenido  ? 30%  ? [Dropdown]     ? 0      ?
? Presentación           ? 20%  ? [Dropdown]     ? 0      ?
? Entrega puntual        ? 25%  ? [Dropdown]     ? 0      ?
???????????????????????????????????????????????????????????
```

---

## ?? **Método Debug Adicional**

Si necesitas más información, usa:

```
https://localhost:18163/Evaluaciones/DiagnosticarRubrica?rubricaId=1
https://localhost:18163/Evaluaciones/DiagnosticarRubrica?rubricaId=2
https://localhost:18163/Evaluaciones/DiagnosticarRubrica?rubricaId=3
```

Esto te dará un reporte completo del estado de cada rúbrica.

---

## ?? **¡Problema Resuelto!**

Una vez ejecutados los datos de prueba, el módulo de Evaluaciones funcionará completamente. Los items se cargarán dinámicamente y podrás crear evaluaciones sin problemas. ??