using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BankApplication.Areas.Identity.Pages.Account.Manage
{
    [Authorize(Roles = "Admin")]
    public class DeletePersonalDataModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<DeletePersonalDataModel> _logger;

        public DeletePersonalDataModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ILogger<DeletePersonalDataModel> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            public string Password { get; set; }
        }

        public bool RequirePassword { get; set; }

        // Ny egenskap för att lagra användarna
        public IList<IdentityUser> Users { get; set; }

        // Egenskap för att hålla ID för den inloggade användaren
        public string CurrentUserId { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            // Hämta alla användare
            Users = _userManager.Users.ToList();

            // Hämta den aktuella inloggade användaren
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            // Sätt CurrentUserId för att markera den inloggade användaren
            CurrentUserId = user.Id;

            RequirePassword = await _userManager.HasPasswordAsync(user);
            return Page();
        }

        // Hantera "Delete my Account" funktionalitet
        public async Task<IActionResult> OnPostDeleteMyAccountAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            // Ta bort användarens roller
            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                await _userManager.RemoveFromRoleAsync(user, role);
            }

            // Ta bort användaren
            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException($"Unexpected error occurred deleting user.");
            }

            // Logga ut användaren
            await _signInManager.SignOutAsync();

            _logger.LogInformation("User with ID '{UserId}' deleted their account.", user.Id);

            return RedirectToPage("/Index"); // Omdirigera till startsidan eller annan sida
        }


        public async Task<IActionResult> OnPostDeleteAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{userId}'.");
            }

            // Ta bort användarens roller
            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                await _userManager.RemoveFromRoleAsync(user, role);
            }

            // Ta bort användaren
            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException($"Unexpected error occurred deleting user.");
            }

            // Logga ut användaren om den raderade användaren är samma som den inloggade användaren
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return NotFound("Unable to retrieve the current logged-in user.");
            }

            if (userId == await _userManager.GetUserIdAsync(currentUser))
            {
                await _signInManager.SignOutAsync();
            }

            _logger.LogInformation("User with ID '{UserId}' deleted themselves.", userId);
            return RedirectToPage("./DeletePersonalData");
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            RequirePassword = await _userManager.HasPasswordAsync(user);
            if (RequirePassword)
            {
                if (!await _userManager.CheckPasswordAsync(user, Input.Password))
                {
                    ModelState.AddModelError(string.Empty, "Incorrect password.");
                    return Page();
                }
            }

            var result = await _userManager.DeleteAsync(user);
            var userId = await _userManager.GetUserIdAsync(user);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException($"Unexpected error occurred deleting user.");
            }

            // Logga ut användaren om den raderade användaren är samma som den inloggade användaren
            await _signInManager.SignOutAsync();

            _logger.LogInformation("User with ID '{UserId}' deleted themselves.", userId);

            return Redirect("~/");
        }
    }
}
