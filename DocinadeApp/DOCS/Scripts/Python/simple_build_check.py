import subprocess
import os

# Cambiar al directorio del proyecto
os.chdir(r"C:\Users\hguido\Desktop\RubricasApp\src\RubricasApp.Web")

print("🔨 Ejecutando dotnet build...")
print("=" * 50)

try:
    # Ejecutar dotnet build
    result = subprocess.run(
        ["dotnet", "build"],
        text=True,
        capture_output=True
    )
    
    print(f"Código de retorno: {result.returncode}")
    print("=" * 50)
    
    if result.stdout:
        print("SALIDA:")
        print(result.stdout)
    
    if result.stderr:
        print("\nERRORES:")
        print(result.stderr)
    
    # Resultado final
    if result.returncode == 0:
        print("\n✅ ¡COMPILACIÓN EXITOSA!")
    else:
        print("\n❌ COMPILACIÓN FALLÓ")
        
except Exception as e:
    print(f"Error: {e}")