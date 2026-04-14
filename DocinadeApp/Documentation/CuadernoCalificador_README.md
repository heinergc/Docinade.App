# Cuaderno Calificador Automático - Primer Cuatrimestre 2025

## ?? Descripción General

Este módulo implementa un **cuaderno calificador automático** no intrusivo que genera dinámicamente las columnas basándose en las relaciones existentes entre:
- **Materias** ? **Instrumentos de Evaluación** ? **Rúbricas**
- **Estudiantes** ? **Calificaciones por Rúbrica**

### ?? Objetivos Cumplidos

? **No intrusividad**: Sin modificar entidades existentes  
? **Generación dinámica**: Columnas basadas en relaciones actuales  
? **Período específico**: Enfocado en "Primer Cuatrimestre 2025"  
? **Cálculo automático**: Fórmula configurable con ponderaciones  
? **Exportación**: CSV con metadatos completos  
? **Estadísticas**: Promedios, rangos y completitud  

## ??? Arquitectura de la Solución

### Estructura de Archivos Creados

```
?? DTOs/Calificador/
??? ?? CalificadorDtos.cs                    # DTOs para transferencia de datos

?? Services/Calificador/
??? ?? CalificadorService.cs                 # Lógica principal del cuaderno

?? Controllers/
??? ?? CalificadorPQ2025Controller.cs        # Controlador no intrusivo

?? Views/CalificadorPQ2025/
??? ?? Index.cshtml                          # Formulario de generación
??? ?? CuadernoGrid.cshtml                   # Grid dinámico del cuaderno

?? Scripts/
??? ?? CuadernoCalificador_Consultas.sql     # Documentación SQL

?? Tests/Calificador/
??? ?? CalificadorServiceTests.cs            # Pruebas unitarias
```

### ?? Componentes Principales

#### 1. DTOs (Transferencia de Datos)
- **`CalificadorRowDto`**: Fila del cuaderno (estudiante + calificaciones)
- **`CalificadorColumnDto`**: Metadatos de columna (Instrumento ? Rúbrica)
- **`CuadernoCalificadorDto`**: DTO principal con filas, columnas y estadísticas
- **`CalificadorQueryDto`**: Parámetros de consulta configurables

#### 2. Servicio Principal (`CalificadorService`)
- **`GenerarCuadernoAsync()`**: Método principal de generación
- **`ObtenerColumnasAsync()`**: Descubrimiento dinámico de columnas
- **`ValidarParametrosAsync()`**: Validación de entrada
- Algoritmos de cálculo: PROMEDIO, SUMA, MEJOR_NOTA

#### 3. Controlador (`CalificadorPQ2025Controller`)
- **`Index`**: Formulario de configuración
- **`Generar`**: Generación del cuaderno
- **`ExportarCsv`**: Exportación con metadatos
- APIs para validación y vista previa

## ?? Funcionalidad del Cuaderno

### Estructura de Datos

**Fórmula Principal:**
```
Total Final = ?(Calificación_Instrumento × Ponderación_%)
```

**Ejemplo de Cálculo:**
```
Estudiante: Juan Pérez
- Tarea 1 ? Rúbrica A: 100 puntos (30%)
- Tarea 2 ? Rúbrica B: 80 puntos  (30%)
- Proyecto 1 ? Rúbrica C: 90 puntos (40%)

Total = (100 × 0.30) + (80 × 0.30) + (90 × 0.40) = 90.00
```

### Características Dinámicas

1. **Descubrimiento Automático**: Las columnas se generan automáticamente basándose en:
   ```sql
   InstrumentoMaterias ? InstrumentosEvaluacion ? InstrumentoRubricas ? Rubricas
   ```

2. **Ponderaciones Configurables**:
   - Si existen en `InstrumentoRubrica.Ponderacion` ? usar valores reales
   - Si no existen ? aplicar por defecto: Tarea1(30%), Tarea2(30%), Proyecto1(40%)

3. **Modos de Cálculo** (para múltiples rúbricas por instrumento):
   - **PROMEDIO**: Promedio aritmético de rúbricas del instrumento
   - **SUMA**: Suma limitada a 100 puntos máximo
   - **MEJOR_NOTA**: La calificación más alta del instrumento

## ?? Instrucciones de Despliegue

### 1. Registro de Servicios (? Completado)

El servicio ya está registrado en `Program.cs`:
```csharp
builder.Services.AddScoped<ICalificadorService, CalificadorService>();
```

### 2. Navegación (? Completado)

El enlace está agregado en `_Layout.cshtml`:
```html
<a class="dropdown-item" asp-controller="CalificadorPQ2025" asp-action="Index">
    <i class="fas fa-calculator"></i> Cuaderno Automático PQ2025
</a>
```

### 3. Base de Datos

**No requiere cambios en esquema**. El módulo utiliza las tablas existentes:
- `Materias`, `PeriodosAcademicos`, `Estudiantes`
- `InstrumentosEvaluacion`, `InstrumentoMaterias`, `InstrumentoRubricas`
- `Rubricas`, `Evaluaciones`

