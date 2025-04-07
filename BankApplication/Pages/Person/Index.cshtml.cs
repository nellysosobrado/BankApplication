using BankApplication.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services.Interface;

namespace BankApplication.Pages.Person
{
    public class IndexModel : PageModel
    {
        //private readonly IPersonService _personService;
        private readonly ICustomerService _customerService;
        //public List<PersonViewModel> Persons { get; set; }
        public List<CustomerViewModel> Customers { get; set; }

        public class CustomerViewModel
        {
            public int CustomerId { get; set; }
            public string Givenname { get; set; }
            public string City { get; set; }
            public string Emailaddress { get; set; }
            public string Surname { get; set; }
            public string Gender { get; set; }
            public DateOnly? Birthday { get; set; }
            public string Streetaddress { get; set; }
            public string Zipcode { get; set; }
            public string Country { get; set; }
            public string CountryCode { get; set; }

        }

        public IndexModel(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        public void OnGet()
        {
            Customers = _customerService.GetCustomer().Select(r => new CustomerViewModel
            {
                City = r.City,
                CustomerId = r.CustomerId,
                Givenname = r.Givenname,
                Emailaddress = r.Emailaddress,
                Surname = r.Surname,
                Gender = r.Gender,
                Birthday = r.Birthday,
                Streetaddress = r.Streetaddress,
                Zipcode = r.Zipcode,
                Country = r.Country,
            }).ToList();
        }

    }
}