using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dessert.Application.Interfaces;
using Dessert.Application.Repositories;
using Dessert.Domain.Entities;
using Dessert.Domain.Entities.Identity;
using MediatR;

namespace Dessert.Application.Handlers.Module.Commands.CreateModule
{
    public class CreateModule : IRequest<Domain.Entities.Module>
    {
        public string Token { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string GithubLink { get; set; }
        public bool IsCore { get; set; }
        public IReadOnlyCollection<ModuleReplacement> Replacements { get; set; }
        
        public class CreateModuleHandler : IRequestHandler<CreateModule, Domain.Entities.Module>
        {
            private readonly IModuleRepository _moduleRepository;

            public CreateModuleHandler(IModuleRepository moduleRepository)
            {
                _moduleRepository = moduleRepository;
            }

            public Task<Domain.Entities.Module> Handle(CreateModule request, CancellationToken cancellationToken)
            {
                return _moduleRepository.CreateModule(request.Token,
                    request.Name,
                    request.Description,
                    request.GithubLink,
                    request.Replacements,
                    request.IsCore);
            }
        }
    }
}