using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dessert.Domain.Entities;
using Dessert.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Dessert.DataLoaders
{
    public static class ModuleById
    {
        public const string Name = nameof(ModuleById);

        public static async Task<IReadOnlyDictionary<long, Module>> GetModuleById(
            this ApplicationDbContext applicationDbContext,
            IReadOnlyList<long> keys,
            CancellationToken cancellationToken)
        {
            return await applicationDbContext.Modules
                .AsNoTracking()
                .Where(x => keys.Contains(x.Id))
                .ToDictionaryAsync(x => x.Id, cancellationToken);
        }
    }
}