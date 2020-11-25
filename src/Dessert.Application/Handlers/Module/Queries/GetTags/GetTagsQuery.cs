using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dessert.Application.Repositories;
using Dessert.Domain.Entities;
using MediatR;

namespace Dessert.Application.Handlers.Module.Queries.GetTags
{
    using ResultType = IReadOnlyCollection<ModuleTag>;
    public class GetTagsQuery : IRequest<ResultType>
    {
        public class GetTagsQueryHandler : IRequestHandler<GetTagsQuery, ResultType>
        {
            private readonly ITagRepository _tagRepository;

            public GetTagsQueryHandler(ITagRepository tagRepository)
            {
                _tagRepository = tagRepository;
            }

            public Task<ResultType> Handle(GetTagsQuery request, CancellationToken cancellationToken)
            {
                return _tagRepository.GetTags();
            }
        }
    }
}