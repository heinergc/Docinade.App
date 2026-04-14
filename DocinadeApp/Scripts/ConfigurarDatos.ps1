# =====================================================================
# SCRIPT DE POWERSHELL PARA CONFIGURAR DATOS DE PRUEBA
# Cuaderno Calificador PQ2025
# =====================================================================

param(
    [string]$DatabasePath = "",
    [switch]$UseCompleteScript = $false,
    [switch]$Verbose = $false
)

Write-Host "?? CONFIGURADOR DE DATOS DE PRUEBA - CUADERNO CALIFICADOR PQ2025" -ForegroundColor Cyan
Write-Host "=================================================================" -ForegroundColor Cyan

# Detectar ruta de la base de datos automáticamente
if ([string]::IsNullOrEmpty($DatabasePath)) {
    $possiblePaths = @(
        ".\Data\rubricas.db",
        ".\RubricasDb.db", 
        ".\App_Data\rubricas.db",
        "..\Data\rubricas.db"
    )
    
    foreach ($path in $possiblePaths) {
        if (Test-Path $path) {
            $DatabasePath = $path
            Write-Host "? Base de datos detectada: $DatabasePath" -ForegroundColor Green
            break
        }
    }
    
    if ([string]::IsNullOrEmpty($DatabasePath)) {
        Write-Host "? No se pudo detectar la base de datos automáticamente." -ForegroundColor Red
        Write-Host "?? Especifica la ruta con: .\ConfigurarDatos.ps1 -DatabasePath 'ruta\a\tu\db'" -ForegroundColor Yellow
        exit 1
    }
}

# Verificar que existe SQLite command line
$sqliteCmd = ""
$sqlitePaths = @("sqlite3", "sqlite3.exe", ".\sqlite3.exe")

foreach ($cmd in $sqlitePaths) {
    try {
        $result = & $cmd -version 2>$null
        if ($result) {
            $sqliteCmd = $cmd
            Write-Host "? SQLite encontrado: $cmd" -ForegroundColor Green
            break
        }
    }
    catch {
        # Continuar buscando
    }
}

if ([string]::IsNullOrEmpty($sqliteCmd)) {
    Write-Host "? SQLite command line no encontrado." -ForegroundColor Red
    Write-Host "?? Instala SQLite desde: https://sqlite.org/download.html" -ForegroundColor Yellow
    Write-Host "?? O ejecuta manualmente el script SQL en tu herramienta de base de datos preferida." -ForegroundColor Yellow
    
    # Mostrar rutas de scripts disponibles
    Write-Host "`n?? Scripts SQL disponibles:" -ForegroundColor Cyan
    if (Test-Path ".\Scripts\DatosPrueba_Simplificado.sql") {
        Write-Host "   • .\Scripts\DatosPrueba_Simplificado.sql (Recomendado)" -ForegroundColor White
    }
    if (Test-Path ".\Scripts\DatosPrueba_CuadernoCalificador.sql") {
        Write-Host "   • .\Scripts\DatosPrueba_CuadernoCalificador.sql (Completo)" -ForegroundColor White
    }
    
    exit 1
}

# Seleccionar script a ejecutar
$scriptPath = ""
if ($UseCompleteScript) {
    $scriptPath = ".\Scripts\DatosPrueba_CuadernoCalificador.sql"
    Write-Host "?? Usando script completo con detalles de evaluación" -ForegroundColor Yellow
} else {
    $scriptPath = ".\Scripts\DatosPrueba_Simplificado.sql"
    Write-Host "?? Usando script simplificado (recomendado)" -ForegroundColor Yellow
}

if (-not (Test-Path $scriptPath)) {
    Write-Host "? Script no encontrado: $scriptPath" -ForegroundColor Red
    exit 1
}

# Crear backup de la base de datos
$backupPath = $DatabasePath + ".backup." + (Get-Date -Format "yyyyMMdd_HHmmss")
try {
    Copy-Item $DatabasePath $backupPath
    Write-Host "?? Backup creado: $backupPath" -ForegroundColor Green
}
catch {
    Write-Host "?? No se pudo crear backup. Continuando..." -ForegroundColor Yellow
}

# Ejecutar script
Write-Host "`n?? Ejecutando script de datos de prueba..." -ForegroundColor Cyan

