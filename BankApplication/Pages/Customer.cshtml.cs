using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services;
using System;
using System.Threading.Tasks;
using BankApplication.ViewModels;

namespace BankApplication.Pages
{
    public class CustomerModel : PageModel
    {
        private readonly ICustomerService _customerService;
        private readonly ILogger<CustomerModel> _logger;

        public CustomerModel(
            ICustomerService customerService,
            ILogger<CustomerModel> logger)
        {
            _customerService = customerService;
            _logger = logger;
        }

        public CustomerDetailViewModel Customer { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id) || !int.TryParse(id, out int customerId))
                {
                    _logger.LogWarning("Invalid customer ID requested: {CustomerId}", id);
                    return RedirectToPage("/Customers");
                }

                Customer = await _customerService.GetCustomerByIdAsync(customerId);

                if (Customer == null)
                {
                    _logger.LogWarning("Customer with ID {CustomerId} not found", id);
                    TempData["ErrorMessage"] = $"Customer with ID {id} not found";
                    return RedirectToPage("/Customers");
                }

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading customer with ID {CustomerId}", id);
                TempData["ErrorMessage"] = "An error occurred while loading customer details";
                return RedirectToPage("/Customers");
            }
        }
    }
}