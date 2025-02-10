using Microsoft.AspNetCore.Http.HttpResults;
using TodoApi.Models;
using TodoApi.Services;

namespace TodoApi.Endpoints;

/// <summary>
/// Handles HTTP endpoints for task management
/// </summary>
public class TaskEndpoints(ITaskService taskService)
{
    private readonly ITaskService _taskService = taskService ?? throw new ArgumentNullException(nameof(taskService));
    
    private static class ValidationMessages
    {
        public static readonly string[] TaskNull = ["Task object cannot be null"];
        public static readonly string[] NameRequired = ["Task name is required"];
        public static readonly string[] InvalidId = ["Invalid task ID"];
    }

    /// <summary>
    /// Maps all task-related endpoints to the application
    /// </summary>
    public void MapTaskEndpoints(IEndpointRouteBuilder app) 
    {
        app.MapPost("/tasks", CreateTask)
            .WithName("CreateTask")
            .WithOpenApi()
            .WithDescription("Creates a new task")
            .Produces<TaskItem>(StatusCodes.Status201Created)
            .ProducesValidationProblem();

        app.MapGet("/tasks", GetTasks)
            .WithName("GetTasks")
            .WithOpenApi()
            .WithDescription("Gets all tasks")
            .Produces<IEnumerable<TaskItem>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);

        app.MapPut("/tasks/{id}", UpdateTask)
            .WithName("UpdateTasks")
            .WithOpenApi()
            .WithDescription("Updates a task")
            .Produces<TaskItem>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        app.MapDelete("/tasks/{id}", DeleteTask)
            .WithName("DeleteTask")
            .WithOpenApi()
            .WithDescription("Deletes a task by ID")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);
    }
    private Results<Created<TaskItem>, ValidationProblem> CreateTask(TaskItem task) 
    {
        if (task == null)
        {
            return TypedResults.ValidationProblem(new Dictionary<string, string[]>
            {
                { "task", ValidationMessages.TaskNull }
            });
        }

        if (string.IsNullOrWhiteSpace(task.Name))
        {
            return TypedResults.ValidationProblem(new Dictionary<string, string[]>
            {
                { "name", ValidationMessages.NameRequired }
            });
        }

        var result = _taskService.CreateTask(task);
        return TypedResults.Created($"/tasks/{result.Id}", result);
    }

    private Results<Ok<IEnumerable<TaskItem>>, BadRequest> GetTasks() 
    {
        try
        {
            var results = _taskService.GetTasks();
            return TypedResults.Ok(results);
        }
        catch (Exception ex)
        {
            // Consider logging the exception here
            return TypedResults.BadRequest();
        }
    }

    private Results<Ok<TaskItem>, NotFound> UpdateTask(TaskItem task)
    {
        var result = _taskService.UpdateTask(task);
        return result is not null ? TypedResults.Ok(result) : TypedResults.NotFound();
    }

    private Results<NoContent, NotFound> DeleteTask(Guid id) 
    {
        if (id == Guid.Empty)
        {
            return TypedResults.NotFound();
        }

        var result = _taskService.DeleteTask(id);
        return result ? TypedResults.NoContent() : TypedResults.NotFound();
    }
}
