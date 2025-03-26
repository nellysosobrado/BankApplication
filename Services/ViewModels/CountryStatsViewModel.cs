namespace BankApplication.ViewModels  
{
    public class CountryStatsViewModel
    {
        public string CountryCode { get; set; } // "se", "no", "dk", "fi"
        public string CountryName { get; set; } // "Sverige", "Norge", etc.
        public int CustomerCount { get; set; }
        public int AccountCount { get; set; }
        public decimal TotalBalance { get; set; }
        public string Currency { get; set; } // "SEK", "NOK", etc.
    }
}