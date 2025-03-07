namespace TodoApi.Models.HealthCheck;

public class HealthCheckResponse
{
    public string Status { get; set; } = string.Empty;
    public TimeSpan Duration { get; set; }
    public IEnumerable<HealthCheckComponentResponse> Components { get; set; } = [];
}

public class HealthCheckComponentResponse
{
    public string Component { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? Description { get; set; }
    public TimeSpan Duration { get; set; }
    public string? Error { get; set; }
}