namespace DocinadeApp.Models.Permissions
{
    /// <summary>
    /// Define todos los roles disponibles en el sistema
    /// </summary>
    public static class ApplicationRoles
    {
        // Constantes de roles unificadas
        public const string SuperAdministrador = "SuperAdmin";
        public const string Administrador = "Admin";
        public const string Coordinador = "Coordinador";
        public const string Docente = "Profesor";
        public const string Evaluador = "Evaluador";
        public const string Observador = "Observador";

        // Array de todos los roles para compatibilidad
        public static readonly string[] AllRoles = {
            SuperAdministrador,
            Administrador,
            Coordinador,
            Docente,
            Evaluador,
            Observador
        };

        /// <summary>
        /// Obtiene todos los roles del sistema
        /// </summary>
        public static List<string> GetAllRoles()
        {
            return new List<string>
            {
                SuperAdministrador,
                Administrador,
                Coordinador,
                Docente,
                Evaluador,
                Observador
            };
        }

        /// <summary>
        /// Obtiene los roles del sistema que no pueden ser eliminados
        /// </summary>
        public static List<string> GetSystemRoles()
        {
            return new List<string>
            {
                SuperAdministrador,
                Administrador,
                Coordinador,
                Docente
            };
        }

        /// <summary>
        /// Verifica si un rol es del sistema
        /// </summary>
        public static bool IsSystemRole(string roleName)
        {
            return GetSystemRoles().Contains(roleName, StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Obtiene la descripción de un rol
        /// </summary>
        public static string GetRoleDescription(string role)
        {
            return role switch
            {
                SuperAdministrador => "Acceso completo al sistema, gestión de usuarios y configuración global",
                Administrador => "Gestión de usuarios, rubricas y evaluaciones del sistema",
                Coordinador => "Gestión de rubricas y evaluaciones de su área/departamento",
                Docente => "Creación y gestión de rubricas, realización de evaluaciones",
                Evaluador => "Realización de evaluaciones con rubricas existentes",
                Observador => "Solo visualización de reportes y estadísticas",
                _ => "Rol no definido"
            };
        }

        /// <summary>
        /// Obtiene los permisos asociados a un rol (para compatibilidad)
        /// </summary>
        public static List<string> GetRolePermissions(string role)
        {
            return role switch
            {
                SuperAdministrador => new List<string>
                {
                    "users.manage", "roles.manage", "system.configure",
                    "rubricas.manage", "evaluaciones.manage", "reportes.view",
                    "estudiantes.manage", "periodos.manage"
                },
                Administrador => new List<string>
                {
                    "users.manage", "rubricas.manage", "evaluaciones.manage",
                    "reportes.view", "estudiantes.manage", "periodos.manage"
                },
                Coordinador => new List<string>
                {
                    "rubricas.manage", "evaluaciones.manage", "reportes.view",
                    "estudiantes.view", "users.view"
                },
                Docente => new List<string>
                {
                    "rubricas.create", "rubricas.edit.own", "evaluaciones.manage",
                    "reportes.view.own", "estudiantes.view"
                },
                Evaluador => new List<string>
                {
                    "evaluaciones.create", "evaluaciones.edit.own",
                    "reportes.view.own", "estudiantes.view"
                },
                Observador => new List<string>
                {
                    "reportes.view", "evaluaciones.view"
                },
                _ => new List<string>()
            };
        }
    }
}