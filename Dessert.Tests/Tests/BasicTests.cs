using System.Collections.Generic;
using System.Threading.Tasks;
using Dessert.Models;
using Dessert.Utilities.Pagination;
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

            Assert.Null(response.Errors);
            Assert.NotEmpty(tags);
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

            var account = response.GetDataFieldAs<Account>("user");

            Assert.Equal(2, account.Id);
            Assert.Equal("Eleanor", account.UserName);
            Assert.Equal("Eleanor", account.FirstName);
            Assert.Equal("Shellstrop", account.LastName);
            Assert.IsType<JArray>(response.Data["user"]["modules"]["result"]);
            Assert.Empty(response.Data["user"]["tokens"]);
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

            Assert.Equal(1, module.Id);
            Assert.Equal("dessert-yaml-js", module.Name);
            Assert.Equal("WASM connector corresponding to the yaml-js library", module.Description);
            Assert.Equal(1, module.Author.Id);
            Assert.Equal("Tahani", module.Author.UserName);
            Assert.Equal("Tahani", module.Author.FirstName);
            Assert.Equal("Al-Jamil", module.Author.LastName);
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
            Assert.NotNull(result.Result);
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

            Assert.NotEmpty(response.Errors);
        }
    }
}