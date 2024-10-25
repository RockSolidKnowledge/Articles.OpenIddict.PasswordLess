using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Rsk.AspNetCore.Fido;
using Rsk.AspNetCore.Fido.Dtos;
using Rsk.AspNetCore.Fido.Models;
using Velusia.Server.Data;
using Velusia.Server.ViewModels.PasswordlessLogin;

namespace Velusia.Server.Controllers;
[AllowAnonymous]
public class PasswordlessLoginController : Controller
{
    private IFidoAuthentication _fido;
    private UserManager<ApplicationUser> _userManager;
    private SignInManager<ApplicationUser> _signInManager;

    public PasswordlessLoginController(IFidoAuthentication fido, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
    {
        _fido = fido;
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public ActionResult StartRegistration(string? returnUrl =null)
    {
        StartRegistration model = new()
        {
            ReturnUrl = returnUrl
        };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Register(StartRegistration model)
    {
            if (ModelState.IsValid)
        {
            if (await _userManager.FindByEmailAsync(model.Username) != null)
            {
                return BadRequest("A user with that email address already exists.");
            }

            //Initiate the Fido registration process.
            FidoRegistrationChallenge challenge = await _fido.InitiateRegistration(model.Username, model.DeviceName);

            // challenge the device
            var register = new Register { Challenge = challenge.ToBase64Dto(), ReturnUrl =model.ReturnUrl};
            return View(register);
        }

        return View("StartRegistration", model);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CompleteRegistration(
        [FromBody] Base64FidoRegistrationResponse registrationResponse)
    {
        var result = await _fido.CompleteRegistration(registrationResponse.ToFidoResponse());

        if (result.IsError)
        {
            return BadRequest(result.ErrorDescription);
        }

        ApplicationUser user = new()
        {
            UserName = result.UserId
        };
        IdentityResult userCreationResult = await _userManager.CreateAsync(user);
        if (userCreationResult.Succeeded)
        {
            return Ok();
        }

        return BadRequest(String.Join(',', userCreationResult.Errors.Select(e => e.Description)));
    }
    public async Task<ActionResult> StartLogin(string? returnUrl =null)
    {
        StartLogin model = new()
        {
            ReturnUrl = returnUrl
        };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> StartLogin(StartLogin model)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByEmailAsync(model.Username);
            if (user == null)
            {
                return BadRequest("A user with that email address does not exist.");
            }

            FidoAuthenticationChallenge challenge = await _fido.InitiateAuthentication(model.Username);
            Login loginModel = new()
            {
                Challenge = challenge.ToBase64Dto(),
                RelyingPartyId = challenge.RelyingPartyId,
                ReturnUrl = model.ReturnUrl
            };
            return View("Login", loginModel);
        }

        return View("StartLogin", model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> CompleteLogin([FromBody] Base64FidoAuthenticationResponse authenticationResponse)
    {
        var result = await _fido.CompleteAuthentication(authenticationResponse.ToFidoResponse());

        if (result.IsError) return BadRequest(result.ErrorDescription);

        var user = await _userManager.FindByEmailAsync(result.UserId);

        if (user is null)
        {
            return BadRequest("No user exists with that email address.");
        }

        _signInManager.SignInAsync(user, false);


        return new EmptyResult();
    }
}