using MyMvcApp.DTOs.Responses;
using MyMvcApp.Models;

namespace MyMvcApp.Mapping;

public static class DtoMapper
{
    public static EventResponseDto ToDto(this Event entity) => new(
        entity.Id,
        entity.Title,
        entity.Description,
        entity.Venue,
        entity.EventDate,
        entity.CreatedAt,
        entity.CreatedByUserId,
        entity.TargetDepartments.Select(item => item.Department).ToArray());

    public static ApplicationResponseDto ToDto(this EventApplication entity) => new(
        entity.Id,
        entity.EventId,
        entity.Event?.Title,
        entity.Event?.TargetDepartments.Select(item => item.Department).ToArray(),
        entity.ApplicantUserId,
        entity.ApplicantName,
        entity.Applicant?.Email,
        entity.Email,
        entity.PhoneNumber,
        entity.Department,
        entity.Status.ToString(),
        entity.SubmittedAt,
        entity.ReviewedByUserId,
        entity.ReviewedAt,
        entity.ReviewerNotes);

    public static ManagedUserResponseDto ToManagedUserDto(this ApplicationUser user) => new(
        user.Id,
        user.Email,
        user.FullName,
        user.Department,
        user.IsActive);
}
