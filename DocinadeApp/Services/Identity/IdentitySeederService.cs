using Microsoft.AspNetCore.Identity;
using RubricasApp.Web.Models.Identity;
using RubricasApp.Web.Models.Permissions;

namespace RubricasApp.Web.Services.Identity
{
    public class IdentitySeederService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<IdentitySeederService> _logger;

        public IdentitySeederService(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<IdentitySeederService> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        public async Task SeedAsync()
        {
            try
            {
                await SeedRolesAsync();
                await SeedDefaultUsersAsync();
                _logger.LogInformation("Identity seeding completed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while seeding identity data");
                throw;
            }
        }

        private async Task SeedRolesAsync()
        {
            _logger.LogInformation("Seeding roles...");
            
            foreach (var roleName in ApplicationRoles.AllRoles)
            {
                if (!await _roleManager.RoleExistsAsync(roleName))
                {
                    var role = new IdentityRole(roleName);
                    var result = await _roleManager.CreateAsync(role);
                    
                    if (result.Succeeded)
                    {
                        _logger.LogInformation($"Role '{roleName}' created successfully");
                    }
                    else
                    {
                        _logger.LogWarning($"Failed to create role '{roleName}': {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    }
                }
                else
                {
                    _logger.LogInformation($"Role '{roleName}' already exists");
                }
            }
        }

        private async Task SeedDefaultUsersAsync()
        {
            _logger.LogInformation("[SEED] Seeding default users...");

            // SOLO crear Super Administrador (mínimo necesario para producción)
            await CreateDefaultUserAsync(
                email: "admin@rubricas.edu",
                userName: "admin@rubricas.edu",
                nombre: "Administrador",
                apellidos: "del Sistema",
                password: "Admin@2025!",
                role: ApplicationRoles.SuperAdministrador,
                numeroIdentificacion: "000000000",
                institucion: "Sistema de Rúbricas",
                departamento: "Administración"
            );

            // Los usuarios de ejemplo (docente, evaluador) se crean solo en desarrollo
            // En producción, el admin los creará desde la UI
        }

        private async Task<bool> CreateDefaultUserAsync(
            string email,
            string userName,
            string nombre,
            string apellidos,
            string password,
            string role,
            string? numeroIdentificacion = null,
            string? institucion = null,
            string? departamento = null)
        {
            try
            {
                var existingUser = await _userManager.FindByEmailAsync(email);
                if (existingUser != null)
                {
                    _logger.LogInformation($"[INFO] Usuario '{email}' ya existe, verificando configuración...");
                    
                    // Verificar que tenga el rol asignado
                    var hasRole = await _userManager.IsInRoleAsync(existingUser, role);
                    if (!hasRole)
                    {
                        await _userManager.AddToRoleAsync(existingUser, role);
                        _logger.LogInformation($"[SUCCESS] Rol '{role}' asignado a usuario existente '{email}'");
                    }
                    
                    // Asegurar que esté activo y confirmado
                    if (!existingUser.EmailConfirmed || !existingUser.Activo)
                    {
                        existingUser.EmailConfirmed = true;
                        existingUser.Activo = true;
                        existingUser.LockoutEnd = null;
                        await _userManager.UpdateAsync(existingUser);
                        _logger.LogInformation($"[SUCCESS] Usuario '{email}' actualizado: activo y email confirmado");
                    }
                    
                    return false; // Ya existía
                }

                var user = new ApplicationUser
                {
                    UserName = userName,
                    Email = email,
                    EmailConfirmed = true,
                    Nombre = nombre,
                    Apellidos = apellidos,
                    NumeroIdentificacion = numeroIdentificacion,
                    Institucion = institucion,
                    Departamento = departamento,
                    FechaRegistro = DateTime.Now,
                    Activo = true,
                    LockoutEnabled = false
                };

                var result = await _userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, role);
                    _logger.LogInformation($"[SUCCESS] Usuario '{email}' creado exitosamente con rol '{role}'");
                    return true; // Creado exitosamente
                }
                else
                {
                    _logger.LogError($"[ERROR] Error creando usuario '{email}': {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[ERROR] Excepción creando usuario '{email}'");
                return false;
            }
        }

        public async Task<ApplicationUser?> GetDefaultAdminAsync()
        {
            return await _userManager.FindByEmailAsync("admin@rubricas.edu");
        }

        public async Task<bool> HasAnyUsersAsync()
        {
            return _userManager.Users.Any();
        }

        public async Task<bool> HasAdminUserAsync()
        {
            var adminUsers = await _userManager.GetUsersInRoleAsync(ApplicationRoles.SuperAdministrador);
            return adminUsers.Any();
        }
    }
}