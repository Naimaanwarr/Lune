using Lune.Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lune.Api.Controllers;

public record BehaviorTypeDto(int Id, string Name, string ColorHex, int SortOrder);

[ApiController]
[Route("api/behavior-types")]
public class BehaviorTypesController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<List<BehaviorTypeDto>> GetAll() =>
        await db.BehaviorTypes
            .OrderBy(b => b.SortOrder)
            .Select(b => new BehaviorTypeDto(b.Id, b.Name, b.ColorHex, b.SortOrder))
            .ToListAsync();
}