using BankApplication.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace BankApplication.Pages.Person
{
    [Authorize(Roles = "Cashier,Admin")]
    public class IndexModel : PageModel
    {
        private readonly ICustomerQueryService _customerQueryService;

        public List<CustomerViewModel> Customers { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SortColumn { get; set; } = "Name";

        [BindProperty(SupportsGet = true)]
        public string SortOrder { get; set; } = "asc";

        [BindProperty(SupportsGet = true)]
        public int PageSize { get; set; } = 50;

        [BindProperty(SupportsGet = true)]
        public int PageIndex { get; set; } = 1;

        public int TotalPages { get; set; }

        public IndexModel(ICustomerQueryService customerQueryService)
        {
            _customerQueryService = customerQueryService;
        }

        public void OnGet()
        {

            var allCustomers = _customerQueryService
                .GetAllCustomers()
                .Select(r => new CustomerViewModel
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
                    CountryCode = r.CountryCode
                })
                .ToList();


            if (!string.IsNullOrEmpty(SearchTerm))
            {
                allCustomers = allCustomers
                    .Where(c =>

                        int.TryParse(SearchTerm, out var id) && c.CustomerId == id ||
                        c.Givenname.Equals(SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                        c.Surname.Equals(SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                        c.City.Equals(SearchTerm, StringComparison.OrdinalIgnoreCase)
                    )
                    .ToList();
            }

            var totalCustomers = allCustomers.Count();
            TotalPages = (int)Math.Ceiling((double)totalCustomers / PageSize);


            Customers = allCustomers
                .Skip((PageIndex - 1) * PageSize) 
                .Take(PageSize)                  
                .ToList();

            ViewData["TotalPages"] = TotalPages;
        }



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
    }
}
