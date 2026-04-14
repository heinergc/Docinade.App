---
title: Gestión de Asignaciones
description: Administra la asignación de estudiantes a grupos y materias a profesores
head:
  - tag: meta
    attrs:
      property: og:title
      content: Gestión de Asignaciones - RubricasApp
  - tag: meta
    attrs:
      property: og:description
      content: Aprende a asignar estudiantes a grupos, materias a profesores y gestionar cambios durante el periodo académico
sidebar:
  order: 1
---

El módulo de **Asignaciones** es el centro de coordinación académica, donde se vinculan estudiantes con grupos, materias con profesores, y se gestionan todos los cambios durante el periodo lectivo.

## Tipos de Asignaciones

RubricasApp gestiona tres tipos principales de asignaciones:

1. **Estudiante → Grupo**: Matricular estudiantes en grupos específicos
2. **Materia → Grupo**: Vincular materias al plan de estudios de un grupo
3. **Profesor → Materia**: Asignar docentes para impartir materias específicas

## Asignación de Estudiantes a Grupos

### Proceso de Matrícula Individual

1. Ve a **Estudiantes** > Selecciona un estudiante
2. Clic en **Asignar a Grupo**
3. Selecciona:
   - **Periodo académico**
   - **Nivel educativo**
   - **Grupo específico** (ej: 10-A, 11-B)
4. Define:
   - **Fecha de asignación** (por defecto: hoy)
   - **Motivo de asignación**:
     - Matrícula nueva
     - Traslado interno
     - Reingreso
     - Repitencia
5. Confirma la asignación

### Asignación Masiva de Estudiantes

Para asignar múltiples estudiantes simultáneamente:

1. Ve a **Grupos** > Selecciona el grupo
2. Clic en **Agregar Estudiantes**
3. Opciones disponibles:
   - **Selección manual**: Marca estudiantes de la lista
   - **Importar Excel**: Sube archivo con lista de IDs
   - **Promoción automática**: Importa desde grupo anterior

#### Ejemplo de Importación Excel

| NumeroId | Nombre | Apellidos | FechaAsignacion | Motivo |
|----------|--------|-----------|-----------------|--------|
| 1-2345-6789 | Juan | Pérez Mora | 01/02/2025 | Matrícula nueva |
| 2-3456-7890 | María | Rodríguez López | 01/02/2025 | Traslado interno |

### Estados de Asignación

| Estado | Descripción | Visible en listados |
|--------|-------------|---------------------|
| **Activo** | Estudiante matriculado actualmente | ✅ Sí |
| **Retirado** | Estudiante se retiró del grupo | ❌ No |
| **Trasladado** | Movido a otro grupo | ❌ No |
| **Suspendido** | Suspensión temporal | ⚠️ Con alerta |
| **Graduado** | Completó el nivel educativo | ❌ No |

### Desasignar o Trasladar Estudiante

#### Desasignación (Retiro)
1. Desde el grupo, localiza al estudiante
2. Clic en **Acciones** > **Retirar del grupo**
3. Completa:
   - **Fecha de retiro**
   - **Motivo**:
     - Traslado a otra institución
     - Abandono
     - Cambio de modalidad
     - Graduación
     - Otra (especificar)
4. Confirma el retiro

El estudiante permanece en el sistema pero no aparecerá en listados activos del grupo.

#### Traslado Interno
1. Desde el perfil del estudiante
2. Clic en **Trasladar de Grupo**
3. Selecciona:
   - **Grupo de origen** (actual)
   - **Grupo de destino**
   - **Fecha efectiva del traslado**
   - **Motivo del traslado**
4. El sistema:
   - Cierra la asignación en el grupo anterior
   - Crea nueva asignación en grupo nuevo
   - Transfiere calificaciones parciales (si aplica)
   - Registra el cambio en auditoría

## Asignación de Materias a Grupos

### Proceso de Asignación

1. Ve a **Grupos** > Selecciona un grupo
2. Clic en **Gestionar Materias**
3. Elige un **Plan de Estudios** (opcional pero recomendado)
4. Selecciona las materias a asignar
5. Para cada materia configura:
   - **Profesor asignado**
   - **Lecciones semanales**
   - **Horario** (días y horas)
   - **Aula/Laboratorio** (si aplica)
   - **Modalidad**: Presencial, Virtual, Híbrida

### Validaciones Automáticas

El sistema valfica:
- ✅ Que el grupo no exceda la carga horaria máxima
- ✅ Que no haya conflictos de horario (mismo día/hora)
- ✅ Que el profesor tenga disponibilidad en ese horario
- ✅ Que las materias correspondan al nivel educativo del grupo

### Ver Carga Horaria del Grupo

En la vista de grupo puedes ver:
- **Total de materias asignadas**
- **Total de lecciones semanales**
- **Horario semanal completo**
- **Profesores involucrados**
- **Aulas utilizadas**

## Asignación de Profesores a Materias

### Método 1: Desde el Grupo

1. **Grupos** > Selecciona grupo > **Materias**
2. Para cada materia sin profesor:
3. Clic en **Asignar Profesor**
4. Selecciona el docente de la lista
5. El sistema filtra automáticamente profesores según:
   - Especialidad coincide con la materia
   - Disponibilidad horaria
   - Carga académica actual

### Método 2: Desde el Profesor

1. **Profesores** > Selecciona profesor
2. Clic en **Asignar Materias**
3. Selecciona **periodo académico**
4. Define grupos y materias:
   - Marca cada combinación Grupo-Materia
   - Establece horario
   - Confirma asignaciones

### Vista de Carga Académica del Profesor

