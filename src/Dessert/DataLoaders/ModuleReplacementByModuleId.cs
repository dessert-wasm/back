using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dessert.Domain.Entities;
using Dessert.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Dessert.DataLoaders
{
    public static class ModuleReplacementByModuleId
    {
        public const string Name = nameof(ModuleReplacementByModuleId);

        public static async Task<ILookup<long, ModuleReplacement>> GetModuleReplacementByModuleId(
            this ApplicationDbContext applicationDbContext,
            IReadOnlyList<long> keys,
            CancellationToken cancellationToken)
        {
            var moduleTags = await applicationDbContext.ModuleModuleReplacementRelations
                .AsNoTracking()
                .Where(x => keys.Contains(x.ModuleId))
                .Include(x => x.ModuleReplacement)
                .ToListAsync(cancellationToken);

            return moduleTags
                .ToLookup(x => x.ModuleId, x => x.ModuleReplacement);
        }
    }
}