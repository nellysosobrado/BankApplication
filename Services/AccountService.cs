using DAL.Models;
using Services.Interface;
using Services.ViewModels;
using System.Linq;
using System.Collections.Generic;

namespace Services
{
    public class AccountService : IAccountService
    {
        private readonly BankAppDataContext _dbContext;
        private readonly ITransactionService _transactionService;

        public AccountService(BankAppDataContext dbContext, ITransactionService transactionService)
        {
            _dbContext = dbContext;
            _transactionService = transactionService;
        }


        public bool TryWithdraw(int accountId, decimal amount, out string errorMessage)
        {
            var account = GetAccount(accountId);
            if (account == null)
            {
                errorMessage = "Account not found.";
                return false;
            }

            if (amount <= 0)
            {
                errorMessage = "Withdrawal amount must be greater than zero.";
                return false;
            }

            if (account.Balance < amount)
            {
                errorMessage = "Insufficient funds in the account.";
                return false;
            }


            account.Balance -= amount;

            var transaction = new Transaction
            {
                AccountId = accountId,
                Type = "Debit",  
                Operation = "Debit",  
                Amount = amount,
                Balance = account.Balance,
                Date = DateTime.Now

            };

            _transactionService.AddTransaction(transaction);

            Update(account);

            errorMessage = null;
            return true;
        }


        public void AddTransaction(Transaction transaction)
        {
            _transactionService.AddTransaction(transaction); 
        }

        public List<Account> GetAccounts()
        {
            return _dbContext.Accounts.ToList();
        }

        public List<AccountViewModel> GetAccountViewModels()
        {
            return _dbContext.Accounts
                .Select(a => new AccountViewModel
                {
                    AccountId = a.AccountId,
                    Balance = a.Balance
                })
                .ToList();
        }

        public Account GetAccount(int accountId)
        {
            return _dbContext.Accounts.First(a => a.AccountId == accountId);
        }

        public void Update(Account account)
        {
            _dbContext.SaveChanges();
        }
    }
}
