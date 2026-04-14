try {
    $response = Invoke-WebRequest -Uri "http://localhost:18164/Materias/Edit/2" -UseBasicParsing
    Write-Host "✅ SUCCESS: Status Code: $($response.StatusCode)"
    Write-Host "Content Length: $($response.Content.Length) characters"
    
    # Verificar si contiene el formulario de edición
    if ($response.Content -like "*EditarMateriaVm*" -or $response.Content -like "*Editar Materia*") {
        Write-Host "✅ La vista de edición se cargó correctamente"
    } else {
        Write-Host "⚠️  La respuesta no parece ser la vista de edición esperada"
    }
}
catch {
    Write-Host "❌ ERROR: $($_.Exception.Message)"
}

# También probar el índice de materias
try {
    $indexResponse = Invoke-WebRequest -Uri "http://localhost:18164/Materias" -UseBasicParsing
    Write-Host "✅ Materias Index Status: $($indexResponse.StatusCode)"
}
catch {
    Write-Host "❌ Materias Index Error: $($_.Exception.Message)"
}