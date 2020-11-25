using System.Threading;
using System.Threading.Tasks;
using Dessert.Application.Repositories;
using Dessert.Domain.Entities.Identity;
using MediatR;

namespace Dessert.Application.Handlers.Module.Queries.GetModule
{
    public class GetModuleQuery : IRequest<Domain.Entities.Module>
    {
        public long ModuleId { get; set; }
        
        public class GetModuleQueryHandler : IRequestHandler<GetModuleQuery, Domain.Entities.Module>
        {
            private readonly IModuleRepository _moduleRepository;

            public GetModuleQueryHandler(IModuleRepository moduleRepository)
            {
                _moduleRepository = moduleRepository;
            }

            public Task<Domain.Entities.Module> Handle(GetModuleQuery request, CancellationToken cancellationToken)
            {
                return _moduleRepository.GetModule(request.ModuleId);
            }
        }
    }
}