// Services/StatsService.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.Models;
using BankApplication.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Services
{
    //here we are using the BankAppDataContext to get the data from the database
    public class StatsService : IStatsService
    {
        private readonly BankAppDataContext _context;

        public StatsService(BankAppDataContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CountryStatsViewModel>> GetCountryStatsAsync()
        {
            //groups customers by country and calculates statistics
            var countryStats = await _context.Customers
                .Where(c => c.CountryCode != null)
                .GroupBy(c => new { c.CountryCode, c.Country })
                .Select(g => new
                {
                    CountryCode = g.Key.CountryCode,
                    CountryName = g.Key.Country,
                    CustomerCount = g.Count(),
                    AccountCount = _context.Dispositions
                        .Count(d => g.Select(c => c.CustomerId).Contains(d.CustomerId)),
                    TotalBalance = _context.Dispositions
                        .Where(d => g.Select(c => c.CustomerId).Contains(d.CustomerId))
                        .Join(_context.Accounts,
                            d => d.AccountId,
                            a => a.AccountId,
                            (d, a) => a.Balance)
                        .Sum()
                })
                .ToListAsync();

            //maps to ViewModel in order to return the data
            var result = countryStats.Select(cs => new CountryStatsViewModel
            {
                CountryCode = cs.CountryCode?.ToLower() ?? "unknown",
                CountryName = cs.CountryName ?? "Okänt land",
                CustomerCount = cs.CustomerCount,
                AccountCount = cs.AccountCount,
                TotalBalance = cs.TotalBalance,
                Currency = GetCurrencyForCountry(cs.CountryCode)
            });

            return result;
        }

        private string GetCurrencyForCountry(string countryCode)
        {
            return countryCode?.ToUpper() switch
            {
                "SE" => "SEK",
                "NO" => "NOK",
                "DK" => "DKK",
                "FI" => "EUR",
                _ => "USD" // Fallback valuta
            };
        }
    }
}