using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services.Interface;

namespace BankApplication.Pages.Person
{
    public class IndexModel : PageModel
    {
        private readonly IPersonService _personService;
        public List<PersonViewModel> Persons { get; set; }

        public class PersonViewModel
        {
            public int Id { get; set; }       
            public string Name { get; set; }
            public string City { get; set; }
            public string Email { get; set; }
        }

        public IndexModel(IPersonService personService)
        {
            _personService = personService;
        }

        public void OnGet()
        {
            Persons = _personService.GetPersons().Select(r => new PersonViewModel
            {
                City = r.City,
                Id = r.Id,
                Name = r.Name,
                Email = r.Email
            }).ToList();
        }

    }
}