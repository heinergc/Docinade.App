# Tablas de la Base de Datos - RubricasApp.Web

**Base de datos:** RubricaDB (SQL Server)  
**Total de tablas:** 53 (46 tablas de aplicación + 7 tablas de ASP.NET Core Identity)

---

## Tablas de Identity (ASP.NET Core) - 7 tablas

| # | Tabla | Descripción |
|---|-------|-------------|
| 1 | **AspNetUsers** | Usuarios del sistema con campos personalizados (Nombre, Apellidos, NumeroIdentificacion, Institucion, Activo) |
| 2 | **AspNetRoles** | Roles del sistema (SuperAdministrador, Administrador, Docente, Evaluador) |
| 3 | **AspNetUserRoles** | Relación muchos a muchos entre usuarios y roles |
| 4 | **AspNetUserClaims** | Claims personalizados de usuarios (permisos específicos) |
| 5 | **AspNetRoleClaims** | Claims asignados a roles (168 permisos del sistema) |
| 6 | **AspNetUserLogins** | Logins externos (Google, Microsoft, etc.) |
| 7 | **AspNetUserTokens** | Tokens de autenticación y recuperación de contraseña |

---

## Módulo de Rúbricas y Evaluaciones - 8 tablas

| # | Tabla | Descripción |
|---|-------|-------------|
| 8 | **Rubricas** | Rúbricas de evaluación con criterios |
| 9 | **ItemsEvaluacion** | Ítems/criterios de evaluación dentro de rúbricas |
| 10 | **NivelesCalificacion** | Niveles de calificación (Excelente, Muy Bueno, Bueno, Suficiente, Insuficiente) |
| 11 | **GruposCalificacion** | Grupos de calificación para organización |
| 12 | **ValoresRubrica** | Valores/calificaciones asignadas a estudiantes por ítem |
| 13 | **RubricaNiveles** | Relación entre rúbricas y niveles de calificación |
| 14 | **Evaluaciones** | Evaluaciones aplicadas a grupos de estudiantes |
| 15 | **DetallesEvaluacion** | Detalles de evaluaciones por estudiante individual |

---

## Módulo de Instrumentos de Evaluación - 3 tablas

| # | Tabla | Descripción |
|---|-------|-------------|
| 16 | **InstrumentosEvaluacion** | Instrumentos de evaluación (exámenes, proyectos, tareas, etc.) |
| 17 | **InstrumentoRubricas** | Relación instrumentos-rúbricas (un instrumento puede usar varias rúbricas) |
| 18 | **InstrumentoMaterias** | Relación instrumentos-materias (instrumentos por asignatura) |

---

## Módulo de Cuaderno Calificador - 2 tablas

| # | Tabla | Descripción |
|---|-------|-------------|
| 19 | **CuadernosCalificadores** | Cuadernos de calificación por grupo y período |
| 20 | **CuadernoInstrumentos** | Instrumentos asociados a cada cuaderno |

---

## Módulo Académico - 5 tablas

| # | Tabla | Descripción |
|---|-------|-------------|
| 21 | **Estudiantes** | Estudiantes con datos personales y académicos |
| 22 | **Materias** | Materias/asignaturas (MAT, ESP, BIO, FIS, QUIM, ESTU, ING) |
| 23 | **MateriaRequisitos** | Requisitos previos para cursar materias |
| 24 | **MateriaPeriodos** | Materias ofrecidas por período académico |
| 25 | **PeriodosAcademicos** | Períodos académicos (2025-II activo, 2026-I planificado) |

---

## Módulo de Grupos - 4 tablas

| # | Tabla | Descripción |
|---|-------|-------------|
| 26 | **GruposEstudiantes** | Grupos de estudiantes (11-A, 11-B, 10-A, 10-B) |
| 27 | **EstudianteGrupos** | Relación bidireccional estudiantes-grupos |
| 28 | **GrupoMaterias** | Materias asignadas a cada grupo |
| 29 | **TiposGrupo** | Catálogo de tipos de grupo (Regular, Avanzado, etc.) |

---

## Módulo de Asistencia MEP - 2 tablas

| # | Tabla | Descripción |
|---|-------|-------------|
| 30 | **Asistencias** | Registro de asistencia diaria por estudiante |
| 31 | **Lecciones** | Lecciones/bloques horarios según especificación MEP |

---

## Módulo de Conducta (REA 40862-V21) - 6 tablas

