using MyMvcApp.DTOs.Requests;
using MyMvcApp.DTOs.Responses;

namespace MyMvcApp.Interfaces.Services;

public interface IApplicationService
{
    Task<ApplicationResponseDto> SubmitAsync(SubmitApplicationRequestDto request, string studentId);
    Task<IReadOnlyList<ApplicationResponseDto>> GetForStudentAsync(string studentId);
    Task<IReadOnlyList<ApplicationResponseDto>> GetForReviewerAsync(string reviewerId);
    Task<ApplicationResponseDto> ReviewAsync(int id, ReviewApplicationRequestDto request, string reviewerId);
}
