using Microsoft.AspNetCore.Identity;

namespace Dessert.Domain.Entities.Identity
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