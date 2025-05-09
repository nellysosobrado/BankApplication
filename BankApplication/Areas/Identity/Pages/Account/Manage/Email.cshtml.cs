using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace BankApplication.Areas.Identity.Pages.Account.Manage
{
    [Authorize(Roles = "Admin")]
    public class EmailModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IEmailSender _emailSender;

        public EmailModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
        }

        /// <summary>
        /// List of all email addresses to be displayed and updated by the admin
        /// </summary>
        public List<UserEmailModel> UserEmails { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "New email")]
            public string NewEmail { get; set; }
        }

        public class UserEmailModel
        {
            public string UserId { get; set; }
            public string CurrentEmail { get; set; }
        }

        private async Task LoadAsync()
        {
            var users = _userManager.Users.ToList();
            UserEmails = users.Select(user => new UserEmailModel
            {
                UserId = user.Id,
                CurrentEmail = user.Email
            }).ToList();
        }

        public async Task<IActionResult> OnGetAsync()
        {
            await LoadAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostChangeEmailAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{userId}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync();
                return Page();
            }

            if (Input.NewEmail != user.Email)
            {
                var code = await _userManager.GenerateChangeEmailTokenAsync(user, Input.NewEmail);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Page(
                    "/Account/ConfirmEmailChange",
                    pageHandler: null,
                    values: new { area = "Identity", userId = userId, email = Input.NewEmail, code = code },
                    protocol: Request.Scheme);

                await _emailSender.SendEmailAsync(
                    Input.NewEmail,
                    "Confirm your email",
                    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                StatusMessage = $"Verification email sent. Please check your email. If you don't see the email, check your spam folder. <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>Click here to confirm your email change</a>.";
                return RedirectToPage();
            }

            StatusMessage = "The email is unchanged.";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostSendVerificationEmailAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{userId}'.");
            }

            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new { area = "Identity", userId = userId, code = code },
                protocol: Request.Scheme);
            await _emailSender.SendEmailAsync(
                user.Email,
                "Confirm your email",
                $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

            StatusMessage = "Verification email sent. Please check your email.";
            return RedirectToPage();
        }
    }
}
