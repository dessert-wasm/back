using System.Threading;
using System.Threading.Tasks;
using Dessert.Application.Interfaces;
using Dessert.Domain.Entities.Identity;
using MediatR;

namespace Dessert.Application.Handlers.Account.Commands.UpdateMyAccount
{
    public class UpdateMyAccountCommand : IRequest<IUser>
    {
        public string Nickname { get; set; }
        
        public string ProfilePicUrl { get; set; }

        public class UpdateMyAccountCommandHandler : IRequestHandler<UpdateMyAccountCommand, IUser>
        {
            private readonly IIdentityService _identityService;
            private readonly ICurrentUserService _currentUserService;

            public UpdateMyAccountCommandHandler(IIdentityService identityService, ICurrentUserService currentUserService)
            {
                _identityService = identityService;
                _currentUserService = currentUserService;
            }

            public Task<IUser> Handle(UpdateMyAccountCommand request, CancellationToken cancellationToken)
            {
                return _identityService.UpdateAccount(_currentUserService.UserId, request.Nickname, request.ProfilePicUrl);
            }
        }
    }
}