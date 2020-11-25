using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dessert.Application.Handlers.Account.Commands.DeleteMyAccount;
using Dessert.Application.Handlers.Account.Commands.Login;
using Dessert.Application.Handlers.Account.Commands.Logout;
using Dessert.Application.Handlers.Account.Commands.Register;
using Dessert.Application.Handlers.Account.Commands.UpdateMyAccount;
using Dessert.Application.Handlers.Module.Commands.CreateModule;
using Dessert.Application.Handlers.Module.Commands.DeleteModule;
using Dessert.Application.Handlers.Token.Commands.CreateToken;
using Dessert.Application.Handlers.Token.Commands.DeleteToken;
using Dessert.Domain.Entities;
using Dessert.Domain.Entities.Identity;
using HotChocolate;
using HotChocolate.Resolvers;
using MediatR;

namespace Dessert.GraphQL
{
    public class Mutation
    {
        public Task<IUser> Login(string email, string password, bool remember, [Service] IMediator mediator)
        {
            return mediator.Send(new LoginCommand
            {
                Email = email,
                Password = password,
                RememberMe = remember,
            });
        }

        public Task<IUser> Register(string email, string nickname, string password, [Service] IMediator mediator)
        {
            return mediator.Send(new RegisterCommand
            {
                Email = email,
                Nickname = nickname,
                Password = password,
            });
        }

        public async Task<bool> Logout([Service] IMediator mediator)
        {
            await mediator.Send(new LogoutCommand());

            return true;
        }

        public async Task<bool> DeleteCurrentUser([Service] IMediator mediator)
        {
            await mediator.Send(new DeleteMyAccountCommand());

            return true;
        }

        public Task<string> CreateToken([Service] IMediator mediator, string description)
        {
            return mediator.Send(new CreateTokenCommand
            {
                Description = description,
            });
        }

        public async Task<bool> DeleteToken([Service] IMediator mediator, string token)
        {
            await mediator.Send(new DeleteTokenCommand
            {
                Token = token,
            });

            return true;
        }

        public Task<Module> CreateModule([Service] IResolverContext context,
            [Service] IMediator mediator,
            string token,
            string name,
            string description,
            string githubLink,
            bool isCore)
        {
            var replacements = context.Argument<IEnumerable<ModuleReplacement>>("replacements");

            return mediator.Send(new CreateModule
            {
                Description = description,
                Name = name,
                Replacements = replacements.ToArray(),
                Token = token,
                GithubLink = githubLink,
                IsCore = isCore
            });
        }

        public async Task<bool> DeleteModule([Service] IMediator mediator, string token, long id)
        {
            await mediator.Send(new DeleteModuleCommand
            {
                Token = token,
                ModuleId = id,
            });

            return true;
        }

        public Task<IUser> UpdateUser([Service] IResolverContext context, [Service] IMediator mediator)
        {
            var input = context.Argument<ApplicationUser>("account");

            return mediator.Send(new UpdateMyAccountCommand()
            {
                Nickname = input.Nickname,
                ProfilePicUrl = input.ProfilePicUrl,
            });
        }
    }
}