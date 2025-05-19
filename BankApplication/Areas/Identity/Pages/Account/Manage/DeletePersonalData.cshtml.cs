using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
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

        public IList<IdentityUser> Users { get; set; } = new List<IdentityUser>();

        public string CurrentUserId { get; set; } = string.Empty;

        public async Task<IActionResult> OnGetAsync()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                await _signInManager.SignOutAsync();
                TempData["StatusMessage"] = "Your account no longer exists. You have been signed out.";
                return RedirectToPage("/Index");
            }

            CurrentUserId = currentUser.Id;
            Users = _userManager.Users.ToList();

            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return BadRequest("User ID is required.");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                TempData["StatusMessage"] = $"User with ID '{userId}' was already deleted or not found.";
                return RedirectToPage();
            }

            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
                await _userManager.RemoveFromRoleAsync(user, role);

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                TempData["StatusMessage"] = "Error deleting user.";
                return RedirectToPage();
            }

            _logger.LogInformation("Admin deleted user: {UserId}", userId);
            TempData["StatusMessage"] = $"User {user.Email} deleted successfully.";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteMyAccountAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                await _signInManager.SignOutAsync();
                TempData["StatusMessage"] = "Your account no longer exists.";
                return RedirectToPage("/Index");
            }

            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
                await _userManager.RemoveFromRoleAsync(user, role);

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
                throw new InvalidOperationException("Unexpected error occurred deleting your account.");

            await _signInManager.SignOutAsync();

            _logger.LogInformation("User with ID '{UserId}' deleted their own account.", user.Id);
            TempData["StatusMessage"] = "Your account has been deleted.";

            return RedirectToPage("/Index");
        }
    }
}
