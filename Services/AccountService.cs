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

        // Metod för att hantera uttag och skapa transaktion
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

            // Subtrahera beloppet från kontots saldo
            account.Balance -= amount;

            // Skapa en transaktion för uttaget och spara den
            var transaction = new Transaction
            {
                AccountId = accountId,
                Type = "Debit",  // För uttag används "Debit"
                Operation = "Debit",  // Uttag är en debitering
                Amount = amount,
                Balance = account.Balance,  // Nytt saldo efter uttag
                Date = DateOnly.FromDateTime(DateTime.Now)  // Sätt aktuellt datum
            };

            // Spara transaktionen genom TransactionService
            _transactionService.AddTransaction(transaction);

            // Uppdatera kontot i databasen
            Update(account);

            errorMessage = null;
            return true;
        }

        // Lägg till en transaktion i databasen
        public void AddTransaction(Transaction transaction)
        {
            _transactionService.AddTransaction(transaction); // Anropa ITransactionService för att spara transaktionen
        }

        // Hämta alla konton (Account)
        public List<Account> GetAccounts()
        {
            return _dbContext.Accounts.ToList();
        }

        // Hämta alla konton som AccountViewModels
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

        // Hämta ett specifikt konto
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
