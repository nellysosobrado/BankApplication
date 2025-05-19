using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ViewModels;

public class CustomerCreateViewModel
{
    [Required]
    public string Givenname { get; set; }

    [Required]
    public string Surname { get; set; }

    [Required]
    public string Gender { get; set; }

    [EmailAddress]
    public string? Emailaddress { get; set; }

    [Required]
    public string Streetaddress { get; set; }

    [Required]
    public string Zipcode { get; set; }

    [Required]
    public string City { get; set; }

    [Required]
    [MaxLength(2)]
    public string CountryCode { get; set; }

    [Required]
    public string Country { get; set; }

    [DataType(DataType.Date)]
    public DateOnly? Birthday { get; set; }

 
    [StringLength(20)]
    public string? NationalId { get; set; }

    [StringLength(5)]
    public string? Telephonecountrycode { get; set; }

    [Phone]
    [StringLength(20)]
    public string? Telephonenumber { get; set; }
}

