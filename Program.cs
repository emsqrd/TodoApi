using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using TodoApi.Data;
using TodoApi.Endpoints;
using TodoApi.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.AddApplicationServices();
builder.Services.AddApplicationHealthChecks();

var app = builder.Build();

app.UseCors("AllowedOrigins");

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exceptionHandler = context.RequestServices.GetRequiredService<IExceptionHandler>();
        var exceptionFeature = context.Features.Get<IExceptionHandlerFeature>();
        if (exceptionFeature != null)
        {
            await exceptionHandler.TryHandleAsync(context, exceptionFeature.Error, context.RequestAborted);
        }
    });
});

// Apply latest migrations to the database on startup
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<TodoDbContext>();
    dbContext.Database.Migrate();
}

app.MapGroup("/api")
.MapTaskEndpoints()
.MapOpenApi();

app.MapApplicationHealthChecks();

app.UseHttpsRedirection();
app.Run();

