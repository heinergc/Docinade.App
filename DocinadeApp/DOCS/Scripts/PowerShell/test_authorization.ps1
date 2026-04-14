# Script simple para probar que las políticas de autorización funcionan
Write-Host "🔧 PROBANDO CONFIGURACIÓN DE POLÍTICAS DE AUTORIZACIÓN" -ForegroundColor Cyan
Write-Host ""

# Ir al directorio del proyecto
Set-Location "C:\Users\hguido\Desktop\RubricasApp\src\RubricasApp.Web"

Write-Host "1. Compilando proyecto..." -ForegroundColor Yellow
$buildResult = dotnet build --configuration Release --verbosity minimal
if ($LASTEXITCODE -eq 0) {
    Write-Host "   ✅ Compilación exitosa" -ForegroundColor Green
} else {
    Write-Host "   ❌ Error en compilación" -ForegroundColor Red
    exit 1
}

Write-Host "2. Verificando archivos de autorización..." -ForegroundColor Yellow
$files = @(
    "Authorization\PermissionPolicyProvider.cs",
    "Configuration\AuthorizationExtensions.cs",
    "Models\Permissions\ApplicationPermissions.cs"
)

foreach ($file in $files) {
    if (Test-Path $file) {
        Write-Host "   ✅ $file" -ForegroundColor Green
    } else {
        Write-Host "   ❌ $file" -ForegroundColor Red
    }
}

Write-Host "3. Iniciando aplicación para prueba..." -ForegroundColor Yellow
Write-Host "   Iniciando en puerto 5555 por 15 segundos..." -ForegroundColor Cyan

# Iniciar aplicación en background
$process = Start-Process "dotnet" -ArgumentList "run --no-build --urls=http://localhost:5555" -PassThru -WindowStyle Hidden

# Esperar un poco para que inicie
Start-Sleep 10

# Verificar si está corriendo
if ($process -and !$process.HasExited) {
    Write-Host "   ✅ Aplicación iniciada correctamente" -ForegroundColor Green
    Write-Host "   📱 Prueba manual: http://localhost:5555/Admin" -ForegroundColor Cyan
    
    # Terminar proceso
    $process.Kill()
    Write-Host "   🔄 Aplicación terminada" -ForegroundColor Yellow
} else {
    Write-Host "   ❌ Aplicación no pudo iniciar" -ForegroundColor Red
}

Write-Host ""
Write-Host "🎉 CONFIGURACIÓN LISTA PARA PRUEBAS" -ForegroundColor Green
Write-Host "Ejecuta: dotnet run" -ForegroundColor White
Write-Host "Navega a: https://localhost:5001/Admin" -ForegroundColor White