### 4. Configuración del Período "Primer Cuatrimestre 2025"

El sistema busca automáticamente períodos que contengan:
- `Año = 2025`
- `Ciclo` que contenga "PRIMER", "1", o "I"
- `Activo = true`

## ?? Uso del Sistema

### Paso 1: Acceso
Navegue a: **Configuración** ? **Cuaderno Automático PQ2025**

### Paso 2: Configuración
1. **Materia**: Seleccione la materia objetivo
2. **Período**: Se preselecciona "Primer Cuatrimestre 2025" si existe
3. **Modo de Cálculo**: Elija cómo manejar múltiples rúbricas por instrumento
4. **Opciones avanzadas**: Valor por defecto, incluir inactivos

### Paso 3: Generación
- **Vista Previa Columnas**: Ver estructura antes de generar
- **Estadísticas**: Obtener resumen rápido
- **Generar Cuaderno**: Crear el cuaderno completo

### Paso 4: Exportación
- **Exportar CSV**: Archivo con metadatos y estadísticas
- **Imprimir**: Versión optimizada para impresión

## ?? Estadísticas Incluidas

El cuaderno proporciona:
- **Total de estudiantes** activos/inactivos
- **Total de instrumentos** en el período
- **Total de rúbricas** vinculadas
- **Promedio general** de la clase
- **Nota máxima/mínima** obtenidas
- **Estudiantes con todas las notas** vs **pendientes**

## ?? Pruebas y Validación

### Casos de Prueba Incluidos

1. **Generación básica** con datos válidos
2. **Cálculo de total final** con fórmula específica
3. **Obtención de columnas** dinámicas
4. **Validación de parámetros** inválidos
5. **Manejo de valores por defecto** para calificaciones faltantes

### Ejecutar Pruebas
```bash
dotnet test --filter "CalificadorServiceTests"
```

### Caso de Aceptación
```
DADO: Materia "Matemáticas I" en "Primer Cuatrimestre 2025"
      Instrumentos: Tarea1(30%), Tarea2(30%), Proyecto1(40%)
      Estudiante con calificaciones: 100, 80, 90

CUANDO: Se genera el cuaderno calificador

ENTONCES: Total Final = (100×0.30) + (80×0.30) + (90×0.40) = 90.00
```

## ?? Configuración Avanzada

### Personalizar Ponderaciones Por Defecto

Modifique en `CalificadorService.AplicarPonderacionesPorDefecto()`:
```csharp
if (totalInstrumentos >= 3)
{
    ponderacionesPorDefecto[instrumentoIds[0]] = 30m;  // Tarea 1
    ponderacionesPorDefecto[instrumentoIds[1]] = 30m;  // Tarea 2
    ponderacionesPorDefecto[instrumentoIds[2]] = 40m;  // Proyecto 1
}
```

### Agregar Nuevos Modos de Cálculo

En `CalificadorService.CargarCalificacionesEstudianteAsync()`:
```csharp
decimal calificacionInstrumento = query.ModoCalculo switch
{
    "PROMEDIO" => kvp.Value.Average(),
    "SUMA" => Math.Min(kvp.Value.Sum(), 100),
    "MEJOR_NOTA" => kvp.Value.Max(),
    "NUEVO_MODO" => // Su lógica personalizada
    _ => kvp.Value.Average()
};
```

## ?? Consideraciones Importantes

### Rendimiento
- El sistema carga dinámicamente las relaciones en cada generación
- Para materias con muchos estudiantes (>500), considere implementar cache
- Las consultas están optimizadas con Entity Framework Core

### Seguridad
- Utiliza los mismos roles y permisos del sistema existente
- Validación de parámetros para prevenir inyección SQL
- No expone información sensible en APIs públicas

### Mantenimiento
- **Logs**: Todos los errores se registran con `ILogger<CalificadorService>`
- **Monitoreo**: Métricas de tiempo de generación en logs
- **Escalabilidad**: Arquitectura permite agregar nuevos períodos sin código

## ?? Extensibilidad Futura

### Agregar Nuevos Períodos
1. Cree un nuevo controlador: `CalificadorS12025Controller`
2. Duplique las vistas con prefijo correspondiente
3. Ajuste las consultas de período objetivo

### Integración con Otros Módulos
- **Notifications**: Notificar cuando se complete un cuaderno
- **Reporting**: Integrar con sistema de reportes existente
- **API**: Exponer endpoints REST para aplicaciones móviles

## ?? Soporte

Para preguntas o problemas:
1. Consulte los logs en la consola de aplicación
2. Verifique las pruebas unitarias para casos de uso
3. Revise la documentación SQL en `/Scripts/`

---
**Versión**: 1.0.0  
**Última actualización**: Enero 2025  
**Compatibilidad**: .NET 8, Entity Framework Core, Bootstrap 5