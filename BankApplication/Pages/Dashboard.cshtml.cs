using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace BankApplication.Pages
{
    [Authorize] // Only for logged-in users
    public class DashboardModel : PageModel
    {

        public void OnGet()
        {
        
        }
    }
}
