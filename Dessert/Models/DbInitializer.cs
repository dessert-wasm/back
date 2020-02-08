using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Bogus;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Dessert.Models
{
    public class DbInitializer
    {
        private static readonly Faker faker = new Faker();

        private static readonly Faker<ModuleReplacement> GenModuleReplacement = new Faker<ModuleReplacement>()
            .RuleFor(m => m.Name, f => f.Company.CatchPhrase())
            .RuleFor(m => m.Link, f => f.Internet.Url());

        public static void Initialize(IServiceProvider serviceProvider, bool withFakeData, DbInitializerOptions options)
        {
            using (var context = serviceProvider.GetRequiredService<ApplicationDbContext>())
            {
                context.Database.EnsureCreated();

                AddTags(context);
                if (withFakeData)
                {
                    AddAccounts(serviceProvider.GetRequiredService<UserManager<Account>>());
                    AddModules(context, options);
                }
            }
        }

        private static Module CreateModule(ApplicationDbContext applicationDbContext,
            string name,
            string desc,
            IEnumerable<ModuleReplacement> replacements,
            IEnumerable<ModuleTag> tags,
            Account account,
            DbInitializerOptions dbInitializerOptions)
        {
            var module = new Module()
            {
                Name = name,
                Description = desc,
                PublishedDateTime = DateTime.Now,
                LastUpdatedDateTime = DateTime.Now,
                Author = account,
                IsCore = faker.Random.Bool(),
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

            foreach (var tag in tags)
            {
                var moduleModuleTagRelation = new ModuleModuleTagRelation()
                {
                    Module = module,
                    ModuleTag = tag,
                };
                applicationDbContext.ModuleModuleTagRelations.Add(moduleModuleTagRelation);
            }

            applicationDbContext.SaveChanges();

            return module;
        }

        private static void AddModules(ApplicationDbContext applicationDbContext,
            DbInitializerOptions dbInitializerOptions)
        {
            var tags = applicationDbContext.ModuleTags.ToList();
            var accounts = applicationDbContext.Users.ToList();

            var moduleReplacement = new ModuleReplacement()
            {
                Name = "yaml-js",
                Link = "https://www.npmjs.com/package/yaml-js",
            };
            var module = CreateModule(applicationDbContext,
                "dessert-yaml-js",
                "WASM connector corresponding to the yaml-js library",
                new[] { moduleReplacement },
                faker.PickRandom(tags, dbInitializerOptions.TagPerModule()),
                accounts.FirstOrDefault(x => x.Id == 1),
                dbInitializerOptions);
            Debug.Assert(module.Id == 1);

            var moduleCount = dbInitializerOptions.ModuleCount();
            var replacementPerModuleCount = dbInitializerOptions.ReplacementPerModule();
            for (int i = 0; i < moduleCount; i++)
            {
                var replacements = GenModuleReplacement.GenerateForever().Take(replacementPerModuleCount);
                CreateModule(applicationDbContext,
                    faker.Company.CatchPhrase(),
                    faker.Lorem.Paragraphs(),
                    replacements,
                    faker.PickRandom(tags, dbInitializerOptions.TagPerModule()),
                    faker.PickRandom(accounts),
                    dbInitializerOptions);
            }


            applicationDbContext.SaveChanges();
        }

        private static Account AddAccount(UserManager<Account> userManager, string username, string password, string email, string firstName, string lastName, string pic)
        {
            var account = new Account()
            {
                UserName = username,
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                ProfilePicUrl = pic,
            };

            userManager.CreateAsync(account, password).Wait();
            return account;
        }

        private static void AddAccounts(UserManager<Account> userManager)
        {
            var account = AddAccount(userManager,
                "Tahani",
                "pass",
                "tahani.aj@gmail.co",
                "Tahani",
                "Al-Jamil",
                "https://vignette.wikia.nocookie.net/thegoodplace/images/d/da/3tahani.jpg/revision/latest/scale-to-width-down/620");
            Debug.Assert(account.Id == 1);
            account = AddAccount(userManager,
                "Eleanor",
                "pass",
                "eleanor.s@gmail.co",
                "Eleanor",
                "Shellstrop",
                "https://vignette.wikia.nocookie.net/thegoodplace/images/e/e1/3shellstrop.jpg/revision/latest/scale-to-width-down/620");
            Debug.Assert(account.Id == 2);
            account = AddAccount(userManager,
                "Chidi",
                "pass",
                "chidi.a@gmail.co",
                "Chidi",
                "Anagonye",
                "https://vignette.wikia.nocookie.net/thegoodplace/images/1/1a/3chidi.jpg/revision/latest/scale-to-width-down/620");
            Debug.Assert(account.Id == 3);
            account = AddAccount(userManager,
                "Janet",
                "pass",
                "janet@gmail.co",
                "Janet",
                "On Sait Pas",
                "https://vignette.wikia.nocookie.net/thegoodplace/images/9/90/3janet.jpg/revision/latest/scale-to-width-down/620");
            Debug.Assert(account.Id == 4);
            account = AddAccount(userManager,
                "Jason",
                "pass",
                "jason.m@gmail.co",
                "Jason",
                "Mendoza",
                "https://vignette.wikia.nocookie.net/thegoodplace/images/a/a5/3jason.jpg/revision/latest/scale-to-width-down/620");
            Debug.Assert(account.Id == 5);
            account = AddAccount(userManager,
                "Michael",
                "pass",
                "mochael@gmail.co",
                "Michael",
                "???",
                "https://vignette.wikia.nocookie.net/thegoodplace/images/b/bb/3michael.jpg/revision/latest/scale-to-width-down/620");
            Debug.Assert(account.Id == 6);
        }

        private static void AddTags(ApplicationDbContext applicationDbContext)
        {
            void CreateTag(string name)
            {
                var tag = new ModuleTag()
                {
                    Name = name,
                };
                applicationDbContext.ModuleTags.Add(tag);
            }

            CreateTag("c");
            CreateTag("cpp");
            CreateTag("c#");
            CreateTag("javascript");
            CreateTag("typescript");
            CreateTag("python");
            CreateTag("lua");
            CreateTag("html");
            CreateTag("webasm");
            CreateTag("webgl");
            CreateTag("websql");
            CreateTag("indexed-db");
            CreateTag("local-storage");
            CreateTag("cookie");
            CreateTag("react");
            CreateTag("vuejs");
            CreateTag("nodejs");
            CreateTag("console");
            CreateTag("ui");
            CreateTag("frontend");
            CreateTag("dofus");
            CreateTag("backend");
            CreateTag("yaml");
            CreateTag("json");
            CreateTag("xml");
            CreateTag("material-design");
            CreateTag("react-components");
            CreateTag("ui-kit");
            CreateTag("css");
            CreateTag("less");
            CreateTag("design");
            CreateTag("testing");
            CreateTag("angular");
            CreateTag("web");
            CreateTag("flux");
            CreateTag("linux");
            CreateTag("windows");
            CreateTag("mac");
            CreateTag("email");
            CreateTag("productivity");
            CreateTag("mobile");
            CreateTag("virtual-dom");
            CreateTag("redux");
            CreateTag("context");
            CreateTag("ios");
            CreateTag("android");
            CreateTag("electron");
            CreateTag("ionic");
            CreateTag("graphql");
            CreateTag("desktop");
            CreateTag("eslint");
            CreateTag("yarn");
            CreateTag("npm");

            applicationDbContext.SaveChanges();
        }
    }

    public class DbInitializerOptions
    {
        public Func<int> ModuleCount { get; set; }
        public Func<int> ReplacementPerModule { get; set; }
        public Func<int> TagPerModule { get; set; }
    }
}
