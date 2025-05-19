using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services;
using System.ComponentModel.DataAnnotations;
using DAL.Models;

namespace BankApplication.Pages.CustomerPages;

[BindProperties]
public class UpdateModel : PageModel
{
    private readonly ICustomerCommandService _commandService;

    public UpdateModel(ICustomerCommandService commandService)
    {
        _commandService = commandService;
    }

    [HiddenInput]
    public int CustomerId { get; set; }

    [MaxLength(100)][Required] public string Givenname { get; set; }
    [MaxLength(100)][Required] public string Surname { get; set; }
    [StringLength(100)] public string Gender { get; set; }

    [StringLength(100)] public string Streetaddress { get; set; }
    [StringLength(10)] public string Zipcode { get; set; }
    [StringLength(2)] public string CountryCode { get; set; }
    public string Country { get; set; }

    [DataType(DataType.Date)]
    public DateOnly? Birthday { get; set; }

    [StringLength(50)][Required] public string City { get; set; }
    [StringLength(150)][EmailAddress] public string Emailaddress { get; set; }

    public IActionResult OnGet(int id)
    {
        var customer = _commandService.GetCustomer(id);
        if (customer == null) return NotFound();

        CustomerId = customer.CustomerId;
        Givenname = customer.Givenname;
        Surname = customer.Surname;
        Gender = customer.Gender;
        Birthday = customer.Birthday;
        City = customer.City;
        CountryCode = customer.CountryCode;
        Country = customer.Country;
        Emailaddress = customer.Emailaddress;
        Zipcode = customer.Zipcode;
        Streetaddress = customer.Streetaddress;

        return Page();
    }


    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
            return Page();

        var customer = _commandService.GetCustomer(CustomerId);
        if (customer == null) return NotFound();

        customer.Givenname = Givenname;
        customer.Surname = Surname;
        customer.Gender = Gender;
        customer.Birthday = Birthday;
        customer.City = City;
        customer.CountryCode = CountryCode;
        customer.Emailaddress = Emailaddress;
        customer.Zipcode = Zipcode;
        customer.Streetaddress = Streetaddress;
        customer.Country = Country;
        customer.LastModified = DateTime.UtcNow;

        _commandService.UpdateCustomer(customer);

        return RedirectToPage("/Customer/Details", new { id = CustomerId }); 
    }

}
