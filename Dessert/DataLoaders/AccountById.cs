using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dessert.Domain.Entities;
using Dessert.Domain.Entities.Identity;
using Dessert.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Dessert.DataLoaders
{
    public static class AccountById
    {
        public const string Name = nameof(AccountById);

        public static async Task<IReadOnlyDictionary<long, Account>> GetAccountById(
            this ApplicationDbContext applicationDbContext,
            IReadOnlyList<long> keys,
            CancellationToken cancellationToken)
        {
            return await applicationDbContext.Users
                .AsNoTracking()
                .Where(x => keys.Contains(x.Id))
                .ToDictionaryAsync(x => x.Id, cancellationToken);
        }
    }
}