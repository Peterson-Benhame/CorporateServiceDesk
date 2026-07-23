using CorporateServiceDesk.Application.Common.Abstractions.Persistence;
using CorporateServiceDesk.Domain.Tickets.Entities;
using Microsoft.EntityFrameworkCore;

namespace CorporateServiceDesk.Infrastructure.Persistence.Contexts;

/// <summary>
/// O ApplicationDbContext representa uma sessão de trabalho com o banco.
/// Ele será responsável por:
/// rastrear entidades;
/// consultar dados;
/// registrar inclusões e alterações;
/// aplicar os mapeamentos;
/// confirmar mudanças com SaveChangesAsync.
/// O EF Core recomenda configurar o DbContext por meio de DbContextOptions, permitindo que o provedor e a connection string sejam definidos externamente pela injeção de dependência
/// </summary>
public sealed class ApplicationDbContext : DbContext, IUnitOfWork
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Ticket> Tickets => Set<Ticket>();

    public Task<int> CommitAsync(CancellationToken cancellationToken = default)
        => SaveChangesAsync(cancellationToken);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(ApplicationDbContext).Assembly);
    }
}