| # | Tabla | Descripción |
|---|-------|-------------|
| 32 | **TiposFalta** | 5 tipos de faltas según REA 40862-V21 Art. 137 |
| 33 | **BoletasConducta** | Boletas de reporte de conducta |
| 34 | **NotasConducta** | Notas de conducta con cálculo de rebajos |
| 35 | **ProgramasAccionesInstitucional** | Programas de acción correctiva institucional |
| 36 | **DecisionesProfesionalesConducta** | Decisiones profesionales sobre conducta |
| 37 | **ParametrosInstitucion** | Parámetros institucionales (nota mínima aprobación: 65) |

---

## Módulo de Profesores - 6 tablas

| # | Tabla | Descripción |
|---|-------|-------------|
| 38 | **Profesores** | Profesores del sistema educativo |
| 39 | **ProfesorFormacionAcademica** | Formación académica de profesores |
| 40 | **ProfesorExperienciaLaboral** | Experiencia laboral profesional |
| 41 | **ProfesorCapacitacion** | Capacitaciones y cursos realizados |
| 42 | **ProfesorGrupo** | Relación profesores-grupos (asignación de docentes) |
| 43 | **ProfesorGuia** | Profesores guía de grupos específicos |

---

## Módulo Geográfico Costa Rica - 6 tablas

| # | Tabla | Descripción |
|---|-------|-------------|
| 44 | **Provincias** | 7 provincias de Costa Rica |
| 45 | **Cantones** | 82 cantones distribuidos en las provincias |
| 46 | **Distritos** | 488 distritos administrativos |
| 47 | **Instituciones** | Instituciones educativas registradas |
| 48 | **Facultades** | Facultades universitarias |
| 49 | **Escuelas** | Escuelas académicas |

---

## Módulo de Empadronamiento - 1 tabla

| # | Tabla | Descripción |
|---|-------|-------------|
| 50 | **EstudiantesEmpadronamiento** | Empadronamiento público de estudiantes |

---

## Módulo de Auditoría - 2 tablas

| # | Tabla | Descripción |
|---|-------|-------------|
| 51 | **AuditLogs** | Logs de auditoría general del sistema |
| 52 | **AuditoriasOperaciones** | Auditoría de operaciones críticas |

---

## Configuración del Sistema - 1 tabla

| # | Tabla | Descripción |
|---|-------|-------------|
| 53 | **ConfiguracionesSistema** | Configuración global del sistema |

---

## Resumen por Módulo

| Módulo | Cantidad de Tablas |
|--------|-------------------|
| Identity (ASP.NET Core) | 7 |
| Rúbricas y Evaluaciones | 8 |
| Instrumentos de Evaluación | 3 |
| Cuaderno Calificador | 2 |
| Académico | 5 |
| Grupos | 4 |
| Asistencia MEP | 2 |
| Conducta (REA 40862-V21) | 6 |
| Profesores | 6 |
| Geográfico Costa Rica | 6 |
| Empadronamiento | 1 |
| Auditoría | 2 |
| Configuración | 1 |
| **TOTAL** | **53 tablas** |

---

## Notas Técnicas

### Inicialización Automática
Las tablas se crean automáticamente mediante:
1. **EF Core Migrations** (`_context.Database.MigrateAsync()`)
2. **SqlServerDatabaseInitializer** (línea 26-28 en Program.cs)
3. **Seeding automático** de datos iniciales:
   - 7 provincias de Costa Rica
   - 82 cantones
   - 488 distritos
   - 5 tipos de falta MEP
   - Admin user (admin@rubricas.edu)
   - 168 permisos del sistema

### Tablas Críticas para Producción
- **Provincias, Cantones, Distritos**: Requieren datos geográficos de Costa Rica
- **TiposFalta**: Requieren 5 tipos según REA 40862-V21
- **AspNetUsers**: Auto-crea admin@rubricas.edu en primer inicio
- **PeriodosAcademicos**: Requiere al menos un período activo

### Script de Datos Iniciales
Ejecutar manualmente (opcional): `DB/DATOS_INICIALES_PRODUCCION.sql`
- 12 materias base
- 4 grupos ejemplo (11-A, 11-B, 10-A, 10-B)
- 10 estudiantes de prueba
- 5 niveles de calificación

---

**Fecha de actualización:** 22 de noviembre de 2025  
**Versión:** .NET 8.0 con EF Core 8.0  
**Base de datos:** SQL Server (Somee.com: RubricaDB.mssql.somee.com)
