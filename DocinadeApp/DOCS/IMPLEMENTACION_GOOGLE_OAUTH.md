# 📚 Guía de Implementación: Autenticación con Google OAuth 2.0 en ASP.NET Core

**Fecha**: 16 de octubre de 2025  
**Proyecto**: RubricasApp.Web  
**Rama**: feature-asistencia  

---

## 🎯 Resumen Ejecutivo

Permitir que los usuarios se autentiquen usando sus cuentas de Google en tu aplicación RubricasApp.Web requiere implementar **OAuth 2.0** usando el proveedor de autenticación externa de Google.

---

## 📋 Requisitos Previos

### 1. **Configuración en Google Cloud Console**

Necesitarás crear un proyecto en Google Cloud y configurar OAuth 2.0:

1. **Ir a**: https://console.cloud.google.com/
2. **Crear un nuevo proyecto** o seleccionar uno existente
3. **Habilitar Google+ API**:
   - Ir a "APIs & Services" > "Library"
   - Buscar "Google+ API" y habilitarla
4. **Crear credenciales OAuth 2.0**:
   - Ir a "APIs & Services" > "Credentials"
   - Crear "OAuth 2.0 Client ID"
   - Tipo: "Web application"
   - **Authorized redirect URIs**:
     - `https://localhost:18163/signin-google` (desarrollo)
     - `https://tudominio.com/signin-google` (producción)
5. **Obtener**:
   - **Client ID**: `xxxxx.apps.googleusercontent.com`
   - **Client Secret**: `GOCSPX-xxxxx`

---

## 🏗️ Arquitectura de la Implementación

```
┌─────────────────────────────────────────────────────────┐
│                    FLUJO DE AUTENTICACIÓN                │
└─────────────────────────────────────────────────────────┘

Usuario                  App Web              Google OAuth         Base Datos
  │                        │                       │                   │
  │  Click "Login Google"  │                       │                   │
  ├───────────────────────>│                       │                   │
  │                        │  Redirect a Google    │                   │
  │                        ├──────────────────────>│                   │
  │                        │                       │                   │
  │  Ingresa credenciales  │                       │                   │
  │<──────────────────────────────────────────────┤                   │
  │                        │                       │                   │
  │  Aprueba permisos      │                       │                   │
  ├───────────────────────────────────────────────>│                   │
  │                        │                       │                   │
  │                        │  Callback con token   │                   │
  │                        │<──────────────────────┤                   │
  │                        │                       │                   │
  │                        │  Validar token        │                   │
  │                        ├──────────────────────>│                   │
  │                        │                       │                   │
  │                        │  Datos del usuario    │                   │
  │                        │<──────────────────────┤                   │
  │                        │                       │                   │
  │                        │  ¿Usuario existe?     │                   │
  │                        ├───────────────────────────────────────────>│
  │                        │                       │                   │
  │                        │  Crear/Actualizar     │                   │
  │                        │<───────────────────────────────────────────┤
  │                        │                       │                   │
  │  Login exitoso         │                       │                   │
  │<───────────────────────┤                       │                   │
```

---

## 📦 Paquetes NuGet Requeridos

```xml
<!-- En RubricasApp.Web.csproj -->
<PackageReference Include="Microsoft.AspNetCore.Authentication.Google" Version="8.0.0" />
```

**Instalación**:
```bash
dotnet add package Microsoft.AspNetCore.Authentication.Google
```

---

## ⚙️ Configuración en `appsettings.json`

```json
{
  "Authentication": {
    "Google": {
      "ClientId": "TU_CLIENT_ID.apps.googleusercontent.com",
      "ClientSecret": "TU_CLIENT_SECRET"
    }
  }
}
```

**Para desarrollo, usar `appsettings.Development.json`**:
```json
{
  "Authentication": {
    "Google": {
      "ClientId": "dev-client-id.apps.googleusercontent.com",
      "ClientSecret": "GOCSPX-dev-secret"
    }
  }
}
```

> ⚠️ **IMPORTANTE**: Nunca subir el `ClientSecret` real a GitHub. Usar variables de entorno en producción.

