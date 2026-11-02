namespace MyMvcApp.DTOs.Responses;

public sealed record ManagedUserResponseDto(string Id, string? Email, string? FullName, string? Department, bool IsActive);
