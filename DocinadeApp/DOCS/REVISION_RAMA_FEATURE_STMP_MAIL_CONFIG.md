# Revisión de Rama: feature-stmp-mail-config vs Remote

## Prompt de revisión

Usa este prompt para repetir el proceso de auditoría y sincronización de la rama con el remote:

---

```
Revisa la rama feature-stmp-mail-config y compara el código local con el remote.
Identifica todos los archivos que existen localmente pero NO están trackeados en git
(es decir, que faltan en origin/feature-stmp-mail-config).

Pasos a seguir:
1. Ejecuta `git ls-files --others --exclude-standard` para listar archivos sin trackear.
2. Compara con `git ls-files` para ver qué archivos .cs faltan en git:
   `comm -23 <(find . -type f -name "*.cs" | grep -v "obj|bin|.git" | sed 's|^\./||' | sort) <(git ls-files "*.cs" | sort)`
3. También revisa otros tipos: .cshtml, .sql, .ps1, .md
4. Si hay archivos ignorados por .gitignore que no deberían estarlo, inspecciona con:
   `git check-ignore -v <archivo>`
5. Revisa el .gitignore por posible corrupción de encoding (UTF-16 mezclado con UTF-8):
   `cat -A .gitignore | tail -30`
   Si ves patrones como `*^@.^@z^@i^@p^@` (bytes nulos entre caracteres), el archivo
   está corrupto y tiene un wildcard `*` activo que bloquea todo.
6. Para corregir el .gitignore corrupto, usar Python para limpiar desde el punto de corrupción:
   ```python
   with open('.gitignore', 'rb') as f:
       content = f.read()
   # Encontrar el byte donde empieza la sección UTF-16
   # Truncar y agregar reglas limpias en UTF-8
   ```
7. Una vez limpio el .gitignore, agregar los archivos faltantes con `git add` y hacer commit/push.

Archivos que NO deben subirse nunca:
- appsettings.Production.json (puede contener passwords/connection strings)
- wwwroot/uploads/ (imágenes subidas por usuarios)
- DB/*.bak, DB/*.dacpac (binarios de base de datos)
- imagenes/, *.lnk (archivos locales)
- docs-site/.astro/, docs-site/test-results/, docs-site/playwright-report/ (generados)
```

---

## Lo que se encontró y resolvió (2026-04-09)

### Problema raíz
El archivo `.gitignore` tenía contenido en **codificación UTF-16 LE** mezclado con UTF-8
en las últimas líneas (a partir del byte 6705). Esto generaba un patrón `*` con bytes nulos
que git interpretaba como un wildcard global, bloqueando absolutamente todos los archivos
no previamente trackeados.

**Síntoma:** `git add <archivo>` devolvía `The following paths are ignored by one of your .gitignore files`
para archivos normales como Controllers, Models, Services.

**Diagnóstico:**
```bash
git check-ignore -v Controllers/InstitucionesController.cs
# Resultado: .gitignore:383:*  Controllers/InstitucionesController.cs
cat -A .gitignore | tail -10
# Resultado: *^@.^@z^@i^@p^@^M^@$  (UTF-16 "*.zip" con wildcard * activo)
```

**Solución aplicada:** Se limpió el .gitignore con Python truncando en el byte 6705
y agregando las reglas limpias en UTF-8 puro.

---

### Archivos agregados — Commit 1 (36 archivos)

| Categoría | Archivos |
|-----------|----------|
| Controllers | InstitucionesController.cs, SEAController.cs, SliderController.cs |
| Models | ErrorLog.cs, EstudianteInstrumentoACS.cs, ExcepcionSistema.cs, SliderItem.cs, SEA/ComponentesSEA.cs, SEA/ConfiguracionComponenteSEA.cs |
| Services | IErrorLogService.cs, ErrorLogService.cs, ExcepcionService.cs, ISliderService.cs, SliderService.cs, AdecuacionCurricular/IACSService.cs, AdecuacionCurricular/ACSService.cs, SEA/ISEAService.cs, SEA/SEAService.cs |
| ViewModels | ACSViewModels.cs, InstitucionViewModels.cs, SEA/SEAViewModels.cs, Admin/ResetPasswordViewModel.cs |
| ViewComponents | SliderViewComponent.cs |
| Utils | AdminSeeder.cs, ResetAdminPassword.cs |
| Migrations | 5 migraciones Feb-Mar 2026 (Instituciones, Grupos, ACS, SEA) |
| .gitignore | Reparado encoding UTF-16 corrupto |

### Archivos agregados — Commit 2 (84 archivos)

| Categoría | Archivos |
|-----------|----------|
| Views | Instituciones (CRUD), SEA (Index/Configurar), Slider (CRUD), Estudiantes/ConfigurarACS, Shared/Slider, Areas Admin (Email, ResetPassword) |
| SQL | Scripts de admin, diagnóstico y estructura de DB |
| PowerShell | Scripts de deploy Azure, IIS, producción |
| Documentación | DOCS/, Documentation/, configs de deploy/render/environment |
| docs-site | Proyecto Astro completo (src, content, config) |
| Otros | .vscode/, .github/instructions/ |

### Archivos excluidos y protegidos en .gitignore

```gitignore
# Sensitive configuration
appsettings.Production.json

# User uploaded files
wwwroot/uploads/

# Database binaries
DB/*.bak
DB/*.dacpac

# Local files
imagenes/
*.lnk

# Generated
docs-site/.astro/
docs-site/test-results/
docs-site/playwright-report/
```

> **Nota importante:** `appsettings.Production.json` contiene credenciales SMTP de Gmail.
> Usar variables de entorno o Azure Key Vault en producción en lugar de este archivo.
