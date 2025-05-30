using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services.Interface;
using System.ComponentModel.DataAnnotations;

namespace BankApplication.Pages.AccountPages
{
    [Authorize(Roles = "Cashier,Admin")]
    [BindProperties]
    public class DepositModel : PageModel
    {
        private readonly IAccountService _accountService;
        private readonly ITransactionService _transactionService;

        public DepositModel(IAccountService accountService, ITransactionService transactionService)
        {
            _accountService = accountService;
            _transactionService = transactionService;
        }

        [Range(100, 1000000, ErrorMessage = "Amount must be between 100 and 1,000,000")]
        public decimal Amount { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime DepositDate { get; set; } = DateTime.Now.AddHours(1);

        [Required(ErrorMessage = "You forgot to write a comment!")]
        [MinLength(5, ErrorMessage = "Comments must be at least 5 characters long")]
        [MaxLength(250, ErrorMessage = "OK, that's just too many words")]
        public string Comment { get; set; }

        [BindProperty(SupportsGet = true)]
        public int CustomerId { get; set; }

        [BindProperty(SupportsGet = true)]
        public int AccountId { get; set; }

        public string? SuccessMessage { get; set; }

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            if (DepositDate < DateTime.Now)
            {
                ModelState.AddModelError("DepositDate", "Cannot deposit money in the past!");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var accountDb = _accountService.GetAccount(AccountId);
            if (accountDb == null)
            {
                ModelState.AddModelError("", "Account not found.");
                return Page();
            }

            var newBalance = accountDb.Balance + Amount;
            accountDb.Balance = newBalance;
            _accountService.Update(accountDb);

            var transaction = new DAL.Models.Transaction
            {
                AccountId = accountDb.AccountId,
                Amount = Amount,
                Balance = newBalance,
                Date = DepositDate,
                Type = "Credit",
                Operation = "Deposit",
            };
            _transactionService.AddTransaction(transaction);

            SuccessMessage = $"Successfully deposited {Amount} kr to account {AccountId}.";

            // Nollställ formulärdata
            Amount = 0;
            Comment = string.Empty;
            DepositDate = DateTime.Now.AddHours(1);
            ModelState.Clear();

            return Page();
        }
    }
}
