using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using TodoApi.Endpoints;
using TodoApi.Models;
using TodoApi.Services;

namespace TodoApi.UnitTests.Endpoints;

[TestClass]
public class TaskEndpointsTests
{
    private Mock<ITaskService> _taskServiceMock = null!;
    private IEndpointRouteBuilder _app = null!;

    [TestInitialize]
    public void Setup()
    {
        _taskServiceMock = new Mock<ITaskService>();

        var builder = WebApplication.CreateBuilder();
        builder.Services.AddSingleton(_taskServiceMock.Object);

        _app = builder.Build();
    }

    [TestMethod]
    public void MapTaskEndpoints_RegistersAllEndpoints()
    {
        // Act
        var result = _app.MapTaskEndpoints();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeSameAs(_app);

        var endpoints = _app.DataSources
            .SelectMany(ds => ds.Endpoints)
            .Cast<RouteEndpoint>()
            .ToList();

        // Verify all expected endpoints are present
        var postEndpoint = endpoints.FirstOrDefault(e =>
            e.RoutePattern.RawText == "/tasks" &&
            GetHttpMethods(e).Contains("POST"));

        var getEndpoint = endpoints.FirstOrDefault(e =>
            e.RoutePattern.RawText == "/tasks" &&
            GetHttpMethods(e).Contains("GET"));

        var putEndpoint = endpoints.FirstOrDefault(e =>
            e.RoutePattern.RawText == "/tasks/{id}" &&
            GetHttpMethods(e).Contains("PUT"));

        var deleteEndpoint = endpoints.FirstOrDefault(e =>
            e.RoutePattern.RawText == "/tasks/{id}" &&
            GetHttpMethods(e).Contains("DELETE"));

        // Assert endpoints exist
        postEndpoint.Should().NotBeNull();
        getEndpoint.Should().NotBeNull();
        putEndpoint.Should().NotBeNull();
        deleteEndpoint.Should().NotBeNull();

        // Assert HTTP methods
        GetHttpMethods(postEndpoint).Should().Contain("POST");
        GetHttpMethods(getEndpoint).Should().Contain("GET");
        GetHttpMethods(putEndpoint).Should().Contain("PUT");
        GetHttpMethods(deleteEndpoint).Should().Contain("DELETE");
    }

    private static IEnumerable<string> GetHttpMethods(Endpoint? endpoint)
    {
        var metadata = endpoint?.Metadata.GetMetadata<HttpMethodMetadata>();
        return metadata?.HttpMethods ?? Array.Empty<string>();
    }
}