---

## 🔧 Configuración en `Program.cs`

### Ubicación en tu archivo actual

Buscar donde está configurado `AddIdentity` (aproximadamente línea 40-50) y agregar después:

```csharp
// Configuración de Identity (YA EXISTE)
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // ... configuración existente
})
.AddEntityFrameworkStores<RubricasDbContext>()
.AddDefaultTokenProviders();

// ========== AGREGAR ESTA SECCIÓN ==========
// Autenticación con Google OAuth
builder.Services.AddAuthentication()
    .AddGoogle(googleOptions =>
    {
        googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"];
        googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
        
        // Callback path (debe coincidir con Google Console)
        googleOptions.CallbackPath = "/signin-google";
        
        // Scopes adicionales (opcional)
        googleOptions.Scope.Add("profile");
        googleOptions.Scope.Add("email");
        
        // Guardar tokens (opcional, si necesitas acceder a API de Google)
        googleOptions.SaveTokens = true;
        
        // Eventos para personalizar el comportamiento
        googleOptions.Events.OnCreatingTicket = context =>
        {
            // Aquí puedes capturar información adicional del perfil
            var email = context.Principal.FindFirstValue(ClaimTypes.Email);
            var name = context.Principal.FindFirstValue(ClaimTypes.Name);
            var googleId = context.Principal.FindFirstValue(ClaimTypes.NameIdentifier);
            
            // Logs para debugging
            var logger = context.HttpContext.RequestServices
                .GetRequiredService<ILogger<Program>>();
            logger.LogInformation($"Usuario Google: {name} ({email}) - ID: {googleId}");
            
            return Task.CompletedTask;
        };
    });
```

---

## 👤 Extender el Modelo de Usuario

### Modificar `Models/Identity/ApplicationUser.cs`

```csharp
public class ApplicationUser : IdentityUser
{
    // Propiedades existentes...
    public string? Nombre { get; set; }
    public string? Apellido { get; set; }
    
    // ========== AGREGAR ESTAS PROPIEDADES ==========
    
    /// <summary>
    /// ID único del proveedor externo (Google, Facebook, etc.)
    /// </summary>
    public string? ExternalProviderId { get; set; }
    
    /// <summary>
    /// Nombre del proveedor: "Google", "Facebook", "Microsoft"
    /// </summary>
    public string? ExternalProvider { get; set; }
    
    /// <summary>
    /// URL de la foto de perfil del proveedor externo
    /// </summary>
    public string? ExternalProfilePictureUrl { get; set; }
    
    /// <summary>
    /// Indica si el usuario se registró mediante proveedor externo
    /// </summary>
    public bool IsExternalAuth { get; set; }
    
    /// <summary>
    /// Fecha del primer login con proveedor externo
    /// </summary>
    public DateTime? ExternalAuthDate { get; set; }
}
```

**Generar migración**:
```bash
dotnet ef migrations add AddGoogleAuthFields
dotnet ef database update
```

---

## 🎮 Controlador: `AccountController.cs`

### Métodos necesarios a agregar/modificar:

