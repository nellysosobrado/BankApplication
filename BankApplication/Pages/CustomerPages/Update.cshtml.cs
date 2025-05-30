using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services;
using Services.Interface;
using Services.ViewModels;
using System.Linq;
using System.Text.Json;

namespace BankApplication.Pages.CustomerPages;

[Authorize(Roles = "Cashier")]
public class UpdateModel : PageModel
{
    private readonly ICustomerCommandService _commandService;

    public UpdateModel(ICustomerCommandService commandService)
    {
        _commandService = commandService;
    }

    [BindProperty]
    public CustomerInputModel Input { get; set; }

    [TempData]
    public string? TempInputJson { get; set; }

    [TempData]
    public string? TempModelStateJson { get; set; }

    public IActionResult OnGet(int id)
    {
   
        Response.Headers["Cache-Control"] = "no-store, no-cache, must-revalidate, max-age=0";
        Response.Headers["Pragma"] = "no-cache";
        Response.Headers["Expires"] = "0";

        if (!string.IsNullOrEmpty(TempInputJson))
        {
            Input = JsonSerializer.Deserialize<CustomerInputModel>(TempInputJson);
        }
        else
        {
            var customer = _commandService.GetCustomer(id);
            if (customer == null) return NotFound();

            Input = new CustomerInputModel
            {
                CustomerId = customer.CustomerId,
                Givenname = customer.Givenname,
                Surname = customer.Surname,
                Gender = customer.Gender,
                Birthday = customer.Birthday,
                City = customer.City,
                CountryCode = customer.CountryCode,
                Country = customer.Country,
                Emailaddress = customer.Emailaddress,
                Zipcode = customer.Zipcode,
                Streetaddress = customer.Streetaddress,
                NationalId = customer.NationalId,
                Telephonecountrycode = customer.Telephonecountrycode,
                Telephonenumber = customer.Telephonenumber
            };
        }

        if (!string.IsNullOrEmpty(TempModelStateJson))
        {
            var errors = JsonSerializer.Deserialize<Dictionary<string, string[]>>(TempModelStateJson);
            if (errors != null)
            {
                foreach (var kvp in errors)
                {
                    foreach (var error in kvp.Value)
                    {
                        ModelState.AddModelError(kvp.Key, error);
                    }
                }
            }
        }

        return Page();
    }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
        {
            TempInputJson = JsonSerializer.Serialize(Input);

            var errors = ModelState
                .Where(e => e.Value?.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray() ?? new string[0]);

            TempModelStateJson = JsonSerializer.Serialize(errors);

            return RedirectToPage(new { id = Input.CustomerId });
        }

        var customer = _commandService.GetCustomer(Input.CustomerId);
        if (customer == null) return NotFound();

        customer.Givenname = Input.Givenname;
        customer.Surname = Input.Surname;
        customer.Gender = Input.Gender;
        customer.Birthday = Input.Birthday;
        customer.City = Input.City;
        customer.CountryCode = Input.CountryCode;
        customer.Country = Input.Country;
        customer.Emailaddress = Input.Emailaddress;
        customer.Zipcode = Input.Zipcode;
        customer.Streetaddress = Input.Streetaddress;
        customer.NationalId = Input.NationalId;
        customer.Telephonecountrycode = Input.Telephonecountrycode;
        customer.Telephonenumber = Input.Telephonenumber;

        _commandService.UpdateCustomer(customer);

        return RedirectToPage("/CustomerPages/Details", new { id = Input.CustomerId });
    }
}
