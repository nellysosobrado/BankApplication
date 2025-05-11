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

    

}
