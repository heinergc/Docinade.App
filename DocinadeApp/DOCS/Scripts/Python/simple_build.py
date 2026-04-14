#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Script simplificado para ejecutar dotnet build
"""

import subprocess
import sys
import os
from datetime import datetime

def run_simple_build():
    """Ejecuta dotnet build en modo simple"""
    try:
        print("🔨 Ejecutando dotnet build...")
        print("=" * 50)
        
        # Cambiar al directorio del proyecto
        project_dir = r"C:\Users\hguido\Desktop\RubricasApp\src\RubricasApp.Web"
        os.chdir(project_dir)
        
        # Ejecutar dotnet build
        result = subprocess.run(
            ["dotnet", "build"],
            capture_output=True,
            text=True,
            encoding='utf-8'
        )
        
        print(f"📊 Código de salida: {result.returncode}")
        print("=" * 50)
        
        if result.stdout:
            print("📝 SALIDA:")
            print(result.stdout)
        
        if result.stderr:
            print("❌ ERRORES:")
            print(result.stderr)
        
        return result.returncode == 0
        
    except Exception as e:
        print(f"💥 Error ejecutando dotnet build: {e}")
        return False

if __name__ == "__main__":
    success = run_simple_build()
    if success:
        print("\n✅ COMPILACIÓN EXITOSA")
    else:
        print("\n❌ COMPILACIÓN FALLÓ")
    
    input("\nPresiona Enter para continuar...")