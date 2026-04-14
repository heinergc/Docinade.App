# 📋 Manual de Empadronamiento Estudiantil

**Sistema**: RubricasApp.Web  
**Fecha**: 16 de octubre de 2025  
**Versión**: 2.0  

---

## 🎯 ¿Qué es el Empadronamiento Estudiantil?

El empadronamiento estudiantil es un **proceso de registro público** que permite a los estudiantes inscribirse en el sistema educativo de forma autónoma, sin necesidad de crear una cuenta de usuario. Es un formulario multi-paso que recopila toda la información necesaria para el registro académico.

---

## 🚀 Acceso al Sistema

### 📍 **URL de Acceso**
```
https://localhost:18163/EmpadronamientoPublico
```

### 🔐 **Requisitos de Acceso**
- ✅ **No requiere autenticación** (proceso público)
- ✅ **No necesita crear cuenta** previamente
- ✅ **Disponible 24/7** mientras esté habilitado por administradores
- ⚠️ **Limitado por IP**: Máximo 5 intentos por hora por dirección IP

### 🎛️ **Control de Habilitación**
El sistema puede ser habilitado/deshabilitado por administradores desde:
- **Panel de administración**: `Admin/Configuracion`
- **Base de datos**: Configuración `EmpadronamientoPublico.Habilitar`

---

## 📋 Proceso Completo (5 Pasos)

### 🏠 **Página de Inicio**
**URL**: `/EmpadronamientoPublico/Index`

**Contenido**:
- Información general del proceso
- Requisitos y documentación necesaria
- Tiempo estimado: **10 minutos**
- Botón "Iniciar Empadronamiento"

**Información Requerida (Resumen)**:
- 📋 Datos personales (cédula, nombre, fecha de nacimiento)
- 📧 Información de contacto y residencia
- 👨‍👩‍👧‍👦 Datos de responsables y contacto de emergencia
- 🎓 Historial académico
- 🏥 Información de salud (opcional)

---

## 📊 **PASO 1: Datos Básicos**
**URL**: `/EmpadronamientoPublico/Paso1_DatosBasicos`

### 📝 **Campos Obligatorios**
| Campo | Tipo | Validación |
|-------|------|------------|
| **Número de Cédula** | Texto (20 chars) | ✅ Requerido, único en sistema |
| **Nombre(s)** | Texto (100 chars) | ✅ Requerido |
| **Apellido(s)** | Texto (100 chars) | ✅ Requerido |
| **Fecha de Nacimiento** | Fecha | ✅ Requerido |
| **Género** | Selección | ✅ Requerido |

### 📝 **Campos Opcionales**
| Campo | Tipo | Validación |
|-------|------|------------|
| **Nacionalidad** | Texto (50 chars) | Opcional |
| **Estado Civil** | Selección | Opcional |

### 🔍 **Validaciones Especiales**
- **Cédula única**: El sistema verifica que no exista otra inscripción con la misma cédula
- **Integración API**: Si es cédula costarricense, se consulta automáticamente la API nacional para autocompletar datos

### ➡️ **Flujo**
1. Usuario completa los campos
2. Sistema valida datos obligatorios
3. Sistema verifica que la cédula no esté duplicada
4. Si es válido → Redirige al **Paso 2**
5. Si hay errores → Muestra mensajes y mantiene en Paso 1

---

## 📧 **PASO 2: Contacto y Residencia**
**URL**: `/EmpadronamientoPublico/Paso2_ContactoResidencia`

### 📝 **Campos Obligatorios**
| Campo | Tipo | Validación |
|-------|------|------------|
| **Correo Electrónico Principal** | Email (100 chars) | ✅ Requerido, formato email |
| **Teléfono Principal** | Teléfono (20 chars) | ✅ Requerido, formato válido |
| **Provincia** | Texto (50 chars) | ✅ Requerido |
| **Cantón** | Texto (50 chars) | ✅ Requerido |
| **Distrito** | Texto (50 chars) | ✅ Requerido |
| **Señas Exactas** | Textarea (500 chars) | ✅ Requerido |

