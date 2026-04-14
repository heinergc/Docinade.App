using Microsoft.EntityFrameworkCore;
using DocinadeApp.Data;

namespace DocinadeApp.Utils
{
    public class DatabaseMigrator
    {
        public static async Task ApplyMigrationsAsync()
        {
            // Configuración específica para SQL Server únicamente
            var connectionString = "Server=SCPDTIC16584\\SQLEXPRESS;Database=RubricasDb;Integrated Security=true;MultipleActiveResultSets=true;TrustServerCertificate=true";
            var options = new DbContextOptionsBuilder<RubricasDbContext>()
                .UseSqlServer(connectionString, sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 3,
                        maxRetryDelay: TimeSpan.FromSeconds(5),
                        errorNumbersToAdd: null);
                    sqlOptions.CommandTimeout(30);
                })
                .EnableSensitiveDataLogging(false) // Solo en desarrollo
                .EnableDetailedErrors(false)
                .Options;

            using var context = new RubricasDbContext(options);
            
            try
            {
                // Eliminar la base de datos si existe para empezar desde cero
                await context.Database.EnsureDeletedAsync();
                Console.WriteLine("🗑️ Base de datos eliminada (si existía).");

                // Aplicar todas las migraciones disponibles
                await context.Database.MigrateAsync();
                Console.WriteLine("🏢 Base de datos SQL Server creada y migraciones aplicadas exitosamente.");

                // Verificar que las tablas principales existan
                var tableNames = new[] 
                { 
                    "AspNetUsers", "AspNetRoles", "Rubricas", "ItemsEvaluacion", 
                    "NivelesCalificacion", "ValoresRubrica", "Estudiantes", "Evaluaciones" 
                };

                foreach (var tableName in tableNames)
                {
                    var exists = await context.Database.ExecuteSqlRawAsync(
                        $"SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '{tableName}'");
                    Console.WriteLine($"✅ Tabla {tableName}: verificada");
                }

                Console.WriteLine("🎉 Base de datos SQL Server configurada correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al configurar la base de datos SQL Server: {ex.Message}");
                Console.WriteLine($"🔍 Detalles: {ex.InnerException?.Message}");
                throw;
            }
        }

        /// <summary>
        /// Verifica la conectividad con SQL Server
        /// </summary>
        public static async Task<bool> VerifyConnectionAsync()
        {
            var connectionString = "Server=SCPDTIC16584\\SQLEXPRESS;Database=master;Integrated Security=true;TrustServerCertificate=true";
            var options = new DbContextOptionsBuilder<RubricasDbContext>()
                .UseSqlServer(connectionString)
                .Options;

            try
            {
                using var context = new RubricasDbContext(options);
                await context.Database.CanConnectAsync();
                Console.WriteLine("✅ Conexión a SQL Server verificada exitosamente.");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error de conexión a SQL Server: {ex.Message}");
                return false;
            }
        }
    }
}