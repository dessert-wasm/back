using Microsoft.AspNetCore.Identity;

namespace Dessert.Models
{
    public class AccountRole : IdentityRole<long>
    {
        public AccountRole()
        {
        }

        public AccountRole(string roleName)
            : base(roleName)
        {
        }
    }
}