namespace MyMvcApp.DTOs.Responses;

public sealed record AuthResponseDto(string AccessToken, string TokenType, int ExpiresIn, string? Role, string? FullName, string? Department);
