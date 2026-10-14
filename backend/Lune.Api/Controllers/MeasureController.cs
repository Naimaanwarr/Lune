using Lune.Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lune.Api.Controllers;

public record MeasureDto(int Id, string Name, string Icon);

[ApiController]
[Route("api/measures")]
public class MeasuresController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<List<MeasureDto>> GetAll() =>
        await db.Measures
            .OrderBy(m => m.Name)
            .Select(m => new MeasureDto(m.Id, m.Name, m.Icon))
            .ToListAsync();
}