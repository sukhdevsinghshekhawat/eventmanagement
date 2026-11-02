using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyMvcApp.DTOs.Requests;
using MyMvcApp.DTOs.Responses;
using MyMvcApp.Interfaces.Services;

namespace MyMvcApp.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
public sealed class AdminController(IAdminService adminService) : ControllerBase
{
    [HttpPost("teachers")]
    [ProducesResponseType(typeof(OperationResponseDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<OperationResponseDto>> CreateTeacher([FromBody] CreateTeacherRequestDto request)
    {
        var result = await adminService.CreateTeacherAsync(request);
        return StatusCode(StatusCodes.Status201Created, result);
    }

    [HttpGet("teachers")]
    [ProducesResponseType(typeof(IReadOnlyList<ManagedUserResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ManagedUserResponseDto>>> GetTeachers() =>
        Ok(await adminService.GetTeachersAsync());

    [HttpDelete("teachers/{id}")]
    public async Task<ActionResult<OperationResponseDto>> DeleteTeacher(string id) =>
        Ok(await adminService.DeleteTeacherAsync(id));

    [HttpGet("students")]
    [ProducesResponseType(typeof(IReadOnlyList<ManagedUserResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ManagedUserResponseDto>>> GetStudents() =>
        Ok(await adminService.GetStudentsAsync());

    [HttpPatch("users/{id}/status")]
    public async Task<ActionResult<OperationResponseDto>> SetUserStatus(string id, [FromBody] SetUserStatusRequestDto request) =>
        Ok(await adminService.SetStudentStatusAsync(id, request));
}
