using System.Collections.Generic;
using Microsoft.AspNetCore.Http.HttpResults;
using TodoApi.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

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

var port = Environment.GetEnvironmentVariable("PORT") ?? "5080";
app.Urls.Add($"http://0.0.0.0:{port}");

app.UseCors("AllowedOrigins");
app.MapOpenApi();
app.UseHttpsRedirection();

app.MapTaskEndpoints();

app.Run();

