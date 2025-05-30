using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services.Interface;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace BankApplication.Pages.AccountPages;

[Authorize(Roles = "Cashier,Admin")]
[BindProperties]
public class DepositModel : PageModel
{
    private readonly IAccountService _accountService;
    private readonly ITransactionService _transactionService;

    public DepositModel(IAccountService accountService, ITransactionService transactionService)
    {
        _accountService = accountService;
        _transactionService = transactionService;
    }

    [Range(typeof(decimal), "1500", "1000000", ErrorMessage = "Amount must be between 1500 and 1,000,000")]
    public decimal? Amount { get; set; }


    [Required]
    [DataType(DataType.DateTime)]
    public DateTime DepositDate { get; set; } = DateTime.Now.AddHours(1);

    [Required(ErrorMessage = "You forgot to write a comment!")]
    [MinLength(5)]
    [MaxLength(250)]
    public string Comment { get; set; }

    [BindProperty(SupportsGet = true)]
    public int CustomerId { get; set; }

    [BindProperty(SupportsGet = true)]
    public int AccountId { get; set; }

    [TempData]
    public string? TempInputJson { get; set; }

    [TempData]
    public string? TempModelStateJson { get; set; }

    [TempData]
    public string? SuccessMessage { get; set; }

    public void OnGet()
    {
        Response.Headers["Cache-Control"] = "no-store, no-cache, must-revalidate, max-age=0";
        Response.Headers["Pragma"] = "no-cache";
        Response.Headers["Expires"] = "0";

        if (!string.IsNullOrEmpty(TempInputJson))
        {
            var restored = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(TempInputJson);
            if (restored != null)
            {
                if (restored.TryGetValue("Amount", out var amountElement) &&
                    amountElement.ValueKind != JsonValueKind.Null &&
                    amountElement.TryGetDecimal(out var amount))
                {
                    Amount = amount;
                }

                if (restored.TryGetValue("DepositDate", out var dateElement) &&
                    dateElement.ValueKind != JsonValueKind.Null &&
                    dateElement.TryGetDateTime(out var date))
                {
                    DepositDate = date;
                }

                if (restored.TryGetValue("Comment", out var commentElement) &&
                    commentElement.ValueKind != JsonValueKind.Null)
                {
                    Comment = commentElement.GetString()!;
                }
            }
        }

        if (!string.IsNullOrEmpty(TempModelStateJson))
        {
            var errors = JsonSerializer.Deserialize<Dictionary<string, string[]>>(TempModelStateJson);
            if (errors != null)
            {
                foreach (var kvp in errors)
                {
                    foreach (var error in kvp.Value)
                    {
                        ModelState.AddModelError(kvp.Key, error);
                    }
                }
            }
        }
    }


    public IActionResult OnPost()
    {
        if (DepositDate < DateTime.Now)
        {
            ModelState.AddModelError("DepositDate", "Cannot deposit money in the past!");
        }

        if (!ModelState.IsValid)
        {
            TempInputJson = JsonSerializer.Serialize(new
            {
                Amount,
                DepositDate,
                Comment
            });

            TempModelStateJson = JsonSerializer.Serialize(
                ModelState.Where(e => e.Value?.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                    )
            );

            return RedirectToPage(new { CustomerId = CustomerId, AccountId = AccountId });
        }

        var accountDb = _accountService.GetAccount(AccountId);
        if (accountDb == null)
        {
            ModelState.AddModelError("", "Account not found.");
            return Page();
        }

        var newBalance = accountDb.Balance + Amount!.Value;
        accountDb.Balance = newBalance;
        _accountService.Update(accountDb);

        var transaction = new DAL.Models.Transaction
        {
            AccountId = accountDb.AccountId,
            Amount = Amount.Value,
            Balance = newBalance,
            Date = DepositDate,
            Type = "Credit",
            Operation = "Deposit",
        };
        _transactionService.AddTransaction(transaction);

        SuccessMessage = $"Successfully deposited {Amount.Value} kr to account {AccountId}.";

        return RedirectToPage(new { CustomerId = CustomerId, AccountId = AccountId });
    }
}
