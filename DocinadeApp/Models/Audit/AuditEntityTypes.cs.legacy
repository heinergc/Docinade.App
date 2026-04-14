namespace RubricasApp.Web.Models.Audit
{
    /// <summary>
    /// Tipos de entidades para auditoría
    /// </summary>
    public static class AuditEntityTypes
    {
        // Entidades principales del dominio
        public const string RUBRICA = "Rubrica";
        public const string EVALUACION = "Evaluacion";
        public const string ESTUDIANTE = "Estudiante";
        public const string PERIODO_ACADEMICO = "PeriodoAcademico";
        public const string NIVEL_CALIFICACION = "NivelCalificacion";
        public const string ITEM_EVALUACION = "ItemEvaluacion";
        public const string DETALLE_EVALUACION = "DetalleEvaluacion";
        public const string VALOR_RUBRICA = "ValorRubrica";

        // Entidades de seguridad y usuarios
        public const string USER = "User";
        public const string ROLE = "Role";
        public const string PERMISSION = "Permission";
        public const string USER_ROLE = "UserRole";
        public const string ROLE_PERMISSION = "RolePermission";

        // Entidades de configuración
        public const string CONFIGURATION = "Configuration";
        public const string SYSTEM_SETTING = "SystemSetting";
        public const string AUDIT_LOG = "AuditLog";

        // Entidades de reportes
        public const string REPORT = "Report";
        public const string REPORT_TEMPLATE = "ReportTemplate";
        public const string SCHEDULED_REPORT = "ScheduledReport";

        // Entidades de archivos e importación
        public const string FILE_UPLOAD = "FileUpload";
        public const string IMPORT_BATCH = "ImportBatch";
        public const string EXPORT_REQUEST = "ExportRequest";

        // Entidades de sesión y actividad
        public const string SESSION = "Session";
        public const string LOGIN_ATTEMPT = "LoginAttempt";
        public const string PASSWORD_RESET = "PasswordReset";

        // Entidades del sistema
        public const string SYSTEM = "System";
        public const string APPLICATION = "Application";
        public const string DATABASE = "Database";
        public const string BACKUP = "Backup";

        /// <summary>
        /// Obtiene todos los tipos de entidad como lista
        /// </summary>
        public static List<string> GetAllEntityTypes()
        {
            return new List<string>
            {
                // Entidades principales
                RUBRICA,
                EVALUACION,
                ESTUDIANTE,
                PERIODO_ACADEMICO,
                NIVEL_CALIFICACION,
                ITEM_EVALUACION,
                DETALLE_EVALUACION,
                VALOR_RUBRICA,

                // Entidades de seguridad
                USER,
                ROLE,
                PERMISSION,
                USER_ROLE,
                ROLE_PERMISSION,

                // Entidades de configuración
                CONFIGURATION,
                SYSTEM_SETTING,
                AUDIT_LOG,

                // Entidades de reportes
                REPORT,
                REPORT_TEMPLATE,
                SCHEDULED_REPORT,

                // Entidades de archivos
                FILE_UPLOAD,
                IMPORT_BATCH,
                EXPORT_REQUEST,

                // Entidades de sesión
                SESSION,
                LOGIN_ATTEMPT,
                PASSWORD_RESET,

                // Entidades del sistema
                SYSTEM,
                APPLICATION,
                DATABASE,
                BACKUP
            };
        }

        /// <summary>
        /// Verifica si un tipo de entidad es válido
        /// </summary>
        public static bool IsValidEntityType(string entityType)
        {
            return GetAllEntityTypes().Contains(entityType);
        }

        /// <summary>
        /// Obtiene el nombre amigable de un tipo de entidad
        /// </summary>
        public static string GetDisplayName(string entityType)
        {
            return entityType switch
            {
                RUBRICA => "Rúbrica",
                EVALUACION => "Evaluación",
                ESTUDIANTE => "Estudiante",
                PERIODO_ACADEMICO => "Período Académico",
                NIVEL_CALIFICACION => "Nivel de Calificación",
                ITEM_EVALUACION => "Ítem de Evaluación",
                DETALLE_EVALUACION => "Detalle de Evaluación",
                VALOR_RUBRICA => "Valor de Rúbrica",
                USER => "Usuario",
                ROLE => "Rol",
                PERMISSION => "Permiso",
                USER_ROLE => "Rol de Usuario",
                ROLE_PERMISSION => "Permiso de Rol",
                CONFIGURATION => "Configuración",
                SYSTEM_SETTING => "Configuración del Sistema",
                AUDIT_LOG => "Log de Auditoría",
                REPORT => "Reporte",
                REPORT_TEMPLATE => "Plantilla de Reporte",
                SCHEDULED_REPORT => "Reporte Programado",
                FILE_UPLOAD => "Carga de Archivo",
                IMPORT_BATCH => "Lote de Importación",
                EXPORT_REQUEST => "Solicitud de Exportación",
                SESSION => "Sesión",
                LOGIN_ATTEMPT => "Intento de Inicio de Sesión",
                PASSWORD_RESET => "Restablecimiento de Contraseña",
                SYSTEM => "Sistema",
                APPLICATION => "Aplicación",
                DATABASE => "Base de Datos",
                BACKUP => "Copia de Seguridad",
                _ => entityType
            };
        }
    }
}