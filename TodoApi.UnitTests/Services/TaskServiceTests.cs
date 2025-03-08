using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TodoApi.Data;
using TodoApi.Exceptions;
using TodoApi.Models;
using TodoApi.Services;

namespace TodoApi.UnitTests.Services;

[TestClass]
public class TaskServiceTests
{
    private TodoDbContext _dbContext = null!;
    private Mock<ILogger<TaskService>> _loggerMock = null!;
    private TaskService _taskService = null!;

    [TestInitialize]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<TodoDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _dbContext = new TodoDbContext(options);
        _loggerMock = new Mock<ILogger<TaskService>>();
        _taskService = new TaskService(_dbContext, _loggerMock.Object);
    }

    [TestMethod]
    public async Task CreateTaskAsync_WithValidTask_ReturnsCreatedTask()
    {
        // Arrange
        var task = new TaskItem { Name = "Test Task", Description = "Test Description" };

        // Act
        var result = await _taskService.CreateTaskAsync(task);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBe(Guid.Empty);
        result.Name.Should().Be("Test Task");
        result.Description.Should().Be("Test Description");
        result.CreateDate.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(1));
        result.UpdateDate.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(1));
    }

    [TestMethod]
    public async Task CreateTaskAsync_WithNullTask_ThrowsArgumentNullException()
    {
        // Act & Assert
        await _taskService.Invoking(s => s.CreateTaskAsync(null!))
            .Should().ThrowAsync<ArgumentNullException>();
    }

    [TestMethod]
    public async Task GetTasksAsync_ReturnsAllTasks()
    {
        // Arrange
        var tasks = new[]
        {
            new TaskItem { Name = "Task 1" },
            new TaskItem { Name = "Task 2" }
        };
        await _dbContext.Tasks.AddRangeAsync(tasks);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _taskService.GetTasksAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(t => t.Name == "Task 1");
        result.Should().Contain(t => t.Name == "Task 2");
    }

    [TestMethod]
    public async Task UpdateTaskAsync_WithValidTask_ReturnsUpdatedTask()
    {
        // Arrange
        var task = new TaskItem { Name = "Original Name" };
        await _dbContext.Tasks.AddAsync(task);
        await _dbContext.SaveChangesAsync();

        var updateTask = new TaskItem
        {
            Id = task.Id,
            Name = "Updated Name",
            Description = "Updated Description"
        };

        // Act
        var result = await _taskService.UpdateTaskAsync(updateTask);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Updated Name");
        result.Description.Should().Be("Updated Description");
        result.UpdateDate.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(1));
    }

    [TestMethod]
    public async Task UpdateTaskAsync_WithNonExistentTask_ThrowsTaskDoesNotExistException()
    {
        // Arrange
        var task = new TaskItem { Id = Guid.NewGuid(), Name = "Non-existent Task" };

        // Act & Assert
        await _taskService.Invoking(s => s.UpdateTaskAsync(task))
            .Should().ThrowAsync<TaskDoesNotExistException>();
    }

    [TestMethod]
    public async Task DeleteTaskAsync_WithValidId_ReturnsTrue()
    {
        // Arrange
        var task = new TaskItem { Name = "Task to Delete" };
        await _dbContext.Tasks.AddAsync(task);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _taskService.DeleteTaskAsync(task.Id);

        // Assert
        result.Should().BeTrue();
        (await _dbContext.Tasks.FindAsync(task.Id)).Should().BeNull();
    }

    [TestMethod]
    public async Task DeleteTaskAsync_WithNonExistentId_ThrowsTaskDoesNotExistException()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act & Assert
        await _taskService.Invoking(s => s.DeleteTaskAsync(nonExistentId))
            .Should().ThrowAsync<TaskDoesNotExistException>();
    }

    [TestCleanup]
    public void Cleanup()
    {
        _dbContext.Database.EnsureDeleted();
        _dbContext.Dispose();
    }
}