import subprocess
import os

print("🔨 Ejecutando dotnet build...")
print("=" * 50)

# Cambiar al directorio del proyecto
os.chdir(r"C:\Users\hguido\Desktop\RubricasApp\src\RubricasApp.Web")

# Ejecutar dotnet build
result = subprocess.run(
    ["dotnet", "build", "--verbosity", "minimal"],
    capture_output=True,
    text=True
)

print(f"Código de retorno: {result.returncode}")

if result.stdout:
    print("\nSALIDA:")
    lines = result.stdout.split('\n')
    for line in lines:
        if line.strip() and any(word in line.lower() for word in ['error', 'warning', 'build', 'failed', 'succeeded']):
            print(f"  {line}")

if result.stderr:
    print("\nERRORES:")
    lines = result.stderr.split('\n')
    for line in lines:
        if line.strip():
            print(f"  {line}")

print(f"\n{'✅ ÉXITO' if result.returncode == 0 else '❌ FALLÓ'}")