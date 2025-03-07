using Microsoft.EntityFrameworkCore;
using TodoApi.Data;
using TodoApi.Exceptions;
using TodoApi.Services;

namespace TodoApi.Extensions;

public static class ServiceExtensions
{
    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {      
		builder.Services.AddEndpointsApiExplorer();
		builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
		builder.Services.AddProblemDetails();
		builder.Services.AddServices();
		builder.Services.AddDbContext(builder.Configuration);

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
    }

	private static void AddServices(this IServiceCollection services)
	{
		services.AddScoped<ITaskService, TaskService>();
	}

	private static void AddDbContext(this IServiceCollection services, IConfiguration configuration)
	{		
		services.AddDbContext<TodoDbContext>(options =>
			options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"))
		);
	}
}
