using System;
using Bogus;
using Dessert.Models;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Web;

namespace Dessert
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
            var app = new CommandLineApplication(throwOnUnexpectedArg: false);

            app.HelpOption("-h |--help");

            try
            {
                logger.Info("Initializing main");

                var webHostBuilder = CreateWebHostBuilder(args);

                //configure database
                webHostBuilder.ConfigureServices(services =>
                {
                    var configuration = Configuration.LoadSettings("settings.yaml");
                    services.AddDbContext<ApplicationDbContext>(options =>
                    {
                        if (configuration.Database.Type == DatabaseSettings.SQLiteName &&
                            configuration.Database.SQLite != null)
                            options.UseSqlite(configuration.Database.SQLiteConnectionString);
                        else if (configuration.Database.Type == DatabaseSettings.PostgresName &&
                                 configuration.Database.Postgres != null)
                            options.UseNpgsql(configuration.Database.PgConnectionString);
                        else
                            throw new Exception($"cannot identify database type: {configuration.Database.Type}");
                    });
                });

                app.Command("migrate",
                    c =>
                    {
                        c.Description = "Create and initialize the database";

                        var withFakeData = c.Option("-f |--with-fake", "add fake data", CommandOptionType.NoValue);

                        c.OnExecute(() =>
                        {
                            var withOrWithout = withFakeData.HasValue() ? string.Empty : "out";
                            logger.Info($"Migrating data, with{withOrWithout} fake");

                            using (var serviceScope = webHostBuilder.Build().Services.CreateScope())
                            {
                                var faker = new Faker();

                                DbInitializer.Initialize(serviceScope.ServiceProvider,
                                    withFakeData.HasValue(),
                                    new DbInitializerOptions
                                    {
                                        ModuleCount = () => faker.Random.Int(250, 400),
                                        ReplacementPerModule = () => faker.Random.Int(0, 3),
                                        TagPerModule = () => faker.Random.Int(2, 6),
                                    });
                            }

                            return 0;
                        });
                    });

                app.Command("start",
                    c =>
                    {
                        c.Description = "Start the application";

                        var host = c.Option("--host", "Where to listen", CommandOptionType.SingleValue);

                        c.OnExecute(() =>
                        {
                            logger.Info($"Start the application");
                            if (host.HasValue())
                                webHostBuilder.UseUrls(host.Value());
                            webHostBuilder.Build().Run();
                            return 0;
                        });
                    });
                app.Execute(args);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Stopped program because of exception");
                throw;
            }
            finally
            {
                // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
                NLog.LogManager.Shutdown();
            }
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(LogLevel.Trace);
                })
                .UseNLog(); // NLog: setup NLog for Dependency injection
    }
}