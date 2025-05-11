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

            _reportFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\Reports");

            if (!Directory.Exists(_reportFolderPath))
            {
                Directory.CreateDirectory(_reportFolderPath);
            }
        }

        public async Task SaveReportAsync(string countryCode, List<string> report)
        {

            string reportFilePath = Path.Combine(_reportFolderPath, $"report_{countryCode}_{DateTime.Now:yyyyMMdd_HHmmss}.txt");


            await File.WriteAllLinesAsync(reportFilePath, report);

            Console.WriteLine($"Repport for {countryCode} saved at: {reportFilePath}");
        }
    }
}
