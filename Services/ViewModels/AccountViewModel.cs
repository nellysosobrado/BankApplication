using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ViewModels
{
    public class AccountViewModel
    {
        public int AccountId { get; set; }
        public string AccountNo { get; set; }
        public decimal Balance { get; set; }
    }
}
