using System;

namespace TodoApi.Models;

public class TaskItem {
  public Guid Id {get; set;}
  public string? Name { get; set; }
  public DateTime DueDate { get; set; }
}