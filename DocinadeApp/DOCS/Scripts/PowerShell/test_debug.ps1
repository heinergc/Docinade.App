try {
    $response = Invoke-WebRequest "http://localhost:18164/Materias/Debug/2" -UseBasicParsing
    Write-Host "Status: $($response.StatusCode)"
    Write-Host "Content: $($response.Content)"
} catch {
    Write-Host "Error: $($_.Exception.Message)"
}