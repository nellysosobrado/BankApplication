using DAL.Models;
using FraudDetectionConsoleApp;
using FraudDetectionConsoleApp.Services;

class Program
{
    static async Task Main()
    {
        var countries = new List<string> { "SE", "NO", "DK", "FI" }; 

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

        using var db = BankAppDataContextFactory.Create();
        var fraudService = new FraudDetectionService(db, debug: true); // set to false in production

        var report = await fraudService.AnalyzeCountryAsync(countryCode, lastRun);

        var reportService = new ReportService();
        await reportService.SaveReportAsync(countryCode, report);

        if (report.Any())
        {
            Console.WriteLine($" Suspicious activity saved to report for {countryCode}");
        }
        else
        {
            Console.WriteLine($" No suspicious transactions found for {countryCode} – but report saved.");
        }

        ProcessLogService.UpdateLog(countryCode, newLastRun);
    }




}
