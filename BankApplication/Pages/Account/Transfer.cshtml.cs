using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services.Interface;
using System.ComponentModel.DataAnnotations;

namespace BankApplication.Pages.Account
{
    [Authorize(Roles = "Cashier,Admin")]
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

        [BindProperty(SupportsGet = true)]
        public int FromAccountId { get; set; }

        [BindProperty(SupportsGet = true)]
        public int CustomerId { get; set; }

        public void OnGet()
        {
            var source = _accountService.GetAccount(FromAccountId);
            Balance = source?.Balance ?? 0;
        }

        public IActionResult OnPost()
        {
            var sourceAccount = _accountService.GetAccount(FromAccountId);
            var targetAccount = _accountService.GetAccount(TargetAccountId);

            if (sourceAccount == null)
            {
                ModelState.AddModelError(string.Empty, "Source account not found.");
            }

            if (targetAccount == null)
            {
                ModelState.AddModelError("TargetAccountId", "Recipient account not found.");
            }

            if (sourceAccount != null && sourceAccount.Balance < Amount)
            {
                ModelState.AddModelError("Amount", "Insufficient funds.");
            }

            if (!ModelState.IsValid)
            {
                Balance = sourceAccount?.Balance ?? 0;
                return Page();
            }

            sourceAccount.Balance -= Amount;
            targetAccount.Balance += Amount;

            _accountService.Update(sourceAccount);
            _accountService.Update(targetAccount);

            return RedirectToPage("/Customer", new { id = CustomerId });
        }
    }
}
