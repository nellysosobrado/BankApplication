using BankApplication.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interface
{

    public interface IStatsService
    {
        Task<IEnumerable<CountryStatsViewModel>> GetCountryStatsAsync();
    }
}
