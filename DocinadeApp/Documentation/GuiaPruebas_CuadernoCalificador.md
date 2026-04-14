# ?? GUÍA DE PRUEBAS - CUADERNO CALIFICADOR PQ2025

## ?? Preparación de Datos

### Paso 1: Ejecutar Script de Datos de Prueba

Tienes dos opciones de scripts SQL para ejecutar:

#### Opción A: Script Completo (Recomendado)
```bash
# Ubicación: src/RubricasApp.Web/Scripts/DatosPrueba_CuadernoCalificador.sql
# Incluye datos completos con detalles de evaluación
```

#### Opción B: Script Simplificado (Más rápido)
```bash
# Ubicación: src/RubricasApp.Web/Scripts/DatosPrueba_Simplificado.sql
# Datos esenciales para pruebas básicas
```

### Paso 2: Ejecutar el Script

**En SQLite (recomendado):**
1. Abrir DB Browser for SQLite o similar
2. Conectar a la base de datos del proyecto
3. Ejecutar el script sección por sección
4. Verificar que no hay errores

**Desde línea de comandos:**
```bash
sqlite3 ruta/a/tu/base.db < Scripts/DatosPrueba_Simplificado.sql
```

## ?? Casos de Prueba

### Caso 1: Generación Básica del Cuaderno

**Datos de entrada:**
- **Materia:** Matemáticas I
- **Período:** Primer Cuatrimestre 2025
- **Modo de cálculo:** PROMEDIO
- **Valor por defecto:** 0

**Pasos:**
1. Ir a `https://localhost:18163/CalificadorPQ2025`
2. Seleccionar "Matemáticas I" en Materia
3. Seleccionar "Primer Cuatrimestre 2025" en Período
4. Hacer clic en "Generar Cuaderno"

**Resultados esperados:**
```
?? CUADERNO GENERADO CORRECTAMENTE

Columnas dinámicas:
? Tarea 1 ? Rúbrica Tarea 1 (30%)
? Tarea 2 ? Rúbrica Tarea 2 (30%) 
? Proyecto 1 ? Rúbrica Proyecto 1 (40%)

Estudiantes y cálculos:
?? Juan Carlos Pérez: (100×0.30) + (80×0.30) + (90×0.40) = 90.00
?? María José González: (85×0.30) + (75×0.30) + (88×0.40) = 83.20
?? Carlos Alberto Martínez: (92×0.30) + (78×0.30) + (95×0.40) = 88.60
?? Ana Patricia Ramírez: (88×0.30) + (82×0.30) + (0×0.40) = 51.00
?? Luis Fernando Torres: (76×0.30) + (0×0.30) + (0×0.40) = 22.80

Estadísticas:
?? Total Estudiantes: 5
?? Total Instrumentos: 3
?? Total Rúbricas: 3
?? Promedio General: 67.00 (aproximado)
```

### Caso 2: Vista Previa de Columnas

**Pasos:**
1. Seleccionar Materia y Período
2. Hacer clic en "Vista Previa Columnas"

**Resultado esperado:**
```
?? INFORMACIÓN DEL CUADERNO

Columnas del Cuaderno (3):

?? Tarea 1 (30%)
  ? Rúbrica Tarea 1

?? Tarea 2 (30%)  
  ? Rúbrica Tarea 2

?? Proyecto 1 (40%)
  ? Rúbrica Proyecto 1
```

### Caso 3: Estadísticas

**Pasos:**
1. Seleccionar Materia y Período
2. Hacer clic en "Estadísticas"

**Resultado esperado:**
```
?? ESTADÍSTICAS DEL CUADERNO

?? 5 Estudiantes
?? 3 Instrumentos  
?? 3 Rúbricas
?? 67.00 Promedio

?? Detalles:
?? Nota máxima: 90.00
?? Nota mínima: 22.80
? Con todas las notas: 3
? Con notas pendientes: 2
```

### Caso 4: Exportación CSV

**Pasos:**
1. Generar el cuaderno
2. Hacer clic en "Exportar CSV"

**Resultado esperado:**
```
?? ARCHIVO CSV DESCARGADO

Nombre: CuadernoCalificador_Matemáticas_I_Primer_Cuatrimestre_2025_yyyyMMdd_HHmmss.csv

Contenido:
# Cuaderno Calificador - Matemáticas I
# Período: Primer Cuatrimestre 2025
# Generado: 2025-01-XX XX:XX:XX
# Total Estudiantes: 5
# Promedio General: 67.00

"Estudiante","Número ID","Tarea 1 ? Rúbrica Tarea 1","Tarea 2 ? Rúbrica Tarea 2","Proyecto 1 ? Rúbrica Proyecto 1","Total Final"
"Pérez Rodríguez, Juan Carlos","2025001","100.00","80.00","90.00","90.00"
...
```

