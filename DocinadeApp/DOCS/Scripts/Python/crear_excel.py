import pandas as pd

data = {
    'Criterio': [
        'Contenido y desarrollo del tema',
        'Organizacion y estructura', 
        'Uso correcto del lenguaje',
        'Presentacion y formato'
    ],
    'Excelente': [4, 3, 2, 1],
    'Bueno': [3, 2, 1.5, 0.75],
    'Regular': [2, 1, 1, 0.5],
    'Deficiente': [1, 0, 0.5, 0.25]
}

df = pd.DataFrame(data)
df.to_excel('rubrica_real.xlsx', index=False)
print('Archivo Excel real creado exitosamente')
print(df)