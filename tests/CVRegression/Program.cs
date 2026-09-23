using CVSalis.Data.Dto;

void Check(string name, int expected, params (int start, int end)[] periods)
{
    var actual = ExperienceYears.Calculate(periods.Select(p => new GetDataExperience
    {
        periode_start = p.start, periode_end = p.end
    }));
    if (actual != expected) throw new Exception($"{name}: expected {expected}, got {actual}");
    Console.WriteLine($"PASS: {name}");
}

Check("No experience", 0);
Check("Single job", 3, (2020, 2023));
Check("Unsorted consecutive jobs", 6, (2022, 2024), (2018, 2022));
Check("Gaps excluded", 4, (2015, 2017), (2020, 2022));
Check("Overlaps counted once", 6, (2018, 2022), (2020, 2024));
Check("Nested jobs counted once", 6, (2018, 2024), (2019, 2021));
Check("Invalid periods ignored", 0, (2024, 2020), (0, 2024), (2020, 2020));
