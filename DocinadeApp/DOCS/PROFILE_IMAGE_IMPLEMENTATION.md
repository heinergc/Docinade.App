# Implementación de Imagen de Perfil

## 📋 Resumen
Sistema implementado para mostrar imágenes de perfil de usuario en el navbar con fallback a iniciales.

## ✅ Cambios Realizados

### 1. Estilos CSS en _Layout.cshtml
```css
.profile-avatar {
    width: 35px;
    height: 35px;
    border-radius: 50%;
    object-fit: cover;
    border: 2px solid #FF8927;
    margin-right: 8px;
}

.profile-initials {
    width: 35px;
    height: 35px;
    background: linear-gradient(135deg, #FF8927 0%, #ff6b00 100%);
    color: white;
    border-radius: 50%;
    /* ... más estilos */
}
```

### 2. HTML Actualizado
El navbar ahora soporta:
- ✅ Imagen de perfil si existe
- ✅ Iniciales como fallback
- ✅ Animación hover
- ✅ Responsive (oculta nombre en móviles)

## 🔧 Pasos para Implementación Completa

### Paso 1: Agregar Campo a la Base de Datos

Agrega un campo `ProfileImageUrl` a tu tabla de usuarios:

```sql
ALTER TABLE AspNetUsers
ADD ProfileImageUrl NVARCHAR(500) NULL;
```

O en tu modelo de usuario:

```csharp
public class ApplicationUser : IdentityUser
{
    // ... otros campos
    public string? ProfileImageUrl { get; set; }
}
```

### Paso 2: Crear ViewComponent para Profile

Crea `ViewComponents/ProfileAvatarViewComponent.cs`:

```csharp
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RubricasApp.Web.Models;

namespace RubricasApp.Web.ViewComponents
{
    public class ProfileAvatarViewComponent : ViewComponent
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ProfileAvatarViewComponent(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                var user = await _userManager.GetUserAsync(HttpContext.User);
                ViewData["ProfileImageUrl"] = user?.ProfileImageUrl;
            }
            
            return View();
        }
    }
}
```

### Paso 3: Agregar Acción para Subir Imagen

En `AccountController.cs`:

```csharp
[HttpPost]
public async Task<IActionResult> UploadProfileImage(IFormFile profileImage)
{
    if (profileImage == null || profileImage.Length == 0)
    {
        return Json(new { success = false, message = "No se seleccionó ninguna imagen" });
    }

    // Validar tipo de archivo
    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
    var extension = Path.GetExtension(profileImage.FileName).ToLowerInvariant();
    
    if (!allowedExtensions.Contains(extension))
    {
        return Json(new { success = false, message = "Tipo de archivo no permitido" });
    }

    // Validar tamaño (máximo 2MB)
    if (profileImage.Length > 2 * 1024 * 1024)
    {
        return Json(new { success = false, message = "La imagen no debe superar 2MB" });
    }

    try
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Json(new { success = false, message = "Usuario no encontrado" });
        }

        // Crear directorio si no existe
        var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "profiles");
        Directory.CreateDirectory(uploadsFolder);

        // Generar nombre único para el archivo
        var fileName = $"{user.Id}_{Guid.NewGuid()}{extension}";
        var filePath = Path.Combine(uploadsFolder, fileName);

        // Eliminar imagen anterior si existe
        if (!string.IsNullOrEmpty(user.ProfileImageUrl))
        {
            var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, user.ProfileImageUrl.TrimStart('/'));
            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);
            }
        }

        // Guardar nueva imagen
        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await profileImage.CopyToAsync(fileStream);
        }

        // Actualizar usuario
        user.ProfileImageUrl = $"/uploads/profiles/{fileName}";
        var result = await _userManager.UpdateAsync(user);

        if (result.Succeeded)
        {
            return Json(new { 
                success = true, 
                message = "Imagen actualizada correctamente",
                imageUrl = user.ProfileImageUrl
            });
        }

        return Json(new { success = false, message = "Error al actualizar el perfil" });
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error al subir imagen de perfil");
        return Json(new { success = false, message = "Error al procesar la imagen" });
    }
}
```

### Paso 4: Actualizar Vista de Perfil

