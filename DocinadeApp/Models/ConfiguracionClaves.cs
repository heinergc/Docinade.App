namespace RubricasApp.Web.Models
{
    /// <summary>
    /// Constantes para las claves de configuración del sistema
    /// </summary>
    public static class ConfiguracionClaves
    {
        /// <summary>
        /// Clave para el modo de registro de usuarios
        /// </summary>
        public const string ModoRegistro = "MODO_REGISTRO";

        /// <summary>
        /// Clave para controlar si el registro está abierto (legacy)
        /// </summary>
        public const string RegistroAbierto = "REGISTRO_ABIERTO";

        /// <summary>
        /// Clave para el nombre de la aplicación
        /// </summary>
        public const string NombreAplicacion = "NOMBRE_APLICACION";

        /// <summary>
        /// Clave para la versión de la aplicación
        /// </summary>
        public const string VersionAplicacion = "VERSION_APLICACION";

        /// <summary>
        /// Clave para el logo de la aplicación
        /// </summary>
        public const string LogoAplicacion = "LOGO_APLICACION";

        /// <summary>
        /// Clave para el favicon de la aplicación
        /// </summary>
        public const string FaviconAplicacion = "FAVICON_APLICACION";

        /// <summary>
        /// Clave para el email de contacto del administrador
        /// </summary>
        public const string EmailContacto = "EMAIL_CONTACTO";

        /// <summary>
        /// Clave para el teléfono de contacto
        /// </summary>
        public const string TelefonoContacto = "TELEFONO_CONTACTO";

        /// <summary>
        /// Clave para la dirección de la institución
        /// </summary>
        public const string DireccionInstitucion = "DIRECCION_INSTITUCION";

        /// <summary>
        /// Clave para el horario de atención
        /// </summary>
        public const string HorarioAtencion = "HORARIO_ATENCION";

        /// <summary>
        /// Clave para el límite máximo de archivos por evaluación
        /// </summary>
        public const string MaxArchivosEvaluacion = "MAX_ARCHIVOS_EVALUACION";

        /// <summary>
        /// Clave para el tamaño máximo de archivo en MB
        /// </summary>
        public const string MaxTamanoArchivo = "MAX_TAMANO_ARCHIVO_MB";

        /// <summary>
        /// Clave para los tipos de archivo permitidos
        /// </summary>
        public const string TiposArchivoPermitidos = "TIPOS_ARCHIVO_PERMITIDOS";

        /// <summary>
        /// Clave para habilitar/deshabilitar notificaciones por email
        /// </summary>
        public const string NotificacionesEmail = "NOTIFICACIONES_EMAIL";

        /// <summary>
        /// Clave para el tiempo de sesión en minutos
        /// </summary>
        public const string TiempoSesionMinutos = "TIEMPO_SESION_MINUTOS";

        /// <summary>
        /// Clave para habilitar/deshabilitar el modo mantenimiento
        /// </summary>
        public const string ModoMantenimiento = "MODO_MANTENIMIENTO";

        /// <summary>
        /// Clave para el mensaje durante el modo mantenimiento
        /// </summary>
        public const string MensajeMantenimiento = "MENSAJE_MANTENIMIENTO";

        /// <summary>
        /// Clave para el modo de registro de usuarios
        /// </summary>  
        public const string ModoRegistroUsuarios = "ModoRegistroUsuarios";
        /// <summary>
        /// Clave para el mensaje mostrado cuando el registro está cerrado
        public const string MensajeRegistroCerrado = "MensajeRegistroCerrado";
        /// <summary>
        /// Clave para permitir invitaciones especiales
        public const string PermitirInvitacionesEspeciales = "PermitirInvitacionesEspeciales";

        // Claves de configuración SMTP
        public const string EmailSmtpServer = "Email.SmtpServer";
        public const string EmailSmtpPort = "Email.SmtpPort";
        public const string EmailEnableSsl = "Email.EnableSsl";
        public const string EmailSmtpUsername = "Email.SmtpUsername";
        public const string EmailSmtpPassword = "Email.SmtpPassword";
        public const string EmailFromEmail = "Email.FromEmail";
        public const string EmailFromName = "Email.FromName";
        public const string EmailHabilitado = "Email.Habilitado";
    }
}