try {
    if ($Verbose) {
        & $sqliteCmd $DatabasePath ".read $scriptPath"
    } else {
        & $sqliteCmd $DatabasePath ".read $scriptPath" 2>$null
    }
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "? Script ejecutado exitosamente!" -ForegroundColor Green
    } else {
        Write-Host "?? Script ejecutado con advertencias (código: $LASTEXITCODE)" -ForegroundColor Yellow
    }
}
catch {
    Write-Host "? Error al ejecutar script: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

# Verificar datos insertados
Write-Host "`n?? Verificando datos insertados..." -ForegroundColor Cyan

$verificationQueries = @(
    @{Query = "SELECT COUNT(*) FROM Materias WHERE MateriaId = 1;"; Expected = 1; Description = "Materia 'Matemáticas I'"},
    @{Query = "SELECT COUNT(*) FROM Estudiantes WHERE PeriodoAcademicoId = 1;"; Expected = 5; Description = "Estudiantes en PQ2025"},
    @{Query = "SELECT COUNT(*) FROM InstrumentosEvaluacion WHERE InstrumentoId IN (1,2,3);"; Expected = 3; Description = "Instrumentos de evaluación"},
    @{Query = "SELECT COUNT(*) FROM InstrumentoMaterias WHERE MateriaId = 1;"; Expected = 3; Description = "Relaciones Instrumento-Materia"},
    @{Query = "SELECT COUNT(*) FROM InstrumentoRubricas;"; Expected = 3; Description = "Relaciones Instrumento-Rúbrica"},
    @{Query = "SELECT COUNT(*) FROM Evaluaciones WHERE Estado = 'FINALIZADA';"; Expected = -1; Description = "Evaluaciones finalizadas"}
)

$allVerified = $true

foreach ($check in $verificationQueries) {
    try {
        $result = & $sqliteCmd $DatabasePath $check.Query
        $count = [int]$result
        
        if ($check.Expected -eq -1) {
            # Para evaluaciones, solo verificamos que hay al menos algunas
            if ($count -gt 0) {
                Write-Host "? $($check.Description): $count registros" -ForegroundColor Green
            } else {
                Write-Host "? $($check.Description): Sin datos" -ForegroundColor Red
                $allVerified = $false
            }
        } elseif ($count -eq $check.Expected) {
            Write-Host "? $($check.Description): $count/$($check.Expected)" -ForegroundColor Green
        } else {
            Write-Host "?? $($check.Description): $count/$($check.Expected) (inesperado)" -ForegroundColor Yellow
        }
    }
    catch {
        Write-Host "? Error verificando $($check.Description)" -ForegroundColor Red
        $allVerified = $false
    }
}

# Mostrar siguiente paso
Write-Host "`n?? SIGUIENTE PASO:" -ForegroundColor Cyan
Write-Host "=================================================================" -ForegroundColor Cyan

if ($allVerified) {
    Write-Host "? ¡Datos configurados correctamente!" -ForegroundColor Green
    Write-Host "`n?? Ahora puedes probar el Cuaderno Calificador:" -ForegroundColor White
    Write-Host "   1. Ejecuta la aplicación web" -ForegroundColor White
    Write-Host "   2. Ve a: https://localhost:PUERTO/CalificadorPQ2025" -ForegroundColor White
    Write-Host "   3. Selecciona:" -ForegroundColor White
    Write-Host "      • Materia: Matemáticas I" -ForegroundColor White
    Write-Host "      • Período: Primer Cuatrimestre 2025" -ForegroundColor White
    Write-Host "   4. Haz clic en 'Generar Cuaderno'" -ForegroundColor White
    
    Write-Host "`n?? Resultados esperados:" -ForegroundColor Cyan
    Write-Host "   • Juan Carlos Pérez: 90.00 puntos" -ForegroundColor White
    Write-Host "   • María José González: 83.20 puntos" -ForegroundColor White
    Write-Host "   • Carlos Alberto Martínez: 88.60 puntos" -ForegroundColor White
    Write-Host "   • Algunos estudiantes con evaluaciones pendientes" -ForegroundColor White
} else {
    Write-Host "?? Algunos datos pueden no haberse insertado correctamente." -ForegroundColor Yellow
    Write-Host "?? Revisa los mensajes anteriores y ejecuta manualmente si es necesario." -ForegroundColor Yellow
}

Write-Host "`n?? Para más información, consulta:" -ForegroundColor Cyan
Write-Host "   • Documentation/GuiaPruebas_CuadernoCalificador.md" -ForegroundColor White
Write-Host "   • Documentation/CuadernoCalificador_README.md" -ForegroundColor White

Write-Host "`n=================================================================" -ForegroundColor Cyan
Write-Host "?? ¡Configuración completada!" -ForegroundColor Green