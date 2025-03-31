using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DTOs
{

        public class CustomerDto
        {
            public int CustomerId { get; set; }
            public string NationalId { get; set; }
            public string Givenname { get; set; }
            public string Surname { get; set; }
            public string Streetaddress { get; set; }
            public string City { get; set; }
            public string Zipcode { get; set; }
            public string Country { get; set; }
            public string Telephonenumber { get; set; }
            public string Emailaddress { get; set; }
        }

        public class CustomerListDto
        {
            public int CustomerId { get; set; }
            public string NationalId { get; set; }
            public string FullName { get; set; }
            public string Streetaddress { get; set; }
            public string City { get; set; }
            public string Telephonenumber { get; set; }
        }

        public class PaginatedCustomerResult
        {
            public List<CustomerListDto> Customers { get; set; }
            public int TotalCount { get; set; }
            public int PageIndex { get; set; }
            public int PageSize { get; set; }
            public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
        }
    
}
