using System.Threading.Tasks;
using Bogus;
using Dessert.Domain.Entities.Identity;
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
                  login(email: $username, password: $password, remember: true) {
                    id
                    nickname
                  }
                }
                ",
                Variables = new
                {
                    username = "eleanor.s@gmail.co",
                    password = "pass"
                }
            });

            var account = response.GetDataFieldAs<ApplicationUser>("login");

            Assert.Equal(2, account.Id);
            Assert.Equal("Eleanor", account.Nickname);

            response = await client.SendQueryAsync(new GraphQLRequest
            {
                Query = @"
                {
                  me {
                    id
                    nickname
                  }
                }
                "
            });

            account = response.GetDataFieldAs<ApplicationUser>("me");

            Assert.Equal(2, account.Id);
            Assert.Equal("Eleanor", account.Nickname);

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
                    nickname
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
            var username = faker.Internet.Email();
            var password = faker.Internet.Password();
            var nickname = faker.Internet.UserName();
            
            var response = await client.SendMutationAsync(new GraphQLRequest
            {
                Query = @"
                mutation($username: String!, $password: String!, $nickname: String!) {
                  register(email: $username, password: $password, nickname: $nickname) {
                    id
                    nickname
                    email
                  }
                }
                ",
                Variables = new
                {
                    username = username,
                    password = password, 
                    nickname = nickname,
                }
            });
            
            var account = response.GetDataFieldAs<ApplicationUser>("register");

            Assert.Equal(username, account.Email);
            Assert.Equal(nickname, account.Nickname);
            
            response = await client.SendQueryAsync(new GraphQLRequest
            {
                Query = @"
                {
                  me {
                    id
                  }
                }
                "
            });
            Assert.NotEmpty(response.Errors);

            response = await client.SendMutationAsync(new GraphQLRequest
            {
                Query = @"
                mutation($username: String!, $password: String!) {
                  login(email: $username, password: $password, remember: true) {
                    id
                    email
                  }
                }
                ",
                Variables = new
                {
                    username = username,
                    password = password
                }
            });

            account = response.GetDataFieldAs<ApplicationUser>("login");

            Assert.Equal(username, account.Email);

            response = await client.SendQueryAsync(new GraphQLRequest
            {
                Query = @"
                {
                  me {
                    id
                    email
                  }
                }
                "
            });

            account = response.GetDataFieldAs<ApplicationUser>("me");

            Assert.Equal(username, account.Email);

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
                  }
                }
                "
            });
            Assert.NotEmpty(response.Errors);
        }
    }
}