using System.Text.Json;

namespace FraudDetectionConsoleApp.Services
{
    public class ReportService
    {
        private readonly string _baseFolderPath;

        public ReportService(string? customBasePath = null)
        {
            try
            {

                var projectRoot = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\.."));
                _baseFolderPath = Path.Combine(projectRoot, customBasePath ?? "Reports");


                Directory.CreateDirectory(_baseFolderPath);
                Console.WriteLine($" Report folder saved at: {_baseFolderPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($" ERROR..Couldnt create report folder: {ex.Message}");
            }
        }


        public async Task SaveReportAsync(string countryCode, List<string> report)
        {
            string safeCountry = string.Concat(countryCode.Where(char.IsLetterOrDigit));
            string timeStamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string subFolder = Path.Combine(_baseFolderPath, safeCountry);

            Directory.CreateDirectory(subFolder);
            Console.WriteLine($" Sub folder for country: {subFolder}");

            string fileName = $"report_{safeCountry}_{timeStamp}";
            string txtPath = Path.Combine(subFolder, fileName + ".txt");
            string jsonPath = Path.Combine(subFolder, fileName + ".json");


            if (!report.Any())
            {
                report.Add(" No sus transactions found.");
            }

            var header = BuildHeader(countryCode, report);
            var fullTextReport = header.Concat(report).ToList();

            try
            {
                await File.WriteAllLinesAsync(txtPath, fullTextReport);
                Console.WriteLine($" Report saved (txt): {txtPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($" Error when saving report: {ex.Message}");
            }

            try
            {
                var json = JsonSerializer.Serialize(report, new JsonSerializerOptions { WriteIndented = true });
                await File.WriteAllTextAsync(jsonPath, json);
                Console.WriteLine($" Report saved at  (json): {jsonPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($" Couldnt save repport in json: {ex.Message}");
            }
        }

        private List<string> BuildHeader(string countryCode, List<string> report)
        {
            int suspectedAccounts = report.Count(s => s.StartsWith("[Person:"));

            return new List<string>
    {
        $"--- Report for {countryCode.ToUpper()} ---",
        $"Date: {DateTime.Now:yyyy-MM-dd HH:mm:ss}",
        $"Suspected accounts: {suspectedAccounts}",
        ""
    };
        }

    }
}
