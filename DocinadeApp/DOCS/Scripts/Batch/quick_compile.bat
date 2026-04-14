dotnet clean > compilation_log.txt 2>&1
echo ===== CLEAN COMPLETED ===== >> compilation_log.txt

dotnet restore >> compilation_log.txt 2>&1  
echo ===== RESTORE COMPLETED ===== >> compilation_log.txt

dotnet build --no-restore >> compilation_log.txt 2>&1
echo ===== BUILD COMPLETED ===== >> compilation_log.txt

echo Build process finished. Check compilation_log.txt for details.