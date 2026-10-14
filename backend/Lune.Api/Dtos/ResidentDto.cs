using Lune.Api.Models;

namespace Lune.Api.Dtos;

public record ResidentDto(
    Guid Id,
    string FirstName,
    string LastName,
    DateOnly DateOfBirth,
    int Age,
    string Gender,
    string DementiaType,
    string Department)
{
    public static ResidentDto From(Resident r, DateOnly today) => new(
        r.Id,
        r.FirstName,
        r.LastName,
        r.DateOfBirth,
        CalculateAge(r.DateOfBirth, today),
        r.Gender.ToString(),
        r.DementiaType.ToString(),
        r.Department);

    private static int CalculateAge(DateOnly dateOfBirth, DateOnly today)
    {
        var age = today.Year - dateOfBirth.Year;
        if (dateOfBirth > today.AddYears(-age)) age--;
        return age;
    }
}