using COMSATS.StudentPortal.Web.Models;

namespace COMSATS.StudentPortal.Web.ViewModels;

public class ProfileViewModel
{
    public required ApplicationUser Student { get; init; }
    public int BatchPosition { get; init; }
    public int BatchSize { get; init; }
    public int CompletedCredits { get; init; }
    public double DegreeCompletionPercentage { get; init; }
    public List<SemesterRow> SemesterHistory { get; init; } = new();

    public record SemesterRow(string SemesterLabel, int CreditsEarned, decimal? Sgpa, decimal Cgpa, string StatusLabel, bool IsInProgress);
}
