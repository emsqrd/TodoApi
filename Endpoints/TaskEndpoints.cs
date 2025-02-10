using Microsoft.AspNetCore.Http.HttpResults;
using TodoApi.Models;
using TodoApi.Services;

namespace TodoApi.Endpoints;

public class TaskEndpoints(ITaskService taskService)
{
    private readonly ITaskService _taskService = taskService ?? throw new ArgumentNullException();

    public void MapTaskEndpoints(IEndpointRouteBuilder app) 
    {    
        app.MapGet("/tasks", GetTasks)
            .WithName("GetTasks")
            .WithOpenApi();

        app.MapPost("/tasks", CreateTask)
            .WithName("CreateTask")
            .WithOpenApi();

        app.MapDelete("/tasks/{id}", DeleteTask)
            .WithName("DeleteTask")
            .WithOpenApi();
    }

    private Ok<IEnumerable<TaskItem>> GetTasks() 
    {
        var results = _taskService.GetTasks();
        return TypedResults.Ok(results);
    }

    private Ok<TaskItem> CreateTask(TaskItem task) 
    {
        var result = _taskService.CreateTask(task);
        return TypedResults.Ok(result);
    }

    private IResult DeleteTask(Guid id) 
    {
        var result = _taskService.DeleteTask(id);
        return result ? TypedResults.NoContent() : TypedResults.NotFound();
    }
}
