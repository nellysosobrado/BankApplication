using DAL.Models;
using System.Collections.Generic;

namespace FraudDetectionConsoleApp.Rules
{
    public class LargeTransactionRule : IFraudRule
    {
        public bool IsSuspicious(Transaction transaction, IEnumerable<Transaction> allTransactions)
        {
            return transaction.Amount >= 15000;
        }
    }
}
