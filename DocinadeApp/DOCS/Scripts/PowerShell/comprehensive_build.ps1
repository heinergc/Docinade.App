Set-Location "C:\Users\hguido\Desktop\RubricasApp\src\RubricasApp.Web"
Write-Host "Limpiando build anterior..."
dotnet clean > clean_results.txt 2>&1

Write-Host "Iniciando nueva compilación..."
dotnet build > build_results_new.txt 2>&1

Write-Host "Compilación completada. Revisando resultados..."
$buildContent = Get-Content build_results_new.txt
Write-Host "=== RESULTADOS DE COMPILACIÓN ==="
$buildContent
Write-Host "=== FIN DE RESULTADOS ==="

# Contar errores
$errorLines = $buildContent | Where-Object { $_ -match "error CS" }
$errorCount = $errorLines.Count
Write-Host "Total de errores encontrados: $errorCount"

if ($errorCount -gt 0) {
    Write-Host "=== ERRORES DETECTADOS ==="
    $errorLines | ForEach-Object { Write-Host $_ }
}