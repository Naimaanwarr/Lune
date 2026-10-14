using Lune.Api.Data;
using Lune.Api.Dtos;
using Lune.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lune.Api.Controllers;

[ApiController]
[Route("api/residents")]
public class ResidentsController(AppDbContext db) : ControllerBase
{
    private static readonly TimeZoneInfo Oslo = TimeZoneInfo.FindSystemTimeZoneById("Europe/Oslo");

    [HttpGet]
    public async Task<ActionResult<List<ResidentDto>>> GetAll()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);

        var residents = await db.Residents
            .Where(r => !r.IsArchived)
            .OrderBy(r => r.LastName)
            .ToListAsync();

        return residents.Select(r => ResidentDto.From(r, today)).ToList();
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ResidentDto>> GetById(Guid id)
    {
        var resident = await db.Residents.FindAsync(id);
        if (resident is null) return NotFound();

        return ResidentDto.From(resident, DateOnly.FromDateTime(DateTime.Today));
    }

    [HttpGet("{id:guid}/day")]
    public async Task<ActionResult<DayViewDto>> GetDay(Guid id, [FromQuery] DateOnly? date)
    {
        if (!await db.Residents.AnyAsync(r => r.Id == id)) return NotFound();

        var day = date ?? DateOnly.FromDateTime(TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Oslo));
        var (fromUtc, toUtc) = DayViewBuilder.GetUtcRange(day, Oslo);

        var observations = await db.Observations
            .Include(o => o.BehaviorType)
            .Include(o => o.Measure)
            .Where(o => o.ResidentId == id && o.ObservedAt >= fromUtc && o.ObservedAt < toUtc)
            .ToListAsync();

        return DayViewBuilder.Build(id, day, observations, Oslo);
    }
}