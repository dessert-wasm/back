using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dessert.DataLoaders;
using Dessert.Models;
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
            return await signInManager.UserManager.GetUserAsync(signInManager.Context.User);
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
            return await context.BatchDataLoader<long, Account>(AccountById.Name, applicationDbContext.GetAccountById)
                .LoadAsync(id);
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
            [Service] ApplicationDbContext applicationDbContext)
        {
            var paginationQuery = context.GetPaginationQuery();

            query = $"%{query}%";

            var sqlQuery = applicationDbContext.Modules
                .AsNoTracking()
                .Where(x =>
                    EF.Functions.Like(x.Name, query) ||
                    EF.Functions.Like(x.Description, query))
                .OrderByDescending(x => x.LastUpdatedDateTime);

            return await Paginator.GetPaginatedResult(paginationQuery, sqlQuery);
        }
    }
}