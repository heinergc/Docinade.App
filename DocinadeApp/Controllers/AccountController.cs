using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using RubricasApp.Web.Models.Identity;
using RubricasApp.Web.Models;
using RubricasApp.Web.Models.Permissions;
using RubricasApp.Web.ViewModels.Account;
using RubricasApp.Web.Services;
using System.Text;
using System.Security.Claims;
using RubricasApp.Web.Services.Audit;
using RubricasApp.Web.Models.Audit;



namespace RubricasApp.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<AccountController> _logger;
        private readonly IEmailService _emailService;
        private readonly IConfiguracionService _configuracionService;
        private readonly IAuditService _auditService;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<AccountController> logger,
            IEmailService emailService,
            IConfiguracionService configuracionService,
            IAuditService auditService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _logger = logger;
            _emailService = emailService;
            _configuracionService = configuracionService;
            _auditService = auditService;
        }

        #region Login / Logout

        [AllowAnonymous]
        public async Task<IActionResult> Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToLocal(returnUrl);
            }

            // Verificar si el registro está abierto
            var esRegistroAbierto = await _configuracionService.EsRegistroAbiertoAsync();
            ViewData["RegistroAbierto"] = esRegistroAbierto;

            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Buscar usuario por email
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Email o contraseña incorrectos.");
                return View(model);
            }

            // Verificar si el usuario está activo
            if (!user.Activo)
            {
                ModelState.AddModelError(string.Empty, "Su cuenta está desactivada. Contacte al administrador.");
                return View(model);
            }

            // Intentar login
            var result = await _signInManager.PasswordSignInAsync(
                user.UserName ?? user.Email ?? string.Empty, 
                model.Password, 
                model.RememberMe, 
                lockoutOnFailure: true);

            if (result.Succeeded)
            {
                // Actualizar último acceso
                user.UltimoAcceso = DateTime.Now;
                await _userManager.UpdateAsync(user);

                // Log de auditoría
                await _auditService.LogAsync(
                    user.Id,
                    "Autenticación",
                    "Login",
                    user.Id,
                    $"Usuario {user.Email} inició sesión exitosamente"
                );

                _logger.LogInformation($"Usuario {user.Email} inició sesión exitosamente");
                return RedirectToLocal(returnUrl);
            }
            
            if (result.RequiresTwoFactor)
            {
                // TODO: Implementar 2FA en el futuro
                ModelState.AddModelError(string.Empty, "Se requiere autenticación de dos factores.");
                return View(model);
            }
            
            if (result.IsLockedOut)
            {
                _logger.LogWarning($"Cuenta bloqueada para el usuario {model.Email}");
                ModelState.AddModelError(string.Empty, "Cuenta bloqueada temporalmente por múltiples intentos fallidos.");
                return View(model);
            }
            
            ModelState.AddModelError(string.Empty, "Email o contraseña incorrectos.");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            var userEmail = User.Identity?.Name;
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            await _signInManager.SignOutAsync();
            
            // Log de auditoría
            if (!string.IsNullOrEmpty(userId))
            {
                await _auditService.LogAsync(
                    userId,
                    "Autenticación",
                    "Logout",
                    userId,
                    $"Usuario {userEmail} cerró sesión"
                );
            }
            
            _logger.LogInformation($"Usuario {userEmail} cerró sesión");
            return RedirectToAction("Index", "Home");
        }

        #endregion

        #region Password Recovery

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || !user.Activo)
            {
                // No revelar que el usuario no existe o está inactivo por seguridad
                return RedirectToAction(nameof(ForgotPasswordConfirmation));
            }

            // Generar token de restablecimiento
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            
            // Crear URL de callback
            var callbackUrl = Url.Action(
                nameof(ResetPassword),
                "Account",
                new { token, email = user.Email },
                protocol: Request.Scheme);

            // Enviar email
            try
            {
                await _emailService.SendPasswordResetEmailAsync(user.Email, callbackUrl!);
                
                _logger.LogInformation($"Email de recuperación enviado a {user.Email}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al enviar email de recuperación a {user.Email}");
                ModelState.AddModelError(string.Empty, "Error al enviar el correo de recuperación. Intente nuevamente.");
                return View(model);
            }

            return RedirectToAction(nameof(ForgotPasswordConfirmation));
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string? token = null, string? email = null)
        {
            if (token == null || email == null)
            {
                return BadRequest("Se requiere un token y email válidos.");
            }

            var model = new ResetPasswordViewModel
            {
                Token = token,
                Email = email
            };

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // No revelar que el usuario no existe
                return RedirectToAction(nameof(ResetPasswordConfirmation));
            }

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
            
            if (result.Succeeded)
            {
                // Log de auditoría
                await _auditService.LogAsync(
                    user.Id,
                    "Seguridad",
                    "ResetPassword",
                    user.Id,
                    $"Usuario {user.Email} restableció su contraseña"
                );
                
                _logger.LogInformation($"Contraseña restablecida para el usuario {user.Email}");
                return RedirectToAction(nameof(ResetPasswordConfirmation));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        #endregion

        #region User Management Actions (Admin only)

        [HttpPost]
        [Authorize(Policy = "RequireAdministratorRole")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { success = false, message = "Datos inválidos. Por favor revise el formulario." });
                }

                // Verificar si el email ya existe
                var existingUser = await _userManager.FindByEmailAsync(model.Email);
                if (existingUser != null)
                {
                    return Json(new { success = false, message = "Ya existe un usuario con ese email." });
                }

                // Crear el nuevo usuario
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    EmailConfirmed = true, // Los administradores pueden crear usuarios verificados
                    Nombre = model.Nombre,
                    Apellidos = model.Apellidos,
                    Institucion = model.Institucion,
                    Departamento = model.Departamento,
                    PhoneNumber = model.Telefono,
                    FechaRegistro = DateTime.Now,
                    Activo = true
                };

                // Generar contraseña temporal
                var tempPassword = GenerateTemporaryPassword();
                var result = await _userManager.CreateAsync(user, tempPassword);

                if (result.Succeeded)
                {
                    // Asignar roles seleccionados
                    if (model.Roles != null && model.Roles.Any())
                    {
                        foreach (var role in model.Roles)
                        {
                            if (await _roleManager.RoleExistsAsync(role))
                            {
                                await _userManager.AddToRoleAsync(user, role);
                            }
                        }
                    }
                    else
                    {
                        // Asignar rol por defecto
                        await _userManager.AddToRoleAsync(user, ApplicationRoles.Observador);
                    }

                    // Enviar email con la contraseña temporal (opcional)
                    try
                    {
                        await _emailService.SendTemporaryPasswordEmailAsync(user.Email, tempPassword);
                        _logger.LogInformation("Usuario {Email} creado exitosamente por {Admin}", user.Email, User.Identity?.Name);
                        return Json(new { success = true, message = $"Usuario creado exitosamente. Se ha enviado un email con la contraseña temporal a {user.Email}." });
                    }
                    catch (Exception emailEx)
                    {
                        _logger.LogWarning(emailEx, "No se pudo enviar email al usuario {Email}", user.Email);
                        return Json(new { success = true, message = $"Usuario creado exitosamente. Contraseña temporal: {tempPassword}" });
                    }
                }
                else
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    return Json(new { success = false, message = $"Error al crear usuario: {errors}" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear usuario {Email}", model.Email);
                return Json(new { success = false, message = "Error interno del servidor al crear el usuario." });
            }
        }

        [HttpPost]
        [Authorize(Policy = "RequireAdministratorRole")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LockUser(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return Json(new { success = false, message = "Usuario no encontrado." });
                }

                // Bloquear por 1 año (efectivamente permanente)
                await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.Now.AddYears(1));
                await _userManager.SetLockoutEnabledAsync(user, true);

                _logger.LogInformation("Usuario {Email} bloqueado por {Admin}", user.Email, User.Identity?.Name);
                return Json(new { success = true, message = "Usuario bloqueado exitosamente." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al bloquear usuario {UserId}", userId);
                return Json(new { success = false, message = "Error al bloquear el usuario." });
            }
        }

        [HttpPost]
        [Authorize(Policy = "RequireAdministratorRole")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UnlockUser(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return Json(new { success = false, message = "Usuario no encontrado." });
                }

                await _userManager.SetLockoutEndDateAsync(user, null);
                await _userManager.ResetAccessFailedCountAsync(user);

                _logger.LogInformation("Usuario {Email} desbloqueado por {Admin}", user.Email, User.Identity?.Name);
                return Json(new { success = true, message = "Usuario desbloqueado exitosamente." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al desbloquear usuario {UserId}", userId);
                return Json(new { success = false, message = "Error al desbloquear el usuario." });
            }
        }

        [HttpPost]
        [Authorize(Policy = "RequireAdministratorRole")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return Json(new { success = false, message = "Usuario no encontrado." });
                }

                // No permitir eliminar el usuario actual
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser?.Id == userId)
                {
                    return Json(new { success = false, message = "No puedes eliminar tu propia cuenta." });
                }

                var result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    _logger.LogInformation("Usuario {Email} eliminado por {Admin}", user.Email, User.Identity?.Name);
                    return Json(new { success = true, message = "Usuario eliminado exitosamente." });
                }
                else
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    return Json(new { success = false, message = $"Error al eliminar usuario: {errors}" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar usuario {UserId}", userId);
                return Json(new { success = false, message = "Error al eliminar el usuario." });
            }
        }

        [HttpGet]
        [Authorize(Policy = "RequireAdministratorRole")]
        public async Task<IActionResult> GetUserDetails(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return Json(new { success = false, message = "Usuario no encontrado." });
                }

                var roles = await _userManager.GetRolesAsync(user);
                var userDetails = new
                {
                    id = user.Id,
                    nombre = user.Nombre,
                    apellidos = user.Apellidos,
                    email = user.Email,
                    institucion = user.Institucion,
                    departamento = user.Departamento,
                    telefono = user.PhoneNumber,
                    roles = roles.ToList(),
                    activo = user.Activo
                };

                return Json(new { success = true, user = userDetails });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener detalles del usuario {UserId}", userId);
                return Json(new { success = false, message = "Error al obtener detalles del usuario." });
            }
        }

        private string GenerateTemporaryPassword()
        {
            // Generar contraseña temporal segura
            var random = new Random();
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var numbers = "0123456789";
            var symbols = "!@#$%";
            
            var password = new StringBuilder();
            
            // Al menos una mayúscula
            password.Append(chars[random.Next(0, 26)]);
            
            // Al menos una minúscula
            password.Append(chars[random.Next(26, 52)]);
            
            // Al menos un número
            password.Append(numbers[random.Next(numbers.Length)]);
            
            // Al menos un símbolo
            password.Append(symbols[random.Next(symbols.Length)]);
            
            // Completar hasta 8 caracteres
            for (int i = 4; i < 8; i++)
            {
                password.Append(chars[random.Next(chars.Length)]);
            }
            
            // Mezclar caracteres
            var result = password.ToString().ToCharArray();
            for (int i = 0; i < result.Length; i++)
            {
                int randomIndex = random.Next(result.Length);
                (result[i], result[randomIndex]) = (result[randomIndex], result[i]);
            }
            
            return new string(result);
        }
        #endregion

        #region Register

        [AllowAnonymous]
        public async Task<IActionResult> Register()
        {
            // Verificar si el registro público está habilitado
            var esRegistroAbierto = await _configuracionService.EsRegistroAbiertoAsync();
            if (!esRegistroAbierto)
            {
                var mensaje = await _configuracionService.ObtenerMensajeRegistroCerradoAsync();
                
                // Registrar intento de acceso bloqueado
                _logger.LogWarning("Intento de acceso al registro público bloqueado. IP: {IP}, UserAgent: {UserAgent}", 
                    Request.HttpContext.Connection.RemoteIpAddress?.ToString(),
                    Request.Headers["User-Agent"].ToString());
                
                ViewBag.MensajeBloqueo = mensaje;
                return View("RegistroCerrado");
            }

            var model = new RegisterViewModel
            {
                AvailableRoles = new List<string>
                {
                    ApplicationRoles.Docente,
                    ApplicationRoles.Evaluador,
                    ApplicationRoles.Observador
                }
            };
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            // Verificar si el registro público está habilitado
            var esRegistroAbierto = await _configuracionService.EsRegistroAbiertoAsync();
            if (!esRegistroAbierto)
            {
                var mensaje = await _configuracionService.ObtenerMensajeRegistroCerradoAsync();
                
                // Registrar intento de registro bloqueado
                _logger.LogWarning("Intento de registro público bloqueado para email: {Email}. IP: {IP}", 
                    model.Email, Request.HttpContext.Connection.RemoteIpAddress?.ToString());
                
                ViewBag.MensajeBloqueo = mensaje;
                return View("RegistroCerrado");
            }

            if (!ModelState.IsValid)
            {
                model.AvailableRoles = new List<string>
                {
                    ApplicationRoles.Docente,
                    ApplicationRoles.Evaluador,
                    ApplicationRoles.Observador
                };
                return View(model);
            }

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                EmailConfirmed = true, // En desarrollo, cambiar en producción
                Nombre = model.Nombre,
                Apellidos = model.Apellidos,
                NumeroIdentificacion = model.NumeroIdentificacion,
                Institucion = model.Institucion,
                Departamento = model.Departamento,
                FechaRegistro = DateTime.Now,
                Activo = true
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            
            if (result.Succeeded)
            {
                // Asignar rol seleccionado o por defecto
                var roleToAssign = !string.IsNullOrEmpty(model.SelectedRole) 
                    ? model.SelectedRole 
                    : ApplicationRoles.Evaluador;
                
                await _userManager.AddToRoleAsync(user, roleToAssign);

                string message = $"Usuario {user.Email} registrado exitosamente con rol {roleToAssign}";
                _logger.LogInformation(message);

                // En desarrollo, iniciar sesión automáticamente
                // En producción, enviar email de confirmación
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            model.AvailableRoles =
            [
                ApplicationRoles.Docente,
                ApplicationRoles.Evaluador,
                ApplicationRoles.Observador
            ];

            return View(model);
        }

        #endregion

        #region Profile Management

        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"No se pudo cargar el usuario con ID '{_userManager.GetUserId(User)}'.");
            }

            var roles = await _userManager.GetRolesAsync(user);
            var model = new ProfileViewModel
            {
                Id = user.Id,
                Nombre = user.Nombre,
                Apellidos = user.Apellidos,
                Email = user.Email ?? string.Empty,
                Institucion = user.Institucion ?? string.Empty,
                Departamento = user.Departamento,
                Telefono = user.PhoneNumber,
                Roles = string.Join(", ", roles)
            };

            // Agregar estadísticas para ViewBag
            ViewBag.RubricasCreadas = 0; // TODO: Implementar consulta real
            ViewBag.EvaluacionesRealizadas = 0; // TODO: Implementar consulta real

            return View(model);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(ProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    model.Roles = string.Join(", ", roles);
                }
                return View(model);
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return NotFound($"No se pudo cargar el usuario con ID '{_userManager.GetUserId(User)}'.");
            }

            // Actualizar propiedades
            currentUser.Nombre = model.Nombre;
            currentUser.Apellidos = model.Apellidos;
            currentUser.Institucion = model.Institucion;
            currentUser.Departamento = model.Departamento;
            currentUser.PhoneNumber = model.Telefono;

            var result = await _userManager.UpdateAsync(currentUser);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Perfil actualizado exitosamente.";
                return RedirectToAction(nameof(Profile));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            // Volver a cargar roles en caso de error
            var userRoles = await _userManager.GetRolesAsync(currentUser);
            model.Roles = string.Join(", ", userRoles);

            return View(model);
        }

        [Authorize]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"No se pudo cargar el usuario con ID '{_userManager.GetUserId(User)}'.");
            }

            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (result.Succeeded)
            {
                await _signInManager.RefreshSignInAsync(user);
                _logger.LogInformation("Usuario cambió su contraseña exitosamente.");
                TempData["SuccessMessage"] = "Contraseña cambiada exitosamente.";
                return RedirectToAction(nameof(Profile));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        #endregion

        #region User Management (Admin only)

        [Authorize(Policy = "RequireAdministratorRole")]
        public async Task<IActionResult> ManageUsers(string? searchTerm = null, string? roleFilter = null, string? statusFilter = null, string? institutionFilter = null)
        {
            var users = _userManager.Users.ToList();
            var userViewModels = new List<UserViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var userViewModel = new UserViewModel
                {
                    Id = user.Id,
                    NombreCompleto = user.NombreCompleto,
                    Email = user.Email,
                    NumeroIdentificacion = user.NumeroIdentificacion,
                    Institucion = user.Institucion,
                    Departamento = user.Departamento,
                    FechaRegistro = user.FechaRegistro,
                    UltimoAcceso = user.UltimoAcceso,
                    Activo = user.Activo,
                    Roles = roles.ToList(),
                    EmailConfirmed = user.EmailConfirmed,
                    LockoutEnabled = user.LockoutEnabled,
                    LockoutEnd = user.LockoutEnd,
                    LastLoginDate = user.UltimoAcceso
                };
                userViewModels.Add(userViewModel);
            }

            // Aplicar filtros si existen
            if (!string.IsNullOrEmpty(searchTerm))
            {
                userViewModels = userViewModels.Where(u => 
                    u.NombreCompleto.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    u.Email.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    (u.Institucion?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false)
                ).ToList();
            }

            if (!string.IsNullOrEmpty(roleFilter))
            {
                userViewModels = userViewModels.Where(u => u.Roles.Contains(roleFilter)).ToList();
            }

            if (!string.IsNullOrEmpty(statusFilter))
            {
                if (statusFilter == "Active")
                    userViewModels = userViewModels.Where(u => u.Activo).ToList();
                else if (statusFilter == "Inactive")
                    userViewModels = userViewModels.Where(u => !u.Activo).ToList();
            }

            if (!string.IsNullOrEmpty(institutionFilter))
            {
                userViewModels = userViewModels.Where(u => 
                    u.Institucion?.Contains(institutionFilter, StringComparison.OrdinalIgnoreCase) ?? false
                ).ToList();
            }

            var viewModel = new UserManagementViewModel
            {
                Users = userViewModels,
                SearchTerm = searchTerm,
                RoleFilter = roleFilter,
                StatusFilter = statusFilter,
                InstitutionFilter = institutionFilter
            };

            return View(viewModel);
        }

        #endregion

        #region Access Denied

        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }

        #endregion

        #region Helper Methods

        private IActionResult RedirectToLocal(string? returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        #endregion
    }
}