---
title: Gestión de Profesores
description: Aprende a administrar el registro y datos de profesores en RubricasApp
head:
  - tag: meta
    attrs:
      property: og:title
      content: Gestión de Profesores - RubricasApp
  - tag: meta
    attrs:
      property: og:description
      content: Guía completa para administrar profesores, asignar materias y gestionar información docente en el sistema
sidebar:
  order: 1
---

El módulo de **Profesores** permite gestionar toda la información del personal docente de la institución, incluyendo datos personales, información laboral, especialidades y asignaciones de materias.

## Acceso al Módulo

1. Desde el menú principal, selecciona **Profesores**
2. Verás el listado de todos los profesores registrados
3. Puedes filtrar por provincia, estado (activo/inactivo) o buscar por nombre/cédula

## Agregar un Nuevo Profesor

### Formulario Multi-Step

RubricasApp utiliza un formulario de 4 pasos para facilitar el registro completo:

#### Paso 1: Datos Personales
- **Nombres completos**
- **Primer y segundo apellido**
- **Cédula** (validación automática con formato MEP)
- **Fecha de nacimiento**
- **Género**
- **Estado civil**
- **Nacionalidad**

#### Paso 2: Información de Contacto
- **Teléfono personal**
- **Email personal** (requerido)
- **Email institucional**
- **Dirección completa**:
  - Provincia
  - Cantón
  - Distrito
  - Señas exactas

#### Paso 3: Datos Laborales
- **Puesto** (Profesor, Director, Coordinador, etc.)
- **Especialidad** (selección múltiple):
  - Matemática
  - Español
  - Ciencias Naturales
  - Estudios Sociales
  - Inglés
  - Educación Física
  - Artes Plásticas
  - Música
  - Cómputo/Informática
  - Otras
- **Fecha de ingreso**
- **Estado laboral** (Interino, Propiedad, Temporal)
- **Jornada laboral** (Tiempo completo, Medio tiempo, Lecciones)
- **Cantidad de lecciones asignadas**

#### Paso 4: Información Académica
- **Grado académico**:
  - Bachillerato universitario
  - Licenciatura
  - Maestría
  - Doctorado
- **Título obtenido**
- **Institución que otorgó el título**
- **Año de graduación**
- **Campo de estudio**
- **Cursos de capacitación** (opcional)

### Acciones Disponibles

| Acción | Descripción | Permiso requerido |
|--------|-------------|-------------------|
| Crear profesor | Registrar nuevo docente | Crear.Profesores |
| Ver detalles | Consultar información completa | Ver.Profesores |
| Editar | Modificar datos del profesor | Editar.Profesores |
| Activar/Desactivar | Cambiar estado laboral | Editar.Profesores |
| Asignar materias | Vincular materias al profesor | Asignar.Materias |
| Ver historial | Consultar registro de cambios | Ver.Auditoria |

## Validación de Cédula

El sistema incluye validación automática para cédulas costarricenses:

### Tipos de Cédula Soportados
- **Física nacional**: 1-0000-0000 (9 dígitos)
- **Física de residencia**: 1-0000-0000 (formato especial)
- **DIMEX**: Para residentes extranjeros
- **Pasaporte**: Para docentes internacionales

El sistema valifica automáticamente el formato y dígito verificador según estándares del TSE de Costa Rica.

## Asignar Materias a Profesores

### Proceso de Asignación

1. Desde el perfil del profesor, haz clic en **Asignar Materias**
2. Selecciona el **periodo académico**
3. Marca las materias que impartirá el profesor
4. Para cada materia, indica:
   - Grupos a los que impartirá la materia
   - Cantidad de lecciones semanales
   - Horario (opcional)
5. Guarda los cambios

### Vista de Carga Académica

El sistema muestra automáticamente:
- Total de lecciones asignadas
- Grupos atendidos
- Materias impartidas por periodo
- Alertas si excede la carga máxima (según jornada laboral)

## Filtros y Búsqueda

### Búsqueda Rápida
Escribe en el campo de búsqueda para filtrar por:
- Nombre completo
- Cédula
- Email
- Especialidad

### Filtros Avanzados
- **Por provincia**: Filtra profesores por ubicación geográfica
- **Por estado**: Activos, inactivos o todos
- **Por especialidad**: Filtra por área de enseñanza
- **Por institución**: Si hay múltiples centros educativos

