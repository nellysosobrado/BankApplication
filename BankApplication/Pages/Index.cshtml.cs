using BankApplication.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services.Interface;

namespace BankApplication.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IStatsService _statsService;

        public IndexModel(IStatsService statsService)
        {
            _statsService = statsService;
        }

        public List<CountryStatsViewModel> CountryStats { get; set; }

        public async Task OnGetAsync()
        {
            CountryStats = (await _statsService.GetCountryStatsAsync()).ToList();
        }
    }
}