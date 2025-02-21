using Microsoft.EntityFrameworkCore;
using TodoApi.Data;
using TodoApi.Models;

namespace TodoApi.Services;

public interface ITaskService 
{
    Task<IEnumerable<TaskItem>> GetTasksAsync();
    Task<TaskItem> CreateTaskAsync(TaskItem task);
    Task<TaskItem> UpdateTaskAsync(TaskItem task);
    Task<bool> DeleteTaskAsync(Guid id);
}

public sealed class TaskService : ITaskService
{    
    private readonly TodoDbContext _dbContext;
    private readonly ILogger<TaskService> _logger;

    public TaskService(TodoDbContext dbContext, ILogger<TaskService> logger) {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    public async Task<TaskItem> CreateTaskAsync(TaskItem task)
    {
        ArgumentNullException.ThrowIfNull(task, nameof(task));
        
        var newTask = new TaskItem
        {
            Name = task.Name,
            DueDate = task.DueDate?.ToUniversalTime(),
        };

        await _dbContext.Tasks.AddAsync(newTask);
        await _dbContext.SaveChangesAsync();
        return newTask;
    }

    public async Task<IEnumerable<TaskItem>> GetTasksAsync() => await _dbContext.Tasks.ToListAsync();

    public async Task<TaskItem> UpdateTaskAsync(TaskItem task)
    {
        ArgumentNullException.ThrowIfNull(task);

        var existingTask = await _dbContext.Tasks.FirstOrDefaultAsync(t => t.Id == task.Id);
        if (existingTask is null) 
        {
            _logger.LogWarning("Task not found");
            throw new KeyNotFoundException("Task not found");
        }

        existingTask.Name = task.Name;
        existingTask.DueDate = task.DueDate?.ToUniversalTime();
        
        await _dbContext.SaveChangesAsync();
        return existingTask;
    }

    public async Task<bool> DeleteTaskAsync(Guid id) 
    {
        var taskToDelete = await _dbContext.Tasks.FirstOrDefaultAsync(task => task.Id == id);
        if (taskToDelete is null)
            return false;
            
        _dbContext.Tasks.Remove(taskToDelete);
        await _dbContext.SaveChangesAsync();
        return true;
    }
}
