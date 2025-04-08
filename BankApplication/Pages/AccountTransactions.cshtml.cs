using DAL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BankApplication.ViewModels;
using Services.ViewModels;

namespace BankApplication.Pages
{
    public class AccountModel : PageModel
    {
        private readonly BankAppDataContext _bankAppDataContext;

        public AccountModel(BankAppDataContext bankAppDataContext)
        {
            _bankAppDataContext = bankAppDataContext;
        }

        [BindProperty(SupportsGet = true)]
        public int AccountId { get; set; }

        public AccountViewModel Account { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var account = await _bankAppDataContext.Accounts
                .Include(a => a.Transactions)
                .FirstOrDefaultAsync(a => a.AccountId == AccountId);

            if (account == null) return NotFound();

            Account = new AccountViewModel
            {
                AccountId = account.AccountId,
                Balance = account.Balance,
                Frequency = account.Frequency,
                Transactions = account.Transactions
                    .OrderByDescending(t => t.Date)
                    .Take(20)
                    .Select(t => new TransactionViewModel
                    {
                        TransactionId = t.TransactionId,
                        Type = t.Type,
                        Operation = t.Operation,
                        Amount = t.Amount,
                        Balance = t.Balance
                    }).ToList()
            };

            return Page();
        }







        public async Task<IActionResult> OnGetLoadMoreAsync(int accountId, int skip)
        {
            var transactions = await _bankAppDataContext.Transactions
                .Where(t => t.AccountId == accountId)
                .OrderByDescending(t => t.Date)
                .Skip(skip)
                .Take(20)
                .Select(t => new TransactionViewModel
                {
                    TransactionId = t.TransactionId,
                    Type = t.Type,
                    Operation = t.Operation,
                    Amount = t.Amount,
                    Balance = t.Balance
                })
                .ToListAsync();  

            return new JsonResult(transactions);
        }
    }
}
