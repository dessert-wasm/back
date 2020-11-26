using System.Reflection;
using Dessert.Application.Behavior;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Dessert.Application
{
    public static class DependecyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddMediatR(Assembly.GetExecutingAssembly());
            
            return services;
        }
    }
}