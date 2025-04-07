using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services.Interface;
using System.ComponentModel.DataAnnotations;
using DAL.Models;

namespace BankApplication.Pages.Customer
{
    [BindProperties]
    public class CreateModel : PageModel
    {
        private readonly ICustomerService _customerService;

        public CreateModel(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        // Personal Information
        [MaxLength(100)]
        [Required(ErrorMessage = "Förnamn är obligatoriskt")]
        public string Givenname { get; set; }

        [MaxLength(100)]
        [Required(ErrorMessage = "Efternamn är obligatoriskt")]
        public string Surname { get; set; }

        [Required(ErrorMessage = "Kön måste anges")]
        public string Gender { get; set; } = "Male"; // Default value

        [DataType(DataType.Date)]
        public DateOnly? Birthday { get; set; }


        // Address Information
        [StringLength(100)]
        [Required(ErrorMessage = "Gatuadress är obligatorisk")]
        public string Streetaddress { get; set; }

        [StringLength(10)]
        [Required(ErrorMessage = "Postnummer är obligatoriskt")]
        public string Zipcode { get; set; }

        [StringLength(50)]
        [Required(ErrorMessage = "Stad är obligatorisk")]
        public string City { get; set; }

        [Required(ErrorMessage = "Land är obligatoriskt")]
        public string Country { get; set; } = "Sweden"; // Default value

        [StringLength(2, MinimumLength = 2, ErrorMessage = "Landskod måste vara 2 tecken")]
        [Required(ErrorMessage = "Landskod är obligatorisk")]
        public string CountryCode { get; set; } = "SE"; // Default value

        // Contact Information
        [StringLength(150)]
        [EmailAddress(ErrorMessage = "Ogiltig e-postadress")]
        public string? Emailaddress { get; set; }

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            Console.WriteLine($"Modell är giltig: {ModelState.IsValid}");
            if (ModelState.IsValid)
            {
                var customer = new DAL.Models.Customer
                {
                    Givenname = Givenname,
                    Surname = Surname,
                    Gender = Gender,
                    Streetaddress = Streetaddress,
                    Zipcode = Zipcode,
                    City = City,
                    Country = Country,
                    CountryCode = CountryCode,
                    Emailaddress = Emailaddress,
                    Birthday = Birthday,
                    Registered = DateTime.UtcNow,
                    LastModified = DateTime.UtcNow
                };

                _customerService.SaveNew(customer);
                return RedirectToPage("/Person/Index");
            }
            return Page();
        }
    }
}