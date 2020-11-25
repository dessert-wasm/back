using System.Collections.Generic;
using System.Threading.Tasks;
using Dessert.Domain.Entities;
using Dessert.Domain.Entities.Identity;
using Dessert.Domain.Pagination;
using GraphQL.Common.Request;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Dessert.Tests.Tests
{
    public class BasicTests : IClassFixture<DessertWebApplicationFactory>
    {
        private readonly DessertWebApplicationFactory _factory;

        public BasicTests(DessertWebApplicationFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task TestTags()
        {
            var client = _factory.CreateGraphQlHttpClient();
            var response = await client.SendQueryAsync(new GraphQLRequest
            {
                Query = @"
                {
                  tags {
                    id
                    name
                  }
                }
                ",
            });

            var tags = response.GetDataFieldAs<List<ModuleTag>>("tags");

            Assertions.Null(response.Errors);
            Assertions.NotEmpty(tags);
        }

        [Fact]
        public async Task TestUser()
        {
            var client = _factory.CreateGraphQlHttpClient();
            var response = await client.SendQueryAsync(new GraphQLRequest
            {
                Query = @"
                {
                  user(id: 2) {
                    id
                    firstName
                    lastName
                    userName
                    profilePicUrl
                    modules(pagination: {pageSize: 1, pageNumber: 1, includeCount: false}) {
                      result { id }
                    }
                    tokens {
                      id
                    }
                  }
                }
                ",
            });

            var account = response.GetDataFieldAs<ApplicationUser>("user");

            Assertions.Equal(2, account.Id);
            Assertions.Equal("Eleanor", account.UserName);
            // Assertions.Equal("Eleanor", account.FirstName);
            // Assertions.Equal("Shellstrop", account.LastName);
            Assertions.IsType<JArray>(response.Data);//["user"]["modules"]["result"]);
            Assertions.Empty(response.Data);//["user"]["tokens"]);
        }

        [Fact]
        public async Task TestModule()
        {
            var client = _factory.CreateGraphQlHttpClient();
            var response = await client.SendQueryAsync(new GraphQLRequest
            {
                Query = @"
                {
                  module(id: 1) {
                    id
                    name
                    description
                    lastUpdatedDateTime
                    publishedDateTime
                    author {
                      id
                      firstName
                      lastName
                      userName
                    }
                    isCore
                    tags {
                      id
                      name
                    }
                    replacements {
                      id
                      link
                      name
                    }
                  }
                }
                ",
            });

            var module = response.GetDataFieldAs<Module>("module");

            Assertions.Equal(1, module.Id);
            Assertions.Equal("dessert-yaml-js", module.Name);
            Assertions.Equal("WASM connector corresponding to the yaml-js library", module.Description);
            Assertions.Equal(new ApplicationUser(), module.Author);
            Assertions.Equal(new ApplicationUser(), module.Author);
            // Assertions.Equal("Tahani", module.Author.FirstName);
            // Assertions.Equal("Al-Jamil", module.Author.LastName);
        }

        [Fact]
        public async Task TestSearch()
        {
            var client = _factory.CreateGraphQlHttpClient();
            var response = await client.SendQueryAsync(new GraphQLRequest
            {
                Query = @"
                query($q: String!) {
                  search(query: $q, pagination: { pageSize: 10, pageNumber: 2, includeCount: true }) {
                    pageNumber
                        pageSize
                    totalPages
                        totalRecords
                    result {
                        name
                            description
                    }
                  }
                }
                ",
                Variables = new
                {
                    q = "a",
                }
            });

            var result = response.GetDataFieldAs<PaginatedResult<Module>>("search");
            Assertions.NotNull(result.Result);
        }

        [Fact]
        public async Task TestMeUserNotConnected()
        {
            var client = _factory.CreateGraphQlHttpClient();
            var response = await client.SendQueryAsync(new GraphQLRequest
            {
                Query = @"
                {
                  me {
                    id
                    userName
                  }
                }
                "
            });

            Assertions.NotEmpty(response.Errors);
        }
    }
}