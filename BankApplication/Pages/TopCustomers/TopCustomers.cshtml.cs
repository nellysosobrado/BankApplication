using AutoMapper;
using AutoMapper.QueryableExtensions;
using DAL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BankApplication.ViewModels;
using Services.ViewModels;

namespace BankApplication.Pages.TopCustomers
{
    public class TopCustomersModel : PageModel
    {
        private readonly BankAppDataContext _context;
        private readonly IMapper _mapper;

        public TopCustomersModel(BankAppDataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [BindProperty(SupportsGet = true)]
        public string Country { get; set; } = "";

        public string CountryName { get; set; } = "";

        public List<CustomerSummaryViewModel> TopCustomers { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            if (string.IsNullOrWhiteSpace(Country))
                return RedirectToPage("/Index");

            CountryName = Country;

            TopCustomers = await _context.Customers
                .Where(c => c.Country == Country)
                .ProjectTo<CustomerSummaryViewModel>(_mapper.ConfigurationProvider)
                .OrderByDescending(c => c.TotalBalance)
                .Take(10)
                .ToListAsync();

            return Page();
        }
    }
}
