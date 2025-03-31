using Services.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface ICustomerService
    {
        Task<PaginatedCustomerResult> SearchCustomersAsync(
            string searchTerm,
            string sortColumn,
            string sortOrder,
            int pageIndex = 1,
            int pageSize = 50);

        Task<CustomerDto> GetCustomerByIdAsync(int id);
        Task<IEnumerable<CustomerListDto>> GetRecentCustomersAsync(int count = 5);
    }
}
