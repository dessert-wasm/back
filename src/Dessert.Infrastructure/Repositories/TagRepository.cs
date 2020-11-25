using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dessert.Application.Repositories;
using Dessert.Domain.Entities;
using Dessert.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Dessert.Infrastructure.Repositories
{
    public class TagRepository : ITagRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public TagRepository(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }
        public async Task<IReadOnlyCollection<ModuleTag>> GetTags()
        {
            return await _applicationDbContext.ModuleTags
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<ILookup<long, ModuleTag>> GetTagsBatch(IReadOnlyList<long> keys, CancellationToken cancellationToken)
        {
            var moduleTags = await _applicationDbContext.ModuleModuleTagRelations
                .AsNoTracking()
                .Where(x => keys.Contains(x.ModuleId))
                .Include(x => x.ModuleTag)
                .ToListAsync(cancellationToken);

            return moduleTags
                .ToLookup(x => x.ModuleId, x => x.ModuleTag);
        }
    }
}