```csharp
public class AccountController : Controller
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<AccountController> _logger;
    
    // ... constructor existente

    // ========== MÉTODO 1: INICIAR LOGIN CON GOOGLE ==========
    [HttpGet]
    [AllowAnonymous]
    public IActionResult ExternalLogin(string provider, string? returnUrl = null)
    {
        // Validar proveedor
        if (string.IsNullOrEmpty(provider))
        {
            return BadRequest("Proveedor de autenticación no especificado");
        }

        // URL de callback después de autenticar en Google
        var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account", 
            new { returnUrl });
        
        // Configurar propiedades de autenticación
        var properties = _signInManager.ConfigureExternalAuthenticationProperties(
            provider, redirectUrl);
        
        // Redirigir a Google OAuth
        return new ChallengeResult(provider, properties);
    }

    // ========== MÉTODO 2: CALLBACK DESPUÉS DE GOOGLE ==========
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> ExternalLoginCallback(
        string? returnUrl = null, 
        string? remoteError = null)
    {
        returnUrl ??= Url.Content("~/");

        // Verificar errores de Google
        if (remoteError != null)
        {
            _logger.LogWarning($"Error de proveedor externo: {remoteError}");
            TempData["ErrorMessage"] = $"Error de autenticación: {remoteError}";
            return RedirectToAction(nameof(Login));
        }

        // Obtener información del login externo
        var info = await _signInManager.GetExternalLoginInfoAsync();
        if (info == null)
        {
            _logger.LogWarning("No se pudo obtener información de login externo");
            TempData["ErrorMessage"] = "No se pudo obtener información de Google";
            return RedirectToAction(nameof(Login));
        }

        // Intentar login con el proveedor externo
        var result = await _signInManager.ExternalLoginSignInAsync(
            info.LoginProvider, 
            info.ProviderKey, 
            isPersistent: false, 
            bypassTwoFactor: true);

        if (result.Succeeded)
        {
            // Usuario ya existe y se autenticó correctamente
            _logger.LogInformation($"Usuario logueado con {info.LoginProvider}");
            
            // Actualizar último acceso
            var user = await _userManager.FindByLoginAsync(
                info.LoginProvider, 
                info.ProviderKey);
            
            if (user != null)
            {
                user.LastLoginDate = DateTime.Now;
                await _userManager.UpdateAsync(user);
            }
            
            return LocalRedirect(returnUrl);
        }

        if (result.IsLockedOut)
        {
            return RedirectToAction(nameof(Lockout));
        }
        else
        {
            // Usuario no existe - mostrar formulario de registro
            ViewData["ReturnUrl"] = returnUrl;
            ViewData["Provider"] = info.LoginProvider;
            
            // Extraer información de Google
            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            var firstName = info.Principal.FindFirstValue(ClaimTypes.GivenName);
            var lastName = info.Principal.FindFirstValue(ClaimTypes.Surname);
            var googleId = info.Principal.FindFirstValue(ClaimTypes.NameIdentifier);
            var profilePicture = info.Principal.FindFirstValue("picture");
            
            var model = new ExternalLoginConfirmationViewModel
            {
                Email = email,
                Nombre = firstName,
                Apellido = lastName,
                Provider = info.LoginProvider,
                ProviderId = googleId,
                ProfilePictureUrl = profilePicture
            };

            return View("ExternalLoginConfirmation", model);
        }
    }

    // ========== MÉTODO 3: CONFIRMAR REGISTRO CON GOOGLE ==========
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ExternalLoginConfirmation(
        ExternalLoginConfirmationViewModel model, 
        string? returnUrl = null)
    {
        returnUrl ??= Url.Content("~/");

        // Obtener información del login externo nuevamente
        var info = await _signInManager.GetExternalLoginInfoAsync();
        if (info == null)
        {
            TempData["ErrorMessage"] = "Error al cargar información de login externo";
            return RedirectToAction(nameof(Login));
        }

        if (ModelState.IsValid)
        {
            // Verificar si el email ya existe
            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            
            if (existingUser != null)
            {
                // Email ya registrado - vincular cuenta
                var addLoginResult = await _userManager.AddLoginAsync(
                    existingUser, info);
                
                if (addLoginResult.Succeeded)
                {
                    // Actualizar información del proveedor externo
                    existingUser.ExternalProvider = model.Provider;
                    existingUser.ExternalProviderId = model.ProviderId;
                    existingUser.ExternalProfilePictureUrl = model.ProfilePictureUrl;
                    existingUser.IsExternalAuth = true;
                    existingUser.ExternalAuthDate = DateTime.Now;
                    
                    await _userManager.UpdateAsync(existingUser);
                    await _signInManager.SignInAsync(existingUser, isPersistent: false);
                    
                    _logger.LogInformation($"Cuenta vinculada: {model.Email}");
                    return LocalRedirect(returnUrl);
                }
                else
                {
                    foreach (var error in addLoginResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(model);
                }
            }
            else
            {
                // Crear nuevo usuario
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    Nombre = model.Nombre,
                    Apellido = model.Apellido,
                    EmailConfirmed = true, // Google ya verificó el email
                    IsActive = true,
                    CreatedDate = DateTime.Now,
                    ExternalProvider = model.Provider,
                    ExternalProviderId = model.ProviderId,
                    ExternalProfilePictureUrl = model.ProfilePictureUrl,
                    IsExternalAuth = true,
                    ExternalAuthDate = DateTime.Now
                };

                var createResult = await _userManager.CreateAsync(user);
                
                if (createResult.Succeeded)
                {
                    // Vincular login externo
                    var addLoginResult = await _userManager.AddLoginAsync(user, info);
                    
                    if (addLoginResult.Succeeded)
                    {
                        // Asignar rol por defecto (ajustar según tu lógica)
                        var defaultRole = "Docente"; // o el rol que corresponda
                        await _userManager.AddToRoleAsync(user, defaultRole);
                        
                        _logger.LogInformation($"Usuario creado con {model.Provider}: {model.Email}");
                        
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                
                foreach (var error in createResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
        }

        ViewData["ReturnUrl"] = returnUrl;
        return View(model);
    }
}
```

