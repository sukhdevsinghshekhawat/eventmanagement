using System.ComponentModel.DataAnnotations;
using MyMvcApp.Validation;

namespace MyMvcApp.DTOs.Requests;

public sealed class UpsertEventRequestDto
{
    [Required, NotWhiteSpace, StringLength(150)]
    public string Title { get; init; } = string.Empty;

    [Required, NotWhiteSpace, StringLength(2000)]
    public string Description { get; init; } = string.Empty;

    [Required, DepartmentList]
    public List<string> Departments { get; init; } = [];

    [Required, NotWhiteSpace, StringLength(100)]
    public string Venue { get; init; } = string.Empty;

    [Required]
    public DateTime EventDate { get; init; }
}
