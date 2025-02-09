using TodoApi.Models;

namespace TodoApi.Endpoints;

public static class TaskEndpoints
{
  public static void MapTaskEndpoints(this IEndpointRouteBuilder app) {
    
    var tasks = new List<TaskItem>
    {
        new() { Id = Guid.NewGuid(), Name = "Walk the dog", DueDate = DateTime.Parse("2025-02-05") },
        new() { Id = Guid.NewGuid(), Name = "Read a book", DueDate = DateTime.Parse("2025-02-23") },
        new() { Id = Guid.NewGuid(), Name = "Take out the garbage", DueDate = DateTime.Parse("2025-02-05") },
        new() { Id = Guid.NewGuid(), Name = "Make dinner", DueDate = DateTime.Parse("2025-02-07") },
        new() { Id = Guid.NewGuid(), Name = "Do laundry", DueDate = DateTime.Parse("2025-02-13") }
    };
    
    var apiGroup = app.MapGroup("/api");

    apiGroup.MapGet("/tasks", () => {
        return Results.Ok(tasks);
    })
    .WithName("GetTasks");

    apiGroup.MapPost("/tasks", (TaskItem task) => {
        var newTask = new TaskItem {
            Id = Guid.NewGuid(),
            Name = task.Name,
            DueDate = task.DueDate,
        };

        tasks.Add(newTask);

        return Results.Ok(newTask);

    })
    .WithName("CreateTask");
  }
}
