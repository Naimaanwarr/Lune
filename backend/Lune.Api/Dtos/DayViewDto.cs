namespace Lune.Api.Dtos;

public record HourDto(int Hour, List<ObservationDto> Observations);

public record DayViewDto(Guid ResidentId, DateOnly Date, List<HourDto> Hours);