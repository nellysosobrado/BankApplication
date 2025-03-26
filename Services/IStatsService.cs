using BankApplication.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    //This interface is used to define the methods that the StatsService class should implement
    public interface IStatsService
    {
        Task<IEnumerable<CountryStatsViewModel>> GetCountryStatsAsync();
    }
}
