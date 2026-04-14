# Configuración de Email para el Sistema de Rúbricas

Este documento explica cómo configurar el servicio de correo electrónico SMTP en el sistema.

## Configuraciones Requeridas

En el archivo `appsettings.json` o `appsettings.Production.json`, agrega la sección de Email:

```json
{
  "Email": {
    "SmtpServer": "smtp.gmail.com",
    "SmtpPort": "587",
    "From": "tu-email@gmail.com",
    "FromName": "Sistema de Rúbricas",
    "Password": "tu-contraseña-de-aplicación",
    "DisplayName": "Sistema de Evaluaciones"
  }
}
```

## Configuraciones por Proveedor

### Gmail
- **SmtpServer**: `smtp.gmail.com`
- **SmtpPort**: `587`
- **EnableSsl**: `true` (automático)
- **Password**: Usar [Contraseñas de Aplicación](https://support.google.com/accounts/answer/185833)

### Outlook/Hotmail
- **SmtpServer**: `smtp-mail.outlook.com`
- **SmtpPort**: `587`
- **EnableSsl**: `true` (automático)

### Yahoo
- **SmtpServer**: `smtp.mail.yahoo.com`
- **SmtpPort**: `587`
- **EnableSsl**: `true` (automático)

### Servidor SMTP Personalizado
- **SmtpServer**: `tu-servidor-smtp.com`
- **SmtpPort**: `587` o `465`
- **EnableSsl**: `true` (recomendado)

## Seguridad

⚠️ **IMPORTANTE**: 
- Nunca incluyas contraseñas reales en el código fuente
- Usa variables de entorno en producción
- Para Gmail, usa contraseñas de aplicación, no la contraseña regular
- Habilita la autenticación de dos factores cuando sea posible

## Variables de Entorno (Producción)

En lugar de poner las credenciales directamente en el archivo de configuración, usa variables de entorno:

```bash
EMAIL__SMTPSERVER=smtp.gmail.com
EMAIL__SMTPPORT=587
EMAIL__FROM=tu-email@gmail.com
EMAIL__PASSWORD=tu-contraseña-de-aplicación
EMAIL__DISPLAYNAME="Sistema de Rúbricas"
```

## Ejemplos de Uso

El servicio se inyecta automáticamente y se puede usar en controladores:

```csharp
public class AccountController : Controller
{
    private readonly IEmailService _emailService;

    public AccountController(IEmailService emailService)
    {
        _emailService = emailService;
    }

    public async Task<IActionResult> SendWelcomeEmail(string userEmail)
    {
        var success = await _emailService.SendEmailAsync(
            userEmail,
            "Bienvenido al Sistema",
            "<h1>¡Bienvenido!</h1><p>Tu cuenta ha sido creada.</p>",
            isHtml: true
        );

        return Json(new { success });
    }
}
```

## Solución de Problemas

### Error: "Las credenciales no son válidas"
- Verifica que el email y contraseña sean correctos
- Para Gmail, asegúrate de usar una contraseña de aplicación
- Verifica que la autenticación de dos factores esté habilitada (Gmail)

### Error: "Conexión rechazada"
- Verifica el servidor SMTP y puerto
- Asegúrate de que EnableSsl esté configurado correctamente
- Verifica que no haya firewall bloqueando la conexión

### Error: "Configuración no establecida"
- Verifica que todas las configuraciones requeridas estén en appsettings.json
- Asegúrate de que los nombres de las claves sean exactos (sensibles a mayúsculas)

## Pruebas

Para probar la configuración de email, puedes usar el endpoint de administración o crear un controlador de prueba temporal.