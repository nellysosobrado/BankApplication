using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Services.ViewModels
{
    public class CustomerInputModel
    {
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "First name is required")]
        [MaxLength(100, ErrorMessage = "First name cannot exceed 100 characters")]
        public string Givenname { get; set; }

        [Required(ErrorMessage = "Surname  is required")]
        [MaxLength(100, ErrorMessage = "Surname cannot exceed 100 characters")]
        public string Surname { get; set; }

        [Required(ErrorMessage = "Gender is required")]
        [MaxLength(100)]
        public string Gender { get; set; }

        [Required(ErrorMessage = "Street address is required")]
        [MaxLength(100)]
        public string Streetaddress { get; set; }

        [Required(ErrorMessage = "Zip code is required")]
        [MaxLength(10)]
        public string Zipcode { get; set; }

        [Required(ErrorMessage = "Country code is required")]
        [MaxLength(2)]
        public string CountryCode { get; set; }

        [Required(ErrorMessage = "Country is required")]
        [MaxLength(100)]
        public string Country { get; set; }

        [Required(ErrorMessage = "Birthday is required")]
        [DataType(DataType.Date, ErrorMessage = "Invalid date format")]
        public DateOnly? Birthday { get; set; }

        [Required(ErrorMessage = "City is required")]
        [MaxLength(50)]
        public string City { get; set; }

        [Required(ErrorMessage = "Email address is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [MaxLength(150)]
        public string Emailaddress { get; set; }

        [Required(ErrorMessage = "National ID is required")]
        [RegularExpression(@"^\d+$", ErrorMessage = "National ID must contain only digits")]
        [MaxLength(20, ErrorMessage = "National ID cannot exceed 20 digits")]
        public string NationalId { get; set; }

        [Required(ErrorMessage = "Telephone country code is required")]
        [MaxLength(10)]
        public string Telephonecountrycode { get; set; }

        [Required(ErrorMessage = "Telephone number is required")]
        [MaxLength(30)]
        public string Telephonenumber { get; set; }
    }
}
