using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace BankApplication.Areas.Identity.Pages.Account.Manage
{
    [Authorize(Roles = "Admin")]
    public class ChangePasswordModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<ChangePasswordModel> _logger;

        public ChangePasswordModel(
            UserManager<IdentityUser> userManager,
            ILogger<ChangePasswordModel> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        [TempData]
        public string StatusMessage { get; set; }

        public SelectList Users { get; set; }

        public class InputModel
        {
            [Required]
            [Display(Name = "Select a user")]
            public string UserId { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "New password")]
            public string NewPassword { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm new password")]
            [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var users = await _userManager.Users
                .Select(u => new { u.Id, u.UserName })
                .ToListAsync();

            Users = new SelectList(users, "Id", "UserName");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadUsersAsync();
                return Page();
            }

            var user = await _userManager.FindByIdAsync(Input.UserId);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{Input.UserId}'.");
            }

            // Generate reset token
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            // Reset the password without knowing the old one
            var resetResult = await _userManager.ResetPasswordAsync(user, token, Input.NewPassword);
            if (!resetResult.Succeeded)
            {
                foreach (var error in resetResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                await LoadUsersAsync();
                return Page();
            }

            _logger.LogInformation("Admin successfully reset the password for user {UserId}.", Input.UserId);
            StatusMessage = "Password was changed successfully.";
            return RedirectToPage();
        }

        private async Task LoadUsersAsync()
        {
            var users = await _userManager.Users
                .Select(u => new { u.Id, u.UserName })
                .ToListAsync();

            Users = new SelectList(users, "Id", "UserName");
        }
    }
}
