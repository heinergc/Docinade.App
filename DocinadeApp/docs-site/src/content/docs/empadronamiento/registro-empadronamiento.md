---
title: Registro de Empadronamiento
description: Sistema completo de empadronamiento de estudiantes según normativa MEP
head:
  - tag: meta
    attrs:
      property: og:title
      content: Registro de Empadronamiento - RubricasApp
  - tag: meta
    attrs:
      property: og:description
      content: Proceso completo para empadronar estudiantes con datos personales, académicos, médicos y familiares
sidebar:
  order: 1
---

El módulo de **Empadronamiento** es el sistema de registro completo del estudiante que recopila toda la información requerida por el MEP para la matrícula oficial. Va más allá del registro básico e incluye datos personales, académicos, médicos, socioeconómicos y familiares.

## ¿Qué es el Empadronamiento?

El **empadronamiento** es el proceso formal de registro y actualización de información completa del estudiante según lo establece el MEP. Es requisito obligatorio para:
- Matrícula oficial
- Participación en programas de becas
- Acceso a servicios estudiantiles
- Generación de documentos oficiales
- Reportes estadísticos institucionales

## Etapas del Empadronamiento

El proceso se divide en **7 etapas** progresivas:

### 1️⃣ Datos Personales Básicos
- Nombres completos
- Apellidos
- Número de identificación (cédula, DIMEX, pasaporte)
- Fecha de nacimiento
- Género
- Nacionalidad
- Estado civil

### 2️⃣ Información de Residencia
- **Provincia**
- **Cantón**
- **Distrito**
- **Barrio o comunidad**
- **Señas exactas**
- **Distancia a la institución** (km)
- **Medio de transporte usual**

### 3️⃣ Contacto y Comunicación
- **Teléfono residencial**
- **Teléfono celular**
- **Email personal**
- **Email institucional** (asignado por el centro educativo)
- **Redes sociales** (opcional)
- **Persona de contacto de emergencia**:
  - Nombre completo
  - Parentesco
  - Teléfonos
  - Dirección

### 4️⃣ Datos Académicos
- **Institución de procedencia** (si viene de otro centro)
- **Último nivel cursado**
- **Año de graduación** (si aplica)
- **Promedio de nivel anterior**
- **Repitencias** (indicar cuáles y cuántas)
- **Adecuaciones curriculares**:
  - Tipo de adecuación (significativa, no significativa, de acceso)
  - Condición que la justifica
  - Documentación de apoyo
- **Idiomas que domina**
- **Habilidades especiales**

### 5️⃣ Información Médica y de Salud
- **Tipo de sangre**
- **Condiciones médicas conocidas**:
  - Alergias (especificar)
  - Enfermedades crónicas
  - Medicamentos de uso continuo
- **Requiere atención especial**: Sí/No
- **Condición de discapacidad** (según CNREE):
  - Física
  - Visual
  - Auditiva
  - Cognitiva
  - Múltiple
  - Otra
- **Porcentaje de discapacidad**
- **Número de carné CONAPDIS** (si aplica)
- **Seguro médico**:
  - CCSS (número de asegurado)
  - Privado (compañía y póliza)

### 6️⃣ Información Familiar
#### Datos del Padre/Tutor 1
- Nombre completo
- Cédula/identificación
- Parentesco
- Ocupación/Profesión
- Lugar de trabajo
- Teléfonos
- Email
- Nivel educativo
- Vive con el estudiante: Sí/No

#### Datos de la Madre/Tutor 2
- Misma información que padre/tutor 1

#### Datos Socioeconómicos
- **Tipo de vivienda**:
  - Casa propia
  - Casa alquilada
  - Prestada
  - Otro
- **Servicios en la vivienda**:
  - Agua potable
  - Electricidad
  - Internet
  - Teléfono fijo
- **Cantidad de personas en el hogar**
- **Ingreso familiar mensual aproximado** (rangos)
- **Recibe alguna beca o ayuda**: Sí/No (especificar)

### 7️⃣ Información Adicional
- **Intereses y pasatiempos**
- **Deportes que practica**
- **Instrumentos musicales que toca**
- **Participación en actividades extracurriculares**
- **Aspiraciones académicas**
- **Carrera universitaria de interés**
- **Observaciones generales**
- **Autorización de uso de imagen**: Sí/No
- **Autorización para salidas educativas**: Sí/No

