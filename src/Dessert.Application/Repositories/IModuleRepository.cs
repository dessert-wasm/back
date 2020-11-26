using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dessert.Domain.Entities;
using Dessert.Domain.Enums;
using Dessert.Domain.Pagination;
using MediatR;

namespace Dessert.Application.Repositories
{
    public interface IModuleRepository
    {
        Task<Module> GetModule(long moduleId);
        Task<PaginatedResult<Module>> SearchModules(string query, ModuleTypeEnum? type, PaginationQuery paginationQuery);
        Task<IReadOnlyCollection<Module>> Recommend(IReadOnlyCollection<JSDependency> dependencies);
        Task<Module> CreateModule(string token,
            string name,
            string description,
            string githubLink,
            IReadOnlyCollection<ModuleReplacement> replacements,
            bool isCore);

        Task DeleteModule(string token, long moduleId);
        Task<ILookup<long, ModuleReplacement>> GetReplacementsBatch(IReadOnlyList<long> keys, CancellationToken cancellationToken);
        Task<PaginatedResult<Module>> GetModulesByAuthorId(long accountId, PaginationQuery paginationQuery);
    }
}