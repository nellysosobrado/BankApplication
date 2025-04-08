using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ViewModels
{
    public class DispositionViewModel
    {
        public int DispositionId { get; set; }
        public string Type { get; set; }
        public AccountViewModel Account { get; set; }
    }
}
