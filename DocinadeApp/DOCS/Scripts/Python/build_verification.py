#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Script para ejecutar dotnet build y analizar los resultados
"""

import subprocess
import sys
import os
from datetime import datetime

def run_dotnet_build():
    """Ejecuta dotnet build y muestra los resultados"""
    try:
        print("🔨 EJECUTANDO DOTNET BUILD")
        print("=" * 60)
        
        # Cambiar al directorio del proyecto
        project_dir = r"C:\Users\hguido\Desktop\RubricasApp\src\RubricasApp.Web"
        os.chdir(project_dir)
        print(f"📁 Directorio: {project_dir}")
        
        # Ejecutar dotnet restore primero
        print("\n🔄 Ejecutando dotnet restore...")
        restore_result = subprocess.run(
            ["dotnet", "restore"],
            capture_output=True,
            text=True,
            encoding='utf-8'
        )
        
        if restore_result.returncode != 0:
            print("❌ Error en dotnet restore:")
            print(restore_result.stderr)
            return False
        else:
            print("✅ Restore completado exitosamente")
        
        # Ejecutar dotnet build
        print("\n🔨 Ejecutando dotnet build...")
        build_result = subprocess.run(
            ["dotnet", "build", "--no-restore", "--verbosity", "normal"],
            capture_output=True,
            text=True,
            encoding='utf-8'
        )
        
        print(f"📊 Código de salida: {build_result.returncode}")
        print("=" * 60)
        
        # Procesar salida
        success = build_result.returncode == 0
        
        if build_result.stdout:
            print("📝 SALIDA DEL BUILD:")
            output_lines = build_result.stdout.split('\n')
            
            # Filtrar líneas importantes
            important_lines = []
            for line in output_lines:
                line = line.strip()
                if any(keyword in line.lower() for keyword in ['error', 'warning', 'failed', 'succeeded', 'build']):
                    important_lines.append(line)
            
            if important_lines:
                for line in important_lines[-20:]:  # Últimas 20 líneas importantes
                    print(f"  {line}")
            else:
                # Si no hay líneas importantes, mostrar las últimas líneas
                for line in output_lines[-10:]:
                    if line.strip():
                        print(f"  {line}")
        
        if build_result.stderr:
            print("\n❌ ERRORES:")
            error_lines = build_result.stderr.split('\n')
            for line in error_lines:
                if line.strip():
                    print(f"  {line}")
        
        # Resumen
        print("\n" + "=" * 60)
        if success:
            print("✅ COMPILACIÓN EXITOSA! 🎉")
            print("El proyecto se compiló sin errores.")
        else:
            print("❌ COMPILACIÓN FALLÓ")
            print("Revisar los errores mostrados arriba.")
        
        return success
        
    except Exception as e:
        print(f"💥 Error ejecutando dotnet build: {e}")
        return False

def main():
    print("🚀 VERIFICADOR DE COMPILACIÓN - RUBRICASAPP.WEB")
    print("=" * 60)
    print(f"⏰ Timestamp: {datetime.now().strftime('%Y-%m-%d %H:%M:%S')}")
    print()
    
    success = run_dotnet_build()
    
    print("\n" + "=" * 60)
    if success:
        print("🎯 RESULTADO: COMPILACIÓN EXITOSA")
        print("✅ El proyecto está listo para ejecutarse")
    else:
        print("⚠️  RESULTADO: COMPILACIÓN FALLÓ")
        print("❌ Revisar y corregir los errores mostrados")
    
    return 0 if success else 1

if __name__ == "__main__":
    exit_code = main()
    print(f"\n🏁 Proceso completado con código: {exit_code}")
    sys.exit(exit_code)