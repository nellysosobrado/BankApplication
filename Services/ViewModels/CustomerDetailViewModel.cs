using Services.ViewModels;
using Microsoft.EntityFrameworkCore;



namespace BankApplication.ViewModels
{

        public class CustomerDetailViewModel
        {
            public string CustomerId { get; set; }
            public string Personnummer { get; set; }
            public string Name { get; set; }
            public string Address { get; set; }
            public string City { get; set; }
            public string PostalCode { get; set; }
            public string Country { get; set; }
            public string Phone { get; set; }
            public string EmailAddress { get; set; }
            public List<DispositionViewModel> Dispositions { get; set; } = new();
        }

        public class DispositionViewModel
        {
            public int DispositionId { get; set; }
            public string Type { get; set; }
            public AccountViewModel Account { get; set; }
        }

    
}