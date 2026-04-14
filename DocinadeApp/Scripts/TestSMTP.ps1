#requires -Version 5.1
# Script de prueba de configuración SMTP

Write-Host "=== Prueba de Configuración SMTP ===" -ForegroundColor Cyan
Write-Host ""

# Asegurar TLS 1.2 (útil en hosts viejos)
try { [Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12 } catch {}

# Resolver ruta de appsettings.json (si PSScriptRoot es $null, usar el directorio actual)
$basePath = if ($PSScriptRoot) { $PSScriptRoot } else { (Get-Location).Path }
$appSettingsPath = Join-Path (Split-Path $basePath -Parent) "appsettings.json"

if (-not (Test-Path -LiteralPath $appSettingsPath)) {
    Write-Host "✗ No se encontró: $appSettingsPath" -ForegroundColor Red
    Write-Host "  Ajuste la ruta o ejecute el script desde la carpeta /scripts del proyecto."
    Write-Host ""
    Read-Host "Presione Enter para salir"
    exit 1
}

# Leer configuración
try {
    $config = Get-Content -LiteralPath $appSettingsPath -Raw -ErrorAction Stop | ConvertFrom-Json -ErrorAction Stop

    # Ajuste aquí si su JSON tiene otro nodo/estructura
    $emailCfg = $config.Email
    if (-not $emailCfg) { throw "No se encontró el nodo 'Email' en appsettings.json." }

    $smtpServer = $emailCfg.SmtpServer
    $smtpPort   = [int]$emailCfg.SmtpPort
    $from       = $emailCfg.From
    $password   = $emailCfg.Password

    if ([string]::IsNullOrWhiteSpace($smtpServer) -or
        -not $smtpPort -or
        [string]::IsNullOrWhiteSpace($from) -or
        [string]::IsNullOrWhiteSpace($password)) {
        throw "Faltan valores en Email.{SmtpServer|SmtpPort|From|Password}."
    }
}
catch {
    Write-Host "✗ Error leyendo configuración: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host ""
    Read-Host "Presione Enter para salir"
    exit 1
}

# Mostrar configuración (enmascarar password)
$pwPreview = if ($password.Length -ge 4) { $password.Substring(0,4) + "****" } else { "****" }
Write-Host "Configuración actual:" -ForegroundColor Yellow
Write-Host "  Servidor SMTP: $smtpServer"
Write-Host "  Puerto:        $smtpPort"
Write-Host "  Desde:         $from"
Write-Host "  Contraseña:    $pwPreview"
Write-Host ""

# Correo de prueba
$toEmail = Read-Host "Ingrese el correo de destino para prueba"
if ([string]::IsNullOrWhiteSpace($toEmail)) {
    Write-Host "✗ Debe ingresar un correo de destino." -ForegroundColor Red
    Read-Host "Presione Enter para salir"
    exit 1
}

Write-Host ""
Write-Host "Enviando correo de prueba..." -ForegroundColor Green

try {
    $securePassword = ConvertTo-SecureString $password -AsPlainText -Force
    $credential = [System.Management.Automation.PSCredential]::new($from, $securePassword)

    $mailParams = @{
        SmtpServer = $smtpServer
        Port       = $smtpPort
        UseSsl     = $true                # Para 587 (STARTTLS). Si usa 465, considere la alternativa PS7 abajo.
        Credential = $credential
        From       = $from
        To         = $toEmail
        Subject    = "Prueba de Configuración SMTP - Sistema de Rúbricas"
        Body       = "Este es un correo de prueba para verificar la configuración SMTP del sistema."
        ErrorAction= 'Stop'
    }

    # Nota: Send-MailMessage está obsoleto y puede no existir en PowerShell 7+
    Send-MailMessage @mailParams

    Write-Host ""
    Write-Host "✓ Correo enviado exitosamente!" -ForegroundColor Green
    Write-Host "  Verifique la bandeja de entrada de: $toEmail" -ForegroundColor Cyan
}
catch {
    Write-Host ""
    Write-Host "✗ Error al enviar correo:" -ForegroundColor Red
    Write-Host "  $($_.Exception.Message)" -ForegroundColor Red
    Write-Host ""

    if ($_.Exception.Message -match "Authentication|Autenticación|535|534") {
        Write-Host "Posibles soluciones:" -ForegroundColor Yellow
        Write-Host "  1) Verificar que la contraseña de aplicación sea correcta"
        Write-Host "  2) Generar una nueva contraseña de aplicación (Gmail): https://myaccount.google.com/apppasswords"
        Write-Host "  3) Asegurar que la verificación en 2 pasos esté activa"
        Write-Host "  4) Usar puerto 587 (TLS) y no 465 con este cmdlet"
    }
}
finally {
    Write-Host ""
    Write-Host "Presione cualquier tecla para salir..."
    $null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
}
