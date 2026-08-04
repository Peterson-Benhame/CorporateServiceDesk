using CorporateServiceDesk.Api.Filter;
using CorporateServiceDesk.Api.Helpers;
using CorporateServiceDesk.Application;
using CorporateServiceDesk.Infrastructure;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using System.Net.Mime;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

var applicationAssembly = Assembly.GetExecutingAssembly();
var applicationVersion = applicationAssembly.GetName().Version?.ToString() ?? "unknown";
var applicationCommit = ResolveCommit(builder.Configuration, applicationAssembly);

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new JsonStringEnumConverter(allowIntegerValues: false));
    });

builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = context =>
    {
        context.ProblemDetails.Extensions["timestampUtc"] =
            DateTimeOffset.UtcNow;

        context.ProblemDetails.Extensions["environment"] =
            builder.Environment.EnvironmentName;
    };
});

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddRepositories();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SchemaFilter<EnumSchemaFilter>();

    string xmlFile = $"{applicationAssembly.GetName().Name}.xml";
    string xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

    options.IncludeXmlComments(xmlPath);
});

builder.Services.AddHealthChecks();

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (!app.Environment.IsProduction())
{
    app.UseHttpsRedirection();
}

app.UseAuthorization();
app.MapControllers();

app.MapGet(
        "/version",
        () => Results.Ok(
            new
            {
                application = "CorporateServiceDesk.Api",
                version = applicationVersion,
                commit = applicationCommit,
                environment = app.Environment.EnvironmentName
            }))
    .WithName("GetApplicationVersion")
    .WithTags("Diagnostics")
    .Produces(StatusCodes.Status200OK);

app.MapHealthChecks(
    "/health",
    new HealthCheckOptions
    {
        ResponseWriter = async (context, report) =>
        {
            var result = JsonSerializer.Serialize(
                new
                {
                    statusApplication = report.Status.ToString(),
                    application = "CorporateServiceDesk.Api",
                    version = applicationVersion,
                    commit = applicationCommit,
                    currentTime = DateTimeOffset.UtcNow,
                    environment = app.Environment.EnvironmentName
                });

            context.Response.ContentType = MediaTypeNames.Application.Json;
            await context.Response.WriteAsync(result);
        }
    });

app.Run();

static string ResolveCommit(IConfiguration configuration, Assembly assembly)
{
    string? configuredCommit =
        configuration["RENDER_GIT_COMMIT"] ??
        configuration["APP_VERSION"];

    if (!string.IsNullOrWhiteSpace(configuredCommit))
    {
        return configuredCommit;
    }

    string? informationalVersion = assembly
        .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
        .InformationalVersion;

    if (string.IsNullOrWhiteSpace(informationalVersion))
    {
        return "unknown";
    }

    int metadataSeparator = informationalVersion.IndexOf('+');

    return metadataSeparator >= 0
        ? informationalVersion[(metadataSeparator + 1)..]
        : informationalVersion;
}

public partial class Program;
