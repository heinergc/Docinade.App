using Microsoft.EntityFrameworkCore;
using DocinadeApp.Data;
using DocinadeApp.Models;

namespace DocinadeApp.Utils
{
    public class SqlServerDatabaseInitializer
    {
        private readonly RubricasDbContext _context;
        private readonly ILogger<SqlServerDatabaseInitializer> _logger;

        public SqlServerDatabaseInitializer(RubricasDbContext context, ILogger<SqlServerDatabaseInitializer> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Inicializa la base de datos SQL Server desde cero
        /// </summary>
        public async Task<bool> InitializeAsync()
        {
            try
            {
                _logger.LogInformation("[INIT] Iniciando configuración de SQL Server...");

                // 1. Verificar conexión antes de aplicar migraciones
                _logger.LogInformation("[INIT] Verificando conexión a la base de datos...");
                var canConnect = await _context.Database.CanConnectAsync();
                
                if (!canConnect)
                {
                    _logger.LogError("[ERROR] No se puede conectar a la base de datos");
                    return false;
                }
                
                _logger.LogInformation("[SUCCESS] Conexión exitosa a la base de datos");

                // 1.5. Verificar permisos del usuario actual
                await VerifyUserPermissionsAsync();

                // 2. Aplicar migraciones pendientes (esto crea la base de datos si no existe)
                _logger.LogInformation("[INIT] Aplicando migraciones EF Core...");
                await _context.Database.MigrateAsync();

                // 3. Verificar que las tablas principales existan
                await VerifyTablesAsync();

                // 4. Inicializar datos básicos usando execution strategy
                await InitializeBasicDataAsync();

                _logger.LogInformation("[SUCCESS] Base de datos SQL Server configurada correctamente");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[ERROR] Error configurando SQL Server: {Message}", ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Verifica los permisos del usuario actual
        /// </summary>
        private async Task VerifyUserPermissionsAsync()
        {
            try
            {
                var command = _context.Database.GetDbConnection().CreateCommand();
                command.CommandText = @"
                    SELECT 
                        CURRENT_USER AS CurrentUser,
                        USER_NAME() AS UserName,
                        HAS_PERMS_BY_NAME(NULL, NULL, 'CREATE TABLE') AS CanCreateTable,
                        IS_MEMBER('db_owner') AS IsDbOwner";
                
                await _context.Database.OpenConnectionAsync();
                using var reader = await command.ExecuteReaderAsync();
                
                if (await reader.ReadAsync())
                {
                    var currentUser = reader.GetString(0);
                    var userName = reader.GetString(1);
                    var canCreateTable = reader.GetInt32(2);
                    var isDbOwner = reader.IsDBNull(3) ? 0 : reader.GetInt32(3);
                    
                    _logger.LogInformation("👤 Usuario actual: {User} ({Name})", currentUser, userName);
                    _logger.LogInformation("📊 Permisos: CREATE TABLE={CanCreate}, DB_OWNER={IsOwner}", 
                        canCreateTable == 1, isDbOwner == 1);
                    
                    if (canCreateTable == 0)
                    {
                        _logger.LogWarning("⚠️ El usuario no tiene permisos para crear tablas. Ejecute: ALTER ROLE db_owner ADD MEMBER [{User}]", currentUser);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "⚠️ No se pudo verificar permisos del usuario");
            }
            finally
            {
                await _context.Database.CloseConnectionAsync();
            }
        }

        /// <summary>
        /// Verifica que las tablas principales existan
        /// </summary>
        private async Task VerifyTablesAsync()
        {
            var requiredTables = new[]
            {
                "AspNetUsers", "AspNetRoles", "AspNetUserRoles",
                "GruposCalificacion", "NivelesCalificacion", 
                "Rubricas", "ItemsEvaluacion", "ValoresRubrica",
                "Estudiantes", "Evaluaciones", "DetallesEvaluacion",
                "PeriodosAcademicos", "Materias",
                "InstrumentosEvaluacion", "InstrumentoRubricas", "InstrumentoMaterias",
                "CuadernosCalificadores", "CuadernoInstrumentos"
            };

            _logger.LogInformation("🔍 Verificando tablas de la base de datos...");

            foreach (var tableName in requiredTables)
            {
                try
                {
                    // Usar ExecuteScalar directamente en lugar de SqlQueryRaw
                    var command = _context.Database.GetDbConnection().CreateCommand();
                    command.CommandText = $@"
                        SELECT CASE 
                            WHEN EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '{tableName}') 
                            THEN 1 
                            ELSE 0 
                        END";
                    
                    await _context.Database.OpenConnectionAsync();
                    var result = await command.ExecuteScalarAsync();
                    var tableExists = Convert.ToInt32(result);
                    
                    if (tableExists == 1)
                    {
                        _logger.LogDebug("✅ Tabla {TableName}: OK", tableName);
                    }
                    else
                    {
                        _logger.LogWarning("❌ Tabla {TableName}: NO ENCONTRADA", tableName);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning("⚠️ Error verificando tabla {TableName}: {Error}", tableName, ex.Message);
                }
            }
        }

        /// <summary>
        /// Inicializa datos básicos del sistema usando execution strategy para compatibilidad con SQL Server retry logic
        /// </summary>
        private async Task InitializeBasicDataAsync()
        {
            _logger.LogInformation("🔧 Inicializando datos básicos...");

            // Use the execution strategy to handle retries and transactions properly
            var strategy = _context.Database.CreateExecutionStrategy();
            
            await strategy.ExecuteAsync(async () =>
            {
                // Use a single transaction for all operations
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    // 1. Grupo de calificación por defecto
                    if (!await _context.GruposCalificacion.AnyAsync())
                    {
                        var grupo = new GrupoCalificacion
                        {
                            NombreGrupo = "Evaluación Estándar",
                            Descripcion = "Niveles de calificación estándar para evaluaciones generales",
                            Estado = "ACTIVO",
                            FechaCreacion = DateTime.Now
                        };
                        _context.GruposCalificacion.Add(grupo);
                        await _context.SaveChangesAsync();
                        _logger.LogInformation("✅ Grupo de calificación creado");
                    }

                    // 2. Período académico por defecto
                    if (!await _context.PeriodosAcademicos.AnyAsync())
                    {
                        var periodo = new PeriodoAcademico
                        {
                            Codigo = "PQ2025-1",
                            Nombre = "Primer Cuatrimestre 2025",
                            Tipo = TipoPeriodo.Cuatrimestre,
                            Anio = 2025,
                            Ciclo = "I",
                            NumeroPeriodo = 1,
                            FechaInicio = new DateTime(2025, 1, 15),
                            FechaFin = new DateTime(2025, 5, 15),
                            Activo = true,
                            Estado = "Activo",
                            FechaCreacion = DateTime.Now
                        };
                        _context.PeriodosAcademicos.Add(periodo);
                        await _context.SaveChangesAsync();
                        _logger.LogInformation("✅ Período académico creado");
                    }

                    await transaction.CommitAsync();
                    _logger.LogInformation("🎉 Datos básicos inicializados correctamente");
                    
                    // 4. Llamar al DatabaseSeeder para datos adicionales de rúbricas
                    _logger.LogInformation("📚 Inicializando datos semilla de rúbricas...");
                    await DatabaseSeeder.SeedDatabase(_context);
                    _logger.LogInformation("✅ Datos semilla de rúbricas inicializados");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "❌ Error inicializando datos básicos: {Message}", ex.Message);
                    throw;
                }
            });
        }

        /// <summary>
        /// Ejecuta una operación con transacción usando execution strategy
        /// </summary>
        public async Task<T> ExecuteWithStrategyAsync<T>(Func<Task<T>> operation)
        {
            var strategy = _context.Database.CreateExecutionStrategy();
            return await strategy.ExecuteAsync(operation);
        }

        /// <summary>
        /// Ejecuta una operación con transacción usando execution strategy (sin retorno)
        /// </summary>
        public async Task ExecuteWithStrategyAsync(Func<Task> operation)
        {
            var strategy = _context.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(operation);
        }

        /// <summary>
        /// Método estático para fácil inicialización
        /// </summary>
        public static async Task<bool> InitializeDatabaseAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<RubricasDbContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<SqlServerDatabaseInitializer>>();
            
            var initializer = new SqlServerDatabaseInitializer(context, logger);
            return await initializer.InitializeAsync();
        }

        /// <summary>
        /// Método helper para ejecutar operaciones de base de datos con retry strategy
        /// </summary>
        public static async Task<T> ExecuteWithRetryAsync<T>(RubricasDbContext context, Func<Task<T>> operation)
        {
            var strategy = context.Database.CreateExecutionStrategy();
            return await strategy.ExecuteAsync(operation);
        }

        /// <summary>
        /// Método helper para ejecutar operaciones de base de datos con retry strategy (sin retorno)
        /// </summary>
        public static async Task ExecuteWithRetryAsync(RubricasDbContext context, Func<Task> operation)
        {
            var strategy = context.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(operation);
        }
    }
}