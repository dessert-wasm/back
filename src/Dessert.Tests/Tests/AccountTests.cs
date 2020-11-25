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
                  login(username: $username, password: $password, remember: true) {
                    firstName
                    lastName
                  }
                }
                ",
                Variables = new
                {
                    username = "Eleanor",
                    password = "pass"
                }
            });
            

            var account = login_response.GetDataFieldAs<ApplicationUser>("login");
           //  Assertions.Equal("Eleanor", account.FirstName);
           //  Assertions.Equal("Shellstrop", account.LastName);
            
            var response = await client.SendQueryAsync(new GraphQLRequest
            {
                Query = @"
                mutation($account: AccountInput!) {
                  updateUser(account: $account) {
                      id
                      firstName
                      lastName
                  }
                }
                ",
                Variables = new
                {
                    account = new {
                        firstName = "Eleaanor",
                        lastName = "Oui"
                    }
                }
            });

            Assertions.Null(response.Errors);
            Assertions.NotNull(response.Data);
            
            var updatedAccount = response.GetDataFieldAs<ApplicationUser>("updateUser");
            
            Assertions.Equal("Eleaanor", updatedAccount.Nickname);
             Assertions.Equal("Oui", updatedAccount.Email);
            
            var me_response = await client.SendQueryAsync(new GraphQLRequest
            {
                Query = @"
                {
                    me {
                        id
                        firstName
                        lastName
                    }
                }
                "
            });
            var me = me_response.GetDataFieldAs<ApplicationUser>("me");
            
            Assertions.Null(me_response.Errors);
            Assertions.NotNull(me_response.Data);
            Assertions.Equal("Eleaanor", me.Nickname);
            Assertions.Equal("Oui", me.Email);
        }
    }
}