---
title: Registro de Asistencia
description: Cómo registrar la asistencia diaria de estudiantes
head:
  - tag: meta
    attrs:
      property: og:title
      content: Registro de Asistencia
sidebar:
  order: 1
---

El módulo de asistencia permite llevar un control diario de la presencia de los estudiantes en el aula.

## ¿Qué es el Registro de Asistencia?

El registro de asistencia documenta:
- **Presencias** de estudiantes
- **Ausencias** (justificadas o injustificadas)
- **Tardanzas** (llegadas después del horario)
- **Salidas tempranas**

## Registrar Asistencia Diaria

### Paso 1: Acceder al Módulo

1. En el menú principal, haz clic en **Asistencia**
2. Selecciona **Registrar Asistencia**

### Paso 2: Seleccionar Grupo y Fecha

- **Grupo**: Selecciona el grupo a registrar
- **Materia**: Selecciona la materia (si aplica)
- **Fecha**: Fecha actual (predeterminada) o selecciona otra
- **Lección**: Número de lección (1, 2, 3, etc.)

### Paso 3: Marcar Asistencia

El sistema muestra la lista completa de estudiantes del grupo.

#### Opciones por Estudiante

Para cada estudiante puedes marcar:

- ✅ **Presente**: El estudiante está en clase
- ❌ **Ausente**: El estudiante no está
- ⏰ **Tardanza**: Llegó después del inicio
- 🚪 **Salida Temprana**: Se retiró antes del final

#### Método Rápido

Por defecto, todos están marcados como **Presente**. Solo marca las excepciones:
1. Haz clic en el ícono de estado del estudiante
2. Cambia a Ausente, Tardanza o Salida
3. Agrega observaciones si es necesario

#### Método Completo

Si prefieres marcar uno por uno:
1. Desactiva la opción **Marcar todos presentes**
2. Ve marcando el estado de cada estudiante
3. Guarda al finalizar

### Paso 4: Justificaciones

Para ausencias justificadas:

1. Haz clic en el estudiante ausente
2. Marca **Justificada**
3. Selecciona el motivo:
   - Enfermedad
   - Cita médica
   - Trámite familiar
   - Otro (especificar)
4. Adjunta comprobante (opcional)
5. Guarda

### Paso 5: Guardar Registro

1. Revisa que todos los estudiantes estén marcados
2. Haz clic en **Guardar Asistencia**
3. El sistema confirmará el registro

## Estados de Asistencia

### Presente
- Estudiante asistió a la totalidad de la lección
- Se marca con ✅
- Cuenta positivamente en estadísticas

### Ausente
- Estudiante no asistió a la lección
- Se marca con ❌
- Puede ser justificada o injustificada

#### Ausencia Justificada
- Con comprobante (nota médica, etc.)
- No afecta negativamente calificación de asistencia
- Se registra pero no cuenta como falta

#### Ausencia Injustificada
- Sin justificación válida
- Cuenta como falta
- Puede afectar la nota de conducta/asistencia

### Tardanza
- Estudiante llega después del inicio (más de 10 minutos)
- Se marca con ⏰
- Indica hora de llegada
- 3 tardanzas = 1 ausencia (configurable)

### Salida Temprana
- Estudiante se retira antes del final de lección
- Se marca con 🚪
- Indica hora de salida
- Requiere autorización o justificante

## Registrar Asistencia por Lección

Si trabajas por lecciones diarias:

### Configuración

1. Ve a **Configuración** → **Horarios**
2. Define las lecciones del día:
   - Lección 1: 7:00 - 7:40
   - Lección 2: 7:40 - 8:20
   - Recreo: 8:20 - 8:40
   - Lección 3: 8:40 - 9:20
   - etc.

### Registro por Lección

1. Selecciona el grupo
2. Selecciona la lección específica
3. Registra asistencia solo para esa lección
4. Guarda

**Ventaja**: Mayor precisión, detecta patrones de ausencias por materia.

## Registro Semanal/Mensual

Para ver y editar asistencia de periodos amplios:

1. Ve a **Asistencia** → **Vista de Calendario**
2. Selecciona el grupo
3. Selecciona semana o mes
4. Verás una matriz:
   - Filas: Estudiantes
   - Columnas: Días
5. Haz clic en cualquier celda para editar

## Justificar Ausencias Retroactivas

