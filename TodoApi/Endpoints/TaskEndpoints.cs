using Microsoft.AspNetCore.Http.HttpResults;
using System.ComponentModel.DataAnnotations;
using TodoApi.Exceptions;
using TodoApi.Extensions;
using TodoApi.Models;
using TodoApi.Services;

namespace TodoApi.Endpoints;

/// <summary>
/// Handles HTTP endpoints for task management
/// </summary>
public static class TaskEndpoints
{
    /// <summary>
    /// Maps all task-related endpoints to the application
    /// </summary>
    public static IEndpointRouteBuilder MapTaskEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/tasks", CreateTaskAsync)
            .WithName("CreateTask")
            .WithOpenApi()
            .WithDescription("Creates a new task")
            .Produces<TaskItem>(StatusCodes.Status201Created)
            .ProducesValidationProblem();

        app.MapGet("/tasks", GetTasksAsync)
            .WithName("GetTasks")
            .WithOpenApi()
            .WithDescription("Gets all tasks")
            .Produces<IEnumerable<TaskItem>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);

        app.MapPut("/tasks/{id}", UpdateTaskAsync)
            .WithName("UpdateTask")
            .WithOpenApi()
            .WithDescription("Updates a task")
            .Produces<TaskItem>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .ProducesValidationProblem();

        app.MapDelete("/tasks/{id}", DeleteTaskAsync)
            .WithName("DeleteTask")
            .WithOpenApi()
            .WithDescription("Deletes a task by ID")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);

        return app;
    }

    private static async Task<Results<Created<TaskItem>, ValidationProblem>> CreateTaskAsync(TaskItem task, ITaskService taskService)
    {
        if (task == null)
        {
            return TypedResults.ValidationProblem(new Dictionary<string, string[]>
            {
                { "task", new[] { "Task object cannot be null" } }
            });
        }

        var validationErrors = task.GetValidationErrors();
        if (validationErrors.Count > 0)
        {
            return TypedResults.ValidationProblem(validationErrors);
        }

        var result = await taskService.CreateTaskAsync(task);
        return TypedResults.Created($"/tasks/{result.Id}", result);
    }

    private static async Task<Results<Ok<IEnumerable<TaskItem>>, BadRequest>> GetTasksAsync(ITaskService taskService)
    {
        var results = await taskService.GetTasksAsync();
        return TypedResults.Ok(results);
    }

    private static async Task<Results<Ok<TaskItem>, NotFound<object>, ValidationProblem>> UpdateTaskAsync(Guid id, TaskItem task, ITaskService taskService)
    {
        if (task == null)
        {
            return TypedResults.ValidationProblem(new Dictionary<string, string[]>
            {
                { "task", new[] { "Task object cannot be null" } }
            });
        }

        var validationErrors = task.GetValidationErrors();
        if (validationErrors.Count > 0)
        {
            return TypedResults.ValidationProblem(validationErrors);
        }

        if (id != task.Id)
        {
            task.Id = id;
        }

        try
        {
            var result = await taskService.UpdateTaskAsync(task);
            return TypedResults.Ok(result);
        }
        catch (TaskDoesNotExistException)
        {
            // Create an anonymous object with proper casting to object
            object errorObj = new { error = "Task not found" };
            return TypedResults.NotFound(errorObj);
        }
    }

    private static async Task<Results<NoContent, NotFound>> DeleteTaskAsync(Guid id, ITaskService taskService)
    {
        if (id == Guid.Empty)
        {
            return TypedResults.NotFound();
        }

        try
        {
            var result = await taskService.DeleteTaskAsync(id);
            if (result)
            {
                return TypedResults.NoContent();
            }
            return TypedResults.NotFound();
        }
        catch (TaskDoesNotExistException)
        {
            return TypedResults.NotFound();
        }
    }
}
