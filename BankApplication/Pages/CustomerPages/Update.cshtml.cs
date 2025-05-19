using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services;
using Services.Interface;
using Services.ViewModels;


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

    public IActionResult OnGet(int id)
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

        return Page();
    }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
            return Page();

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
