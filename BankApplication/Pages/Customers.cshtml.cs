using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services;
using BankApplication.ViewModels;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Services.Interface;

namespace BankApplication.Pages
{
    [Authorize]
    public class CustomersModel : PageModel
    {
        private readonly ICustomerQueryService _customerService;
        //private readonly ICustomerService _customerService;

        public CustomersModel(ICustomerQueryService customerService)
        {
            _customerService = customerService;
        }

        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SortColumn { get; set; } = "Name"; // Default sort column

        [BindProperty(SupportsGet = true)]
        public string SortOrder { get; set; } = "asc"; // Default sort order

        [BindProperty(SupportsGet = true)]
        public int PageSize { get; set; } = 50; // Default page size

        public PaginatedList<CustomerViewModel> Customers { get; set; }

        public async Task OnGetAsync(int pageIndex = 1)
        {


            Customers = await _customerService.SearchCustomersAsync(
                searchTerm: SearchTerm,
                sortColumn: SortColumn,
                sortOrder: SortOrder,
                pageIndex: pageIndex,
                pageSize: PageSize);
        }
    }
}