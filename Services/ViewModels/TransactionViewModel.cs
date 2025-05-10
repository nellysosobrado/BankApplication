namespace Services.ViewModels
{
    public class TransactionViewModel
    {
        public int TransactionId { get; set; }
        public DateTime Date { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Operation { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public decimal Balance { get; set; }

        // Ny egenskap: formaterat belopp med +/- 
        public string DisplayAmount =>
            Type == "Debit" ? "-" + Amount.ToString("C") : Amount.ToString("C");

        // Ny egenskap: CSS-klass för att färga rött vid negativt
        public string CssClass =>
            Type == "Debit" ? "text-danger" : string.Empty;
    }
}
