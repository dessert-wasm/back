using Microsoft.AspNetCore.Identity;

namespace Dessert.Domain.Entities.Identity
{
    public class ApplicationUser : IdentityUser<long>, IUser
    {
        public string ProfilePicUrl { get; set; }
        public string Nickname { get; set; }
    }

    public interface IUser
    {
        long Id { get; }
        
        string Email { get; }
        
        string Nickname { get; }
        
        string ProfilePicUrl { get; }
    }
}
