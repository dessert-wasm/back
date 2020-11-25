using System.Threading;
using System.Threading.Tasks;
using Dessert.Application.Interfaces;
using Dessert.Application.Repositories;
using MediatR;

namespace Dessert.Application.Handlers.Token.Commands.CreateToken
{
    public class CreateTokenCommand : IRequest<string>
    {
        public string Description { get; set; }
        
        public class CreateTokenCommandHandler : IRequestHandler<CreateTokenCommand, string>
        {
            private readonly ITokenRepository _tokenRepository;
            private readonly ICurrentUserService _currentUserService;

            public CreateTokenCommandHandler(ITokenRepository tokenRepository, ICurrentUserService currentUserService)
            {
                _tokenRepository = tokenRepository;
                _currentUserService = currentUserService;
            }

            public Task<string> Handle(CreateTokenCommand request, CancellationToken cancellationToken)
            {
                return _tokenRepository.CreateToken(request.Description, _currentUserService.UserId);
            }
        }
    }
}