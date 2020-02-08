using System;
using System.Net;
using System.Threading.Tasks;
using Dessert.Authorization;
using Dessert.GraphQL;
using Dessert.Models;
using HotChocolate;
using HotChocolate.AspNetCore;
using HotChocolate.AspNetCore.Voyager;
using HotChocolate.Configuration;
using HotChocolate.Execution.Configuration;
using HotChocolate.Types;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Dessert
{
    public class Startup
    {
        private readonly IHostingEnvironment _environment;

        public Startup(IHostingEnvironment environment)
        {
            _environment = environment;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvcCore(options =>
            {
                options.EnableEndpointRouting = false;
            })
                .AddAuthorization()
                .AddNewtonsoftJson()
                .AddCors()
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

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

            services.AddAuthentication(
                    options =>
                    {
                        options.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
                        options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
                        options.DefaultSignInScheme = IdentityConstants.ApplicationScheme;
                    })
                .AddCookie(IdentityConstants.ApplicationScheme,
                    options =>
                    {
                        options.Cookie.Name = "auth";
                        options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
                        options.SlidingExpiration = true;
                        options.Cookie.HttpOnly = false;
                        options.Cookie.SameSite = SameSiteMode.None;
                        options.Cookie.SecurePolicy = CookieSecurePolicy.None;
                        options.Events = new CookieAuthenticationEvents
                        {
                            OnRedirectToLogin = redirectContext =>
                            {
                                redirectContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                                return Task.CompletedTask;
                            },
                            OnRedirectToLogout = redirectContext =>
                            {
                                redirectContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                                return Task.CompletedTask;
                            }
                        };
                    });

            services.Configure<IdentityOptions>(options =>
            {
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
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

                options.User.RequireUniqueEmail = false;
            });

            services.AddCors();

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
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            if (_environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCookiePolicy();
            app.UseAuthentication();

            app.UseCors(policy =>
            {
                policy.AllowAnyHeader();
                policy.AllowAnyMethod();
                policy.WithOrigins(
                    "http://localhost:3000",
                    "https://dessert.ovh",
                    "https://dev.dessert.ovh"
                    );
                policy.AllowCredentials();
            });

            app.UseWebSockets();
            app.UseGraphQL();

            if (_environment.IsDevelopment())
            {
                app.UseGraphiQL();
                app.UsePlayground();
                app.UseVoyager();
            }

            app.UseMvc();
        }
    }
}
