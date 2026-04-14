using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using RubricasApp.Web.Data;

namespace RubricasApp.Web.Data
{
    public static class InstrumentoEvaluacionFixer
    {
        /// <summary>
        /// Corrige el esquema de la tabla InstrumentosEvaluacion para que funcione con el modelo actual
        /// </summary>
        public static async Task FixInstrumentoEvaluacionSchema(RubricasDbContext context)
        {
            try
            {
                Console.WriteLine("?? Iniciando corrección del esquema de InstrumentosEvaluacion...");
                
                var connection = context.Database.GetDbConnection();
                await connection.OpenAsync();
                
                // 1. Verificar estructura actual de la tabla
                using var tableInfoCommand = connection.CreateCommand();
                tableInfoCommand.CommandText = "PRAGMA table_info(InstrumentosEvaluacion)";
                
                var columns = new List<string>();
                using (var reader = await tableInfoCommand.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        columns.Add(reader.GetString(1)); // nombre de columna
                    }
                }
                
                bool hasActivo = columns.Contains("Activo");
                bool hasEstaActivo = columns.Contains("EstaActivo");
                
                Console.WriteLine($"?? Estado actual: Activo={hasActivo}, EstaActivo={hasEstaActivo}");
                
                // 2. Agregar columna Activo si no existe
                if (!hasActivo)
                {
                    using var addActivoCommand = connection.CreateCommand();
                    addActivoCommand.CommandText = "ALTER TABLE InstrumentosEvaluacion ADD COLUMN Activo INTEGER DEFAULT 1 NOT NULL";
                    await addActivoCommand.ExecuteNonQueryAsync();
                    Console.WriteLine("? Columna Activo agregada");
                }
                
                // 3. Si existe EstaActivo, sincronizar datos
                if (hasEstaActivo)
                {
                    using var syncCommand = connection.CreateCommand();
                    syncCommand.CommandText = @"
                        UPDATE InstrumentosEvaluacion 
                        SET Activo = COALESCE(EstaActivo, 1) 
                        WHERE Activo IS NULL OR Activo != COALESCE(EstaActivo, 1)";
                    var rowsUpdated = await syncCommand.ExecuteNonQueryAsync();
                    Console.WriteLine($"?? {rowsUpdated} registros sincronizados desde EstaActivo a Activo");
                }
                
                // 4. Asegurar que no hay valores NULL en Activo
                using var fixNullCommand = connection.CreateCommand();
                fixNullCommand.CommandText = "UPDATE InstrumentosEvaluacion SET Activo = 1 WHERE Activo IS NULL";
                var nullsFixed = await fixNullCommand.ExecuteNonQueryAsync();
                Console.WriteLine($"??? {nullsFixed} valores NULL corregidos en Activo");
                
                // 5. Verificar que todo está correcto
                using var verifyCommand = connection.CreateCommand();
                verifyCommand.CommandText = "SELECT COUNT(*) FROM InstrumentosEvaluacion WHERE Activo IS NULL";
                var nullCount = Convert.ToInt32(await verifyCommand.ExecuteScalarAsync());
                
                if (nullCount == 0)
                {
                    Console.WriteLine("?? Esquema de InstrumentosEvaluacion corregido exitosamente");
                }
                else
                {
                    Console.WriteLine($"?? Aún quedan {nullCount} valores NULL en Activo");
                }
                
                await connection.CloseAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"? Error corrigiendo esquema: {ex.Message}");
                throw;
            }
        }
        
        /// <summary>
        /// Ejecuta la corrección antes de cualquier operación de la aplicación
        /// </summary>
        public static async Task EnsureSchemaIsFixed(RubricasDbContext context)
        {
            try
            {
                await FixInstrumentoEvaluacionSchema(context);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"? Error en EnsureSchemaIsFixed: {ex.Message}");
                // No lanzar excepción para no romper la aplicación
            }
        }
    }
}