using System.Threading;
using System.Threading.Tasks;
using Dessert.Application.Interfaces;
using Dessert.Domain.Entities.Identity;
using MediatR;

namespace Dessert.Application.Handlers.Account.Queries.GetMyAccount
{
    public class GetMyAccountQuery : IRequest<IUser>
    {
        public class GetMyAccountQueryHandler : IRequestHandler<GetMyAccountQuery, IUser>
        {
            private readonly ICurrentUserService _currentUserService;
            private readonly IIdentityService _identityService;

            public GetMyAccountQueryHandler(IIdentityService identityService, ICurrentUserService currentUserService)
            {
                _identityService = identityService;
                _currentUserService = currentUserService;
            }

            public Task<IUser> Handle(GetMyAccountQuery request, CancellationToken cancellationToken)
            {
                return _identityService.GetUserFromId(_currentUserService.UserId);
            }
        }
    }
}