Si un estudiante trae justificante después:

1. Ve a **Asistencia** → **Historial**
2. Busca al estudiante
3. Localiza la ausencia
4. Haz clic en **Justificar**
5. Selecciona motivo y adjunta comprobante
6. Guarda

El sistema actualizará las estadísticas automáticamente.

## Reportes de Asistencia

### Reporte Individual

Para ver la asistencia de un estudiante:

1. Ve a **Asistencia** → **Reportes**
2. Selecciona **Reporte Individual**
3. Elige al estudiante
4. Define el periodo
5. Genera PDF o Excel

**Contenido**:
- Porcentaje de asistencia
- Días presentes/ausentes
- Tardanzas
- Gráfico mensual
- Comparativo con el grupo

### Reporte Grupal

Para todo el grupo:

1. Selecciona **Reporte Grupal**
2. Elige el grupo y periodo
3. Genera el reporte

**Contenido**:
- Listado completo
- Porcentaje por estudiante
- Promedio del grupo
- Estudiantes con más ausencias
- Estadísticas por mes

### Reporte de Tardanzas

Reporte especializado en llegadas tarde:

1. Selecciona **Reporte de Tardanzas**
2. Elige grupo y periodo
3. Genera

**Muestra**:
- Estudiantes con más tardanzas
- Días con más tardanzas
- Patrones horarios

## Alertas de Ausencias

### Configurar Alertas

El sistema puede notificar automáticamente:

1. Ve a **Configuración** → **Alertas de Asistencia**
2. Define umbrales:
   - Alerta después de 3 ausencias consecutivas
   - Alerta al superar 10% de inasistencias
   - Alerta crítica al 15%
3. Selecciona destinatarios:
   - Encargados del estudiante
   - Coordinador académico
   - Orientador
4. Guarda

### Notificaciones a Encargados

Cuando se activa una alerta:
- Se envía email al encargado
- Se genera reporte de asistencia
- Se sugiere reunión
- Se documenta en el expediente

## Estadísticas de Asistencia

### Panel de Control

El dashboard de asistencia muestra:

- **Porcentaje global** de asistencia institucional
- **Tendencias** por mes
- **Grupos con mejor/peor asistencia**
- **Estudiantes en riesgo** (más de 15% ausencias)
- **Días con más ausencias**

### Indicadores Clave

- **Asistencia Promedio**: Meta 95%+
- **Tasa de Justificación**: % de ausencias justificadas
- **Tardanzas**: Promedio por estudiante
- **Deserción**: Estudiantes sin asistir 5+ días consecutivos

## Importar/Exportar Asistencia

### Exportar a Excel

Para análisis externo:

1. Ve a **Asistencia** → **Exportar**
2. Selecciona formato y periodo
3. Descarga el archivo

**Formato**: Incluye todos los registros con fechas, estados, justificaciones.

### Importar desde Excel

Si migras de otro sistema:

1. Descarga la plantilla
2. Completa con datos históricos
3. Importa el archivo
4. Revisa la vista previa
5. Confirma

## Mejores Prácticas

### Puntualidad en el Registro

- Registra la asistencia **al inicio** de cada lección
- No esperes al final del día
- Actualiza si hay cambios durante la clase

### Documentación

- Siempre solicita **comprobantes** para justificaciones
- Digitaliza y archiva los documentos
- Mantén copias de respaldo

### Seguimiento

- Revisa semanalmente las estadísticas
- Contacta a encargados de estudiantes en riesgo
- Documenta todas las comunicaciones

### Coherencia

- Usa los mismos criterios para todos
- Define claramente qué es tardanza (10 min estándar)
- Sé consistente con justificaciones

## Problemas Comunes

### No aparecen todos los estudiantes

Verifica que:
- El grupo seleccionado sea el correcto
- Los estudiantes estén activos
- La materia esté asignada al grupo

### No puedo editar asistencia pasada

Depende de los permisos:
- Algunos usuarios solo pueden editar el día actual
- Solicita permiso al administrador si necesitas editar fechas antiguas

### Las estadísticas no coinciden

Asegúrate de que:
- El periodo seleccionado sea el correcto
- Se hayan guardado todos los registros
- No haya registros duplicados

---

**Siguiente paso**: Aprende a generar [Reportes](/reportes/reportes-generales).
