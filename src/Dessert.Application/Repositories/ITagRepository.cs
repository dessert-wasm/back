using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dessert.Domain.Entities;

namespace Dessert.Application.Repositories
{
    public interface ITagRepository
    {
        Task<IReadOnlyCollection<ModuleTag>> GetTags();
        Task<ILookup<long, ModuleTag>> GetTagsBatch(IReadOnlyList<long> keys, CancellationToken cancellationToken);
    }
}