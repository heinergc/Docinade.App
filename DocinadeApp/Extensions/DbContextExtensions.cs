using Microsoft.EntityFrameworkCore;
using DocinadeApp.Data;
using DocinadeApp.Interfaces;
using DocinadeApp.Models;

namespace DocinadeApp.Extensions
{
    /// <summary>
    /// Extension methods for DbContext to handle SQL Server execution strategy properly
    /// </summary>
    public static class DbContextExtensions
    {
        /// <summary>
        /// Executes an operation with proper retry strategy handling for SQL Server
        /// </summary>
        public static async Task<T> ExecuteWithStrategyAsync<T>(this RubricasDbContext context, Func<Task<T>> operation)
        {
            var strategy = context.Database.CreateExecutionStrategy();
            return await strategy.ExecuteAsync(operation);
        }

        /// <summary>
        /// Executes an operation with proper retry strategy handling for SQL Server (no return value)
        /// </summary>
        public static async Task ExecuteWithStrategyAsync(this RubricasDbContext context, Func<Task> operation)
        {
            var strategy = context.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(operation);
        }

        /// <summary>
        /// Executes a transaction with proper retry strategy handling for SQL Server
        /// </summary>
        public static async Task<T> ExecuteInTransactionAsync<T>(this RubricasDbContext context, Func<Task<T>> operation)
        {
            var strategy = context.Database.CreateExecutionStrategy();
            return await strategy.ExecuteAsync(async () =>
            {
                using var transaction = await context.Database.BeginTransactionAsync();
                try
                {
                    var result = await operation();
                    await transaction.CommitAsync();
                    return result;
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            });
        }

        /// <summary>
        /// Executes a transaction with proper retry strategy handling for SQL Server (no return value)
        /// </summary>
        public static async Task ExecuteInTransactionAsync(this RubricasDbContext context, Func<Task> operation)
        {
            var strategy = context.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                using var transaction = await context.Database.BeginTransactionAsync();
                try
                {
                    await operation();
                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            });
        }

        /// <summary>
        /// Aplica filtros de usuario a las consultas
        /// </summary>
        public static IQueryable<T> ForUser<T>(this IQueryable<T> query, string userId)
            where T : class, IUserOwned
        {
            return query.Where(x => x.CreadoPorId == userId);
        }

        /// <summary>
        /// Aplica filtros de usuario para rúbricas
        /// </summary>
        public static IQueryable<Rubrica> ForCurrentUser(this IQueryable<Rubrica> query, string userId)
        {
            return query.Where(r => r.CreadoPorId == userId || r.EsPublica == 1);
        }

        /// <summary>
        /// Aplica filtros de usuario para rúbricas editables
        /// </summary>
        public static IQueryable<Rubrica> EditableByUser(this IQueryable<Rubrica> query, string userId)
        {
            return query.Where(r => r.CreadoPorId == userId);
        }

        /// <summary>
        /// Aplica auditoría automática al guardar
        /// </summary>
        public static void ApplyAuditInfo(this DbContext context, string userId)
        {
            var entries = context.ChangeTracker.Entries<IAuditable>();

            foreach (var entry in entries)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreadoPorId = userId;
                        entry.Entity.FechaCreacion = DateTime.Now;
                        break;
                    case EntityState.Modified:
                        entry.Entity.ModificadoPorId = userId;
                        entry.Entity.FechaModificacion = DateTime.Now;
                        break;
                }
            }
        }
    }
}