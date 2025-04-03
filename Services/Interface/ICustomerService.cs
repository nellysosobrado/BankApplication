using BankApplication.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface ICustomerService
    {
        Task<PaginatedList<CustomerViewModel>> SearchCustomersAsync(
            string searchTerm,
            string sortColumn,
            string sortOrder,
            int pageIndex = 1,
            int pageSize = 50);
        Task<CustomerDetailViewModel> GetCustomerByIdAsync(int id);
        Task<IEnumerable<CustomerViewModel>> GetRecentCustomersAsync(int count = 5);
    }
}
