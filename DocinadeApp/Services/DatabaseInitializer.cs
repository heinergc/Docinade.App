using Microsoft.EntityFrameworkCore;
using RubricasApp.Web.Data;

namespace RubricasApp.Web.Services
{
    public static class DatabaseInitializer
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<RubricasDbContext>();
            
            try
            {
                // Verificar si la base de datos existe y tiene las tablas de Identity
                var canConnect = await context.Database.CanConnectAsync();
                
                if (!canConnect)
                {
                    Console.WriteLine("No se puede conectar a la base de datos. Creando base de datos...");
                    await context.Database.EnsureCreatedAsync();
                }
                else
                {
                    // Verificar si las tablas de Identity existen
                    var hasIdentityTables = await HasIdentityTablesAsync(context);
                    
                    if (!hasIdentityTables)
                    {
                        Console.WriteLine("Las tablas de Identity no existen. Recreando base de datos...");
                        
                        // Eliminar y recrear la base de datos
                        await context.Database.EnsureDeletedAsync();
                        await context.Database.EnsureCreatedAsync();
                        
                        Console.WriteLine("Base de datos recreada con tablas de Identity.");
                    }
                    else
                    {
                        Console.WriteLine("Base de datos existente con tablas de Identity encontrada.");
                        
                        // Aplicar migraciones pendientes si las hay
                        var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
                        if (pendingMigrations.Any())
                        {
                            Console.WriteLine($"Aplicando {pendingMigrations.Count()} migraciones pendientes...");
                            await context.Database.MigrateAsync();
                        }
                    }
                }
                
                Console.WriteLine("Inicialización de base de datos completada.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error durante la inicialización de la base de datos: {ex.Message}");
                
                // Como último recurso, recrear la base de datos
                try
                {
                    Console.WriteLine("Intentando recrear la base de datos como último recurso...");
                    await context.Database.EnsureDeletedAsync();
                    await context.Database.EnsureCreatedAsync();
                    Console.WriteLine("Base de datos recreada exitosamente.");
                }
                catch (Exception recreateEx)
                {
                    Console.WriteLine($"Error crítico al recrear la base de datos: {recreateEx.Message}");
                    throw;
                }
            }
        }
        
        private static async Task<bool> HasIdentityTablesAsync(RubricasDbContext context)
        {
            try
            {
                // Intentar una consulta simple en una tabla de Identity
                var userCount = await context.Users.CountAsync();
                return true;
            }
            catch
            {
                // Si falla, las tablas de Identity no existen
                return false;
            }
        }
    }
}