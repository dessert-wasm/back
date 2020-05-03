using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dessert.Domain.Entities;
using Dessert.Domain.Entities.Identity;
using Dessert.Persistence;
using Dessert.Utilities;
using HotChocolate;
using HotChocolate.Resolvers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Dessert.GraphQL
{
    public class Mutation
    {
        private readonly ILogger<Mutation> _logger;

        public Mutation(ILogger<Mutation> logger)
        {
            _logger = logger;
        }

        public async Task<Account> Login(
            string email,
            string password,
            bool remember,
            [Service] SignInManager<Account> signInManager)
        {
            var account = await signInManager.UserManager.FindByEmailAsync(email);

            if (account == null)
                throw new Exception("invalid credentials");

            var passwordResult = await signInManager.CheckPasswordSignInAsync(account, password, false);
            if (!passwordResult.Succeeded)
                throw new Exception("invalid password");
            await signInManager.SignInAsync(account, remember, CookieAuthenticationDefaults.AuthenticationScheme);
            return account;
        }

        public async Task<Account> Register(
            IResolverContext context,
            string email,
            string nickname,
            string password,
            [Service] UserManager<Account> userManager,
            [Service] SignInManager<Account> signInManager)
        {
            if (signInManager.IsSignedIn(context.GetClaimsPrincipal()))
                throw new Exception("Already connected");

            var account = new Account()
            {
                UserName = email,
                Email = email,
                Nickname = nickname,
                ProfilePicUrl = "https://www.bbcgoodfood.com/sites/default/files/recipe-collections/collection-image/2018/09/dessert-main-image-molten-cake.jpg"
            };

            var result = await userManager.CreateAsync(account, password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(x => x.Description));
                throw new Exception($"cannot create user, {errors}");
            }

            return account;
        }

        public async Task<bool> Logout([Service] SignInManager<Account> signInManager)
        {
            await signInManager.Context.SignOutAsync(IdentityConstants.ApplicationScheme);
            return true;
        }
        
        public async Task<bool> DeleteCurrentUser(IResolverContext context,
            [Service] ApplicationDbContext applicationDbContext,
            [Service] SignInManager<Account> signInManager)
        {
            var user = await signInManager.UserManager.GetUserAsync(signInManager.Context.User);

            applicationDbContext.Users.Remove(user);
            await applicationDbContext.SaveChangesAsync();

            return true;
        }

        public async Task<string> CreateToken(IResolverContext context,
            [Service] UserManager<Account> userManager,
            [Service] ApplicationDbContext applicationDbContext,
            string description)
        {
            var account = await userManager.GetUserAsync(context.GetClaimsPrincipal());

            var token = new AuthToken()
            {
                Description = description,
                Account = account,
                Token = Guid.NewGuid().ToString(),
            };

            applicationDbContext.AuthTokens.Add(token);
            await applicationDbContext.SaveChangesAsync();

            return token.Token;
        }

        public async Task<bool> DeleteToken(IResolverContext context,
            [Service] ApplicationDbContext applicationDbContext,
            string token)
        {
            AuthToken authToken = null;

            if (token != null)
                authToken = await applicationDbContext.AuthTokens.FirstOrDefaultAsync(x => x.Token == token);

            if (authToken == null)
                return false;

            applicationDbContext.AuthTokens.Remove(authToken);
            await applicationDbContext.SaveChangesAsync();

            return true;
        }

        public async Task<Module> CreateModule(IResolverContext context,
            [Service] ApplicationDbContext applicationDbContext,
            string token,
            string name,
            string description,
            bool isCore)
        {
            var account = await GetAccountFromToken(applicationDbContext, token);
            var replacements = context.Argument<IEnumerable<ModuleReplacement>>("replacements");

            var module = new Module()
            {
                Name = name,
                Description = description,
                IsCore = isCore,
                PublishedDateTime = DateTime.Now,
                LastUpdatedDateTime = DateTime.Now,
                Author = account
            };
            applicationDbContext.Modules.Add(module);

            foreach (var r in replacements)
            {
                var moduleModuleReplacementRelation = new ModuleModuleReplacementRelation()
                {
                    Module = module,
                    ModuleReplacement = r,
                };
                applicationDbContext.ModuleModuleReplacementRelations.Add(moduleModuleReplacementRelation);
            }

            await applicationDbContext.SaveChangesAsync();

            return module;
        }

        public async Task<bool> DeleteModule(IResolverContext context, [Service] ApplicationDbContext applicationDbContext, string token, long id)
        {
            var account = await GetAccountFromToken(applicationDbContext, token);

            var module = await applicationDbContext.Modules.FirstOrDefaultAsync(x => x.Id == id);

            //todo custom error depending on the reason why it failed ?
            if (module == null)
                return false;
            if (module.Author != account)
                return false;

            applicationDbContext.Modules.Remove(module);
            await applicationDbContext.SaveChangesAsync();

            return true;
        }

        public async Task<Account> UpdateUser(IResolverContext context,
            [Service] UserManager<Account> userManager)
        {
            var input = context.Argument<Account>("account");
            var account = await userManager.GetUserAsync(context.GetClaimsPrincipal());

            account.UserName = input.UserName;
            account.ProfilePicUrl = input.ProfilePicUrl;

            var result = await userManager.UpdateAsync(account);
            if (!result.Succeeded)
                throw new Exception("Failed to update account");

            return account;
        }

        private static async Task<Account> GetAccountFromToken(ApplicationDbContext applicationDbContext, string token)
        {
            var authToken = await applicationDbContext.AuthTokens
                .Include(x => x.Account)
                .FirstOrDefaultAsync(x => x.Token == token);

            if (authToken == null)
                throw new Exception("invalid token");

            return authToken.Account;
        }
    }
}
