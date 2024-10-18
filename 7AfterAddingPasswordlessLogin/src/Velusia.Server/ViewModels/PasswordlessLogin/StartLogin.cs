using System.ComponentModel.DataAnnotations;

namespace Velusia.Server.ViewModels.PasswordlessLogin;

public class StartLogin
{
    [Required]
    [EmailAddress]
    [Display(Name = "Username")]
    public string Username { get; set; }

    public string ReturnUrl { get; set; }
}