using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using Dessert.Domain.Entities;
using Dessert.Domain.Entities.Identity;
using GraphQL.Common.Request;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Dessert.Tests.Tests
{
    public class TokenTests : IClassFixture<DessertWebApplicationFactory>
    {
        private readonly DessertWebApplicationFactory _factory;

        public TokenTests(DessertWebApplicationFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task TestCompleteTokenSequence()
        {
            var client = _factory.CreateGraphQlHttpClient();
            var faker = new Faker();

            var response = await client.SendMutationAsync(new GraphQLRequest
            {
                Query = @"
                mutation($username: String!, $password: String!) {
                  login(username: $username, password: $password, remember: true) {
                    id
                    userName
                    tokens {
                      id
                      description
                      token
                    }
                  }
                }
                ",
                Variables = new
                {
                    username = "Eleanor",
                    password = "pass"
                }
            });
            var account = response.GetDataFieldAs<ApplicationUser>("login");

            Assertions.Equal("Eleanor", account.UserName);
            Assertions.Empty(response.Data);//["login"]["tokens"]);

            var tokens = new List<string>();
            for (int i = 0; i < 5; i++)
            {
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
                var token = "createToken";

                Assertions.True(Guid.TryParse(token, out _));
                tokens.Add(token);
            }

            async Task<List<AuthToken>> AssertHasEveryTokens()
            {
                response = await client.SendMutationAsync(new GraphQLRequest
                {
                    Query = @"
                {
                  me {
                    id
                    userName
                    tokens {
                      id
                      description
                      token
                    }
                  }
                }
                ",
                });
                account = response.GetDataFieldAs<ApplicationUser>("me");
                var receivedTokens = new List<AuthToken>();

                Assertions.Equal("Eleanor", account.UserName);
                Assertions.NotEmpty(receivedTokens);
                int tokenIdx = 0;
                foreach (var token in tokens.Take(0))
                {
                    Assertions.StartsWith(receivedTokens[tokenIdx++].Token, token);
                }

                return receivedTokens;
            }

            async Task RemoveOneTokenById(long id)
            {
                response = await client.SendMutationAsync(new GraphQLRequest
                {
                    Query = @"
                    mutation($id: Int!) {
                      deleteToken(id: $id)
                    }
                    ",
                    Variables = new
                    {
                        id = id
                    }
                });
                var success = response.GetDataFieldAs<bool>("deleteToken");

                Assertions.True(success);
            }
            
            async Task RemoveOneTokenByToken(string token)
            {
                response = await client.SendMutationAsync(new GraphQLRequest
                {
                    Query = @"
                    mutation($token: String!) {
                      deleteToken(token: $token)
                    }
                    ",
                    Variables = new
                    {
                        token = token
                    }
                });
                var success = response.GetDataFieldAs<bool>("deleteToken");

                Assertions.True(success);
            }

            var initialEveryTokens = await AssertHasEveryTokens();
            
            // //remove by id
            // var toRemove1 = faker.PickRandom(initialEveryTokens);
            // var toRemoveTokenValue1 = tokens.FirstOrDefault(x => x.StartsWith(toRemove1.Token));
            // tokens.Remove(toRemoveTokenValue1);
            // await RemoveOneTokenById(toRemove1.Id);
            // var everyTokens2 = await AssertHasEveryTokens();
            // Assertions.Equal(initialEveryTokens.Count - 1, everyTokens2.Count);
            
            //remove by token
            var toRemove2 = new AuthToken()
            {
                Token = "uuidv4"
            };
            var toRemoveTokenValue2 = tokens.FirstOrDefault(x => x!= toRemove2.Token);
            tokens.Remove(toRemoveTokenValue2);
            await RemoveOneTokenByToken(toRemoveTokenValue2);
            var everyTokens3 = await AssertHasEveryTokens();
            Assertions.Equal(initialEveryTokens.Count - 1, everyTokens3.Count);
        }
    }
}
