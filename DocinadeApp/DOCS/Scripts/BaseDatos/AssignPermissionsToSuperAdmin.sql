-- Script para asignar TODOS los permisos al rol SuperAdministrador
-- Fecha: 2025-12-05

SET NOCOUNT ON;
GO

DECLARE @RoleId NVARCHAR(450);
DECLARE @PermissionCount INT = 0;

-- Obtener el ID del rol SuperAdministrador
SELECT @RoleId = Id FROM AspNetRoles WHERE NormalizedName = 'SUPERADMINISTRADOR';

IF @RoleId IS NULL
BEGIN
    PRINT 'ERROR: Rol SuperAdministrador no encontrado';
    RETURN;
END

PRINT 'Asignando permisos al rol SuperAdministrador (ID: ' + @RoleId + ')';
PRINT '';

-- Eliminar permisos existentes del rol para evitar duplicados
DELETE FROM AspNetRoleClaims WHERE RoleId = @RoleId;
PRINT 'Permisos existentes eliminados';

-- Insertar TODOS los 168 permisos del sistema
-- Módulo: Estudiantes (12 permisos)
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'Ver.Estudiantes');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'Crear.Estudiante');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'Editar.Estudiante');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'Eliminar.Estudiante');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'Ver.DetallesEstudiante');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'Importar.Estudiantes');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'Exportar.Estudiantes');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'Buscar.Estudiantes');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'AsignarGrupo.Estudiante');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'RemoverGrupo.Estudiante');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'VerHistorial.Estudiante');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'CambiarEstado.Estudiante');
SET @PermissionCount = @PermissionCount + 12;

-- Módulo: Grupos (10 permisos)
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'Ver.Grupos');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'Crear.Grupo');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'Editar.Grupo');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'Eliminar.Grupo');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'Ver.DetallesGrupo');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'AsignarEstudiantes.Grupo');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'RemoverEstudiantes.Grupo');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'CambiarEstado.Grupo');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'AsignarProfesor.Grupo');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'VerListado.Grupo');
SET @PermissionCount = @PermissionCount + 10;

-- Módulo: Materias (8 permisos)
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'Ver.Materias');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'Crear.Materia');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'Editar.Materia');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'Eliminar.Materia');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'Ver.DetallesMateria');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'AsignarProfesor.Materia');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'CambiarEstado.Materia');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'VerPlanEstudios.Materia');
SET @PermissionCount = @PermissionCount + 8;

-- Módulo: Evaluaciones (14 permisos)
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'Ver.Evaluaciones');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'Crear.Evaluacion');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'Editar.Evaluacion');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'Eliminar.Evaluacion');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'Ver.DetallesEvaluacion');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'Calificar.Evaluacion');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'VerCalificaciones.Evaluacion');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'EditarCalificaciones.Evaluacion');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'Publicar.Evaluacion');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'Cerrar.Evaluacion');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'Clonar.Evaluacion');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'ExportarNotas.Evaluacion');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'VerEstadisticas.Evaluacion');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'GenerarReporte.Evaluacion');
SET @PermissionCount = @PermissionCount + 14;

-- Módulo: Rúbricas (12 permisos)
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'Ver.Rubricas');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'Crear.Rubrica');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'Editar.Rubrica');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'Eliminar.Rubrica');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'Ver.DetallesRubrica');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'Importar.Rubrica');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'Exportar.Rubrica');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'Clonar.Rubrica');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'Compartir.Rubrica');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'Publicar.Rubrica');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'CambiarEstado.Rubrica');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'AsignarMateria.Rubrica');
SET @PermissionCount = @PermissionCount + 12;

-- Módulo: Instrumentos (10 permisos)
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'Ver.Instrumentos');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'Crear.Instrumento');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'Editar.Instrumento');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'Eliminar.Instrumento');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'Ver.DetallesInstrumento');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'Clonar.Instrumento');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'Compartir.Instrumento');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'AsignarRubrica.Instrumento');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'CambiarEstado.Instrumento');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'VerHistorial.Instrumento');
SET @PermissionCount = @PermissionCount + 10;

-- Módulo: Calificaciones (10 permisos)
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'Ver.Calificaciones');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'Ingresar.Calificacion');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'Editar.Calificacion');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'Eliminar.Calificacion');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'Ver.BoletaNotas');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'Exportar.Calificaciones');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'Importar.Calificaciones');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'GenerarReporte.Calificaciones');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'VerEstadisticas.Calificaciones');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'CerrarPeriodo.Calificaciones');
SET @PermissionCount = @PermissionCount + 10;

-- Módulo: Reportes (8 permisos)
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'Ver.Reportes');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'Generar.ReporteAcademico');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'Generar.ReporteAsistencia');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'Generar.ReporteConducta');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'Generar.ReporteEstadistico');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'Exportar.Reporte');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'Compartir.Reporte');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'Programar.Reporte');
SET @PermissionCount = @PermissionCount + 8;