### 📝 **Campos Opcionales**
| Campo | Tipo | Validación |
|-------|------|------------|
| **Correo Alterno** | Email (100 chars) | Opcional, formato email |
| **Barrio** | Texto (100 chars) | Opcional |

### ➡️ **Flujo**
1. Usuario completa información de contacto
2. Sistema valida formatos (email, teléfono)
3. Si es válido → Redirige al **Paso 3**

---

## 👨‍👩‍👧‍👦 **PASO 3: Responsables**
**URL**: `/EmpadronamientoPublico/Paso3_Responsables`

### 📝 **Campos Disponibles** (Todos opcionales)
| Campo | Tipo | Validación |
|-------|------|------------|
| **Nombre Completo del Padre** | Texto (100 chars) | Opcional |
| **Nombre Completo de la Madre** | Texto (100 chars) | Opcional |
| **Nombre Completo del Tutor Legal** | Texto (100 chars) | Opcional |
| **Teléfono del Padre** | Teléfono (20 chars) | Opcional |
| **Teléfono de la Madre** | Teléfono (20 chars) | Opcional |
| **Teléfono del Tutor** | Teléfono (20 chars) | Opcional |
| **Contacto de Emergencia** | Texto (100 chars) | Opcional |
| **Teléfono de Emergencia** | Teléfono (20 chars) | Opcional |
| **Parentesco Emergencia** | Texto (50 chars) | Opcional |

### ⚠️ **Nota Importante**
Aunque todos los campos son opcionales, se recomienda completar al menos un contacto de emergencia.

### ➡️ **Flujo**
1. Usuario completa información de responsables
2. Sistema guarda datos (sin validaciones obligatorias)
3. Redirige al **Paso 4**

---

## 🎓 **PASO 4: Información Académica**
**URL**: `/EmpadronamientoPublico/Paso4_Academico`

### 📝 **Campos Obligatorios**
| Campo | Tipo | Validación |
|-------|------|------------|
| **Nivel Académico Actual** | Selección | ✅ Requerido |
| **Año que desea cursar** | Número | ✅ Requerido |

### 📝 **Campos Opcionales**
| Campo | Tipo | Validación |
|-------|------|------------|
| **Institución de Procedencia** | Texto (200 chars) | Opcional |
| **Año de Graduación** | Número | Opcional |
| **Promedio Anterior** | Decimal | Opcional |
| **Observaciones Académicas** | Textarea (500 chars) | Opcional |

### 🎯 **Opciones de Nivel Académico**
- Primaria
- Secundaria  
- Bachillerato
- Universidad
- Técnico
- Otro

### ➡️ **Flujo**
1. Usuario selecciona nivel académico y año
2. Completa información adicional (opcional)
3. Sistema valida campos obligatorios
4. Si es válido → Redirige al **Paso 5**

---

## 🏥 **PASO 5: Información de Salud**
**URL**: `/EmpadronamientoPublico/Paso5_Salud`

### 📝 **Campos Disponibles** (Todos opcionales)
| Campo | Tipo | Validación |
|-------|------|------------|
| **Tipo de Sangre** | Selección | Opcional |
| **Alergias** | Textarea (500 chars) | Opcional |
| **Medicamentos que toma** | Textarea (500 chars) | Opcional |
| **Condiciones médicas** | Textarea (500 chars) | Opcional |
| **Contacto médico de emergencia** | Texto (100 chars) | Opcional |
| **Teléfono médico emergencia** | Teléfono (20 chars) | Opcional |

### 🩸 **Tipos de Sangre Disponibles**
- A+, A-, B+, B-, AB+, AB-, O+, O-
- No sabe

### ⚠️ **Nota de Privacidad**
Esta información es confidencial y solo será utilizada en casos de emergencia médica.

