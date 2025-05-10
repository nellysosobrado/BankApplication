using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DAL.Models;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Account = DAL.Models.Account;
using BankApplication.ViewModels;
using Services.ViewModels;



namespace BankApplication.Pages.Customer;

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

        var newCustomerId = await _context.Customers.MaxAsync(c => (int?)c.CustomerId) ?? 0;
        newCustomerId += 1;

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

        return RedirectToPage("/Customer/Index");
    }
}
