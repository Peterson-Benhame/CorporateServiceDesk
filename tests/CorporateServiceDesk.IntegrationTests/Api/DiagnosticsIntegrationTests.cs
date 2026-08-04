using CorporateServiceDesk.Infrastructure.Persistence.Contexts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Net;
using System.Net.Http.Json;

namespace CorporateServiceDesk.IntegrationTests.Api;

[Collection(PostgreSqlCollection.Name)]
public sealed class DiagnosticsIntegrationTests(PostgreSqlFixture database) : IAsyncLifetime
{
    public Task InitializeAsync() => database.ResetAsync();

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task Version_ShouldReturnConfiguredCommitAndEnvironment()
    {
        const string expectedCommit = "integration-test-commit";

        await using var factory = CreateFactory(expectedCommit);
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/version");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<VersionPayload>();

        Assert.NotNull(payload);
        Assert.Equal("CorporateServiceDesk.Api", payload.Application);
        Assert.Equal(expectedCommit, payload.Commit);
        Assert.Equal("Development", payload.Environment);
        Assert.False(string.IsNullOrWhiteSpace(payload.Version));
    }

    [Fact]
    public async Task Health_ShouldReturnHealthyStatusAndConfiguredCommit()
    {
        const string expectedCommit = "integration-test-commit";

        await using var factory = CreateFactory(expectedCommit);
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<HealthPayload>();

        Assert.NotNull(payload);
        Assert.Equal("Healthy", payload.StatusApplication);
        Assert.Equal(expectedCommit, payload.Commit);
    }

    private WebApplicationFactory<Program> CreateFactory(string commit) =>
        new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseSetting("APP_VERSION", commit);
                builder.ConfigureTestServices(services =>
                {
                    services.RemoveAll<DbContextOptions<ApplicationDbContext>>();
                    services.RemoveAll<ApplicationDbContext>();
                    services.AddDbContext<ApplicationDbContext>(
                        options => options.UseNpgsql(database.ConnectionString));
                });
            });

    private sealed record VersionPayload(
        string Application,
        string Version,
        string Commit,
        string Environment);

    private sealed record HealthPayload(
        string StatusApplication,
        string Commit);
}
