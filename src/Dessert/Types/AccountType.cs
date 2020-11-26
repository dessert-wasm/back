using Dessert.Application.Repositories;
using Dessert.DataLoaders;
using Dessert.Domain.Entities;
using Dessert.Domain.Entities.Identity;
using Dessert.Infrastructure.Repositories;
using Dessert.Types.Pagination;
using Dessert.Utilities;
using GreenDonut;
using HotChocolate.Resolvers;
using HotChocolate.Types;

namespace Dessert.Types
{
    public class AccountType : ObjectType<IUser>
    {
        protected override void Configure(IObjectTypeDescriptor<IUser> descriptor)
        {
            descriptor.BindFieldsExplicitly();

            descriptor.Name("Account");

            descriptor.Field(f => f.Id)
                .Type<NonNullType<IntType>>();

            descriptor.Field("tokens")
                .Type<NonNullType<ListType<NonNullType<AuthTokenType>>>>()
                .Resolver(ctx =>
                {
                    var dataContext = ctx.DataLoader<AuthTokensByAccountId>();
                    var account = ctx.Parent<IUser>();

                    return dataContext.LoadAsync(account.Id);
                });

            descriptor.Field("modules")
                .Type<NonNullType<PaginatedResultType<Module>>>()
                .AddPaginationArgument()
                .Resolver(ctx =>
                {
                    var paginationQuery = ctx.GetPaginationQuery();

                    var moduleRepository = ctx.Service<IModuleRepository>();
                    var account = ctx.Parent<IUser>();

                    return moduleRepository.GetModulesByAuthorId(account.Id, paginationQuery);
                });

            descriptor.Field(f => f.ProfilePicUrl)
                .Type<NonNullType<UrlType>>();

            descriptor.Field(f => f.Nickname)
                .Type<NonNullType<StringType>>();

            descriptor.Field(f => f.Email)
                .Type<NonNullType<StringType>>();
        }
    }
}
