using DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services.Interface;
using System.ComponentModel.DataAnnotations;

namespace BankApplication.Pages.AccountPages
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


        [Range(100, 1000000, ErrorMessage = "Amount must be between 100 and 1,000,000")]
        public decimal Amount { get; set; }


        [BindProperty]
        [Required(ErrorMessage = "Please enter account number")]
        public int? TargetAccountId { get; set; }



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
            Account? targetAccount = null;

            if (TargetAccountId == null)
            {
                ModelState.AddModelError("TargetAccountId", "Please enter account number.");
            }
            else
            {
                targetAccount = _accountService.GetAccount(TargetAccountId.Value);
                if (targetAccount == null)
                {
                    ModelState.AddModelError("TargetAccountId", "Recipient account not found.");
                }
            }

            if (sourceAccount == null)
            {
                ModelState.AddModelError(string.Empty, "Source account not found.");
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
            targetAccount!.Balance += Amount; // targetAccount är säkert inte null här

            _accountService.Update(sourceAccount);
            _accountService.Update(targetAccount);

            return RedirectToPage("/CustomerPages/Details", new { id = CustomerId });
        }

    }
}
