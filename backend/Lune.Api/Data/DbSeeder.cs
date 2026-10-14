using Lune.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Lune.Api.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext db)
    {
        if (await db.Residents.AnyAsync()) return;

        var behaviors = new List<BehaviorType>
        {
            new() { Name = "Rolig", ColorHex = "#43A047", SortOrder = 1 },
            new() { Name = "Uro", ColorHex = "#FB8C00", SortOrder = 2 },
            new() { Name = "Aggresjon", ColorHex = "#E53935", SortOrder = 3 },
            new() { Name = "Vandring", ColorHex = "#8E24AA", SortOrder = 4 },
            new() { Name = "Søvn", ColorHex = "#1E88E5", SortOrder = 5 }
        };

        var measures = new List<Measure>
        {
            new() { Name = "Skjerming", Icon = "door-closed" },
            new() { Name = "Musikk", Icon = "music" },
            new() { Name = "Tur", Icon = "footprints" },
            new() { Name = "Samtale", Icon = "message-circle" },
            new() { Name = "Aktivitet", Icon = "puzzle" }
        };

        const string department = "Skjermet avdeling";
        var residents = new List<Resident>
        {
            new() { FirstName = "Ingrid", LastName = "Solberg", DateOfBirth = new DateOnly(1949, 3, 14), Gender = Gender.Kvinne, DementiaType = DementiaType.Alzheimers, Department = department },
            new() { FirstName = "Arne", LastName = "Bakken", DateOfBirth = new DateOnly(1955, 11, 2), Gender = Gender.Mann, DementiaType = DementiaType.Vaskulaer, Department = department },
            new() { FirstName = "Solveig", LastName = "Haugen", DateOfBirth = new DateOnly(1941, 7, 21), Gender = Gender.Kvinne, DementiaType = DementiaType.Uspesifisert, Department = department },
            new() { FirstName = "Kjell", LastName = "Nygård", DateOfBirth = new DateOnly(1966, 1, 9), Gender = Gender.Mann, DementiaType = DementiaType.Frontotemporal, Department = department },
            new() { FirstName = "Randi", LastName = "Lunde", DateOfBirth = new DateOnly(1952, 9, 30), Gender = Gender.Kvinne, DementiaType = DementiaType.LewyBody, Department = department },
            new() { FirstName = "Odd", LastName = "Strand", DateOfBirth = new DateOnly(1945, 5, 17), Gender = Gender.Mann, DementiaType = DementiaType.Alzheimers, Department = department },
            new() { FirstName = "Margit", LastName = "Eide", DateOfBirth = new DateOnly(1958, 12, 5), Gender = Gender.Kvinne, DementiaType = DementiaType.Blandet, Department = department },
            new() { FirstName = "Torbjørn", LastName = "Moen", DateOfBirth = new DateOnly(1960, 4, 26), Gender = Gender.Mann, DementiaType = DementiaType.ParkinsonsMedDemens, Department = department }
        };

        db.BehaviorTypes.AddRange(behaviors);
        db.Measures.AddRange(measures);
        db.Residents.AddRange(residents);
        await db.SaveChangesAsync();

        string?[] comments =
        {
            "Roet seg etter samtale",
            "Lette etter familien",
            "Ville gå hjem",
            "Urolig ved vaktskifte",
            "Sov godt",
            "Hyggelig stund i fellesstua"
        };

        var random = new Random(42);
        var today = DateTime.UtcNow.Date;
        var observations = new List<Observation>();

        foreach (var resident in residents)
        {
            for (var day = 13; day >= 0; day--)
            {
                var date = today.AddDays(-day);
                var count = random.Next(4, 9);

                for (var i = 0; i < count; i++)
                {
                    var hour = random.Next(0, 24);
                    var behavior = PickBehavior(hour, behaviors, random);

                    Measure? measure = null;
                    var isRestless = behavior.Name is "Uro" or "Aggresjon" or "Vandring";
                    if (isRestless && random.NextDouble() < 0.6)
                        measure = measures[random.Next(measures.Count)];

                    observations.Add(new Observation
                    {
                        Resident = resident,
                        BehaviorType = behavior,
                        Measure = measure,
                        ObservedAt = DateTime.SpecifyKind(
                            date.AddHours(hour).AddMinutes(random.Next(0, 60)),
                            DateTimeKind.Utc),
                        Comment = random.NextDouble() < 0.3
                            ? comments[random.Next(comments.Length)]
                            : null
                    });
                }
            }
        }

        db.Observations.AddRange(observations);
        await db.SaveChangesAsync();
    }

    // Rekkefølge i listen: 0 Rolig, 1 Uro, 2 Aggresjon, 3 Vandring, 4 Søvn
    private static BehaviorType PickBehavior(int hour, List<BehaviorType> b, Random random)
    {
        var roll = random.NextDouble();

        // Natt: mest søvn
        if (hour >= 22 || hour < 6)
            return roll < 0.7 ? b[4] : roll < 0.85 ? b[3] : roll < 0.95 ? b[1] : b[0];

        // Ettermiddag: mer uro (solnedgangssyndrom)
        if (hour >= 15 && hour < 19)
            return roll < 0.35 ? b[1] : roll < 0.5 ? b[2] : roll < 0.7 ? b[3] : b[0];

        // Resten av dagen: mest rolig
        return roll < 0.6 ? b[0] : roll < 0.8 ? b[1] : roll < 0.9 ? b[3] : b[2];
    }
}