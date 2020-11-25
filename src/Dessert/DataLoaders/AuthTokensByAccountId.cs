using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dessert.Application.Repositories;
using Dessert.Domain.Entities;
using HotChocolate.DataLoader;

namespace Dessert.DataLoaders
{
    public class AuthTokensByAccountId : GroupedDataLoader<long, AuthToken>
    {
        private readonly ITokenRepository _tokenRepository;

        public AuthTokensByAccountId(ITokenRepository tokenRepository)
        {
            _tokenRepository = tokenRepository;
        }

        protected override Task<ILookup<long, AuthToken>> LoadGroupedBatchAsync(IReadOnlyList<long> keys,
            CancellationToken cancellationToken)
        {
            return _tokenRepository.GetTokensBatch(keys, cancellationToken);
        }
    }
}