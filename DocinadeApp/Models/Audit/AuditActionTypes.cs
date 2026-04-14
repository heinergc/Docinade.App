namespace DocinadeApp.Models.Audit
{
    /// <summary>
    /// Tipos de acciones de auditoría organizados por módulos
    /// </summary>
    public static class AuditActionTypes
    {
        /// <summary>
        /// Acciones de autenticación y autorización
        /// </summary>
        public static class Auth
        {
            public const string Login = "Auth.Login";
            public const string Logout = "Auth.Logout";
            public const string FailedLogin = "Auth.FailedLogin";
            public const string PasswordChange = "Auth.PasswordChange";
            public const string PasswordReset = "Auth.PasswordReset";
            public const string AccountLockout = "Auth.AccountLockout";
            public const string AccountUnlock = "Auth.AccountUnlock";
            public const string TwoFactorEnabled = "Auth.TwoFactorEnabled";
            public const string TwoFactorDisabled = "Auth.TwoFactorDisabled";
            public const string SessionExpired = "Auth.SessionExpired";
            public const string AccessDenied = "Auth.AccessDenied";
            public const string UnauthorizedAccess = "Auth.UnauthorizedAccess";
        }

        /// <summary>
        /// Acciones de gestión de usuarios
        /// </summary>
        public static class Usuarios
        {
            public const string Crear = "Usuarios.Crear";
            public const string Ver = "Usuarios.Ver";
            public const string Editar = "Usuarios.Editar";
            public const string Eliminar = "Usuarios.Eliminar";
            public const string Activar = "Usuarios.Activar";
            public const string Desactivar = "Usuarios.Desactivar";
            public const string CambiarPassword = "Usuarios.CambiarPassword";
            public const string ResetearPassword = "Usuarios.ResetearPassword";
            public const string GestionarRoles = "Usuarios.GestionarRoles";
            public const string GestionarPermisos = "Usuarios.GestionarPermisos";
            public const string VerActividad = "Usuarios.VerActividad";
            public const string Importar = "Usuarios.Importar";
            public const string Exportar = "Usuarios.Exportar";
            public const string Bloquear = "Usuarios.Bloquear";
            public const string Desbloquear = "Usuarios.Desbloquear";
        }

        /// <summary>
        /// Acciones de gestión de roles
        /// </summary>
        public static class Roles
        {
            public const string Crear = "Roles.Crear";
            public const string Ver = "Roles.Ver";
            public const string Editar = "Roles.Editar";
            public const string Eliminar = "Roles.Eliminar";
            public const string GestionarPermisos = "Roles.GestionarPermisos";
            public const string AsignarUsuarios = "Roles.AsignarUsuarios";
            public const string RemoverUsuarios = "Roles.RemoverUsuarios";
            public const string SincronizarPermisos = "Roles.SincronizarPermisos";
            public const string Clonar = "Roles.Clonar";
        }

        /// <summary>
        /// Acciones de gestión de permisos
        /// </summary>
        public static class Permisos
        {
            public const string Ver = "Permisos.Ver";
            public const string Asignar = "Permisos.Asignar";
            public const string Revocar = "Permisos.Revocar";
            public const string VerMatriz = "Permisos.VerMatriz";
            public const string Sincronizar = "Permisos.Sincronizar";
            public const string Verificar = "Permisos.Verificar";
        }

        /// <summary>
        /// Acciones de rúbricas
        /// </summary>
        public static class Rubricas
        {
            public const string Crear = "Rubricas.Crear";
            public const string Ver = "Rubricas.Ver";
            public const string Editar = "Rubricas.Editar";
            public const string Eliminar = "Rubricas.Eliminar";
            public const string Duplicar = "Rubricas.Duplicar";
            public const string Importar = "Rubricas.Importar";
            public const string Exportar = "Rubricas.Exportar";
            public const string Aprobar = "Rubricas.Aprobar";
            public const string Publicar = "Rubricas.Publicar";
            public const string Archivar = "Rubricas.Archivar";
            public const string Compartir = "Rubricas.Compartir";
            public const string Validar = "Rubricas.Validar";
            public const string Versionear = "Rubricas.Versionear";
        }

        /// <summary>
        /// Acciones de evaluaciones
        /// </summary>
        public static class Evaluaciones
        {
            public const string Crear = "Evaluaciones.Crear";
            public const string Ver = "Evaluaciones.Ver";
            public const string Editar = "Evaluaciones.Editar";
            public const string Eliminar = "Evaluaciones.Eliminar";
            public const string Evaluar = "Evaluaciones.Evaluar";
            public const string Iniciar = "Evaluaciones.Iniciar";
            public const string Completar = "Evaluaciones.Completar";
            public const string Enviar = "Evaluaciones.Enviar";
            public const string Revisar = "Evaluaciones.Revisar";
            public const string Aprobar = "Evaluaciones.Aprobar";
            public const string Rechazar = "Evaluaciones.Rechazar";
            public const string Reabrir = "Evaluaciones.Reabrir";
            public const string Finalizar = "Evaluaciones.Finalizar";
            public const string VerResultados = "Evaluaciones.VerResultados";
            public const string Exportar = "Evaluaciones.Exportar";
            public const string CalificarMasivo = "Evaluaciones.CalificarMasivo";
        }

        /// <summary>
        /// Acciones de estudiantes
        /// </summary>
        public static class Estudiantes
        {
            public const string Crear = "Estudiantes.Crear";
            public const string Ver = "Estudiantes.Ver";
            public const string Editar = "Estudiantes.Editar";
            public const string Eliminar = "Estudiantes.Eliminar";
            public const string Importar = "Estudiantes.Importar";
            public const string Exportar = "Estudiantes.Exportar";
            public const string VerHistorial = "Estudiantes.VerHistorial";
            public const string VerNotas = "Estudiantes.VerNotas";
            public const string EditarNotas = "Estudiantes.EditarNotas";
            public const string AsignarGrupo = "Estudiantes.AsignarGrupo";
            public const string CambiarEstado = "Estudiantes.CambiarEstado";
        }

        /// <summary>
        /// Acciones de períodos académicos
        /// </summary>
        public static class Periodos
        {
            public const string Crear = "Periodos.Crear";
            public const string Ver = "Periodos.Ver";
            public const string Editar = "Periodos.Editar";
            public const string Eliminar = "Periodos.Eliminar";
            public const string Activar = "Periodos.Activar";
            public const string Cerrar = "Periodos.Cerrar";
            public const string Archivar = "Periodos.Archivar";
            public const string Restaurar = "Periodos.Restaurar";
        }

        /// <summary>
        /// Acciones de niveles de calificación
        /// </summary>
        public static class Niveles
        {
            public const string Crear = "Niveles.Crear";
            public const string Ver = "Niveles.Ver";
            public const string Editar = "Niveles.Editar";
            public const string Eliminar = "Niveles.Eliminar";
            public const string Reordenar = "Niveles.Reordenar";
            public const string Activar = "Niveles.Activar";
            public const string Desactivar = "Niveles.Desactivar";
        }

        /// <summary>
        /// Acciones de ítems de evaluación
        /// </summary>
        public static class Items
        {
            public const string Crear = "Items.Crear";
            public const string Ver = "Items.Ver";
            public const string Editar = "Items.Editar";
            public const string Eliminar = "Items.Eliminar";
            public const string Reordenar = "Items.Reordenar";
            public const string Duplicar = "Items.Duplicar";
            public const string Activar = "Items.Activar";
            public const string Desactivar = "Items.Desactivar";
        }

        /// <summary>
        /// Acciones de configuración del sistema
        /// </summary>
        public static class Configuracion
        {
            public const string Ver = "Configuracion.Ver";
            public const string EditarSistema = "Configuracion.EditarSistema";
            public const string EditarSeguridad = "Configuracion.EditarSeguridad";
            public const string EditarEmail = "Configuracion.EditarEmail";
            public const string GestionarRoles = "Configuracion.GestionarRoles";
            public const string GestionarPermisos = "Configuracion.GestionarPermisos";
            public const string Backup = "Configuracion.Backup";
            public const string Restaurar = "Configuracion.Restaurar";
            public const string ModoMantenimiento = "Configuracion.ModoMantenimiento";
            public const string Initialize = "Configuracion.Initialize";
            public const string SyncPermissions = "Configuracion.SyncPermissions";
            public const string HealthCheck = "Configuracion.HealthCheck";
            public const string LimpiarLogs = "Configuracion.LimpiarLogs";
            public const string ReiniciarSistema = "Configuracion.ReiniciarSistema";
        }

        /// <summary>
        /// Acciones de auditoría
        /// </summary>
        public static class Auditoria
        {
            public const string Ver = "Auditoria.Ver";
            public const string VerAccesos = "Auditoria.VerAccesos";
            public const string VerCambios = "Auditoria.VerCambios";
            public const string VerErrores = "Auditoria.VerErrores";
            public const string VerMetricas = "Auditoria.VerMetricas";
            public const string Exportar = "Auditoria.Exportar";
            public const string Limpiar = "Auditoria.Limpiar";
            public const string Filtrar = "Auditoria.Filtrar";
            public const string VerDetalles = "Auditoria.VerDetalles";
            public const string GenerarReporte = "Auditoria.GenerarReporte";
        }

        /// <summary>
        /// Acciones de reportes
        /// </summary>
        public static class Reportes
        {
            public const string VerBasicos = "Reportes.VerBasicos";
            public const string VerAvanzados = "Reportes.VerAvanzados";
            public const string CrearPersonalizados = "Reportes.CrearPersonalizados";
            public const string Exportar = "Reportes.Exportar";
            public const string Programar = "Reportes.Programar";
            public const string Generar = "Reportes.Generar";
            public const string Enviar = "Reportes.Enviar";
            public const string Archivar = "Reportes.Archivar";
        }

        /// <summary>
        /// Acciones administrativas
        /// </summary>
        public static class Admin
        {
            public const string ViewDashboard = "Admin.ViewDashboard";
            public const string ManageUsers = "Admin.ManageUsers";
            public const string ManageRoles = "Admin.ManageRoles";
            public const string ManagePermissions = "Admin.ManagePermissions";
            public const string ViewAudit = "Admin.ViewAudit";
            public const string SystemHealth = "Admin.SystemHealth";
            public const string ManageConfiguration = "Admin.ManageConfiguration";
            public const string ViewStatistics = "Admin.ViewStatistics";
            public const string ManageBackups = "Admin.ManageBackups";
            public const string MonitorSystem = "Admin.MonitorSystem";
        }

        /// <summary>
        /// Acciones del sistema
        /// </summary>
        public static class Sistema
        {
            public const string Iniciar = "Sistema.Iniciar";
            public const string Detener = "Sistema.Detener";
            public const string Reiniciar = "Sistema.Reiniciar";
            public const string ActualizarVersion = "Sistema.ActualizarVersion";
            public const string CrearBackup = "Sistema.CrearBackup";
            public const string RestaurarBackup = "Sistema.RestaurarBackup";
            public const string LimpiarCache = "Sistema.LimpiarCache";
            public const string OptimizarBD = "Sistema.OptimizarBD";
            public const string VerificarIntegridad = "Sistema.VerificarIntegridad";
            public const string MonitorearRendimiento = "Sistema.MonitorearRendimiento";
        }

        /// <summary>
        /// Obtiene todas las acciones como lista
        /// </summary>
        public static List<string> GetAllActions()
        {
            var actions = new List<string>();
            var type = typeof(AuditActionTypes);
            
            foreach (var nestedType in type.GetNestedTypes())
            {
                var fields = nestedType.GetFields();
                foreach (var field in fields)
                {
                    if (field.IsStatic && field.FieldType == typeof(string))
                    {
                        var value = field.GetValue(null)?.ToString();
                        if (!string.IsNullOrEmpty(value))
                        {
                            actions.Add(value);
                        }
                    }
                }
            }
            
            return actions.Distinct().OrderBy(a => a).ToList();
        }

        /// <summary>
        /// Obtiene las acciones por categoría
        /// </summary>
        public static Dictionary<string, List<string>> GetActionsByCategory()
        {
            var actionsByCategory = new Dictionary<string, List<string>>();
            var type = typeof(AuditActionTypes);
            
            foreach (var nestedType in type.GetNestedTypes())
            {
                var categoryName = nestedType.Name;
                var actions = new List<string>();
                
                var fields = nestedType.GetFields();
                foreach (var field in fields)
                {
                    if (field.IsStatic && field.FieldType == typeof(string))
                    {
                        var value = field.GetValue(null)?.ToString();
                        if (!string.IsNullOrEmpty(value))
                        {
                            actions.Add(value);
                        }
                    }
                }
                
                if (actions.Any())
                {
                    actionsByCategory[categoryName] = actions.OrderBy(a => a).ToList();
                }
            }
            
            return actionsByCategory;
        }

        /// <summary>
        /// Verifica si una acción es válida
        /// </summary>
        public static bool IsValidAction(string action)
        {
            return GetAllActions().Contains(action);
        }

        /// <summary>
        /// Obtiene el nombre amigable de una acción
        /// </summary>
        public static string GetDisplayName(string action)
        {
            if (string.IsNullOrEmpty(action)) return action;
            
            var parts = action.Split('.');
            if (parts.Length >= 2)
            {
                return $"{parts[0]}: {parts[1].Replace("Ver", "Ver ")}";
            }
            
            return action;
        }
    }
}