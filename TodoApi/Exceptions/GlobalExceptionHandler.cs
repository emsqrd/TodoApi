using System;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace TodoApi.Exceptions;

/// <summary>
/// Handles global exceptions in the application
/// </summary>
/// <remarks>
/// This handler is for unexpected errors and validation failures.
/// Expected application errors that are part of normal API operation 
/// (like 404 Not Found) should be handled at the endpoint level.
/// </remarks>
public class GlobalExceptionHandler : IExceptionHandler
{
	private readonly ILogger<GlobalExceptionHandler> _logger;

	public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
	{
		_logger = logger ?? throw new ArgumentNullException(nameof(logger));
	}

	public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
	{
		_logger.LogError(exception, "An error occurred while processing your request");

		var errorResponse = new ProblemDetails
		{
			Detail = exception.InnerException?.Message ?? exception.Message,
			Instance = httpContext.Request.Path,
		};

		switch (exception)
		{
			case ValidationException validationException:
				errorResponse.Title = "Validation failed";
				errorResponse.Status = StatusCodes.Status400BadRequest;
				errorResponse.Extensions["errors"] = validationException.ValidationErrors;
				break;
			default:
				errorResponse.Title = "An error occurred while processing your request";
				errorResponse.Status = StatusCodes.Status500InternalServerError;
				break;
		}

		// Set the response status code
		httpContext.Response.StatusCode = errorResponse.Status.Value;

		// Write the error response as JSON
		await httpContext.Response.WriteAsJsonAsync(errorResponse, cancellationToken);

		return true;
	}
}
