using Microsoft.AspNetCore.Authorization;

namespace Dessert.Authorization
{
    public class Policies
    {
        public static void RequireAdministrator(AuthorizationPolicyBuilder policy)
        {
            policy
                .RequireAuthenticatedUser()
                .RequireRole(RoleConstants.Administrator);
        }
    }
}