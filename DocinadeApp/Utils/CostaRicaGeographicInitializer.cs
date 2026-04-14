using Microsoft.EntityFrameworkCore;
using RubricasApp.Web.Data;
using RubricasApp.Web.Models;

namespace RubricasApp.Web.Utils
{
    /// <summary>
    /// Inicializador simplificado para los datos geográficos de Costa Rica (solo Provincias)
    /// </summary>
    public static class CostaRicaGeographicInitializer
    {
        /// <summary>
        /// Inicializa las provincias de Costa Rica si no existen
        /// </summary>
        public static async Task<bool> InitializeAsync(RubricasDbContext context)
        {
            try
            {
                // Verificar si ya existen provincias
                var provinciaCount = await context.Set<Provincia>().CountAsync();
                if (provinciaCount > 0)
                {
                    return true; // Ya están inicializadas
                }

                // Insertar Provincias (sin IDs explícitos para evitar IDENTITY_INSERT issues)
                var provincias = new[]
                {
                    new Provincia { Nombre = "San José", Codigo = "01", Estado = true },
                    new Provincia { Nombre = "Alajuela", Codigo = "02", Estado = true },
                    new Provincia { Nombre = "Cartago", Codigo = "03", Estado = true },
                    new Provincia { Nombre = "Heredia", Codigo = "04", Estado = true },
                    new Provincia { Nombre = "Guanacaste", Codigo = "05", Estado = true },
                    new Provincia { Nombre = "Puntarenas", Codigo = "06", Estado = true },
                    new Provincia { Nombre = "Limón", Codigo = "07", Estado = true }
                };

                await context.Set<Provincia>().AddRangeAsync(provincias);
                await context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                // Log error pero no lanzar excepción para no bloquear el startup
                Console.WriteLine($"[WARNING] Error inicializando datos geográficos: {ex.Message}");
                return false;
            }
        }
    }
}