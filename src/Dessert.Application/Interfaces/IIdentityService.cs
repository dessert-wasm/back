using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dessert.Domain.Entities.Identity;

namespace Dessert.Application.Interfaces
{
    public interface IIdentityService
    {
        Task<IUser> GetUserFromId(long userId);
        Task<IUser> Login(string email, string password, bool rememberMe);
        Task Logout();
        Task<IUser> CreateUser(string email, string nickname, string password);
        Task Delete(long userId);
        Task<IUser> UpdateAccount(long userId, string nickname, string profilePicUrl);
        Task<IReadOnlyDictionary<long, ApplicationUser>> GetUserFromBatch(IReadOnlyList<long> keys, CancellationToken cancellationToken);
    }
}