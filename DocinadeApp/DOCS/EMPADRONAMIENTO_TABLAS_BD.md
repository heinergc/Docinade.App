# 📊 Tablas de Base de Datos - Empadronamiento Público

## Resumen Ejecutivo

El sistema de **Empadronamiento Público** utiliza **2 tablas principales** en SQL Server para almacenar toda la información del proceso de registro público de estudiantes.

---

## 🗄️ Tablas Utilizadas

### 1. **`Estudiantes`** (Tabla Principal)

**Propósito**: Almacena la información básica del estudiante que se requiere en todo el sistema.

**Ubicación del Modelo**: `Models/Estudiante.cs`

#### Campos Guardados desde el Empadronamiento Público:

| Campo | Tipo | Descripción | Origen |
|-------|------|-------------|--------|
| `IdEstudiante` | int (PK, Identity) | ID único auto-generado | Sistema |
| `NumeroId` | string(20) | Número de cédula o ID | Paso 1 |
| `Nombre` | string(100) | Nombre del estudiante | Paso 1 |
| `Apellidos` | string(100) | Apellidos del estudiante | Paso 1 |
| `DireccionCorreo` | string(100) | Correo electrónico principal | Paso 2 |
| `Institucion` | string(100) | Institución actual (se guarda "Pendiente" desde empadronamiento público) | Sistema |
| `Grupos` | string(50) | Grupos asignados (se guarda "Pendiente de asignación") | Sistema |
| `Año` | int | Año del registro | ViewModel |
| `PeriodoAcademicoId` | int (FK) | Referencia al período académico activo | Sistema |

#### Código de Creación (Controlador líneas 383-395):
```csharp
var estudiante = new Estudiante
{
    NumeroId = datosCompletos.NumeroId,
    Nombre = datosCompletos.Nombre,
    Apellidos = datosCompletos.Apellidos,
    DireccionCorreo = datosCompletos.DireccionCorreo,
    Institucion = datosCompletos.Institucion ?? "Pendiente",
    Grupos = "Pendiente de asignación",
    Año = datosCompletos.Año,
    PeriodoAcademicoId = datosCompletos.PeriodoAcademicoId ?? await ObtenerPeriodoActualId()
};

_context.Estudiantes.Add(estudiante);
await _context.SaveChangesAsync();
```

---

### 2. **`EstudiantesEmpadronamiento`** (Tabla de Detalles Extendidos)

**Propósito**: Almacena **todos los datos adicionales** del proceso de empadronamiento público que no están en la tabla principal de Estudiantes.

**Ubicación del Modelo**: `Models/EstudianteEmpadronamiento.cs`

**Relación**: Uno a Uno con `Estudiantes` (FK: `IdEstudiante`)

#### Campos por Categoría:

#### 📋 **Datos Personales Complementarios** (Paso 1)
| Campo | Tipo | Descripción |
|-------|------|-------------|
| `NumeroId` | string(20) | Número de ID (duplicado para referencia) |
| `FechaNacimiento` | DateTime? | Fecha de nacimiento |
| `Genero` | string(20) | Género (Masculino, Femenino, Otro) |
| `Nacionalidad` | string(50) | Nacionalidad |
| `EstadoCivil` | string(30) | Estado civil |

#### 📧 **Contacto y Residencia** (Paso 2)
| Campo | Tipo | Descripción |
|-------|------|-------------|
| `Provincia` | string(50) | Provincia de residencia (CR) |
| `Canton` | string(50) | Cantón |
| `Distrito` | string(50) | Distrito |
| `Barrio` | string(100) | Barrio o comunidad |
| `Senas` | string(500) | Señas exactas de la dirección |
| `TelefonoAlterno` | string(20) | Teléfono adicional |
| `CorreoAlterno` | string(100) | Correo electrónico secundario |

#### 👨‍👩‍👧‍👦 **Responsables y Contacto de Emergencia** (Paso 3)
| Campo | Tipo | Descripción |
|-------|------|-------------|
| `NombrePadre` | string(100) | Nombre completo del padre |
| `NombreMadre` | string(100) | Nombre completo de la madre |
| `NombreTutor` | string(100) | Nombre del tutor legal (opcional) |
| `ContactoEmergencia` | string(100) | Nombre del contacto de emergencia |
| `TelefonoEmergencia` | string(20) | Teléfono de emergencia |
| `RelacionEmergencia` | string(50) | Relación con el estudiante |

