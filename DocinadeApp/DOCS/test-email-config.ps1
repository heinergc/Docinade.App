# Script de prueba para verificar configuración de Email SMTP
# Prueba la conexión con Gmail usando la configuración del sistema

$smtpServer = "smtp.gmail.com"
$smtpPort = 587
$emailFrom = "heiguicam@gmail.com"
$emailPassword = "ldlh pzqx bbzu qbgo"
$emailTo = "heiguicam@gmail.com"  # Enviar a sí mismo para prueba

Write-Host "======================================" -ForegroundColor Cyan
Write-Host "  PRUEBA DE CONFIGURACION SMTP" -ForegroundColor Cyan
Write-Host "======================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Servidor SMTP: $smtpServer" -ForegroundColor Yellow
Write-Host "Puerto: $smtpPort" -ForegroundColor Yellow
Write-Host "De: $emailFrom" -ForegroundColor Yellow
Write-Host "Para: $emailTo" -ForegroundColor Yellow
Write-Host ""

try {
    Write-Host "[INFO] Creando credenciales..." -ForegroundColor Green
    $securePassword = ConvertTo-SecureString $emailPassword -AsPlainText -Force
    $credential = New-Object System.Management.Automation.PSCredential($emailFrom, $securePassword)
    
    Write-Host "[INFO] Configurando mensaje de prueba..." -ForegroundColor Green
    
    $subject = "[PRUEBA] Configuracion SMTP - Sistema de Rubricas"
    $body = @"
<html>
<head>
    <style>
        body { font-family: Arial, sans-serif; }
        .container { padding: 20px; background-color: #f0f0f0; }
        .header { background-color: #4CAF50; color: white; padding: 10px; }
        .content { background-color: white; padding: 20px; margin-top: 10px; }
        .footer { margin-top: 20px; font-size: 12px; color: #666; }
    </style>
</head>
<body>
    <div class="container">
        <div class="header">
            <h2>Prueba de Configuracion SMTP</h2>
        </div>
        <div class="content">
            <h3>Estado: EXITOSO</h3>
            <p>Este es un correo de prueba del Sistema de Rubricas MEP.</p>
            <p><strong>Fecha:</strong> $(Get-Date -Format "dd/MM/yyyy HH:mm:ss")</p>
            <p><strong>Servidor:</strong> $smtpServer</p>
            <p><strong>Puerto:</strong> $smtpPort</p>
            <p>Si recibe este correo, la configuracion SMTP esta funcionando correctamente.</p>
        </div>
        <div class="footer">
            <p>Sistema de Evaluacion por Rubricas - MEP Costa Rica</p>
        </div>
    </div>
</body>
</html>
"@

    Write-Host "[INFO] Enviando correo de prueba..." -ForegroundColor Green
    
    Send-MailMessage -SmtpServer $smtpServer `
                     -Port $smtpPort `
                     -From $emailFrom `
                     -To $emailTo `
                     -Subject $subject `
                     -Body $body `
                     -BodyAsHtml `
                     -UseSsl `
                     -Credential $credential `
                     -Encoding UTF8
    
    Write-Host ""
    Write-Host "======================================" -ForegroundColor Green
    Write-Host "  CORREO ENVIADO EXITOSAMENTE" -ForegroundColor Green
    Write-Host "======================================" -ForegroundColor Green
    Write-Host ""
    Write-Host "Revise la bandeja de entrada de: $emailTo" -ForegroundColor Yellow
    Write-Host ""
    
} catch {
    Write-Host ""
    Write-Host "======================================" -ForegroundColor Red
    Write-Host "  ERROR AL ENVIAR CORREO" -ForegroundColor Red
    Write-Host "======================================" -ForegroundColor Red
    Write-Host ""
    Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host ""
    Write-Host "Posibles causas:" -ForegroundColor Yellow
    Write-Host "1. La contraseña de aplicacion es incorrecta" -ForegroundColor Yellow
    Write-Host "2. La cuenta de Gmail no tiene habilitada la verificacion en 2 pasos" -ForegroundColor Yellow
    Write-Host "3. No se ha generado una contraseña de aplicacion en Gmail" -ForegroundColor Yellow
    Write-Host "4. El firewall bloquea el puerto 587" -ForegroundColor Yellow
    Write-Host "5. Problemas de conexion a Internet" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Para generar una contraseña de aplicacion:" -ForegroundColor Cyan
    Write-Host "1. Ir a https://myaccount.google.com/" -ForegroundColor Cyan
    Write-Host "2. Seguridad > Verificacion en 2 pasos > Contraseñas de aplicaciones" -ForegroundColor Cyan
    Write-Host "3. Crear nueva contraseña para 'Correo'" -ForegroundColor Cyan
    Write-Host "4. Copiar la contraseña y actualizar appsettings.json" -ForegroundColor Cyan
    Write-Host ""
}
