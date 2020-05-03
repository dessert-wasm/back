using System;
using System.IO;
using Dessert.Persistance;
using Dessert.Utilities.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Dessert
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var app = new CommandLineApplication(throwOnUnexpectedArg: false);

            app.HelpOption("-h |--help");

            var host = new WebHostBuilder()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .ConfigureAppConfiguration((context, builder) =>
                {
                    builder.Sources.Clear();

                    var env = context.HostingEnvironment;

                    builder.AddYamlFile("settings.yaml", optional: true);
                    builder.AddYamlFile($"settings.{env.EnvironmentName.ToLower()}.yaml", optional: true);

                    builder.AddEnvironmentVariables();

                    if (args != null)
                        builder.AddCommandLine(args);
                })
                .ConfigureLogging((context, logging) =>
                {
                    logging.AddConfiguration(context.Configuration.GetSection("Logging"));
                    logging.AddConsole();
                    logging.AddDebug();
                    logging.AddEventSourceLogger();
                })
                .UseDefaultServiceProvider((context, options) =>
                {
                    var isDevelopment = context.HostingEnvironment.IsDevelopment();
                    options.ValidateScopes = isDevelopment;
                    options.ValidateOnBuild = isDevelopment;
                })
                .UseStartup<Startup>()
                .UseKestrel((builderContext, options) =>
                {
                    options.Configure(builderContext.Configuration.GetSection("Kestrel"));
                })
                .Build();

            var logger = host.Services.CreateScope().ServiceProvider.GetRequiredService<ILogger<Program>>();

            app.Command("migrate",
                c =>
                {
                    c.Description = "Create and initialize the database";

                    var withFakeData = c.Option("-f |--with-fake", "add fake data", CommandOptionType.NoValue);

                    c.OnExecute(() =>
                    {
                        var withOrWithout = withFakeData.HasValue() ? string.Empty : "out";
                        logger.LogInformation($"Migrating data, with{withOrWithout} fake");

                        using (var serviceScope = host.Services.CreateScope())
                        {
                            DbFakesOptions dbFakesOptions = null;
                            if (withFakeData.HasValue())
                            {
                                var rng = new Random();
                                dbFakesOptions = new DbFakesOptions()
                                {
                                    ModuleCount = () => rng.Next(250, 400),
                                    ReplacementsCount = () => rng.Next(50, 100),
                                    ReplacementPerModule = () => rng.Next(0, 3),
                                    TagPerModule = () => rng.Next(2, 6),
                                };
                            }

                            var seeder = new DbSeeder(serviceScope.ServiceProvider,
                                new DbSeederOptions()
                                {
                                    FakesOptions = dbFakesOptions,
                                });
                            try
                            {
                                seeder.Seed().Wait();
                                logger.LogInformation("Migration finished");
                            }
                            catch (Exception e)
                            {
                                logger.LogError(e, "Fatal error while migrating or seeding the database");
                                return 1;
                            }
                        }

                        return 0;
                    });
                });

            app.Command("start",
                c =>
                {
                    c.Description = "Start the application";

                    c.OnExecute(() =>
                    {
                        logger.LogInformation($"Start the application");
                        host.Run();
                        return 0;
                    });
                });
            app.Execute(args);
        }
    }
}