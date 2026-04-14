using Microsoft.AspNetCore.Identity;
using DocinadeApp.Models.Identity;
using DocinadeApp.Models.Permissions;

namespace DocinadeApp.Utils;

/// <summary>
/// Clase simple para crear un usuario administrador por defecto
/// </summary>
public static class AdminSeeder
{
    /// <summary>
    /// Crea un usuario administrador por defecto si no existe
    /// </summary>
    public static async Task SeedDefaultAdminAsync(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        ILogger logger)
    {
        const string adminEmail = "admin@rubricas.com";
        const string adminPassword = "Admin123!";
        
        try
        {
            logger.LogInformation("👤 Verificando usuario administrador por defecto...");
            
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    Nombre = "Administrador",
                    Apellidos = "Sistema",
                    Activo = true,
                    FechaRegistro = DateTime.Now,
                    UltimoAcceso = DateTime.Now
                };
                
                var result = await userManager.CreateAsync(adminUser, adminPassword);
                if (result.Succeeded)
                {
                    // Asignar rol de Administrador
                    if (await roleManager.RoleExistsAsync(ApplicationRoles.Administrador))
                    {
                        await userManager.AddToRoleAsync(adminUser, ApplicationRoles.Administrador);
                        logger.LogInformation("✅ Usuario administrador creado: {Email} con contraseña: {Password}", 
                            adminEmail, adminPassword);
                    }
                    else
                    {
                        logger.LogWarning("⚠️ Rol 'Administrador' no existe, no se pudo asignar al usuario");
                    }
                }
                else
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    logger.LogError("❌ Error creando usuario administrador: {Errors}", errors);
                }
            }
            else
            {
                logger.LogInformation("ℹ️ Usuario administrador ya existe: {Email}", adminEmail);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "❌ Error en seeding de usuario administrador");
        }
    }
}
