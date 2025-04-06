using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using Services.Interface;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BankApplication.Pages.Person
{
    [BindProperties]
    public class NewModel : PageModel
    {
        private readonly IPersonService _personService;

        public NewModel(IPersonService personService)
        {
            _personService = personService;
        }

        [MaxLength(100)]
        [Required]
        public string Name { get; set; }

        [StringLength(100)]
        public string StreetAddress { get; set; }

        [StringLength(10)]
        public string PostalCode { get; set; }

        [StringLength(2)]
        public string CountryCode { get; set; }

        [Range(0, 100000, ErrorMessage = "Skriv ett tal mellan 0 och 100000")]
        public decimal Salary { get; set; }

        [Range(0, 100)]
        public int Age { get; set; }

        [StringLength(50)]
        [Required]
        public string City { get; set; }

        [StringLength(150)]
        [EmailAddress]
        public string Email { get; set; }

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            if (ModelState.IsValid)
            {
                var person = new DAL.Models.Person
                {
                    Age = Age,
                    StreetAddress = StreetAddress,
                    Email = Email,
                    City = City,
                    Salary = Salary,
                    Name = Name,
                    PostalCode = PostalCode,
                    CountryCode = CountryCode,
                };

                _personService.SaveNew(person);
                return RedirectToPage("Index");
            }
            return Page();
        }
    }
}
