using FraudDetectionConsoleApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FraudDetectionConsoleApp.Services
{
    public static class ProcessLogService
    {
        private static readonly string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\Logs", "process_log.json");



        public static List<ProcessLog> LoadLogs()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(logPath)!); 
            if (!File.Exists(logPath)) return new();
            var json = File.ReadAllText(logPath);
            return JsonSerializer.Deserialize<List<ProcessLog>>(json) ?? new();
        }

        public static void SaveLogs(List<ProcessLog> logs)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(logPath)!); 
            var json = JsonSerializer.Serialize(logs, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(logPath, json);
        }

        public static DateTime GetLastProcessed(string country)
        {
            var logs = LoadLogs();
            return logs.FirstOrDefault(x => x.Country == country)?.LastProcessed ?? DateTime.MinValue;
        }

        public static void UpdateLog(string country, DateTime time)
        {
            var logs = LoadLogs();
            var log = logs.FirstOrDefault(x => x.Country == country);
            if (log == null)
                logs.Add(new ProcessLog { Country = country, LastProcessed = time });
            else
                log.LastProcessed = time;

            SaveLogs(logs);
        }
    }


}
