using System.Linq;
using Dessert.DataLoaders;
using Dessert.Domain.Entities;
using Dessert.Domain.Entities.Identity;
using Dessert.Persistence;
using Dessert.Types.Pagination;
using Dessert.Utilities;
using Dessert.Utilities.Pagination;
using GreenDonut;
using HotChocolate.Resolvers;
using HotChocolate.Types;
using Microsoft.EntityFrameworkCore;

namespace Dessert.Types
{
    public class AccountType : ObjectType<Account>
    {
        protected override void Configure(IObjectTypeDescriptor<Account> descriptor)
        {
            descriptor.BindFieldsExplicitly();

            descriptor.Field(f => f.Id)
                .Type<NonNullType<IntType>>();

            descriptor.Field("tokens")
                .Type<NonNullType<ListType<NonNullType<AuthTokenType>>>>()
                .Resolver(ctx =>
                {
                    var dataContext = ctx.Service<ApplicationDbContext>();
                    var account = ctx.Parent<Account>();

                    return ctx.GroupDataLoader<long, AuthToken>(AuthTokensByAccountId.Name,
                        dataContext.GetAuthTokensByAccountId).LoadAsync(account.Id);
                });

            descriptor.Field("modules")
                .Type<NonNullType<PaginatedResultType<Module>>>()
                .AddPaginationArgument()
                .Resolver(ctx =>
                {
                    var paginationQuery = ctx.GetPaginationQuery();

                    var dataContext = ctx.Service<ApplicationDbContext>();
                    var account = ctx.Parent<Account>();

                    var sqlQuery = dataContext.Modules
                        .AsNoTracking()
                        .Where(x => x.AuthorId == account.Id)
                        .OrderByDescending(x => x.LastUpdatedDateTime);

                    return Paginator.GetPaginatedResult(paginationQuery, sqlQuery);
                });

            descriptor.Field(f => f.ProfilePicUrl)
                .Type<NonNullType<UrlType>>();

            descriptor.Field(f => f.Nickname)
                .Type<NonNullType<StringType>>();
        }
    }
}
