namespace TodoApi.Exceptions;

public class TaskDoesNotExistException : Exception
{
  private Guid Id { get; set; }

  public TaskDoesNotExistException(Guid id) : base($"Task with id {id} does not exist")
  {
    this.Id = id;
  }
}
