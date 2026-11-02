using System.ComponentModel.DataAnnotations;

namespace MyMvcApp.Validation;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public sealed class NotWhiteSpaceAttribute : ValidationAttribute
{
    public NotWhiteSpaceAttribute() => ErrorMessage = "{0} cannot be blank.";

    public override bool IsValid(object? value) => value is null || value is string text && !string.IsNullOrWhiteSpace(text);
}
