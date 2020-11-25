using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dessert.Application.Repositories;
using Dessert.Domain.Entities;
using HotChocolate.DataLoader;

namespace Dessert.DataLoaders
{
    public class ModuleReplacementByModuleId : GroupedDataLoader<long, ModuleReplacement>
    {
        private readonly IModuleRepository _moduleRepository;

        public ModuleReplacementByModuleId(IModuleRepository moduleRepository)
        {
            _moduleRepository = moduleRepository;
        }

        protected override Task<ILookup<long, ModuleReplacement>> LoadGroupedBatchAsync(IReadOnlyList<long> keys, CancellationToken cancellationToken)
        {
            return _moduleRepository.GetReplacementsBatch(keys, cancellationToken);
        }
    }
}