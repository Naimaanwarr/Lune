using System.ComponentModel.DataAnnotations;

namespace Lune.Api.Dtos;

public record SaveObservationRequest(
    [Required(ErrorMessage = "Velg en beboer.")] Guid ResidentId,
    [Range(1, int.MaxValue, ErrorMessage = "Velg en atferdstype.")] int BehaviorTypeId,
    int? MeasureId,
    [Required(ErrorMessage = "Tidspunkt må fylles ut.")] DateTime ObservedAt,
    [MaxLength(500, ErrorMessage = "Kommentaren kan være maks 500 tegn.")] string? Comment);