using Microsoft.EntityFrameworkCore;
using DocinadeApp.Data;
using System.Data;

namespace DocinadeApp.Data
{
    public static class SqliteBooleanFixer
    {
        /// <summary>
        /// Corrige los valores NULL en campos booleanos de SQLite
        /// </summary>
        public static async Task FixBooleanFields(RubricasDbContext context)
        {
            try
            {
                // 1. Verificar y corregir EsPublica en Rubricas
                var nullEsPublicaCount = await context.Database.ExecuteSqlRawAsync(
                    "UPDATE Rubricas SET EsPublica = 1 WHERE EsPublica IS NULL"
                );
                
                // 2. Verificar y corregir otros campos booleanos
                await context.Database.ExecuteSqlRawAsync(
                    "UPDATE AspNetUsers SET IsActive = 1 WHERE IsActive IS NULL"
                );
                
                await context.Database.ExecuteSqlRawAsync(
                    "UPDATE AspNetUsers SET Activo = 1 WHERE Activo IS NULL"
                );
                
                await context.Database.ExecuteSqlRawAsync(
                    "UPDATE PeriodosAcademicos SET Activo = 0 WHERE Activo IS NULL"
                );
                
                await context.Database.ExecuteSqlRawAsync(
                    "UPDATE Materias SET Activa = 1 WHERE Activa IS NULL"
                );
                
                await context.Database.ExecuteSqlRawAsync(
                    "UPDATE InstrumentosEvaluacion SET EstaActivo = 1 WHERE EstaActivo IS NULL"
                );
                
                Console.WriteLine("? Campos booleanos corregidos en SQLite");
                Console.WriteLine($"   - {nullEsPublicaCount} registros de EsPublica corregidos");
                
                // 3. Verificar que no queden valores NULL
                await VerifyBooleanFields(context);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"? Error corrigiendo campos booleanos: {ex.Message}");
                throw;
            }
        }
        
        /// <summary>
        /// Verifica que no queden valores NULL en campos booleanos
        /// </summary>
        private static async Task VerifyBooleanFields(RubricasDbContext context)
        {
            var queries = new Dictionary<string, string>
            {
                ["Rubricas.EsPublica"] = "SELECT COUNT(*) FROM Rubricas WHERE EsPublica IS NULL",
                ["AspNetUsers.IsActive"] = "SELECT COUNT(*) FROM AspNetUsers WHERE IsActive IS NULL",
                ["AspNetUsers.Activo"] = "SELECT COUNT(*) FROM AspNetUsers WHERE Activo IS NULL",
                ["PeriodosAcademicos.Activo"] = "SELECT COUNT(*) FROM PeriodosAcademicos WHERE Activo IS NULL",
                ["Materias.Activa"] = "SELECT COUNT(*) FROM Materias WHERE Activa IS NULL"
            };
            
            foreach (var query in queries)
            {
                try
                {
                    using var command = context.Database.GetDbConnection().CreateCommand();
                    command.CommandText = query.Value;
                    
                    if (command.Connection?.State != ConnectionState.Open)
                        await command.Connection!.OpenAsync();
                        
                    var result = await command.ExecuteScalarAsync();
                    var count = Convert.ToInt32(result);
                    
                    if (count > 0)
                    {
                        Console.WriteLine($"??  Advertencia: {count} valores NULL encontrados en {query.Key}");
                    }
                    else
                    {
                        Console.WriteLine($"? {query.Key}: Sin valores NULL");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"? Error verificando {query.Key}: {ex.Message}");
                }
            }
        }
        
        /// <summary>
        /// Convierte expl�citamente los valores string boolean a enteros para SQLite
        /// </summary>
        public static int BooleanToSqliteInt(bool value)
        {
            return value ? 1 : 0;
        }
        
        /// <summary>
        /// Convierte valores de SQLite integer a boolean
        /// </summary>
        public static bool SqliteIntToBoolean(object? value)
        {
            if (value == null || value == DBNull.Value)
                return false;
                
            if (value is int intValue)
                return intValue != 0;
                
            if (value is long longValue)
                return longValue != 0;
                
            if (bool.TryParse(value.ToString(), out bool boolValue))
                return boolValue;
                
            return false;
        }
    }
}