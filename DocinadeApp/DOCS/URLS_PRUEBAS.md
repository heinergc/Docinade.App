# URLs para Pruebas del Sistema RubricasApp

## GESTION ACADEMICA
- http://localhost:18163/Estudiantes
- http://localhost:18163/Estudiantes/Create
- http://localhost:18163/Empadronamiento
- http://localhost:18163/GruposEstudiantes
- http://localhost:18163/GruposEstudiantes/Create
- http://localhost:18163/Materias
- http://localhost:18163/Materias/Create
- http://localhost:18163/PeriodosAcademicos

## RUBRICAS Y EVALUACION
- http://localhost:18163/Rubricas
- http://localhost:18163/Rubricas/Create
- http://localhost:18163/ItemsEvaluacion
- http://localhost:18163/NivelesCalificacion
- http://localhost:18163/ValorRubrica
- http://localhost:18163/InstrumentosEvaluacion
- http://localhost:18163/InstrumentoRubrica
- http://localhost:18163/InstrumentoMaterias
- http://localhost:18163/ImportarRubrica

## EVALUACIONES
- http://localhost:18163/Evaluaciones
- http://localhost:18163/Evaluaciones/Create
- http://localhost:18163/Evaluaciones/Reportes

## CALIFICACIONES
- http://localhost:18163/CuadernoCalificador
- http://localhost:18163/CalificadorPQ2025
- http://localhost:18163/GruposCalificacion

## ASISTENCIA
- http://localhost:18163/Horarios
- http://localhost:18163/Asistencia
- http://localhost:18163/AsistenciaLeccion
- http://localhost:18163/AsistenciaLeccion/Estadisticas
- http://localhost:18163/AsistenciaLeccion/Ausencias
- http://localhost:18163/Asistencias

## CONDUCTA
- http://localhost:18163/NotaConducta/Dashboard
- http://localhost:18163/BoletasConducta
- http://localhost:18163/ProgramaAcciones

## PROFESORES E INSTITUCIONES
- http://localhost:18163/Profesores
- http://localhost:18163/Profesores/Create
- http://localhost:18163/Instituciones
- http://localhost:18163/Instituciones/Create
- http://localhost:18163/TiposGrupo

## REPORTES
- http://localhost:18163/Reportes
- http://localhost:18163/Reportes/ArbolEvaluaciones
- http://localhost:18163/SEA
- http://localhost:18163/Auditoria

## ADMINISTRACION
- http://localhost:18163/Admin/Admin
- http://localhost:18163/Admin/Users
- http://localhost:18163/Admin/Roles
- http://localhost:18163/Admin/Permissions
- http://localhost:18163/Slider
- http://localhost:18163/Dashboard

## PUBLICAS (Sin autenticación requerida)
- http://localhost:18163/
- http://localhost:18163/Home
- http://localhost:18163/Account/Login
- http://localhost:18163/Account/Register
- http://localhost:18163/EmpadronamientoPublico

---

## Notas para Pruebas

### Usuario de Prueba
- **Email**: admin@rubricas.edu
- **Password**: Admin@2025!

### Qué verificar:
1. **Autenticación**: Todas las URLs (excepto las PUBLICAS) deben redirigir a Login si no estás autenticado
2. **Permisos**: Algunas vistas pueden requerir permisos específicos según tu rol
3. **Funcionalidad**: Verifica botones de Create, Edit, Delete, Export
4. **Ordenamiento**: Tablas con columnas ordenables (clic en encabezados)
5. **Búsqueda/Filtros**: Campos de búsqueda en las vistas Index

### Controladores Recientemente Protegidos (Auditoría de Seguridad)
Los siguientes controladores ahora requieren autenticación:
- ✅ AsistenciasController
- ✅ AsistenciaController
- ✅ MateriasController
- ✅ ImportarRubricaController
- ✅ GruposCalificacionController
- ✅ ItemsEvaluacionController
- ✅ InstrumentosEvaluacionController
- ✅ InstrumentoRubricaController
- ✅ NivelesCalificacionController
- ✅ PeriodosAcademicosController
- ✅ ValorRubricaController
- ✅ InstitucionesController
- ✅ EvaluacionesController

### Prueba de Seguridad Rápida
1. Cerrar sesión: http://localhost:18163/Account/Logout
2. Intentar acceder a cualquier URL de GESTION ACADEMICA
3. **Resultado esperado**: Redirección automática a Login
