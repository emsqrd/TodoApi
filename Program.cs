using TodoApi.Endpoints;
using TodoApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddSingleton<ITaskService, TaskService>();
builder.Services.AddSingleton<TaskEndpoints>();

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

// var port = Environment.GetEnvironmentVariable("PORT") ?? "5080";
// app.Urls.Add($"http://0.0.0.0:{port}");

app.UseCors("AllowedOrigins");

var apiGroup = app.MapGroup("/api");
apiGroup.MapOpenApi();

var taskEndpoints = app.Services.GetRequiredService<TaskEndpoints>();
taskEndpoints.MapTaskEndpoints(apiGroup);

app.UseHttpsRedirection();
app.Run();

