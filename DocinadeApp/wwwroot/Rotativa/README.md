# Instalación de wkhtmltopdf para Rotativa

## 📥 Descarga e Instalación

Para que la generación de PDFs funcione correctamente, necesitas instalar **wkhtmltopdf**.

### Opción 1: Descarga Manual (Recomendado para Windows)

1. Descarga wkhtmltopdf desde: https://wkhtmltopdf.org/downloads.html
2. Para Windows 64-bit, usa este enlace directo:
   https://github.com/wkhtmltopdf/packaging/releases/download/0.12.6-1/wkhtmltox-0.12.6-1.msvc2015-win64.exe

3. Ejecuta el instalador
4. Copia los siguientes archivos desde `C:\Program Files\wkhtmltopdf\bin\` a esta carpeta (`wwwroot\Rotativa\`):
   - `wkhtmltopdf.exe`
   - `wkhtmltoimage.exe`
   - Todos los archivos `.dll`

### Opción 2: Instalación Automática (PowerShell)

Ejecuta este comando desde la raíz del proyecto:

```powershell
# Descargar wkhtmltopdf
$url = "https://github.com/wkhtmltopdf/packaging/releases/download/0.12.6-1/wkhtmltox-0.12.6-1.msvc2015-win64.exe"
$output = "$env:TEMP\wkhtmltopdf.exe"
Invoke-WebRequest -Uri $url -OutFile $output

# Instalar
Start-Process -FilePath $output -ArgumentList "/S", "/D=C:\wkhtmltopdf" -Wait

# Copiar archivos a Rotativa
Copy-Item -Path "C:\wkhtmltopdf\bin\*" -Destination ".\wwwroot\Rotativa\" -Force

Write-Host "✅ wkhtmltopdf instalado correctamente!" -ForegroundColor Green
```

### Verificación

Después de la instalación, esta carpeta debe contener:
- ✅ `wkhtmltopdf.exe`
- ✅ `wkhtmltoimage.exe`
- ✅ Archivos `.dll` necesarios

## 🚀 Uso en el Sistema

Una vez instalado, podrás:
- Generar currículums en PDF desde el módulo de Profesores
- Exportar reportes y otros documentos a PDF

## ❓ Troubleshooting

### Error: "wkhtmltopdf.exe no encontrado"
- Verifica que `wkhtmltopdf.exe` esté en la carpeta `wwwroot\Rotativa\`
- Asegúrate de haber copiado también las DLLs necesarias

### El PDF se genera vacío o con errores
- Verifica que la vista Razor esté bien formada
- Asegúrate de que los estilos CSS sean inline o estén embebidos en la vista

### Problemas de permisos
- Ejecuta Visual Studio o la aplicación como Administrador
- Verifica que los archivos en `wwwroot\Rotativa\` tengan permisos de ejecución

## 📚 Documentación

- Rotativa.AspNetCore: https://github.com/webgio/Rotativa.AspNetCore
- wkhtmltopdf: https://wkhtmltopdf.org/
