using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using DAL.Models;
using Services.ViewModels;

namespace BankApplication.Pages.CustomerPages
{
    [Authorize(Roles = "Cashier")]
    public class CreateModel : PageModel
    {
        private readonly BankAppDataContext _context;
        private readonly IMapper _mapper;

        public CreateModel(BankAppDataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [BindProperty]
        public CustomerCreateViewModel Input { get; set; } = new();

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

   
            var customer = _mapper.Map<DAL.Models.Customer>(Input);
            customer.Registered = DateTime.Now;
            customer.LastModified = DateTime.Now;

            _context.Customers.Add(customer);

            var account = new DAL.Models.Account
            {
                Frequency = "Monthly",
                Created = DateOnly.FromDateTime(DateTime.Now),
                Balance = 0
            };
            _context.Accounts.Add(account);

     
            var disposition = new Disposition
            {
                Customer = customer,
                Account = account,
                Type = "OWNER"
            };
            _context.Dispositions.Add(disposition);

    
            await _context.SaveChangesAsync();

            return RedirectToPage("/CustomerPages/Index");
        }
    }
}
