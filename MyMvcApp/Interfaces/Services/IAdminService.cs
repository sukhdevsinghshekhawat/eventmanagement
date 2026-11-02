using MyMvcApp.DTOs.Requests;
using MyMvcApp.DTOs.Responses;

namespace MyMvcApp.Interfaces.Services;

public interface IAdminService
{
    Task<OperationResponseDto> CreateTeacherAsync(CreateTeacherRequestDto request);
    Task<IReadOnlyList<ManagedUserResponseDto>> GetTeachersAsync();
    Task<OperationResponseDto> DeleteTeacherAsync(string id);
    Task<IReadOnlyList<ManagedUserResponseDto>> GetStudentsAsync();
    Task<OperationResponseDto> SetStudentStatusAsync(string id, SetUserStatusRequestDto request);
}
