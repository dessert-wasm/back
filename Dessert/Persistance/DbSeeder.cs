using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using Dessert.Domain.Entities;
using Dessert.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Dessert.Persistance
{
    public class DbSeeder
    {
        private readonly Faker _faker;
        private readonly DbSeederOptions _options;
        private readonly ApplicationDbContext _db;
        private readonly UserManager<Account> _userManager;
        
        public DbSeeder(IServiceProvider serviceProvider, DbSeederOptions options)
        {
            _options = options;
            _faker = new Faker();
            
            _db = serviceProvider.GetRequiredService<ApplicationDbContext>();
            _userManager = serviceProvider.GetRequiredService<UserManager<Account>>();
        }

        public async Task Seed()
        {
            _db.Database.EnsureCreated();

            await AddTags();
            if (_options.FakesOptions != null)
            {
                await AddAccounts();
                await AddModules();
            }
        }

        private async Task<Module> CreateModule(
            string name,
            string desc,
            IEnumerable<ModuleReplacement> replacements,
            IEnumerable<ModuleTag> tags,
            Account account)
        {
            var module = new Module()
            {
                Name = name,
                Description = desc,
                PublishedDateTime = DateTime.Now,
                LastUpdatedDateTime = DateTime.Now,
                Author = account,
                IsCore = _faker.Random.Bool(),
            };
            await _db.Modules.AddAsync(module);

            foreach (var r in replacements)
            {
                var moduleModuleReplacementRelation = new ModuleModuleReplacementRelation()
                {
                    Module = module,
                    ModuleReplacement = r,
                };
                await _db.ModuleModuleReplacementRelations.AddAsync(moduleModuleReplacementRelation);
            }

            foreach (var tag in tags)
            {
                var moduleModuleTagRelation = new ModuleModuleTagRelation()
                {
                    Module = module,
                    ModuleTag = tag,
                };
                await _db.ModuleModuleTagRelations.AddAsync(moduleModuleTagRelation);
            }

            await _db.SaveChangesAsync();

            return module;
        }

        private async Task AddModules()
        {
            var tags = await _db.ModuleTags.ToListAsync();
            var accounts = await _db.Users.ToListAsync();

            await CreateReplacements();

            var replacements = await _db.ModuleReplacements.ToListAsync();

            var module = CreateModule(
                "dessert-yaml-js",
                "WASM connector corresponding to the yaml-js library",
                replacements.Take(1),
                _faker.PickRandom(tags, _options.FakesOptions.TagPerModule()),
                accounts.FirstOrDefault(x => x.Id == 1));
            Debug.Assert(module.Id == 1);

            var moduleCount = _options.FakesOptions.ModuleCount();
            for (int i = 0; i < moduleCount; i++)
            {
                await CreateModule(
                    _faker.Company.CatchPhrase(),
                    _faker.Lorem.Paragraphs(),
                    _faker.PickRandom(replacements, _options.FakesOptions.ReplacementPerModule()),
                    _faker.PickRandom(tags, _options.FakesOptions.TagPerModule()),
                    _faker.PickRandom(accounts));
            }

            await _db.SaveChangesAsync();
        }

        private async Task CreateReplacements()
        {
            var moduleReplacement = new ModuleReplacement()
            {
                Name = "yaml-js",
                Link = "https://www.npmjs.com/package/yaml-js"
            };
            await _db.ModuleReplacements.AddAsync(moduleReplacement);

            var replacements = new Faker<ModuleReplacement>()
                .RuleFor(m => m.Name, f => f.Hacker.Adjective())
                .RuleFor(m => m.Link, f => f.Internet.Url());
            var replacementCount = _options.FakesOptions.ReplacementsCount();
            await _db.ModuleReplacements.AddRangeAsync(replacements.GenerateForever().Take(replacementCount));
            
            await _db.SaveChangesAsync();
        }

        private async Task<Account> AddAccount(string username,
            string password,
            string email,
            string firstName,
            string lastName,
            string pic)
        {
            var account = new Account()
            {
                UserName = username,
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                ProfilePicUrl = pic,
            };

            await _userManager.CreateAsync(account, password);
            return account;
        }
        

        private async Task AddAccounts()
        {
            var account = await AddAccount(
                "Tahani",
                "pass",
                "tahani.aj@gmail.co",
                "Tahani",
                "Al-Jamil",
                "https://vignette.wikia.nocookie.net/thegoodplace/images/d/da/3tahani.jpg/revision/latest/scale-to-width-down/620");
            Debug.Assert(account.Id == 1);
            account = await AddAccount(
                "Eleanor",
                "pass",
                "eleanor.s@gmail.co",
                "Eleanor",
                "Shellstrop",
                "https://vignette.wikia.nocookie.net/thegoodplace/images/e/e1/3shellstrop.jpg/revision/latest/scale-to-width-down/620");
            Debug.Assert(account.Id == 2);
            account = await AddAccount(
                "Chidi",
                "pass",
                "chidi.a@gmail.co",
                "Chidi",
                "Anagonye",
                "https://vignette.wikia.nocookie.net/thegoodplace/images/1/1a/3chidi.jpg/revision/latest/scale-to-width-down/620");
            Debug.Assert(account.Id == 3);
            account = await AddAccount(
                "Janet",
                "pass",
                "janet@gmail.co",
                "Janet",
                "On Sait Pas",
                "https://vignette.wikia.nocookie.net/thegoodplace/images/9/90/3janet.jpg/revision/latest/scale-to-width-down/620");
            Debug.Assert(account.Id == 4);
            account = await AddAccount(
                "Jason",
                "pass",
                "jason.m@gmail.co",
                "Jason",
                "Mendoza",
                "https://vignette.wikia.nocookie.net/thegoodplace/images/a/a5/3jason.jpg/revision/latest/scale-to-width-down/620");
            Debug.Assert(account.Id == 5);
            account = await AddAccount(
                "Michael",
                "pass",
                "mochael@gmail.co",
                "Michael",
                "???",
                "https://vignette.wikia.nocookie.net/thegoodplace/images/b/bb/3michael.jpg/revision/latest/scale-to-width-down/620");
            Debug.Assert(account.Id == 6);
        }

        private async Task AddTags()
        {
            async Task CreateTag(string name)
            {
                var tag = new ModuleTag()
                {
                    Name = name,
                };
                await _db.ModuleTags.AddAsync(tag);
            }

            await CreateTag("c");
            await CreateTag("cpp");
            await CreateTag("c#");
            await CreateTag("javascript");
            await CreateTag("typescript");
            await CreateTag("python");
            await CreateTag("lua");
            await CreateTag("html");
            await CreateTag("webasm");
            await CreateTag("webgl");
            await CreateTag("websql");
            await CreateTag("indexed-db");
            await CreateTag("local-storage");
            await CreateTag("cookie");
            await CreateTag("react");
            await CreateTag("vuejs");
            await CreateTag("nodejs");
            await CreateTag("console");
            await CreateTag("ui");
            await CreateTag("frontend");
            await CreateTag("dofus");
            await CreateTag("backend");
            await CreateTag("yaml");
            await CreateTag("json");
            await CreateTag("xml");
            await CreateTag("material-design");
            await CreateTag("react-components");
            await CreateTag("ui-kit");
            await CreateTag("css");
            await CreateTag("less");
            await CreateTag("design");
            await CreateTag("testing");
            await CreateTag("angular");
            await CreateTag("web");
            await CreateTag("flux");
            await CreateTag("linux");
            await CreateTag("windows");
            await CreateTag("mac");
            await CreateTag("email");
            await CreateTag("productivity");
            await CreateTag("mobile");
            await CreateTag("virtual-dom");
            await CreateTag("redux");
            await CreateTag("context");
            await CreateTag("ios");
            await CreateTag("android");
            await CreateTag("electron");
            await CreateTag("ionic");
            await CreateTag("graphql");
            await CreateTag("desktop");
            await CreateTag("eslint");
            await CreateTag("yarn");
            await CreateTag("npm");

            await _db.SaveChangesAsync();
        }
    }

    public class DbSeederOptions
    {
        /// <summary>
        /// null for no fakes
        /// </summary>
        public DbFakesOptions FakesOptions { get; set; }
    }

    public class DbFakesOptions
    {
        public Func<int> ModuleCount { get; set; }
        public Func<int> ReplacementsCount { get; set; }
        public Func<int> ReplacementPerModule { get; set; }
        public Func<int> TagPerModule { get; set; }
    }
}
