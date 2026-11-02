namespace MyMvcApp.DTOs.Responses;

public sealed record ApplicationResponseDto(
    int Id,
    int EventId,
    string? EventTitle,
    IReadOnlyList<string>? EventDepartments,
    string ApplicantUserId,
    string ApplicantName,
    string? ApplicantEmail,
    string Email,
    string PhoneNumber,
    string Department,
    string Status,
    DateTime SubmittedAt,
    string? ReviewedByUserId,
    DateTime? ReviewedAt,
    string? ReviewerNotes);

public sealed record ReviewApplicationResponseDto(string Message, ApplicationResponseDto Data);
public sealed record EventUpdatedResponseDto(string Message, EventResponseDto Data);