#### 🎓 **Historial Académico** (Paso 4)
| Campo | Tipo | Descripción |
|-------|------|-------------|
| `InstitucionProcedencia` | string(100) | Institución educativa anterior |
| `UltimoNivelCursado` | string(50) | Último nivel educativo completado |
| `PromedioAnterior` | decimal? | Promedio de calificaciones anterior |
| `AdaptacionesPrevias` | string(500) | Adaptaciones curriculares especiales |

#### 🏥 **Información de Salud** (Paso 5)
| Campo | Tipo | Descripción |
|-------|------|-------------|
| `Alergias` | string(500) | Alergias conocidas |
| `CondicionesMedicas` | string(500) | Condiciones médicas relevantes |
| `Medicamentos` | string(500) | Medicamentos que toma regularmente |
| `SeguroMedico` | string(100) | Compañía de seguro médico |
| `CentroMedicoHabitual` | string(100) | Centro médico de preferencia |

#### 📁 **Documentación** (Uso Administrativo)
| Campo | Tipo | Descripción | Uso |
|-------|------|-------------|-----|
| `DocumentosRecibidosJson` | string | JSON con lista de documentos entregados | Admin |
| `DocumentosPendientesJson` | string | JSON con lista de documentos faltantes | Admin |
| `FechaEntregaDocumentos` | DateTime? | Cuándo entregó documentos completos | Admin |
| `FechaVencimientoPoliza` | DateTime? | Vencimiento de póliza de seguro | Admin |

#### 🔄 **Estado del Proceso**
| Campo | Tipo | Descripción | Valor Inicial |
|-------|------|-------------|---------------|
| `EtapaActual` | string(50) | Etapa del empadronamiento | **"PreRegistro"** |
| `FechaEtapa` | DateTime? | Fecha de última actualización de etapa | `DateTime.Now` |
| `UsuarioEtapa` | string(100) | Usuario que actualizó la etapa | "Empadronamiento Público" |
| `NotasInternas` | string | Notas del administrador sobre el caso | "Registro completado vía formulario público el {fecha}. Requiere aprobación." |

#### 🔍 **Auditoría**
| Campo | Tipo | Descripción | Valor |
|-------|------|-------------|-------|
| `FechaCreacion` | DateTime | Cuándo se creó el registro | `DateTime.Now` |
| `FechaModificacion` | DateTime? | Última modificación | null |
| `UsuarioCreacion` | string(100) | Quién creó el registro | "Sistema Público" |
| `UsuarioModificacion` | string(100) | Quién modificó | null |

#### Código de Creación (Controlador líneas 397-436):
```csharp
var empadronamiento = new EstudianteEmpadronamiento
{
    IdEstudiante = estudiante.IdEstudiante, // FK
    NumeroId = datosCompletos.NumeroId,
    FechaNacimiento = datosCompletos.FechaNacimiento,
    Genero = datosCompletos.Genero,
    Nacionalidad = datosCompletos.Nacionalidad,
    EstadoCivil = datosCompletos.EstadoCivil,
    Provincia = datosCompletos.Provincia,
    Canton = datosCompletos.Canton,
    Distrito = datosCompletos.Distrito,
    Barrio = datosCompletos.Barrio,
    Senas = datosCompletos.Senas,
    TelefonoAlterno = datosCompletos.TelefonoAlterno,
    CorreoAlterno = datosCompletos.CorreoAlterno,
    NombrePadre = datosCompletos.NombrePadre,
    NombreMadre = datosCompletos.NombreMadre,
    NombreTutor = datosCompletos.NombreTutor,
    ContactoEmergencia = datosCompletos.ContactoEmergencia,
    TelefonoEmergencia = datosCompletos.TelefonoEmergencia,
    RelacionEmergencia = datosCompletos.RelacionEmergencia,
    Alergias = datosCompletos.Alergias,
    CondicionesMedicas = datosCompletos.CondicionesMedicas,
    Medicamentos = datosCompletos.Medicamentos,
    SeguroMedico = datosCompletos.SeguroMedico,
    CentroMedicoHabitual = datosCompletos.CentroMedicoHabitual,
    InstitucionProcedencia = datosCompletos.InstitucionProcedencia,
    UltimoNivelCursado = datosCompletos.UltimoNivelCursado,
    PromedioAnterior = datosCompletos.PromedioAnterior,
    AdaptacionesPrevias = datosCompletos.AdaptacionesPrevias,
    EtapaActual = "PreRegistro",
    FechaEtapa = DateTime.Now,
    UsuarioEtapa = "Empadronamiento Público",
    NotasInternas = $"Registro completado vía formulario público el {DateTime.Now:dd/MM/yyyy HH:mm}. Requiere aprobación.",
    FechaCreacion = DateTime.Now,
    UsuarioCreacion = "Sistema Público"
};

_context.EstudiantesEmpadronamiento.Add(empadronamiento);
await _context.SaveChangesAsync();
```

