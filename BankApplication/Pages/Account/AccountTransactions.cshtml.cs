using DAL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BankApplication.ViewModels;
using Services.ViewModels;
using Services;

namespace BankApplication.Pages
{
    public class AccountTransactionsModel : PageModel
    {
        private readonly BankAppDataContext _bankAppDataContext;

        public AccountTransactionsModel(BankAppDataContext bankAppDataContext)
        {
            _bankAppDataContext = bankAppDataContext;
        }

        [BindProperty(SupportsGet = true)]
        public int AccountId { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PageIndex { get; set; } = 1;

        public int PageSize { get; set; } = 20;
        public AccountViewModel Account { get; set; }
        public List<TransactionViewModel> Transactions { get; set; }
        public int TotalTransactions { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var account = await _bankAppDataContext.Accounts
                .Include(a => a.Transactions)
                .FirstOrDefaultAsync(a => a.AccountId == AccountId);

            if (account == null) return NotFound();

            TotalTransactions = await _bankAppDataContext.Transactions
                .Where(t => t.AccountId == AccountId)
                .CountAsync();

            Transactions = await _bankAppDataContext.Transactions
                .Where(t => t.AccountId == AccountId)
                .OrderByDescending(t => t.Date)
                .Skip((PageIndex - 1) * PageSize)
                .Take(PageSize)
                .Select(t => new TransactionViewModel
                {
                    TransactionId = t.TransactionId,
                    Type = t.Type,
                    Operation = t.Operation,
                    Amount = t.Amount,
                    Balance = t.Balance,
                    Date = t.Date
                })
                .ToListAsync();

            Account = new AccountViewModel
            {
                AccountId = account.AccountId,
                Balance = account.Balance,
                Frequency = account.Frequency,
            };

            return Page();
        }

  
        public async Task<IActionResult> OnGetLoadMoreAsync(int accountId, int skip)
        {
            const int pageSize = 20;

            var query = _bankAppDataContext.Transactions
                .Where(t => t.AccountId == accountId)
                .OrderByDescending(t => t.Date);

            var totalCount = await query.CountAsync();

            var transactions = await query
                .Skip(skip)
                .Take(pageSize)
                .Select(t => new TransactionViewModel
                {
                    TransactionId = t.TransactionId,
                    Type = t.Type,
                    Operation = t.Operation,
                    Amount = t.Amount,
                    Balance = t.Balance,
                    Date = t.Date
                })
                .ToListAsync();

            bool hasMore = totalCount > skip + pageSize;

            return new JsonResult(new { transactions, hasMore });
        }
    }
}
