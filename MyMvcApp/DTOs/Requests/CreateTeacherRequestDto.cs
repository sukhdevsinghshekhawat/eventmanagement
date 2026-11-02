using System.ComponentModel.DataAnnotations;
using MyMvcApp.Validation;

namespace MyMvcApp.DTOs.Requests;

public sealed class CreateTeacherRequestDto
{
    [Required, NotWhiteSpace, StringLength(150)]
    public string FullName { get; init; } = string.Empty;

    [Required, EmailAddress, StringLength(256)]
    public string Email { get; init; } = string.Empty;

    [Required, MinLength(8), StringLength(128), RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z0-9]).{8,128}$", ErrorMessage = "Password must be 8–128 characters and include uppercase and lowercase letters, a number, and a special character.")]
    public string Password { get; init; } = string.Empty;

    [Required, NotWhiteSpace, DepartmentCode]
    public string Department { get; init; } = string.Empty;
}
