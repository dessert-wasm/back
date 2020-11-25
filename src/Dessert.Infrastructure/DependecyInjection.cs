using System;
using System.Reflection;
using Dessert.Application.Interfaces;
using Dessert.Application.Repositories;
using Dessert.Domain.Entities.Identity;
using Dessert.Infrastructure.Identity;
using Dessert.Infrastructure.Persistence;
using Dessert.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Dessert.Infrastructure
{
    public static class DependecyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                {
                    var dbSettings = configuration.GetSection("Database").Get<DatabaseSettings>();
                    if (dbSettings == null)
                        throw new Exception("No Database configuration found");

                    options.UseNpgsql(dbSettings.GetConnectionString());
                    options.EnableSensitiveDataLogging();
                },
                ServiceLifetime.Transient);

            services.AddScoped<IIdentityService, IdentityService>();
            
            services.AddScoped<IModuleRepository, ModuleRepository>();
            services.AddScoped<ITagRepository, TagRepository>();
            services.AddScoped<ITokenRepository, TokenRepository>();
            
            services.AddIdentityCore<ApplicationUser>()
                .AddRoles<AccountRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders()
                .AddSignInManager();
            
            return services;
        }
    }
}