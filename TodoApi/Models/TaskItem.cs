using TodoApi.Models.Validation;

namespace TodoApi.Models;

public sealed class TaskItem
{
    public TaskItem()
    {
        Id = Guid.NewGuid();
        Description = string.Empty;
    }

    public Guid Id { get; set; }

    [TaskNameValidation]
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;
    public DateTimeOffset? DueDate { get; set; }
    public DateTimeOffset UpdateDate { get; set; }
    public DateTimeOffset CreateDate { get; set; }
}