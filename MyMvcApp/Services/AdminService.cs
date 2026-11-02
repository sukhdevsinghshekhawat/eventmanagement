using System.Net;
using Microsoft.AspNetCore.Identity;
using MyMvcApp.DTOs.Requests;
using MyMvcApp.DTOs.Responses;
using MyMvcApp.Exceptions;
using MyMvcApp.Interfaces.Services;
using MyMvcApp.Mapping;
using MyMvcApp.Models;

namespace MyMvcApp.Services;

public sealed class AdminService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager) : IAdminService
{
    public async Task<OperationResponseDto> CreateTeacherAsync(CreateTeacherRequestDto request)
    {
        var email = request.Email.Trim();
        if (await userManager.FindByEmailAsync(email) is not null)
            throw new ServiceException((int)HttpStatusCode.Conflict, "Email is already registered.");
        if (!await roleManager.RoleExistsAsync("Teacher"))
            throw new ServiceException((int)HttpStatusCode.InternalServerError, "Teacher role is not configured.");

        var teacher = new ApplicationUser
        {
            UserName = email,
            Email = email,
            FullName = request.FullName.Trim(),
            Department = request.Department.Trim(),
            EmailConfirmed = true,
            IsActive = true
        };

        var createResult = await userManager.CreateAsync(teacher, request.Password);
        if (!createResult.Succeeded)
            throw new ServiceException((int)HttpStatusCode.BadRequest, "Failed to create teacher.", createResult.Errors.Select(error => error.Description).ToArray());

        var roleResult = await userManager.AddToRoleAsync(teacher, "Teacher");
        if (!roleResult.Succeeded)
        {
            await userManager.DeleteAsync(teacher);
            throw new ServiceException((int)HttpStatusCode.InternalServerError, "Could not assign the Teacher role.", roleResult.Errors.Select(error => error.Description).ToArray());
        }

        return new OperationResponseDto("Teacher created successfully.", teacher.Id);
    }

    public async Task<IReadOnlyList<ManagedUserResponseDto>> GetTeachersAsync() =>
        (await userManager.GetUsersInRoleAsync("Teacher")).Select(user => user.ToManagedUserDto()).ToArray();

    public async Task<OperationResponseDto> DeleteTeacherAsync(string id)
    {
        var teacher = await userManager.FindByIdAsync(id)
            ?? throw new ServiceException((int)HttpStatusCode.NotFound, "Teacher not found.");
        if (!await userManager.IsInRoleAsync(teacher, "Teacher"))
            throw new ServiceException((int)HttpStatusCode.BadRequest, "User is not a teacher.");

        var result = await userManager.DeleteAsync(teacher);
        if (!result.Succeeded)
            throw new ServiceException((int)HttpStatusCode.BadRequest, "Could not delete teacher.", result.Errors.Select(error => error.Description).ToArray());
        return new OperationResponseDto("Teacher deleted.");
    }

    public async Task<IReadOnlyList<ManagedUserResponseDto>> GetStudentsAsync() =>
        (await userManager.GetUsersInRoleAsync("Student")).Select(user => user.ToManagedUserDto()).ToArray();

    public async Task<OperationResponseDto> SetStudentStatusAsync(string id, SetUserStatusRequestDto request)
    {
        var student = await userManager.FindByIdAsync(id)
            ?? throw new ServiceException((int)HttpStatusCode.NotFound, "User not found.");
        if (!await userManager.IsInRoleAsync(student, "Student"))
            throw new ServiceException((int)HttpStatusCode.BadRequest, "This action is only available for student accounts.");

        student.IsActive = request.IsActive
            ?? throw new ServiceException((int)HttpStatusCode.BadRequest, "Account status is required.");
        var result = await userManager.UpdateAsync(student);
        if (!result.Succeeded)
            throw new ServiceException((int)HttpStatusCode.BadRequest, "Could not update account status.", result.Errors.Select(error => error.Description).ToArray());
        return new OperationResponseDto("Status updated.");
    }
}
