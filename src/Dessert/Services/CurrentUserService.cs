using System;
using System.Security.Claims;
using Dessert.Application.Interfaces;
using Dessert.Domain.Entities.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Dessert.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<ApplicationUser> _userManager;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager)
        {
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }

        private long GetClaimValue(string claimType)
        {
            var claimValue = _httpContextAccessor.HttpContext.User.FindFirstValue(claimType);
            if (claimValue == null)
                throw new NullReferenceException(nameof(claimValue));
            if (!long.TryParse(claimValue, out var id))
                throw new Exception("invalid claim value");
            return id;
        }

        public string Email =>
            _httpContextAccessor.HttpContext.User.FindFirstValue(_userManager.Options.ClaimsIdentity.UserNameClaimType);

        public long UserId =>
            GetClaimValue(_userManager.Options.ClaimsIdentity.UserIdClaimType);
    }
}