using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services.Interface;
using System.ComponentModel.DataAnnotations;

namespace BankApplication.Pages.Customer
{
    [BindProperties]
    public class UpdateModel : PageModel
    {
        private readonly ICustomerService _customerService;

        public UpdateModel( ICustomerService customerService)
        {

            _customerService = customerService;
        }

        [MaxLength(100)][Required] public string Givenname { get; set; }
        [MaxLength(100)][Required] public string Surname { get; set; }
        [StringLength(100)] public string Gender { get; set; }



        [StringLength(100)] public string Streetaddress { get; set; }
        [StringLength(10)] public string Zipcode { get; set; }
        [StringLength(2)] public string CountryCode { get; set; }
        public string Country { get; set; } // Default value
        //[Range(0, 100000, ErrorMessage = "Skriv ett tal mellan 0 och 100000")]
        // public decimal Salary { get; set; }
        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateOnly? Birthday { get; set; }
        [StringLength(50)][Required] public string City { get; set; }
        [StringLength(150)][EmailAddress] public string Emailaddress { get; set; }

        public void OnGet(int customerId)
        {
            var customerDB = _customerService.GetCustomer(customerId);
            Givenname = customerDB.Givenname;
            Surname = customerDB.Surname;
            Gender = customerDB.Gender;

            Birthday = customerDB.Birthday;
            City = customerDB.City;
            CountryCode = customerDB.CountryCode;
            Country = customerDB.Country;
            Emailaddress = customerDB.Emailaddress;
            Zipcode = customerDB.Zipcode;
            Streetaddress = customerDB.Streetaddress;
        }
        public IActionResult OnPost(int customerId)
        {
            Console.WriteLine($"Modell är giltig: {ModelState.IsValid}");

            if (!ModelState.IsValid)
            {
                // Logga alla valideringsfel
                foreach (var entry in ModelState)
                {
                    if (entry.Value.Errors.Count > 0)
                    {
                        Console.WriteLine($"Fält: {entry.Key}, Fel: {string.Join(", ", entry.Value.Errors.Select(e => e.ErrorMessage))}");
                    }
                }
            }
              //testing
            Console.WriteLine($"Modell är giltig: {ModelState.IsValid}");
            Console.WriteLine($"Givenname: {Givenname}, Surname: {Surname}, Email: {Emailaddress}, BirthdayString: {Birthday}");
            if (ModelState.IsValid)
            {
                var customerDB = _customerService.GetCustomer(customerId);
                customerDB.Givenname = Givenname;
                customerDB.Surname = Surname;
                customerDB.Gender = Gender;

                customerDB.Birthday = Birthday;
                customerDB.City = City;
                customerDB.CountryCode = CountryCode;
                customerDB.Emailaddress = Emailaddress;
                customerDB.Zipcode = Zipcode;

                customerDB.Streetaddress = Streetaddress;


                _customerService.Update(customerDB);

                return RedirectToPage("/Person/Index");
            }
            return Page();
        }
    }
}
