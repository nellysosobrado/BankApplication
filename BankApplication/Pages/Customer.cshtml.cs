using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services;
using System;
using System.Threading.Tasks;
using BankApplication.ViewModels;
using Services.Interface;
using Microsoft.Extensions.Logging;

namespace BankApplication.Pages
{
    public class CustomerModel : PageModel
    {
        //private readonly ICustomerService _customerService;
        private readonly ICustomerQueryService _customerService;
        private readonly ILogger<CustomerModel> _logger;


        public CustomerDetailViewModel Customer { get; set; }
        public decimal TotalBalance { get; set; }

        public CustomerModel( ILogger<CustomerModel> logger, ICustomerQueryService customerService)
        {
            //_customerService = customerService;
            _customerService = customerService;

            _logger = logger;
        }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (!int.TryParse(id, out int customerId))
            {
                TempData["ErrorMessage"] = "Ogiltigt kund-ID";
                return RedirectToPage("/Customers");
            }

            try
            {
                Customer = await _customerService.GetCustomerByIdAsync(customerId);

                if (Customer == null)
                {
                    TempData["ErrorMessage"] = "Kunden hittades inte";
                    return RedirectToPage("/Customers");
                }

                TotalBalance = Customer.Dispositions?
                    .Where(d => d.Account != null)
                    .Sum(d => d.Account.Balance) ?? 0;

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fel vid hämtning av kunddata");
                TempData["ErrorMessage"] = "Ett fel uppstod vid hämtning av kundinformation";
                return RedirectToPage("/Customers");
            }
        }
    }
}