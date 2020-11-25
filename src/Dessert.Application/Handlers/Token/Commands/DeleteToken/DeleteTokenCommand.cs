using System.Threading;
using System.Threading.Tasks;
using Dessert.Application.Interfaces;
using Dessert.Application.Repositories;
using MediatR;

namespace Dessert.Application.Handlers.Token.Commands.DeleteToken
{
    public class DeleteTokenCommand : IRequest
    {
        public string Token { get; set; }
        
        public class DeleteTokenCommandHandler : IRequestHandler<DeleteTokenCommand>
        {
            private readonly ITokenRepository _tokenRepository;

            public DeleteTokenCommandHandler(ITokenRepository tokenRepository)
            {
                _tokenRepository = tokenRepository;
            }

            public async Task<Unit> Handle(DeleteTokenCommand request, CancellationToken cancellationToken)
            {
                await _tokenRepository.DeleteToken(request.Token);
                return Unit.Value;
            }
        }
    }
}