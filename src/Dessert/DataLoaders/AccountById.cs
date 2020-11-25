using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dessert.Application.Interfaces;
using Dessert.Domain.Entities.Identity;
using HotChocolate.DataLoader;

namespace Dessert.DataLoaders
{
    public class AccountById : BatchDataLoader<long, ApplicationUser>
    {
        private readonly IIdentityService _identityService;

        public AccountById(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        protected override Task<IReadOnlyDictionary<long, ApplicationUser>> LoadBatchAsync(IReadOnlyList<long> keys,
            CancellationToken cancellationToken)
        {
            return _identityService.GetUserFromBatch(keys, cancellationToken);
        }
    }
}