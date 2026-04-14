using Microsoft.EntityFrameworkCore;
using System;

namespace RubricasApp.Web.Data
{
    public static class DatabaseFixHelper
    {
        public static string GetFixEsPublicaScript()
        {
            return @"
                -- Fix for SQLite Error 19: NOT NULL constraint failed: Rubricas.EsPublica
                
                -- Step 1: Check if there are any NULL values in EsPublica
                SELECT COUNT(*) as NullCount FROM Rubricas WHERE EsPublica IS NULL;
                
                -- Step 2: Update any NULL values to TRUE (1 in SQLite)
                UPDATE Rubricas SET EsPublica = 1 WHERE EsPublica IS NULL;
                
                -- Step 3: Verify the update
                SELECT COUNT(*) as NullCountAfter FROM Rubricas WHERE EsPublica IS NULL;
                
                -- Alternative approach: If the column doesn't exist, add it with default value
                -- PRAGMA table_info(Rubricas);
                
                -- Note: SQLite doesn't support ALTER COLUMN to add NOT NULL with DEFAULT
                -- So we'll handle this in code by always setting EsPublica explicitly
            ";
        }
        
        public static async Task FixEsPublicaColumn(RubricasDbContext context)
        {
            try
            {
                // Update any NULL values to true
                await context.Database.ExecuteSqlRawAsync(
                    "UPDATE Rubricas SET EsPublica = 1 WHERE EsPublica IS NULL"
                );
                
                Console.WriteLine("✅ Fixed NULL values in EsPublica column");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error fixing EsPublica column: {ex.Message}");
            }
        }
    }
}