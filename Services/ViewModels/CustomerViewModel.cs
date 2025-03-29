namespace BankApplication.ViewModels
{
    public class CustomerViewModel
    {
        public string CustomerId { get; set; }
        public string Personnummer { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Phone { get; set; } // Tillagt för att matcha servicen
    }
}