---

## 🔗 Relación entre Tablas

```
┌─────────────────────────┐
│      Estudiantes        │
│  (Tabla Principal)      │
├─────────────────────────┤
│ • IdEstudiante (PK) ←───┼───┐
│ • NumeroId              │   │ Relación 1:1
│ • Nombre                │   │
│ • Apellidos             │   │
│ • DireccionCorreo       │   │
│ • Institucion           │   │
│ • Año                   │   │
│ • PeriodoAcademicoId    │   │
└─────────────────────────┘   │
                              │
                              │ (FK)
                              │
┌─────────────────────────────▼──┐
│  EstudiantesEmpadronamiento    │
│  (Detalles Extendidos)         │
├────────────────────────────────┤
│ • IdEstudiante (PK, FK)        │
│ • [50+ campos adicionales]     │
│ • EtapaActual: "PreRegistro"   │
│ • FechaCreacion                │
│ • UsuarioCreacion: "Sistema    │
│   Público"                     │
└────────────────────────────────┘
```

---

## 📝 Flujo de Datos en el Proceso

### 1. **Usuario Completa el Formulario** (5 pasos)
- **Paso 1**: Datos básicos → Guardado en TempData
- **Paso 2**: Contacto/Residencia → Guardado en TempData
- **Paso 3**: Responsables → Guardado en TempData
- **Paso 4**: Académico → Guardado en TempData
- **Paso 5**: Salud + Términos → Guardado en TempData

### 2. **Al Finalizar** (método `Finalizar()`)
```
1. Recuperar todos los datos de TempData
2. Validar aceptación de términos
3. Crear registro en tabla `Estudiantes`
4. SaveChanges() → Obtener IdEstudiante auto-generado
5. Crear registro en `EstudiantesEmpadronamiento` con IdEstudiante
6. SaveChanges() → Confirmar empadronamiento
7. Generar código de confirmación
8. Limpiar TempData
9. Redirigir a página de confirmación
```

### 3. **Código de Confirmación Generado**
```csharp
var codigoConfirmacion = $"EMP-{estudiante.IdEstudiante:D6}-{DateTime.Now:yyyyMMdd}";
// Ejemplo: EMP-000123-20251015
```

---

## 🔍 Consultas SQL Útiles

### Ver todos los empadronamientos públicos pendientes:
```sql
SELECT 
    e.IdEstudiante,
    e.NumeroId,
    e.Nombre,
    e.Apellidos,
    e.DireccionCorreo,
    ee.EtapaActual,
    ee.FechaCreacion,
    ee.NotasInternas
FROM 
    Estudiantes e
INNER JOIN 
    EstudiantesEmpadronamiento ee ON e.IdEstudiante = ee.IdEstudiante
WHERE 
    ee.EtapaActual = 'PreRegistro'
    AND ee.UsuarioCreacion = 'Sistema Público'
ORDER BY 
    ee.FechaCreacion DESC;
```

### Ver información completa de un empadronamiento específico:
```sql
SELECT 
    e.*,
    ee.*
FROM 
    Estudiantes e
INNER JOIN 
    EstudiantesEmpadronamiento ee ON e.IdEstudiante = ee.IdEstudiante
WHERE 
    e.NumeroId = '1-1234-5678';  -- Número de cédula
```

