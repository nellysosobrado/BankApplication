using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services.Interface;
using System.ComponentModel.DataAnnotations;

namespace BankApplication.Pages.Account
{
    [BindProperties]
    public class TransferModel : PageModel
    {
        private readonly IAccountService _accountService;

        public TransferModel(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [Range(1, 10000, ErrorMessage = "Amount must be between 1 and 10,000")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Recipient account number is required")]
        public int TargetAccountId { get; set; }

        public string? Comment { get; set; }

        public decimal Balance { get; set; }

        public void OnGet(int accountId)
        {
            Balance = _accountService.GetAccount(accountId).Balance;
        }

        public IActionResult OnPost(int accountId)
        {
            var sourceAccount = _accountService.GetAccount(accountId);
            var targetAccount = _accountService.GetAccount(TargetAccountId);

            if (targetAccount == null)
            {
                ModelState.AddModelError("TargetAccountId", "Recipient account not found");
            }

            if (sourceAccount.Balance < Amount)
            {
                ModelState.AddModelError("Amount", "Insufficient funds");
            }

            if (ModelState.IsValid)
            {
                // Perform the transfer
                sourceAccount.Balance -= Amount;
                targetAccount.Balance += Amount;

                _accountService.Update(sourceAccount);
                _accountService.Update(targetAccount);

                return RedirectToPage("Index");
            }

            // Reload balance if validation fails
            Balance = _accountService.GetAccount(accountId).Balance;
            return Page();
        }
    }
}