En `Views/Account/Profile.cshtml`, agregar formulario de carga:

```html
<div class="card mb-4">
    <div class="card-header">
        <h5 class="mb-0">
            <i class="fas fa-camera"></i> Imagen de Perfil
        </h5>
    </div>
    <div class="card-body text-center">
        <div class="mb-3">
            @if (!string.IsNullOrEmpty(Model.ProfileImageUrl))
            {
                <img src="@Model.ProfileImageUrl" alt="Profile" class="rounded-circle" style="width: 150px; height: 150px; object-fit: cover; border: 3px solid #FF8927;" />
            }
            else
            {
                <div class="rounded-circle d-inline-flex align-items-center justify-content-center" 
                     style="width: 150px; height: 150px; background: linear-gradient(135deg, #FF8927 0%, #ff6b00 100%); font-size: 3rem; color: white;">
                    @Model.Initials
                </div>
            }
        </div>
        
        <form id="profileImageForm" enctype="multipart/form-data">
            <div class="mb-3">
                <input type="file" class="form-control" id="profileImageInput" name="profileImage" accept="image/*" />
                <small class="text-muted">Formatos permitidos: JPG, PNG, GIF. Tamaño máximo: 2MB</small>
            </div>
            <button type="submit" class="btn btn-primary">
                <i class="fas fa-upload"></i> Subir Imagen
            </button>
        </form>
    </div>
</div>

@section Scripts {
<script>
    document.getElementById('profileImageForm').addEventListener('submit', async function(e) {
        e.preventDefault();
        
        const formData = new FormData();
        const fileInput = document.getElementById('profileImageInput');
        
        if (!fileInput.files.length) {
            alert('Por favor selecciona una imagen');
            return;
        }
        
        formData.append('profileImage', fileInput.files[0]);
        
        try {
            const response = await fetch('@Url.Action("UploadProfileImage", "Account")', {
                method: 'POST',
                body: formData
            });
            
            const result = await response.json();
            
            if (result.success) {
                alert(result.message);
                location.reload(); // Recargar para ver la nueva imagen
            } else {
                alert(result.message);
            }
        } catch (error) {
            console.error('Error:', error);
            alert('Error al subir la imagen');
        }
    });
</script>
}
```

### Paso 5: Agregar IWebHostEnvironment al Constructor

En `AccountController.cs`:

```csharp
private readonly IWebHostEnvironment _webHostEnvironment;

public AccountController(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    IWebHostEnvironment webHostEnvironment,
    // ... otros parámetros
)
{
    _userManager = userManager;
    _signInManager = signInManager;
    _webHostEnvironment = webHostEnvironment;
    // ... otros
}
```

### Paso 6: Cargar Imagen en BaseController o _Layout

Opción A - En BaseController:

```csharp
public class BaseController : Controller
{
    protected readonly UserManager<ApplicationUser> _userManager;

    public BaseController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    protected async Task LoadProfileImageAsync()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            var user = await _userManager.GetUserAsync(User);
            ViewData["ProfileImageUrl"] = user?.ProfileImageUrl;
        }
    }
}
```

Opción B - Middleware o ViewComponent (recomendado):

El código del ViewComponent ya está en el Paso 2.

## 🎨 Características del Diseño Actual

- ✅ Avatar circular con borde naranja (#FF8927)
- ✅ Gradiente naranja para iniciales
- ✅ Animación hover (escala 1.05)
- ✅ Transiciones suaves
- ✅ Responsive (oculta nombre en pantallas pequeñas)
- ✅ Fallback automático a iniciales

## 📝 Notas Importantes

1. **Seguridad**: Implementar validación de imágenes en el servidor
2. **Optimización**: Considerar redimensionar imágenes al subirlas
3. **Storage**: Para producción, considerar usar Azure Blob Storage o AWS S3
4. **Caché**: La imagen se almacena en `/wwwroot/uploads/profiles/`

## 🚀 Para Usar Ahora Mismo

Puedes probar el sistema actual pasando la URL de la imagen desde cualquier controller:

```csharp
ViewData["ProfileImageUrl"] = "/images/default-avatar.png";
```

O dejar que use las iniciales automáticamente (ya implementado).
