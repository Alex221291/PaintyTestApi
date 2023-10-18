using System.Linq.Expressions;

namespace PaintyTestApi.Extensions;

public static class CusomomSorting
{
    public static IQueryable<T> OrderByPropertyName<T>(this IQueryable<T> q, string SortField, bool Ascending)
    {
        var param = Expression.Parameter(typeof(T), "p");
        var prop = Expression.Property(param, SortField);
        var exp = Expression.Lambda(prop, param);
        var method = Ascending ? "OrderBy" : "OrderByDescending";
        var types = new[] { q.ElementType, exp.Body.Type };
        var rs = Expression.Call(typeof(Queryable), method, types, q.Expression, exp);
        return q.Provider.CreateQuery<T>(rs);
    }
}