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
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestLogger<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehavior<,>));
            
            return services;
        }
    }
}