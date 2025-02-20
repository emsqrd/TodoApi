using Microsoft.EntityFrameworkCore;
using TodoApi.Data;
using TodoApi.Endpoints;
using TodoApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();

// Ensure DbContext and services are registered as scoped
builder.Services.AddDbContext<TodoDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);
builder.Services.AddScoped<ITaskService, TaskService>();

builder.Services.AddCors(options =>
{
    var allowedOrigins = builder.Configuration.GetSection("CorsOrigins").Get<string[]>() ?? [];
    
    options.AddPolicy("AllowedOrigins",
        policy =>
        {
            policy.WithOrigins(allowedOrigins)
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

var app = builder.Build();

app.UseCors("AllowedOrigins");

app.MapGroup("/api")
.MapTaskEndpoints()
.MapOpenApi();


app.UseHttpsRedirection();
app.Run();

