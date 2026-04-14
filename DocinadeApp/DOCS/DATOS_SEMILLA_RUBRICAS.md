# Datos Semilla de Rúbricas

## 📋 Resumen

Este documento describe los datos semilla (seed data) que se insertan automáticamente en la base de datos cuando la aplicación se ejecuta por primera vez.

## 🔧 Ejecución

Los datos se insertan automáticamente cuando:
- La aplicación se ejecuta por primera vez
- La base de datos no tiene datos en la tabla `PeriodosAcademicos`
- Se llama al método `DatabaseSeeder.SeedDatabase()` desde `SqlServerDatabaseInitializer`

## 📊 Datos que se Insertan

### 1. Período Académico
```
Código: PQ2025-1
Nombre: Primer Cuatrimestre 2025
Tipo: Cuatrimestre
Año: 2025
Número: 1
Fecha Inicio: 2025-01-15
Fecha Fin: 2025-05-15
Estado: Activo
```

### 2. Grupo de Calificación
```
Nombre: Evaluación Estándar
Descripción: Niveles de calificación estándar para evaluaciones generales
Estado: ACTIVO
```

### 3. ⚠️ Niveles de Calificación
**IMPORTANTE:** Los niveles de calificación ya **NO se insertan automáticamente**.
Deben ser creados manualmente por el usuario desde la interfaz:
- Navegar a: Configuración → Rúbricas → Niveles de Calificación
- Crear los niveles necesarios para cada grupo de calificación

**Nota:** Las rúbricas de ejemplo solo se insertarán si existen niveles de calificación en la base de datos.

### 4. Rúbricas (3 rúbricas completas) - Solo si hay niveles

#### Rúbrica 1: Proyecto de Investigación
**Descripción:** Evaluación del proyecto de investigación - Análisis completo con formulación y solución

**Items de Evaluación:**
1. **Análisis del problema** (25%)
   - Descripción: Identificación y análisis profundo del problema planteado

2. **Formulación matemática** (25%)
   - Descripción: Modelado matemático correcto del problema

3. **Solución e interpretación** (25%)
   - Descripción: Aplicación de métodos de solución e interpretación de resultados

4. **Presentación y documentación** (25%)
   - Descripción: Calidad de la presentación escrita y documentación del proyecto

#### Rúbrica 2: Presentación Oral
**Descripción:** Evaluación de la presentación oral del proyecto - Comunicación y dominio del tema

**Items de Evaluación:**
1. **Claridad en la exposición** (30%)
   - Descripción: Capacidad de comunicar ideas de forma clara y organizada

2. **Dominio del tema** (35%)
   - Descripción: Conocimiento profundo y respuestas a preguntas

3. **Material de apoyo** (20%)
   - Descripción: Calidad de las diapositivas y recursos visuales

4. **Gestión del tiempo** (15%)
   - Descripción: Uso efectivo del tiempo asignado

#### Rúbrica 3: Trabajo en Equipo
**Descripción:** Evaluación de las competencias de trabajo colaborativo

**Items de Evaluación:**
1. **Colaboración** (30%)
   - Descripción: Participación activa y apoyo a los compañeros

2. **Comunicación** (25%)
   - Descripción: Comunicación efectiva dentro del equipo

3. **Responsabilidad** (25%)
   - Descripción: Cumplimiento de tareas y compromisos

4. **Resolución de conflictos** (20%)
   - Descripción: Capacidad de resolver desacuerdos constructivamente

### 5. Valores de Rúbrica

Se crean **48 valores de rúbrica** en total:
- 3 rúbricas × 4 items por rúbrica × 4 niveles = 48 valores

Cada combinación de (Item, Nivel) tiene un valor de puntos:
- Excelente: 4.0 puntos
- Bueno: 3.0 puntos
- Regular: 2.0 puntos
- Deficiente: 1.0 puntos

### 6. RubricaNiveles

Se crean **12 asociaciones** entre rúbricas y niveles:
- 3 rúbricas × 4 niveles = 12 asociaciones

### 7. Estudiante de Prueba
```
Nombre: JUAN PABLO
Apellidos: ABARCA BRENES
Número ID: 020839034
Correo: juan.pablo@uned.cr
Institución: PALMARES (06)
Grupos: Grupo 02: Tutor Heiner Guido Cambronero
Año: 2025
```

## 📁 Archivos Modificados

1. **Data/DatabaseSeeder.cs**
   - Contiene la lógica completa de inserción de datos semilla
   - Incluye 3 rúbricas completas con todos sus componentes

2. **Utils/SqlServerDatabaseInitializer.cs**
   - Llama al `DatabaseSeeder.SeedDatabase()` después de inicializar los datos básicos
   - Se ejecuta automáticamente al iniciar la aplicación

