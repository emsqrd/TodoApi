using TodoApi.Models;

namespace TodoApi.Services;

public interface ITaskService 
{
    IEnumerable<TaskItem> GetTasks();
    TaskItem CreateTask(TaskItem task);
    TaskItem UpdateTask(TaskItem task);
    bool DeleteTask(Guid id);
}

public sealed class TaskService : ITaskService
{    
    private readonly List<TaskItem> _tasks =
    [
        new() { Id = Guid.NewGuid(), Name = "Walk the dog", DueDate = DateTime.Parse("2025-02-05") },
        new() { Id = Guid.NewGuid(), Name = "Read a book", DueDate = DateTime.Parse("2025-02-23") },
        new() { Id = Guid.NewGuid(), Name = "Take out the garbage", DueDate = DateTime.Parse("2025-02-05") },
        new() { Id = Guid.NewGuid(), Name = "Make dinner", DueDate = DateTime.Parse("2025-02-07") },
        new() { Id = Guid.NewGuid(), Name = "Do laundry", DueDate = DateTime.Parse("2025-02-13") }
    ];

    public TaskItem CreateTask(TaskItem task)
    {
        ArgumentNullException.ThrowIfNull(task);
        
        var newTask = new TaskItem
        {
            Id = Guid.NewGuid(),
            Name = task.Name,
            DueDate = task.DueDate
        };

        _tasks.Add(newTask);
        return newTask;
    }

    public IEnumerable<TaskItem> GetTasks() => _tasks;

    public TaskItem UpdateTask(TaskItem task)
    {
        ArgumentNullException.ThrowIfNull(task);
        var index = _tasks.FindIndex(x => x.Id == task.Id);
        if (index == -1)
            throw new KeyNotFoundException("Task not found");

        var updatedTask = new TaskItem
        {
            Id = task.Id,
            Name = task.Name,
            DueDate = task.DueDate
        };
        _tasks[index] = updatedTask;
        return updatedTask;
    }

    public bool DeleteTask(Guid id) 
    {
        var taskToDelete = _tasks.FirstOrDefault(task => task.Id == id);
        if (taskToDelete is null)
            return false;
            
        return _tasks.Remove(taskToDelete);
    }
}
