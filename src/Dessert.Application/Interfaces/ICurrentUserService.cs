namespace Dessert.Application.Interfaces
{
    public interface ICurrentUserService
    {
        string Email { get; }
        long UserId { get; }
    }
}