using DAL.Models;
using Services.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface IAccountService
    {
        List<Account> GetAccounts();
        List<AccountViewModel> GetAccountViewModels();
        void Update(Account account);
        Account GetAccount(int accountId);
        bool TryWithdraw(int accountId, decimal amount, out string errorMessage);





    }

}
