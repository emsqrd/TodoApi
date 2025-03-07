using Microsoft.EntityFrameworkCore;
using TodoApi.Data;
using TodoApi.Exceptions;
using TodoApi.Extensions;
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

    public TaskService(TodoDbContext dbContext, ILogger<TaskService> logger)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<TaskItem> CreateTaskAsync(TaskItem task)
    {
        ArgumentNullException.ThrowIfNull(task, nameof(task));

        // Validation should be handled at the endpoint level
        var newTask = new TaskItem
        {
            Name = task.Name,
            Description = task.Description,
            DueDate = task.DueDate?.ToUniversalTime(),
            CreateDate = DateTimeOffset.UtcNow,
            UpdateDate = DateTimeOffset.UtcNow,
        };

        await _dbContext.Tasks.AddAsync(newTask);
        await _dbContext.SaveChangesAsync();
        return newTask;
    }

    public async Task<IEnumerable<TaskItem>> GetTasksAsync() => await _dbContext.Tasks.ToListAsync();

    public async Task<TaskItem> UpdateTaskAsync(TaskItem task)
    {
        ArgumentNullException.ThrowIfNull(task, nameof(task));

        var existingTask = await _dbContext.Tasks.FirstOrDefaultAsync(t => t.Id == task.Id);
        if (existingTask is null)
        {
            _logger.LogWarning("Task with id {TaskId} not found", task.Id);
            throw new TaskDoesNotExistException(task.Id);
        }

        existingTask.Name = task.Name;
        existingTask.Description = task.Description;
        existingTask.DueDate = task.DueDate?.ToUniversalTime();
        existingTask.UpdateDate = DateTimeOffset.UtcNow;

        await _dbContext.SaveChangesAsync();
        return existingTask;
    }

    public async Task<bool> DeleteTaskAsync(Guid id)
    {
        var task = await _dbContext.Tasks.FindAsync(id);

        if (task is null)
        {
            _logger.LogWarning("Task with id {TaskId} not found", id);
            throw new TaskDoesNotExistException(id);
        }

        _dbContext.Tasks.Remove(task);
        await _dbContext.SaveChangesAsync();
        return true;
    }
}
