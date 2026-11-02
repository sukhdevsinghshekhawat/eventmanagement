using System.ComponentModel.DataAnnotations;

namespace MyMvcApp.Validation;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public sealed class DepartmentCodeAttribute : ValidationAttribute
{
    private static readonly HashSet<string> Departments = new(StringComparer.OrdinalIgnoreCase)
    {
        "MCA", "BCA", "B.Tech", "MBA", "BBA"
    };

    public override bool IsValid(object? value) => value is null || value is string department && Departments.Contains(department.Trim());

    public override string FormatErrorMessage(string name) =>
        $"Select a valid department: {string.Join(", ", Departments.OrderBy(value => value))}.";
}