### Caso 5: Modo de Cálculo SUMA

**Pasos:**
1. Cambiar "Modo de Cálculo" a "SUMA"
2. Generar cuaderno

**Comportamiento esperado:**
- Si hubiera múltiples rúbricas por instrumento, se sumarían
- Máximo 100 puntos por instrumento
- En este caso, comportamiento igual a PROMEDIO (una rúbrica por instrumento)

### Caso 6: Valor Por Defecto Personalizado

**Pasos:**
1. Cambiar "Valor por Defecto" a 50
2. Generar cuaderno

**Resultado esperado:**
```
?? Ana Patricia Ramírez: (88×0.30) + (82×0.30) + (50×0.40) = 71.00
?? Luis Fernando Torres: (76×0.30) + (50×0.30) + (50×0.40) = 57.80
```

## ?? Verificaciones de Calidad

### ? Checklist de Funcionamiento

- [ ] **Generación dinámica:** Las columnas se crean automáticamente basándose en relaciones
- [ ] **Cálculo correcto:** Las fórmulas matemáticas son exactas
- [ ] **Manejo de faltantes:** Los valores por defecto se aplican correctamente
- [ ] **Ponderaciones:** Los porcentajes suman 100% y se aplican correctamente
- [ ] **UTF-8:** Los caracteres especiales (ñ, acentos) se muestran correctamente
- [ ] **Responsividad:** La tabla se adapta a diferentes tamaños de pantalla
- [ ] **Exportación:** El CSV se descarga con formato correcto
- [ ] **Estadísticas:** Los cálculos agregados son precisos

### ?? Casos Edge a Verificar

1. **Sin estudiantes:** Materia sin estudiantes inscritos
2. **Sin instrumentos:** Materia sin instrumentos asignados
3. **Sin rúbricas:** Instrumentos sin rúbricas asignadas
4. **Período inválido:** Seleccionar combinación inexistente
5. **Evaluaciones parciales:** Estudiantes con solo algunas evaluaciones
6. **Ponderaciones faltantes:** Instrumentos sin ponderación definida

## ?? Resolución de Problemas

### Problema: "No se encontraron instrumentos y rúbricas"

**Causas posibles:**
- Script de datos no ejecutado completamente
- Período académico incorrecto
- Relaciones InstrumentoMaterias faltantes

**Solución:**
```sql
-- Verificar relaciones
SELECT COUNT(*) FROM InstrumentoMaterias WHERE MateriaId = 1 AND PeriodoAcademicoId = 1;
SELECT COUNT(*) FROM InstrumentoRubricas WHERE InstrumentoEvaluacionId IN (1,2,3);
```

### Problema: "Caracteres especiales mal codificados"

**Solución:**
- Verificar que el middleware UTF-8 esté activo
- Reiniciar la aplicación
- Verificar configuración del navegador

### Problema: "Cálculos incorrectos"

**Verificación:**
```sql
-- Verificar ponderaciones
SELECT ir.*, ie.Nombre, r.NombreRubrica 
FROM InstrumentoRubricas ir
JOIN InstrumentosEvaluacion ie ON ir.InstrumentoEvaluacionId = ie.InstrumentoId
JOIN Rubricas r ON ir.RubricaId = r.IdRubrica;
```

## ?? Datos de Referencia

### Ponderaciones Configuradas
- **Tarea 1:** 30%
- **Tarea 2:** 30%  
- **Proyecto 1:** 40%
- **Total:** 100%

### Estudiantes de Prueba
| ID | Nombre | Tarea 1 | Tarea 2 | Proyecto 1 | Total Esperado |
|----|--------|---------|---------|------------|----------------|
| 1 | Juan Carlos Pérez | 100 | 80 | 90 | 90.00 |
| 2 | María José González | 85 | 75 | 88 | 83.20 |
| 3 | Carlos Alberto Martínez | 92 | 78 | 95 | 88.60 |
| 4 | Ana Patricia Ramírez | 88 | 82 | - | 51.00 |
| 5 | Luis Fernando Torres | 76 | - | - | 22.80 |

---

## ?? Objetivo de las Pruebas

Verificar que el **Cuaderno Calificador Automático PQ2025** cumple con todos los requisitos:

? **No intrusividad:** Sin modificar entidades existentes  
? **Descubrimiento dinámico:** Columnas basadas en relaciones actuales  
? **Cálculo automático:** Fórmulas correctas con ponderaciones  
? **Período específico:** Filtrado por "Primer Cuatrimestre 2025"  
? **Exportación funcional:** CSV con metadatos  
? **Interfaz intuitiva:** UX/UI responsiva y clara  
? **Caracteres UTF-8:** Soporte completo para español  

¡El sistema está listo para uso en producción! ??