using System;
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
    public class TokenRepository : ITokenRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public TokenRepository(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task<string> CreateToken(string description, long userId)
        {

            var token = new AuthToken()
            {
                Description = description,
                AccountId = userId,
                Token = Guid.NewGuid().ToString(),
            };

            await _applicationDbContext.AuthTokens.AddAsync(token);
            await _applicationDbContext.SaveChangesAsync();

            return token.Token;
        }

        public async Task DeleteToken(string token)
        {
            AuthToken authToken = null;

            if (token != null)
                authToken = await _applicationDbContext.AuthTokens.FirstOrDefaultAsync(x => x.Token == token);

            if (authToken == null)
                return;

            _applicationDbContext.AuthTokens.Remove(authToken);
            await _applicationDbContext.SaveChangesAsync();
        }

        public async Task<ILookup<long, AuthToken>> GetTokensBatch(IReadOnlyList<long> keys,
            CancellationToken cancellationToken)
        {
            var authTokens = await _applicationDbContext.AuthTokens
                .AsNoTracking()
                .Where(x => keys.Contains(x.AccountId))
                .ToListAsync(cancellationToken);

            return authTokens
                .ToLookup(x => x.AccountId);
        }
    }
}