## Exportación de Datos

### Opciones de Exportación

1. **Excel**: Exporta listado completo con todos los campos
2. **PDF**: Genera reporte imprimible con datos seleccionados
3. **CSV**: Para integración con otros sistemas

### Datos Exportados
- Información personal básica
- Contacto y dirección
- Datos laborales
- Carga académica actual
- Especialidades

## Reportes Disponibles

### Reporte de Carga Académica
Muestra para cada profesor:
- Materias asignadas
- Grupos atendidos
- Total de lecciones
- Porcentaje de carga laboral

### Reporte de Cumplimiento de Perfil
Identifica profesores que imparten materias fuera de su especialidad registrada.

### Reporte de Certificaciones
Lista profesores con certificaciones próximas a vencer (capacitaciones MEP).

## Perfil de Profesor Guía

### Asignar Profesor Guía a Grupo

1. Ve al módulo de **Grupos**
2. Selecciona el grupo
3. En la sección "Profesor Guía", elige el docente
4. El profesor tendrá acceso completo a:
   - Registro de conducta del grupo
   - Asistencia grupal
   - Evaluaciones del grupo
   - Comunicados a padres de familia

### Responsabilidades del Profesor Guía
- Supervisar conducta de estudiantes
- Aprobar boletas de conducta
- Coordinar reuniones con padres
- Generar reportes periódicos del grupo

## Integración con Otros Módulos

El módulo de Profesores se conecta con:

| Módulo | Integración |
|--------|-------------|
| **Materias** | Asignación de materias a impartir |
| **Grupos** | Designación como profesor guía |
| **Evaluaciones** | Registro de calificaciones |
| **Conducta** | Supervisión de incidentes |
| **Asistencia** | Registro de asistencia de estudiantes |
| **Reportes** | Generación automática de informes |

## Seguridad y Permisos

### Permisos del Módulo

| Permiso | Descripción |
|---------|-------------|
| `Ver.Profesores` | Visualizar listado y detalles |
| `Crear.Profesores` | Registrar nuevos profesores |
| `Editar.Profesores` | Modificar datos existentes |
| `Eliminar.Profesores` | Desactivar profesores |
| `Asignar.Materias` | Vincular materias a profesores |
| `Ver.CargaAcademica` | Consultar distribución de lecciones |

### Auditoría
Todos los cambios en el módulo de profesores quedan registrados:
- Usuario que realizó el cambio
- Fecha y hora exacta
- Datos anteriores y nuevos (para ediciones)
- Acción realizada

## Solución de Problemas

### La cédula no valida correctamente
- Verifica que el formato sea correcto (con guiones)
- Para DIMEX o pasaporte, selecciona el tipo correcto
- Contacta con soporte si el error persiste

### No puedo asignar una materia
- Verifica que la materia esté activa en el sistema
- Confirma que el periodo académico esté vigente
- Revisa que tengas el permiso `Asignar.Materias`

### El profesor no aparece en el listado de profesor guía
- Confirma que el profesor esté **Activo**
- Verifica que tenga al menos una especialidad registrada
- Actualiza la página y vuelve a intentar

## Mejores Prácticas

1. **Mantén actualizada la información de contacto** de los profesores
2. **Revisa periódicamente la carga académica** para evitar sobrecargas
3. **Actualiza las especialidades** cuando un profesor complete capacitaciones
4. **Asigna profesor guía al inicio de cada periodo académico**
5. **Exporta respaldos** mensuales de la base de datos de profesores
6. **Documenta cambios importantes** en las observaciones del perfil

## Preguntas Frecuentes

**¿Puedo importar profesores desde Excel?**  
Sí, desde el botón "Importar" puedes cargar un archivo Excel con formato predefinido.

**¿Qué pasa con los datos cuando desactivo un profesor?**  
Los datos permanecen en el sistema pero el profesor no aparece en asignaciones nuevas.

**¿Puedo asignar un profesor a múltiples grupos de la misma materia?**  
Sí, al asignar materias puedes seleccionar múltiples grupos.

**¿Cómo cambio de profesor guía en medio del año?**  
Edita el grupo y cambia el profesor guía. El cambio queda registrado en auditoría.
