using System;
using Bogus;
using Dessert.Models;
using GraphQL.Client.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Dessert.Tests
{
    public class DessertWebApplicationFactory
        : WebApplicationFactory<Startup>
    {
        protected override TestServer CreateServer(IWebHostBuilder builder)
        {
            var server = base.CreateServer(builder);
            try
            {
                using (var serviceScope = server.Host.Services.CreateScope())
                {
                    var faker = new Faker();
                    
                    DbInitializer.Initialize(serviceScope.ServiceProvider, true, new DbInitializerOptions
                    {
                        ModuleCount = () => 30,
                        ReplacementPerModule = () => faker.Random.Int(0, 2),
                        TagPerModule = () => faker.Random.Int(1, 2),
                    });
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred initializing the " +
                                    $"database with test messages. Error: {ex.Message}");
            }

            return server;
        }

        public GraphQLHttpClient CreateGraphQlHttpClient()
        {
            var httpClient = CreateClient();
            return httpClient.AsGraphQLClient(new GraphQLHttpClientOptions()
            {
                EndPoint = Server.BaseAddress,
                JsonSerializerSettings = new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver() 
                },
            });
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Create a new service provider.
                var serviceProvider = new ServiceCollection()
                    .AddEntityFrameworkInMemoryDatabase()
                    .BuildServiceProvider();

                // in memory database for testing.
                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryDbForTesting");
                    options.UseInternalServiceProvider(serviceProvider);
                });
            });
        }
    }
}