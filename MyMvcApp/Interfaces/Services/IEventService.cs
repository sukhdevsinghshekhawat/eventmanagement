using MyMvcApp.DTOs.Requests;
using MyMvcApp.DTOs.Responses;

namespace MyMvcApp.Interfaces.Services;

public interface IEventService
{
    Task<IReadOnlyList<EventResponseDto>> GetVisibleEventsAsync(string userId);
    Task<EventResponseDto?> GetEventAsync(int id, string userId);
    Task<EventResponseDto> CreateEventAsync(UpsertEventRequestDto request, string userId);
    Task<EventResponseDto> UpdateEventAsync(int id, UpsertEventRequestDto request, string userId);
    Task DeleteEventAsync(int id, string userId);
}
