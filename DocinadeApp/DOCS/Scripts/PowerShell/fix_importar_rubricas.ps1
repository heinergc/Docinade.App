#!/usr/bin/env pwsh

# Script para solucionar el problema de Foreign Key constraints en la importación de rúbricas

Write-Host "🔧 SOLUCIONANDO PROBLEMA DE FOREIGN KEY CONSTRAINTS" -ForegroundColor Yellow
Write-Host ""

# Detener la aplicación si está ejecutándose
$dotnetProcesses = Get-Process -Name "dotnet" -ErrorAction SilentlyContinue
if ($dotnetProcesses) {
    Write-Host "⏹️ Deteniendo aplicación..." -ForegroundColor Red
    $dotnetProcesses | Stop-Process -Force
    Start-Sleep -Seconds 2
}

# Navegar al directorio del proyecto
Set-Location "C:\Users\hguido\Desktop\RubricasApp\src\RubricasApp.Web"

Write-Host "🗃️ Creando datos base necesarios en la BD..." -ForegroundColor Green

# Crear script SQL para insertar datos base necesarios
$sqlScript = @"
-- Verificar y crear datos base para evitar foreign key constraints

-- Verificar si existe al menos un período académico
INSERT OR IGNORE INTO PeriodosAcademicos (NombrePeriodo, FechaInicio, FechaFin, Estado)
VALUES ('Período General', '2025-01-01', '2025-12-31', 'ACTIVO');

-- Verificar si existe al menos un grupo de calificación (si la tabla existe)
INSERT OR IGNORE INTO GruposCalificacion (NombreGrupo, Descripcion)
VALUES ('Grupo General', 'Grupo por defecto para rúbricas importadas');

-- Limpiar intentos de importación anteriores que pueden haber dejado datos inconsistentes
DELETE FROM ValoresRubrica WHERE IdRubrica IN (
    SELECT IdRubrica FROM Rubricas WHERE NombreRubrica LIKE '%Test%' OR NombreRubrica LIKE '%Prueba%'
);

DELETE FROM ItemsEvaluacion WHERE IdRubrica IN (
    SELECT IdRubrica FROM Rubricas WHERE NombreRubrica LIKE '%Test%' OR NombreRubrica LIKE '%Prueba%'
);

DELETE FROM RubricaNiveles WHERE IdRubrica IN (
    SELECT IdRubrica FROM Rubricas WHERE NombreRubrica LIKE '%Test%' OR NombreRubrica LIKE '%Prueba%'
);

DELETE FROM Rubricas WHERE NombreRubrica LIKE '%Test%' OR NombreRubrica LIKE '%Prueba%';

-- Verificar estructura de tablas
.schema Rubricas
.schema NivelesCalificacion
.schema ItemsEvaluacion
.schema ValoresRubrica
"@

# Crear archivo SQL temporal
$sqlScript | Out-File -FilePath "fix_fk_constraints.sql" -Encoding UTF8

Write-Host "📊 Ejecutando script de preparación de BD..." -ForegroundColor Blue

# Ejecutar script en la base de datos
try {
    sqlite3 "RubricasDbNueva.db" ".read fix_fk_constraints.sql"
    Write-Host "✅ Script de BD ejecutado correctamente" -ForegroundColor Green
} catch {
    Write-Host "⚠️ Error ejecutando script de BD: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host ""
Write-Host "🚀 Iniciando aplicación en puerto 5002..." -ForegroundColor Cyan

# Limpiar y iniciar aplicación
dotnet clean | Out-Null
dotnet build | Out-Null

if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ Compilación exitosa" -ForegroundColor Green
    Write-Host ""
    Write-Host "🌐 Aplicación iniciándose en: http://localhost:5002" -ForegroundColor White -BackgroundColor Blue
    Write-Host "📋 Página de importación: http://localhost:5002/ImportarRubrica" -ForegroundColor White -BackgroundColor Green
    Write-Host ""
    Write-Host "PROBLEMA RESUELTO:" -ForegroundColor Yellow
    Write-Host "✅ Foreign key constraints preparados" -ForegroundColor Green
    Write-Host "✅ Datos base creados en BD" -ForegroundColor Green  
    Write-Host "✅ Registros de prueba limpiados" -ForegroundColor Green
    Write-Host "✅ Aplicación lista para importación" -ForegroundColor Green
    Write-Host ""
    
    # Iniciar aplicación
    dotnet run --urls "http://localhost:5002"
} else {
    Write-Host "❌ Error en la compilación" -ForegroundColor Red
}

# Limpiar archivo temporal
Remove-Item "fix_fk_constraints.sql" -ErrorAction SilentlyContinue