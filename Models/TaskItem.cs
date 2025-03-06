namespace TodoApi.Models;

public sealed class TaskItem
{
    public TaskItem()
    {
        Id = Guid.NewGuid();
        Description = string.Empty;
    }

    public Guid Id { get; set; }
    public required string Name { get; set; }
    public string Description { get; set; }
    public DateTimeOffset? DueDate { get; set; }
    public DateTimeOffset UpdateDate { get; set; }
    public DateTimeOffset CreateDate { get; set; }
}