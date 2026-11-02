using System.ComponentModel.DataAnnotations;
using MyMvcApp.Models;

namespace MyMvcApp.DTOs.Requests;

public sealed class ReviewApplicationRequestDto
{
    [EnumDataType(typeof(ApplicationStatus))]
    public ApplicationStatus Status { get; init; }

    [StringLength(500)]
    public string? Notes { get; init; }
}
