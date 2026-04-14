using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using DocinadeApp.Data;

namespace DocinadeApp.Utils
{
    public static class DatabaseFixer
    {
        public static async Task FixMissingObservacionesColumns(RubricasDbContext context)
        {
            Console.WriteLine("🔧 Verificando y corrigiendo columnas Observaciones faltantes...");
            
            try
            {
                var connectionString = context.Database.GetConnectionString();
                
                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();
                
                // Script para agregar las columnas faltantes
                var sql = @"
                    -- Verificar y agregar columna Observaciones a InstrumentoMaterias
                    IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
                                   WHERE TABLE_NAME = 'InstrumentoMaterias' AND COLUMN_NAME = 'Observaciones')
                    BEGIN
                        ALTER TABLE InstrumentoMaterias ADD Observaciones NVARCHAR(MAX) NULL;
                        PRINT 'Columna Observaciones agregada a InstrumentoMaterias';
                    END

                    -- Verificar y agregar columna Observaciones a MateriaPeriodos
                    IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
                                   WHERE TABLE_NAME = 'MateriaPeriodos' AND COLUMN_NAME = 'Observaciones')
                    BEGIN
                        ALTER TABLE MateriaPeriodos ADD Observaciones NVARCHAR(500) NULL;
                        PRINT 'Columna Observaciones agregada a MateriaPeriodos';
                    END";
                
                using var command = new SqlCommand(sql, connection);
                await command.ExecuteNonQueryAsync();
                
                // Verificar que las columnas fueron agregadas
                var verificationSql = @"
                    SELECT 'InstrumentoMaterias' as Tabla, COLUMN_NAME, DATA_TYPE 
                    FROM INFORMATION_SCHEMA.COLUMNS 
                    WHERE TABLE_NAME = 'InstrumentoMaterias' AND COLUMN_NAME = 'Observaciones'
                    
                    UNION ALL
                    
                    SELECT 'MateriaPeriodos' as Tabla, COLUMN_NAME, DATA_TYPE 
                    FROM INFORMATION_SCHEMA.COLUMNS 
                    WHERE TABLE_NAME = 'MateriaPeriodos' AND COLUMN_NAME = 'Observaciones'";
                
                using var verifyCommand = new SqlCommand(verificationSql, connection);
                using var reader = await verifyCommand.ExecuteReaderAsync();
                
                Console.WriteLine("✅ Verificación de columnas Observaciones:");
                while (await reader.ReadAsync())
                {
                    var tabla = reader["Tabla"].ToString();
                    var columna = reader["COLUMN_NAME"].ToString();
                    var tipo = reader["DATA_TYPE"].ToString();
                    Console.WriteLine($"   - {tabla}.{columna}: {tipo}");
                }
                
                Console.WriteLine("🎉 Corrección de base de datos completada exitosamente!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al corregir la base de datos: {ex.Message}");
                throw;
            }
        }
    }
}
