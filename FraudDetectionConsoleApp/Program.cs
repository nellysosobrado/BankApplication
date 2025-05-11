using DAL.Models;
using FraudDetectionConsoleApp.Services;
using FraudDetectionConsoleApp.Rules;
using Microsoft.EntityFrameworkCore;

class Program
{
    static async Task Main()
    {
        var countries = new List<string> { "SE", "NO", "DK" };

        foreach (var country in countries)
        {
            await ProcessCountryAsync(country);
        }

        Console.WriteLine("Finished!");
    }

    static async Task ProcessCountryAsync(string countryCode)
    {
        var lastRun = ProcessLogService.GetLastProcessed(countryCode);

        var newLastRun = DateTime.UtcNow;

        var report = new List<string>();

        using var db = new BankAppDataContext();

        var customers = await db.Customers
            .Where(c => c.CountryCode == countryCode)
            .Include(c => c.Dispositions)
                .ThenInclude(d => d.Account)
                    .ThenInclude(a => a.Transactions)
            .ToListAsync();

        foreach (var customer in customers)
        {
            var accounts = customer.Dispositions
                .Where(d => d.Type == "OWNER")
                .Select(d => d.Account);

            foreach (var account in accounts)
            {
                var suspicious = new List<int>();

                var transactions = account.Transactions
                    .Where(t => t.Date > lastRun)
                    .OrderBy(t => t.Date)
                    .ToList();

                Console.WriteLine($"Account {account.AccountId} has {transactions.Count} new  transaction after {lastRun})");

                foreach (var tx in transactions)
                {
                    Console.WriteLine($" Analyzing {tx.TransactionId} on {tx.Amount} kr, type: {tx.Type}, date: {tx.Date}");

                    if (FraudDetectionRules.IsSuspiciousTransaction(tx) ||
                        FraudDetectionRules.HasSuspiciousRecentActivity(account.Transactions.ToList(), tx.Date))
                    {
                        Console.WriteLine($"🚨 Suspicious transaction: {tx.TransactionId}");
                        suspicious.Add(tx.TransactionId);
                    }
                }

                if (suspicious.Any())
                {
                    report.Add($"[Person: {customer.Givenname} {customer.Surname}]");
                    report.Add($"Account: {account.AccountId}");
                    report.Add($"Suspicious Transaction IDs: {string.Join(", ", suspicious)}");
                    report.Add("");
                }
            }
        }

        if (report.Any())
        {
            var reportService = new ReportService();
            await reportService.SaveReportAsync(countryCode, report);
            Console.WriteLine($"Repport saved for: {countryCode}");
        }
        else
        {
            Console.WriteLine($"No sus transactions for {countryCode}");
        }

        ProcessLogService.UpdateLog(countryCode, newLastRun);
        Console.WriteLine($" Logg updated for {countryCode}: {newLastRun}");

        Console.WriteLine($"Logfile saved on: {Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs", "process_log.json")}");
    }
}
