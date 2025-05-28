using DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Services
{
    public class CustomerCommandService : ICustomerCommandService
    {
        private readonly BankAppDataContext _context;

        public CustomerCommandService(BankAppDataContext context)
        {
            _context = context;
        }

        public int CreateCustomer(Customer customer)
        {
            customer.Registered = DateTime.UtcNow;
            customer.LastModified = DateTime.UtcNow;
            _context.Customers.Add(customer);
            _context.SaveChanges();
            return customer.CustomerId;
        }

        public async Task<bool> DeleteCustomerAsync(int customerId)
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.CustomerId == customerId);

            if (customer == null)
                return false;

            customer.IsDeleted = true;
            customer.LastModified = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public void UpdateCustomer(Customer customer)
        {
            customer.LastModified = DateTime.UtcNow;
            _context.SaveChanges();
        }

        public Customer GetCustomer(int id)
        {
            return _context.Customers.FirstOrDefault(c => c.CustomerId == id && !c.IsDeleted);
        }
    }
}
