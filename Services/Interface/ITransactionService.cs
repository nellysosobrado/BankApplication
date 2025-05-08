using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;
using DAL.Models;

namespace Services.Interface
{
    public interface ITransactionService
    {
        void AddTransaction(Transaction transaction);
    }
}
