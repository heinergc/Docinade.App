using Microsoft.EntityFrameworkCore;
using RubricasApp.Web.Data;

namespace RubricasApp.Web.Scripts
{
    /// <summary>
    /// Script para crear la tabla de auditoría si no existe
    /// </summary>
    public class CrearTablaAuditoria
    {
        private readonly RubricasDbContext _context;

        public CrearTablaAuditoria(RubricasDbContext context)
        {
            _context = context;
        }

        public async Task EjecutarAsync()
        {
            try
            {
                Console.WriteLine("🔍 Verificando si existe la tabla AuditoriasOperaciones...");

                // Verificar si la tabla ya existe
                var tableExists = await TablaExisteAsync("AuditoriasOperaciones");
                
                if (tableExists)
                {
                    Console.WriteLine("✅ La tabla AuditoriasOperaciones ya existe.");
                    return;
                }

                Console.WriteLine("📋 Creando tabla AuditoriasOperaciones...");

                // 🔧 CORRECCIÓN: Usar sintaxis de SQL Server en lugar de SQLite
                await _context.Database.ExecuteSqlRawAsync(@"
                    CREATE TABLE [AuditoriasOperaciones] (
                        [Id] [int] IDENTITY(1,1) NOT NULL,
                        [TipoOperacion] [nvarchar](50) NOT NULL,
                        [TablaAfectada] [nvarchar](100) NOT NULL,
                        [RegistroId] [int] NOT NULL,
                        [Descripcion] [nvarchar](500) NOT NULL,
                        [Motivo] [nvarchar](200) NULL,
                        [DatosAnteriores] [nvarchar](max) NULL,
                        [DatosNuevos] [nvarchar](max) NULL,
                        [DireccionIP] [nvarchar](45) NULL,
                        [UserAgent] [nvarchar](500) NULL,
                        [FechaOperacion] [datetime2](7) NOT NULL DEFAULT GETDATE(),
                        [UsuarioId] [nvarchar](450) NOT NULL,
                        [OperacionExitosa] [bit] NOT NULL DEFAULT 1,
                        [MensajeError] [nvarchar](max) NULL,
                        
                        CONSTRAINT [PK_AuditoriasOperaciones] PRIMARY KEY ([Id]),
                        CONSTRAINT [FK_AuditoriasOperaciones_AspNetUsers] 
                            FOREIGN KEY ([UsuarioId]) REFERENCES [AspNetUsers] ([Id])
                    )
                ");

                Console.WriteLine("📊 Creando índices para optimizar consultas...");

                // 🔧 CORRECCIÓN: Usar sintaxis de índices para SQL Server
                await _context.Database.ExecuteSqlRawAsync(@"
                    CREATE NONCLUSTERED INDEX [IX_AuditoriaOperacion_TablaAfectada] 
                    ON [AuditoriasOperaciones] ([TablaAfectada])
                ");

                await _context.Database.ExecuteSqlRawAsync(@"
                    CREATE NONCLUSTERED INDEX [IX_AuditoriaOperacion_RegistroId] 
                    ON [AuditoriasOperaciones] ([RegistroId])
                ");

                await _context.Database.ExecuteSqlRawAsync(@"
                    CREATE NONCLUSTERED INDEX [IX_AuditoriaOperacion_FechaOperacion] 
                    ON [AuditoriasOperaciones] ([FechaOperacion])
                ");

                await _context.Database.ExecuteSqlRawAsync(@"
                    CREATE NONCLUSTERED INDEX [IX_AuditoriaOperacion_UsuarioId] 
                    ON [AuditoriasOperaciones] ([UsuarioId])
                ");

                await _context.Database.ExecuteSqlRawAsync(@"
                    CREATE NONCLUSTERED INDEX [IX_AuditoriaOperacion_Tabla_Registro] 
                    ON [AuditoriasOperaciones] ([TablaAfectada], [RegistroId])
                ");

                Console.WriteLine("✅ Tabla AuditoriasOperaciones creada exitosamente con todos los índices.");

                // 🔧 CORRECCIÓN: Usar sintaxis de SQL Server para insertar registro
                await _context.Database.ExecuteSqlRawAsync(@"
                    INSERT INTO [AuditoriasOperaciones] (
                        [TipoOperacion], [TablaAfectada], [RegistroId], [Descripcion], 
                        [UsuarioId], [OperacionExitosa], [FechaOperacion]
                    ) VALUES (
                        'CREATE', 'AuditoriasOperaciones', 0, 
                        'Tabla de auditoría creada automáticamente por el sistema',
                        'SYSTEM', 1, GETDATE()
                    )
                ");

                Console.WriteLine("🎯 Registro de auditoría inicial creado.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"💥 Error al crear tabla de auditoría: {ex.Message}");
                throw;
            }
        }

        private async Task<bool> TablaExisteAsync(string nombreTabla)
        {
            try
            {
                // Usar ExecuteSqlRaw con una consulta que no devuelve resultados
                var command = _context.Database.GetDbConnection().CreateCommand();
                command.CommandText = $@"
                    SELECT CASE 
                        WHEN EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '{nombreTabla}') 
                        THEN 1 
                        ELSE 0 
                    END";
                
                await _context.Database.OpenConnectionAsync();
                var result = await command.ExecuteScalarAsync();
                return Convert.ToInt32(result) == 1;
            }
            catch
            {
                // Si hay error, asumimos que no existe
                return false;
            }
        }
    }
}
