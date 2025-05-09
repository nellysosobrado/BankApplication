using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace BankApplication.Areas.Admin.Pages.Users
{
    [Authorize(Roles = "Admin")]
    public class CreateModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public CreateModel(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Required]
            public string Role { get; set; }
        }

        // Lista av tillg�ngliga roller
        public string[] Roles { get; set; } = { "Admin", "Cashier" };

        public async Task<IActionResult> OnGetAsync()
        {
            return Page();  // Skicka till samma sida f�r att visa formul�ret
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            // Kontrollera om anv�ndaren redan finns
            var existingUser = await _userManager.FindByEmailAsync(Input.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError(string.Empty, "En anv�ndare med denna e-postadress finns redan.");
                return Page();
            }

            // Skapa anv�ndare
            var user = new IdentityUser { UserName = Input.Email, Email = Input.Email };
            var result = await _userManager.CreateAsync(user, Input.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return Page();
            }

            // Kontrollera om rollen finns och skapa den om den inte g�r det
            if (!await _roleManager.RoleExistsAsync(Input.Role))
            {
                await _roleManager.CreateAsync(new IdentityRole(Input.Role));
            }

            // L�gg till anv�ndaren i rollen
            await _userManager.AddToRoleAsync(user, Input.Role);
            return RedirectToPage("Index");
        }
    }
}
