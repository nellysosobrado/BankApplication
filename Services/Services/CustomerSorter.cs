using DAL.Models;
using Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services
{
    public class CustomerSorter : ICustomerSorter
    {
        public IQueryable<Customer> ApplySorting(IQueryable<Customer> query, string column, string order)
        {
            bool asc = order?.ToLower() == "asc";

            return column switch
            {
                "CustomerId" => asc ? query.OrderBy(c => c.CustomerId) : query.OrderByDescending(c => c.CustomerId),
                "Personnummer" => asc ? query.OrderBy(c => c.NationalId) : query.OrderByDescending(c => c.NationalId),
                "Name" => asc
                    ? query.OrderBy(c => c.Givenname).ThenBy(c => c.Surname)
                    : query.OrderByDescending(c => c.Givenname).ThenByDescending(c => c.Surname),
                "Address" => asc ? query.OrderBy(c => c.Streetaddress) : query.OrderByDescending(c => c.Streetaddress),
                "City" => asc ? query.OrderBy(c => c.City) : query.OrderByDescending(c => c.City),
                _ => query.OrderBy(c => c.Surname)
            };
        }

    }
}
