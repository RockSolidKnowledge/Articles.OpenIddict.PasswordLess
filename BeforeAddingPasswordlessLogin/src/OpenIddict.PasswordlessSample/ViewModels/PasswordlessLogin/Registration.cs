using System.ComponentModel.DataAnnotations;

namespace OpenIddict.PasswordlessSample.ViewModels.PasswordlessLogin;

public class Registration
{
    [Required]
    [EmailAddress]
    [Display(Name = "Username")]
    public string Username { get; set; }

    [Required]
    [Display(Name = "Device Name")]
    public string DeviceName { get; set; }
}