import pandas as pd
import os

# Crear archivo Excel de ejemplo para importar
data = {
    'Criterio': ['Formato del documento', 'Desarrollo del contenido', 'Uso de referencias'],
    'Excelente': [3, 11, 5],
    'Bien': [2, 7, 3],
    'Regular': [1, 3, 1],
    'Deficiente': [0, 0, 0]
}

df = pd.DataFrame(data)
df.to_excel('rubrica_ejemplo.xlsx', index=False)
print("Archivo Excel creado: rubrica_ejemplo.xlsx")
print("Contenido:")
print(df.to_string(index=False))