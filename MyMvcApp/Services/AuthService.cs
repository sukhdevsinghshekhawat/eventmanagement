using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using MyMvcApp.DTOs.Requests;
using MyMvcApp.DTOs.Responses;
using MyMvcApp.Exceptions;
using MyMvcApp.Interfaces.Services;
using MyMvcApp.Models;

namespace MyMvcApp.Services;

public sealed class AuthService(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    RoleManager<IdentityRole> roleManager,
    IConfiguration configuration) : IAuthService
{
    private static readonly TimeSpan TokenLifetime = TimeSpan.FromMinutes(30);

    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request)
    {
        var user = await userManager.FindByEmailAsync(request.Email.Trim());
        if (user is null || !user.IsActive)
            throw new ServiceException((int)HttpStatusCode.Unauthorized, "Invalid email or password.");

        var result = await signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);
        if (!result.Succeeded)
            throw new ServiceException((int)HttpStatusCode.Unauthorized, "Invalid email or password.");

        var roles = await userManager.GetRolesAsync(user);
        return new AuthResponseDto(await CreateTokenAsync(user, roles), "Bearer", (int)TokenLifetime.TotalSeconds,
            roles.FirstOrDefault(), user.FullName, user.Department);
    }

    public async Task<OperationResponseDto> RegisterStudentAsync(RegisterStudentRequestDto request)
    {
        if (await userManager.FindByEmailAsync(request.Email.Trim()) is not null)
            throw new ServiceException((int)HttpStatusCode.Conflict, "Email is already registered.");

        if (!await roleManager.RoleExistsAsync("Student"))
            throw new ServiceException((int)HttpStatusCode.InternalServerError, "Student role is not configured.");

        var user = new ApplicationUser
        {
            UserName = request.Email.Trim(),
            Email = request.Email.Trim(),
            FullName = request.FullName.Trim(),
            Department = request.Department.Trim(),
            EmailConfirmed = true,
            IsActive = true
        };

        var createResult = await userManager.CreateAsync(user, request.Password);
        if (!createResult.Succeeded)
            throw new ServiceException((int)HttpStatusCode.BadRequest, "Registration failed.", createResult.Errors.Select(error => error.Description).ToArray());

        var roleResult = await userManager.AddToRoleAsync(user, "Student");
        if (!roleResult.Succeeded)
        {
            await userManager.DeleteAsync(user);
            throw new ServiceException((int)HttpStatusCode.InternalServerError, "Could not assign the Student role.", roleResult.Errors.Select(error => error.Description).ToArray());
        }

        return new OperationResponseDto("Student registered successfully. Please login.");
    }

    public async Task<UserResponseDto> GetCurrentUserAsync(string userId)
    {
        var user = await userManager.FindByIdAsync(userId)
            ?? throw new ServiceException((int)HttpStatusCode.NotFound, "User not found.");
        var roles = await userManager.GetRolesAsync(user);
        return new UserResponseDto(user.Id, user.Email, user.FullName, user.Department, roles.FirstOrDefault() ?? string.Empty, roles.ToArray());
    }

    private async Task<string> CreateTokenAsync(ApplicationUser user, IEnumerable<string> roles)
    {
        var jwt = configuration.GetSection("Jwt");
        var key = jwt["Key"] ?? throw new InvalidOperationException("JWT signing key is not configured.");
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Name, user.UserName ?? string.Empty),
            new(ClaimTypes.Email, user.Email ?? string.Empty)
        };

        if (!string.IsNullOrWhiteSpace(user.FullName)) claims.Add(new Claim("fullName", user.FullName));
        if (!string.IsNullOrWhiteSpace(user.Department)) claims.Add(new Claim("department", user.Department));
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(jwt["Issuer"], jwt["Audience"], claims,
            expires: DateTime.UtcNow.Add(TokenLifetime), signingCredentials: credentials);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
