using CorporateServiceDesk.Application.Common.Abstractions.Persistence;
using CorporateServiceDesk.Application.Tickets.Abstractions;
using CorporateServiceDesk.Infrastructure.Persistence;
using CorporateServiceDesk.Infrastructure.Persistence.Contexts;
using CorporateServiceDesk.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CorporateServiceDesk.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration
            .GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' was not configured.");

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(connectionString);
        });

        services.AddScoped<IUnitOfWork>(serviceProvider =>
            serviceProvider.GetRequiredService<ApplicationDbContext>());

        services.AddRepositories();

        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<ITicketRepository, TicketRepository>();

        return services;
    }
}