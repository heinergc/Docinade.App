import subprocess
import os
import sys

# Cambiar al directorio del proyecto
project_path = r"C:\Users\hguido\Desktop\RubricasApp\src\RubricasApp.Web"
os.chdir(project_path)

try:
    # Ejecutar dotnet build para verificar errores de compilación
    result = subprocess.run(['dotnet', 'build', '--verbosity', 'normal'], 
                          capture_output=True, text=True, encoding='utf-8')
    
    print("=== RESULTADO DE COMPILACIÓN ===")
    print(f"Código de salida: {result.returncode}")
    print("\n=== SALIDA ESTÁNDAR ===")
    print(result.stdout)
    
    if result.stderr:
        print("\n=== ERRORES ===")
        print(result.stderr)
    
    # Si hay errores de compilación (código de salida != 0), mostrar resumen
    if result.returncode != 0:
        print("\n=== RESUMEN DE ERRORES ===")
        lines = result.stdout.split('\n')
        error_lines = [line for line in lines if 'error CS' in line or 'Error(' in line]
        warning_lines = [line for line in lines if 'warning CS' in line or 'Warning(' in line]
        
        if error_lines:
            print(f"ERRORES ENCONTRADOS ({len(error_lines)}):")
            for error in error_lines:
                print(f"  - {error.strip()}")
        
        if warning_lines:
            print(f"\nADVERTENCIAS ({len(warning_lines)}):")
            for warning in warning_lines[:5]:  # Solo mostrar las primeras 5
                print(f"  - {warning.strip()}")
    else:
        print("\n✅ COMPILACIÓN EXITOSA - No se encontraron errores")

except Exception as e:
    print(f"Error al ejecutar dotnet build: {e}")
    sys.exit(1)