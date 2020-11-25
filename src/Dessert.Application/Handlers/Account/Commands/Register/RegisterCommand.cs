using System.Threading;
using System.Threading.Tasks;
using Dessert.Application.Interfaces;
using Dessert.Domain.Entities.Identity;
using MediatR;

namespace Dessert.Application.Handlers.Account.Commands.Register
{
    public class RegisterCommand : IRequest<IUser>
    {
        public string Email { get; set; }
        public string Nickname { get; set; }
        public string Password { get; set; }

        public class RegisterCommandHandler : IRequestHandler<RegisterCommand, IUser>
        {
            private readonly IIdentityService _identityService;

            public RegisterCommandHandler(IIdentityService identityService)
            {
                _identityService = identityService;
            }

            public Task<IUser> Handle(RegisterCommand request, CancellationToken cancellationToken)
            {
                return _identityService.CreateUser(request.Email, request.Nickname, request.Password);
            }
        }
    }
}