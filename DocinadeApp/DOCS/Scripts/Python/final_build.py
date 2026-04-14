#!/usr/bin/env python3
import subprocess
import os

os.chdir(r"C:\Users\hguido\Desktop\RubricasApp\src\RubricasApp.Web")

print("🔨 EJECUTANDO DOTNET BUILD")
print("=" * 50)

result = subprocess.run(["dotnet", "build"], capture_output=True, text=True)

print(f"Código: {result.returncode}")

if result.returncode == 0:
    print("✅ COMPILACIÓN EXITOSA! 🎉")
    print("El proyecto se compiló sin errores.")
else:
    print("❌ COMPILACIÓN FALLÓ")
    if result.stderr:
        print("\nERRORES:")
        for line in result.stderr.split('\n'):
            if line.strip():
                print(f"  {line}")

# Mostrar últimas líneas de salida
if result.stdout:
    lines = result.stdout.split('\n')
    print(f"\nÚLTIMAS LÍNEAS DE SALIDA:")
    for line in lines[-10:]:
        if line.strip():
            print(f"  {line}")

print("=" * 50)