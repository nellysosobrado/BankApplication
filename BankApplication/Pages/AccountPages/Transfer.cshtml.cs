using DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services;
using Services.Interface;
using System.ComponentModel.DataAnnotations;

namespace BankApplication.Pages.AccountPages
{
    [Authorize(Roles = "Cashier,Admin")]
    [BindProperties]
    public class TransferModel : PageModel
    {
        private readonly IAccountService _accountService;
        private readonly ICustomerCommandService _customerService;

        public TransferModel(IAccountService accountService, ICustomerCommandService customerService)
        {
            _accountService = accountService;
            _customerService = customerService;
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

        public string? SuccessMessage { get; set; }

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
                else
                {
                    if (targetAccount.AccountId == FromAccountId)
                    {
                        ModelState.AddModelError("TargetAccountId", "You cannot transfer to the same account.");
                    }

                    var targetCustomer = _customerService.GetCustomer(targetAccount.AccountId);
                    if (targetCustomer == null)
                    {
                        ModelState.AddModelError("TargetAccountId", "Recipient customer is not available.");
                    }
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
            targetAccount!.Balance += Amount;

            _accountService.Update(sourceAccount);
            _accountService.Update(targetAccount);

            SuccessMessage = $"Successfully transferred {Amount} kr to account {TargetAccountId}.";
            Balance = sourceAccount.Balance;


            Amount = 0;
            TargetAccountId = null;
            Comment = string.Empty;

            ModelState.Clear();
            return Page();
        }
    }
}
