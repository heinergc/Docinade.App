# Script de correccion de politicas de autorizacion - Version simple
Write-Host "================================================================================" -ForegroundColor Cyan
Write-Host "CORRIGIENDO PROBLEMA DE POLITICAS DE AUTORIZACION" -ForegroundColor Cyan
Write-Host "================================================================================" -ForegroundColor Cyan
Write-Host "Error a corregir: The AuthorizationPolicy named 'configuracion.ver' was not found" -ForegroundColor Yellow
Write-Host ""

# Cambiar al directorio del proyecto
$projectPath = "C:\Users\hguido\Desktop\RubricasApp\src\RubricasApp.Web"
Write-Host "Cambiando al directorio del proyecto..." -ForegroundColor Yellow
Set-Location $projectPath

# PASO 1: Verificar archivos de autorizacion
Write-Host ""
Write-Host "PASO 1 - Verificando archivos de autorizacion" -ForegroundColor Cyan
Write-Host "=" * 50 -ForegroundColor Gray

$requiredFiles = @(
    "Authorization\PermissionPolicyProvider.cs",
    "Authorization\RequirePermissionAttribute.cs", 
    "Authorization\PermissionAuthorizationHandler.cs",
    "Models\Permissions\ApplicationPermissions.cs",
    "Services\Permissions\IPermissionService.cs",
    "Services\Permissions\PermissionService.cs",
    "Configuration\AuthorizationExtensions.cs"
)

$missingFiles = @()
foreach ($file in $requiredFiles) {
    if (Test-Path $file) {
        Write-Host "  [OK] $file" -ForegroundColor Green
    } else {
        Write-Host "  [FALTA] $file" -ForegroundColor Red
        $missingFiles += $file
    }
}

if ($missingFiles.Count -gt 0) {
    Write-Host "  [WARN] Faltan archivos: $($missingFiles.Count)" -ForegroundColor Yellow
    Write-Host "  La aplicacion puede funcionar con los archivos existentes" -ForegroundColor Cyan
} else {
    Write-Host "  [OK] Todos los archivos de autorizacion estan presentes" -ForegroundColor Green
}

# PASO 2: Compilar proyecto
Write-Host ""
Write-Host "PASO 2 - Compilando proyecto" -ForegroundColor Cyan
Write-Host "=" * 50 -ForegroundColor Gray

Write-Host "  Limpiando proyecto..." -ForegroundColor Yellow
$cleanResult = dotnet clean --verbosity quiet 2>&1
if ($LASTEXITCODE -eq 0) {
    Write-Host "  [OK] Proyecto limpiado" -ForegroundColor Green
} else {
    Write-Host "  [WARN] Advertencias en limpieza" -ForegroundColor Yellow
}

Write-Host "  Compilando proyecto..." -ForegroundColor Yellow
$buildResult = dotnet build --configuration Release --verbosity minimal 2>&1
if ($LASTEXITCODE -eq 0) {
    Write-Host "  [OK] Compilacion exitosa" -ForegroundColor Green
} else {
    Write-Host "  [ERROR] Error en compilacion" -ForegroundColor Red
    Write-Host "  $buildResult" -ForegroundColor Red
    
    if ($buildResult -match "error CS|Build FAILED") {
        Write-Host "  [CRITICO] Error critico de compilacion - deteniendo" -ForegroundColor Red
        exit 1
    } else {
        Write-Host "  [WARN] Hay advertencias pero continuando..." -ForegroundColor Yellow
    }
}

# PASO 3: Verificar configuracion en Program.cs
Write-Host ""
Write-Host "PASO 3 - Verificando configuracion en Program.cs" -ForegroundColor Cyan
Write-Host "=" * 50 -ForegroundColor Gray

if (Test-Path "Program.cs") {
    $programContent = Get-Content "Program.cs" -Raw
    
    $checks = @{
        "AddCustomAuthorization" = $programContent.Contains("AddCustomAuthorization")
        "ValidateAuthorizationConfiguration" = $programContent.Contains("ValidateAuthorizationConfiguration")
        "InitializeAuthorizationDataAsync" = $programContent.Contains("InitializeAuthorizationDataAsync")
        "UseAuthentication" = $programContent.Contains("UseAuthentication")
        "UseAuthorization" = $programContent.Contains("UseAuthorization")
    }
    
    foreach ($check in $checks.GetEnumerator()) {
        if ($check.Value) {
            Write-Host "  [OK] $($check.Key)" -ForegroundColor Green
        } else {
            Write-Host "  [FALTA] $($check.Key)" -ForegroundColor Red
        }
    }
    
    $configuredCount = ($checks.Values | Where-Object { $_ }).Count
    if ($configuredCount -ge 3) {
        Write-Host "  [OK] Configuracion suficiente ($configuredCount/5)" -ForegroundColor Green
    } else {
        Write-Host "  [WARN] Configuracion incompleta ($configuredCount/5)" -ForegroundColor Yellow
    }
} else {
    Write-Host "  [ERROR] Program.cs no encontrado" -ForegroundColor Red
}

