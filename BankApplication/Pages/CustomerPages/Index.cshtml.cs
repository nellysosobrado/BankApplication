using BankApplication.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services;

namespace BankApplication.Pages.CustomerPages;

[Authorize(Roles = "Cashier")]
public class IndexModel : PageModel
{
    private readonly ICustomerQueryService _customerService;

    public IndexModel(ICustomerQueryService customerService)
    {
        _customerService = customerService;
    }

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

    public PaginatedList<CustomerViewModel> Customers { get; set; }

    public async Task OnGetAsync()
    {
        Customers = await _customerService.SearchCustomersAsync(
            searchTerm: SearchTerm,
            sortColumn: SortColumn,
            sortOrder: SortOrder,
            pageIndex: PageIndex,
            pageSize: PageSize);
    }
}