## Proceso de Empadronamiento

### Inicio del Proceso

1. Ve a **Estudiantes** > Selecciona un estudiante
2. Verás una alerta si el estudiante no tiene empadronamiento completo
3. Clic en **Completar Empadronamiento**
4. El sistema mostrará las 7 etapas

### Navegación por Etapas

- Completa cada etapa en orden
- Puedes **Guardar y continuar después** en cualquier momento
- El sistema indica el **% de avance**:
  ```
  Progreso: 3/7 etapas completadas (43%)
  ```
- Las etapas completadas muestran ✅
- La etapa actual muestra 📝
- Las etapas pendientes muestran ⏺️

### Campos Obligatorios vs Opcionales

#### Obligatorios para Matrícula Oficial ✅
- Todos los datos de la Etapa 1 (Personales)
- Residencia completa (Provinica, Cantón, Distrito)
- Al menos un teléfono de contacto
- Datos de al menos un tutor/encargado
- Autorización firmada (documento físico)

#### Opcionales pero Recomendados ⚠️
- Información médica detallada
- Datos socioeconómicos (para becas)
- Información adicional (intereses, aspiraciones)

### Guardar Progreso

- Clic en **Guardar y Continuar** al final de cada etapa
- El sistema guarda automáticamente cada 2 minutos
- Indicador visual: "Guardado automático hace 30 segundos"

### Finalizar Empadronamiento

1. Completa las 7 etapas
2. Revisa el **Resumen General**
3. Marca la casilla "Confirmo que la información es correcta y completa"
4. Clic en **Finalizar Empadronamiento**
5. El sistema genera automáticamente:
   - Ficha de empadronamiento (PDF)
   - Código QR del estudiante
   - Actualiza base de datos SIGED (si está integrado)

## Acceso al Empadronamiento

### Quién Puede Empadronar

| Rol | Permiso | Puede empadronar |
|-----|---------|------------------|
| **Administrador** | Completo | ✅ Todos los estudiantes |
| **Director** | Completo | ✅ Todos los estudiantes |
| **Personal administrativo** | Editar empadronamiento | ✅ Asignados |
| **Orientador** | Ver/Editar datos sensibles | ✅ Con autorización |
| **Profesor** | Solo lectura | ❌ Solo consulta |
| **Padre/Encargado** | Editar propios datos | ✅ Solo sus hijos (portal web) |

### Portal de Autoempadronamiento

RubricasApp incluye un portal público donde padres pueden iniciar el empadronamiento:

1. Accede a la URL pública (ej: `https://rubricas.edu.cr/empadronamiento`)
2. Ingresa:
   - Cédula del encargado
   - Código de acceso (enviado por la institución)
3. Completa el formulario en línea
4. Sube documentos requeridos (PDF):
   - Copia de cédula estudiante
   - Certificado de nacimiento
   - Constancia de notas (si aplica)
5. Envía la solicitud
6. La institución revisa y aprueba

**Ventajas:**
- Reduce carga administrativa
- Padres completan información desde casa
- Valida datos en tiempo real
- Acelera proceso de matrícula

## Actualización de Datos

### Cuándo Actualizar

El empadronamiento debe actualizarse:
- ✅ Al inicio de cada año lectivo
- ✅ Cuando cambia información de contacto
- ✅ Al cambiar de residencia
- ✅ Ante cambios en situación familiar
- ✅ Si hay nuevas condiciones médicas
- ✅ Actualización de encargados legales

### Proceso de Actualización

1. Busca al estudiante
2. Ve a **Empadronamiento** > **Actualizar Datos**
3. El sistema muestra los datos actuales
4. Modifica solo lo necesario
5. Agrega **motivo de actualización**
6. Guarda los cambios

El sistema registra:
- Qué campo se modificó
- Valor anterior y nuevo valor
- Fecha y hora del cambio
- Usuario que realizó la modificación

## Impresión de Documentos

### Ficha de Empadronamiento

Documento oficial con toda la información:

1. **Empadronamiento** > **Imprimir Ficha**
2. Selecciona formato:
   - **Completo**: Todas las 7 etapas
   - **Resumido**: Solo datos esenciales
   - **Para SIGED**: Formato requerido por MEP
3. Genera PDF

El documento incluye:
- Encabezado institucional
- Foto del estudiante
- Datos completos
- Firmas de validación (padre y funcionario)
- Código QR para verificación

