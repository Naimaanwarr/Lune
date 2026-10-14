using Lune.Api.Dtos;
using Lune.Api.Models;

namespace Lune.Api.Services;

public static class DayViewBuilder
{
    // Finner start og slutt på et norsk døgn, omregnet til UTC
    public static (DateTime FromUtc, DateTime ToUtc) GetUtcRange(DateOnly date, TimeZoneInfo timeZone)
    {
        var localStart = date.ToDateTime(TimeOnly.MinValue);
        var fromUtc = TimeZoneInfo.ConvertTimeToUtc(localStart, timeZone);
        var toUtc = TimeZoneInfo.ConvertTimeToUtc(localStart.AddDays(1), timeZone);
        return (fromUtc, toUtc);
    }

    // Grupperer observasjonene i 24 timer etter lokal tid
    public static DayViewDto Build(
        Guid residentId,
        DateOnly date,
        IEnumerable<Observation> observations,
        TimeZoneInfo timeZone)
    {
        var byHour = observations
            .GroupBy(o => TimeZoneInfo.ConvertTimeFromUtc(o.ObservedAt, timeZone).Hour)
            .ToDictionary(
                g => g.Key,
                g => g.OrderBy(o => o.ObservedAt).Select(ObservationDto.From).ToList());

        var hours = Enumerable.Range(0, 24)
            .Select(hour => new HourDto(hour, byHour.GetValueOrDefault(hour, new List<ObservationDto>())))
            .ToList();

        return new DayViewDto(residentId, date, hours);
    }
}