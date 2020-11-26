using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dessert.Application.Repositories;
using Dessert.Domain.Entities;
using Dessert.Domain.Entities.Identity;
using Dessert.Domain.Enums;
using Dessert.Domain.Pagination;
using Dessert.Infrastructure.Pagination;
using Dessert.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Dessert.Infrastructure.Repositories
{
    public class ModuleRepository : IModuleRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public ModuleRepository(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public Task<Module> GetModule(long moduleId)
        {
            return _applicationDbContext.Modules
                .FirstOrDefaultAsync(x => x.Id == moduleId);
        }

        public Task<PaginatedResult<Module>> SearchModules(string query,
            ModuleTypeEnum? type,
            PaginationQuery paginationQuery)
        {
            query = $"%{query}%";
            var sqlQuery = _applicationDbContext.Modules
                .Where(x =>
                    EF.Functions.ILike(x.Name, query) ||
                    EF.Functions.ILike(x.Description, query) ||
                    EF.Functions.ILike(x.GithubLink, query) ||
                    EF.Functions.ILike(x.Author.Nickname, query) ||
                    _applicationDbContext.ModuleModuleReplacementRelations
                        .Any(t => t.ModuleId == x.Id && (EF.Functions.ILike(t.ModuleReplacement.Name, query) ||
                                                         EF.Functions.ILike(t.ModuleReplacement.Link, query))) ||
                    _applicationDbContext.ModuleModuleTagRelations
                        .Any(t => t.ModuleId == x.Id && EF.Functions.ILike(t.ModuleTag.Name, query)));
            if (type.HasValue)
            {
                sqlQuery = sqlQuery
                    .Where(x => x.IsCore == (type.Value == ModuleTypeEnum.Core));
            }

            var orderedQuery = sqlQuery.OrderByDescending(x => x.LastUpdatedDateTime);
            return Paginator.GetPaginatedResult(paginationQuery, orderedQuery);
        }

        public async Task<IReadOnlyCollection<Module>> Recommend(IReadOnlyCollection<JSDependency> dependencies)
        {
            var dependencyNames = dependencies.Select(x => x.Name);

            var result = await _applicationDbContext.ModuleModuleReplacementRelations
                .Where(x => dependencyNames.Contains(x.ModuleReplacement.Name))
                .Select(x => x.Module)
                .Distinct()
                .ToListAsync();
            return result;
        }

        public async Task<Module> CreateModule(string token,
            string name,
            string description,
            string githubLink,
            IReadOnlyCollection<ModuleReplacement> replacements,
            bool isCore)
        {

            var account = await GetAccountFromToken(token);

            var module = new Module()
            {
                Name = name,
                Description = description,
                IsCore = isCore,
                PublishedDateTime = DateTime.Now,
                LastUpdatedDateTime = DateTime.Now,
                AuthorId = account.Id,
                GithubLink = githubLink,
            };
            await _applicationDbContext.Modules.AddAsync(module);

            foreach (var r in replacements)
            {
                var replacement = new ModuleReplacement()
                {
                    Link = r.Link,
                    Name = r.Name,
                };
                await _applicationDbContext.ModuleReplacements.AddAsync(replacement);

                var moduleModuleReplacementRelation = new ModuleModuleReplacementRelation()
                {
                    Module = module,
                    ModuleReplacement = replacement,
                };
                await _applicationDbContext.ModuleModuleReplacementRelations.AddAsync(moduleModuleReplacementRelation);
            }

            await _applicationDbContext.SaveChangesAsync();

            return module;
        }

        public async Task DeleteModule(string token, long moduleId)
        {
            var account = await GetAccountFromToken(token);

            var module = await _applicationDbContext.Modules.FirstOrDefaultAsync(x => x.Id == moduleId);

            //todo custom error depending on the reason why it failed ?
            if (module == null)
                return;
            if (module.Author != account)
                return;

            _applicationDbContext.Modules.Remove(module);
            await _applicationDbContext.SaveChangesAsync();
        }

        private async Task<IUser> GetAccountFromToken(string token)
        {
            var authToken = await _applicationDbContext.AuthTokens
                .Include(x => x.Account)
                .FirstOrDefaultAsync(x => x.Token == token);

            if (authToken == null)
                throw new Exception("invalid token");

            return authToken.Account;
        }

        public async Task<ILookup<long, ModuleReplacement>> GetReplacementsBatch(IReadOnlyList<long> keys,
            CancellationToken cancellationToken)
        {
            var moduleTags = await _applicationDbContext.ModuleModuleReplacementRelations
                .AsNoTracking()
                .Where(x => keys.Contains(x.ModuleId))
                .Include(x => x.ModuleReplacement)
                .ToListAsync(cancellationToken);

            return moduleTags
                .ToLookup(x => x.ModuleId, x => x.ModuleReplacement);
        }

        public Task<PaginatedResult<Module>> GetModulesByAuthorId(long accountId, PaginationQuery paginationQuery)
        {
            var sqlQuery = _applicationDbContext.Modules
                .AsNoTracking()
                .Where(x => x.AuthorId == accountId)
                .OrderByDescending(x => x.LastUpdatedDateTime);

            return Paginator.GetPaginatedResult(paginationQuery, sqlQuery);
        }
    }
}