using System.ComponentModel.DataAnnotations;

namespace TodoApi.Extensions;

public static class ValidationExtensions
{
    public static bool IsValid(this object obj)
    {
        if (obj == null) return false;
        var context = new ValidationContext(obj);
        return Validate(obj, context).Count == 0;
    }

    public static ICollection<ValidationResult> Validate(this object obj, ValidationContext context)
    {
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(obj, context, results, validateAllProperties: true);
        return results;
    }
}