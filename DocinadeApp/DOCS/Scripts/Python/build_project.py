#!/usr/bin/env python3
"""
Script para ejecutar dotnet build y capturar la salida
Fecha: 28 de julio de 2025
"""

import subprocess
import sys
import os
from datetime import datetime

def run_dotnet_build():
    """Ejecuta dotnet build y captura la salida"""
    
    # Cambiar al directorio del proyecto
    project_dir = r"C:\Users\hguido\Desktop\RubricasApp\src\RubricasApp.Web"
    os.chdir(project_dir)
    
    print("=" * 60)
    print("🏗️  INICIANDO COMPILACIÓN DE PROYECTO RUBRICAS APP")
    print("=" * 60)
    print(f"📅 Fecha: {datetime.now().strftime('%Y-%m-%d %H:%M:%S')}")
    print(f"📁 Directorio: {project_dir}")
    print(f"🐍 Python: {sys.version}")
    print("=" * 60)
    
    try:
        # Ejecutar dotnet clean primero
        print("\n🧹 Limpiando proyecto...")
        clean_result = subprocess.run(
            ["dotnet", "clean"], 
            capture_output=True, 
            text=True, 
            cwd=project_dir
        )
        
        if clean_result.returncode == 0:
            print("✅ Limpieza completada exitosamente")
        else:
            print("⚠️  Advertencia en la limpieza:")
            print(clean_result.stderr)
        
        # Ejecutar dotnet restore
        print("\n📦 Restaurando dependencias...")
        restore_result = subprocess.run(
            ["dotnet", "restore"], 
            capture_output=True, 
            text=True, 
            cwd=project_dir
        )
        
        if restore_result.returncode == 0:
            print("✅ Restauración completada exitosamente")
        else:
            print("❌ Error en la restauración:")
            print(restore_result.stderr)
            return False
        
        # Ejecutar dotnet build
        print("\n🔨 Compilando proyecto...")
        build_result = subprocess.run(
            ["dotnet", "build", "--configuration", "Debug", "--verbosity", "normal"], 
            capture_output=True, 
            text=True, 
            cwd=project_dir
        )
        
        print("\n" + "=" * 60)
        print("📊 RESULTADOS DE LA COMPILACIÓN")
        print("=" * 60)
        
        if build_result.returncode == 0:
            print("✅ COMPILACIÓN EXITOSA!")
            print("\n📝 Salida de la compilación:")
            print(build_result.stdout)
            
            # Guardar log exitoso
            with open("DOCS/BuildLogs/successful_build_log.txt", "w", encoding="utf-8") as f:
                f.write(f"Compilación exitosa - {datetime.now()}\n")
                f.write("=" * 50 + "\n")
                f.write(build_result.stdout)
            
            return True
        else:
            print("❌ ERROR EN LA COMPILACIÓN")
            print("\n🚨 Errores encontrados:")
            print(build_result.stderr)
            
            if build_result.stdout:
                print("\n📝 Salida adicional:")
                print(build_result.stdout)
            
            # Guardar log de errores
            with open("DOCS/BuildLogs/error_build_log.txt", "w", encoding="utf-8") as f:
                f.write(f"Errores de compilación - {datetime.now()}\n")
                f.write("=" * 50 + "\n")
                f.write("STDERR:\n")
                f.write(build_result.stderr)
                f.write("\n\nSTDOUT:\n")
                f.write(build_result.stdout)
            
            return False
            
    except FileNotFoundError:
        print("❌ Error: dotnet no está instalado o no está en el PATH")
        return False
    except Exception as e:
        print(f"❌ Error inesperado: {e}")
        return False

def check_project_files():
    """Verifica que los archivos principales del proyecto existan"""
    
    required_files = [
        "RubricasApp.Web.csproj",
        "Program.cs",
        "appsettings.json"
    ]
    
    print("\n🔍 Verificando archivos del proyecto...")
    
    for file in required_files:
        if os.path.exists(file):
            print(f"✅ {file}")
        else:
            print(f"❌ {file} - NO ENCONTRADO")
            return False
    
    return True

if __name__ == "__main__":
    print("🐍 Ejecutando desde Python con Conda")
    
    # Verificar archivos del proyecto
    if not check_project_files():
        print("❌ Faltan archivos esenciales del proyecto")
        sys.exit(1)
    
    # Ejecutar compilación
    success = run_dotnet_build()
    
    if success:
        print("\n🎉 ¡COMPILACIÓN COMPLETADA EXITOSAMENTE!")
        print("✅ El proyecto está listo para ejecutarse")
        sys.exit(0)
    else:
        print("\n💥 LA COMPILACIÓN FALLÓ")
        print("❌ Revisa los errores arriba y corrige los problemas")
        sys.exit(1)