using RubricasApp.Web.Models;
using System.Net;
using System.Net.Mail;

namespace RubricasApp.Web.Services
{
    /// <summary>
    /// Implementación del servicio de correo electrónico con SMTP para el Sistema de Rúbricas.
    /// Proporciona funcionalidades para enviar diferentes tipos de correos electrónicos utilizando
    /// un servidor SMTP configurado con autenticación y conexión SSL segura.
    /// </summary>
    /// <remarks>
    /// Este servicio soporta:
    /// - Envío de correos de texto plano y HTML
    /// - Correos de confirmación de cuenta
    /// - Correos de restablecimiento de contraseña
    /// - Correos con contraseñas temporales
    /// - Validación automática de configuraciones SMTP
    /// - Logging detallado de operaciones y errores
    /// - Manejo robusto de excepciones
    /// 
    /// Configuraciones requeridas en appsettings.json:
    /// - Email:SmtpServer: Servidor SMTP
    /// - Email:SmtpPort: Puerto del servidor SMTP
    /// - Email:From: Dirección de correo del remitente
    /// - Email:Password: Contraseña o token de aplicación
    /// - Email:DisplayName: Nombre mostrado (opcional)
    /// </remarks>
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;
        private readonly IConfiguration _configuration;
        private readonly IConfiguracionService _configuracionService;

        public EmailService(ILogger<EmailService> logger, IConfiguration configuration, IConfiguracionService configuracionService)
        {
            _logger = logger;
            _configuration = configuration;
            _configuracionService = configuracionService;
        }

        /// <summary>
        /// Obtiene un valor de configuración SMTP. Prioriza la BD sobre appsettings.
        /// </summary>
        private async Task<string?> GetSmtpValueAsync(string dbKey, string appsettingsKey)
        {
            var dbValue = await _configuracionService.ObtenerConfiguracionAsync(dbKey);
            if (!string.IsNullOrWhiteSpace(dbValue))
                return dbValue;
            return _configuration[appsettingsKey];
        }

        /// <summary>
        /// Valida que las configuraciones de email estén correctamente establecidas antes de intentar enviar correos.
        /// Verifica la presencia y validez de todas las configuraciones SMTP requeridas.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Se lanza cuando:
        /// - Alguna configuración requerida (SmtpServer, SmtpPort, From) no está establecida
        /// - El puerto SMTP no es un número válido
        /// </exception>
        /// <remarks>
        /// Este método valida:
        /// - Email:SmtpServer: Debe estar presente y no ser nulo o vacío
        /// - Email:SmtpPort: Debe estar presente y ser un número entero válido
        /// - Email:From: Debe estar presente y no ser nulo o vacío
        /// 
        /// Configuraciones opcionales que no se validan:
        /// - Email:Password: Puede ser null para servidores que no requieren autenticación
        /// - Email:DisplayName: Se usará un valor por defecto si no está presente
        /// </remarks>
        private async Task<(string smtpServer, int smtpPort, string emailFrom, string? password, string displayName, bool enableSsl)> GetEmailConfigAsync()
        {
            var smtpServer = await GetSmtpValueAsync(ConfiguracionClaves.EmailSmtpServer, "Email:SmtpServer");
            var smtpPortStr = await GetSmtpValueAsync(ConfiguracionClaves.EmailSmtpPort, "Email:SmtpPort");
            var emailFrom = await GetSmtpValueAsync(ConfiguracionClaves.EmailFromEmail, "Email:From");
            var password = await GetSmtpValueAsync(ConfiguracionClaves.EmailSmtpPassword, "Email:Password");
            var displayName = await GetSmtpValueAsync(ConfiguracionClaves.EmailFromName, "Email:DisplayName") ?? "Sistema de Rúbricas";
            var enableSslStr = await GetSmtpValueAsync(ConfiguracionClaves.EmailEnableSsl, null!);

            if (string.IsNullOrWhiteSpace(smtpServer))
                throw new InvalidOperationException("La configuración 'SmtpServer' no está establecida.");
            if (string.IsNullOrWhiteSpace(emailFrom))
                throw new InvalidOperationException("La configuración 'FromEmail' no está establecida.");
            if (!int.TryParse(smtpPortStr, out int smtpPort))
                throw new InvalidOperationException("La configuración 'SmtpPort' debe ser un número válido.");

            bool enableSsl = string.IsNullOrWhiteSpace(enableSslStr) || bool.TryParse(enableSslStr, out bool ssl) && ssl;

            return (smtpServer, smtpPort, emailFrom, password, displayName, enableSsl);
        }

