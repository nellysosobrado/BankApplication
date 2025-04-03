using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BankApplication.Pages
{
    [Authorize]// only for logged in people
    public class DashboardModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
