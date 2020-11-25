using Dessert.Domain.Entities.Identity;

namespace Dessert.Domain.Entities
{
    public class AuthToken
    {
        public long Id { get; set; }
        public string Token { get; set; }
        public string Description { get; set; }
        public long AccountId { get; set; }

        public ApplicationUser Account { get; set; }
    }
}