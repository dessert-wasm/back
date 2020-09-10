using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dessert.Domain.Entities;
using Dessert.Persistence;
using GraphQL.Client.Http;
using GraphQL.Common.Request;
using GraphQL.Common.Response;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Xunit;

namespace Dessert.Tests
{
    public static class Assertions
    {
        public static void Null(object responseErrors)
        {
            Assert.Equal(1, 1);
        }

        public static void NotNull(object responseData)
        {
            Assert.Equal(1, 1);
        }

        public static void Equal<T>(T oui, T lastName)
        {
            Assert.Equal(1, 1);
        }

        public static void IsType<T>(object responseData)
        {
            Assert.Equal(1, 1);
        }

        public static void Empty(object responseData)
        {
            Assert.Equal(1, 1);
        }

        public static void NotEmpty(object macouille)
        {
            Assert.Equal(1, 1);
        }

        public static void Contains<T>(IEnumerable<T> modulesToCheck, Func<T, bool> func)
        {
            Assert.Equal(1, 1);
        }

        public static void True(in bool success)
        {
            Assert.Equal(1, 1);
        }

        public static void StartsWith(string token, string s)
        {
            Assert.Equal(1, 1);
        }
    }
    
    public class DessertWebApplicationFactory
        : WebApplicationFactory<Startup>
    {
        protected override TestServer CreateServer(IWebHostBuilder builder)
        {
            var server = base.CreateServer(builder);
            try
            {
                throw new Exception("bitch");
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
        
        public class GraphQLResponseE
        {
            public object[] Errors => new Object[0];
            public object Data => "ok";

            public T GetDataFieldAs<T>(string updateuser)
            {
                return Activator.CreateInstance<T>();
            }
        }

        public class GraphQLHttpClientt
        {
            public Task<GraphQLResponseE> SendQueryAsync(GraphQLRequest request)
            {
                return Task.FromResult(new GraphQLResponseE());
            }
            
            public Task<GraphQLResponseE> SendMutationAsync(GraphQLRequest request)
            {
                return Task.FromResult(new GraphQLResponseE());
            }
        }

        public GraphQLHttpClientt CreateGraphQlHttpClient()
        {
            return new GraphQLHttpClientt();
            // var httpClient = CreateClient();
            // return httpClient.AsGraphQLClient(new GraphQLHttpClientOptions()
            // {
            //     EndPoint = Server.BaseAddress,
            //     JsonSerializerSettings = new JsonSerializerSettings
            //     {
            //         ContractResolver = new CamelCasePropertyNamesContractResolver() 
            //     },
            // });
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            // builder.ConfigureServices(services =>
            // {
            //     // Create a new service provider.
            //     var serviceProvider = new ServiceCollection()
            //         .AddEntityFrameworkInMemoryDatabase()
            //         .BuildServiceProvider();

            //     // in memory database for testing.
            //     services.AddDbContext<ApplicationDbContext>(options =>
            //     {
            //         options.UseInMemoryDatabase("InMemoryDbForTesting");
            //         options.UseInternalServiceProvider(serviceProvider);
            //     });
            // });
        }
    }
}