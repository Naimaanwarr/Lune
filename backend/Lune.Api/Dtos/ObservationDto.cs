using Lune.Api.Models;

namespace Lune.Api.Dtos;

public record ObservationDto(
    Guid Id,
    DateTime ObservedAt,
    int BehaviorTypeId,
    string BehaviorName,
    string ColorHex,
    int? MeasureId,
    string? MeasureName,
    string? MeasureIcon,
    string? Comment)
{
    public static ObservationDto From(Observation o) => new(
        o.Id,
        o.ObservedAt,
        o.BehaviorTypeId,
        o.BehaviorType.Name,
        o.BehaviorType.ColorHex,
        o.MeasureId,
        o.Measure?.Name,
        o.Measure?.Icon,
        o.Comment);
}