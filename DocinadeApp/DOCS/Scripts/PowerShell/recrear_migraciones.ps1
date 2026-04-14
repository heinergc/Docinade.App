# Script para recrear migraciones con Identity
Write-Host "Recreando migraciones con soporte completo para Identity..." -ForegroundColor Green

# 1. Eliminar base de datos existente
Write-Host "`n1. Eliminando base de datos existente..." -ForegroundColor Yellow
$dbFiles = @("RubricasDbNueva.db", "RubricasDbNueva.db-shm", "RubricasDbNueva.db-wal")
foreach ($file in $dbFiles) {
    if (Test-Path $file) {
        Remove-Item $file -Force
        Write-Host "   Eliminado: $file" -ForegroundColor Gray
    }
}

# 2. Eliminar migraciones existentes
Write-Host "`n2. Eliminando migraciones existentes..." -ForegroundColor Yellow
if (Test-Path "Migrations") {
    Remove-Item "Migrations" -Recurse -Force
    Write-Host "   Carpeta Migrations eliminada" -ForegroundColor Gray
}

# 3. Crear nueva migración inicial
Write-Host "`n3. Creando nueva migración inicial..." -ForegroundColor Yellow
try {
    dotnet ef migrations add InitialCreateWithIdentity --verbose
    Write-Host "   Migración creada exitosamente" -ForegroundColor Green
} catch {
    Write-Host "   Error al crear migración: $_" -ForegroundColor Red
    exit 1
}

# 4. Aplicar migración a la base de datos
Write-Host "`n4. Aplicando migraciones a la base de datos..." -ForegroundColor Yellow
try {
    dotnet ef database update --verbose
    Write-Host "   Base de datos actualizada exitosamente" -ForegroundColor Green
} catch {
    Write-Host "   Error al actualizar base de datos: $_" -ForegroundColor Red
    exit 1
}

Write-Host "`n✅ Proceso completado exitosamente!" -ForegroundColor Green
Write-Host "La base de datos ahora incluye todas las tablas de Identity y la aplicación." -ForegroundColor Green

# Opcional: Mostrar tablas creadas
Write-Host "`n5. Verificando tablas creadas..." -ForegroundColor Yellow
try {
    $connectionString = "Data Source=RubricasDbNueva.db"
    Add-Type -Path "$env:USERPROFILE\.nuget\packages\microsoft.data.sqlite\8.0.0\lib\net8.0\Microsoft.Data.Sqlite.dll"
    
    $connection = New-Object Microsoft.Data.Sqlite.SqliteConnection($connectionString)
    $connection.Open()
    
    $command = $connection.CreateCommand()
    $command.CommandText = "SELECT name FROM sqlite_master WHERE type='table' ORDER BY name"
    $reader = $command.ExecuteReader()
    
    Write-Host "   Tablas en la base de datos:" -ForegroundColor Gray
    while ($reader.Read()) {
        $tableName = $reader.GetString(0)
        Write-Host "   - $tableName" -ForegroundColor Gray
    }
    
    $reader.Close()
    $connection.Close()
} catch {
    Write-Host "   No se pudo verificar las tablas: $_" -ForegroundColor Yellow
}

Write-Host "`nPresione Enter para continuar..."
Read-Host