using System.ComponentModel.DataAnnotations;

namespace TodoApi.Extensions;

public static class ValidationExtensions
{
    public static bool IsValid(this object obj)
    {
        if (obj == null) return false;
        var context = new ValidationContext(obj);
        return !Validate(obj, context).Any();
    }

    public static ICollection<ValidationResult> Validate(this object obj, ValidationContext context)
    {
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(obj, context, results, validateAllProperties: true);
        return results;
    }

    /// <summary>
    /// Validates the object and returns a dictionary suitable for use with ValidationProblem
    /// </summary>
    public static Dictionary<string, string[]> GetValidationErrors(this object obj)
    {
        var context = new ValidationContext(obj);
        var validationResults = Validate(obj, context);

        if (!validationResults.Any())
            return new Dictionary<string, string[]>();

        return validationResults
            .GroupBy(r => r.MemberNames.FirstOrDefault() ?? "")
            .ToDictionary(
                g => string.IsNullOrEmpty(g.Key) ? "Error" : g.Key,
                g => g.Select(r => r.ErrorMessage ?? "Validation error occurred").ToArray()
            );
    }
}