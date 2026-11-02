using MyMvcApp.DTOs.Requests;
using MyMvcApp.DTOs.Responses;

namespace MyMvcApp.Interfaces.Services;

public interface IAuthService
{
    Task<AuthResponseDto> LoginAsync(LoginRequestDto request);
    Task<OperationResponseDto> RegisterStudentAsync(RegisterStudentRequestDto request);
    Task<UserResponseDto> GetCurrentUserAsync(string userId);
}
