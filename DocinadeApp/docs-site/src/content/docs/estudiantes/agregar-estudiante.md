---
title: Agregar Estudiantes
description: Cómo registrar nuevos estudiantes en el sistema
head:
  - tag: meta
    attrs:
      property: og:title
      content: Agregar Estudiantes
sidebar:
  order: 1
---

Esta guía te ayudará a registrar nuevos estudiantes en **RubricasApp** y asignarlos a grupos.

## Métodos para Agregar Estudiantes

Existen tres formas de agregar estudiantes:

1. **Agregar manualmente** (uno por uno)
2. **Importar desde Excel** (varios a la vez)
3. **Sincronizar con sistema externo** (si está disponible)

## Agregar Estudiante Manualmente

### Paso 1: Acceder al Módulo de Estudiantes

1. En el menú principal, haz clic en **Estudiantes**
2. Haz clic en el botón **+ Agregar Estudiante**

### Paso 2: Información Personal

Completa los siguientes campos:

#### Datos Básicos (Obligatorios)

- **Nombre**: Nombre completo del estudiante
- **Apellidos**: Apellidos completos
- **Número de Identificación**: Cédula o número de identidad
- **Fecha de Nacimiento**: Formato DD/MM/AAAA
- **Sexo**: Masculino o Femenino

#### Información de Contacto

- **Email**: Correo electrónico del estudiante (opcional)
- **Teléfono**: Número de contacto (opcional)
- **Dirección**: Dirección de residencia

#### Datos del Responsable

- **Nombre del Encargado**: Nombre completo del padre/madre/tutor
- **Teléfono del Encargado**: Número de contacto principal
- **Email del Encargado**: Correo del responsable
- **Parentesco**: Relación con el estudiante (padre, madre, tutor, etc.)

### Paso 3: Información Académica

- **Nivel Educativo**: Primaria, Secundaria, etc.
- **Año Actual**: Grado o año que cursa
- **Número de Estudiante**: Código interno (si aplica)

### Paso 4: Información Adicional

- **Foto del Estudiante**: Imagen (JPG/PNG, máximo 2MB)
- **Observaciones**: Notas especiales, condiciones médicas, etc.
- **Adecuaciones Curriculares**: Si requiere ACS (Adecuación Curricular Significativa)

### Paso 5: Asignar a Grupo

- **Grupo**: Selecciona el grupo al que pertenece
- **Periodo Académico**: Año lectivo actual

### Paso 6: Guardar

1. Verifica que toda la información esté correcta
2. Haz clic en **Guardar**
3. El sistema confirmará el registro

## Importar Estudiantes desde Excel

Para agregar múltiples estudiantes a la vez:

### Paso 1: Descargar Plantilla

1. Ve a **Estudiantes**
2. Haz clic en **Importar desde Excel**
3. Descarga la plantilla proporcionada

### Paso 2: Completar la Plantilla

La plantilla incluye las siguientes columnas:

| Columna | Descripción | Obligatorio |
|---------|-------------|-------------|
| Nombre | Nombre del estudiante | Sí |
| Apellidos | Apellidos completos | Sí |
| Identificacion | Cédula o ID | Sí |
| FechaNacimiento | DD/MM/AAAA | Sí |
| Sexo | M o F | Sí |
| Email | Correo electrónico | No |
| Telefono | Número de contacto | No |
| NombreEncargado | Nombre del responsable | No |
| TelefonoEncargado | Teléfono del responsable | No |
| Grupo | Código del grupo | Sí |

### Paso 3: Importar el Archivo

1. Guarda el archivo Excel con los datos completados
2. En **Estudiantes**, haz clic en **Importar desde Excel**
3. Selecciona tu archivo
4. El sistema validará los datos

### Paso 4: Revisar y Confirmar

1. Revisa la vista previa de los estudiantes a importar
2. El sistema mostrará posibles errores o advertencias
3. Corrige los errores si los hay
4. Haz clic en **Confirmar Importación**

## Editar Información de Estudiante

Para modificar datos de un estudiante existente:

1. Ve a **Estudiantes**
2. Busca al estudiante en la lista
3. Haz clic en **Editar** (icono de lápiz)
4. Modifica los campos necesarios
5. Haz clic en **Guardar Cambios**

## Ver Perfil del Estudiante

El perfil completo del estudiante muestra:

### Información General
- Datos personales
- Foto
- Información de contacto
- Responsable legal

### Información Académica
- Grupos actuales e históricos
- Materias inscritas
- Promedio general
- Evaluaciones

### Asistencia
- Registro de asistencia
- Porcentaje de ausencias
- Tardanzas
- Justificaciones

### Conducta
- Incidentes registrados
- Boletas de conducta
- Seguimientos

### Reportes
- Boletín de calificaciones
- Reporte de asistencia
- Reporte de conducta

## Gestionar Estudiantes con ACS

Para estudiantes con **Adecuación Curricular Significativa**:

### Activar ACS

1. Edita el perfil del estudiante
2. Marca la casilla **Requiere ACS**
3. Completa la información adicional:
   - Tipo de adecuación
   - Descripción detallada
   - Documentos de respaldo (opcional)
4. Guarda los cambios

### Implicaciones de ACS

Los estudiantes con ACS:
- Tienen rúbricas de evaluación adaptadas
- Pueden tener criterios diferentes
- Se generan reportes especializados
- Se identifican visualmente en listas

## Problemas Comunes

### No puedo agregar un estudiante

Verifica que:
- Tienes permisos de creación de estudiantes
- El número de identificación no esté duplicado
- Todos los campos obligatorios estén completos
- El grupo seleccionado exista y esté activo

### Error al importar Excel

Revisa que:
- El archivo tenga el formato correcto (.xlsx)
- Las columnas coincidan con la plantilla
- No haya filas vacías entre datos
- Los códigos de grupo existan en el sistema
- Las fechas estén en formato DD/MM/AAAA

### La foto no se carga

Asegúrate de que:
- La imagen sea JPG o PNG
- El tamaño no exceda 2MB
- La resolución sea razonable (max 2000x2000px)

---

**Siguiente paso**: Aprende a [Gestionar Rúbricas](/rubricas/crear-rubrica).
