using System;

namespace TodoApi.Exceptions;

public class NoTaskFoundException : Exception
{
  public NoTaskFoundException() : base("Task not found")
  {
  }
}
