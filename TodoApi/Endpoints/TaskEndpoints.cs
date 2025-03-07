using Microsoft.AspNetCore.Http.HttpResults;
using System.ComponentModel.DataAnnotations;
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
            .Produces(StatusCodes.Status404NotFound);

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
        if (!task.IsValid())
        {
            var validationResults = task.Validate(new ValidationContext(task));
            var errors = validationResults.ToDictionary(
                vr => vr.MemberNames.First(),
                vr => new[] { vr.ErrorMessage ?? "Validation failed" });

            return TypedResults.ValidationProblem(errors);
        }

        var result = await taskService.CreateTaskAsync(task);
        return TypedResults.Created($"/tasks/{result.Id}", result);
    }

    private static async Task<Results<Ok<IEnumerable<TaskItem>>, BadRequest>> GetTasksAsync(ITaskService taskService)
    {
        var results = await taskService.GetTasksAsync();
        return TypedResults.Ok(results);
    }

    private static async Task<Results<Ok<TaskItem>, NotFound<object>>> UpdateTaskAsync(Guid id, TaskItem task, ITaskService taskService)
    {
        if (id != task.Id)
        {
            task.Id = id;
        }

        var result = await taskService.UpdateTaskAsync(task);
        return result is not null ? TypedResults.Ok(result) : TypedResults.NotFound((object)new { error = "Task not found" });
    }

    private static async Task<Results<NoContent, NotFound>> DeleteTaskAsync(Guid id, ITaskService taskService)
    {
        if (id == Guid.Empty)
        {
            return TypedResults.NotFound();
        }

        var result = await taskService.DeleteTaskAsync(id);
        return result ? TypedResults.NoContent() : TypedResults.NotFound();
    }
}
