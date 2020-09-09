using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Dessert.DataLoaders;
using Dessert.Domain.Entities;
using Dessert.Domain.Entities.Identity;
using Dessert.Domain.Enums;
using Dessert.Persistence;
using Dessert.Types.Arguments;
using Dessert.Utilities;
using Dessert.Utilities.Pagination;
using HotChocolate;
using GreenDonut;
using HotChocolate.Resolvers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Dessert.GraphQL
{
    public class Query
    {
        public async Task<Account> Me(IResolverContext context, [Service] SignInManager<Account> signInManager)
        {
            var email = signInManager.Context.User.GetEmail();
            var acc = await signInManager.UserManager.FindByEmailAsync(email);
            return acc;
            //return await signInManager.UserManager.GetUserAsync(signInManager.Context.User);
        }

        public async Task<Module> Module(IResolverContext context,
            long id,
            [Service] ApplicationDbContext applicationDbContext)
        {
            return await context.BatchDataLoader<long, Module>(ModuleById.Name, applicationDbContext.GetModuleById)
                .LoadAsync(id);
        }

        public async Task<Account> User(IResolverContext context,
            long id,
            [Service] ApplicationDbContext applicationDbContext)
        {
            return await applicationDbContext.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<ModuleTag>> Tags(IResolverContext context,
            [Service] ApplicationDbContext applicationDbContext)
        {
            return await applicationDbContext.ModuleTags
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<PaginatedResult<Module>> Search(IResolverContext context,
            string query,
            ModuleTypeEnum? type,
            [Service] ApplicationDbContext applicationDbContext)
        {
            var paginationQuery = context.GetPaginationQuery();

            var sqlQuery = applicationDbContext.Modules
                .AsNoTracking()
                .Where(x =>
                    EF.Functions.TrigramsAreSimilar(query, x.Name));

            if (type.HasValue)
            {
                sqlQuery = sqlQuery
                    .Where(x => x.IsCore == (type.Value == ModuleTypeEnum.Core));
            }

            var orderedQuery = sqlQuery.OrderByDescending(x => x.LastUpdatedDateTime);

            return await Paginator.GetPaginatedResult(paginationQuery, orderedQuery);
        }

        public async Task<IReadOnlyCollection<Module>> Recommend(IResolverContext context,
            IReadOnlyCollection<JSDependency> dependencies,
            [Service] ApplicationDbContext applicationDbContext)
        {
            var dependencyNames = dependencies.Select(x => x.Name);

            var replacements = await applicationDbContext.ModuleModuleReplacementRelations
                .Where(x => dependencyNames.Contains(x.ModuleReplacement.Name))
                .Select(x => x.Module)
                .Distinct()
                .ToArrayAsync();
            return replacements;
        }
    }
}
