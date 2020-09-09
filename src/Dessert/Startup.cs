using System;
using System.Net;
using System.Threading.Tasks;
using AspNet.Security.OAuth.GitHub;
using Dessert.Authorization;
using Dessert.Domain.Entities.Identity;
using Dessert.GraphQL;
using Dessert.Persistence;
using HotChocolate;
using HotChocolate.AspNetCore;
using HotChocolate.AspNetCore.Voyager;
using HotChocolate.Configuration;
using HotChocolate.Execution.Configuration;
using HotChocolate.Types;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;

namespace Dessert
{
    public class Startup
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IConfiguration _configuration;

        public Startup(IWebHostEnvironment environment, IConfiguration configuration)
        {
            _environment = environment;
            _configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                {
                    var dbSettings = _configuration.GetSection("Database").Get<DatabaseSettings>();
                    if (dbSettings == null)
                        throw new Exception("No Database configuration found");

                    options.UseNpgsql(dbSettings.GetConnectionString(),
                        opt =>
                        {
                            opt.UseTrigrams();
                        });
                    options.EnableSensitiveDataLogging();
                },
                ServiceLifetime.Transient);

            services.AddAuthorization(options =>
            {
                options.AddPolicy(PolicyConstants.RequireAdministrator, Policies.RequireAdministrator);
            });

            services.AddIdentityCore<Account>()
                .AddRoles<AccountRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders()
                .AddSignInManager();

            services.AddHttpContextAccessor();

            services.AddRouting();

            services
                .AddAuthentication(options =>
                {
                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                })
                .AddGitHub(GitHubAuthenticationDefaults.AuthenticationScheme,
                    options =>
                    {
                        options.ClientId = _configuration["GitHub:ClientId"];
                        options.ClientSecret = _configuration["GitHub:ClientSecret"];
                        options.Scope.Add("user:email");
                        options.CorrelationCookie.HttpOnly = true;
                        options.CorrelationCookie.SameSite = SameSiteMode.None;
                        options.CorrelationCookie.SecurePolicy = CookieSecurePolicy.Always;
                    })
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme,//IdentityConstants.ApplicationScheme,
                    options =>
                    {
                        options.Cookie.Name = "auth";
                        options.ExpireTimeSpan = TimeSpan.FromDays(30);
                        options.SlidingExpiration = true;
                        options.Cookie.HttpOnly = true;
                        options.Cookie.SameSite = SameSiteMode.None;
                        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                        options.Events = new CookieAuthenticationEvents
                        {
                            OnRedirectToLogin = redirectContext =>
                            {
                                redirectContext.HttpContext.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
                                return Task.CompletedTask;
                            },
                            OnRedirectToLogout = redirectContext =>
                            {
                                redirectContext.HttpContext.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
                                return Task.CompletedTask;
                            },
                            OnRedirectToAccessDenied = redirectContext =>
                            {
                                redirectContext.HttpContext.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
                                return Task.CompletedTask;
                            }
                        };
                    });

            services.Configure<CookiePolicyOptions>(options =>
            {
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.Configure<IdentityOptions>(options =>
            {
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromDays(30);
                options.Lockout.MaxFailedAccessAttempts = 10;
                options.Lockout.AllowedForNewUsers = true;

                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 0;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredUniqueChars = 0;
                options.Password.RequireNonAlphanumeric = false;

                options.SignIn.RequireConfirmedEmail = false;
                options.SignIn.RequireConfirmedPhoneNumber = false;

                options.User.RequireUniqueEmail = true;
            });

            services.AddCors(options =>
            {
                var allowedOrigins = _configuration.GetSection("AllowedOrigins").Get<string[]>();
                if (allowedOrigins == null)
                    throw new Exception("No AllowedOrigins configuration found");

                options.AddDefaultPolicy(policy =>
                    policy.SetIsOriginAllowedToAllowWildcardSubdomains()
                        .WithOrigins(allowedOrigins)
                        .AllowAnyMethod()
                        .AllowCredentials()
                        .AllowAnyHeader());
            });

            services.AddDataLoaderRegistry();
            services.AddGraphQL(sp =>
                    SchemaBuilder.New()
                        .SetOptions(new SchemaOptions
                        {
                            DefaultBindingBehavior = BindingBehavior.Explicit,
                        })
                        .AddServices(sp)
                        .AddQueryType<QueryType>()
                        .AddMutationType<MutationType>()
                        .AddAuthorizeDirectiveType()
                        .Create(),
                new QueryExecutionOptions()
                {
                    TracingPreference =
                        _environment.IsDevelopment() ? TracingPreference.OnDemand : TracingPreference.Never,
                }
            );
            
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Latest);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            app.UseRouting();
            app.UseWebSockets();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseCors();

            app.UseGraphQL();
         
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });

            app.UsePlayground();
            app.UseVoyager();
        }
    }
}
