using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dessert.Domain.Pagination;
using Microsoft.EntityFrameworkCore;

namespace Dessert.Infrastructure.Pagination
{
    public static class Paginator
    {
        public static Task<PaginatedResult<TQuery>> GetPaginatedResult<TQuery>(
            PaginationQuery paginationQuery,
            IQueryable<TQuery> query)
        {
            return GetPaginatedResult(paginationQuery, query, entry => entry);
        }

        public static async Task<PaginatedResult<TOut>> GetPaginatedResult<TQuery, TOut>(
            PaginationQuery paginationQuery,
            IQueryable<TQuery> query,
            Func<IReadOnlyCollection<TQuery>, IEnumerable<TOut>> transform)
        {
            int? totalRecord = null;
            if (paginationQuery.IncludeCount)
                totalRecord = await query.CountAsync();

            var result = await query
                .Skip((int)paginationQuery.ToSkip)
                .Take((int)paginationQuery.PageSize)
                .ToArrayAsync();

            return new PaginatedResult<TOut>(paginationQuery, totalRecord, transform(result).ToArray());
        }
    }
}