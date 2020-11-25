using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dessert.Application.Repositories;
using Dessert.Domain.Enums;
using Dessert.Domain.Pagination;
using MediatR;

namespace Dessert.Application.Handlers.Module.Queries.SearchModules
{
    using ResultType = PaginatedResult<Domain.Entities.Module>;
    public class SearchModulesQuery : IRequest<ResultType>
    {
        public string Query { get; set; }
        public ModuleTypeEnum? Type { get; set; }
        
        public PaginationQuery PaginationQuery { get; set; }

        public class SearchModulesQueryHandler : IRequestHandler<SearchModulesQuery, ResultType>
        {
            private readonly IModuleRepository _moduleRepository;

            public SearchModulesQueryHandler(IModuleRepository moduleRepository)
            {
                _moduleRepository = moduleRepository;
            }

            public Task<ResultType> Handle(SearchModulesQuery request, CancellationToken cancellationToken)
            {
                return _moduleRepository.SearchModules(request.Query, request.Type, request.PaginationQuery);
            }
        }
    }
}