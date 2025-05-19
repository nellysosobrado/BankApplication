using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using BankApplication.ViewModels;
using Services.Interface;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

namespace BankApplication.Pages.CustomerPages;

[Authorize(Roles = "Cashier")]
public class DetailsModel : PageModel
{
    private readonly ICustomerQueryService _customerService;
    private readonly ICustomerCommandService _customerCommandService;
    private readonly ILogger<DetailsModel> _logger;

    public CustomerDetailViewModel Customer { get; set; }
    public decimal TotalBalance { get; set; }

    public DetailsModel(
        ILogger<DetailsModel> logger,
        ICustomerQueryService customerService,
        ICustomerCommandService customerCommandService)
    {
        _customerService = customerService;
        _customerCommandService = customerCommandService;
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


    public async Task<IActionResult> OnPostDeleteAsync(int CustomerId)
    {
        try
        {
            var deleted = await _customerCommandService.DeleteCustomerAsync(CustomerId);

            if (!deleted)
            {
                TempData["ErrorMessage"] = "Customer could not be deleted (maybe has accounts?).";
                return RedirectToPage(new { id = CustomerId });
            }

            TempData["SuccessMessage"] = "Customer deleted successfully.";
            return RedirectToPage("/CustomerPages/Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting customer");
            TempData["ErrorMessage"] = "An error occurred while deleting the customer.";
            return RedirectToPage(new { id = CustomerId });
        }
    }
}
