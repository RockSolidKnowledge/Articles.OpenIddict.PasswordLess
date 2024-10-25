using Rsk.AspNetCore.Fido.Dtos;

namespace Velusia.Server.ViewModels.PasswordlessLogin;

public class Register
{
    public Base64FidoRegistrationChallenge Challenge { get; set; }
    public string? ReturnUrl { get; set;  }
}