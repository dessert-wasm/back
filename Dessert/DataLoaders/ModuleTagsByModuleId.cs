using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dessert.Domain.Entities;
using Dessert.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Dessert.DataLoaders
{
    public static class ModuleTagsByModuleId
    {
        public const string Name = nameof(ModuleTagsByModuleId);

        public static async Task<ILookup<long, ModuleTag>> GetModuleTagsByModuleId(
            this ApplicationDbContext applicationDbContext,
            IReadOnlyList<long> keys,
            CancellationToken cancellationToken)
        {
            var moduleTags = await applicationDbContext.ModuleModuleTagRelations
                .AsNoTracking()
                .Where(x => keys.Contains(x.ModuleId))
                .Include(x => x.ModuleTag)
                .ToListAsync(cancellationToken);

            return moduleTags
                .ToLookup(x => x.ModuleId, x => x.ModuleTag);
        }
    }
}