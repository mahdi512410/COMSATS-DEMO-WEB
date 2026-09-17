using System.ComponentModel.DataAnnotations;

namespace COMSATS.StudentPortal.Web.ViewModels;

public class LoginViewModel
{
    [Required(ErrorMessage = "Select your registered campus.")]
    [Display(Name = "Campus Affiliation")]
    public string CampusName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Enter your Registration ID or campus email.")]
    [Display(Name = "Registration ID or Campus Email")]
    public string RegistrationIdOrEmail { get; set; } = string.Empty;

    [Required(ErrorMessage = "Enter your password.")]
    [DataType(DataType.Password)]
    [Display(Name = "Secret Password / CU-PIN")]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "Remember this terminal for 14 days")]
    public bool RememberMe { get; set; }

    public string? ReturnUrl { get; set; }
}
