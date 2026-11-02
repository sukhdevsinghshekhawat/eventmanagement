using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using MyMvcApp.DTOs.Requests;
using MyMvcApp.DTOs.Responses;
using MyMvcApp.Interfaces.Services;

namespace MyMvcApp.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("login")]
    [EnableRateLimiting("auth")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginRequestDto request) =>
        Ok(await authService.LoginAsync(request));

    [HttpPost("register")]
    [EnableRateLimiting("auth")]
    [ProducesResponseType(typeof(OperationResponseDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<OperationResponseDto>> Register([FromBody] RegisterStudentRequestDto request) =>
        StatusCode(StatusCodes.Status201Created, await authService.RegisterStudentAsync(request));

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpGet("me")]
    [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<UserResponseDto>> Me()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();
        return Ok(await authService.GetCurrentUserAsync(userId));
    }
}
