using System.Linq.Expressions;

namespace DocinadeApp.Extensions
{
    // TODO: Filtro cascada - Extensi�n para aplicar filtros condicionales
    public static class QueryableExtensions
    {
        public static IQueryable<T> WhereIf<T>(this IQueryable<T> query, bool condition, Expression<Func<T, bool>> predicate)
        {
            return condition ? query.Where(predicate) : query;
        }
    }
}