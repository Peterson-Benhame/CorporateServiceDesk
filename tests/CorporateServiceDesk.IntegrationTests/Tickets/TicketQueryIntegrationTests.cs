using CorporateServiceDesk.Application.Common.Pagination;
using CorporateServiceDesk.Application.Tickets.Queries.List;
using CorporateServiceDesk.Application.Tickets.Queries.List.Specifications;
using CorporateServiceDesk.Domain.Tickets.Entities;
using CorporateServiceDesk.Domain.Tickets.Enums;
using CorporateServiceDesk.Infrastructure.Persistence.Contexts;
using CorporateServiceDesk.Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Net;
using System.Net.Http.Json;

namespace CorporateServiceDesk.IntegrationTests.Tickets;

[Collection(PostgreSqlCollection.Name)]
public sealed class TicketQueryIntegrationTests(PostgreSqlFixture database) : IAsyncLifetime
{
    public Task InitializeAsync() => database.ResetAsync();
    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task Repository_ShouldFilterPageCountProjectAndOrder()
    {
        await using var context = database.CreateContext();
        context.Tickets.AddRange(
            Create("VPN B", TicketPriority.High, 1),
            Create("VPN A", TicketPriority.High, 2),
            Create("Printer", TicketPriority.Low, 3));
        await context.SaveChangesAsync();

        var repository = new TicketRepository(context);
        var filter = new QueryListTicketsFilter(
            new PageRequest(1, 1, true),
            new SortRequest<TicketSortColumn>(TicketSortColumn.Title, SortDirection.Ascending),
            Priority: TicketPriority.High,
            Search: "VPN");

        var result = await repository.QueryAsync(
            new TicketListSpecification(filter),
            filter.Pagination,
            filter.Sort,
            CancellationToken.None);

        Assert.Single(result.Items);
        Assert.Equal("VPN A", result.Items[0].Title);
        Assert.Equal(2, result.TotalCount);
        Assert.Equal(2, result.TotalPages);
    }

    [Fact]
    public async Task GetTickets_ShouldReturnOkWithEmptyPage()
    {
        await using var factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder => builder.ConfigureTestServices(services =>
            {
                services.RemoveAll<DbContextOptions<ApplicationDbContext>>();
                services.RemoveAll<ApplicationDbContext>();
                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseNpgsql(database.ConnectionString));
            }));
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/tickets?countTotal=true");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var payload = await response.Content.ReadFromJsonAsync<PagePayload>();
        Assert.NotNull(payload);
        Assert.Empty(payload.Items);
        Assert.Equal(0, payload.TotalCount);
    }

    private static Ticket Create(string title, TicketPriority priority, int day) =>
        Ticket.Open(
            title, "Sufficiently detailed ticket description.", Guid.NewGuid(), priority,
            new FixedTimeProvider(new DateTimeOffset(2026, 7, day, 12, 0, 0, TimeSpan.Zero)));

    private sealed record PagePayload(IReadOnlyList<object> Items, int? TotalCount);

    private sealed class FixedTimeProvider(DateTimeOffset now) : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => now;
    }
}
