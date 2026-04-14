#!/usr/bin/env python3
import subprocess
import os
import sys

def main():
    # Cambiar al directorio del proyecto
    os.chdir(r"C:\Users\hguido\Desktop\RubricasApp\src\RubricasApp.Web")
    
    print("🔨 Ejecutando dotnet build...")
    print("📁 Directorio:", os.getcwd())
    print("=" * 60)
    
    try:
        # Ejecutar dotnet build
        result = subprocess.run(
            ["dotnet", "build"],
            capture_output=True,
            text=True,
            timeout=60  # 60 segundos timeout
        )
        
        print(f"📊 Código de retorno: {result.returncode}")
        
        if result.stdout:
            stdout_lines = result.stdout.split('\n')
            print(f"📝 Líneas de salida: {len(stdout_lines)}")
            
            # Mostrar líneas importantes
            important_lines = []
            for line in stdout_lines:
                if any(word in line.lower() for word in ['error', 'warning', 'failed', 'succeeded', 'build']):
                    important_lines.append(line.strip())
            
            if important_lines:
                print("\n🔍 LÍNEAS IMPORTANTES:")
                for line in important_lines[-15:]:  # Últimas 15 líneas importantes
                    if line:
                        print(f"  {line}")
            
            # Mostrar las últimas líneas generales
            print(f"\n📄 ÚLTIMAS LÍNEAS DE SALIDA:")
            for line in stdout_lines[-8:]:
                if line.strip():
                    print(f"  {line}")
        
        if result.stderr:
            print(f"\n❌ ERRORES ({len(result.stderr.split())} líneas):")
            error_lines = result.stderr.split('\n')
            for line in error_lines[:20]:  # Primeros 20 errores
                if line.strip():
                    print(f"  {line}")
        
        # Resultado final
        print("\n" + "=" * 60)
        if result.returncode == 0:
            print("✅ COMPILACIÓN EXITOSA! 🎉")
        else:
            print("❌ COMPILACIÓN FALLÓ")
            print("Revisar los errores mostrados arriba.")
        
        return result.returncode == 0
        
    except subprocess.TimeoutExpired:
        print("⏰ TIMEOUT: El build tardó más de 60 segundos")
        return False
    except Exception as e:
        print(f"💥 ERROR EJECUTANDO BUILD: {e}")
        return False

if __name__ == "__main__":
    success = main()
    sys.exit(0 if success else 1)