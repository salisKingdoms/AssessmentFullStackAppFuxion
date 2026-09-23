namespace CVSalis.Data.Dto
{
    public static class ExperienceYears
    {
        // Year-only periods: exclude gaps and count overlapping employment only once.
        public static int Calculate(IEnumerable<GetDataExperience>? experiences)
        {
            var total = 0;
            var coveredUntil = 0;
            foreach (var experience in (experiences ?? Enumerable.Empty<GetDataExperience>())
                .Where(e => e.periode_start > 0 && e.periode_end > e.periode_start)
                .OrderBy(e => e.periode_start))
            {
                total += Math.Max(0, experience.periode_end - Math.Max(coveredUntil, experience.periode_start));
                coveredUntil = Math.Max(coveredUntil, experience.periode_end);
            }
            return total;
        }
    }
}
