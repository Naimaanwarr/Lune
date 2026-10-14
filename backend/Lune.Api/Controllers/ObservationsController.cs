using Lune.Api.Data;
using Lune.Api.Dtos;
using Lune.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lune.Api.Controllers;

[ApiController]
[Route("api/observations")]
public class ObservationsController(AppDbContext db) : ControllerBase
{
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ObservationDto>> GetById(Guid id)
    {
        var observation = await db.Observations
            .Include(o => o.BehaviorType)
            .Include(o => o.Measure)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (observation is null) return NotFound();
        return ObservationDto.From(observation);
    }

    [HttpPost]
    public async Task<ActionResult<ObservationDto>> Create(SaveObservationRequest request)
    {
        var error = await ValidateAsync(request);
        if (error is not null) return error;

        var observation = new Observation();
        Apply(observation, request);

        db.Observations.Add(observation);
        await db.SaveChangesAsync();
        await LoadReferencesAsync(observation);

        return CreatedAtAction(nameof(GetById), new { id = observation.Id }, ObservationDto.From(observation));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ObservationDto>> Update(Guid id, SaveObservationRequest request)
    {
        var observation = await db.Observations.FindAsync(id);
        if (observation is null) return NotFound();

        var error = await ValidateAsync(request);
        if (error is not null) return error;

        Apply(observation, request);
        await db.SaveChangesAsync();
        await LoadReferencesAsync(observation);

        return ObservationDto.From(observation);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var observation = await db.Observations.FindAsync(id);
        if (observation is null) return NotFound();

        db.Observations.Remove(observation);
        await db.SaveChangesAsync();
        return NoContent();
    }

    private static void Apply(Observation observation, SaveObservationRequest request)
    {
        observation.ResidentId = request.ResidentId;
        observation.BehaviorTypeId = request.BehaviorTypeId;
        observation.MeasureId = request.MeasureId;
        observation.ObservedAt = request.ObservedAt.ToUniversalTime();
        observation.Comment = string.IsNullOrWhiteSpace(request.Comment) ? null : request.Comment.Trim();
    }

    private async Task LoadReferencesAsync(Observation observation)
    {
        await db.Entry(observation).Reference(o => o.BehaviorType).LoadAsync();
        await db.Entry(observation).Reference(o => o.Measure).LoadAsync();
    }

    private async Task<ActionResult?> ValidateAsync(SaveObservationRequest request)
    {
        if (request.ObservedAt.ToUniversalTime() > DateTime.UtcNow.AddMinutes(5))
            ModelState.AddModelError(nameof(request.ObservedAt), "Tidspunktet kan ikke være i fremtiden.");

        if (!await db.Residents.AnyAsync(r => r.Id == request.ResidentId && !r.IsArchived))
            ModelState.AddModelError(nameof(request.ResidentId), "Beboeren finnes ikke eller er arkivert.");

        if (!await db.BehaviorTypes.AnyAsync(b => b.Id == request.BehaviorTypeId))
            ModelState.AddModelError(nameof(request.BehaviorTypeId), "Atferdstypen finnes ikke.");

        if (request.MeasureId is not null && !await db.Measures.AnyAsync(m => m.Id == request.MeasureId))
            ModelState.AddModelError(nameof(request.MeasureId), "Tiltaket finnes ikke.");

        return ModelState.IsValid ? null : ValidationProblem(ModelState);
    }
}