-- Módulo: Usuarios (12 permisos)
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'Ver.Usuarios');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'Crear.Usuario');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'Editar.Usuario');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'Eliminar.Usuario');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'Ver.DetallesUsuario');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'CambiarContrasena.Usuario');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'AsignarRol.Usuario');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'RemoverRol.Usuario');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'BloquearDesbloquear.Usuario');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'VerActividad.Usuario');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'ResetearContrasena.Usuario');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'CambiarEstado.Usuario');
SET @PermissionCount = @PermissionCount + 12;

-- Módulo: Roles (10 permisos)
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'Ver.Roles');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'Crear.Rol');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'Editar.Rol');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'Eliminar.Rol');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'Ver.DetallesRol');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'AsignarPermisos.Rol');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'RemoverPermisos.Rol');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'Clonar.Rol');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'VerUsuarios.Rol');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'CambiarEstado.Rol');
SET @PermissionCount = @PermissionCount + 10;

-- Módulo: Asistencia (12 permisos)
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'Ver.Asistencia');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'Registrar.Asistencia');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'Editar.Asistencia');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'Eliminar.Asistencia');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'Ver.ReporteAsistencia');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'Exportar.Asistencia');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'Justificar.Ausencia');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'VerHistorial.Asistencia');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'GenerarReporte.Asistencia');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'VerEstadisticas.Asistencia');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'EnviarNotificacion.Ausencia');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'ConfigurarHorario.Asistencia');
SET @PermissionCount = @PermissionCount + 12;

-- Módulo: Conducta (10 permisos)
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'Ver.Conducta');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'Registrar.Falta');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'Editar.Falta');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'Eliminar.Falta');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'Ver.BoletaConducta');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'Generar.BoletaConducta');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'Ver.HistorialConducta');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'Exportar.Conducta');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'GenerarReporte.Conducta');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'ConfigurarTipos.Falta');
SET @PermissionCount = @PermissionCount + 10;

-- Módulo: Períodos Académicos (8 permisos)
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'Ver.PeriodosAcademicos');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'Crear.PeriodoAcademico');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'Editar.PeriodoAcademico');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'Eliminar.PeriodoAcademico');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'Activar.PeriodoAcademico');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'Cerrar.PeriodoAcademico');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'Ver.DetallesPeriodo');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'CambiarEstado.Periodo');
SET @PermissionCount = @PermissionCount + 8;

-- Módulo: Configuración (10 permisos)
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'Ver.Configuracion');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'Editar.ConfiguracionGeneral');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'Editar.ConfiguracionInstitucion');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'Editar.ConfiguracionCalificaciones');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'Editar.ConfiguracionAsistencia');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'Editar.ConfiguracionConducta');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'Ver.RegistroAuditoria');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'Exportar.ConfiguracionSistema');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'Importar.ConfiguracionSistema');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'ResetearConfiguracion');
SET @PermissionCount = @PermissionCount + 10;

-- Módulo: Dashboard (6 permisos)
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'Ver.Dashboard');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'Ver.EstadisticasGenerales');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'Ver.GraficosAnalisis');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'Exportar.Dashboard');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'Personalizar.Dashboard');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'VerAlertas.Sistema');
SET @PermissionCount = @PermissionCount + 6;

-- Módulo: Cuaderno Calificador (12 permisos)
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'Ver.CuadernoCalificador');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'Editar.CuadernoCalificador');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'Exportar.CuadernoCalificador');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'Importar.CuadernoCalificador');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'GenerarReporte.CuadernoCalificador');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'VerEstadisticas.CuadernoCalificador');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'ConfigurarColumnas.CuadernoCalificador');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'CalcularPromedios.CuadernoCalificador');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'AplicarFormulas.CuadernoCalificador');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'BloquearDesbloquear.CuadernoCalificador');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'VerHistorial.CuadernoCalificador');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'Compartir.CuadernoCalificador');
SET @PermissionCount = @PermissionCount + 12;

-- Módulo: Profesores (8 permisos)
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'Ver.Profesores');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'Crear.Profesor');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'Editar.Profesor');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'Eliminar.Profesor');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'Ver.DetallesProfesor');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'AsignarMaterias.Profesor');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'AsignarGrupos.Profesor');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'VerCargaAcademica.Profesor');
SET @PermissionCount = @PermissionCount + 8;

-- Módulo: Empadronamiento (6 permisos)
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'Ver.Empadronamiento');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'Crear.FichaEmpadronamiento');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'Editar.FichaEmpadronamiento');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'Ver.DetallesFichaEmpadronamiento');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'Exportar.Empadronamiento');
INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue) VALUES (@RoleId, 'Permission', 'GenerarReporte.Empadronamiento');
SET @PermissionCount = @PermissionCount + 6;

PRINT '';
PRINT 'Total de permisos asignados: ' + CAST(@PermissionCount AS VARCHAR(10));
PRINT 'Proceso completado exitosamente';

-- Verificar la asignación
SELECT COUNT(*) AS TotalPermisosAsignados 
FROM AspNetRoleClaims 
WHERE RoleId = @RoleId AND ClaimType = 'Permission';

GO
