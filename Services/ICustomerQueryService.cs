using BankApplication.ViewModels;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{

    public interface ICustomerQueryService //Read
    {
        IEnumerable<Customer> GetAllCustomers();
        Task<CustomerDetailViewModel> GetCustomerDetailAsync(int id);
        Task<PaginatedList<CustomerViewModel>> SearchAsync(string searchTerm, string sortColumn, string sortOrder, int pageIndex, int pageSize);
        Task<IEnumerable<CustomerViewModel>> GetRecentCustomersAsync(int count);
        Task<CustomerDetailViewModel> GetCustomerByIdAsync(int customerId);
        Task<PaginatedList<CustomerViewModel>> SearchCustomersAsync(
        string searchTerm, string sortColumn, string sortOrder, int pageIndex, int pageSize);

    }

    public interface ICustomerCommandService//Create, Update, Delete
    {
        int CreateCustomer(Customer customer);
        void UpdateCustomer(Customer customer);
        Customer GetCustomer(int id);
        Task<bool> DeleteCustomerAsync(int customerId);



    }

}