        /// <summary>
        /// Envía un correo electrónico utilizando el servidor SMTP configurado.
        /// Valida las configuraciones necesarias antes de proceder con el envío.
        /// </summary>
        /// <param name="to">Dirección de correo electrónico del destinatario</param>
        /// <param name="subject">Asunto del mensaje</param>
        /// <param name="body">Contenido del mensaje (puede ser texto plano o HTML)</param>
        /// <param name="isHtml">Indica si el contenido del mensaje está en formato HTML. Por defecto es false (texto plano)</param>
        /// <returns>True si el correo se envía exitosamente, False si ocurre algún error</returns>
        /// <exception cref="InvalidOperationException">Se lanza cuando las configuraciones SMTP requeridas no están establecidas</exception>
        /// <remarks>
        /// Este método:
        /// - Valida que todas las configuraciones SMTP estén presentes
        /// - Crea una conexión SSL segura al servidor SMTP
        /// - Envía el correo utilizando las credenciales configuradas
        /// - Registra el resultado en los logs del sistema
        /// - Maneja errores y los registra apropiadamente
        /// </remarks>
        public async Task<bool> SendEmailAsync(string to, string subject, string body, bool isHtml = false)
        {
            try
            {
                var (smtpServer, smtpPort, emailFrom, password, displayName, enableSsl) = await GetEmailConfigAsync();

                using var client = new SmtpClient(smtpServer, smtpPort)
                {
                    Credentials = new NetworkCredential(emailFrom, password),
                    EnableSsl = enableSsl
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(emailFrom, displayName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = isHtml
                };

                mailMessage.To.Add(to);

                await client.SendMailAsync(mailMessage);
                _logger.LogInformation("Correo enviado exitosamente a {To} con asunto: {Subject}", to, subject);
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al enviar correo a {To} con asunto: {Subject}", to, subject);
                return false;
            }
        }

        /// <summary>
        /// Envía un correo electrónico de confirmación de cuenta con un enlace de activación.
        /// Utiliza una plantilla predefinida para el mensaje de confirmación.
        /// </summary>
        /// <param name="to">Dirección de correo electrónico del destinatario que debe confirmar su cuenta</param>
        /// <param name="confirmationLink">URL completa que el usuario debe hacer clic para confirmar su cuenta</param>
        /// <returns>True si el correo se envía exitosamente, False si ocurre algún error</returns>
        /// <remarks>
        /// Este método:
        /// - Crea un mensaje estándar de confirmación de cuenta
        /// - Incluye el enlace de confirmación en el mensaje
        /// - Envía el correo como texto plano
        /// - Es utilizado típicamente durante el proceso de registro de usuarios
        /// </remarks>
        public async Task<bool> SendConfirmationEmailAsync(string to, string confirmationLink)
        {
            var subject = "Confirma tu cuenta";
            var body = $"Por favor confirma tu cuenta haciendo clic en este enlace: {confirmationLink}";
            
            return await SendEmailAsync(to, subject, body);
        }

        /// <summary>
        /// Envía un correo electrónico para restablecer la contraseña de un usuario.
        /// Utiliza una plantilla predefinida con enlace de restablecimiento seguro.
        /// </summary>
        /// <param name="to">Dirección de correo electrónico del usuario que solicita restablecer su contraseña</param>
        /// <param name="resetLink">URL segura y temporal que permite al usuario restablecer su contraseña</param>
        /// <returns>True si el correo se envía exitosamente, False si ocurre algún error</returns>
        /// <remarks>
        /// Este método:
        /// - Crea un mensaje estándar para restablecimiento de contraseña
        /// - Incluye un enlace seguro con token temporal
        /// - Envía el correo como texto plano
        /// - Es utilizado cuando un usuario olvida su contraseña
        /// - El enlace proporcionado debe incluir validación de seguridad y caducidad
        /// </remarks>
        public async Task<bool> SendPasswordResetEmailAsync(string to, string resetLink)
        {
            var subject = "Restablecer contraseña";
            var body = $"Para restablecer tu contraseña, haz clic en este enlace: {resetLink}";
            
            return await SendEmailAsync(to, subject, body);
        }

        /// <summary>
        /// Envía un correo electrónico con una contraseña temporal para usuarios creados por administradores.
        /// Utiliza una plantilla HTML profesional con instrucciones de seguridad.
        /// </summary>
        /// <param name="to">Dirección de correo electrónico del nuevo usuario que recibirá la contraseña temporal</param>
        /// <param name="temporaryPassword">Contraseña temporal generada automáticamente que el usuario debe cambiar en su primer acceso</param>
        /// <returns>True si el correo se envía exitosamente, False si ocurre algún error</returns>
        /// <remarks>
        /// Este método:
        /// - Crea un mensaje HTML profesional de bienvenida
        /// - Incluye la contraseña temporal de forma segura
        /// - Proporciona instrucciones para el primer acceso
        /// - Recomienda cambiar la contraseña por seguridad
        /// - Incluye un enlace directo al sistema de login
        /// - Es utilizado cuando un administrador crea cuentas para otros usuarios
        /// - El mensaje está formateado en HTML para mejor presentación
        /// </remarks>
        public async Task<bool> SendTemporaryPasswordEmailAsync(string to, string temporaryPassword)
        {
            var subject = "Tu cuenta ha sido creada - Contraseña temporal";
            var body = $@"
                <h2>Bienvenido al Sistema de Rúbricas</h2>
                <p>Tu cuenta ha sido creada exitosamente por un administrador.</p>
                <p><strong>Contraseña temporal:</strong> {temporaryPassword}</p>
                <p><strong>Importante:</strong> Por razones de seguridad, te recomendamos cambiar esta contraseña temporal la primera vez que inicies sesión.</p>
                <p>Puedes acceder al sistema en: <a href='{_configuration["BaseUrl"] ?? "http://localhost"}/Account/Login'>Iniciar Sesión</a></p>
                <hr>
                <p><small>Este es un mensaje automático, por favor no respondas a este correo.</small></p>
            ";
            
            return await SendEmailAsync(to, subject, body, isHtml: true);
        }

        /// <summary>
        /// Envía una notificación por correo electrónico al profesor guía cuando se registra una nueva boleta de conducta.
        /// Incluye todos los detalles relevantes de la falta y el estudiante involucrado.
        /// </summary>
        /// <param name="to">Dirección de correo electrónico del profesor guía</param>
        /// <param name="nombreProfesor">Nombre completo del profesor guía destinatario</param>
        /// <param name="nombreEstudiante">Nombre completo del estudiante que recibió la boleta</param>
        /// <param name="numeroId">Número de identificación del estudiante</param>
        /// <param name="tipoFalta">Clasificación de la falta (Leve, Grave, Muy Grave)</param>
        /// <param name="rebajo">Puntos de rebajo aplicados a la nota de conducta</param>
        /// <param name="descripcion">Descripción detallada de la falta cometida</param>
        /// <param name="fechaEmision">Fecha y hora en que se emitió la boleta</param>
        /// <param name="docenteEmisor">Nombre del docente que emitió la boleta</param>
        /// <returns>True si el correo se envía exitosamente, False si ocurre algún error</returns>
        /// <remarks>
        /// Este método:
        /// - Crea un mensaje HTML profesional con todos los detalles de la boleta
        /// - Incluye formato visual para facilitar la lectura de la información
        /// - Proporciona un enlace directo al sistema para revisar más detalles
        /// - Utiliza colores y estilos para destacar información importante
        /// - Se ejecuta automáticamente al registrar una nueva boleta de conducta
        /// </remarks>
        public async Task<bool> SendBoletaConductaNotificationAsync(string to, string nombreProfesor, 
            string nombreEstudiante, string numeroId, string tipoFalta, decimal rebajo, string descripcion, 
            DateTime fechaEmision, string docenteEmisor)
        {
            var subject = $"Nueva Boleta de Conducta - {nombreEstudiante}";
            var body = $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <style>
                        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                        .header {{ background-color: #dc3545; color: white; padding: 20px; text-align: center; border-radius: 5px 5px 0 0; }}
                        .content {{ background-color: #f8f9fa; padding: 20px; border: 1px solid #dee2e6; }}
                        .info-box {{ background-color: white; padding: 15px; margin: 10px 0; border-left: 4px solid #dc3545; }}
                        .info-label {{ font-weight: bold; color: #495057; }}
                        .info-value {{ color: #212529; margin-left: 10px; }}
                        .warning-box {{ background-color: #fff3cd; border: 1px solid #ffc107; padding: 15px; margin: 15px 0; border-radius: 5px; }}
                        .footer {{ background-color: #343a40; color: white; padding: 15px; text-align: center; border-radius: 0 0 5px 5px; }}
                        .btn {{ display: inline-block; padding: 10px 20px; background-color: #007bff; color: white; text-decoration: none; border-radius: 5px; margin-top: 15px; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h2>🔔 Nueva Boleta de Conducta Registrada</h2>
                        </div>
                        <div class='content'>
                            <p>Estimado(a) <strong>{nombreProfesor}</strong>,</p>
                            <p>Le informamos que se ha registrado una nueva boleta de conducta para uno de los estudiantes a su cargo:</p>
                            
                            <div class='info-box'>
                                <p><span class='info-label'>👤 Estudiante:</span><span class='info-value'>{nombreEstudiante}</span></p>
                                <p><span class='info-label'>🆔 Número de ID:</span><span class='info-value'>{numeroId}</span></p>
                            </div>

                            <div class='info-box'>
                                <p><span class='info-label'>⚠️ Tipo de Falta:</span><span class='info-value' style='color: #dc3545; font-weight: bold;'>{tipoFalta}</span></p>
                                <p><span class='info-label'>📉 Rebajo Aplicado:</span><span class='info-value' style='color: #dc3545; font-weight: bold;'>{rebajo} puntos</span></p>
                            </div>

                            <div class='info-box'>
                                <p><span class='info-label'>📝 Descripción de la Falta:</span></p>
                                <p style='margin-left: 10px; font-style: italic;'>{descripcion}</p>
                            </div>

                            <div class='info-box'>
                                <p><span class='info-label'>📅 Fecha de Emisión:</span><span class='info-value'>{fechaEmision:dd/MM/yyyy HH:mm}</span></p>
                                <p><span class='info-label'>👨‍🏫 Docente Emisor:</span><span class='info-value'>{docenteEmisor}</span></p>
                            </div>

                            <div class='warning-box'>
                                <p>⚠️ <strong>Importante:</strong> Como profesor guía, le solicitamos dar seguimiento a esta situación y coordinar las acciones correspondientes según el Reglamento de Evaluación de los Aprendizajes (REA 40862-V21).</p>
                            </div>

                            <p style='text-align: center;'>
                                <a href='{_configuration["BaseUrl"] ?? "http://localhost"}/BoletasConducta' class='btn'>Ver Detalles en el Sistema</a>
                            </p>
                        </div>
                        <div class='footer'>
                            <p><small>Este es un mensaje automático del Sistema de Gestión de Rúbricas y Conducta.</small></p>
                            <p><small>Por favor, no responda a este correo.</small></p>
                        </div>
                    </div>
                </body>
                </html>
            ";
            
            return await SendEmailAsync(to, subject, body, isHtml: true);
        }
    }
}