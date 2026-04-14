namespace DocinadeApp.Services
{
    /// <summary>
    /// Interfaz para el servicio de correo electrónico
    /// </summary>
    public interface IEmailService
    {
        /// <summary>
        /// Envía un correo electrónico
        /// </summary>
        Task<bool> SendEmailAsync(string to, string subject, string body, bool isHtml = false);

        /// <summary>
        /// Envía un correo electrónico de confirmación
        /// </summary>
        Task<bool> SendConfirmationEmailAsync(string to, string confirmationLink);

        /// <summary>
        /// Envía un correo electrónico de recuperación de contraseña
        /// </summary>
        Task<bool> SendPasswordResetEmailAsync(string to, string resetLink);

        /// <summary>
        /// Envía un correo electrónico con contraseña temporal
        /// </summary>
        Task<bool> SendTemporaryPasswordEmailAsync(string to, string temporaryPassword);

        /// <summary>
        /// Envía notificación de boleta de conducta al profesor guía
        /// </summary>
        Task<bool> SendBoletaConductaNotificationAsync(string to, string nombreProfesor, string nombreEstudiante, 
            string numeroId, string tipoFalta, decimal rebajo, string descripcion, DateTime fechaEmision, 
            string docenteEmisor);
    }

}