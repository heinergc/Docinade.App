# Script para verificar y ejecutar la aplicación
Write-Host "=== Verificando Base de Datos ===" -ForegroundColor Green

# Verificar materias
$materiasCount = & sqlite3 RubricasDb.db "SELECT COUNT(*) FROM Materias;"
Write-Host "Total Materias: $materiasCount" -ForegroundColor Yellow

# Verificar específicamente la materia ID 2
$materia2 = & sqlite3 RubricasDb.db "SELECT MateriaId, Codigo, Nombre FROM Materias WHERE MateriaId = 2;"
Write-Host "Materia ID 2: $materia2" -ForegroundColor Yellow

# Verificar períodos
$periodosCount = & sqlite3 RubricasDb.db "SELECT COUNT(*) FROM PeriodosAcademicos;"
Write-Host "Total Períodos: $periodosCount" -ForegroundColor Yellow

# Verificar estudiantes
$estudiantesCount = & sqlite3 RubricasDb.db "SELECT COUNT(*) FROM Estudiantes;"
Write-Host "Total Estudiantes: $estudiantesCount" -ForegroundColor Yellow

Write-Host "`n=== Ejecutando Aplicación ===" -ForegroundColor Green
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd 'C:\Users\hguido\Desktop\RubricasApp\src\RubricasApp.Web'; dotnet run --urls 'http://localhost:18164'"

Write-Host "Esperando que la aplicación inicie..." -ForegroundColor Yellow
Start-Sleep -Seconds 10

Write-Host "`n=== Probando Endpoints ===" -ForegroundColor Green

# Probar endpoint principal
try {
    $response = Invoke-WebRequest -Uri "http://localhost:18164/" -UseBasicParsing
    Write-Host "✅ Home: Status $($response.StatusCode)" -ForegroundColor Green
} catch {
    Write-Host "❌ Home: Error - $($_.Exception.Message)" -ForegroundColor Red
}

# Probar índice de materias
try {
    $response = Invoke-WebRequest -Uri "http://localhost:18164/Materias" -UseBasicParsing
    Write-Host "✅ Materias Index: Status $($response.StatusCode)" -ForegroundColor Green
} catch {
    Write-Host "❌ Materias Index: Error - $($_.Exception.Message)" -ForegroundColor Red
}

# Probar edición de materia
try {
    $response = Invoke-WebRequest -Uri "http://localhost:18164/Materias/Edit/2" -UseBasicParsing
    Write-Host "✅ Materias Edit/2: Status $($response.StatusCode)" -ForegroundColor Green
} catch {
    Write-Host "❌ Materias Edit/2: Error - $($_.Exception.Message)" -ForegroundColor Red
}

# Probar detalles de materia
try {
    $response = Invoke-WebRequest -Uri "http://localhost:18164/Materias/Details/2" -UseBasicParsing
    Write-Host "✅ Materias Details/2: Status $($response.StatusCode)" -ForegroundColor Green
} catch {
    Write-Host "❌ Materias Details/2: Error - $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host "`n=== Pruebas Completadas ===" -ForegroundColor Green