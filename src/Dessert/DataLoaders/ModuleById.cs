using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dessert.Application.Repositories;
using Dessert.Domain.Entities;
using HotChocolate.DataLoader;

namespace Dessert.DataLoaders
{
    public class ModuleById : BatchDataLoader<long, Module>
    {
        private readonly IModuleRepository _moduleRepository;

        public ModuleById(IModuleRepository moduleRepository)
        {
            _moduleRepository = moduleRepository;
        }

        protected override Task<IReadOnlyDictionary<long, Module>> LoadBatchAsync(IReadOnlyList<long> keys, CancellationToken cancellationToken)
        {
            return _moduleRepository.GetModuleBatch(keys, cancellationToken);
        }
    }
}