## 🚀 Cómo Usar

1. **Inicialización automática:**
   ```bash
   dotnet run
   ```
   Los datos se insertarán automáticamente en el primer arranque.

2. **Reinsertar datos (si es necesario):**
   - Eliminar todos los registros de la tabla `PeriodosAcademicos`
   - Ejecutar la aplicación nuevamente

3. **Verificar datos insertados:**
   - Navegar a: Configuración → Rúbricas → Lista de Rúbricas
   - Deberías ver las 3 rúbricas creadas

## 🔍 Verificación

Para verificar que los datos se insertaron correctamente, ejecuta estas consultas SQL:

```sql
-- Verificar rúbricas
SELECT * FROM Rubricas;

-- Verificar items de evaluación
SELECT r.NombreRubrica, i.NombreItem, i.Peso
FROM ItemsEvaluacion i
INNER JOIN Rubricas r ON i.IdRubrica = r.IdRubrica
ORDER BY r.IdRubrica, i.OrdenItem;

-- Verificar valores de rúbrica
SELECT r.NombreRubrica, i.NombreItem, n.NombreNivel, v.ValorPuntos
FROM ValoresRubrica v
INNER JOIN Rubricas r ON v.IdRubrica = r.IdRubrica
INNER JOIN ItemsEvaluacion i ON v.IdItem = i.IdItem
INNER JOIN NivelesCalificacion n ON v.IdNivel = n.IdNivel
ORDER BY r.IdRubrica, i.OrdenItem, n.OrdenNivel;

-- Contar registros
SELECT 
    (SELECT COUNT(*) FROM Rubricas) as TotalRubricas,
    (SELECT COUNT(*) FROM ItemsEvaluacion) as TotalItems,
    (SELECT COUNT(*) FROM NivelesCalificacion) as TotalNiveles,
    (SELECT COUNT(*) FROM ValoresRubrica) as TotalValores;
```

**Resultado esperado (si existen niveles de calificación):**
- TotalRubricas: 3
- TotalItems: 12 (4 items × 3 rúbricas)
- TotalNiveles: Variable (depende de cuántos niveles creó el usuario)
- TotalValores: 12 items × cantidad de niveles

**Si NO hay niveles de calificación:**
- TotalRubricas: 0
- TotalItems: 0
- TotalNiveles: 0
- TotalValores: 0

## ⚠️ Notas Importantes

1. **Condición de ejecución:** Los datos solo se insertan si NO existen rúbricas en la base de datos.

2. **Niveles de calificación requeridos:** Las rúbricas de ejemplo solo se insertan si ya existen niveles de calificación creados manualmente por el usuario.

3. **Datos públicos:** Todas las rúbricas se crean con `EsPublica = 1`, lo que significa que son visibles para todos los usuarios.

4. **Grupos de calificación:** 
   - "Evaluación Estándar" se crea automáticamente (sin niveles)
   - "Proyecto IO 2025" se crea si se insertan las rúbricas de ejemplo

5. **Pesos:** Los pesos de los items suman 100% en cada rúbrica (25+25+25+25=100, 30+35+20+15=100, 30+25+25+20=100).

6. **Orden:** Los items y niveles tienen valores de orden que determinan su visualización en la interfaz.

## 🎯 Casos de Uso

Estas rúbricas de ejemplo son útiles para:
- **Demostración:** Mostrar el funcionamiento completo del sistema (requiere crear niveles primero)
- **Pruebas:** Realizar pruebas de evaluación sin necesidad de crear datos manualmente
- **Plantillas:** Servir como base para crear nuevas rúbricas personalizadas
- **Capacitación:** Entrenar a los usuarios en el uso del sistema

## 📝 Personalización

Para agregar más rúbricas de ejemplo:

1. Abrir `Data/DatabaseSeeder.cs`
2. Agregar nuevas instancias de `Rubrica` después de `rubrica3`
3. Crear los items de evaluación correspondientes
4. Agregar la nueva rúbrica al array en el bucle de `RubricaNiveles`
5. Los valores de rúbrica se crearán automáticamente

**Ejemplo:**
```csharp
var rubrica4 = new Rubrica
{
    NombreRubrica = "Nueva Rúbrica",
    Descripcion = "Descripción de la nueva rúbrica",
    Estado = "ACTIVO",
    FechaCreacion = DateTime.Now,
    EsPublica = 1,
    IdGrupo = grupo.IdGrupo
};
context.Rubricas.Add(rubrica4);
```

---

**Fecha de creación:** 11 de noviembre de 2025  
**Última actualización:** 11 de noviembre de 2025  
**Autor:** Sistema RubricasApp.Web
