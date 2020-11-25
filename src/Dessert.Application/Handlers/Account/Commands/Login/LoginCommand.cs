using System.Threading;
using System.Threading.Tasks;
using Dessert.Application.Interfaces;
using Dessert.Domain.Entities.Identity;
using MediatR;

namespace Dessert.Application.Handlers.Account.Commands.Login
{
    public class LoginCommand : IRequest<IUser>
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public bool RememberMe { get; set; }

        public class LoginCommandHandler : IRequestHandler<LoginCommand, IUser>
        {
            private readonly IIdentityService _identityService;

            public LoginCommandHandler(IIdentityService identityService)
            {
                _identityService = identityService;
            }

            public Task<IUser> Handle(LoginCommand request, CancellationToken cancellationToken)
            {
                return _identityService.Login(request.Email, request.Password, request.RememberMe);
            }
        }
    }
}