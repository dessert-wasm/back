using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dessert.Domain.Entities;
using Dessert.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Dessert.DataLoaders
{
    public static class AuthTokensByAccountId
    {
        public const string Name = nameof(AuthTokensByAccountId);

        public static async Task<ILookup<long, AuthToken>> GetAuthTokensByAccountId(
            this ApplicationDbContext applicationDbContext,
            IReadOnlyList<long> keys,
            CancellationToken cancellationToken)
        {
            var authTokens = await applicationDbContext.AuthTokens
                .AsNoTracking()
                .Where(x => keys.Contains(x.AccountId))
                .ToListAsync(cancellationToken);

            return authTokens
                .ToLookup(x => x.AccountId);
        }
    }
}