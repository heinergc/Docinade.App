#!/usr/bin/env python3
"""
Script para ejecutar dotnet build y verificar errores
"""
import subprocess
import os

def run_build():
    os.chdir(r"C:\Users\hguido\Desktop\RubricasApp\src\RubricasApp.Web")
    
    result = subprocess.run(
        ["dotnet", "build", "--verbosity", "minimal"],
        capture_output=True,
        text=True,
        encoding='utf-8'
    )
    
    print(f"Código de retorno: {result.returncode}")
    
    if result.stdout:
        print("SALIDA:")
        print(result.stdout)
    
    if result.stderr:
        print("\nERRORES:")
        print(result.stderr)
    
    return result.returncode == 0

if __name__ == "__main__":
    success = run_build()
    print(f"\n{'✅ ÉXITO' if success else '❌ FALLÓ'}")