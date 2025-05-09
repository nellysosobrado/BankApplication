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

namespace BankApplication.Areas.Identity.Pages.Account.Manage
{
    [Authorize(Roles = "Admin")]
    public class ChangePasswordModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<ChangePasswordModel> _logger;

        public ChangePasswordModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ILogger<ChangePasswordModel> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        // List of users for the admin to select from
        public SelectList Users { get; set; }

        public class InputModel
        {
            [Required]
            [Display(Name = "Select a user")]
            public string UserId { get; set; }

            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Current password")]
            public string OldPassword { get; set; }

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

        // Load users for the admin to select from
        public async Task<IActionResult> OnGetAsync()
        {
            // Only load if the user is an Admin
            if (!User.IsInRole("Admin"))
            {
                return Unauthorized();
            }

            // Get all users for the dropdown
            var users = _userManager.Users.Select(u => new { u.Id, u.UserName }).ToList();
            Users = new SelectList(users, "Id", "UserName");

            return Page();
        }

        // Handle password change for the selected user
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.FindByIdAsync(Input.UserId);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{Input.UserId}'.");
            }

            // Change password for the selected user
            var changePasswordResult = await _userManager.ChangePasswordAsync(user, Input.OldPassword, Input.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                foreach (var error in changePasswordResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return Page();
            }

            _logger.LogInformation("Admin changed the password successfully for user {UserId}.", Input.UserId);
            StatusMessage = "The password has been changed successfully.";

            return RedirectToPage();
        }
    }
}
