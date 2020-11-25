using System.Threading;
using System.Threading.Tasks;
using Dessert.Application.Interfaces;
using MediatR;

namespace Dessert.Application.Handlers.Account.Commands.Logout
{
    public class LogoutCommand : IRequest
    {
        public class LogoutCommandHandler : IRequestHandler<LogoutCommand>
        {
            private readonly IIdentityService _identityService;

            public LogoutCommandHandler(IIdentityService identityService)
            {
                _identityService = identityService;
            }

            public async Task<Unit> Handle(LogoutCommand request, CancellationToken cancellationToken)
            {
                await _identityService.Logout();
                return Unit.Value;
            }
        }
    }
}