namespace TodoApi.Models;

public sealed class TaskItem
{
    public Guid Id { get; init; }
    public required string Name { get; init; }
    public DateTimeOffset? DueDate { get; init; }
}