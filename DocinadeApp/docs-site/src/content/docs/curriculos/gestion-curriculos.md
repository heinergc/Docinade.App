---
title: Gestión de Currículos
description: Administra materias, planes de estudio y estructura curricular del MEP
head:
  - tag: meta
    attrs:
      property: og:title
      content: Gestión de Currículos - RubricasApp
  - tag: meta
    attrs:
      property: og:description
      content: Crea y administra materias, planes de estudio y estructura curricular según normativa MEP
sidebar:
  order: 1
---

El módulo de **Currículos** permite gestionar la estructura académica de la institución, incluyendo materias, planes de estudio, créditos y distribución por ciclos educativos según las directrices del Ministerio de Educación Pública (MEP) de Costa Rica.

## Estructura Curricular

### Niveles Educativos Soportados

- **Primaria**: I Ciclo (1°-3°) y II Ciclo (4°-6°)
- **Secundaria**: 
  - III Ciclo (7°-9°)
  - Educación Diversificada (10°-11° o 10°-12°)
- **Educación Técnica**: Especialidades vocacionales

## Gestión de Materias

### Crear una Nueva Materia

1. Desde el menú principal, selecciona **Currículos** > **Materias**
2. Haz clic en **Nueva Materia**
3. Completa el formulario:

#### Información Básica
- **Código**: Identificador único (ej: MAT-10, ESP-09)
- **Nombre**: Nombre oficial de la materia
- **Créditos**: Valor crediticio según MEP
- **Tipo de materia**:
  - Académica básica
  - Académica especializada
  - Técnica
  - Artística
  - Deportiva
  - Complementaria

#### Configuración Académica
- **Ciclo sugerido**: Nivel educativo recomendado
- **Horas semanales**: Lecciones por semana
- **Requiere laboratorio**: Sí/No
- **Materia co-requisito**: Dependencias académicas
- **Materia prerequisito**: Requisitos previos

#### Descripción y Objetivo
- **Descripción breve**: Resumen de la materia
- **Objetivo general**: Meta educativa principal
- **Contenidos principales**: Temas a desarrollar

### Campos de la Materia

| Campo | Tipo | Descripción | Requerido |
|-------|------|-------------|-----------|
| Codigo | Texto | Identificador único (ej: MAT-10) | Sí |
| Nombre | Texto | Nombre oficial MEP | Sí |
| Creditos | Número | Valor crediticio (0-10) | Sí |
| Tipo | Lista | Categoría de materia | Sí |
| CicloSugerido | Lista | Nivel educativo | No |
| HorasSemanales | Número | Lecciones por semana | Sí |
| RequiereLaboratorio | Booleano | Si necesita laboratorio | No |
| Descripcion | Texto largo | Descripción detallada | No |
| ObjetivoGeneral | Texto largo | Meta educativa | No |
| Estado | Booleano | Activa/Inactiva | Sí |

## Materias por Ciclo Educativo

### III Ciclo (7°-9°)

**Materias Básicas:**
- Español
- Matemáticas
- Estudios Sociales
- Ciencias Naturales
- Inglés
- Educación Física
- Artes Plásticas o Música
- Educación Religiosa (optativa)

**Carga Horaria**: 40 lecciones semanales

### Educación Diversificada (10°-11°)

**Rama Académica:**
- Español
- Matemáticas
- Biología
- Física
- Química
- Estudios Sociales
- Inglés
- Educación Física
- Psicología
- Filosofía

**Rama Técnica:**
- Materias académicas básicas
- Especialidad técnica (múltiples lecciones)
- Práctica supervisada

## Planes de Estudio

### Crear un Plan de Estudio

Un plan de estudio agrupa materias para un nivel educativo específico:

1. Ve a **Currículos** > **Planes de Estudio**
2. Clic en **Nuevo Plan**
3. Define:
   - **Nombre del plan**: (ej: "Décimo Año - Académico 2025")
   - **Nivel educativo**: Selecciona el nivel
   - **Modalidad**: Académica, Técnica, Artística
   - **Año lecivo**: Periodo de vigencia
   - **Estado**: Activo/Inactivo

4. **Asignar materias al plan**:
   - Selecciona cada materia
   - Define si es **obligatoria** u **optativa**
   - Establece el **orden de evaluación**
   - Asigna **ponderación** (% de la nota final)

### Ejemplo: Plan de Estudios Décimo Año

| Materia | Tipo | Créditos | % Nota Final | Lecciones/semana |
|---------|------|----------|--------------|------------------|
| Español | Obligatoria | 5 | 15% | 5 |
| Matemáticas | Obligatoria | 5 | 15% | 5 |
| Biología | Obligatoria | 4 | 12% | 4 |
| Física | Obligatoria | 4 | 12% | 4 |
| Química | Obligatoria | 4 | 12% | 4 |
| Estudios Sociales | Obligatoria | 4 | 12% | 4 |
| Inglés | Obligatoria | 4 | 12% | 4 |
| Educación Física | Obligatoria | 2 | 5% | 2 |
| Filosofía | Obligatoria | 3 | 5% | 3 |

**Total**: 35 créditos, 35 lecciones semanales

## Asignación de Materias a Grupos

### Vincular Materia con Grupo

Una vez creadas las materias y los grupos:

