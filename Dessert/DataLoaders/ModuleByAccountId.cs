using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dessert.Domain.Entities;
using Dessert.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Dessert.DataLoaders
{
    public static class ModuleByAccountId
    {
        public const string Name = nameof(ModuleByAccountId);

        public static async Task<ILookup<long, Module>> GetModuleByAccountId(
            this ApplicationDbContext applicationDbContext,
            IReadOnlyList<long> keys,
            CancellationToken cancellationToken)
        {
            var moduleTags = await applicationDbContext.Modules
                .AsNoTracking()
                .Where(x => keys.Contains(x.AuthorId))
                .ToListAsync(cancellationToken);

            return moduleTags
                .ToLookup(x => x.AuthorId);
        }
    }
}