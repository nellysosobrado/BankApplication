using DAL.Models;
using FraudDetectionConsoleApp.Rules;
using Microsoft.EntityFrameworkCore;

namespace FraudDetectionConsoleApp.Services
{
    public class FraudDetectionService
    {
        private readonly BankAppDataContext _db;
        private readonly List<IFraudRule> _rules;
        private readonly bool _debug;

        public FraudDetectionService(BankAppDataContext db, bool debug = false)
        {
            _db = db;
            _debug = debug;
            _rules = new List<IFraudRule>
            {
                new LargeTransactionRule(),
                new SumLast72HoursRule()
            };
        }

        public async Task<List<string>> AnalyzeCountryAsync(string countryCode, DateTime lastRun)
        {
            var report = new List<string>();

            var customers = await _db.Customers
                .Where(c => c.CountryCode == countryCode)
                .Include(c => c.Dispositions)
                    .ThenInclude(d => d.Account)
                        .ThenInclude(a => a.Transactions)
                .ToListAsync();

            int totalAccounts = 0;
            int accountsWithNewTransactions = 0;
            int suspiciousAccounts = 0;

            foreach (var customer in customers)
            {
                var accounts = customer.Dispositions
                    .Where(d => d.Type == "OWNER")
                    .Select(d => d.Account);

                foreach (var account in accounts)
                {
                    totalAccounts++;

                    var transactions = account.Transactions
                        .Where(t => t.Date > lastRun)
                        .OrderBy(t => t.Date)
                        .ToList();

                    if (transactions.Any())
                    {
                        accountsWithNewTransactions++;
                        if (_debug)
                        {
                            Console.WriteLine($" Account {account.AccountId} has {transactions.Count} new transactions after {lastRun:yyyy-MM-dd HH:mm}");
                        }
                    }

                    var suspicious = new List<Transaction>();

                    foreach (var tx in transactions)
                    {
                        if (_rules.Any(rule => rule.IsSuspicious(tx, account.Transactions)))
                        {
                            suspicious.Add(tx);
                            if (_debug)
                            {
                                Console.WriteLine($" Suspicious transaction: {tx.TransactionId} ({tx.Amount:N0} kr, {tx.Date:yyyy-MM-dd})");
                            }
                        }
                    }

                    if (suspicious.Any())
                    {
                        suspiciousAccounts++;

                        report.Add($"[Person: {customer.Givenname} {customer.Surname}]");
                        report.Add($"Account: {account.AccountId}");
                        report.Add($"Suspicious Transaction IDs: {string.Join(", ", suspicious.Select(s => s.TransactionId))}");
                        report.Add("");
                    }
                }
            }

            Console.WriteLine();
            Console.WriteLine($" Summary for {countryCode}:");
            Console.WriteLine($" - Accounts analyzed: {totalAccounts}");
            Console.WriteLine($" - Accounts with new transactions: {accountsWithNewTransactions}");
            Console.WriteLine($" - Accounts with suspicious activity: {suspiciousAccounts}");
            Console.WriteLine();

            return report;
        }
    }
}
