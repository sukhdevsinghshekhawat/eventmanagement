using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyMvcApp.DTOs.Requests;
using MyMvcApp.DTOs.Responses;
using MyMvcApp.Interfaces.Services;

namespace MyMvcApp.Controllers;

[ApiController]
[Route("api/applications")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public sealed class ApplicationsController(IApplicationService applicationService) : ControllerBase
{
    [HttpPost]
    [Authorize(Roles = "Student")]
    [ProducesResponseType(typeof(ApplicationResponseDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<ApplicationResponseDto>> Submit([FromBody] SubmitApplicationRequestDto request) =>
        StatusCode(StatusCodes.Status201Created, await applicationService.SubmitAsync(request, CurrentUserId()));

    [HttpGet("my")]
    [Authorize(Roles = "Student")]
    [ProducesResponseType(typeof(IReadOnlyList<ApplicationResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ApplicationResponseDto>>> MyApplications() =>
        Ok(await applicationService.GetForStudentAsync(CurrentUserId()));

    [HttpGet]
    [Authorize(Roles = "Admin,Teacher")]
    [ProducesResponseType(typeof(IReadOnlyList<ApplicationResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ApplicationResponseDto>>> GetAll() =>
        Ok(await applicationService.GetForReviewerAsync(CurrentUserId()));

    [HttpPut("{id:int}/status")]
    [Authorize(Roles = "Admin,Teacher")]
    [ProducesResponseType(typeof(ReviewApplicationResponseDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<ReviewApplicationResponseDto>> Review(int id, [FromBody] ReviewApplicationRequestDto request)
    {
        var result = await applicationService.ReviewAsync(id, request, CurrentUserId());
        return Ok(new ReviewApplicationResponseDto("Application reviewed.", result));
    }

    private string CurrentUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? throw new UnauthorizedAccessException("User identity is missing from the access token.");
}