### Listado de Empadronamientos

Genera reporte Excel con:
- Todos los estudiantes
- Estado de empadronamiento (completo, pendiente, incompleto)
- Etapas faltantes
- Última actualización

Útil para:
- Control de matrícula
- Reportes al MEP
- Análisis estadístico

## Validaciones y Alertas

### Validaciones Automáticas

El sistema valida:
- ✅ Formato de cédula costarricense
- ✅ Mayoría de edad de encargados
- ✅ Formato de email
- ✅ Formato de teléfono (8 dígitos)
- ✅ Coherencia de fechas (ej: fecha nacimiento < fecha actual)
- ✅ Códigos postales de Costa Rica

### Alertas del Sistema

- ⚠️ **Empadronamiento vencido** (más de 1 año sin actualizar)
- ⚠️ **Datos incompletos** (faltan campos obligatorios)
- ⚠️ **Sin contacto de emergencia**
- ⚠️ **Documentos digitales pendientes**
- ⚠️ **Adecuación sin documentación de respaldo**

## Reportes Estadísticos

### Reporte Demográfico
- Distribución por género
- Distribución por edad
- Procedencia geográfica (mapa de calor)
- Nacionalidades representadas

### Reporte Socioeconómico
- Tipo de vivienda (%)
- Acceso a servicios básicos
- Nivel educativo de encargados
- Estudiantes con necesidades de beca

### Reporte de Adecuaciones
- Total de estudiantes con ACS
- Tipos de adecuación (%)
- Condiciones que las justifican
- Distribución por nivel educativo

### Reporte de Cobertura
- Empadronamientos completos vs pendientes
- Promedio de días para completar proceso
- Etapas con mayor tasa de abandono

## Integración con SIGED-MEP

Si tu institución está conectada al Sistema de gestión de Estudiantes del MEP:

### Sincronización Automática

El sistema puede:
- ✅ Enviar datos de empadronamiento al SIGED
- ✅ Actualizar automáticamente en ambos sistemas
- ✅ Obtener validación de cédulas desde TSE
- ✅ Recibir notificaciones de discrepancias

### Configuración de Integración

Contacta al administrador del sistema para:
1. Activar módulo de integración SIGED
2. Configurar credenciales de acceso
3. Definir frecuencia de sincronización
4. Mapear campos personalizados

## Seguridad y Privacidad

### Protección de Datos

- 🔒 **Encriptación** de datos sensibles (salud, identificaciones)
- 🔒 **Acceso restringido** según permisos de usuario
- 🔒 **Auditoría completa** de quién accedió a qué información
- 🔒 **Backups automáticos** diarios

### Ley de Protección de Datos

El sistema cumple con la **Ley N° 8968** de Costa Rica:
- Consentimiento informado para uso de datos
- Derecho al olvido (eliminación de datos)
- Acceso del titular a su información
- Corrección de datos erróneos

## Mejores Prácticas

1. **Inicia el empadronamiento temprano** (antes del inicio de clases)
2. **Habilita el portal de autoempadronamiento** para reducir carga administrativa
3. **Valida documentos físicos** aunque se suban digitales
4. **Actualiza anualmente** aunque no haya cambios
5. **Capacita a personal** en el uso correcto del módulo
6. **Mantén respaldos** de documentos sensibles
7. **Revisa alertas** semanalmente para identificar pendientes

## Preguntas Frecuentes

**¿Es obligatorio completar todas las etapas?**  
Las etapas 1-3 y 6 (datos personales, residencia, contacto y familia) son obligatorias. Las demás son recomendadas.

**¿Pueden los padres modificar el empadronamiento después de aprobado?**  
Sí, pero requiere aprobación del centro educativo para reflejar los cambios.

**¿Qué hago si un estudiante no tiene encargado legal?**  
Registra al tutor asignado por PANI con la documentación correspondiente.

**¿Cómo manejo estudiantes extranjeros sin cédula?**  
Usa el DIMEX o pasaporte. El sistema acepta múltiples tipos de identificación.

**¿Se puede importar datos desde Excel?**  
Sí, hay una opción de importación masiva con plantilla predefinida para matrícula inicial.

**¿El empadronamiento afecta la matrícula?**  
Un estudiante puede matricularse con datos básicos, pero debe completar el empadronamiento durante el primer trimestre.
