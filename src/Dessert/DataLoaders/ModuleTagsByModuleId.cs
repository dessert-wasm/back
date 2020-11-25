using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dessert.Application.Repositories;
using Dessert.Domain.Entities;
using HotChocolate.DataLoader;

namespace Dessert.DataLoaders
{
    public class ModuleTagsByModuleId : GroupedDataLoader<long, ModuleTag>
    {
        private readonly ITagRepository _tagRepository;

        public ModuleTagsByModuleId(ITagRepository tagRepository)
        {
            _tagRepository = tagRepository;
        }

        protected override Task<ILookup<long, ModuleTag>> LoadGroupedBatchAsync(IReadOnlyList<long> keys, CancellationToken cancellationToken)
        {
            return _tagRepository.GetTagsBatch(keys, cancellationToken);
        }
    }
}