---

## 📝 ViewModels Necesarios

### Crear: `ViewModels/Account/ExternalLoginConfirmationViewModel.cs`

```csharp
namespace RubricasApp.Web.ViewModels.Account
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required(ErrorMessage = "El correo electrónico es obligatorio")]
        [EmailAddress(ErrorMessage = "Formato de correo inválido")]
        [Display(Name = "Correo electrónico")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido es obligatorio")]
        [StringLength(100, ErrorMessage = "El apellido no puede exceder 100 caracteres")]
        [Display(Name = "Apellido")]
        public string Apellido { get; set; } = string.Empty;

        [Display(Name = "Aceptar términos y condiciones")]
        public bool AceptaTerminos { get; set; }

        // Datos del proveedor (no editables)
        public string Provider { get; set; } = string.Empty;
        public string ProviderId { get; set; } = string.Empty;
        public string? ProfilePictureUrl { get; set; }
    }
}
```

---

## 🎨 Vistas necesarias

### 1. **Modificar `Views/Account/Login.cshtml`**

Agregar botón de Google:

```html
<div class="card-body">
    <!-- Formulario de login tradicional existente -->
    <form asp-action="Login" asp-route-returnurl="@ViewData["ReturnUrl"]" method="post">
        <!-- ... campos existentes ... -->
    </form>

    <hr class="my-4">

    <!-- ========== SECCIÓN NUEVA: LOGIN EXTERNO ========== -->
    <div class="text-center">
        <p class="text-muted mb-3">O inicia sesión con:</p>
        
        <form asp-action="ExternalLogin" asp-route-returnurl="@ViewData["ReturnUrl"]" method="post">
            <button type="submit" 
                    name="provider" 
                    value="Google" 
                    class="btn btn-outline-danger btn-lg w-100"
                    title="Iniciar sesión con Google">
                <i class="fab fa-google me-2"></i>
                Continuar con Google
            </button>
        </form>

        <p class="text-muted small mt-3">
            Al usar Google, aceptas nuestros 
            <a asp-action="Terms">términos y condiciones</a>
        </p>
    </div>
</div>
```

**Agregar Font Awesome para iconos** (en `_Layout.cshtml`):
```html
<link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css" />
```

### 2. **Crear `Views/Account/ExternalLoginConfirmation.cshtml`**

