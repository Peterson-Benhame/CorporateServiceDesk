using CorporateServiceDesk.Application.Common.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace CorporateServiceDesk.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            Assembly assembly = typeof(DependencyInjection).Assembly;

            IEnumerable<Type> useCaseTypes = assembly
                .GetTypes()
                .Where(type => type is { IsClass: true, IsAbstract: false } && typeof(IUseCase).IsAssignableFrom(type));

            foreach (Type useCaseType in useCaseTypes)
            {
                services.AddScoped(useCaseType);
            }

            services.AddSingleton(TimeProvider.System);

            return services;
        }
    }
}
