using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Text.Json;
using TodoApi.Models.HealthCheck;

namespace TodoApi.Extensions;

public static class HealthCheckExtensions
{
    public static IServiceCollection AddApplicationHealthChecks(this IServiceCollection services)
    {
        return services.AddHealthChecks()
            .AddDbContextCheck<Data.TodoDbContext>(
                name: "database",
                failureStatus: HealthStatus.Degraded,
                tags: ["database", "sql", "postgres"])
            .Services;
    }

    public static IEndpointRouteBuilder MapApplicationHealthChecks(this IEndpointRouteBuilder app)
    {
        app.MapHealthChecks("/health", new HealthCheckOptions
        {
            ResponseWriter = async (context, report) =>
            {
                var response = new HealthCheckResponse
                {
                    Status = report.Status.ToString(),
                    Duration = report.TotalDuration,
                    Components = report.Entries.Select(e => new HealthCheckComponentResponse
                    {
                        Component = e.Key,
                        Status = e.Value.Status.ToString(),
                        Description = e.Value.Description,
                        Duration = e.Value.Duration,
                        Error = e.Value.Exception?.Message
                    })
                };

                context.Response.StatusCode = report.Status switch
                {
                    HealthStatus.Healthy => StatusCodes.Status200OK,
                    HealthStatus.Degraded => StatusCodes.Status200OK, // Most cloud providers expect 200 for partial availability
                    HealthStatus.Unhealthy => StatusCodes.Status503ServiceUnavailable,
                    _ => StatusCodes.Status503ServiceUnavailable
                };

                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(JsonSerializer.Serialize(response,
                    new JsonSerializerOptions { WriteIndented = true }));
            },
            AllowCachingResponses = false,
            Predicate = _ => true
        });

        return app;
    }
}