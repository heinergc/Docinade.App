# Script para verificar y corregir la codificación UTF-8 de archivos de vistas
# Ejecutar desde el directorio raíz del proyecto

Write-Host "?? Verificando codificación de archivos de vistas..." -ForegroundColor Yellow

$viewsPath = "Views"
$problematicFiles = @()

# Buscar archivos .cshtml
$cshtmlFiles = Get-ChildItem -Path $viewsPath -Filter "*.cshtml" -Recurse

foreach ($file in $cshtmlFiles) {
    try {
        # Leer el archivo como bytes
        $bytes = [System.IO.File]::ReadAllBytes($file.FullName)
        
        # Verificar si tiene BOM UTF-8 (EF BB BF)
        $hasUtf8Bom = $bytes.Length -ge 3 -and $bytes[0] -eq 0xEF -and $bytes[1] -eq 0xBB -and $bytes[2] -eq 0xBF
        
        # Leer como texto para verificar contenido
        $content = [System.IO.File]::ReadAllText($file.FullName, [System.Text.Encoding]::UTF8)
        
        # Verificar si contiene caracteres especiales que podrían causar problemas
        $hasSpecialChars = $content -match '[áéíóúñÁÉÍÓÚÑ¿¡ü]'
        
        if ($hasSpecialChars -and -not $hasUtf8Bom) {
            $problematicFiles += $file
            Write-Host "??  $($file.FullName) - Tiene caracteres especiales pero no BOM UTF-8" -ForegroundColor Red
        } elseif ($hasUtf8Bom) {
            Write-Host "? $($file.FullName) - Correcto (UTF-8 con BOM)" -ForegroundColor Green
        } else {
            Write-Host "??  $($file.FullName) - Sin caracteres especiales" -ForegroundColor Gray
        }
    }
    catch {
        Write-Host "? Error al procesar $($file.FullName): $($_.Exception.Message)" -ForegroundColor Red
    }
}

# Corregir archivos problemáticos
if ($problematicFiles.Count -gt 0) {
    Write-Host "`n?? Corrigiendo archivos problemáticos..." -ForegroundColor Yellow
    
    foreach ($file in $problematicFiles) {
        try {
            # Leer el contenido
            $content = [System.IO.File]::ReadAllText($file.FullName, [System.Text.Encoding]::UTF8)
            
            # Escribir con UTF-8 BOM
            $utf8WithBom = New-Object System.Text.UTF8Encoding($true)
            [System.IO.File]::WriteAllText($file.FullName, $content, $utf8WithBom)
            
            Write-Host "? Corregido: $($file.FullName)" -ForegroundColor Green
        }
        catch {
            Write-Host "? Error al corregir $($file.FullName): $($_.Exception.Message)" -ForegroundColor Red
        }
    }
    
    Write-Host "`n?? Proceso completado. $($problematicFiles.Count) archivos corregidos." -ForegroundColor Green
} else {
    Write-Host "`n? Todos los archivos tienen la codificación correcta." -ForegroundColor Green
}

Write-Host "`n?? Recomendaciones:" -ForegroundColor Cyan
Write-Host "1. Reinicia el servidor de desarrollo (dotnet run)" -ForegroundColor White
Write-Host "2. Limpia la caché del navegador (Ctrl+F5)" -ForegroundColor White
Write-Host "3. Verifica que tu editor esté configurado para UTF-8 con BOM" -ForegroundColor White

# Verificar configuración del layout
$layoutPath = "Views\Shared\_Layout.cshtml"
if (Test-Path $layoutPath) {
    $layoutContent = Get-Content $layoutPath -Raw
    if ($layoutContent -match 'charset=utf-8') {
        Write-Host "? Layout tiene charset UTF-8 configurado" -ForegroundColor Green
    } else {
        Write-Host "??  Layout podría necesitar charset UTF-8" -ForegroundColor Yellow
    }
}

Read-Host "Presiona Enter para continuar..."