using System.Threading;
using System.Threading.Tasks;
using Dessert.Application.Interfaces;
using Dessert.Domain.Entities.Identity;
using MediatR;

namespace Dessert.Application.Handlers.Account.Queries.GetAccount
{
    public class GetAccountQuery : IRequest<IUser>
    {
        public long UserId { get; set; }
        
        public class GetAccountQueryHandler : IRequestHandler<GetAccountQuery, IUser>
        {
            private readonly IIdentityService _identityService;

            public GetAccountQueryHandler(IIdentityService identityService)
            {
                _identityService = identityService;
            }

            public Task<IUser> Handle(GetAccountQuery request, CancellationToken cancellationToken)
            {
                return _identityService.GetUserFromId(request.UserId);
            }
        }
    }
}