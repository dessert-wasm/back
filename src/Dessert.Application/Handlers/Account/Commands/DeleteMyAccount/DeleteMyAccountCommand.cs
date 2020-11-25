using System.Threading;
using System.Threading.Tasks;
using Dessert.Application.Interfaces;
using MediatR;

namespace Dessert.Application.Handlers.Account.Commands.DeleteMyAccount
{
    public class DeleteMyAccountCommand : IRequest
    {
        public class DeleteMyAccountCommandHandler : IRequestHandler<DeleteMyAccountCommand>
        {
            private readonly ICurrentUserService _currentUserService;
            private readonly IIdentityService _identityService;

            public DeleteMyAccountCommandHandler(IIdentityService identityService, ICurrentUserService currentUserService)
            {
                _currentUserService = currentUserService;
                _identityService = identityService;
            }

            public async Task<Unit> Handle(DeleteMyAccountCommand request, CancellationToken cancellationToken)
            {
                await _identityService.Delete(_currentUserService.UserId);
                return Unit.Value;
            }
        }
    }
}