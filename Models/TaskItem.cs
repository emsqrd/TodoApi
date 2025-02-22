namespace TodoApi.Models;

public sealed class TaskItem
{
    public TaskItem()
    {
        Id = Guid.NewGuid();
    }
    
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public DateTimeOffset? DueDate { get; set; }
}