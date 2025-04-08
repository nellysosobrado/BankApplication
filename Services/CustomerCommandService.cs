using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public void UpdateCustomer(Customer customer)
        {
            customer.LastModified = DateTime.UtcNow;
            _context.SaveChanges();
        }

        public Customer GetCustomer(int id)
        {
            return _context.Customers.FirstOrDefault(c => c.CustomerId == id);
        }
    }

}
