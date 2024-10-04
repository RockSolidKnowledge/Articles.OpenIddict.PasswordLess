using System.ComponentModel.DataAnnotations;

namespace OpenIddict.PasswordlessSample.ViewModels.PasswordlessLogin;

public class StartLogin
{
    [Required]
    [EmailAddress]
    [Display(Name = "Username")]
    public string Username { get; set; }

    public string ReturnUrl { get; set; }
}