```html
@model RubricasApp.Web.ViewModels.Account.ExternalLoginConfirmationViewModel
@{
    ViewData["Title"] = "Completar Registro";
}

<div class="container mt-5">
    <div class="row justify-content-center">
        <div class="col-md-6">
            <div class="card shadow">
                <div class="card-header bg-primary text-white">
                    <h4 class="mb-0">
                        <i class="fas fa-user-check me-2"></i>
                        Completar Registro con @Model.Provider
                    </h4>
                </div>
                <div class="card-body">
                    @if (!string.IsNullOrEmpty(Model.ProfilePictureUrl))
                    {
                        <div class="text-center mb-4">
                            <img src="@Model.ProfilePictureUrl" 
                                 alt="Foto de perfil" 
                                 class="rounded-circle" 
                                 width="100" 
                                 height="100">
                        </div>
                    }

                    <div class="alert alert-info">
                        <i class="fas fa-info-circle me-2"></i>
                        Te has autenticado correctamente con <strong>@Model.Provider</strong>.
                        Por favor completa la información para finalizar tu registro.
                    </div>

                    <form asp-action="ExternalLoginConfirmation" 
                          asp-route-returnurl="@ViewData["ReturnUrl"]" 
                          method="post">
                        @Html.AntiForgeryToken()
                        
                        <input type="hidden" asp-for="Provider" />
                        <input type="hidden" asp-for="ProviderId" />
                        <input type="hidden" asp-for="ProfilePictureUrl" />

                        <div class="mb-3">
                            <label asp-for="Email" class="form-label"></label>
                            <input asp-for="Email" 
                                   class="form-control" 
                                   readonly 
                                   style="background-color: #f0f0f0;">
                            <span asp-validation-for="Email" class="text-danger"></span>
                            <small class="text-muted">
                                Este email está verificado por @Model.Provider
                            </small>
                        </div>

                        <div class="mb-3">
                            <label asp-for="Nombre" class="form-label"></label>
                            <input asp-for="Nombre" class="form-control">
                            <span asp-validation-for="Nombre" class="text-danger"></span>
                        </div>

                        <div class="mb-3">
                            <label asp-for="Apellido" class="form-label"></label>
                            <input asp-for="Apellido" class="form-control">
                            <span asp-validation-for="Apellido" class="text-danger"></span>
                        </div>

                        <div class="form-check mb-3">
                            <input asp-for="AceptaTerminos" 
                                   class="form-check-input" 
                                   type="checkbox">
                            <label asp-for="AceptaTerminos" class="form-check-label">
                                Acepto los 
                                <a asp-action="Terms" target="_blank">términos y condiciones</a>
                            </label>
                            <span asp-validation-for="AceptaTerminos" class="text-danger"></span>
                        </div>

                        <div class="d-grid gap-2">
                            <button type="submit" class="btn btn-primary btn-lg">
                                <i class="fas fa-check-circle me-2"></i>
                                Completar Registro
                            </button>
                            <a asp-action="Login" class="btn btn-outline-secondary">
                                <i class="fas fa-times me-2"></i>
                                Cancelar
                            </a>
                        </div>
                    </form>
                </div>
            </div>

            <div class="text-center mt-3">
                <small class="text-muted">
                    <i class="fas fa-shield-alt me-1"></i>
                    Tus datos están protegidos y nunca compartimos tu información personal
                </small>
            </div>
        </div>
    </div>
</div>

@section Scripts {
    <partial name="_ValidationScriptsPartial" />
}
```

---

## 🔒 Consideraciones de Seguridad

### 1. **Variables de Entorno en Producción**

```bash
# En servidor de producción
export Authentication__Google__ClientId="prod-client-id"
export Authentication__Google__ClientSecret="prod-secret"
```

### 2. **Validación de Email Verificado**

Google ya verifica el email, por lo que puedes confiar en él:
```csharp
user.EmailConfirmed = true; // Google ya verificó
```

### 3. **Manejo de Cuentas Duplicadas**

```csharp
// Verificar si existe usuario con ese email
var existingUser = await _userManager.FindByEmailAsync(email);

if (existingUser != null)
{
    // Opción 1: Vincular automáticamente
    await _userManager.AddLoginAsync(existingUser, info);
    
    // Opción 2: Solicitar confirmación al usuario
    // return View("LinkAccount", model);
}
```

### 4. **Rate Limiting**

