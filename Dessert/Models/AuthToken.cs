namespace Dessert.Models
{
    public class AuthToken
    {
        public long Id { get; set; }
        public string Token { get; set; }
        public string Description { get; set; }
        public long AccountId { get; set; }


        public Account Account { get; set; }
    }
}