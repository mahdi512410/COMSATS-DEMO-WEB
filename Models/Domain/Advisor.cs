using System.ComponentModel.DataAnnotations;

namespace COMSATS.StudentPortal.Web.Models.Domain;

public class Advisor
{
    public int Id { get; set; }

    [Required, MaxLength(120)]
    public string FullName { get; set; } = string.Empty;

    [MaxLength(120)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(120)]
    public string Department { get; set; } = string.Empty;

    [MaxLength(120)]
    public string OfficeLocation { get; set; } = string.Empty;

    [MaxLength(160)]
    public string OfficeHours { get; set; } = string.Empty;

    [MaxLength(160)]
    public string Email { get; set; } = string.Empty;

    public ICollection<ApplicationUser> Advisees { get; set; } = new List<ApplicationUser>();
}
