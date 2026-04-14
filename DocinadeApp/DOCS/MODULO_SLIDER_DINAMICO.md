# Módulo Slider Dinámico - RubricasApp.Web

## Descripción General

Sistema completo de gestión de sliders dinámicos para la **página principal pública (pre-login)**, con administración CRUD completa, soporte para hipervínculos personalizados, y carrusel Bootstrap 5 responsive con autoplay.

**📌 IMPORTANTE**: El slider **solo se muestra en la página de inicio cuando el usuario NO está autenticado** (http://localhost:5000/). Una vez que el usuario inicia sesión, el slider se oculta automáticamente.

---

## Características Principales

### Administración (Backend)
- ✅ **CRUD Completo**: Crear, Leer, Actualizar, Eliminar sliders
- ✅ **Gestión de Imágenes**: Upload con validación (5MB max, formatos: JPG, PNG, GIF, WebP)
- ✅ **Hipervínculos Personalizados**: URL y texto de botón administrado por el usuario
- ✅ **Activación/Desactivación**: Toggle AJAX sin recargar página
- ✅ **Orden Personalizable**: Controlar secuencia de visualización (1-999)
- ✅ **DataTables**: Búsqueda, ordenamiento y paginación
- ✅ **Auditoría**: Registro de usuario creador y modificador con timestamps
- ✅ **Vista Previa**: Preview de imagen antes de guardar

### Frontend Público
- ✅ **Carrusel Bootstrap 5**: Diseño responsive y moderno
- ✅ **Autoplay**: Transición automática cada 5 segundos
- ✅ **Animaciones**: Efectos Animate.css en títulos y subtítulos
- ✅ **Controles**: Navegación con flechas e indicadores
- ✅ **Lazy Loading**: Carga optimizada de imágenes
- ✅ **Botones CTA**: Call-to-action opcional con enlaces externos

---

## Estructura de Archivos

### Modelo
```
Models/SliderItem.cs
- Entidad principal con validaciones Data Annotations
- Propiedades: Id, Titulo, Subtitulo, EnlaceUrl, TextoBoton, ImagenUrl, Orden, Activo
- Navigation properties a ApplicationUser (auditoría)
```

### Capa de Servicio
```
Services/ISliderService.cs
- Contrato de interfaz con 9 métodos

Services/SliderService.cs
- Implementación completa de lógica de negocio
- Métodos principales:
  * GetAllAsync() - Obtener todos los sliders (admin)
  * GetActiveAsync() - Solo sliders activos (frontend)
  * GetByIdAsync(id) - Obtener por ID
  * CreateAsync(slider, imageFile, userId) - Crear con upload
  * UpdateAsync(slider, imageFile, userId) - Actualizar con opción de cambiar imagen
  * DeleteAsync(id) - Eliminar con cascada a archivo físico
  * ToggleActiveAsync(id) - Cambiar estado activo/inactivo
  * SaveImageAsync(imageFile) - Guardar imagen con validación
  * DeleteImageAsync(imageUrl) - Eliminar archivo físico
```

### Controlador
```
Controllers/SliderController.cs
- Atributo [Authorize] - Requiere autenticación
- Acciones:
  * Index (GET) - Listar todos
  * Create (GET/POST) - Formulario de creación con upload
  * Edit (GET/POST) - Formulario de edición con preview
  * Delete (GET/POST) - Confirmación de eliminación
  * ToggleActive (POST/AJAX) - Cambio de estado sin reload
```

### Vistas (Razor)
```
Views/Slider/Index.cshtml
- DataTable con búsqueda y ordenamiento
- Botones toggle AJAX para activar/desactivar
- Thumbnails de imágenes
- Enlaces a Edit y Delete

Views/Slider/Create.cshtml
- Form con enctype multipart/form-data
- Vista previa de imagen en tiempo real
- Validación client-side y server-side
- Campos: Titulo, Subtitulo, EnlaceUrl, TextoBoton, Orden, Activo

Views/Slider/Edit.cshtml
- Muestra imagen actual
- Permite cambiar imagen opcionalmente
- Pre-carga valores existentes
- Vista previa de nueva imagen

Views/Slider/Delete.cshtml
- Confirmación con preview de datos
- Checkbox de confirmación requerido
- Doble confirmación JS
- Muestra imagen a eliminar
```

### ViewComponent
```
ViewComponents/SliderViewComponent.cs
- Componente reutilizable para frontend
- Obtiene sliders activos ordenados
- Manejo de errores silencioso (no rompe la página si falla)

Views/Shared/Components/Slider/Default.cshtml
- Carrusel Bootstrap 5 con fade effect
- Indicadores tipo dots
- Caption con gradiente oscuro
- Botones CTA con icono Font Awesome
- Estilos CSS inline responsive
- Soporte para Animate.css
```

### Base de Datos
```
DB/CREATE_TABLE_SliderItems.sql
- Script de creación completa de tabla
- Constraints: PK, FK, Defaults
- Índices en Orden y Activo
- Columnas:
  * Id (INT IDENTITY PK)
  * Titulo (NVARCHAR(200) NOT NULL)
  * Subtitulo (NVARCHAR(500) NULL)
  * EnlaceUrl (NVARCHAR(500) NULL)
  * TextoBoton (NVARCHAR(100) NULL)
  * ImagenUrl (NVARCHAR(500) NOT NULL)
  * Orden (INT DEFAULT 1)
  * Activo (BIT DEFAULT 1)
  * FechaCreacion, FechaModificacion (DATETIME2)
  * UsuarioCreacionId, UsuarioModificacionId (FK AspNetUsers)

DB/ALTER_TABLE_SliderItems_Add_Hyperlinks.sql
- Script de actualización para agregar EnlaceUrl y TextoBoton
- Verificación de existencia previa (idempotente)
```

### Configuración
```
Data/RubricasDbContext.cs
- DbSet<SliderItem> SliderItems
- Fluent API configuration:
  * MaxLength constraints
  * Default values
  * Foreign keys con DeleteBehavior.Restrict
  * Indexes

Program.cs
- Registro de servicio: builder.Services.AddScoped<ISliderService, SliderService>();
```

---

## Ubicación de Archivos Físicos

### Imágenes de Sliders
```
wwwroot/uploads/sliders/
```

**Convención de nombres**: `{Guid}.{extension}`

**Ejemplo**: `a3f8c9d4-1234-5678-90ab-cdef12345678.jpg`

---

## Validaciones

### Imagen (Upload)
- **Tamaño máximo**: 5 MB (5,242,880 bytes)
- **Formatos permitidos**: .jpg, .jpeg, .png, .gif, .webp
- **Validación**: Client-side (JavaScript) y Server-side (C#)

### Campos del Modelo
- **Titulo**: Requerido, máximo 200 caracteres
- **Subtitulo**: Opcional, máximo 500 caracteres
- **EnlaceUrl**: Opcional, máximo 500 caracteres, debe ser URL válida ([Url] attribute)
- **TextoBoton**: Opcional, máximo 100 caracteres
- **Orden**: Requerido, rango 1-999
- **Activo**: Boolean, default true

---

## Permisos y Seguridad

### Autorización
- **Controlador**: `[Authorize]` - Requiere usuario autenticado
- **Acceso a menú**: SuperAdministrador o Administrador (ApplicationRoles)
- **Sugerencia futura**: Implementar permisos granulares:
  - `Ver.Slider`
  - `Crear.Slider`
  - `Editar.Slider`
  - `Eliminar.Slider`

### Auditoría
- **UsuarioCreacionId**: ID del usuario que creó el slider
- **FechaCreacion**: Timestamp de creación
- **UsuarioModificacionId**: ID del usuario que modificó por última vez
- **FechaModificacion**: Timestamp de última modificación

---

## Integración en la Aplicación

### Navegación (Menú)
```cshtml
<!-- Views/Shared/_Layout.cshtml -->
<li><h6 class="dropdown-header">Administración</h6></li>
<li><a class="dropdown-item" asp-controller="Slider" asp-action="Index">
    <i class="fas fa-images"></i> Gestión de Slider
</a></li>
```

### Página Principal (Solo usuarios NO autenticados)
```cshtml
<!-- Views/Home/Index.cshtml -->
<!-- El slider SOLO se muestra en la página pública (antes del login) -->
@if (!User.Identity?.IsAuthenticated ?? true)
{
    <vc:slider></vc:slider>
}
```

**Nota importante**: El slider está configurado para aparecer **únicamente en la página inicial pública** (http://localhost:5000/) cuando el usuario **NO está autenticado**. Una vez que el usuario inicia sesión, el slider se oculta automáticamente.

---

## Uso del ViewComponent

### En cualquier vista Razor
```cshtml
<vc:slider></vc:slider>
```

### Programáticamente en un controlador
```csharp
return ViewComponent("Slider");
```

---

## Flujo de Trabajo Típico

### 1. Crear Nuevo Slider
1. Ir a **Configuración > Gestión de Slider**
2. Click en **"Nuevo Slider"**
3. Completar formulario:
   - Título (requerido)
   - Subtítulo (opcional)
   - Enlace URL (opcional, ej: https://mep.go.cr)
   - Texto del botón (opcional, ej: "Más información")
   - Seleccionar imagen (requerido, max 5MB)
   - Orden de visualización (1-999, menor = primero)
   - Marcar "Activo" si desea mostrar inmediatamente
4. Click en **"Guardar Slider"**
5. El slider aparecerá en el carrusel si está activo

### 2. Editar Slider Existente
1. En la lista, click en botón **"Editar"** (icono lápiz)
2. Modificar campos deseados
3. Opcionalmente cambiar imagen (si no selecciona, mantiene la actual)
4. Click en **"Guardar Cambios"**

### 3. Activar/Desactivar Rápidamente
1. En la lista, click en botón de estado (Activo/Inactivo)
2. Confirmar acción en diálogo
3. El estado cambia sin recargar la página (AJAX)

### 4. Cambiar Orden de Visualización
1. Editar slider
2. Cambiar valor del campo "Orden"
3. Los sliders se muestran en orden ascendente (1, 2, 3...)

### 5. Eliminar Slider
1. En la lista, click en botón **"Eliminar"** (icono basura)
2. Revisar información del slider
3. Marcar checkbox de confirmación
4. Click en **"Confirmar Eliminación"**
5. Confirmar en diálogo adicional
6. El slider y su imagen se eliminan permanentemente

---

## Comportamiento del Carrusel

### Características
- **Transición**: Fade effect (desvanecimiento)
- **Intervalo**: 5 segundos entre slides
- **Altura**: 500px desktop, 300px tablet, 250px móvil
- **Controles**: Flechas prev/next y dots indicadores
- **Lazy Loading**: Primera imagen eager, resto lazy
- **Responsive**: Se adapta a pantallas pequeñas

### Lógica de Visualización
- Solo muestra sliders con `Activo = true`
- Ordenados por campo `Orden` ascendente
- Si no hay sliders activos, no renderiza nada (no rompe el layout)
- Errores se registran en log pero no se muestran al usuario

---

## Registro de Actividad (Logs)

### Prefijos de Log
- `[INFO]` - Operaciones normales
- `[SUCCESS]` - Operaciones exitosas
- `[WARNING]` - Advertencias (ej: imagen no encontrada al eliminar)
- `[ERROR]` - Errores que requieren atención

### Ejemplos de Logs
```
[INFO] Configurado para SQL Server: Server=localhost\SQLEXPRESS;...
[SUCCESS] Slider creado exitosamente. ID: 5
[WARNING] Archivo de imagen no encontrado al eliminar: /uploads/sliders/abc.jpg
[ERROR] Error al cambiar estado del slider ID 3
```

---

## Dependencias

### NuGet Packages
- `Microsoft.AspNetCore.Identity.EntityFrameworkCore` (v8.0.x)
- `Microsoft.EntityFrameworkCore.SqlServer` (v8.0.x)

### Frontend Libraries (CDN)
- **Bootstrap 5**: Carrusel y componentes UI
- **Font Awesome 6**: Iconos
- **DataTables 1.13.6**: Tabla con búsqueda/ordenamiento
- **Animate.css 4.1.1**: Animaciones de texto
- **jQuery**: Manipulación DOM y AJAX

---

## Troubleshooting

### Problema: Imagen no se sube
**Solución**:
- Verificar tamaño < 5MB
- Verificar formato válido (JPG/PNG/GIF/WebP)
- Verificar permisos de escritura en `wwwroot/uploads/sliders/`

### Problema: Slider no aparece en página principal
**Solución**:
- Verificar que `Activo = true`
- Verificar que existe al menos un slider activo
- Verificar que ViewComponent está llamado en la vista: `<vc:slider></vc:slider>`
- Revisar logs del servidor

### Problema: Error al eliminar slider
**Solución**:
- Verificar que no tenga referencias FK (aunque no debería en este caso)
- Revisar permisos del usuario en el sistema de archivos
- Si la imagen ya no existe, el error se registra pero no falla la eliminación

### Problema: DataTable no funciona
**Solución**:
- Verificar que jQuery esté cargado antes de DataTables
- Verificar URL de CDN de DataTables
- Abrir consola del navegador para ver errores JS

---

## Mejoras Futuras Sugeridas

### Funcionalidad
- [ ] Sistema de permisos granulares (`Ver.Slider`, `Crear.Slider`, etc.)
- [ ] Drag & drop para cambiar orden visualmente
- [ ] Múltiples carruseles (Home, Institucional, etc.)
- [ ] Programación de fechas de inicio/fin de visualización
- [ ] Estadísticas de clics en botones CTA
- [ ] Soporte para videos (YouTube/Vimeo embeds)
- [ ] Slider responsive por dispositivo (desktop/mobile)

### Performance
- [ ] Compresión automática de imágenes al subir
- [ ] Generación de thumbnails para lista admin
- [ ] Caché de sliders activos (Redis/MemoryCache)
- [ ] CDN para imágenes

### UX/UI
- [ ] Editor WYSIWYG para subtítulos
- [ ] Crop/resize de imágenes en el navegador
- [ ] Preview en tiempo real del carrusel
- [ ] Biblioteca de imágenes reutilizables

---

## Testing

### Casos de Prueba Manuales

#### Crear Slider
1. Login como admin
2. Ir a Gestión de Slider
3. Click "Nuevo Slider"
4. Completar título "Test Slider"
5. Subir imagen válida < 5MB
6. Guardar
7. **Esperado**: Mensaje de éxito, redirige a Index

#### Editar Slider
1. Click "Editar" en un slider existente
2. Cambiar título a "Test Modificado"
3. No cambiar imagen
4. Guardar
5. **Esperado**: Cambios guardados, imagen intacta

#### Toggle Activo/Inactivo
1. Click en botón "Activo" de un slider
2. Confirmar diálogo
3. **Esperado**: Botón cambia a "Inactivo" sin reload, slider desaparece del carrusel

#### Eliminar Slider
1. Click "Eliminar"
2. Marcar checkbox de confirmación
3. Click "Confirmar Eliminación"
4. Confirmar en diálogo JS
5. **Esperado**: Slider eliminado, archivo físico eliminado

#### Visualización Frontend
1. Crear 3 sliders activos con orden 1, 2, 3
2. Ir a Home (página principal)
3. **Esperado**: Carrusel muestra los 3 sliders en orden, autoplay cada 5 seg

---

## Contacto y Soporte

Para consultas sobre este módulo:
- **Documentación**: Este archivo
- **Código fuente**: `d:\Fuentes_gitHub\RubricasApp.Web\`
- **Base de datos**: RubricasDb (localhost\SQLEXPRESS)

---

## Historial de Cambios

### Versión 1.0.0 (2025-01-28)
- [x] Implementación inicial del módulo completo
- [x] CRUD con Entity Framework Core
- [x] Upload de imágenes con validación
- [x] Carrusel Bootstrap 5 responsive
- [x] Soporte para hipervínculos personalizados (EnlaceUrl, TextoBoton)
- [x] Toggle AJAX de activación
- [x] DataTables en lista admin
- [x] ViewComponent para reutilización
- [x] Auditoría de usuario y timestamps
- [x] Documentación completa

---

**Desarrollado para**: RubricasApp.Web - Sistema de Gestión de Rúbricas Educativas MEP Costa Rica
**Framework**: ASP.NET Core 8.0 MVC
**Fecha**: Enero 2025
