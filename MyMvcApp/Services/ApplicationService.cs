using System.Net;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyMvcApp.DTOs.Requests;
using MyMvcApp.DTOs.Responses;
using MyMvcApp.Exceptions;
using MyMvcApp.Interfaces.Services;
using MyMvcApp.Mapping;
using MyMvcApp.Models;

namespace MyMvcApp.Services;

public sealed class ApplicationService(ApplicationDbContext context, UserManager<ApplicationUser> userManager) : IApplicationService
{
    public async Task<ApplicationResponseDto> SubmitAsync(SubmitApplicationRequestDto request, string studentId)
    {
        var student = await GetUserAsync(studentId);
        var roles = await userManager.GetRolesAsync(student);
        if (!roles.Contains("Student"))
            throw new ServiceException((int)HttpStatusCode.Forbidden, "Only students can apply.");

        var eventItem = await context.Events.Include(item => item.TargetDepartments)
            .FirstOrDefaultAsync(item => item.Id == request.EventId)
            ?? throw new ServiceException((int)HttpStatusCode.NotFound, "Event not found.");

        if (!eventItem.TargetDepartments.Any(target => target.Department == student.Department))
            throw new ServiceException((int)HttpStatusCode.Forbidden, "You are not eligible for this event.");

        var alreadyApplied = await context.EventApplications.AnyAsync(application =>
            application.EventId == request.EventId && application.ApplicantUserId == student.Id);
        if (alreadyApplied)
            throw new ServiceException((int)HttpStatusCode.Conflict, "You have already applied to this event.");

        var application = new EventApplication
        {
            EventId = eventItem.Id,
            ApplicantUserId = student.Id,
            ApplicantName = student.FullName ?? student.Email ?? string.Empty,
            Email = request.Email.Trim(),
            PhoneNumber = request.PhoneNumber.Trim(),
            Department = student.Department ?? string.Empty,
            Status = ApplicationStatus.Pending,
            SubmittedAt = DateTime.UtcNow
        };
        context.EventApplications.Add(application);
        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            context.Entry(application).State = EntityState.Detached;
            if (await context.EventApplications.AnyAsync(existing =>
                    existing.EventId == request.EventId && existing.ApplicantUserId == student.Id))
                throw new ServiceException((int)HttpStatusCode.Conflict, "You have already applied to this event.");
            throw;
        }

        application.Event = eventItem;
        return application.ToDto();
    }

    public async Task<IReadOnlyList<ApplicationResponseDto>> GetForStudentAsync(string studentId)
    {
        var applications = await context.EventApplications.AsNoTracking()
            .Include(item => item.Event).ThenInclude(item => item.TargetDepartments)
            .Where(item => item.ApplicantUserId == studentId)
            .OrderByDescending(item => item.SubmittedAt)
            .ToListAsync();
        return applications.Select(item => item.ToDto()).ToArray();
    }

    public async Task<IReadOnlyList<ApplicationResponseDto>> GetForReviewerAsync(string reviewerId)
    {
        var reviewer = await GetUserAsync(reviewerId);
        var roles = await userManager.GetRolesAsync(reviewer);
        var query = context.EventApplications.AsNoTracking()
            .Include(item => item.Event).ThenInclude(item => item.TargetDepartments)
            .Include(item => item.Applicant)
            .AsQueryable();

        if (roles.Contains("Admin"))
        {
            // Admins can review every application.
        }
        else if (roles.Contains("Teacher"))
        {
            var department = reviewer.Department;
            query = query.Where(item =>
                (!string.IsNullOrWhiteSpace(department) && item.Event.TargetDepartments.Any(target => target.Department == department)) ||
                item.Event.CreatedByUserId == reviewer.Id);
        }
        else
        {
            return [];
        }

        var applications = await query.OrderByDescending(item => item.SubmittedAt).ToListAsync();
        return applications.Select(item => item.ToDto()).ToArray();
    }

    public async Task<ApplicationResponseDto> ReviewAsync(int id, ReviewApplicationRequestDto request, string reviewerId)
    {
        if (!Enum.IsDefined(request.Status) || request.Status == ApplicationStatus.Pending)
            throw new ServiceException((int)HttpStatusCode.BadRequest, "Choose Approved or Rejected as the review status.");

        var reviewer = await GetUserAsync(reviewerId);
        var roles = await userManager.GetRolesAsync(reviewer);
        if (!roles.Contains("Admin") && !roles.Contains("Teacher"))
            throw new ServiceException((int)HttpStatusCode.Forbidden, "You are not allowed to review applications.");

        var application = await context.EventApplications
            .Include(item => item.Event).ThenInclude(item => item.TargetDepartments)
            .Include(item => item.Applicant)
            .FirstOrDefaultAsync(item => item.Id == id)
            ?? throw new ServiceException((int)HttpStatusCode.NotFound, "Application not found.");

        if (roles.Contains("Teacher") && !roles.Contains("Admin"))
        {
            var departmentMatch = application.Event.TargetDepartments.Any(target => target.Department == reviewer.Department);
            if (!departmentMatch && application.Event.CreatedByUserId != reviewer.Id)
                throw new ServiceException((int)HttpStatusCode.Forbidden, "You are not allowed to review this application.");
        }

        application.Status = request.Status;
        application.ReviewerNotes = request.Notes?.Trim();
        application.ReviewedByUserId = reviewer.Id;
        application.ReviewedAt = DateTime.UtcNow;
        await context.SaveChangesAsync();
        return application.ToDto();
    }

    private async Task<ApplicationUser> GetUserAsync(string userId) =>
        await userManager.FindByIdAsync(userId)
        ?? throw new ServiceException((int)HttpStatusCode.Unauthorized, "The authenticated account no longer exists.");
}
