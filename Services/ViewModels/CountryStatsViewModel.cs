namespace BankApplication.ViewModels  
{
    public class CountryStatsViewModel
    {
        public string CountryCode { get; set; } 
        public string CountryName { get; set; }
        public int CustomerCount { get; set; }
        public int AccountCount { get; set; }
        public decimal TotalBalance { get; set; }
        public string Currency { get; set; }
    }
}