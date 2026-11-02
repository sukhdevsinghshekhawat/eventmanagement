using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace MyMvcApp.Validation;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public sealed class DepartmentListAttribute : ValidationAttribute
{
    private static readonly HashSet<string> Departments = new(StringComparer.OrdinalIgnoreCase)
    {
        "MCA", "BCA", "B.Tech", "MBA", "BBA"
    };

    public override bool IsValid(object? value)
    {
        if (value is null) return true;
        if (value is not IEnumerable items) return false;
        var values = items.Cast<object?>().OfType<string>().Select(item => item.Trim()).ToArray();
        return values.Length > 0 && values.All(Departments.Contains);
    }

    public override string FormatErrorMessage(string name) =>
        $"Choose at least one valid department: {string.Join(", ", Departments.OrderBy(value => value))}.";
}
