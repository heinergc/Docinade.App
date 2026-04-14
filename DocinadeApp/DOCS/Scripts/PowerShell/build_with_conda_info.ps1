# Script PowerShell para compilar proyecto con información de Python/Conda
# Fecha: 28 de julio de 2025

Write-Host "============================================================" -ForegroundColor Cyan
Write-Host "           COMPILACIÓN DE PROYECTO RUBRICAS APP           " -ForegroundColor Yellow
Write-Host "============================================================" -ForegroundColor Cyan
Write-Host "📅 Fecha: $(Get-Date)" -ForegroundColor Green
Write-Host "📁 Directorio: $(Get-Location)" -ForegroundColor Green
Write-Host "🐍 Python configurado con Conda" -ForegroundColor Green
Write-Host "============================================================" -ForegroundColor Cyan
Write-Host ""

# Verificar que estamos en el directorio correcto
if (-not (Test-Path "RubricasApp.Web.csproj")) {
    Write-Host "❌ Error: No se encontró RubricasApp.Web.csproj" -ForegroundColor Red
    Write-Host "📁 Asegúrate de estar en el directorio correcto del proyecto" -ForegroundColor Yellow
    Read-Host "Presiona Enter para continuar"
    exit 1
}

# Verificar información de Python/Conda
Write-Host "🔍 Verificando entorno Python/Conda..." -ForegroundColor Cyan
try {
    if ($env:CONDA_DEFAULT_ENV) {
        Write-Host "✅ Entorno Conda activo: $($env:CONDA_DEFAULT_ENV)" -ForegroundColor Green
    } else {
        Write-Host "⚠️  No se detectó entorno Conda activo" -ForegroundColor Yellow
    }
    
    $pythonVersion = python --version 2>&1
    Write-Host "✅ Python disponible: $pythonVersion" -ForegroundColor Green
} catch {
    Write-Host "⚠️  Python no disponible en PATH" -ForegroundColor Yellow
}

Write-Host ""

# Función para ejecutar comando y capturar salida
function Invoke-DotNetCommand {
    param(
        [string]$Command,
        [string]$Description,
        [string]$LogFile
    )
    
    Write-Host "🔄 $Description..." -ForegroundColor Cyan
    
    try {
        $result = Invoke-Expression "dotnet $Command" 2>&1
        $exitCode = $LASTEXITCODE
        
        # Guardar resultado en log
        $logPath = "DOCS\BuildLogs\$LogFile"
        $result | Out-File -FilePath $logPath -Encoding UTF8
        
        if ($exitCode -eq 0) {
            Write-Host "✅ $Description completado exitosamente" -ForegroundColor Green
            return $true
        } else {
            Write-Host "❌ Error en $Description" -ForegroundColor Red
            Write-Host "📄 Log guardado en: $logPath" -ForegroundColor Yellow
            Write-Host "Errores:" -ForegroundColor Red
            $result | Write-Host -ForegroundColor Red
            return $false
        }
    } catch {
        Write-Host "❌ Excepción en $Description`: $($_.Exception.Message)" -ForegroundColor Red
        return $false
    }
}

# Paso 1: Limpiar proyecto
$cleanSuccess = Invoke-DotNetCommand -Command "clean" -Description "Limpieza del proyecto" -LogFile "clean_output_28julio.txt"

Write-Host ""

# Paso 2: Restaurar dependencias
$restoreSuccess = Invoke-DotNetCommand -Command "restore" -Description "Restauración de dependencias" -LogFile "restore_output_28julio.txt"

if (-not $restoreSuccess) {
    Write-Host "❌ No se puede continuar sin restaurar dependencias" -ForegroundColor Red
    Read-Host "Presiona Enter para salir"
    exit 1
}

Write-Host ""

# Paso 3: Compilar proyecto
$buildSuccess = Invoke-DotNetCommand -Command "build --configuration Debug --verbosity normal" -Description "Compilación del proyecto" -LogFile "build_output_28julio.txt"

Write-Host ""
Write-Host "============================================================" -ForegroundColor Cyan
Write-Host "                    RESUMEN FINAL                         " -ForegroundColor Yellow
Write-Host "============================================================" -ForegroundColor Cyan

if ($env:CONDA_DEFAULT_ENV) {
    Write-Host "✅ Python/Conda: Configurado ($($env:CONDA_DEFAULT_ENV))" -ForegroundColor Green
} else {
    Write-Host "⚠️  Python/Conda: No detectado" -ForegroundColor Yellow
}

if ($cleanSuccess) {
    Write-Host "✅ Limpieza: Exitosa" -ForegroundColor Green
} else {
    Write-Host "⚠️  Limpieza: Con advertencias" -ForegroundColor Yellow
}

if ($restoreSuccess) {
    Write-Host "✅ Restauración: Exitosa" -ForegroundColor Green
} else {
    Write-Host "❌ Restauración: Falló" -ForegroundColor Red
}

if ($buildSuccess) {
    Write-Host "✅ Compilación: Exitosa" -ForegroundColor Green
    Write-Host "🚀 Estado: LISTO PARA EJECUTAR" -ForegroundColor Green
    Write-Host ""
    Write-Host "🎉 ¡PROYECTO COMPILADO EXITOSAMENTE!" -ForegroundColor Green
    Write-Host "💡 Puedes ejecutar el proyecto con: dotnet run" -ForegroundColor Cyan
} else {
    Write-Host "❌ Compilación: Falló" -ForegroundColor Red
    Write-Host "💥 Estado: ERROR EN COMPILACIÓN" -ForegroundColor Red
    Write-Host ""
    Write-Host "📋 Acciones requeridas:" -ForegroundColor Yellow
    Write-Host "   1. Revisar errores en DOCS\BuildLogs\build_output_28julio.txt" -ForegroundColor Yellow
    Write-Host "   2. Corregir errores de código" -ForegroundColor Yellow
    Write-Host "   3. Volver a ejecutar compilación" -ForegroundColor Yellow
}

Write-Host "============================================================" -ForegroundColor Cyan
Read-Host "Presiona Enter para continuar"

if ($buildSuccess) {
    exit 0
} else {
    exit 1
}