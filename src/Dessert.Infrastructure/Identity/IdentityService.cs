using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dessert.Application.Interfaces;
using Dessert.Domain.Entities.Identity;
using Dessert.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Dessert.Infrastructure.Identity
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public IdentityService(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext applicationDbContext,
            IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _applicationDbContext = applicationDbContext;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IUser> GetUserFromId(long userId)
        {
            return await _userManager.FindByIdAsync(userId.ToString());
        }

        public async Task<IUser> Login(string email, string password, bool rememberMe)
        {
            var account = await _signInManager.UserManager.FindByEmailAsync(email);

            if (account == null)
                throw new Exception("invalid credentials");

            var passwordResult = await _signInManager.CheckPasswordSignInAsync(account, password, false);
            if (!passwordResult.Succeeded)
                throw new Exception("invalid password");

            var userPrincipal = await _signInManager.CreateUserPrincipalAsync(account);
            await _httpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                userPrincipal,
                new AuthenticationProperties
                {
                    IsPersistent = rememberMe,
                });
            return account;
        }

        public async Task Logout()
        {
            await _signInManager.Context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }

        public async Task<IUser> CreateUser(string email, string nickname, string password)
        {
            var account = new ApplicationUser()
            {
                UserName = email,
                Email = email,
                Nickname = nickname,
                ProfilePicUrl = "https://i.imgur.com/JrLLeco.jpg"
            };

            var result = await _userManager.CreateAsync(account, password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(x => x.Description));
                throw new Exception($"cannot create user, {errors}");
            }

            return account;
        }

        public Task Delete(long userId)
        {
            return Task.CompletedTask;
        }

        public async Task<IUser> UpdateAccount(long userId, string nickname, string profilePicUrl)
        {
            var account = await _userManager.FindByIdAsync(userId.ToString());

            account.Nickname = nickname;
            account.ProfilePicUrl = profilePicUrl;

            var result = await _userManager.UpdateAsync(account);
            if (!result.Succeeded)
                throw new Exception("Failed to update account");

            return account;
        }

        public async Task<IReadOnlyDictionary<long, ApplicationUser>> GetUserFromBatch(IReadOnlyList<long> keys,
            CancellationToken cancellationToken)
        {
            return await _applicationDbContext.Users
                .AsNoTracking()
                .Where(x => keys.Contains(x.Id))
                .ToDictionaryAsync(x => x.Id, cancellationToken);
        }
    }
}