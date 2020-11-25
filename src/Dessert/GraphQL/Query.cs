using System.Collections.Generic;
using System.Threading.Tasks;
using Dessert.Application.Handlers.Account.Queries.GetAccount;
using Dessert.Application.Handlers.Account.Queries.GetMyAccount;
using Dessert.Application.Handlers.Module.Queries.GetModule;
using Dessert.Application.Handlers.Module.Queries.GetTags;
using Dessert.Application.Handlers.Module.Queries.Recommend;
using Dessert.Application.Handlers.Module.Queries.SearchModules;
using Dessert.Domain.Entities;
using Dessert.Domain.Entities.Identity;
using Dessert.Domain.Enums;
using Dessert.Domain.Pagination;
using Dessert.Utilities;
using HotChocolate;
using HotChocolate.Resolvers;
using MediatR;

namespace Dessert.GraphQL
{
    public class Query
    {
        public Task<IUser> Me([Service] IMediator mediator)
        {
            return mediator.Send(new GetMyAccountQuery());
        }

        public Task<Module> Module(long id, [Service] IMediator mediator)
        {
            return mediator.Send(new GetModuleQuery
            {
                ModuleId = id,
            });
        }

        public Task<IUser> User(long id, [Service] IMediator mediator)
        {
            return mediator.Send(new GetAccountQuery
            {
                UserId = id,
            });
        }

        public Task<IReadOnlyCollection<ModuleTag>> Tags([Service] IMediator mediator)
        {
            return mediator.Send(new GetTagsQuery());
        }

        public Task<PaginatedResult<Module>> Search([Service] IResolverContext context,
            string query,
            ModuleTypeEnum? type,
            [Service] IMediator mediator)
        {
            var paginationQuery = context.GetPaginationQuery();

            return mediator.Send(new SearchModulesQuery
            {
                Query = query,
                Type = type,
                PaginationQuery = paginationQuery,
            });
        }

        public Task<IReadOnlyCollection<Module>> Recommend(IReadOnlyCollection<JSDependency> dependencies,
            [Service] IMediator mediator)
        {
            return mediator.Send(new RecommendQuery
            {
                Dependencies = dependencies,
            });
        }
    }
}