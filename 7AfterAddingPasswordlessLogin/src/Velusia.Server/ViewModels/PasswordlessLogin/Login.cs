using Rsk.AspNetCore.Fido.Dtos;

namespace Velusia.Server.ViewModels.PasswordlessLogin;

public class Login
{
    public string RelyingPartyId { get; set; }
    public Base64FidoAuthenticationChallenge Base64Challenge { get; set; }
    public string ReturnUrl { get; set; }
}