
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.Models;
using BankApplication.ViewModels;
using Microsoft.EntityFrameworkCore;
using Services.Interface;

namespace Services
{

    public class StatsService : IStatsService
    {
        private readonly BankAppDataContext _context;

        public StatsService(BankAppDataContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CountryStatsViewModel>> GetCountryStatsAsync()
        {
            var query = from c in _context.Customers
                        where c.CountryCode != null
                        join d in _context.Dispositions on c.CustomerId equals d.CustomerId
                        join a in _context.Accounts on d.AccountId equals a.AccountId
                        group new { c, a } by new { c.CountryCode, c.Country } into g
                        select new CountryStatsViewModel
                        {
                            CountryCode = g.Key.CountryCode.ToLower(),
                            CountryName = g.Key.Country,
                            CustomerCount = g.Select(x => x.c.CustomerId).Distinct().Count(),
                            AccountCount = g.Select(x => x.a.AccountId).Distinct().Count(),
                            TotalBalance = g.Sum(x => x.a.Balance),
                            Currency = GetCurrencyForCountryStatic(g.Key.CountryCode)
                        };

            return await query.OrderByDescending(x => x.CustomerCount).ToListAsync();
        }

        private static string GetCurrencyForCountryStatic(string countryCode)
        {
            return countryCode?.ToUpper() switch
            {
                "SE" => "SEK",
                "NO" => "NOK",
                "DK" => "DKK",
                "FI" => "EUR",
                _ => "USD"
            };
        }
    }
}