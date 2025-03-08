using System.ComponentModel.DataAnnotations;

namespace TodoApi.Models.Validation;

public class TaskNameValidationAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if (value is not string name)
            return false;

        return !string.IsNullOrWhiteSpace(name) && name.Length <= 100;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not string name)
            return new ValidationResult("Name is required");

        if (string.IsNullOrWhiteSpace(name))
            return new ValidationResult("Name cannot be empty");

        if (name.Length > 100)
            return new ValidationResult("Name cannot be longer than 100 characters");

        return ValidationResult.Success;
    }
}