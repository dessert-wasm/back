using System.Collections.Generic;
using System.Threading.Tasks;
using Dessert.Models;
using Dessert.Utilities.Pagination;
using GraphQL.Common.Request;
using Newtonsoft.Json.Linq;
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
            

            var account = login_response.GetDataFieldAs<Account>("login");
            Assert.Equal("Eleanor", account.FirstName);
            Assert.Equal("Shellstrop", account.LastName);
            
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

            Assert.Null(response.Errors);
            Assert.NotNull(response.Data);
            
            var updatedAccount = response.GetDataFieldAs<Account>("updateUser");
            
            Assert.Equal("Eleaanor", updatedAccount.FirstName);
            Assert.Equal("Oui", updatedAccount.LastName);
            
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
            var me = me_response.GetDataFieldAs<Account>("me");
            
            Assert.Null(me_response.Errors);
            Assert.NotNull(me_response.Data);
            Assert.Equal("Eleaanor", me.FirstName);
            Assert.Equal("Oui", me.LastName);
        }
    }
}