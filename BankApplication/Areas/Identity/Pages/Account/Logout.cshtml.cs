using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;

[ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
[AllowAnonymous]
public class LogoutModel : PageModel
{
    private readonly SignInManager<IdentityUser> _signInManager;

    public LogoutModel(SignInManager<IdentityUser> signInManager)
    {
        _signInManager = signInManager;
    }

    public async Task<IActionResult> OnPost()
    {
        await _signInManager.SignOutAsync();
        return LocalRedirect("~/");
    }
}