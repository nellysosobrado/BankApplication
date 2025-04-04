using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services.Interface;
using System.ComponentModel.DataAnnotations;

namespace BankApplication.Pages.Account
{
    [BindProperties]
    public class WithdrawModel : PageModel
    {
        private readonly IAccountService _accountService;

        public WithdrawModel(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [Range(100, 10000)]
        public decimal Amount { get; set; }

        public decimal Balance { get; set; }
        public void OnGet(int accountId)
        {
            var accountDb = _accountService.GetAccount(accountId);
            Balance = accountDb.Balance;

            // Option 2 - Cleaner?
            // Balance = _accountService.GetAccount(accountId).Balance;
        }

        public IActionResult OnPost(int accountId)
        {
            var accountDb = _accountService.GetAccount(accountId);
            if (accountDb.Balance < Amount)
            {
                ModelState.AddModelError("Amount", "You don't have that much money!");
            }

            if (ModelState.IsValid)
            {
                accountDb.Balance -= Amount;
                _accountService.Update(accountDb);
                return RedirectToPage("Index");
            }
            return Page();
        }
    }
}
