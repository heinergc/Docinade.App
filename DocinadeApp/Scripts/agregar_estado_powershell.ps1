-- Comando PowerShell para agregar columna Estado a SQLite
-- Ejecutar desde C:\Users\hguido\Desktop\RubricasApp\src\RubricasApp.Web

# Verificar si sqlite3 está disponible
if (Get-Command sqlite3 -ErrorAction SilentlyContinue) {
    Write-Host "SQLite3 encontrado, ejecutando migración..."
    sqlite3 RubricasDbNueva.db ".schema Evaluaciones"
    sqlite3 RubricasDbNueva.db "ALTER TABLE Evaluaciones ADD COLUMN Estado TEXT DEFAULT 'BORRADOR';"
    sqlite3 RubricasDbNueva.db "UPDATE Evaluaciones SET Estado = 'COMPLETADA' WHERE Estado IS NULL OR Estado = '';"
    sqlite3 RubricasDbNueva.db ".schema Evaluaciones"
} else {
    Write-Host "SQLite3 no encontrado. Usar método alternativo..."
    
    # Usar Entity Framework para la migración
    dotnet ef migrations add AgregarColumnEstado
    dotnet ef database update
}