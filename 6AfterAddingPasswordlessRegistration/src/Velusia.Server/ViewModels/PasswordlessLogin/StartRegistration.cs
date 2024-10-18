using System.ComponentModel.DataAnnotations;

namespace Velusia.Server.ViewModels.PasswordlessLogin;

public class StartRegistration
{
    [Required]
    [EmailAddress]
    [Display(Name = "Username")]
    public string Username { get; set; }

    [Required]
    [Display(Name = "Device Name")]
    public string DeviceName { get; set; }
}