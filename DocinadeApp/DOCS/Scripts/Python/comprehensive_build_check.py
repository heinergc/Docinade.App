#!/usr/bin/env python3
"""
Script de verificación pre-compilación y dotnet build
Integrado con Conda Python
Fecha: 28 de julio de 2025
"""

import os
import sys
import subprocess
from pathlib import Path
from datetime import datetime

def print_header():
    """Imprime el encabezado del script"""
    print("=" * 70)
    print("🏗️  VERIFICACIÓN Y COMPILACIÓN - RUBRICAS APP")
    print("=" * 70)
    print(f"📅 Fecha: {datetime.now().strftime('%Y-%m-%d %H:%M:%S')}")
    print(f"🐍 Python: {sys.version.split()[0]}")
    print(f"📁 Directorio: {os.getcwd()}")
    print("=" * 70)

def check_conda_environment():
    """Verifica el entorno Conda"""
    print("\n🔍 Verificando entorno Conda...")
    
    conda_env = os.environ.get('CONDA_DEFAULT_ENV')
    if conda_env:
        print(f"✅ Entorno Conda activo: {conda_env}")
        return True
    else:
        print("⚠️  No se detectó entorno Conda activo")
        
        # Intentar obtener información de conda
        try:
            result = subprocess.run(['conda', 'info', '--envs'], 
                                  capture_output=True, text=True)
            if result.returncode == 0:
                print("📋 Entornos Conda disponibles:")
                print(result.stdout)
        except FileNotFoundError:
            print("❌ Conda no está instalado o no está en el PATH")
        
        return False

def check_project_structure():
    """Verifica la estructura del proyecto"""
    print("\n🔍 Verificando estructura del proyecto...")
    
    required_files = [
        "RubricasApp.Web.csproj",
        "Program.cs", 
        "appsettings.json"
    ]
    
    required_dirs = [
        "Models",
        "Controllers", 
        "Views",
        "Data",
        "Services"
    ]
    
    all_good = True
    
    print("\n📄 Archivos principales:")
    for file in required_files:
        if os.path.exists(file):
            print(f"✅ {file}")
        else:
            print(f"❌ {file} - NO ENCONTRADO")
            all_good = False
    
    print("\n📁 Directorios principales:")
    for dir_name in required_dirs:
        if os.path.exists(dir_name):
            print(f"✅ {dir_name}/")
        else:
            print(f"❌ {dir_name}/ - NO ENCONTRADO")
            all_good = False
    
    return all_good

def check_recent_changes():
    """Verifica cambios recientes"""
    print("\n🔍 Verificando cambios recientes...")
    
    # Verificar que se eliminó la duplicación de TipoPeriodo
    periodo_file = "Models/PeriodoAcademico.cs"
    if os.path.exists(periodo_file):
        with open(periodo_file, 'r', encoding='utf-8') as f:
            content = f.read()
            if 'TipoPeriodo2' in content:
                print("❌ Aún existe TipoPeriodo2 duplicado en PeriodoAcademico.cs")
                return False
            else:
                print("✅ Duplicación de TipoPeriodo eliminada")
    
    # Verificar organización de DOCS
    docs_structure = [
        "DOCS/Scripts/PowerShell",
        "DOCS/Scripts/Batch", 
        "DOCS/Scripts/Python",
        "DOCS/Documentation",
        "DOCS/BuildLogs",
        "DOCS/README.md"
    ]
    
    print("\n📁 Verificando organización de DOCS:")
    for path in docs_structure:
        if os.path.exists(path):
            print(f"✅ {path}")
        else:
            print(f"⚠️  {path} - No encontrado")
    
    return True

def run_dotnet_commands():
    """Ejecuta los comandos de dotnet"""
    print("\n🔨 Iniciando proceso de compilación...")
    
    commands = [
        ("clean", "Limpiando proyecto"),
        ("restore", "Restaurando dependencias"), 
        ("build --configuration Debug --verbosity normal", "Compilando proyecto")
    ]
    
    os.makedirs("DOCS/BuildLogs", exist_ok=True)
    
    for cmd, description in commands:
        print(f"\n🔄 {description}...")
        
        try:
            result = subprocess.run(
                f"dotnet {cmd}".split(),
                capture_output=True,
                text=True,
                cwd=os.getcwd()
            )
            
            # Guardar log
            log_file = f"DOCS/BuildLogs/{cmd.split()[0]}_output_28julio.txt"
            with open(log_file, 'w', encoding='utf-8') as f:
                f.write(f"Comando: dotnet {cmd}\n")
                f.write(f"Fecha: {datetime.now()}\n")
                f.write("=" * 50 + "\n")
                f.write("STDOUT:\n")
                f.write(result.stdout)
                f.write("\n\nSTDERR:\n") 
                f.write(result.stderr)
            
            if result.returncode == 0:
                print(f"✅ {description} completado exitosamente")
                if result.stdout.strip():
                    print("📝 Salida:")
                    print(result.stdout)
            else:
                print(f"❌ Error en {description}")
                print(f"📄 Log guardado en: {log_file}")
                if result.stderr:
                    print("🚨 Errores:")
                    print(result.stderr)
                if result.stdout:
                    print("📝 Salida:")
                    print(result.stdout)
                return False
                
        except FileNotFoundError:
            print("❌ Error: dotnet no está instalado o no está en el PATH")
            return False
        except Exception as e:
            print(f"❌ Error inesperado: {e}")
            return False
    
    return True

def main():
    """Función principal"""
    print_header()
    
    # Cambiar al directorio del proyecto
    project_dir = Path(__file__).parent
    os.chdir(project_dir)
    
    # Verificaciones previas
    conda_ok = check_conda_environment()
    structure_ok = check_project_structure()
    changes_ok = check_recent_changes()
    
    if not structure_ok:
        print("\n❌ La estructura del proyecto tiene problemas")
        return False
    
    if not changes_ok:
        print("\n❌ Hay problemas con los cambios recientes")
        return False
    
    # Ejecutar compilación
    build_ok = run_dotnet_commands()
    
    # Resumen final
    print("\n" + "=" * 70)
    print("📊 RESUMEN FINAL")
    print("=" * 70)
    
    print(f"🐍 Python/Conda: {'✅ Configurado' if conda_ok else '⚠️  No detectado'}")
    print(f"📁 Estructura: {'✅ OK' if structure_ok else '❌ Problemas'}")
    print(f"🔄 Cambios: {'✅ OK' if changes_ok else '❌ Problemas'}")
    print(f"🔨 Compilación: {'✅ EXITOSA' if build_ok else '❌ FALLÓ'}")
    
    if build_ok:
        print("\n🎉 ¡PROYECTO COMPILADO EXITOSAMENTE!")
        print("🚀 Estado: LISTO PARA EJECUTAR")
        print("💡 Ejecuta: dotnet run")
    else:
        print("\n💥 LA COMPILACIÓN FALLÓ")
        print("📋 Revisa los logs en DOCS/BuildLogs/")
        print("🔧 Corrige los errores y vuelve a intentar")
    
    print("=" * 70)
    return build_ok

if __name__ == "__main__":
    success = main()
    sys.exit(0 if success else 1)