| Grupo | Materia | Lecciones/semana | Horario |
|-------|---------|------------------|---------|
| 10-A | Matemáticas | 5 | L-M-X-J-V 8:00-8:40 |
| 10-B | Matemáticas | 5 | L-M-X-J-V 9:00-9:40 |
| 11-A | Física | 4 | L-M-X-J 10:00-10:40 |

**Total lecciones**: 14 de 20 asignadas (70% de capacidad)

### Reasignar Profesor

Si necesitas cambiar de profesor durante el periodo:

1. Ve al grupo > Materias
2. Selecciona la materia
3. Clic en **Cambiar Profesor**
4. Elige el nuevo profesor
5. Define **fecha efectiva del cambio**
6. Agrega **motivo del cambio**:
   - Licencia médica
   - Permiso
   - Redistribución de carga
   - Incapacidad

El sistema:
- Cierra la asignación anterior
- Crea nueva asignación desde la fecha indicada
- Transfiere acceso a calificaciones
- Notifica al nuevo profesor (si está configurado)

## Gestión de Horarios

### Crear Horario Automático

RubricasApp puede generar horarios automáticamente:

1. **Grupos** > **Herramientas** > **Generar Horario**
2. Define parámetros:
   - **Lecciones por día**: (ej: 7 lecciones)
   - **Duración por lección**: (ej: 40 minutos)
   - **Recreos**: Define horarios de descanso
   - **Restricciones**:
     - No laboratorios después de recreo largo
     - Máximo 2 lecciones consecutivas por materia
     - Educación Física en horarios específicos

3. El sistema:
   - Distribuye materias equitativamente
   - Respeta disponibilidad de profesores
   - Evita conflictos de aulas/laboratorios
   - Genera horario óptimo

### Horario Manual

Si prefieres control total:

1. Desde el grupo, ve a **Horario**
2. Haz clic en cada celda (día/hora)
3. Asigna:
   - Materia
   - Profesor
   - Aula
   - Tipo de lección
4. Guarda los cambios

El sistema alertará si detecta conflictos.

## Reportes de Asignaciones

### Reporte de Matrícula por Grupo

Muestra para cada grupo:
- Total de estudiantes matriculados
- Estudiantes activos
- Traslados en el periodo
- Retiros en el periodo
- Tasa de retención

### Reporte de Carga Académica

Para cada profesor:
- Grupos atendidos
- Materias impartidas
- Total de lecciones
- Porcentaje de capacidad
- Estudiantes bajo su responsabilidad

### Reporte de Asignaciones Pendientes

Lista:
- Grupos sin profesor guía
- Materias sin profesor asignado
- Estudiantes sin grupo
- Profesores con sobrecarga

### Reporte de Cambios de Asignación

Auditoría completa de:
- Traslados de estudiantes con fechas y motivos
- Cambios de profesor con justificación
- Modificaciones de horario
- Usuario que realizó cada cambio

## Reglas de Negocio

### Capacidad de Grupos

- **Mínimo**: 15 estudiantes (alerta si está por debajo)
- **Máximo**: 40 estudiantes (no permite exceder)
- **Óptimo**: 25-35 estudiantes (rango recomendado MEP)

### Carga de Profesores

- **Tiempo completo**: 20 lecciones mínimo, 22 máximo
- **Medio tiempo**: 10-11 lecciones
- **Interino**: Según contrato, máximo 22 lecciones

### Traslados de Estudiantes

- ❌ **No permitido** después del cierre del I Trimestre
- ⚠️ **Requiere aprobación** del director si hay calificaciones registradas
- ✅ **Permitido libremente** durante el periodo de matrícula

## Integración con Otros Módulos

Al asignar estudiantes y materias, automáticamente se activan:
- **Evaluaciones**: El profesor puede registrar calificaciones
- **Asistencia**: Se habilita el registro diario
- **Conducta**: Se pueden registrar incidentes
- **Reportes**: Generación de documentos oficiales

## Permisos Requeridos

| Acción | Permiso |
|--------|---------|
| Asignar estudiantes a grupos | `Asignar.Estudiantes` |
| Trasladar estudiantes | `Trasladar.Estudiantes` |
| Asignar materias a grupos | `Asignar.Materias` |
| Asignar profesores a materias | `Asignar.Profesores` |
| Generar horarios | `Crear.Horarios` |
| Ver carga académica | `Ver.CargaAcademica` |

## Mejores Prácticas

1. **Realiza asignaciones al inicio del periodo** para evitar problemas posteriores
2. **Valida la carga de profesores** antes de finalizar asignaciones
3. **Documenta todos los traslados** con motivo claro
4. **Revisa horarios** para evitar conflictos de aulas
5. **Mantén actualizado** el estado de cada asignación
6. **Exporta respaldos** antes de cambios masivos

## Preguntas Frecuentes

**¿Puedo asignar un estudiante a dos grupos simultáneamente?**  
No, cada estudiante solo puede estar en un grupo activo por periodo académico.

**¿Qué pasa con las calificaciones si traslado un estudiante?**  
Se transfieren automáticamente al nuevo grupo si las materias coinciden.

**¿Puedo deshacer una asignación?**  
Sí, siempre que no haya calificaciones registradas. De lo contrario requiere proceso de retiro formal.

**¿Cómo manejo estudiantes que repiten el año?**  
Crea nueva asignación en el grupo correspondiente con motivo "Repitencia". Las calificaciones del año anterior permanecen en el historial.

**¿El sistema notifica automáticamente los cambios?**  
Depende de la configuración. Puedes activar notificaciones por email a profesores y estudiantes afectados.
