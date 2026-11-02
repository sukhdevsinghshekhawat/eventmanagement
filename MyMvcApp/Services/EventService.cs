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

public sealed class EventService(ApplicationDbContext context, UserManager<ApplicationUser> userManager) : IEventService
{
    public async Task<IReadOnlyList<EventResponseDto>> GetVisibleEventsAsync(string userId)
    {
        var user = await GetUserAsync(userId);
        var roles = await userManager.GetRolesAsync(user);
        var query = context.Events.AsNoTracking().Include(item => item.TargetDepartments).AsQueryable();

        if (!roles.Contains("Admin"))
        {
            var department = user.Department;
            var isTeacher = roles.Contains("Teacher");
            if (string.IsNullOrWhiteSpace(department) && !isTeacher)
                return [];

            query = query.Where(item =>
                (!string.IsNullOrWhiteSpace(department) && item.TargetDepartments.Any(target => target.Department == department)) ||
                (isTeacher && item.CreatedByUserId == user.Id));
        }

        var events = await query.OrderByDescending(item => item.EventDate).ToListAsync();
        return events.Select(item => item.ToDto()).ToArray();
    }

    public async Task<EventResponseDto?> GetEventAsync(int id, string userId)
    {
        var user = await GetUserAsync(userId);
        var roles = await userManager.GetRolesAsync(user);
        var item = await context.Events.AsNoTracking()
            .Include(eventItem => eventItem.TargetDepartments)
            .FirstOrDefaultAsync(eventItem => eventItem.Id == id);

        if (item is null) return null;
        var isAdmin = roles.Contains("Admin");
        var isTeacherOwner = roles.Contains("Teacher") && item.CreatedByUserId == user.Id;
        var departmentMatch = item.TargetDepartments.Any(target => target.Department == user.Department);
        return isAdmin || isTeacherOwner || departmentMatch ? item.ToDto() : null;
    }

    public async Task<EventResponseDto> CreateEventAsync(UpsertEventRequestDto request, string userId)
    {
        ValidateEventRequest(request);
        var user = await GetUserAsync(userId);
        var departments = NormalizeDepartments(request.Departments);
        var item = new Event
        {
            Title = request.Title.Trim(),
            Description = request.Description.Trim(),
            Venue = request.Venue.Trim(),
            EventDate = request.EventDate,
            CreatedAt = DateTime.UtcNow,
            CreatedByUserId = user.Id,
            TargetDepartments = departments
        };

        context.Events.Add(item);
        await context.SaveChangesAsync();
        return item.ToDto();
    }

    public async Task<EventResponseDto> UpdateEventAsync(int id, UpsertEventRequestDto request, string userId)
    {
        ValidateEventRequest(request);
        var user = await GetUserAsync(userId);
        var item = await context.Events.Include(eventItem => eventItem.TargetDepartments)
            .FirstOrDefaultAsync(eventItem => eventItem.Id == id)
            ?? throw new ServiceException((int)HttpStatusCode.NotFound, "Event not found.");

        var roles = await userManager.GetRolesAsync(user);
        if (!roles.Contains("Admin") && item.CreatedByUserId != user.Id)
            throw new ServiceException((int)HttpStatusCode.Forbidden, "You are not allowed to update this event.");

        item.Title = request.Title.Trim();
        item.Description = request.Description.Trim();
        item.Venue = request.Venue.Trim();
        item.EventDate = request.EventDate;
        await using var transaction = await context.Database.BeginTransactionAsync();
        context.EventDepartments.RemoveRange(item.TargetDepartments);
        await context.SaveChangesAsync();
        item.TargetDepartments.Clear();
        foreach (var department in NormalizeDepartments(request.Departments))
        {
            department.EventId = item.Id;
            item.TargetDepartments.Add(department);
            context.EventDepartments.Add(department);
        }

        await context.SaveChangesAsync();
        await transaction.CommitAsync();
        return item.ToDto();
    }

    public async Task DeleteEventAsync(int id, string userId)
    {
        var user = await GetUserAsync(userId);
        var item = await context.Events.FirstOrDefaultAsync(eventItem => eventItem.Id == id)
            ?? throw new ServiceException((int)HttpStatusCode.NotFound, "Event not found.");
        var roles = await userManager.GetRolesAsync(user);

        if (!roles.Contains("Admin") && item.CreatedByUserId != user.Id)
            throw new ServiceException((int)HttpStatusCode.Forbidden, "You are not allowed to delete this event.");

        context.Events.Remove(item);
        await context.SaveChangesAsync();
    }

    private async Task<ApplicationUser> GetUserAsync(string userId) =>
        await userManager.FindByIdAsync(userId)
        ?? throw new ServiceException((int)HttpStatusCode.Unauthorized, "The authenticated account no longer exists.");

    private static void ValidateEventRequest(UpsertEventRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Title) || string.IsNullOrWhiteSpace(request.Description) ||
            string.IsNullOrWhiteSpace(request.Venue))
            throw new ServiceException((int)HttpStatusCode.BadRequest, "Event title, description, and venue are required.");
        if (request.EventDate == default)
            throw new ServiceException((int)HttpStatusCode.BadRequest, "A valid event date is required.");
        if (NormalizeDepartments(request.Departments).Count == 0)
            throw new ServiceException((int)HttpStatusCode.BadRequest, "At least one department is required.");
    }

    private static List<EventDepartment> NormalizeDepartments(IEnumerable<string> departments) => departments
        .Where(value => !string.IsNullOrWhiteSpace(value))
        .Select(value => value.Trim())
        .Distinct(StringComparer.OrdinalIgnoreCase)
        .Select(value => new EventDepartment { Department = value })
        .ToList();
}