### Contar empadronamientos por fecha:
```sql
SELECT 
    CAST(ee.FechaCreacion AS DATE) as Fecha,
    COUNT(*) as TotalEmpadronamientos
FROM 
    EstudiantesEmpadronamiento ee
WHERE 
    ee.UsuarioCreacion = 'Sistema Público'
GROUP BY 
    CAST(ee.FechaCreacion AS DATE)
ORDER BY 
    Fecha DESC;
```

---

## 🛡️ Validaciones Anti-Fraude Implementadas

Antes de guardar en base de datos, el controlador verifica:

1. **Cédula Duplicada** (línea 111-115):
```csharp
var cedulaExiste = await _context.Estudiantes
    .AnyAsync(e => e.NumeroId == viewModel.NumeroId);
```

2. **Correo Duplicado** (línea 188-191):
```csharp
var correoExiste = await _context.Estudiantes
    .AnyAsync(e => e.DireccionCorreo == viewModel.DireccionCorreo);
```

3. **Límite de Intentos por IP** (Session):
```csharp
private const int MAX_INTENTOS = 5;
private const string IP_ATTEMPTS_KEY = "EmpadronamientoIntentos_";
```

---

## 📊 Estadísticas del Modelo de Datos

| Característica | Valor |
|----------------|-------|
| **Tablas Principales** | 2 |
| **Campos Totales** | ~60 campos |
| **Campos Obligatorios** | 8 (básicos en Estudiantes) |
| **Campos Opcionales** | ~50 (en EstudiantesEmpadronamiento) |
| **Relación** | 1:1 (Estudiantes → EstudiantesEmpadronamiento) |
| **Índices** | PK en ambas tablas, FK en EstudiantesEmpadronamiento |

---

## 🎯 Etapas del Empadronamiento

| Etapa | Descripción | Quién la Actualiza |
|-------|-------------|-------------------|
| **PreRegistro** | Registro inicial desde formulario público | Sistema Público |
| **Revisión** | En proceso de revisión por administrador | Admin |
| **Aprobado** | Estudiante aprobado para matrícula | Admin |
| **Documentos Pendientes** | Faltan documentos | Admin |
| **Completo** | Empadronamiento completo | Admin |
| **Rechazado** | No cumple requisitos | Admin |

---

## 📂 Archivos Relacionados

| Archivo | Propósito |
|---------|-----------|
| `Models/Estudiante.cs` | Definición de tabla Estudiantes |
| `Models/EstudianteEmpadronamiento.cs` | Definición de tabla EstudiantesEmpadronamiento |
| `Controllers/EmpadronamientoPublicoController.cs` | Lógica de guardado (líneas 340-450) |
| `ViewModels/EmpadronamientoPublicoViewModel.cs` | ViewModel unificado para los 5 pasos |
| `Data/ApplicationDbContext.cs` | Configuración de DbContext |

---

## 🔧 Mantenimiento

### Para cambiar el estado de un empadronamiento:
```csharp
var empadronamiento = await _context.EstudiantesEmpadronamiento
    .FindAsync(idEstudiante);
    
empadronamiento.EtapaActual = "Aprobado";
empadronamiento.FechaEtapa = DateTime.Now;
empadronamiento.UsuarioEtapa = User.Identity.Name;
empadronamiento.FechaModificacion = DateTime.Now;
empadronamiento.UsuarioModificacion = User.Identity.Name;

await _context.SaveChangesAsync();
```

---

## ✅ Resumen

**El sistema de Empadronamiento Público utiliza:**
- ✅ **2 tablas principales** en SQL Server
- ✅ **~60 campos** para capturar información completa del estudiante
- ✅ **Relación 1:1** entre Estudiantes y EstudiantesEmpadronamiento
- ✅ **Estado inicial**: "PreRegistro" para identificar registros públicos
- ✅ **Auditoría completa** con fechas y usuarios
- ✅ **Validaciones** para evitar duplicados y fraude

---

**Documento generado**: 15 de octubre de 2025  
**Sistema**: RubricasApp.Web - Empadronamiento Público  
**Base de Datos**: SQL Server (SCPDTIC16584\SQLEXPRESS)
