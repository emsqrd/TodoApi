using System.ComponentModel.DataAnnotations;

namespace TodoApi.Exceptions;

public class ValidationException : Exception
{
    public Dictionary<string, string[]> ValidationErrors { get; }

    public ValidationException(string message, Dictionary<string, string[]> validationErrors)
        : base(message)
    {
        ValidationErrors = validationErrors;
    }

    public ValidationException(string message, IEnumerable<ValidationResult> validationResults)
        : base(message)
    {
        ValidationErrors = validationResults
            .GroupBy(r => r.MemberNames.FirstOrDefault() ?? "")
            .ToDictionary(
                g => string.IsNullOrEmpty(g.Key) ? "Error" : g.Key,
                g => g.Select(r => r.ErrorMessage ?? "Validation error occurred").ToArray()
            );
    }
}