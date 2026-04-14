# Implementación de Envío de Correos - Sistema de Rúbricas

## Funcionalidades Agregadas

Se han agregado las siguientes funcionalidades al sistema de evaluaciones:

### 1. Envío Individual de Evaluaciones
- Botón de envío por cada evaluación completada
- Confirmación antes del envío
- Indicadores visuales de estado (cargando, enviado, error)
- Mensajes de retroalimentación al usuario

### 2. Envío Masivo de Evaluaciones
- Botón para enviar todas las evaluaciones que coincidan con los filtros aplicados
- Barra de progreso durante el proceso
- Resumen de resultados con detalles de errores si los hay
- Solo envía evaluaciones con estado "COMPLETADA"

### 3. Mejoras en la Interfaz
- Estadísticas de evaluaciones en la parte superior
- Columna adicional para botones de envío
- Animaciones y transiciones CSS
- Mensajes temporales con auto-ocultación
- Indicadores visuales para evaluaciones enviadas

## Archivos Modificados

### Controlador: `EvaluacionesController.cs`
- `EnviarEvaluacion(int evaluacionId)`: Envía una evaluación individual
- `EnviarTodasEvaluaciones(...)`: Envía múltiples evaluaciones
- `SimularEnvioCorreo(Evaluacion evaluacion)`: Simula el envío (para reemplazar)
- `GenerarContenidoCorreo(Evaluacion evaluacion)`: Genera HTML del correo

### Vista: `Views/Evaluaciones/Index.cshtml`
- Agregada columna "Enviar" en la tabla
- Botón "Enviar Todas las Evaluaciones"
- Estadísticas de evaluaciones
- JavaScript para manejo de envíos
- Estilos CSS mejorados

## Implementación Real del Envío de Correos

Actualmente, el sistema simula el envío de correos. Para implementar el envío real, siga estos pasos:

### Opción 1: Usando SendGrid

1. **Instalar paquete NuGet:**
```bash
dotnet add package SendGrid
```

2. **Configurar appsettings.json:**
```json
{
  "SendGrid": {
    "ApiKey": "SG.tu_api_key_aqui",
    "FromEmail": "noreply@tudominio.com",
    "FromName": "Sistema de Evaluación por Rúbricas"
  }
}
```

3. **Crear servicio de email:**
```csharp
public interface IEmailService
{
    Task<bool> EnviarCorreoAsync(string destinatario, string asunto, string contenidoHtml);
}

public class SendGridEmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    
    public SendGridEmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public async Task<bool> EnviarCorreoAsync(string destinatario, string asunto, string contenidoHtml)
    {
        try
        {
            var apiKey = _configuration["SendGrid:ApiKey"];
            var client = new SendGridClient(apiKey);
            
            var from = new EmailAddress(_configuration["SendGrid:FromEmail"], _configuration["SendGrid:FromName"]);
            var to = new EmailAddress(destinatario);
            
            var msg = MailHelper.CreateSingleEmail(from, to, asunto, null, contenidoHtml);
            var response = await client.SendEmailAsync(msg);
            
            return response.StatusCode == System.Net.HttpStatusCode.Accepted;
        }
        catch (Exception ex)
        {
            // Log del error
            Console.WriteLine($"Error enviando correo: {ex.Message}");
            return false;
        }
    }
}
```

4. **Registrar el servicio en Program.cs:**
```csharp
builder.Services.AddScoped<IEmailService, SendGridEmailService>();
```

5. **Reemplazar SimularEnvioCorreo en el controlador:**
```csharp
private readonly IEmailService _emailService;

public EvaluacionesController(RubricasDbContext context, IEmailService emailService)
{
    _context = context;
    _emailService = emailService;
}

private async Task EnviarCorreoReal(Evaluacion evaluacion)
{
    var contenidoCorreo = GenerarContenidoCorreo(evaluacion);
    var asunto = $"Resultado de Evaluación - {evaluacion.Rubrica.Titulo}";
    
    var enviado = await _emailService.EnviarCorreoAsync(
        evaluacion.Estudiante.DireccionCorreo,
        asunto,
        contenidoCorreo
    );
    
    if (!enviado)
    {
        throw new Exception("No se pudo enviar el correo electrónico");
    }
}
```

### Opción 2: Usando SMTP (Gmail, Outlook, etc.)

