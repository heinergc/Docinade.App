Set-Location "C:\Users\hguido\Desktop\RubricasApp\src\RubricasApp.Web"

Write-Host "🔧 CORRIGIENDO ERRORES Y EJECUTANDO BUILD" -ForegroundColor Cyan
Write-Host ("=" * 60) -ForegroundColor Cyan

# Ejecutar build
Write-Host "`n🔨 Ejecutando dotnet build..." -ForegroundColor Yellow
$result = Start-Process -FilePath "dotnet" -ArgumentList "build" -Wait -PassThru -NoNewWindow -RedirectStandardOutput "build_output.txt" -RedirectStandardError "build_errors.txt"

Write-Host "Código de salida: $($result.ExitCode)" -ForegroundColor Yellow

if ($result.ExitCode -eq 0) 
{
    Write-Host "`n✅ COMPILACIÓN EXITOSA! 🎉" -ForegroundColor Green
    Write-Host "Todos los errores del UsersController han sido corregidos." -ForegroundColor Green
    
    if (Test-Path "build_output.txt") 
    {
        $output = Get-Content "build_output.txt" -Tail 10
        Write-Host "`nÚLTIMAS LÍNEAS DE SALIDA:" -ForegroundColor Cyan
        $output | ForEach-Object { Write-Host "  $_" }
    }
} 
else 
{
    Write-Host "`n❌ COMPILACIÓN FALLÓ" -ForegroundColor Red
    Write-Host "Revisando errores restantes..." -ForegroundColor Yellow
    
    if (Test-Path "build_errors.txt") 
    {
        $errors = Get-Content "build_errors.txt"
        if ($errors) 
        {
            Write-Host "`nERRORES ENCONTRADOS:" -ForegroundColor Red
            $errors | ForEach-Object { 
                if ($_ -match "error CS") 
                {
                    Write-Host "  🔸 $_" -ForegroundColor Yellow
                }
            }
        }
    }
    
    if (Test-Path "build_output.txt") 
    {
        $output = Get-Content "build_output.txt" -Tail 15
        Write-Host "`nÚLTIMAS LÍNEAS DE SALIDA:" -ForegroundColor Cyan
        $output | ForEach-Object { Write-Host "  $_" }
    }
}

Write-Host ("`n" + ("=" * 60)) -ForegroundColor Cyan

# Limpiar archivos temporales
if (Test-Path "build_output.txt") { Remove-Item "build_output.txt" }
if (Test-Path "build_errors.txt") { Remove-Item "build_errors.txt" }