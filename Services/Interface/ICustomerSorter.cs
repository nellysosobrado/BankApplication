using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface ICustomerSorter
    {
        IQueryable<Customer> ApplySorting(IQueryable<Customer> query, string sortColumn, string sortOrder);
    }

    public class CustomerSorter : ICustomerSorter
    {
        public IQueryable<Customer> ApplySorting(IQueryable<Customer> query, string column, string order)
        {
            bool asc = order?.ToLower() == "asc";

            return column switch
            {
                "CustomerId" => asc ? query.OrderBy(c => c.CustomerId) : query.OrderByDescending(c => c.CustomerId),
                "Personnummer" => asc ? query.OrderBy(c => c.NationalId) : query.OrderByDescending(c => c.NationalId),
                "Name" => asc ? query.OrderBy(c => c.Surname) : query.OrderByDescending(c => c.Surname),
                "Address" => asc ? query.OrderBy(c => c.Streetaddress) : query.OrderByDescending(c => c.Streetaddress),
                "City" => asc ? query.OrderBy(c => c.City) : query.OrderByDescending(c => c.City),
                _ => query.OrderBy(c => c.Surname)
            };
        }
    }

}
