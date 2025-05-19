using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services.Interface;
using Services;
using Services.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace BankApplication.Pages.AccountPages
{
    [Authorize(Roles = "Cashier,Admin")]
    public class IndexModel : PageModel
    {
        public List<AccountViewModel> Accounts { get; set; }
        private readonly IAccountService _accountService;

        public IndexModel(IAccountService accountService)
        {
            _accountService = accountService;
        }

        public void OnGet()
        {
            Accounts = _accountService.GetAccountViewModels();
        }

    }
}

