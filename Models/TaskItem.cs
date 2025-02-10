namespace TodoApi.Models;

public sealed class TaskItem
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required DateTime DueDate { get; init; }
}