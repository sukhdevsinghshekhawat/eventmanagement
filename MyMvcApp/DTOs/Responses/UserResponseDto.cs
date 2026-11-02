namespace MyMvcApp.DTOs.Responses;

public sealed record UserResponseDto(string Id, string? Email, string? FullName, string? Department, string Role, IReadOnlyList<string> Roles);
