using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using Dessert.Domain.Entities;
using Dessert.Domain.Entities.Identity;
using Dessert.Persistence;
using GraphQL.Common.Request;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Dessert.Tests.Tests
{
    public class ModuleTests : IClassFixture<DessertWebApplicationFactory>
    {
        private readonly DessertWebApplicationFactory _factory;

        public ModuleTests(DessertWebApplicationFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task TestCompleteTokenSequence()
        {
            var client = _factory.CreateGraphQlHttpClient();
            var faker = new Faker();

            //remove all of tahanie's modules
            using (var serviceScope = _factory.Server.Host.Services.CreateScope())
            {
                var db = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                db.RemoveRange(db.Modules.Where(x => x.AuthorId == 2));
                await db.SaveChangesAsync();
            }

            var response = await client.SendMutationAsync(new GraphQLRequest
            {
                Query = @"
                mutation($username: String!, $password: String!, $pagination: PaginationQueryInput!) {
                  login(username: $username, password: $password, remember: true) {
                    id
                    userName
                    tokens {
                      id
                      description
                      token
                    }
                    modules(pagination: $pagination) {
                      result { id }
                    }
                  }
                }
                ",
                Variables = new
                {
                    username = "Eleanor",
                    password = "pass",
                    pagination = new {
                        pageNumber = 1,
                        pageSize = 1,
                        includeCount = false
                    }
                }
            });
            var account = response.GetDataFieldAs<Account>("login");

            Assert.Equal("Eleanor", account.UserName);
            Assert.Empty(response.Data["login"]["tokens"]);
            Assert.Empty(response.Data["login"]["modules"]["result"]);

            response = await client.SendMutationAsync(new GraphQLRequest
            {
                Query = @"
                mutation($description: String!) {
                  createToken(description: $description)
                }
                ",
                Variables = new
                {
                    description = faker.Lorem.Paragraph()
                }
            });
            var token = response.GetDataFieldAs<string>("createToken");

            async Task<Module> CreateModule()
            {
                var name = faker.Company.CatchPhrase();
                var description = faker.Lorem.Paragraphs();
                var isCore = faker.Random.Bool();

                response = await client.SendMutationAsync(new GraphQLRequest
                {
                    Query = @"
                    mutation(
                      $name: String!
                      $description: String!
                      $replacements: [ModuleReplacementInput!]!
                      $isCore: Boolean!
                      $token: String!
                    ) {
                      createModule(
                        name: $name
                        description: $description
                        replacements: $replacements
                        isCore: $isCore
                        token: $token
                      ) {
                        id
                        name
                        description
                        isCore
                        replacements {
                          id
                          name
                          link
                        }
                        tags {
                          id
                          name
                        }
                      }
                    }
                    ",
                    Variables = new
                    {
                        name = name,
                        description = description,
                        replacements = new [] { new {name="oui", link="oui.js"}},
                        isCore = isCore,
                        token = token,
                    }
                });
                var module = response.GetDataFieldAs<Module>("createModule");

                Assert.Equal(name, module.Name);
                Assert.Equal(description, module.Description);
                Assert.Equal(isCore, module.IsCore);
                Assert.NotEmpty(response.Data["createModule"]["replacements"]);
                Assert.Empty(response.Data["createModule"]["tags"]);

                return module;
            }

            async Task DeleteModule(long id)
            {
                response = await client.SendMutationAsync(new GraphQLRequest
                {
                    Query = @"
                    mutation($id: Int!, $token: String!) {
                      deleteModule(id: $id, token: $token)
                    }
                    ",
                    Variables = new
                    {
                        id = id,
                        token = token,
                    }
                });
                var success = response.GetDataFieldAs<bool>("deleteModule");

                Assert.True(success);
            }

            async Task AssertUserHasSameModules(List<Module> modulesToCheck)
            {
                response = await client.SendMutationAsync(new GraphQLRequest
                {
                    Query = @"
                {
                  me {
                    id
                    userName
                    modules(pagination: {pageNumber: 1, pageSize: 50, includeCount: false}) {
                      result { id }
                    }
                  }
                }
                ",
                });
                // TODO: This now only checks for the first page of modules with a pageSize of 50
                account = response.GetDataFieldAs<Account>("me");
                var receivedModules = (response.Data["me"]["modules"]["result"] as JArray)?.ToObject<List<Module>>();
                Assert.Equal(modulesToCheck.Count, receivedModules.Count);
                foreach (var receivedModule in receivedModules)
                {
                    Assert.Contains(modulesToCheck, x => x.Id == receivedModule.Id);
                }
            }

            var modules = new List<Module>();
            modules.Add(await CreateModule());
            modules.Add(await CreateModule());
            modules.Add(await CreateModule());

            await AssertUserHasSameModules(modules);
            var modulesToRemoveCount = modules.Count;
            while (modulesToRemoveCount-- > 0)
            {
                var module = modules.FirstOrDefault();
                await DeleteModule(module.Id);
                modules.Remove(module);
                await AssertUserHasSameModules(modules);
            }
        }
    }
}
