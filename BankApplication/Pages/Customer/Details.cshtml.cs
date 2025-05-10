using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services;
using System;
using System.Threading.Tasks;
using BankApplication.ViewModels;
using Services.Interface;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

namespace BankApplication.Pages.Customer
{
    [Authorize(Roles = "Cashier,Admin")]
    public class DetailsModel : PageModel
    {
        private readonly ICustomerQueryService _customerService;
        private readonly ILogger<DetailsModel> _logger;


        public CustomerDetailViewModel Customer { get; set; }
        public decimal TotalBalance { get; set; }

        public DetailsModel( ILogger<DetailsModel> logger, ICustomerQueryService customerService)
        {
            _customerService = customerService;

            _logger = logger;
        }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (!int.TryParse(id, out int customerId))
            {
                TempData["ErrorMessage"] = "Invalid Customer ID";
                return RedirectToPage("/Customers");
            }

            try
            {
                Customer = await _customerService.GetCustomerByIdAsync(customerId);

                if (Customer == null)
                {
                    TempData["ErrorMessage"] = "Customer not found";
                    return RedirectToPage("/Customers");
                }

                TotalBalance = Customer.Dispositions?
                    .Where(d => d.Account != null)
                    .Sum(d => d.Account.Balance) ?? 0;

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error on fetching");
                TempData["ErrorMessage"] = "Error fetching data";
                return RedirectToPage("/Customer/index");
            }
        }
    }
}