### ➡️ **Flujo**
1. Usuario completa información médica (todo opcional)
2. Sistema guarda datos sin validaciones obligatorias
3. Redirige a **Confirmación**

---

## ✅ **CONFIRMACIÓN Y FINALIZACIÓN**
**URL**: `/EmpadronamientoPublico/Confirmacion`

### 📋 **Resumen Completo**
La página de confirmación muestra:

1. **Todos los datos ingresados** organizados por sección
2. **Código de empadronamiento único** generado automáticamente
3. **Fecha y hora** del registro
4. **Estado**: "Pendiente de Aprobación"
5. **Instrucciones** para el siguiente paso

### 🔢 **Generación de Código**
```
Formato: EMP-YYYY-NNNNNN
Ejemplo: EMP-2025-000123
```

### 📧 **Notificación por Correo** (Futuro)
- Se enviará automáticamente al correo registrado
- Contendrá el código de empadronamiento
- Incluirá instrucciones de seguimiento

---

## 🛡️ **Sistema de Seguridad y Validaciones**

### 🚫 **Prevención de Fraude**
1. **Límite por IP**: Máximo 5 intentos por hora
2. **Validación de cédula única**: No permite duplicados
3. **Sesión temporal**: Los datos se pierden si se abandona el proceso
4. **Tokens antifalsificación**: En todos los formularios POST

### 🔒 **Protección de Datos**
1. **Persistencia temporal**: Datos guardados en TempData durante el proceso
2. **Sesión única**: Cada proceso tiene un SessionId único
3. **Limpieza automática**: Los datos temporales se eliminan al completar o abandonar

### ✅ **Validaciones por Paso**
- **HTML5**: Validación en el navegador
- **Servidor**: Validación completa en backend
- **Base de datos**: Verificación de duplicados e integridad

---

## 🎮 **Flujo de Navegación**

### ➡️ **Navegación Normal**
```
Inicio → Paso 1 → Paso 2 → Paso 3 → Paso 4 → Paso 5 → Confirmación
```

### ⬅️ **Navegación de Regreso**
- Los usuarios **NO pueden** regresar a pasos anteriores
- El proceso es **secuencial** y debe completarse en orden
- Si abandonan, deben **reiniciar** desde el Paso 1

### 🔄 **Reinicio de Proceso**
- Automático si la sesión expira
- Manual si hay problemas técnicos
- Se pierde todo el progreso anterior

---

## 📊 **Estados del Empadronamiento**

### 📝 **Durante el Proceso**
- **En progreso**: Completando pasos del 1 al 5
- **Temporal**: Datos guardados en TempData

### ✅ **Después de Completar**
- **Pendiente**: Registro enviado, esperando aprobación administrativa
- **Aprobado**: Estudiante puede acceder al sistema (futuro)
- **Rechazado**: Requiere correcciones (futuro)

---

## 🛠️ **Para Administradores**

### 🎛️ **Control del Sistema**
**Panel**: `Admin/Configuracion`

**Opciones disponibles**:
- ✅ **Habilitar/Deshabilitar** empadronamiento público
- 📊 **Ver estadísticas** de registros pendientes
- 👥 **Gestionar** solicitudes de empadronamiento
- ⚙️ **Configurar** mensajes y límites

### 📈 **Reportes y Estadísticas**
- Cantidad de registros por día/mes
- Registros pendientes de aprobación
- Tasas de abandono por paso
- Intentos bloqueados por IP

### 🔧 **Configuraciones Técnicas**
**Archivo**: `appsettings.json`
```json
{
  "EmpadronamientoPublico": {
    "Habilitado": true,
    "MaxIntentosPorIP": 5,
    "TiempoLimiteHoras": 1,
    "MensajeDeshabilitado": "El sistema está temporalmente deshabilitado"
  }
}
```

---

## 🎯 **Casos de Uso Comunes**

