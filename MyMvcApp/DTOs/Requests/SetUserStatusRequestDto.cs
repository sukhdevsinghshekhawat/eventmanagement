using System.ComponentModel.DataAnnotations;

namespace MyMvcApp.DTOs.Requests;

public sealed class SetUserStatusRequestDto
{
    [Required]
    public bool? IsActive { get; init; }
}
