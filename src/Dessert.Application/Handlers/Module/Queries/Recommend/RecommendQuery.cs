using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dessert.Application.Repositories;
using Dessert.Domain.Entities;
using MediatR;

namespace Dessert.Application.Handlers.Module.Queries.Recommend
{
    using ResultType = IReadOnlyCollection<Domain.Entities.Module>;
    public class RecommendQuery : IRequest<ResultType>
    {
        public IReadOnlyCollection<JSDependency> Dependencies { get; set; }
        
        public class RecommendQueryHandler : IRequestHandler<RecommendQuery, ResultType>
        {
            private readonly IModuleRepository _moduleRepository;

            public RecommendQueryHandler(IModuleRepository moduleRepository)
            {
                _moduleRepository = moduleRepository;
            }

            public Task<ResultType> Handle(RecommendQuery request, CancellationToken cancellationToken)
            {
                return _moduleRepository.Recommend(request.Dependencies);
            }
        }
    }
}