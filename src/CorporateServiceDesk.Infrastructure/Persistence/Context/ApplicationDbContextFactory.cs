using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CorporateServiceDesk.Infrastructure.Persistence.Contexts;

/// <summary>
/// Fábrica utilizada exclusivamente pelas ferramentas de design do Entity Framework Core.
///
/// Permite que comandos como migrations add, migrations remove, migrations script,
/// migrations bundle e database update criem o ApplicationDbContext sem depender
/// da inicialização completa da API.
///
/// A connection string é obtida por variável de ambiente para evitar credenciais
/// fixas no código-fonte. Durante a execução normal da aplicação, o DbContext continua
/// sendo criado pela configuração registrada no container de injeção de dependência.
/// </summary>
public sealed class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    /// <summary>
    /// Cria uma instância do ApplicationDbContext para operações de design do Entity Framework Core.
    /// </summary>
    /// <param name="args">Argumentos recebidos pelas ferramentas do Entity Framework.</param>
    /// <returns>Uma instância configurada do ApplicationDbContext.</returns>
    /// <exception cref="InvalidOperationException">
    /// Lançada quando a variável de ambiente ConnectionStrings__DefaultConnection não está configurada.
    /// </exception>
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "A variável de ambiente 'ConnectionStrings__DefaultConnection' não foi configurada."
            );
        }

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        return new ApplicationDbContext(options);
    }
}