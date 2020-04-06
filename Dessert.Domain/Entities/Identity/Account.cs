using Microsoft.AspNetCore.Identity;

namespace Dessert.Domain.Entities.Identity
{
    public class Account : IdentityUser<long>
    {
        public string ProfilePicUrl { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
