namespace Lune.Api.Models;

public class Observation
{
    public Guid Id { get; set; }

    public Guid ResidentId { get; set; }
    public Resident Resident { get; set; } = null!;

    public int BehaviorTypeId { get; set; }
    public BehaviorType BehaviorType { get; set; } = null!;

    public int? MeasureId { get; set; }
    public Measure? Measure { get; set; }

    public DateTime ObservedAt { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}