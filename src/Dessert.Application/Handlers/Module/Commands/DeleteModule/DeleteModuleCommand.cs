using System.Threading;
using System.Threading.Tasks;
using Dessert.Application.Repositories;
using MediatR;

namespace Dessert.Application.Handlers.Module.Commands.DeleteModule
{
    public class DeleteModuleCommand : IRequest
    {
        public string Token { get; set; }
        public long ModuleId { get; set; }
        
        public class DeleteModuleCommandHandler : IRequestHandler<DeleteModuleCommand>
        {
            private readonly IModuleRepository _moduleRepository;

            public DeleteModuleCommandHandler(IModuleRepository moduleRepository)
            {
                _moduleRepository = moduleRepository;
            }

            public async Task<Unit> Handle(DeleteModuleCommand request, CancellationToken cancellationToken)
            {
                await _moduleRepository.DeleteModule(request.Token, request.ModuleId);
                return Unit.Value;
            }
        }
    }
}