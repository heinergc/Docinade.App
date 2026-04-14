#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Script para verificar errores de compilación en el proyecto RubricasApp.Web
"""

import subprocess
import sys
import os
from datetime import datetime

def run_dotnet_build():
    """Ejecuta dotnet build y captura la salida"""
    try:
        print("🔨 Ejecutando dotnet build...")
        print("=" * 60)
        
        # Cambiar al directorio del proyecto
        project_dir = r"C:\Users\hguido\Desktop\RubricasApp\src\RubricasApp.Web"
        os.chdir(project_dir)
        
        # Ejecutar dotnet build
        result = subprocess.run(
            ["dotnet", "build", "--verbosity", "detailed"],
            capture_output=True,
            text=True,
            encoding='utf-8'
        )
        
        print(f"📊 Código de salida: {result.returncode}")
        print("=" * 60)
        
        if result.stdout:
            print("📝 SALIDA ESTÁNDAR:")
            print(result.stdout)
            print("=" * 60)
        
        if result.stderr:
            print("❌ ERRORES:")
            print(result.stderr)
            print("=" * 60)
        
        # Guardar log detallado
        timestamp = datetime.now().strftime("%Y%m%d_%H%M%S")
        log_file = f"build_log_{timestamp}.txt"
        
        with open(log_file, 'w', encoding='utf-8') as f:
            f.write(f"BUILD LOG - {datetime.now()}\n")
            f.write("=" * 60 + "\n")
            f.write(f"Return Code: {result.returncode}\n")
            f.write("=" * 60 + "\n")
            f.write("STDOUT:\n")
            f.write(result.stdout)
            f.write("\n" + "=" * 60 + "\n")
            f.write("STDERR:\n")
            f.write(result.stderr)
        
        print(f"📄 Log guardado en: {log_file}")
        
        return result.returncode == 0, result.stdout, result.stderr
        
    except Exception as e:
        print(f"💥 Error ejecutando dotnet build: {e}")
        return False, "", str(e)

def analyze_errors(stderr_output):
    """Analiza los errores y proporciona un resumen"""
    if not stderr_output:
        print("✅ No se encontraron errores.")
        return
    
    print("🔍 ANÁLISIS DE ERRORES:")
    print("=" * 60)
    
    lines = stderr_output.split('\n')
    error_count = 0
    warning_count = 0
    
    errors_by_type = {}
    
    for line in lines:
        line = line.strip()
        if not line:
            continue
            
        if "error CS" in line:
            error_count += 1
            # Extraer tipo de error
            if "CS" in line:
                error_code = line.split("CS")[1].split(":")[0]
                error_type = f"CS{error_code}"
                if error_type not in errors_by_type:
                    errors_by_type[error_type] = []
                errors_by_type[error_type].append(line)
        elif "warning CS" in line:
            warning_count += 1
    
    print(f"❌ Total de errores: {error_count}")
    print(f"⚠️  Total de advertencias: {warning_count}")
    print()
    
    if errors_by_type:
        print("📋 ERRORES POR TIPO:")
        for error_type, error_lines in errors_by_type.items():
            print(f"\n{error_type}: {len(error_lines)} ocurrencias")
            for i, error_line in enumerate(error_lines[:3]):  # Mostrar solo las primeras 3
                print(f"  • {error_line}")
            if len(error_lines) > 3:
                print(f"  ... y {len(error_lines) - 3} más")

def main():
    print("🚀 VERIFICADOR DE ERRORES DE COMPILACIÓN")
    print("=" * 60)
    print(f"Timestamp: {datetime.now()}")
    print(f"Directorio: {os.getcwd()}")
    print()
    
    success, stdout, stderr = run_dotnet_build()
    
    print()
    analyze_errors(stderr)
    
    if success:
        print("✅ COMPILACIÓN EXITOSA")
    else:
        print("❌ COMPILACIÓN FALLÓ")
    
    return 0 if success else 1

if __name__ == "__main__":
    sys.exit(main())