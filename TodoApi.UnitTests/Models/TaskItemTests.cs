using System.ComponentModel.DataAnnotations;
using TodoApi.Models;

namespace TodoApi.UnitTests.Models;

[TestClass]
public class TaskItemTests
{
    [TestMethod]
    public void Constructor_ShouldInitialize_WithDefaultValues()
    {
        // Act
        var task = new TaskItem { Name = "Test Task" };

        // Assert
        task.Id.Should().NotBe(Guid.Empty);
        task.Description.Should().BeEmpty();
        task.DueDate.Should().BeNull();
    }

    [TestMethod]
    public void Name_ShouldNotAllowNull()
    {
        // Arrange
        var task = new TaskItem { Name = "Test Task" };
        var context = new ValidationContext(task);
        task.Name = null!;

        // Act
        var results = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(task, context, results, validateAllProperties: true);

        // Assert
        isValid.Should().BeFalse();
        results.Should().ContainSingle()
            .Which.ErrorMessage.Should().Contain("required");
    }

    [TestMethod]
    public void Name_ShouldBeRequired()
    {
        // Arrange
        var task = new TaskItem { Name = string.Empty };
        var context = new ValidationContext(task);

        // Act
        var results = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(task, context, results, validateAllProperties: true);

        // Assert
        isValid.Should().BeFalse();
        results.Should().ContainSingle()
            .Which.ErrorMessage.Should().Contain("empty");
    }

    [TestMethod]
    public void DueDate_WhenSet_ShouldBeNullable()
    {
        // Arrange
        var task = new TaskItem { Name = "Test Task" };
        var dueDate = DateTimeOffset.UtcNow.AddDays(1);

        // Act
        task.DueDate = dueDate;

        // Assert
        task.DueDate.Should().Be(dueDate);
        task.DueDate = null;
        task.DueDate.Should().BeNull();
    }

    [TestMethod]
    public void CreateDate_AndUpdateDate_ShouldBeRequired()
    {
        // Arrange
        var task = new TaskItem { Name = "Test Task" };
        var now = DateTimeOffset.UtcNow;

        // Act
        task.CreateDate = now;
        task.UpdateDate = now;

        // Assert
        task.CreateDate.Should().Be(now);
        task.UpdateDate.Should().Be(now);
    }
}