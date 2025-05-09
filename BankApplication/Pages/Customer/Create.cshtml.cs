//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.RazorPages;
//using Services.Interface;
//using System.ComponentModel.DataAnnotations;
//using DAL.Models;
//using Services;

//namespace BankApplication.Pages.Customer
//{
//    [BindProperties]
//    public class CreateModel : PageModel
//    {
//        private readonly ICustomerCommandService _customerService;


//        public CreateModel(ICustomerCommandService customerService)
//        {
//            _customerService = customerService;
//        }
//        [MaxLength(100)]
//        [Required(ErrorMessage = "Please enter Givenname")]
//        public string Givenname { get; set; }

//        [MaxLength(100)]
//        [Required(ErrorMessage = "Please enter Surname")]
//        public string Surname { get; set; }

//        [Required(ErrorMessage = "Please enter gender")]
//        public string Gender { get; set; } = "Male"; 

//        [DataType(DataType.Date)]
//        public DateOnly? Birthday { get; set; }

//        [StringLength(100)]
//        [Required(ErrorMessage = "Please enter adress")]
//        public string Streetaddress { get; set; }

//        [StringLength(10)]
//        [Required(ErrorMessage = "Please enter ZipCode")]
//        public string Zipcode { get; set; }

//        [StringLength(50)]
//        [Required(ErrorMessage = "Please enter a city")]
//        public string City { get; set; }

//        [Required(ErrorMessage = "Please enter a city")]
//        public string Country { get; set; } = "Sweden"; 

//        [StringLength(2, MinimumLength = 2, ErrorMessage = "Country code must be 2 letters")]
//        [Required(ErrorMessage = "Pleas eneter country code")]
//        public string CountryCode { get; set; } = "SE"; 
//        [StringLength(150)]
//        [EmailAddress(ErrorMessage = "Invalid Email")]
//        public string? Emailaddress { get; set; }

//        public void OnGet()
//        {
//        }

//        public IActionResult OnPost()
//        {
//            Console.WriteLine($"Error: {ModelState.IsValid}");
//            if (ModelState.IsValid)
//            {
//                var customer = new DAL.Models.Customer
//                {
//                    Givenname = Givenname,
//                    Surname = Surname,
//                    Gender = Gender,
//                    Streetaddress = Streetaddress,
//                    Zipcode = Zipcode,
//                    City = City,
//                    Country = Country,
//                    CountryCode = CountryCode,
//                    Emailaddress = Emailaddress,
//                    Birthday = Birthday,
//                    Registered = DateTime.UtcNow,
//                    LastModified = DateTime.UtcNow
//                };

//                _customerService.CreateCustomer(customer);
//                return RedirectToPage("/Person/Index");
//            }
//            return Page();
//        }
//    }
//}