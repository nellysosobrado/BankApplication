using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services.Interface;
using System.ComponentModel.DataAnnotations;

namespace BankApplication.Pages.Account
{
    [BindProperties]

    public class DepositModel : PageModel
    {
        private readonly IAccountService _accountService;

        public DepositModel(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [Range(100, 10000)]

        public decimal Amount { get; set; }
        public DateTime DepositDate { get; set; }

        [Required(ErrorMessage = "You forgot to write a comment!")]
        [MinLength(5, ErrorMessage = "Comments must be at least 5 characters long")]
        [MaxLength(250, ErrorMessage = "OK, thats just too many words")]
        public string Comment { get; set; }

        public void OnGet(int accountId)
        {
            DepositDate = DateTime.Now.AddHours(1);
        }
        public IActionResult OnPost(int accountId)
        {
            if (DepositDate < DateTime.Now)
            {
                ModelState.AddModelError(
        "DepositDate", "Cannot Deposit money in the past!");
            }

            if (ModelState.IsValid)
            {
                var accountDb = _accountService.GetAccount(accountId);
                accountDb.Balance += Amount;
                _accountService.Update(accountDb);
                return RedirectToPage("Index");
            }
            return Page();
        }


    }

}
