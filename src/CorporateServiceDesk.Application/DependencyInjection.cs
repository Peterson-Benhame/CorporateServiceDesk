using CorporateServiceDesk.Application.Tickets.Create;
using Microsoft.Extensions.DependencyInjection;

namespace CorporateServiceDesk.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<CreateTicketUseCase>();
            services.AddSingleton(TimeProvider.System);

            return services;
        }
    }
}
