using Lune.Api.Data;
using Lune.Api.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lune.Api.Controllers;

[ApiController]
[Route("api/residents")]
public class ResidentsController(AppDbContext db) : ControllerBase
{
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
}