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
            public int Id { get; set; }
            public string Name { get; set; }
            public string City { get; set; }
            public string Email { get; set; }
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
                Id = r.CustomerId,
                Name = r.Givenname,
                Email = r.Emailaddress
            }).ToList();
        }

    }
}