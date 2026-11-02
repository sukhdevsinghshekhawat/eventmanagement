using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyMvcApp.DTOs.Requests;
using MyMvcApp.DTOs.Responses;
using MyMvcApp.Interfaces.Services;

namespace MyMvcApp.Controllers;

[ApiController]
[Route("api/events")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public sealed class EventsController(IEventService eventService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<EventResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<EventResponseDto>>> GetEvents() =>
        Ok(await eventService.GetVisibleEventsAsync(CurrentUserId()));

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(EventResponseDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<EventResponseDto>> GetEvent(int id)
    {
        var result = await eventService.GetEventAsync(id, CurrentUserId());
        return result is null ? NotFound(new { message = "Event not found." }) : Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Teacher")]
    [ProducesResponseType(typeof(EventResponseDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<EventResponseDto>> CreateEvent([FromBody] UpsertEventRequestDto request)
    {
        var result = await eventService.CreateEventAsync(request, CurrentUserId());
        return CreatedAtAction(nameof(GetEvent), new { id = result.Id }, result);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,Teacher")]
    public async Task<ActionResult<EventUpdatedResponseDto>> UpdateEvent(int id, [FromBody] UpsertEventRequestDto request)
    {
        var result = await eventService.UpdateEventAsync(id, request, CurrentUserId());
        return Ok(new EventUpdatedResponseDto("Event updated successfully.", result));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin,Teacher")]
    public async Task<ActionResult<OperationResponseDto>> DeleteEvent(int id)
    {
        await eventService.DeleteEventAsync(id, CurrentUserId());
        return Ok(new OperationResponseDto("Event deleted successfully."));
    }

    private string CurrentUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? throw new UnauthorizedAccessException("User identity is missing from the access token.");
}
