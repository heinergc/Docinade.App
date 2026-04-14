# Script de PowerShell para aplicar migración de base de datos
param(
    [string]$ConnectionString = "Data Source=RubricasDb.db"
)

Write-Host "Aplicando migración para agregar columna Ciclo..."

try {
    # Usar System.Data.SQLite si está disponible
    Add-Type -AssemblyName System.Data.SQLite

    $connection = New-Object System.Data.SQLite.SQLiteConnection($ConnectionString)
    $connection.Open()

    $command = $connection.CreateCommand()
    $command.CommandText = "ALTER TABLE PeriodosAcademicos ADD COLUMN Ciclo TEXT NOT NULL DEFAULT 'I';"
    $command.ExecuteNonQuery()

    Write-Host "Columna Ciclo agregada exitosamente."

    # Verificar la estructura de la tabla
    $command.CommandText = "PRAGMA table_info(PeriodosAcademicos);"
    $reader = $command.ExecuteReader()
    
    Write-Host "Estructura de la tabla PeriodosAcademicos:"
    while ($reader.Read()) {
        Write-Host "- $($reader["name"]) ($($reader["type"]))"
    }
    $reader.Close()

    $connection.Close()
    Write-Host "Migración completada exitosamente."
}
catch {
    Write-Error "Error al aplicar la migración: $($_.Exception.Message)"
    
    # Fallback: usar dotnet ef si System.Data.SQLite no está disponible
    Write-Host "Intentando con dotnet ef..."
    
    # Cambiar al directorio del proyecto
    Push-Location "C:\Users\hguido\Desktop\RubricasApp\src\RubricasApp.Web"
    
    try {
        # Aplicar migraciones pendientes
        dotnet ef database update
        Write-Host "Migración aplicada con dotnet ef."
    }
    catch {
        Write-Error "Error con dotnet ef: $($_.Exception.Message)"
    }
    finally {
        Pop-Location
    }
}