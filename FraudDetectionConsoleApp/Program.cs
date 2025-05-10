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

        Console.WriteLine("Klart!");
    }

    static async Task ProcessCountryAsync(string countryCode)
    {
        var lastRun = DateTime.MinValue; // tvingar igenom allt
        var newLastRun = DateTime.UtcNow;

        var report = new List<string>(); // ✅ Nu är report deklarerad

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
                .Where(d => d.Type == "OWNER") // filtrera om du vill
                .Select(d => d.Account);

            foreach (var account in accounts)
            {
                var suspicious = new List<int>();

                var transactions = account.Transactions
                    .Where(t => t.Date > lastRun)
                    .OrderBy(t => t.Date)
                    .ToList();

                Console.WriteLine($"🧾 Konto {account.AccountId} har {account.Transactions.Count} transaktioner (total)");

                foreach (var tx in transactions)
                {
                    Console.WriteLine($"🔍 Kollar TX {tx.TransactionId} på {tx.Amount} kr, typ: {tx.Type}, datum: {tx.Date}");

                    if (FraudDetectionRules.IsSuspiciousTransaction(tx) ||
                        FraudDetectionRules.HasSuspiciousRecentActivity(account.Transactions.ToList(), tx.Date))
                    {
                        Console.WriteLine($"🚨 MISSTÄNKT transaktion: {tx.TransactionId}");
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
            var reportService = new ReportService(); // Skapa en instans av ReportService
            await reportService.SaveReportAsync(countryCode, report); // Anropa den asynkrona metoden via instansen
            Console.WriteLine($"📄 Rapport sparad för {countryCode}");
        }
        else
        {
            Console.WriteLine($"✅ Inga misstänkta transaktioner för {countryCode}");
        }
    }

}
