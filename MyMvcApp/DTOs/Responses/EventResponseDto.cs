namespace MyMvcApp.DTOs.Responses;

public sealed record EventResponseDto(
    int Id,
    string Title,
    string Description,
    string Venue,
    DateTime EventDate,
    DateTime CreatedAt,
    string? CreatedByUserId,
    IReadOnlyList<string> Departments);
