using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WaterTracker.Contracts.WaterIntake;
using WaterTracker.Core.Entities;
using WaterTracker.Core.Interfaces;

namespace WaterTracker.API.Controllers;

[Authorize]
[ApiController]
[Route("api/water-intake")]
public sealed class WaterIntakeController : ControllerBase
{
    private readonly IWaterIntakeService _waterIntakeService;

    public WaterIntakeController(IWaterIntakeService waterIntakeService)
    {
        _waterIntakeService = waterIntakeService;
    }

    private string? CurrentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier);

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<WaterIntakeResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var userId = CurrentUserId;
        if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();

        var entries = await _waterIntakeService.GetForUserAsync(userId, cancellationToken);
        return Ok(entries.Select(ToResponse).ToList());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<WaterIntakeResponse>> GetById(int id, CancellationToken cancellationToken)
    {
        var userId = CurrentUserId;
        if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();

        // Service always filters by both id and userId, so guessing another user's entry ID just returns a 404.
        var entry = await _waterIntakeService.GetForUserByIdAsync(userId, id, cancellationToken);
        return entry is null ? NotFound() : Ok(ToResponse(entry));
    }

    [HttpPost]
    public async Task<ActionResult<WaterIntakeResponse>> Create([FromBody] LogWaterIntakeRequest request, CancellationToken cancellationToken)
    {
        var userId = CurrentUserId;
        if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();

        if (request.ConsumedAtUtc == default)
            return BadRequest(new { Message = "Consumed date and time is required." });

        var entry = await _waterIntakeService.CreateForUserAsync(
            userId, request.ConsumedAtUtc, request.AmountMl, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = entry.Id }, ToResponse(entry));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<WaterIntakeResponse>> Update(int id, [FromBody] UpdateWaterIntakeRequest request, CancellationToken cancellationToken)
    {
        var userId = CurrentUserId;
        if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();

        if (request.ConsumedAtUtc == default)
            return BadRequest(new { Message = "Consumed date and time is required." });

        var entry = await _waterIntakeService.UpdateForUserAsync(
            userId, id, request.ConsumedAtUtc, request.AmountMl, cancellationToken);

        return entry is null ? NotFound() : Ok(ToResponse(entry));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var userId = CurrentUserId;
        if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();

        var deleted = await _waterIntakeService.DeleteForUserAsync(userId, id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }

    private static WaterIntakeResponse ToResponse(WaterIntakeEntry entry) => new(
        entry.Id,
        entry.AmountMl,
        entry.ConsumedAtUtc,
        entry.CreatedAtUtc,
        entry.LastUpdatedAtUtc);
}
