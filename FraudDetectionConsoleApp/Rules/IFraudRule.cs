using DAL.Models;
using System.Collections.Generic;

namespace FraudDetectionConsoleApp.Rules
{
    public interface IFraudRule
    {
        bool IsSuspicious(Transaction transaction, IEnumerable<Transaction> allTransactions);
    }
}
