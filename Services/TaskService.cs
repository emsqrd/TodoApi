using TodoApi.Data;
using TodoApi.Models;

namespace TodoApi.Services;

public interface ITaskService 
{
    IEnumerable<TaskItem> GetTasks();
    Task<TaskItem> CreateTaskAsync(TaskItem task);
    TaskItem UpdateTask(TaskItem task);
    bool DeleteTask(Guid id);
}

public sealed class TaskService : ITaskService
{    
    private readonly List<TaskItem> _tasks = [];
    private readonly TodoDbContext _dbContext;

    public TaskService(TodoDbContext dbContext) {
        _dbContext = dbContext;
    }
    
    public async Task<TaskItem> CreateTaskAsync(TaskItem task)
    {
        ArgumentNullException.ThrowIfNull(task);
        
        var newTask = new TaskItem
        {
            Name = task.Name,
            DueDate = task.DueDate?.ToUniversalTime(),
        };

        _dbContext.Tasks.Add(newTask);
        await _dbContext.SaveChangesAsync();
        return newTask;
    }

    public IEnumerable<TaskItem> GetTasks() => _dbContext.Tasks.ToList();

    public TaskItem UpdateTask(TaskItem task)
    {
        ArgumentNullException.ThrowIfNull(task);
        var index = _tasks.FindIndex(x => x.Id == task.Id);
        if (index == -1)
            throw new KeyNotFoundException("Task not found");

        var updatedTask = new TaskItem
        {
            Name = task.Name,
            DueDate = task.DueDate?.ToUniversalTime()
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
