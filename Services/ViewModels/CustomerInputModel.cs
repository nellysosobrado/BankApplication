using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ViewModels
{
    public class CustomerInputModel
    {
        public int CustomerId { get; set; }

        [Required, MaxLength(100)]
        public string Givenname { get; set; }

        [Required, MaxLength(100)]
        public string Surname { get; set; }

        [MaxLength(100)]
        public string Gender { get; set; }

        [MaxLength(100)]
        public string Streetaddress { get; set; }

        [MaxLength(10)]
        public string Zipcode { get; set; }

        [MaxLength(2)]
        public string CountryCode { get; set; }

        [MaxLength(100)]
        public string Country { get; set; }

        [DataType(DataType.Date)]
        public DateOnly? Birthday { get; set; }

        [Required, MaxLength(50)]
        public string City { get; set; }

        [EmailAddress, MaxLength(150)]
        public string Emailaddress { get; set; }

        [MaxLength(20)]
        public string NationalId { get; set; }

        [MaxLength(10)]
        public string Telephonecountrycode { get; set; }

        [MaxLength(30)]
        public string Telephonenumber { get; set; }
    }
}