1. Ve a **Grupos** > Selecciona un grupo
2. Clic en **Asignar Materias**
3. Selecciona el **plan de estudio** (opcional)
4. Marca las materias a asignar
5. Para cada materia:
   - Asigna el **profesor** que la impartirá
   - Define **horario** (días y horas)
   - Establece **modalidad** (presencial, virtual, híbrida)

### Validaciones Automáticas

El sistema verifica:
- ✅ Que el grupo tenga capacidad para las lecciones
- ✅ Que no haya conflictos de horario
- ✅ Que el profesor tenga disponibilidad
- ✅ Que las materias correspondan al nivel educativo

## Instrumentos de Evaluación por Materia

### Tipos de Instrumentos

Para cada materia puedes definir instrumentos de evaluación:

#### Pruebas Escritas
- **Peso**: 30-50% según MEP
- Cantidad por periodo
- Fecha de aplicación
- Contenidos evaluados

#### Trabajos Cotidianos
- **Peso**: 20-30%
- Asignaciones diarias
- Participación en clase
- Tareas

#### Proyecto
- **Peso**: 15-25%
- Individual o grupal
- Rúbrica de evaluación
- Presentación oral

#### Concepto / Actitud
- **Peso**: 5-15%
- Asistencia
- Puntualidad
- Participación
- Respeto a normas

### Configuración de Ponderaciones

```
Matemáticas - Décimo Año:
├── Pruebas escritas: 50%
│   ├── Prueba 1: 15%
│   ├── Prueba 2: 15%
│   └── Prueba 3: 20%
├── Trabajo cotidiano: 25%
├── Proyecto: 15%
└── Concepto: 10%
Total: 100%
```

## Exportación e Importación

### Importar Materias desde Excel

1. Descarga la **plantilla de importación**
2. Llena los datos de cada materia
3. Sube el archivo Excel
4. Revisa el resumen de importación
5. Confirma para crear las materias

**Formato de plantilla Excel:**
| Codigo | Nombre | Creditos | Tipo | CicloSugerido | HorasSemanales |
|--------|--------|----------|------|---------------|----------------|
| MAT-10 | Matemáticas | 5 | Académica básica | 10 | 5 |
| ESP-10 | Español | 5 | Académica básica | 10 | 5 |

### Exportar Plan de Estudios

Genera un documento PDF o Excel con:
- Listado completo de materias
- Distribución de créditos
- Carga horaria total
- Profesores asignados
- Horarios por grupo

## Cumplimiento Normativo MEP

### Reglamentos Aplicados

El sistema aplica automáticamente:
- **REA 40862-V21**: Evaluación de los aprendizajes
- **Decreto Ejecutivo 38949-MEP**: Reglamento de Evaluación
- **Programas de Estudio MEP**: Contenidos oficiales

### Validaciones de Cumplimiento

✅ **Carga horaria mínima** según nivel educativo  
✅ **Distribución de ponderaciones** según lineamientos MEP  
✅ **Materias obligatorias** incluidas en todos los planes  
✅ **Créditos mínimos** para promoción de nivel  

## Reportes Curriculares

### Reporte de Malla Curricular
Muestra la estructura completa del plan de estudios:
- Materias por nivel
- Créditos totales
- Distribución horaria
- Profesores asignados

### Reporte de Cobertura Curricular
Identifica:
- Materias sin profesor asignado
- Grupos sin plan de estudios completo
- Materias con carga horaria insuficiente

### Reporte de Avance Programático
Para cada materia:
- Contenidos planificados vs impartidos
- Porcentaje de avance
- Temas pendientes
- Proyección de cierre de programa

## Mejores Prácticas

1. **Define códigos consistentes** para las materias (ej: MAT-07, MAT-08, MAT-09)
2. **Revisa anualmente** los planes de estudio según actualizaciones del MEP
3. **Documenta cambios** en los programas de cada materia
4. **Mantén actualizada** la información de créditos y horas
5. **Valida ponderaciones** al inicio de cada periodo académico
6. **Comunica cambios** curriculares a profesores y estudiantes

## Integración con Otros Módulos

| Módulo | Integración |
|--------|-------------|
| **Profesores** | Asignación de materias a impartir |
| **Grupos** | Vinculación de plan de estudios |
| **Evaluaciones** | Instrumentos por materia |
| **Calificaciones** | Ponderaciones y cálculo de notas |
| **Reportes** | Estadísticas de rendimiento por materia |

## Permisos Requeridos

| Acción | Permiso |
|--------|---------|
| Ver materias | `Ver.Materias` |
| Crear materias | `Crear.Materias` |
| Editar materias | `Editar.Materias` |
| Asignar a grupos | `Asignar.Materias` |
| Ver planes de estudio | `Ver.Planes` |
| Crear planes | `Crear.PlanesCurriculares` |

## Preguntas Frecuentes

**¿Puedo modificar una materia que ya tiene evaluaciones registradas?**  
Sí, pero los cambios no afectan retroactivamente las evaluaciones existentes.

**¿Cómo agrego una materia nueva a mitad de año?**  
Crea la materia normalmente y asígnala a los grupos. Las evaluaciones iniciarán desde ese momento.

**¿El sistema valida que las ponderaciones sumen 100%?**  
Sí, no permitirá guardar instrumentos si la suma no es exactamente 100%.

**¿Puedo tener materias compartidas entre varios niveles?**  
Sí, como Educación Física o Artes que pueden compartirse entre grados.
