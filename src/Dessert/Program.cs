using System.IO;
using Dessert.Infrastructure;
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
                .ConfigureServices((context, collection) =>
                {
                    collection.AddInfrastructureDb(context.Configuration).AddInfrastructure();
                })
                .Build();

            var logger = host.Services.CreateScope().ServiceProvider.GetRequiredService<ILogger<Program>>();

            logger.LogInformation($"Start the application");
            host.Run();
        }
    }
}