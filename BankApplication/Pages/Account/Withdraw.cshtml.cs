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

        [Range(100, 10000, ErrorMessage = "Amount must be between 100 and 10 000")]
        public decimal Amount { get; set; }

        public decimal Balance { get; set; }

        public void OnGet(int accountId)
        {
            var accountDb = _accountService.GetAccount(accountId);
            Balance = accountDb.Balance;
        }

        public IActionResult OnPost(int accountId)
        {
            if (!ModelState.IsValid)
                return Page();

            if (!_accountService.TryWithdraw(accountId, Amount, out string errorMessage))
            {
                ModelState.AddModelError(nameof(Amount), errorMessage);
                OnGet(accountId);
                return Page();
            }

            return RedirectToPage("Index");
        }


    }
}