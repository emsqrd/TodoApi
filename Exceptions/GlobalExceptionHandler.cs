using System;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace TodoApi.Exceptions;

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
			Detail = exception.Message,
			Instance = httpContext.Request.Path,
		};

		switch (exception)
		{
			case TaskDoesNotExistException:
				errorResponse.Title = "Task does not exist";
				errorResponse.Status = StatusCodes.Status404NotFound;
				break;
			case NoTaskFoundException:
				errorResponse.Title = "Task not found";
				errorResponse.Status = StatusCodes.Status404NotFound;
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
