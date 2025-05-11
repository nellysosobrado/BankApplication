using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services.Interface;
using System.ComponentModel.DataAnnotations;

namespace BankApplication.Pages.Account
{

    [Authorize(Roles = "Cashier,Admin")]
    [BindProperties]
    public class WithdrawModel : PageModel
    {
        private readonly IAccountService _accountService;

        public WithdrawModel(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [Range(100, 1000000, ErrorMessage = "Amount must be between 100 and 1,000,000")]
        public decimal Amount { get; set; }

        public decimal Balance { get; set; }

        public void OnGet(int accountId, int customerId)
        {
            var accountDb = _accountService.GetAccount(accountId);
            Balance = accountDb.Balance;
            ViewData["CustomerId"] = customerId;
        }

        public IActionResult OnPost(int accountId, int customerId)
        {
            if (!ModelState.IsValid)
                return Page();

            if (!_accountService.TryWithdraw(accountId, Amount, out string errorMessage))
            {
                ModelState.AddModelError(nameof(Amount), errorMessage);
                OnGet(accountId, customerId);
                return Page();
            }

            return RedirectToPage("/Customer/Details", new { id = customerId });
        }



    }
}