using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace FraudDetectionConsoleApp.Services
{
    public class ReportService
    {
        private readonly string _reportFolderPath;

        public ReportService()
        {
            // Hitta den absoluta sökvägen till Reports-mappen i BankApplication\FraudDetectionConsoleApp
            _reportFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\Reports");

            // Kontrollera om mappen finns, annars skapa den
            if (!Directory.Exists(_reportFolderPath))
            {
                Directory.CreateDirectory(_reportFolderPath);
            }
        }

        // Asynkron metod för att spara rapporten
        public async Task SaveReportAsync(string countryCode, List<string> report)
        {
            // Skapa rapportfilens sökväg baserat på landkoden och datumet
            string reportFilePath = Path.Combine(_reportFolderPath, $"report_{countryCode}_{DateTime.Now:yyyyMMdd_HHmmss}.txt");

            // Spara rapporten asynkront
            await File.WriteAllLinesAsync(reportFilePath, report);

            Console.WriteLine($"Rapport för {countryCode} sparad på: {reportFilePath}");
        }
    }
}
