using DAL.Models;
using Services.Interface;
using Services.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class AccountService : IAccountService
    {
        private readonly BankAppDataContext _dbContext;

        public AccountService(BankAppDataContext dbContext)
        {
            _dbContext = dbContext;
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
            Update(account);

            errorMessage = null;
            return true;
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
                    AccountNo = a.AccountNo,
                    Balance = a.Balance
                }).ToList();
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
