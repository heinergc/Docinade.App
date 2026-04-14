using System.Linq.Expressions;

namespace RubricasApp.Web.Extensions
{
    // TODO: Filtro cascada - Extensión para aplicar filtros condicionales
    public static class QueryableExtensions
    {
        public static IQueryable<T> WhereIf<T>(this IQueryable<T> query, bool condition, Expression<Func<T, bool>> predicate)
        {
            return condition ? query.Where(predicate) : query;
        }
    }
}