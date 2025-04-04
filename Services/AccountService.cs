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
        public List<Account> GetAccounts()
        {
            return _dbContext.Accounts.ToList();
        }
        public List<AccountViewModel> GetAccountViewModels()
        {
            return _dbContext.Accounts
                .Select(a => new AccountViewModel
                {
                    Id = a.AccountId,
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
