using System.ComponentModel.DataAnnotations;

namespace MyMvcApp.DTOs.Requests;

public sealed class SubmitApplicationRequestDto
{
    [Range(1, int.MaxValue)]
    public int EventId { get; init; }

    [Required, EmailAddress, StringLength(256)]
    public string Email { get; init; } = string.Empty;

    [Required, RegularExpression(@"^[0-9+() -]{7,20}$", ErrorMessage = "Enter a valid phone number (7–20 characters)."), StringLength(30)]
    public string PhoneNumber { get; init; } = string.Empty;
}
