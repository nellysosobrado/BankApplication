using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ViewModels
{
    public class CustomerSummaryViewModel
    {
        public string FullName { get; set; } = "";
        public int AccountCount { get; set; }
        public decimal TotalBalance { get; set; }
    }
}
