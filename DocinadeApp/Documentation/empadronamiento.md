# Empadronamiento estudiantil

## Objetivo general

Consolidar la información necesaria para gestionar el proceso de empadronamiento y matrícula de estudiantes, garantizando que los datos capturados cubran aspectos personales, familiares, académicos y administrativos sin afectar la compatibilidad con el modelo actual.

## Categorías de información

### 1. Identificación y datos personales
- Datos básicos: nombres, apellidos, número de identificación nacional o pasaporte.
- Datos complementarios: fecha de nacimiento, género, nacionalidad, estado civil (si aplica).
- Documentos asociados: copias digitales de cédula, partida de nacimiento, certificado de notas.

### 2. Información de contacto y residencia
- Dirección completa (provincia, cantón, distrito, barrio, señas).
- Teléfonos (personal y alternativo) y correo electrónico.
- Comprobantes de domicilio (recibo de servicios, contrato de alquiler).

### 3. Responsables y tutores legales
- Datos de padre, madre y/o tutor: nombre completo, parentesco, ocupación, contacto.
- Identificación y documentación de cada responsable.
- Autorizaciones específicas (salidas, emergencias, uso de imagen).

### 4. Salud y necesidades especiales
- Información médica relevante: alergias, padecimientos crónicos, medicamentos.
- Contacto de emergencia y centro médico habitual.
- Certificados o constancias médicas que respalden condiciones especiales.

### 5. Historial académico previo
- Institución de procedencia, último nivel cursado, promedio general.
- Certificados de estudios anteriores y constancia de buena conducta.
- Observaciones sobre adaptaciones curriculares previas.

### 6. Documentación de soporte
- Lista de documentos recibidos y pendientes.
- Estado de revisión/validación por parte de la institución.
- Fechas de entrega y vencimiento de documentos sensibles (por ejemplo, pólizas).

### 7. Estado del proceso de empadronamiento
- Etapas del flujo (pre-registro, revisión documental, aprobación, matrícula finalizada).
- Fecha y usuario responsable de cada etapa.
- Comentarios internos y notas de seguimiento.

### 8. Integraciones y trazabilidad
- Referencias a otros sistemas (por ejemplo, plataforma del Ministerio o SIGED).
- Identificadores externos necesarios para reportes oficiales.
- Registro de consentimientos informados o contratos firmados digitalmente.

## Recomendaciones de implementación

1. **Compatibilidad**: agregar nuevas columnas como campos opcionales (nullable) o crear tablas relacionadas (`Responsables`, `DocumentosEstudiante`, `HistorialMedico`, etc.) para evitar romper flujos existentes.
2. **Captura gradual**: dividir el formulario de empadronamiento en secciones/multistep, permitiendo guardar avances parciales.
3. **Validación**: aplicar reglas específicas por etapa (por ejemplo, no exigir documentación médica si el estudiante no reporta condiciones especiales).
4. **Auditoría**: registrar quién cargó o modificó cada dato/documento y cuándo lo hizo.
5. **Automatización**: generar checklists y recordatorios automáticos para documentos faltantes o por vencer.

### Tabla hija propuesta `EstudianteEmpadronamiento`

Para preservar intacta la tabla `Estudiante` y su programación asociada, se sugiere crear una tabla hija 1 a 1 con la siguiente estructura:

- `IdEstudiante` **(PK y FK)**: misma clave primaria de `Estudiante`, garantiza relación directa sin modificar el esquema original.
- `NumeroId`: se duplica el identificador actual para mantener trazabilidad dentro del módulo de empadronamiento y facilitar integraciones externas.
- Campos sugeridos adicionales (todas como columnas opcionales):
	- Datos personales complementarios: `FechaNacimiento`, `Genero`, `Nacionalidad`, `EstadoCivil`.
	- Contacto y residencia: `Provincia`, `Canton`, `Distrito`, `Barrio`, `Senas`, `TelefonoAlterno`, `CorreoAlterno`.
	- Responsables: `NombrePadre`, `NombreMadre`, `NombreTutor`, `ContactoEmergencia`, `TelefonoEmergencia`, `RelacionEmergencia`.
	- Salud: `Alergias`, `CondicionesMedicas`, `Medicamentos`, `SeguroMedico`, `CentroMedicoHabitual`.
	- Historial académico: `InstitucionProcedencia`, `UltimoNivelCursado`, `PromedioAnterior`, `AdaptacionesPrevias`.
	- Documentación: `DocumentosRecibidosJson`, `DocumentosPendientesJson`, `FechaEntregaDocumentos`, `FechaVencimientoPoliza`.
	- Estado del proceso: `EtapaActual`, `FechaEtapa`, `UsuarioEtapa`, `NotasInternas`.

Consideraciones clave:

- Mantener todas las nuevas columnas como `NULL` permitiendo poblar los datos de forma gradual.
- Utilizar índices sobre `NumeroId` y campos de búsqueda frecuente (por ejemplo, `EtapaActual`).
- Describir las colecciones JSON en tablas hijas específicas cuando el volumen o la complejidad lo ameriten (`ResponsableEstudiante`, `DocumentoEmpadronamiento`, etc.).

## Próximos pasos sugeridos

1. Priorizar qué conjuntos de datos requieren soporte inmediato (por ejemplo, tutores legales y documentación).
2. Diseñar el esquema de base de datos (nuevos campos/tablas) y planificar las migraciones de EF Core.
3. Actualizar vistas y APIs para capturar y mostrar la información adicional.
4. Definir reportes o exportaciones necesarios para los procesos administrativos.
5. Elaborar políticas de privacidad y retención de datos vinculadas al empadronamiento.