```csharp
// En Program.cs
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("auth", config =>
    {
        config.PermitLimit = 5;
        config.Window = TimeSpan.FromMinutes(1);
    });
});

// En el controlador
[EnableRateLimiting("auth")]
public IActionResult ExternalLogin(string provider)
```

---

## 📊 Flujo de Datos

### Información que Google proporciona:

```csharp
// Claims disponibles de Google:
ClaimTypes.NameIdentifier    // ID único de Google
ClaimTypes.Email             // Email verificado
ClaimTypes.Name              // Nombre completo
ClaimTypes.GivenName         // Nombre
ClaimTypes.Surname           // Apellido
"picture"                    // URL foto de perfil
"locale"                     // Idioma (es, en, etc.)
```

---

## 🧪 Testing

### Usuarios de prueba en Google:

1. Ir a Google Cloud Console
2. OAuth consent screen > Test users
3. Agregar emails de prueba
4. Solo esos usuarios podrán autenticarse mientras esté en modo "Testing"

### Publicar en producción:

1. OAuth consent screen > Publishing status
2. Cambiar a "In production"
3. Google puede requerir verificación si pides scopes sensibles

---

## 📈 Mejoras Opcionales

### 1. **Múltiples Proveedores**

```csharp
.AddGoogle(...)
.AddFacebook(facebookOptions =>
{
    facebookOptions.AppId = configuration["Authentication:Facebook:AppId"];
    facebookOptions.AppSecret = configuration["Authentication:Facebook:AppSecret"];
})
.AddMicrosoftAccount(microsoftOptions =>
{
    microsoftOptions.ClientId = configuration["Authentication:Microsoft:ClientId"];
    microsoftOptions.ClientSecret = configuration["Authentication:Microsoft:ClientSecret"];
});
```

### 2. **Sincronización de Foto de Perfil**

```csharp
// Guardar foto localmente
var httpClient = new HttpClient();
var imageBytes = await httpClient.GetByteArrayAsync(profilePictureUrl);
var fileName = $"{userId}_profile.jpg";
await File.WriteAllBytesAsync($"wwwroot/profiles/{fileName}", imageBytes);
user.ProfilePictureUrl = $"/profiles/{fileName}";
```

### 3. **Desvinculación de Cuenta**

```csharp
[HttpPost]
public async Task<IActionResult> UnlinkExternalLogin(string provider)
{
    var user = await _userManager.GetUserAsync(User);
    var result = await _userManager.RemoveLoginAsync(
        user, provider, user.ExternalProviderId);
    
    if (result.Succeeded)
    {
        user.IsExternalAuth = false;
        await _userManager.UpdateAsync(user);
    }
    
    return RedirectToAction("Manage");
}
```

---

## 📖 Recursos Adicionales

- **Documentación oficial**: https://learn.microsoft.com/en-us/aspnet/core/security/authentication/social/google-logins
- **Google OAuth 2.0**: https://developers.google.com/identity/protocols/oauth2
- **GitHub ejemplo**: https://github.com/dotnet/AspNetCore.Docs/tree/main/aspnetcore/security/authentication/social

---

## ✅ Checklist de Implementación

```
[ ] 1. Crear proyecto en Google Cloud Console
[ ] 2. Configurar OAuth 2.0 y obtener credenciales
[ ] 3. Instalar paquete NuGet Microsoft.AspNetCore.Authentication.Google
[ ] 4. Agregar configuración en appsettings.json
[ ] 5. Configurar autenticación en Program.cs
[ ] 6. Extender ApplicationUser con campos externos
[ ] 7. Generar y aplicar migración de base de datos
[ ] 8. Crear ExternalLoginConfirmationViewModel
[ ] 9. Implementar métodos en AccountController
[ ] 10. Modificar vista Login.cshtml
[ ] 11. Crear vista ExternalLoginConfirmation.cshtml
[ ] 12. Probar flujo completo en desarrollo
[ ] 13. Configurar variables de entorno en producción
[ ] 14. Publicar OAuth consent screen
```

---

**Autor**: GitHub Copilot  
**Revisión**: Pendiente  
**Estado**: Documentado para implementación futura