Set-Location "C:\Users\hguido\Desktop\RubricasApp\src\RubricasApp.Web"
dotnet build > build_results.txt 2>&1
Write-Host "Build completed. Check build_results.txt for results."
Get-Content build_results.txt