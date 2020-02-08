using System.Threading.Tasks;
using Bogus;
using Dessert.Models;
using GraphQL.Common.Request;
using Xunit;

namespace Dessert.Tests.Tests
{
    public class LoginTests : IClassFixture<DessertWebApplicationFactory>
    {
        private readonly DessertWebApplicationFactory _factory;

        public LoginTests(DessertWebApplicationFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task TestCompleteLoginSequence()
        {
            var client = _factory.CreateGraphQlHttpClient();

            var response = await client.SendMutationAsync(new GraphQLRequest
            {
                Query = @"
                mutation($username: String!, $password: String!) {
                  login(username: $username, password: $password, remember: true) {
                    id
                    userName
                  }
                }
                ",
                Variables = new
                {
                    username = "Eleanor",
                    password = "pass"
                }
            });

            var account = response.GetDataFieldAs<Account>("login");

            Assert.Equal(2, account.Id);
            Assert.Equal("Eleanor", account.UserName);

            response = await client.SendQueryAsync(new GraphQLRequest
            {
                Query = @"
                {
                  me {
                    id
                    userName
                    firstName
                    lastName
                  }
                }
                "
            });

            account = response.GetDataFieldAs<Account>("me");

            Assert.Equal(2, account.Id);
            Assert.Equal("Eleanor", account.UserName);
            Assert.Equal("Eleanor", account.FirstName);
            Assert.Equal("Shellstrop", account.LastName);

            response = await client.SendMutationAsync(new GraphQLRequest
            {
                Query = @"
                mutation {
                  logout
                }
                ",
            });

            var isLogoutSuccess = response.GetDataFieldAs<bool>("logout");
            Assert.True(isLogoutSuccess);
            
            response = await client.SendQueryAsync(new GraphQLRequest
            {
                Query = @"
                {
                  me {
                    id
                    userName
                    firstName
                    lastName
                  }
                }
                "
            });
            Assert.NotEmpty(response.Errors);
        }
        
        [Fact]
        public async Task TestCompleteRegisterThenLoginSequence()
        {
            var client = _factory.CreateGraphQlHttpClient();
            var faker = new Faker();
            var username = faker.Internet.UserName();
            var password = faker.Internet.Password();
            
            var response = await client.SendMutationAsync(new GraphQLRequest
            {
                Query = @"
                mutation($username: String!, $password: String!) {
                  register(username: $username, password: $password) {
                    id
                    userName
                  }
                }
                ",
                Variables = new
                {
                    username = username,
                    password = password, 
                }
            });
            
            var account = response.GetDataFieldAs<Account>("register");

            Assert.Equal(username, account.UserName);
            
            response = await client.SendQueryAsync(new GraphQLRequest
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

            response = await client.SendMutationAsync(new GraphQLRequest
            {
                Query = @"
                mutation($username: String!, $password: String!) {
                  login(username: $username, password: $password, remember: true) {
                    id
                    userName
                  }
                }
                ",
                Variables = new
                {
                    username = username,
                    password = password
                }
            });

            account = response.GetDataFieldAs<Account>("login");

            Assert.Equal(username, account.UserName);

            response = await client.SendQueryAsync(new GraphQLRequest
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

            account = response.GetDataFieldAs<Account>("me");

            Assert.Equal(username, account.UserName);

            response = await client.SendMutationAsync(new GraphQLRequest
            {
                Query = @"
                mutation {
                  logout
                }
                ",
            });

            var isLogoutSuccess = response.GetDataFieldAs<bool>("logout");
            Assert.True(isLogoutSuccess);
            
            response = await client.SendQueryAsync(new GraphQLRequest
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