### ✅ **Registro Exitoso**
1. Estudiante accede al sistema
2. Completa los 5 pasos secuencialmente
3. Recibe código de empadronamiento
4. Espera aprobación administrativa

### ❌ **Cédula Duplicada**
1. Estudiante ingresa cédula en Paso 1
2. Sistema detecta que ya existe
3. Muestra mensaje de error
4. No permite continuar

### ⏰ **Sesión Expirada**
1. Estudiante abandona el proceso por mucho tiempo
2. TempData expira automáticamente
3. Al regresar, debe reiniciar desde Paso 1

### 🚫 **IP Bloqueada**
1. Demasiados intentos desde la misma IP
2. Sistema bloquea por 1 hora
3. Muestra mensaje de límite excedido

---

## 🔧 **Mantenimiento y Soporte**

### 📝 **Logs del Sistema**
Ubicación: `Controllers/EmpadronamientoPublicoController.cs`
- Registros exitosos
- Intentos fallidos
- Bloqueos por IP
- Errores técnicos

### 🆘 **Resolución de Problemas Comunes**

| Problema | Causa | Solución |
|----------|-------|----------|
| "Sistema deshabilitado" | Admin desactivó el sistema | Contactar administrador |
| "Cédula ya registrada" | Duplicado en base de datos | Verificar registros existentes |
| "Límite de intentos" | Muchos intentos desde misma IP | Esperar 1 hora o cambiar IP |
| "Sesión perdida" | TempData expiró | Reiniciar proceso |

### 📞 **Contacto de Soporte**
- **Email**: soporte@institución.edu
- **Teléfono**: +506 XXXX-XXXX
- **Horario**: Lunes a Viernes, 8:00 AM - 5:00 PM

---

## 📋 **Checklist para Estudiantes**

### 📄 **Antes de Comenzar**
- [ ] Tener cédula de identidad a mano
- [ ] Conocer información de contacto actual
- [ ] Preparar datos de responsables/emergencia
- [ ] Tener información académica anterior
- [ ] Reservar 15 minutos sin interrupciones

### ✅ **Durante el Proceso**
- [ ] Completar todos los campos obligatorios (marcados con *)
- [ ] Verificar que el correo electrónico sea correcto
- [ ] Revisar datos antes de continuar a cada paso
- [ ] No cerrar la ventana del navegador
- [ ] Completar el proceso en una sola sesión

### 📋 **Después de Completar**
- [ ] Guardar el código de empadronamiento
- [ ] Verificar que llegó email de confirmación
- [ ] Estar pendiente de comunicaciones oficiales
- [ ] Contactar soporte si hay dudas

---

## 📞 **Preguntas Frecuentes (FAQ)**

### ❓ **¿Puedo guardar el progreso y continuar después?**
**R**: No, el proceso debe completarse en una sola sesión. Si abandona, deberá reiniciar desde el Paso 1.

### ❓ **¿Qué pasa si ingreso mal un dato?**
**R**: No puede regresar a pasos anteriores. Si detecta un error, deberá contactar soporte después de completar el proceso.

### ❓ **¿Cuánto tiempo dura el proceso de aprobación?**
**R**: Generalmente de 1 a 3 días hábiles, dependiendo de la carga de trabajo administrativa.

### ❓ **¿Puedo registrarme si ya tengo cuenta en el sistema?**
**R**: No, el empadronamiento público es solo para nuevos estudiantes. Si ya tiene cuenta, use el login normal.

### ❓ **¿Es obligatorio completar la información de salud?**
**R**: No, toda la información del Paso 5 es opcional, pero es recomendable para casos de emergencia.

### ❓ **¿Qué pasa si mi cédula ya está registrada?**
**R**: El sistema no permitirá continuar. Contacte soporte para verificar su situación.

---

**📧 ¿Necesita ayuda adicional?**  
Contacte al equipo de soporte técnico con su código de empadronamiento para recibir asistencia personalizada.

---
**Documento generado automáticamente**  
**Última actualización**: 16 de octubre de 2025