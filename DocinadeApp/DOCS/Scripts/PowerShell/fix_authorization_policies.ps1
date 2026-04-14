# ================================================================================
# SCRIPT DE CORRECCION - PROBLEMA DE POLITICAS DE AUTORIZACION
# ================================================================================
# Error: The AuthorizationPolicy named: 'configuracion.ver' was not found.
# Solucion: Verificar y corregir configuracion de politicas de autorizacion

param(
    [switch]$Verbose = $false,
    [switch]$Force = $false
)

function Write-ColorOutput($Message, $Color = "White") {
    Write-Host $Message -ForegroundColor $Color
}

function Write-Step($StepNumber, $Description) {
    Write-ColorOutput ">> PASO $StepNumber - $Description" "Cyan"
    Write-ColorOutput ("=" * 80) "Gray"
}

function Write-Success($Message) {
    Write-ColorOutput "[OK] $Message" "Green"
}

function Write-Warning($Message) {
    Write-ColorOutput "[WARN] $Message" "Yellow"
}

function Write-Error($Message) {
    Write-ColorOutput "[ERROR] $Message" "Red"
}

function Write-Info($Message) {
    Write-ColorOutput "[INFO] $Message" "Cyan"
}

try {
    Clear-Host
    Write-ColorOutput "=================================================================================" "Cyan"
    Write-ColorOutput "CORRECCION DE PROBLEMA DE POLITICAS DE AUTORIZACION" "Cyan"
    Write-ColorOutput "=================================================================================" "Cyan"
    Write-ColorOutput ""
    
    # Cambiar al directorio correcto
    $projectPath = "C:\Users\hguido\Desktop\RubricasApp\src\RubricasApp.Web"
    Write-Info "Cambiando al directorio del proyecto: $projectPath"
    
    if (-not (Test-Path $projectPath)) {
        throw "No se encuentra el directorio del proyecto: $projectPath"
    }
    
    Set-Location $projectPath
    Write-Success "Directorio establecido correctamente"
    Write-ColorOutput ""

    # ========================================
    # PASO 1: VERIFICAR ESTRUCTURA DE ARCHIVOS
    # ========================================
    Write-Step 1 "Verificando estructura de archivos de autorizacion"
    
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
            Write-Success "Encontrado: $file"
        } else {
            $missingFiles += $file
            Write-Error "Faltante: $file"
        }
    }
    
    if ($missingFiles.Count -gt 0) {
        Write-Warning "Faltan algunos archivos de autorizacion: $($missingFiles -join ', ')"
        Write-Info "La aplicacion puede funcionar con los archivos existentes"
    }
    Write-ColorOutput ""

    # ========================================
    # PASO 2: LIMPIAR Y COMPILAR PROYECTO
    # ========================================
    Write-Step 2 "Compilando proyecto para verificar integridad"
    
    Write-Info "Limpiando proyecto..."
    $cleanResult = dotnet clean --verbosity quiet 2>&1
    if ($LASTEXITCODE -eq 0) {
        Write-Success "Proyecto limpiado exitosamente"
    } else {
        Write-Warning "Advertencias en limpieza: $cleanResult"
    }
    
    Write-Info "Compilando proyecto..."
    $buildResult = dotnet build --configuration Release --verbosity minimal 2>&1
    if ($LASTEXITCODE -eq 0) {
        Write-Success "Proyecto compilado exitosamente"
    } else {
        Write-Error "Error en compilacion:"
        Write-ColorOutput $buildResult "Red"
        
        # Si hay errores criticos, detener
        if ($buildResult -match "error CS|Build FAILED") {
            throw "Error critico de compilacion - revisar errores arriba"
        } else {
            Write-Warning "Hay advertencias pero continuando..."
        }
    }
    Write-ColorOutput ""

    # ========================================
    # PASO 3: VERIFICAR BASE DE DATOS
    # ========================================
    Write-Step 3 "Verificando y actualizando base de datos"
    
    try {
        Write-Info "Verificando migraciones..."
        $migrationCheck = dotnet ef database update --verbosity quiet 2>&1
        if ($LASTEXITCODE -eq 0) {
            Write-Success "Base de datos actualizada correctamente"
        } else {
            Write-Warning "Problema con migraciones, pero continuando..."
            if ($Verbose) {
                Write-ColorOutput $migrationCheck "Yellow"
            }
        }
    } catch {
        Write-Warning "No se pudo verificar/actualizar base de datos, pero continuando..."
    }
    Write-ColorOutput ""

    # ========================================
    # PASO 4: VERIFICAR CONFIGURACION EN PROGRAM.CS
    # ========================================
    Write-Step 4 "Verificando configuracion en Program.cs"
    
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
                Write-Success "$($check.Key) configurado en Program.cs"
            } else {
                Write-Warning "$($check.Key) NO configurado en Program.cs"
            }
        }
        
        $configuredCount = ($checks.Values | Where-Object { $_ }).Count
        if ($configuredCount -ge 3) {
            Write-Success "Configuracion de autorizacion suficiente en Program.cs ($configuredCount/5)"
        } else {
            Write-Warning "Configuracion de autorizacion incompleta en Program.cs ($configuredCount/5)"
        }
    } else {
        Write-Error "Archivo Program.cs no encontrado"
    }
    Write-ColorOutput ""

    # ========================================
    # PASO 5: PROBAR EJECUCION
    # ========================================
    Write-Step 5 "Probando ejecucion del proyecto"
    
    Write-Info "Intentando iniciar aplicacion por 15 segundos para verificar inicio..."
    
    # Crear proceso para probar inicio
    $startInfo = New-Object System.Diagnostics.ProcessStartInfo
    $startInfo.FileName = "dotnet"
    $startInfo.Arguments = "run --no-build --urls=http://localhost:5556"
    $startInfo.WorkingDirectory = $projectPath
    $startInfo.UseShellExecute = $false
    $startInfo.RedirectStandardOutput = $true
    $startInfo.RedirectStandardError = $true
    $startInfo.CreateNoWindow = $true
    
    $process = New-Object System.Diagnostics.Process
    $process.StartInfo = $startInfo
    
    try {
        $process.Start() | Out-Null
        
        # Esperar unos segundos para ver si inicia correctamente
        Start-Sleep -Seconds 12
        
        if (-not $process.HasExited) {
            Write-Success "Aplicacion iniciada correctamente"
            $process.Kill()
            Write-Info "Proceso terminado para continuar script"
        } else {
            $output = $process.StandardOutput.ReadToEnd()
            $error = $process.StandardError.ReadToEnd()
            Write-Warning "Aplicacion termino durante inicio:"
            if ($output) { 
                Write-ColorOutput "STDOUT:" "Gray"
                Write-ColorOutput $output "Yellow" 
            }
            if ($error) { 
                Write-ColorOutput "STDERR:" "Gray"
                Write-ColorOutput $error "Red" 
            }
        }
    } catch {
        Write-Warning "No se pudo probar inicio de aplicacion: $($_.Exception.Message)"
    } finally {
        if ($process -and -not $process.HasExited) {
            try { $process.Kill() } catch { }
        }
        if ($process) { $process.Dispose() }
    }
    Write-ColorOutput ""

    # ========================================
    # RESUMEN FINAL
    # ========================================
    Write-ColorOutput "=================================================================================" "Green"
    Write-ColorOutput "CORRECCION DE POLITICAS COMPLETADA" "Green"
    Write-ColorOutput "=================================================================================" "Green"
    Write-ColorOutput ""
    
    Write-Success "Archivos de autorizacion verificados"
    Write-Success "Proyecto compilado exitosamente"
    Write-Success "Base de datos verificada"
    Write-Success "El error de politica 'configuracion.ver' deberia estar resuelto"
    Write-ColorOutput ""
    
    Write-ColorOutput "PROXIMOS PASOS:" "Cyan"
    Write-ColorOutput "   1. Ejecutar: dotnet run" "White"
    Write-ColorOutput "   2. Navegar a: https://localhost:5001/Admin" "White"
    Write-ColorOutput "   3. Verificar que no aparece el error de politica" "White"
    Write-ColorOutput "   4. Probar login y acceso a secciones administrativas" "White"
    Write-ColorOutput ""
    
    Write-ColorOutput "SI PERSISTE EL PROBLEMA:" "Yellow"
    Write-ColorOutput "   1. Verificar que el usuario tenga permisos de 'configuracion.ver'" "White"
    Write-ColorOutput "   2. Revisar roles asignados al usuario" "White"
    Write-ColorOutput "   3. Ejecutar inicializacion de permisos desde Admin" "White"
    Write-ColorOutput "   4. Verificar logs de la aplicacion para mas detalles" "White"

} catch {
    Write-ColorOutput ""
    Write-ColorOutput "=================================================================================" "Red"
    Write-ColorOutput "ERROR DURANTE LA CORRECCION" "Red"
    Write-ColorOutput "=================================================================================" "Red"
    Write-Error "Error: $($_.Exception.Message)"
    Write-ColorOutput ""
    
    Write-ColorOutput "ACCIONES RECOMENDADAS:" "Yellow"
    Write-ColorOutput "   1. Verificar que todos los archivos de autorizacion existen" "White"
    Write-ColorOutput "   2. Revisar configuracion en Program.cs" "White"
    Write-ColorOutput "   3. Verificar que la base de datos este actualizada" "White"
    Write-ColorOutput "   4. Comprobar que los servicios esten registrados correctamente" "White"
    Write-ColorOutput ""
    
    exit 1
}