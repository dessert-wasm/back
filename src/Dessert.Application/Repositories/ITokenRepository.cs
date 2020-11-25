using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dessert.Domain.Entities;
using MediatR;

namespace Dessert.Application.Repositories
{
    public interface ITokenRepository
    {
        Task<string> CreateToken(string requestDescription, long userId);
        Task DeleteToken(string token);
        Task<ILookup<long, AuthToken>> GetTokensBatch(IReadOnlyList<long> keys, CancellationToken cancellationToken);
    }
}