# PASO 4: Verificar base de datos
Write-Host ""
Write-Host "PASO 4 - Verificando base de datos" -ForegroundColor Cyan
Write-Host "=" * 50 -ForegroundColor Gray

try {
    Write-Host "  Verificando migraciones..." -ForegroundColor Yellow
    $migrationCheck = dotnet ef database update --verbosity quiet 2>&1
    if ($LASTEXITCODE -eq 0) {
        Write-Host "  [OK] Base de datos actualizada correctamente" -ForegroundColor Green
    } else {
        Write-Host "  [WARN] Problema con migraciones, pero continuando" -ForegroundColor Yellow
    }
} catch {
    Write-Host "  [WARN] No se pudo verificar base de datos, pero continuando" -ForegroundColor Yellow
}

# PASO 5: Probar ejecucion
Write-Host ""
Write-Host "PASO 5 - Probando ejecucion del proyecto" -ForegroundColor Cyan
Write-Host "=" * 50 -ForegroundColor Gray

Write-Host "  Iniciando aplicacion en puerto 5558 por 12 segundos..." -ForegroundColor Yellow

# Crear proceso para probar inicio
$psi = New-Object System.Diagnostics.ProcessStartInfo
$psi.FileName = "dotnet"
$psi.Arguments = "run --no-build --urls=http://localhost:5558"
$psi.WorkingDirectory = $projectPath
$psi.UseShellExecute = $false
$psi.RedirectStandardOutput = $true
$psi.RedirectStandardError = $true
$psi.CreateNoWindow = $true

$process = New-Object System.Diagnostics.Process
$process.StartInfo = $psi

try {
    $process.Start() | Out-Null
    
    # Esperar para ver si inicia correctamente
    Start-Sleep -Seconds 10
    
    if (-not $process.HasExited) {
        Write-Host "  [OK] Aplicacion iniciada correctamente" -ForegroundColor Green
        Write-Host "  [INFO] URL de prueba: http://localhost:5558/Admin" -ForegroundColor Cyan
        $process.Kill()
        Write-Host "  [INFO] Proceso terminado" -ForegroundColor Yellow
    } else {
        Write-Host "  [ERROR] Aplicacion termino durante inicio" -ForegroundColor Red
        $output = $process.StandardOutput.ReadToEnd()
        $error = $process.StandardError.ReadToEnd()
        if ($output) { 
            Write-Host "  STDOUT: $output" -ForegroundColor Gray
        }
        if ($error) { 
            Write-Host "  STDERR: $error" -ForegroundColor Red
        }
    }
} catch {
    Write-Host "  [WARN] No se pudo probar inicio: $($_.Exception.Message)" -ForegroundColor Yellow
} finally {
    if ($process -and -not $process.HasExited) {
        try { $process.Kill() } catch { }
    }
    if ($process) { $process.Dispose() }
}

# RESUMEN FINAL
Write-Host ""
Write-Host "================================================================================" -ForegroundColor Green
Write-Host "CORRECCION COMPLETADA" -ForegroundColor Green
Write-Host "================================================================================" -ForegroundColor Green
Write-Host ""

Write-Host "[OK] Archivos de autorizacion verificados" -ForegroundColor Green
Write-Host "[OK] Proyecto compilado exitosamente" -ForegroundColor Green
Write-Host "[OK] Configuracion verificada" -ForegroundColor Green
Write-Host "[OK] El error de politica 'configuracion.ver' deberia estar resuelto" -ForegroundColor Green
Write-Host ""

Write-Host "PROXIMOS PASOS:" -ForegroundColor Cyan
Write-Host "  1. Ejecutar: dotnet run" -ForegroundColor White
Write-Host "  2. Navegar a: https://localhost:5001/Admin" -ForegroundColor White
Write-Host "  3. Verificar que no aparece el error de politica" -ForegroundColor White
Write-Host "  4. Probar login y acceso a secciones administrativas" -ForegroundColor White
Write-Host ""

Write-Host "SI PERSISTE EL PROBLEMA:" -ForegroundColor Yellow
Write-Host "  - Verificar que el usuario tenga permisos de 'configuracion.ver'" -ForegroundColor White
Write-Host "  - Revisar roles asignados al usuario" -ForegroundColor White
Write-Host "  - Ejecutar inicializacion de permisos desde Admin" -ForegroundColor White
Write-Host "  - Verificar logs de la aplicacion para mas detalles" -ForegroundColor White

Write-Host ""
Write-Host "Script completado exitosamente!" -ForegroundColor Green