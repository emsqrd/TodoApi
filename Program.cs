var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

var port = Environment.GetEnvironmentVariable("PORT") ?? "5080";
app.Urls.Add($"http://0.0.0.0:{port}");

app.MapOpenApi();
app.UseHttpsRedirection();


var tasks = new[]
{
    new { id = Guid.NewGuid(), name = "Walk the dog", dueDate = "2025-02-05" },
    new { id = Guid.NewGuid(), name = "Read a book", dueDate = "2025-02-23" },
    new { id = Guid.NewGuid(), name = "Take out the garbage", dueDate = "2025-02-05" },
    new { id = Guid.NewGuid(), name = "Make dinner", dueDate = "2025-02-07" },
    new { id = Guid.NewGuid(), name = "Do laundry", dueDate = "2025-02-13" }
};

app.MapGet("/tasks", () => {
    return tasks;
})
.WithName("GetTasks");

app.Run();