1. **Configurar appsettings.json:**
```json
{
  "EmailSettings": {
    "SmtpServer": "smtp.gmail.com",
    "SmtpPort": 587,
    "SenderEmail": "tu-email@gmail.com",
    "SenderPassword": "tu-app-password",
    "SenderName": "Sistema de Evaluación por Rúbricas",
    "EnableSsl": true
  }
}
```

2. **Crear servicio SMTP:**
```csharp
public class SmtpEmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    
    public SmtpEmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public async Task<bool> EnviarCorreoAsync(string destinatario, string asunto, string contenidoHtml)
    {
        try
        {
            var smtpClient = new SmtpClient(_configuration["EmailSettings:SmtpServer"])
            {
                Port = int.Parse(_configuration["EmailSettings:SmtpPort"]),
                Credentials = new NetworkCredential(
                    _configuration["EmailSettings:SenderEmail"],
                    _configuration["EmailSettings:SenderPassword"]
                ),
                EnableSsl = bool.Parse(_configuration["EmailSettings:EnableSsl"])
            };
            
            var mailMessage = new MailMessage
            {
                From = new MailAddress(
                    _configuration["EmailSettings:SenderEmail"],
                    _configuration["EmailSettings:SenderName"]
                ),
                Subject = asunto,
                Body = contenidoHtml,
                IsBodyHtml = true
            };
            
            mailMessage.To.Add(destinatario);
            
            await smtpClient.SendMailAsync(mailMessage);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error enviando correo: {ex.Message}");
            return false;
        }
    }
}
```

## Mejoras Adicionales Sugeridas

### 1. Sistema de Colas
Para manejar grandes volúmenes de correos:
```csharp
// Usar Hangfire o similar
[Queue("emails")]
public async Task EnviarCorreoEnBackground(int evaluacionId)
{
    // Lógica de envío
}
```

### 2. Plantillas de Correo
Crear plantillas más sofisticadas:
```csharp
public class PlantillaCorreo
{
    public static string GenerarPlantillaEvaluacion(Evaluacion evaluacion)
    {
        // Usar Razor Engine o similar para plantillas más complejas
        return templateHtml;
    }
}
```

### 3. Registro de Envíos
Crear tabla para auditoría:
```csharp
public class RegistroEnvio
{
    public int Id { get; set; }
    public int EvaluacionId { get; set; }
    public string DestinatarioEmail { get; set; }
    public DateTime FechaEnvio { get; set; }
    public bool Exitoso { get; set; }
    public string MensajeError { get; set; }
}
```

### 4. Reenvío de Correos
Agregar funcionalidad para reenviar correos fallidos:
```csharp
[HttpPost]
public async Task<IActionResult> ReenviarEvaluacion(int evaluacionId)
{
    // Lógica de reenvío
}
```

### 5. Personalización de Plantillas
Permitir que los usuarios personalicen las plantillas de correo desde la interfaz.

### 6. Notificaciones en Tiempo Real
Usar SignalR para mostrar el progreso de envíos en tiempo real.

## Consideraciones de Seguridad

1. **Variables de Entorno**: Nunca hardcodear credenciales en el código
2. **Rate Limiting**: Implementar límites de envío para evitar spam
3. **Validación de Email**: Validar direcciones antes del envío
4. **Logs**: Registrar todos los intentos de envío para auditoría

## Testing

Para probar el sistema:
1. Configurar un servicio de email de prueba (MailHog, Mailtrap)
2. Crear evaluaciones de prueba
3. Verificar que los correos se generen correctamente
4. Probar escenarios de error

## Troubleshooting

### Problemas Comunes:
1. **SMTP Authentication Failed**: Verificar credenciales y habilitar "Less secure apps" en Gmail
2. **SendGrid API Key Invalid**: Verificar que la API key sea válida y tenga permisos
3. **Rate Limits**: Respetar los límites de envío del proveedor
4. **Spam Filters**: Configurar SPF, DKIM y DMARC para evitar spam

## Monitoreo

Implementar métricas para monitorear:
- Tasa de éxito de envíos
- Tiempo de respuesta
- Errores por tipo
- Volumen de correos por período

---

**Nota**: Recuerde reemplazar las llamadas a `SimularEnvioCorreo` con `EnviarCorreoReal` una vez implementado el servicio de email real.
