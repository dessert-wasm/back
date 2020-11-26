using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Dessert.Domain.Entities;
using Dessert.Infrastructure;
using Dessert.Infrastructure.Persistence;
using Dessert.Persistence;
using Dessert.Utilities.Configuration;
using GraphQL.Client.Http;
using GraphQL.Common.Request;
using GraphQL.Common.Response;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting.Internal;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Xunit;

namespace Dessert.Tests
{
    public class DessertWebApplicationFactory
        : WebApplicationFactory<Startup>
    {
        protected override IWebHostBuilder CreateWebHostBuilder()
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");
            
            return new WebHostBuilder()
                .UseStartup<Startup>();
        }

        protected override TestServer CreateServer(IWebHostBuilder builder)
        {
            var server = base.CreateServer(builder);
            try
            {
                using (var serviceScope = server.Host.Services.CreateScope())
                {
                    var rng = new Random();

                    var seeder = new DbSeeder(serviceScope.ServiceProvider,
                        new DbSeederOptions()
                        {
                            FakesOptions = new DbFakesOptions()
                            {
                                ModuleCount = () => 30,
                                ReplacementPerModule = () => rng.Next(0, 2),
                                TagPerModule = () => rng.Next(1, 2),
                                ReplacementsCount = () => rng.Next(2, 3),
                            },
                        });
                    seeder.Seed().Wait();
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
                        },
                        ServiceLifetime.Transient);
                })
                .ConfigureServices(services =>
                {
                    services.AddInfrastructure();
                })
                .UseSetting("AllowedOrigins:0", "http://localhost")
                .UseSetting("GitHub:ClientId", "123")
                .UseSetting("GitHub:ClientSecret", "321");
        }
    }
}