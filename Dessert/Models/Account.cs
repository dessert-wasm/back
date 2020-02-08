using Microsoft.AspNetCore.Identity;

namespace Dessert.Models
{
    public class Account : IdentityUser<long>
    {
        public string ProfilePicUrl { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
