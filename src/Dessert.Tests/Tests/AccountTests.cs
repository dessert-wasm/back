using System.Threading.Tasks;
using Dessert.Domain.Entities.Identity;
using GraphQL.Common.Request;
using Xunit;

namespace Dessert.Tests.Tests
{
    public class AccountTests : IClassFixture<DessertWebApplicationFactory>
    {
        private readonly DessertWebApplicationFactory _factory;

        public AccountTests(DessertWebApplicationFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task UpdateUser()
        {
            var client = _factory.CreateGraphQlHttpClient();
            // Login first
            var login_response = await client.SendMutationAsync(new GraphQLRequest
            {
                Query = @"
                mutation($username: String!, $password: String!) {
                  login(email: $username, password: $password, remember: true) {
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
            

            var account = login_response.GetDataFieldAs<ApplicationUser>("login");
            Assert.Equal("Eleanor", account.Nickname);
            
            var response = await client.SendQueryAsync(new GraphQLRequest
            {
                Query = @"
                mutation($account: AccountInput!) {
                  updateUser(account: $account) {
                    id
                    nickname
                    profilePicUrl
                  }
                }
                ",
                Variables = new
                {
                    account = new {
                        nickname = "Oui",
                        profilePicUrl = "mok"
                    }
                }
            });

            Assert.Null(response.Errors);
            Assert.NotNull(response.Data);
            
            var updatedAccount = response.GetDataFieldAs<ApplicationUser>("updateUser");
            
            Assert.Equal("Oui", updatedAccount.Nickname);
            Assert.Equal("mok", updatedAccount.ProfilePicUrl);
            
            var me_response = await client.SendQueryAsync(new GraphQLRequest
            {
                Query = @"
                {
                    me {
                        id
                    nickname
                    profilePicUrl
                    }
                }
                "
            });
            var me = me_response.GetDataFieldAs<ApplicationUser>("me");
            
            Assert.Null(me_response.Errors);
            Assert.NotNull(me_response.Data);
            Assert.Equal("Oui", me.Nickname);
            Assert.Equal("mok", me.ProfilePicUrl);
        }
    }
}