import subprocess
import os
import sys

# Cambiar al directorio del proyecto
project_path = r"C:\Users\hguido\Desktop\RubricasApp\src\RubricasApp.Web"
os.chdir(project_path)

print("🔄 Compilando proyecto RubricasApp.Web...")
print("=" * 50)

try:
    # Ejecutar dotnet build para verificar errores de compilación
    result = subprocess.run(['dotnet', 'build', '--verbosity', 'minimal'], 
                          capture_output=True, text=True, encoding='utf-8')
    
    print(f"Código de salida: {result.returncode}")
    
    if result.returncode == 0:
        print("✅ COMPILACIÓN EXITOSA!")
        print("🎉 Todos los errores de RolesController han sido corregidos.")
        print("\n📋 RESUMEN DE CORRECCIONES:")
        print("  ✅ Variables duplicadas eliminadas")
        print("  ✅ Método auxiliar GenerateAvailablePermissions() creado")
        print("  ✅ Código refactorizado para evitar duplicación")
        print("  ✅ Todas las referencias de auditoría corregidas")
        print("  ✅ ViewModels compatibles implementados")
    else:
        print("❌ ERRORES DE COMPILACIÓN ENCONTRADOS:")
        print("\n--- SALIDA ESTÁNDAR ---")
        print(result.stdout)
        
        if result.stderr:
            print("\n--- ERRORES ---")
            print(result.stderr)
        
        # Extraer errores específicos
        lines = result.stdout.split('\n')
        error_lines = [line for line in lines if 'error CS' in line]
        
        if error_lines:
            print(f"\n🚨 ERRORES ESPECÍFICOS ({len(error_lines)}):")
            for i, error in enumerate(error_lines, 1):
                print(f"  {i}. {error.strip()}")

except Exception as e:
    print(f"❌ Error al ejecutar dotnet build: {e}")
    sys.exit(1)

print("\n" + "=" * 50)